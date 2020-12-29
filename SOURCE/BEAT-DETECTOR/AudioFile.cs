using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio
{
    public abstract class AudioFile
    {
        public int BytesPerSample { get; set; }
        public long NumFrames { get; set; }
        public int NumChannels { get; set; }
        public long SampleRate { get; set; }
        public int BlockAlign { get; set; }
        public int ValidBits { get; set; }
        public long LengthInSeconds { get; set; }
        public long TrackSeconds { get; set; }
        public long TrackMinutes { get; set; }
        public long trackHours { get; set; }

        public abstract int readNormalizedFrames(Double[] sampleBuffer, int offset, int numFramesToRead);
        /**
        * Read frames values, normalized between 0 and 1
        * @param sampleBuffer Buffer where to put normalized frames
        * @param numFramesToRead Number of frames to read
        * @return The number of frames read
        **/

        public abstract int readNormalizedFrames(Double[] sampleBuffer, int numFramesToRead);
        /**
        * Read frames values, normalized between 0 and 1
        * @param sampleBuffer Buffer where to put normalized frames
        * @param offset Offset inside sampleBuffer where to start writing frames
        * @param numFramesToRead Number of frames to read
        * @return The number of frames read
        **/

        public abstract int readFrames(int[] sampleBuffer, int numFramesToRead);

        /**
        * Read frames values
        * @param sampleBuffer Buffer where to put frames
        * @param numFramesToRead Number of frames to read
        * @return The number of frames read
        **/

        public abstract int readFrames(int[] sampleBuffer, int offset, int numFramesToRead);
        /**
        * Read frames values
        * @param sampleBuffer Buffer where to put frames
        * @param offset Offset inside sampleBuffer where to start writing frames
        * @param numFramesToRead Number of frames to read
        * @return The number of frames read
        **/

        public abstract void close();
        /**
        * Close the audio file
        **/

    }
}
