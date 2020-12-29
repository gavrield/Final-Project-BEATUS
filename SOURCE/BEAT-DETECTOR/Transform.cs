using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioBeatDetector.Wavelet
{
    class Transform
    {
        Wavelet _wavelet;

        public Transform(Wavelet wavelet)
        {
            _wavelet = wavelet;
        }

        public double[] Forward(double[] arrTime, int level)
        {
            if (!Util.IsBinary(arrTime.Length)) return null;

            int noOfLevels =(int) Math.Log(arrTime.Length, 2);
            if (level < 0 || level > noOfLevels) return null;
            double[] arrHilb = new double[arrTime.Length];

            Array.Copy(arrTime, arrHilb, arrTime.Length);
            int l = 0;
            int h = arrHilb.Length;
            int transformWavelength = _wavelet._transformWavelength;
            while (h >= transformWavelength && l < level)
            {

                double[] arrTempPart = _wavelet.forward(arrHilb, h);
                Array.Copy(arrTempPart, arrHilb, h);
                h = h >> 1;
                l++;
            }
            return arrHilb;
        }

        public double[] Reverse(double[] arrHilb, int level)
        {
            if (!Util.IsBinary(arrHilb.Length)) return null;

            int noOfLevels = (int)Math.Log(arrHilb.Length, 2);
            if (level < 0 || level > noOfLevels) return null;
            double[] arrTime = new double[arrHilb.Length];

            Array.Copy(arrTime, arrHilb, arrTime.Length);
            int l = 0;
            int h = arrHilb.Length;
            int transformWavelength = _wavelet._transformWavelength;
            while (h >= transformWavelength && l < level)
            {

                double[] arrTempPart = _wavelet.reverse(arrHilb, h);
                Array.Copy(arrTempPart, arrHilb, h);
                h = h >> 1;
                l++;
            }
            return arrHilb;
        }

        public double[][] Decompose(double[] arrTime)
        {
            int length = arrTime.Length;
            int levels = (int)Math.Log(length, 2);
            double[][] matDeComp = new double[levels + 1][];
            for (int p = 0; p <= levels; p++)
            {
                matDeComp[p] = new double[length];
                Array.Copy(Forward(arrTime, p), matDeComp[p], length);
            }

            return matDeComp;
        }
    }
}
