using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Drawing.Design;
using NBagOfTricks;
using NBagOfUis;
using NBagOfTricks.Slog;
using AudioLib;


namespace Wavicler
{
    [Serializable]
    public sealed class UserSettings : Settings
    {
        #region Persisted Editable Properties
        [DisplayName("Root Directories")]
        [Description("Where to look in order as they appear.")]
        [Browsable(true)]
        [Editor(typeof(StringListEditor), typeof(UITypeEditor))] // Ideally a multi folder picker.
        public List<string> RootDirs { get; set; } = new();

        [DisplayName("Control Color")]
        [Description("Pick what you like.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color ControlColor { get; set; } = Color.MediumOrchid;

        [DisplayName("Wave Color")]
        [Description("Pick what you like.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonColorConverter))]
        public Color WaveColor { get; set; } = Color.ForestGreen;

        [DisplayName("Auto Convert Stereo")]
        [Description("Automatically convert stereo files to mono otherwise ask.")]
        [Browsable(true)]
        public bool AutoConvert { get; set; } = false;

        [DisplayName("File Log Level")]
        [Description("Log level for file write.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel FileLogLevel { get; set; } = LogLevel.Trace;

        [DisplayName("File Log Level")]
        [Description("Log level for UI notification.")]
        [Browsable(true)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LogLevel NotifLogLevel { get; set; } = LogLevel.Debug;

        [DisplayName("Audio Settings")]
        [Description("Edit audio settings.")]
        [Browsable(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AudioSettings AudioSettings { get; set; } = new();
        #endregion

        #region Persisted Non-editable Persisted Properties
        [Browsable(false)]
        public bool Autoplay { get; set; } = true;

        [Browsable(false)]
        public bool Loop { get; set; } = false;

        [Browsable(false)]
        public bool Snap { get; set; } = true;

        [Browsable(false)]
        public double Volume { get; set; } = AudioLibDefs.VOLUME_MAX / 2;

        [Browsable(false)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WaveSelectionMode SelectionMode { get; set; } = WaveSelectionMode.Sample;

        [Browsable(false)]
        public double BPM { get; set; } = 100.0;
        #endregion

        #region Non-persisted Properties
        [Browsable(false)]
        public bool Valid { get; set; } = false;
        #endregion
    }
}
