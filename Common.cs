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

    /// <summary>Selection.</summary>
    public enum SelectionMode { Sample, BarBeat, Time };
    #endregion


    public static class Common
    {
        /// <summary>The global options.</summary>
        public static UserSettings TheSettings { get; set; } = new();
    }


    // BPM calcs:
    //sampleRate: 44100 smplPerSec:: 22.67573696145125 usec
    //tempo: 100 beatsPerMin == 100 / 60 beatsPerSec == 60 / 100 secPerBeat
    //smpl per beat = smplPerSec * secPerBeat
    // Debug.WriteLine($"tempo,secPerBeat,samplesPerBeat");
    // for (float tempo = 60.0f; tempo < 200.0f; tempo += 2.5f)
    // {
    //     float secPerBeat = 60.0f / tempo;
    //  utils>>>   float samplesPerBeat = secPerBeat * reader.WaveFormat.SampleRate;
    //     Debug.WriteLine($"{tempo},{secPerBeat},{samplesPerBeat}");
    // }
    //tempo ,secPerBeat,samplesPerBeat
    // 80   ,0.75      ,33075
    // 90   ,0.6666667 ,29400
    // 100  ,0.6       ,26460.002
    // 110  ,0.54545456,24054.547
    // 120  ,0.5       ,22050
    // 190  ,0.31578946,13926.315


    public static class Utils
    {
        public static (int bar, int beat) Convert(int sample)
        {

            return (0, 0);
        }

        public static int Convert(int bar, int beat)
        {

            return 0;
        }

        public static TimeSpan ConvertX(int sample)
        {

            return new();
        }

        public static int Convert(TimeSpan ts)
        {

            return 0;
        }
    }
}
