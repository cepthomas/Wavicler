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
    public sealed class UserSettings : SettingsCore
    {
        #region Persisted Editable Properties
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
        public double Volume { get; set; } = AudioLibDefs.VOLUME_MAX / 2;

        [Browsable(false)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WaveSelectionMode DefaultSelectionMode { get; set; } = WaveSelectionMode.Time;

        [Browsable(false)]
        public double DefaultBPM { get; set; } = 100.0;

        [Browsable(false)]
        public FilTreeSettings FilTreeSettings { get; set; } = new();
        #endregion
    }
}
