using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioBeatDetector.Wavelet
{
    public abstract class Wavelet
    {
        public String _name { get; set; }
        public int _motherWavelength { get; set; }
        public int _transformWavelength { get; set; }
        public double[] _scalingDeCom { get; set; }
        public double[] _waveletDeCom { get; set; }
        public double[] _scalingReCon { get; set; }
        public double[] _waveletReCon { get; set; }

        public Wavelet()
        {
            _name = null;
            _motherWavelength = 0;
            _transformWavelength = 0;
            _scalingDeCom = null;
            _waveletDeCom = null;
            _scalingReCon = null;
            _waveletReCon = null;
        } // Wavelet
          /**
     * The method builds form the scaling (low pass) coefficients for
     * decomposition of a filter, the matching coefficients for the wavelet (high
     * pass) for decomposition, for the scaling (low pass) for reconstruction, and
     * for the wavelet (high pass) of reconstruction. This method should be called
     * in the constructor of an orthonormal filter directly after defining the
     * orthonormal coefficients of the scaling (low pass) for decomposition!
     * 
     * @author Christian Scheiblich (cscheiblich@gmail.com)
     * @date 16.02.2014 13:19:27
     */
        protected void _buildOrthonormalSpace()
        {
            // building wavelet as orthogonal (orthonormal) space from
            // scaling coefficients (low pass filter). Have a look into
            // Alfred Haar's wavelet or the Daubechies Wavelet with 2
            // vanishing moments for understanding what is done here. ;-)
            _waveletDeCom = new double[_motherWavelength];
            for (int i = 0; i < _motherWavelength; i++)
                if (i % 2 == 0)
                    _waveletDeCom[i] = _scalingDeCom[(_motherWavelength - 1) - i];
                else
                    _waveletDeCom[i] = -_scalingDeCom[(_motherWavelength - 1) - i];
            // Copy to reconstruction filters due to orthogonality (orthonormality)!
            _scalingReCon = new double[_motherWavelength];
            _waveletReCon = new double[_motherWavelength];
            for (int i = 0; i < _motherWavelength; i++)
            {
                _scalingReCon[i] = _scalingDeCom[i];
                _waveletReCon[i] = _waveletDeCom[i];
            } // i
        } // _buildOrthonormalSpace

        /**
   * Performs the forward transform for the given array from time domain to
   * Hilbert domain and returns a new array of the same size keeping
   * coefficients of Hilbert domain and should be of length 2 to the power of p
   * -- length = 2^p where p is a positive integer.
   * 
   * @date 10.02.2010 08:18:02
   * @author Christian Scheiblich (cscheiblich@gmail.com)
   * @param arrTime
   *          array keeping time domain coefficients
   * @param arrTimeLength
   *          is necessary, due to working only on a part of arrTime not on the
   *          full length of arrTime!
   * @return coefficients represented by frequency domain
   */
        public double[] forward(double[] arrTime, int arrTimeLength)
        {

            double[] arrHilb = new double[arrTimeLength];

            int h = arrHilb.Length >> 1; // .. -> 8 -> 4 -> 2 .. shrinks in each step by half wavelength
            for (int i = 0; i < h; i++)
            {

                arrHilb[i] = arrHilb[i + h] = 0.0; // set to zero before sum up

                for (int j = 0; j < _motherWavelength; j++)
                {

                    int k = (i << 1) + j; // k = ( i * 2 ) + j;
                    while (k >= arrHilb.Length)
                        k -= arrHilb.Length; // circulate over arrays if scaling and wavelet are are larger

                    arrHilb[i] += arrTime[k] * _scalingDeCom[j]; // low pass filter for the energy (approximation)
                    arrHilb[i + h] += arrTime[k] * _waveletDeCom[j]; // high pass filter for the details

                } // Sorting each step in patterns of: { scaling coefficients | wavelet coefficients }

            } // h = 2^(p-1) | p = { 1, 2, .., N } .. shrinks in each step by half wavelength 

            return arrHilb;

        } // forward
          /**
     * Performs the reverse transform for the given array from Hilbert domain to
     * time domain and returns a new array of the same size keeping coefficients
     * of time domain and should be of length 2 to the power of p -- length = 2^p
     * where p is a positive integer.
     * 
     * @date 10.02.2010 08:19:24
     * @author Christian Scheiblich (cscheiblich@gmail.com)
     * @param arrHilb
     *          array keeping frequency domain coefficients
     * @param arrHilbLength
     *          is necessary, due to working only on a part of arrHilb not on the
     *          full length of arrHilb!
     * @return coefficients represented by time domain
     */
        public double[] reverse(double[] arrHilb, int arrHilbLength)
        {

            double[] arrTime = new double[arrHilbLength];
            for (int i = 0; i < arrTime.Length; i++)
                arrTime[i] = 0.0; // set to zero before sum up

            int h = arrTime.Length >> 1; // .. -> 8 -> 4 -> 2 .. shrinks in each step by half wavelength
            for (int i = 0; i < h; i++)
            {

                for (int j = 0; j < _motherWavelength; j++)
                {

                    int k = (i << 1) + j; // k = ( i * 2 ) + j;
                    while (k >= arrTime.Length)
                        k -= arrTime.Length; // circulate over arrays if scaling and wavelet are larger

                    // adding up energy from low pass (approximation) and details from high pass filter
                    arrTime[k] +=
                        (arrHilb[i] * _scalingReCon[j])
                            + (arrHilb[i + h] * _waveletReCon[j]); // looks better with brackets

                } // Reconstruction from patterns of: { scaling coefficients | wavelet coefficients }

            } // h = 2^(p-1) | p = { 1, 2, .., N } .. shrink in each step by half wavelength 

            return arrTime;

        } // reverse

    }
}
