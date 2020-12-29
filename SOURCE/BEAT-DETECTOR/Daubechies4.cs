using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioBeatDetector.Wavelet
{
    public class Daubechies4: Wavelet
    {
        public Daubechies4()
        {

            _name = "Daubechies 4"; // name of the wavelet

            _transformWavelength = 2; // minimal wavelength of input signal

            _motherWavelength = 8; // wavelength of mother wavelet

            _scalingDeCom = new double[_motherWavelength];
            _scalingDeCom[0] = -0.010597401784997278;
            _scalingDeCom[1] = 0.032883011666982945;
            _scalingDeCom[2] = 0.030841381835986965;
            _scalingDeCom[3] = -0.18703481171888114;
            _scalingDeCom[4] = -0.02798376941698385;
            _scalingDeCom[5] = 0.6308807679295904;
            _scalingDeCom[6] = 0.7148465705525415;
            _scalingDeCom[7] = 0.23037781330885523;

            _buildOrthonormalSpace(); // build all other coefficients from low pass decomposition

        } // Daubechies4

    }
}
