using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioBeatDetector.Wavelet
{
    class Util
    {
        static public double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static bool IsBinary(int number)
        { 

            var isBinary = false;

            int power = (int)(Math.Log(number) / Math.Log(2.0));

            double result = 1.0 * Math.Pow(2.0, power);

            if (result == number)
                isBinary = true;

            return isBinary;

        } // isBinary

    }
}
