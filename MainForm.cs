using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NBagOfTricks;
using NBagOfTricks.Slog;
using NBagOfUis;
using AudioLib;


namespace Wavicler
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>The actual player.</summary>
        readonly AudioPlayer _player;

        /// <summary>Dynamically connect input providers to the player.</summary>
        readonly SwappableSampleProvider _waveOutSwapper = new();

        /// <summary>Where we be.</summary>
        AppState _currentState = AppState.Stop;

        /// <summary>The settings.</summary>
        readonly UserSettings _settings;

        /// <summary>UI indicator.</summary>
        const string DIRTY_FILE_IND = "*";
        #endregion

        #region Types
        /// <summary>What are we doing.</summary>
        public enum AppState { Stop, Play, Rewind, Complete, Dead }
        #endregion

        #region Lifecycle
        public MainForm()
        {
            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            _settings = (UserSettings)SettingsCore.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            AudioSettings.LibSettings = _settings.AudioSettings;

            InitializeComponent();

            // Init logging.
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogEvent += (object? sender, LogEventArgs e) => { this.InvokeIfRequired(_ => { tvLog.AppendLine($"{e.Message}"); }); };
            LogManager.Run(Path.Join(appDir, "log.txt"), 100000);

            // Log display.
            tvLog.Font = Font;
            tvLog.MatchColors.Add("ERR", Color.LightPink);
            tvLog.MatchColors.Add("WRN", Color.Plum);

            // Init main form from settings.
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(_settings.FormGeometry.X, _settings.FormGeometry.Y);
            Size = new Size(_settings.FormGeometry.Width, _settings.FormGeometry.Height);
            KeyPreview = true; // for routing kbd strokes through OnKeyDown first.

            // Create output.
            _player = new(_settings.AudioSettings.WavOutDevice, int.Parse(_settings.AudioSettings.Latency), _waveOutSwapper);
            _player.PlaybackStopped += Player_PlaybackStopped;

            // Other UI items.
            ToolStrip.Renderer = new NBagOfUis.CheckBoxRenderer() { SelectedColor = _settings.ControlColor };

            btnAutoplay.Checked = _settings.Autoplay;
            btnAutoplay.Click += (_, __) => _settings.Autoplay = btnAutoplay.Checked;

            btnLoop.Checked = _settings.Loop;
            btnLoop.Click += (_, __) => _settings.Loop = btnLoop.Checked;

            sldVolume.DrawColor = _settings.ControlColor;
            sldVolume.Value = _settings.Volume;
            sldVolume.ValueChanged += (_, __) => _player.Volume = (float)sldVolume.Value;

            Globals.BPM = _settings.BPM;
            txtBPM.Text = Globals.BPM.ToString();
            txtBPM.KeyPress += (object? sender, KeyPressEventArgs e) => KeyUtils.TestForNumber_KeyPress(sender!, e);
            txtBPM.LostFocus += (_, __) => Globals.BPM = double.Parse(txtBPM.Text);

            ftree.Settings = _settings.FilTreeSettings;
            ftree.RecentFiles = _settings.RecentFiles;
            ftree.Init();

            cmbSelMode.Items.Add(WaveSelectionMode.Time);
            cmbSelMode.Items.Add(WaveSelectionMode.Bar);
            cmbSelMode.Items.Add(WaveSelectionMode.Sample);
            cmbSelMode.SelectedIndexChanged += (_, __) =>
            {
                _settings.SelectionMode = (WaveSelectionMode)cmbSelMode.SelectedItem;
                switch (_settings.SelectionMode)
                {
                    case WaveSelectionMode.Time: Globals.ConverterOps = new TimeOps(); break;
                    case WaveSelectionMode.Bar: Globals.ConverterOps = new BarOps(); break;
                    case WaveSelectionMode.Sample: Globals.ConverterOps = new SampleOps(); break;
                }
                ActiveClipEditor()?.Invalidate();
            };
            cmbSelMode.SelectedItem = _settings.SelectionMode;

            btnRewind.Click += (_, __) => UpdateState(AppState.Rewind);
            btnPlay.Click += (_, __) => UpdateState(btnPlay.Checked ? AppState.Play : AppState.Stop);

            // File handling.
            OpenMenuItem.Click += (_, __) => Open_Click();
            SaveAsMenuItem.Click += (_, __) => SaveClipAs(ActiveClipEditor());
            CloseMenuItem.Click += (_, __) => Close(false);
            CloseAllMenuItem.Click += (_, __) => Close(true);
            ExitMenuItem.Click += (_, __) => Close(true);
            MenuStrip.MenuActivate += (_, __) => UpdateUi();
            FileMenuItem.DropDownOpening += File_DropDownOpening;

            // Tools.
            AboutMenuItem.Click += (_, __) => MiscUtils.ShowReadme("Wavicler");
            SettingsMenuItem.Click += (_, __) => EditSettings();
            ResampleMenuItem.Click += (_, __) => Resample();
            SplitStereoMenuItem.Click += (_, __) => SplitStereo();
            ToMonoMenuItem.Click += (_, __) => ToMono();

            // Tab control.
            TabControl.TabPages.Clear();
            TabControl.MouseDoubleClick += TabControl_MouseDoubleClick;

            UpdateUi();

            Text = $"Wavicler {MiscUtils.GetVersionString()}";
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // Initialize tree from user settings.
            InitNavigator();

            if (!_player.Valid)
            {
                var s = $"Something wrong with your audio output device:{_settings.AudioSettings.WavOutDevice}";
                _logger.Error(s);
                UpdateState(AppState.Dead);
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Bye-bye.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Close(true);
            LogManager.Stop();
            UpdateState(AppState.Stop);
            SaveSettings();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            // My stuff here.
            _player.Run(false);
            _player.Dispose();

            base.Dispose(disposing);
        }
        #endregion

        #region State management
        /// <summary>
        /// General state management. Everything goes through here.
        /// </summary>
        void UpdateState(AppState newState)
        {
            // Unhook temporarily.
            btnPlay.CheckedChanged -= ChkPlay_CheckedChanged;

            if (newState != _currentState)
            {
                _logger.Info($"State change:{newState}");
            }

            switch (newState)
            {
                case AppState.Complete:
                    Rewind();
                    if (_settings.Loop)
                    {
                        Play();
                    }
                    else
                    {
                        Stop();
                    }
                    break;

                case AppState.Play:
                    Play();
                    break;

                case AppState.Stop:
                    Stop();
                    break;

                case AppState.Rewind:
                    Rewind();
                    break;

                case AppState.Dead:
                    Stop();
                    break;
            }

            _currentState = newState;

            // Rehook.
            btnPlay.CheckedChanged += ChkPlay_CheckedChanged;

            // Local funcs
            void Play()
            {
                btnPlay.Checked = true;
                _player.Run(true);
                if(!_player.Playing)
                {
                    _logger.Error($"Player won't run");
                    btnPlay.Checked = false;
                }
            }

            void Stop()
            {
                btnPlay.Checked = false;
                _player.Run(false);
            }

            void Rewind()
            {
                var cled = ActiveClipEditor();
                cled?.Rewind();
            }
        }

        /// <summary>
        /// Play button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChkPlay_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateState(btnPlay.Checked ? AppState.Play : AppState.Stop);
        }

        /// <summary>
        /// Usually end of file but could be error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Player_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            if (e.Exception is not null)
            {
                _logger.Exception(e.Exception, "Other NAudio error");
                UpdateState(AppState.Dead);
            }
            else
            {
                UpdateState(AppState.Complete);
            }
        }

        /// <summary>
        /// Do some global key handling. Space bar is used for stop/start playing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    // Toggle.
                    UpdateState(btnPlay.Checked ? AppState.Stop : AppState.Play);
                    e.Handled = true;
                    break;
            }

            base.OnKeyDown(e);
        }
        #endregion

        #region Menu management
        /// <summary>
        /// Show the recent files in the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void File_DropDownOpening(object? sender, EventArgs e)
        {
            var vv = FileMenuItem.DropDownItems;

            RecentMenuItem.DropDownItems.Clear();

            _settings.RecentFiles.ForEach(f =>
            {
                ToolStripMenuItem menuItem = new(f);
                menuItem.Click += (object? sender, EventArgs e) =>
                {
                    string fn = sender!.ToString()!;
                    OpenFile(fn);
                };

                RecentMenuItem.DropDownItems.Add(menuItem);
            });
        }

        /// <summary>
        /// Set UI item enables according to system states.
        /// </summary>
        void UpdateUi()
        {
            bool anyOpen = false;// TabControl.TabPages.Count > 0;
            bool anyDirty = false;
            bool currentDirty = false;

            var page = ActivePage();
            var cled = ActiveClipEditor();

            if (page is not null)
            {
                currentDirty = page.Text.Contains(DIRTY_FILE_IND);
            }

            foreach (TabPage tab in TabControl.TabPages)
            {
                anyOpen = true;
                anyDirty |= tab.Text.Contains(DIRTY_FILE_IND);
            }

            btnRewind.Enabled = anyOpen;
            btnPlay.Enabled = anyOpen;

            OpenMenuItem.Enabled = true;
            SaveAsMenuItem.Enabled = currentDirty;
            CloseMenuItem.Enabled = anyOpen;
            CloseAllMenuItem.Enabled = anyOpen;
            ExitMenuItem.Enabled = true;

            AboutMenuItem.Enabled = true;
            SettingsMenuItem.Enabled = true;

            ResampleMenuItem.Enabled = true;
            SplitStereoMenuItem.Enabled = true;
            ToMonoMenuItem.Enabled = true;
        }
        #endregion

        #region File I/O
        /// <summary>
        /// Common file opener.
        /// </summary>
        /// <param name="fn">The file to open.</param>
        /// <returns>Success.</returns>
        bool OpenFile(string fn)
        {
            bool ok = true;

            UpdateState(AppState.Stop);

            try
            {
                // Do validity checks.
                if (!File.Exists(fn))
                {
                    throw new InvalidOperationException($"Invalid file.");
                }

                var ext = Path.GetExtension(fn).ToLower();
                var baseFn = Path.GetFileName(fn);

                if (!AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
                {
                    throw new InvalidOperationException($"Invalid file type.");
                }

                // Valid file name.
                _logger.Info($"Opening file: {fn}");

                // Find out about the requested file.
                using var reader = new AudioFileReader(fn);

                // Check sample rate etc.
                reader.Validate(false);

                // Create a tab page or select if it already exists.
                var coerce = StereoCoercion.None;

                if (reader.WaveFormat.Channels == 2)
                {
                    if (!_settings.AutoConvert)
                    {
                        // Ask user what to do with a stereo file.
                        MultipleChoiceSelector selector = new() { Text = "Convert stereo" };
                        selector.SetOptions(new() { "Left", "Right", "Mono" });
                        var dlgres = selector.ShowDialog();
                        if (dlgres == DialogResult.OK)
                        {
                            coerce = selector.SelectedOption switch
                            {
                                "Left" => StereoCoercion.Left,
                                "Right" => StereoCoercion.Right,
                                "Mono" => StereoCoercion.Mono,
                                _ => StereoCoercion.None,
                            };
                        }
                        else
                        {
                            // Bail out.
                            ok = false;
                        }
                    }
                    else
                    {
                        coerce = StereoCoercion.Mono;
                        _logger.Info($"Autoconvert {baseFn} to mono");
                    }
                }
                else // mono
                {
                    coerce = StereoCoercion.None;
                }

                if(ok)
                {
                    var tmod = coerce switch
                    {
                        StereoCoercion.Right => " RIGHT",
                        StereoCoercion.Left => " LEFT",
                        StereoCoercion.Mono => " MONO",
                        _ => "",
                    };

                    // Is this already open?
                    var tabName = baseFn + tmod;
                    TabPage? seltab = null;
                    foreach (TabPage tab in TabControl.TabPages)
                    {
                        if (tab.Text == tabName)
                        {
                            seltab = tab;
                            TabControl.SelectedTab = seltab;
                            break;
                        }
                    }

                    if (seltab is null) // new tab
                    {
                        using (new WaitCursor())
                        {
                            var prov = new ClipSampleProvider(fn, coerce);
                            CreateTab(prov, tabName, fn);
                        }
                    }
                }

                if (ok)
                {
                    _settings.RecentFiles.UpdateMru(fn);

                    if (_settings.Autoplay)
                    {
                        UpdateState(AppState.Rewind);
                        UpdateState(AppState.Play);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Couldn't open the file: {fn} because: {ex.Message}");
                ok = false;
            }

            UpdateUi();

            return ok;
        }

        /// <summary>
        /// Allows the user to select from file system.
        /// </summary>
        void Open_Click()
        {
            var fn = GetUserFilename();
            if (fn != "")
            {
                OpenFile(fn);
            }
        }

        /// <summary>
        /// Save the page/clip to file.
        /// </summary>
        /// <param name="cled">Data source.</param>
        /// <param name="fn">The file to save to.</param>
        /// <returns>Status.</returns>
        bool SaveClip(ClipEditor? cled, string fn)
        {
            bool ok = false;

            if (cled is not null)
            {
                try
                {
                    WaveFileWriter.CreateWaveFile(fn, cled.SampleProvider.ToWaveProvider());
                }
                catch (Exception e)
                {
                    _logger.Error($"Couldn't save {fn}: {e.Message}");
                }
            }

            UpdateUi();

            return ok;
        }

        /// <summary>
        /// Save the page/clip to file.
        /// </summary>
        /// <param name="cled">Data source.</param>
        void SaveClipAs(ClipEditor? cled)
        {
            if (cled is not null)
            {
                using SaveFileDialog saveDlg = new()
                {
                    Filter = $"Audio Files|{AudioLibDefs.AUDIO_FILE_TYPES}",
                    Title = "Save as file"
                };

                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    SaveClip(cled, saveDlg.FileName);
                }
            }

            UpdateUi();
        }

        /// <summary>
        /// General closer/saver.
        /// </summary>
        /// <param name="all">Close all if true otherwise just the current selected.</param>
        void Close(bool all)
        {
            if (all)
            {
                while (TabControl.TabCount > 0)
                {
                    CloseTab(TabControl.TabPages[0]);
                }
            }
            else if (TabControl.TabCount > 0)
            {
                CloseTab(TabControl.SelectedTab);
            }

            // Local function.
            void CloseTab(TabPage page)
            {
                bool closeIt = true;

                if (page.Text.Contains(DIRTY_FILE_IND))
                {
                    // Ask to save.
                    var res = MessageBox.Show($"Clip {page.Text.Replace(DIRTY_FILE_IND, "")} has unsaved changes. Do you want to save it?",
                        "Close Clip", MessageBoxButtons.YesNoCancel);

                    switch(res)
                    {
                        case DialogResult.Yes:
                            SaveClipAs(ActiveClipEditor());
                            break;

                        case DialogResult.No:
                            // Don't save file.
                            break;

                        case DialogResult.Cancel:
                            closeIt = false;
                            break;
                    }
                }

                if (closeIt)
                {
                    ActiveClipEditor()?.Dispose();
                    TabControl.TabPages.Remove(page);
                    page.Dispose();
                }
            }

            UpdateUi();
        }
        #endregion

        #region Navigator
        /// <summary>
        /// Initialize tree from user settings.
        /// </summary>
        void InitNavigator()
        {
            var s = AudioLibDefs.AUDIO_FILE_TYPES;
            ftree.Settings.FilterExts = s.SplitByTokens("|;*");

            try
            {
                ftree.Init();
            }
            catch (DirectoryNotFoundException)
            {
                _logger.Warn("No tree directories");
            }
        }

        /// <summary>
        /// Tree has selected a file to play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fn"></param>
        void Navigator_FileSelectedEvent(object? sender, string fn)
        {
            OpenFile(fn);
        }
        #endregion

        #region Tab management
        /// <summary>
        /// Function to open a new tab.
        /// </summary>
        /// <param name="prov"></param>
        /// <param name="fn"></param>
        /// <param name="tabName"></param>
        void CreateTab(ClipSampleProvider prov, string fn, string tabName)
        {
            ClipEditor cled = new(prov, fn) { Dock = DockStyle.Fill };
            cled.ServiceRequestEvent += ClipEditor_ServiceRequest;
            _waveOutSwapper.SetInput(cled.SampleProvider);
            statusInfo.Text = cled.SampleProvider.GetInfoString();

            TabPage page = new() { Text = tabName };
            page.Controls.Add(cled);
            TabControl.TabPages.Add(page);
            TabControl.SelectedTab = page;
        }

        /// <summary>
        /// User changes tab of interest.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateState(AppState.Stop);

            var cled = ActiveClipEditor();
            if (cled is not null)
            {
                _waveOutSwapper.SetInput(cled.SampleProvider);
                statusInfo.Text = cled.SampleProvider.GetInfoString();
            }
            else
            {
                _waveOutSwapper.SetInput(null);
                statusInfo.Text = "";
            }
        }

        /// <summary>
        /// User wants to close this tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabControl_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < TabControl.TabCount; ++i)
            {
                if (TabControl.GetTabRect(i).Contains(e.Location))
                {
                    // Found it.
                    Close(false);
                    break;
                }
            }
        }

        /// <summary>
        /// Clip editor wants main to do something for it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClipEditor_ServiceRequest(object? sender, ClipEditor.ServiceRequestEventArgs e)
        {
            var cled = sender as ClipEditor;

            switch (e.Request)
            {
                case ClipEditor.ServiceRequest.Close:
                    Close(false);
                    break;

                case ClipEditor.ServiceRequest.CopySelectionToNewClip:
                    ClipSampleProvider clnew = new(cled!.SampleProvider, StereoCoercion.None);
                    CreateTab(clnew, DIRTY_FILE_IND, DIRTY_FILE_IND);
                    break;
            }
        }
        #endregion

        #region Settings
        /// <summary>
        /// Edit user settings.
        /// </summary>
        void EditSettings()
        {
            var changes = SettingsEditor.Edit(_settings, "User Settings", 500);

            // Detect changes of interest.
            bool navChange = false;
            bool restart = false;

            foreach (var (name, cat) in changes)
            {
                switch(name)
                {
                    case "WavOutDevice":
                    case "Latency":
                    case "ControlColor":
                    case "WaveColor":
                    case "FileLogLevel":
                    case "NotifLogLevel":
                        restart = true;
                        break;

                    case "RootDirs":
                        navChange = true;
                        break;
                }
            }

            if (restart)
            {
                MessageBox.Show("Restart required for device changes to take effect");
            }

            if (navChange)
            {
                InitNavigator();
            }

            SaveSettings();
        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            _settings.BPM = Globals.BPM;
            _settings.Save();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// 
        /// </summary>
        void Resample()
        {
            var fn = GetUserFilename();
            if (fn != "")
            {
                var ok = NAudioEx.Convert(Conversion.Resample, fn);
                if (!ok)
                {
                    _logger.Warn($"{fn} is already 44.1k");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void SplitStereo()
        {
            var fn = GetUserFilename();
            if (fn != "")
            {
                var ok = NAudioEx.Convert(Conversion.SplitStereo, fn);
                if (!ok)
                {
                    _logger.Warn($"{fn} is a mono file");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ToMono()
        {
            var fn = GetUserFilename();
            if (fn != "")
            {
                var ok = NAudioEx.Convert(Conversion.ToMonoWav, fn);
                if (!ok)
                {
                    _logger.Warn($"{fn} is already 44.1k");
                }
            }
        }

        /// <summary>
        /// Utility to get filename from the user.
        /// </summary>
        /// <returns>Filename or empty if cancelled.</returns>
        string GetUserFilename()
        {
            string fn = "";

            using OpenFileDialog openDlg = new()
            {
                // Output is only wav.
                Filter = $"Audio Files|*.wav",
                Title = "Select a wav file"
            };

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                fn = openDlg.FileName;
            }

            return fn;           
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Helper function.
        /// </summary>
        /// <returns></returns>
        TabPage? ActivePage()
        {
            TabPage? page = TabControl.TabPages.Count > 0 ?
                TabControl.SelectedTab :
                null;
            return page;
        }

        /// <summary>
        /// Helper function.
        /// </summary>
        /// <returns></returns>
        ClipEditor? ActiveClipEditor()
        {
            ClipEditor? cled = 
                TabControl.TabPages.Count > 0 && TabControl.SelectedTab.Controls.Count > 0 ?
                TabControl.SelectedTab.Controls[0] as ClipEditor :
                null;
            return cled;
        }
        #endregion
    }
}
