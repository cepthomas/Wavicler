using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;


namespace Wavicler
{
    internal class WaveEditSampleProvider : ISampleProvider
    {
        float[] _dataL = Array.Empty<float>();
        float[] _dataR = Array.Empty<float>();


        /// <summary>
        /// Gets the WaveFormat of this Sample Provider.
        /// </summary>
        /// <value>The wave format. Default is (44100, 16, 2).</value>
        public WaveFormat WaveFormat { get; } = new(); // (44100, 16, 2)

        /// <summary>
        /// Fill the specified buffer with 32 bit floating point samples
        /// </summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>the number of samples written to the buffer.</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
