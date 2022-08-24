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

    public static class Utils
    {
        // sampleRate: 44100
        // smplPerSec:: 22.67573696145125 usec
        // tempo: 100 beatsPerMin == 100 / 60 beatsPerSec == 60 / 100 secPerBeat
        // smplPerBeat = sampleRate * secPerBeat

        /// <summary>
        /// Conversion function.
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="bpm"></param>
        /// <returns>(bar, beat)</returns>
        public static (int bar, int beat) SampleToBarBeat(int sample, float bpm)
        {
            float minPerBeat = 1.0f / bpm;
            float secPerBeat = minPerBeat * 60;
            float smplPerBeat = AudioLibDefs.SAMPLE_RATE * secPerBeat;
            int totalBeats = (int)(sample / smplPerBeat);
            int bar = totalBeats / 4;
            int beat = totalBeats % 4;
            return (bar, beat);
        }

        /// <summary>
        /// Conversion function.
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="beat"></param>
        /// <param name="bpm"></param>
        /// <returns>Sample</returns>
        public static int BarBeatToSample(int bar, int beat, float bpm)
        {
            int totalBeats = 4 * bar + beat;
            float minPerBeat = 1.0f / bpm;
            float secPerBeat = minPerBeat * 60;
            float smplPerBeat = AudioLibDefs.SAMPLE_RATE * secPerBeat;
            return (int)(totalBeats * smplPerBeat);
        }
    }
}
