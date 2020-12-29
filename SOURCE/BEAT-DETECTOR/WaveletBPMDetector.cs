using Audio;
using AudioBeatDetector.Wavelet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioBeatDetector
{
    /**
 * Class <code>WaveletBPMDetector</code> can be used to 
 * detect the tempo of a track in beats-per-minute.
 * The class implements the algorithm presented by 
 * Tzanetakis, Essl and Cookin the paper titled 
 * "Audio Analysis using the Discrete Wavelet Transform"
 *
 * Objects of the class can be created using the companion 
 * object's factory method. 
 * 
 * To detect the tempo the discrete wavelet transform is used. 
 * Track samples are divided into windows of frames. 
 * For each window data are divided into 4 frequency sub-bands 
 * through DWT. For each frequency sub-band an envelope is 
 * estracted from the detail coffecients by:
 * 1) Full wave rectification (take the absolute value), 
 * 2) Downsampling of the coefficients, 
 * 3) Normalization (via mean removal)
 * These 4 sub-band envelopes are then summed together. 
 * The resulting collection of data is then autocorrelated. 
 * Peaks in the correlated data correspond to peaks in the 
 * original signal. 
 * then peaks are identified on the filtered data.
 * Given the position of such a peak the approximated 
 * tempo of the window is computed and appended to a colletion.
 * Once all windows in the track are processed the beat-per-minute 
 * value is returned as the median of the windows values.
 * 
 * Audio track data is buffered so that there's no need 
 * to load the whole track in memory before applying 
 * the detection.
 * 
 * Class constructor is private, use the companion 
 * object instead.
 **/
    public class WaveletBPMDetector: BPMDetector
    {
        private AudioFile audioFile;
        private int windowFrames;
        public List<double> InstantBpm { get; }
        private int windowsToProcess;
        private Double _bpm = -1.0;
        private Wavelet.Wavelet wavelet = new Daubechies4();

        public WaveletBPMDetector(AudioFile audioFile, int windowFrames)
        {
            this.audioFile = audioFile;
            this.windowFrames = windowFrames;
            InstantBpm = new List<double>();
            windowsToProcess = (int)(audioFile.NumFrames / windowFrames);
        }

        public void ComputeWindowBpm(double[] data)
        {
            double[] aC = null;
            double[] dC = null;
            double[] dCSum = null;
            int dCMinLength = 0;
            var levels = 4;
            var maxDecimation = Math.Pow(2, levels - 1);
            int minIndex = (int)(60.0 / 220 * (double)audioFile.SampleRate / maxDecimation);
            int maxIndex = (int)(60.0/ 40 * (double)audioFile.SampleRate / maxDecimation);

            // 4 Level DWT
            for(int loop = 0; loop < levels; loop++)
            {
                // Apply DWT
                var transform = new Transform(wavelet);
                if (loop == 0)
                {
                    var coefficients = transform.Decompose(data);
                    var l = coefficients.Length - 1;
                    aC = coefficients[0].Take(coefficients[0].Length / 2).ToArray();
                    dC = 
                        coefficients[l]
                        .Skip(coefficients[0].Length / 2)
                        .Take(coefficients[0].Length)
                        .ToArray();
                    dCMinLength = (int)(dC.Length / maxDecimation) + 1;
                }
                else
                {
                    var coefficients = transform.Decompose(aC);
                    var l = coefficients.Length - 1;
                    aC = coefficients[0].Take(coefficients[0].Length / 2).ToArray();
                    dC = 
                        coefficients[l]
                        .Skip(coefficients[0].Length / 2)
                        .Take(coefficients[0].Length)
                        .ToArray();
                }
                // Extract envelope from detail coefficients
                //  1) Undersample
                //  2) Absolute value
                //  3) Subtract mean
                var pace = (int)Math.Pow(2, (levels - loop - 1));
                dC = Enumerable.Range(start: 0, count: dC.Length)
                    .Zip(dC, (i, v) => Tuple.Create(item1: v, item2: i))
                    .Where(tuple => tuple.Item2 % pace == 0)
                    .Select(tuple => Math.Abs(tuple.Item1))
                    .ToArray();                             //undersample
                double dCMean = dC.Sum()/ dC.Length;
                for (int i = 0; i < dC.Length; i++)
                    dC[i] -= dCMean;

                // Recombine detail coeffients
                if (dCSum == null)
                {
                    dCSum = dC.Take(dCMinLength).ToArray();
                }
                else
                {
                    if (dCMinLength <= dC.Length)
                        for (int i = 0; i < dCMinLength; i++)
                            dCSum[i] += dC[i];
                    else
                        for (int i = 0; i < dC.Length; i++)
                            dCSum[i] += dC[i];
                }
            }
            // Add the last approximated data
            aC = aC.Select(item => Math.Abs(item)).ToArray();
            var aCMean = aC.Sum() / aC.Length;
            for (int i = 0; i < aC.Length; i++) aC[i] -= aCMean;
            if (dCMinLength <= dC.Length)
                for (int i = 0; i < dCMinLength; i++)
                    dCSum[i] += aC[i];
            else
                for (int i = 0; i < dC.Length; i++)
                    dCSum[i] += aC[i];

            // Autocorrelation
            var correlated = correlate(dCSum);
            var correlatedTmp = correlated.Skip(minIndex).Take(maxIndex - minIndex).ToArray();

            // Detect peak in correlated data
            var location = detectPeak(correlatedTmp);

            // Compute window BPM given the peak
            var realLocation = minIndex + location;
            double windowBpm = 60.0 / realLocation * ((double)audioFile.SampleRate / maxDecimation);
            InstantBpm.Add(windowBpm);
        }

        public double bpm()
        {
            if (_bpm == -1)
            {
                var buffer = new int[windowFrames * audioFile.NumChannels];
                int framesRead;
                int numOfElements = buffer.Length/2;
                double[] leftChannelSamples = new double[numOfElements];
                int k;
                for (int currentWindow = 0; currentWindow < windowsToProcess; currentWindow++)
                {
                    framesRead = audioFile.readFrames(buffer, windowFrames);
                    k = 0;
                    for(int i = 0; i < numOfElements; i++)
                    {
                        leftChannelSamples[i] = buffer[k];
                        k += 2;
                    }
                    ComputeWindowBpm(leftChannelSamples);
                }
                _bpm = InstantBpm.Average();
            }
            
            return _bpm;
        }

        private double[] correlate(double[] a)
        {
            var n = a.Length;
            var correlation = new double[n];
            for (int k = 0; k < n; k++)
                for (int i = 0; i < n; i++)
                    if (k + i < n)
                        correlation[k] = correlation[k] + a[i] * a[k + i];

            return correlation;
        }

        private int detectPeak(double[] data)
        {
            var max = Double.MinValue;
            foreach (double x in data)
                if (Math.Abs(x) > max) max = x;

            var location = -1;
            for (int i = 0; i < data.Length && location == -1; i++)
                if (Math.Abs(data[i]) == Math.Abs(max)) location = i;

            return location;
        }
    }
}
