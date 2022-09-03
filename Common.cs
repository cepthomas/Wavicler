using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    #region Types
    /// <summary>What are we doing.</summary>
    public enum AppState { Stop, Play, Rewind, Complete, Dead }
    #endregion
}
