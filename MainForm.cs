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
using AudioLib; // TODO restore dll ref.


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
        const string NEW_FILE_IND = "*";
        #endregion

        #region Lifecycle
        public MainForm()
        {
            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            _settings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
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

            Globals.BPM = (float)_settings.DefaultBPM;
            txtBPM.Text = Globals.BPM.ToString();
            txtBPM.KeyPress += (object? sender, KeyPressEventArgs e) => KeyUtils.TestForNumber_KeyPress(sender!, e);
            txtBPM.LostFocus += (_, __) => Globals.BPM = float.Parse(txtBPM.Text); 

            cmbSelMode.Items.Add(WaveSelectionMode.Time);
            cmbSelMode.Items.Add(WaveSelectionMode.Bar);
            cmbSelMode.Items.Add(WaveSelectionMode.Sample);
            cmbSelMode.SelectedIndexChanged += (_, __) =>
            {
                switch(cmbSelMode.SelectedItem)
                {
                    case WaveSelectionMode.Time: Globals.ConverterOps = new TimeOps(); break;
                    case WaveSelectionMode.Bar: Globals.ConverterOps = new BarOps(); break;
                    case WaveSelectionMode.Sample: Globals.ConverterOps = new SampleOps(); break;
                }
                ActiveClipEditor()?.Invalidate();
            };
            cmbSelMode.SelectedItem = _settings.DefaultSelectionMode;

            btnRewind.Click += (_, __) => UpdateState(AppState.Rewind);
            btnPlay.Click += (_, __) => UpdateState(btnPlay.Checked ? AppState.Play : AppState.Stop);

            // File handling.
            //NewMenuItem.Click += (_, __) => OpenFile();
            OpenMenuItem.Click += (_, __) => Open_Click();
            //SaveMenuItem.Click += (_, __) => SaveFile(ActiveClipEditor());
            SaveAsMenuItem.Click += (_, __) => SaveFileAs(ActiveClipEditor());
            CloseMenuItem.Click += (_, __) => Close(false);
            CloseAllMenuItem.Click += (_, __) => Close(true);
            ExitMenuItem.Click += (_, __) => Close(true);
            MenuStrip.MenuActivate += (_, __) => UpdateMenu();
            FileMenuItem.DropDownOpening += File_DropDownOpening;

            // Tools.
            AboutMenuItem.Click += (_, __) => MiscUtils.ShowReadme("Wavicler");
            SettingsMenuItem.Click += (_, __) => EditSettings();

            // Tab control.
            TabControl.TabPages.Clear();
            TabControl.MouseDoubleClick += TabControl_MouseDoubleClick;

            UpdateMenu();

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
            // Unhook.
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
            }

            void Stop()
            {
                btnPlay.Checked = false;
                _player.Run(false);
            }

            void Rewind()
            {
                var cled = ActiveClipEditor();
                if(cled is not null)
                {
                    cled.SampleProvider.Position = 0;
                }
                //_waveOutSwapper.Rewind();
                //_player.Rewind();
                //TODO1               timeBar.Current = TimeSpan.Zero;
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
        /// Organize the file menu item drop down.
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
        /// Set menu item enables according to system states. TODO tweak later.
        /// </summary>
        void UpdateMenu()
        {
            bool anyOpen = false;
            bool dirty = false;

            var page = ActivePage();
            if (page is not null)
            {
                anyOpen = true;
                dirty = page.Text == NEW_FILE_IND;
            }

            btnRewind.Enabled = anyOpen;
            btnPlay.Enabled = anyOpen;
            sldVolume.Enabled = true;
            OpenMenuItem.Enabled = true;
            SaveMenuItem.Enabled = false;
            SaveAsMenuItem.Enabled = dirty;
            CloseMenuItem.Enabled = anyOpen;
            CloseAllMenuItem.Enabled = anyOpen;
            ExitMenuItem.Enabled = true;
        }
        #endregion

        #region File I/O
        /// <summary>
        /// Common file opener.
        /// </summary>
        /// <param name="fn">The file to open or create new if empty.</param>
        /// <returns>Success.</returns>
        bool OpenFile(string fn = "")
        {
            bool ok = true;

            UpdateState(AppState.Stop);

            try
            {
                if (fn == "")
                {
                    _logger.Info($"Creating new tab");
                    ClipSampleProvider prov = new(Array.Empty<float>());
                    CreateTab(prov, NEW_FILE_IND);
                }
                else
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
                                //switch (selector.SelectedOption)
                                //{
                                //    case "Left": CreateOrSelect(StereoCoercion.Left); break;
                                //    case "Right": CreateOrSelect(StereoCoercion.Right); break;
                                //    case "Mono": CreateOrSelect(StereoCoercion.Mono); break;
                                //    default: ok = false; break;
                                //}
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
                        //CreateOrSelect(StereoCoercion.Mono);
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
                        //var tab = GetTab(tabName);
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

                        if (seltab is null)
                        {
                            var prov = new ClipSampleProvider(reader, coerce);
                            CreateTab(prov, tabName);
                            //newTab = true;
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
            }
            catch (Exception ex)
            {
                _logger.Error($"Couldn't open the file: {fn} because: {ex.Message}");
                ok = false;
            }

            btnPlay.Enabled = ok;
            UpdateMenu();

            return ok;
        }

        /// <summary>
        /// Allows the user to select an audio clip or midi from file system.
        /// </summary>
        void Open_Click()
        {
            var fileTypes = $"Audio Files|{AudioLibDefs.AUDIO_FILE_TYPES}";
            using OpenFileDialog openDlg = new()
            {
                Filter = fileTypes,
                Title = "Select a file"
            };

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openDlg.FileName);
            }
        }

        /// <summary>
        /// Common file saver.
        /// </summary>
        /// <param name="cled">Data source.</param>
        /// <param name="fn">The file to save to.</param>
        /// <returns>Status.</returns>
        bool SaveFile(ClipEditor? cled, string fn = "")
        {
            bool ok = false;

            if (cled is not null)
            {
                // TODO get all rendered data and save to audio file - to fn if specified else old.
            }

            return ok;
        }

        /// <summary>
        /// Save the file in the current page.
        /// </summary>
        /// <param name="cled">Data source.</param>
        void SaveFileAs(ClipEditor? cled)
        {
            if (cled is not null)
            {
                using SaveFileDialog saveDlg = new()
                {
                    Filter = AudioLibDefs.AUDIO_FILE_TYPES,
                    Title = "Save as file"
                };

                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(cled, saveDlg.FileName);
                }
            }
        }

        /// <summary>
        /// General closer/saver.
        /// </summary>
        /// <param name="all">Close all if true otherwise just the current selected.</param>
        /// <returns></returns>
        bool Close(bool all)
        {
            bool ok = true;

            if (all)
            {
                while (TabControl.TabCount > 0)
                {
                    CloseOne(TabControl.TabPages[0]);
                }
            }
            else if (TabControl.TabCount > 0)
            {
                CloseOne(TabControl.SelectedTab);
            }

            // Local function.
            void CloseOne(TabPage page)
            {
                if (page.Text == NEW_FILE_IND)
                {
                    // TODO ask to save.

                }

                var cled = page.Controls[0] as ClipEditor;
                cled?.Dispose();

                TabControl.TabPages.Remove(page);
                page.Dispose();
            }

            UpdateMenu();

            return ok;
        }
        #endregion

        #region Navigator
        /// <summary>
        /// Initialize tree from user settings.
        /// </summary>
        void InitNavigator()
        {
            var s = AudioLibDefs.AUDIO_FILE_TYPES;
            ftree.FilterExts = s.SplitByTokens("|;*");
            ftree.RootDirs = _settings.RootDirs;

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
        /// <param name="tabName"></param>
        void CreateTab(ClipSampleProvider prov, string tabName)
        {
            ClipEditor ed = new(prov)
            {
                Dock = DockStyle.Fill,
                DrawColor = _settings.WaveColor,
                GridColor = Color.LightGray,
            };
            ed.ServiceRequestEvent += ClipEditor_ServiceRequest;

            TabPage page = new() { Text = tabName };
            page.Controls.Add(ed);
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
                    // Found it, do something TODO close maybe
                    //...
                    break;
                }
            }
        }
        #endregion

        #region Settings
        /// <summary>
        /// Edit user settings.
        /// </summary>
        void EditSettings()
        {
            var changes = _settings.Edit("User Settings", 500);

            // Detect changes of interest.
            bool navChange = false;
            bool restart = false;

            foreach (var (name, cat) in changes)
            {
                switch(name) //TODO check all these names
                {
                    case "WavOutDevice":
                    case "Latency":
                    case "ControlColor":
                    case "WaveColor":
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
            _settings.Save();
        }
        #endregion

        #region Utilities
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ResampleMenuItem_Click(object sender, EventArgs e)
        {
            //TODO get two file names and execute.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StereoSplitMenuItem_Click(object sender, EventArgs e)
        {
            //TODO get one file name and execute.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StereoToMonoMenuItem_Click(object sender, EventArgs e)
        {
            //TODO get one file name and execute.
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Helper function.
        /// </summary>
        /// <returns></returns>
        TabPage? ActivePage()
        {
            TabPage? page = TabControl.TabPages.Count > 0 ? TabControl.SelectedTab : null;
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

        /// <summary>
        /// 
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

                case ClipEditor.ServiceRequest.CopySelectionToNewClip: // TODO
                    break;
            }
        }
    }
}
