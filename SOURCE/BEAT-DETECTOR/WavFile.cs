using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio
{
    class WavFile : AudioFile
    {
        const int BUFFER_SIZE = 4096;
        const int FMT_CHUNK_ID = 0x20746D66;
        const int DATA_CHUNK_ID = 0x61746164;
        const int RIFF_CHUNK_ID = 0x46464952;
        const int RIFF_TYPE_ID = 0x45564157;

        private int bufferPointer;
        private int bytesRead;
        private FileStream iStream;
        private Byte[] buffer = new Byte[BUFFER_SIZE];
        private double unsignedToSigned = 0.0;

        public FileStream File { get; set; }
        
        public long FrameCounter { get; set; }
        public int FloatOffset { get; private set; }
        public int FloatScale { get; private set; }
        public int MaxSignedPCMValue { get; private set; }

        public override void close()
        {
            throw new NotImplementedException();
        }

        public override int readFrames(int[] sampleBuffer, int numFramesToRead)
        {
            return readFrames(sampleBuffer, 0, numFramesToRead);
        }

        public override int readFrames(int[] sampleBuffer, int offset, int numFramesToRead)
        {
            int index = offset;
            for(int f = 0; f < numFramesToRead; f++)
            {
                if (FrameCounter == NumFrames) return f;

                for(int c = 0; c < NumChannels; c++)
                {
                    long sample = readSample();
                    sampleBuffer[index] = (int)sample;
                    index++;
                }
                FrameCounter++;
            }
            return numFramesToRead;
        }

        public override int readNormalizedFrames(double[] sampleBuffer, int numFramesToRead)
        {
            return readNormalizedFrames(sampleBuffer, 0, numFramesToRead);
        }

        public override int readNormalizedFrames(double[] sampleBuffer, int offset, int numFramesToRead)
        {
            int index = offset;
            for (int f = 0; f < numFramesToRead; f++)
            {
                if (FrameCounter == NumFrames) return f;

                for (int c = 0; c < NumChannels; c++)
                {
                    long sample = readSample();
                    sampleBuffer[index] =(double) Math.Abs(Math.Abs(sample) - unsignedToSigned);
                    index++;
                }
                FrameCounter++;
            }
            return numFramesToRead;
        }

        private long readSample()
        {
            long value = 0;
            for (int b = 0; b < BytesPerSample; b++)
            {
                if (bufferPointer == bytesRead)
                {
                    int read = iStream.Read(buffer, 0, BUFFER_SIZE);
                    if (read == -1) throw new WavFileException("Not enough data available");
                    bytesRead = read;
                    bufferPointer = 0;
                }

                int byteValue = buffer[bufferPointer];
                if (b < BytesPerSample - 1 || BytesPerSample == 1)
                    byteValue = byteValue & 0xFF;
                value += (byteValue << (b * 8));

                bufferPointer++;
            }
            return value;
        }

        public long GetLE(Byte[] buffer, int pos, int numBytes)
        {
            var idx = pos + numBytes - 1;
            long value = buffer[idx] & 0xFF;
            for (int b = 0; b < numBytes - 1; b++)
            {
                idx--;
                value = (value << 8) + (buffer[idx] & 0xFF);
            }

            return value;
        }
        //Factory Methode
        public WavFile Create(FileStream file)
        {
            var wavFile = new WavFile();
            this.File = file;

            // Read the first 12 bytes of the file
            var bytesRead = wavFile.File.Read(wavFile.buffer, 0, 12);
            if (bytesRead != 12) throw new WavFileException("Not enough wav file bytes for header");

            // Extract parts from the header
            var riffChunkID = GetLE(wavFile.buffer, 0, 4);
            var chunkSize = GetLE(wavFile.buffer, 4, 4);
            var riffTypeID = GetLE(wavFile.buffer, 8, 4);

            // Check the header bytes contains the correct signature
            if (riffChunkID != RIFF_CHUNK_ID) throw new WavFileException("Invalid Wav Header data, incorrect riff chunk ID");
            if (riffTypeID != RIFF_TYPE_ID) throw new WavFileException("Invalid Wav Header data, incorrect riff type ID");

            // Check that the file size matches the number of bytes listed in header
            if (file.Length != chunkSize + 8)
                throw new WavFileException("Header chunk size (" + chunkSize + ") does not match file size (" + file.Length + ")");

            var foundFormat = false;
            var foundData = false;

            // Search for the Format and Data Chunks
            while (!foundData)
            {
                // Read the first 8 bytes of the chunk (ID and chunk size)
                bytesRead = wavFile.File.Read(wavFile.buffer, 0, 8);
                if (bytesRead == -1) throw new WavFileException("Reached end of file without finding format chunk");
                if (bytesRead != 8) throw new WavFileException("Could not read chunk header");

                // Extract the chunk ID and Size
                var chunkID = GetLE(wavFile.buffer, 0, 4);
                chunkSize = GetLE(wavFile.buffer, 4, 4);

                // Word align the chunk size
                // chunkSize specifies the number of bytes holding data. However,
                // the data should be word aligned (2 bytes) so we need to calculate
                // the actual number of bytes in the chunk
                long numChunkBytes;
                if (chunkSize % 2 == 1)
                    numChunkBytes = chunkSize + 1;
                else 
                    numChunkBytes = chunkSize;

                if (chunkID == FMT_CHUNK_ID)
                {
                    // Flag that the format chunk has been found
                    foundFormat = true;

                    // Read in the header info
                    bytesRead = wavFile.File.Read(wavFile.buffer, 0, 16);
                    // Check this is uncompressed data
                    var compressionCode = GetLE(wavFile.buffer, 0, 2);
                    if (compressionCode != 1)
                        throw new WavFileException("Compression Code " + compressionCode + " not supported");

                    // Extract the format information
                    wavFile.NumChannels = (int)GetLE(wavFile.buffer, 2, 2);
                    wavFile.SampleRate = (int)GetLE(wavFile.buffer, 4, 4);
                    wavFile.BlockAlign = (int)GetLE(wavFile.buffer, 12, 2);
                    wavFile.ValidBits = (int)GetLE(wavFile.buffer, 14, 2);

                    if (wavFile.NumChannels == 0) throw new WavFileException("Number of channels specified in header is equal to zero");
                    if (wavFile.BlockAlign == 0) throw new WavFileException("Block Align specified in header is equal to zero");
                    if (wavFile.ValidBits < 2) throw new WavFileException("Valid Bits specified in header is less than 2");
                    if (wavFile.ValidBits > 64) throw new WavFileException("Valid Bits specified in header is greater than 64, this is greater than a long can hold");

                    // Calculate the number of bytes required to hold 1 sample
                    wavFile.BytesPerSample = (wavFile.ValidBits + 7) / 8;
                    if (wavFile.BytesPerSample * wavFile.NumChannels != wavFile.BlockAlign)
                        throw new WavFileException("Block Align does not agree with bytes required for validBits and number of channels");

                    // Account for number of format bytes and then skip over
                    // any extra format bytes
                    numChunkBytes -= 16;
                    if (numChunkBytes > 0) wavFile.File.Seek(numChunkBytes, SeekOrigin.Current);
                }
                else if (chunkID == DATA_CHUNK_ID)
                {
                    // Check if we've found the format chunk,
                    // If not, throw an exception as we need the format information
                    // before we can read the data chunk
                    if (foundFormat == false) throw new WavFileException("Data chunk found before Format chunk");

                    // Check that the chunkSize (wav data length) is a multiple of the
                    // block align (bytes per frame)
                    if (chunkSize % wavFile.BlockAlign != 0) throw new WavFileException("Data Chunk size is not multiple of Block Align");

                    // Calculate the number of frames
                    wavFile.NumFrames = chunkSize / wavFile.BlockAlign;

                    // Calculate the length of the track in seconds
                    wavFile.LengthInSeconds = wavFile.NumFrames / wavFile.SampleRate;

                    // Flag that we've found the wave data chunk
                    foundData = true;
                }
                else
                {
                    // If an unknown chunk ID is found, just skip over the chunk data
                    wavFile.File.Seek(numChunkBytes, SeekOrigin.Current);
                }
            }
            // Throw an exception if no data chunk has been found
            if (foundData == false) throw new WavFileException("Did not find a data chunk");
            // Calculate the scaling factor for converting to a normalised double
            if (wavFile.ValidBits > 8)
            {
                // If more than 8 validBits, data is signed
                // Conversion required dividing by magnitude of max negative value
                wavFile.FloatOffset = 0;
                wavFile.FloatScale = 1 << (wavFile.ValidBits - 1);
                wavFile.unsignedToSigned = 0x0;
                wavFile.MaxSignedPCMValue = (0x1 << (wavFile.ValidBits - 1));
            }
            else
            {
                // Else if 8 or less validBits, data is unsigned
                // Conversion required dividing by max positive value
                wavFile.FloatOffset = -1;
                wavFile.unsignedToSigned = (0x1 << (wavFile.ValidBits - 1));
                wavFile.FloatScale = (int)(0.5 * ((1 << wavFile.ValidBits) - 1));
                wavFile.MaxSignedPCMValue = (0x1 << (wavFile.ValidBits - 1));
            }

            wavFile.bufferPointer = 0;
            wavFile.bytesRead = 0;
            wavFile.FrameCounter = 0;
            wavFile.TrackMinutes = wavFile.LengthInSeconds / 60;
            wavFile.TrackSeconds = wavFile.LengthInSeconds % 60;
            wavFile.trackHours = wavFile.TrackMinutes / 60;
            wavFile.TrackMinutes = wavFile.TrackMinutes % 60;

            return wavFile;
        }

        public WavFile Create(String filename)
        {
            var file = new FileStream(filename, FileMode.Open);
            return Create(file);
        }
    class WavFileException: Exception
        {
            public WavFileException(string massage = null, Exception cause = null): base(massage, cause)
            {
                
            }
        }
    }
}
