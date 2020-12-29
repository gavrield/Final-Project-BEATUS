using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioBeatDetector.Wavelet
{

    /**
     * A class to represent a complex number. A Complex object is immutable once
     * created; the add, subtract and multiply routines return newly-created Complex
     * objects containing each requested result.
     */
    public class Complex
    {
        private double _r;
        private double _j;


        public Complex()
        {
            _r = 0.0;
            _j = 0.0;
        } // Complex

        /**
         * Copy constructor.
         * 
         * @date 19.11.2010 13:22:54
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param c
         *          complex number
         */
        public Complex(Complex c)
        {
            _r = c._r;
            _j = c._j;
        } // Complex

        /**
         * Constructor taking real and imaginary number.
         * 
         * @date 19.11.2010 13:21:48
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param r
         *          real number
         * @param j
         *          imaginary number
         */
        public Complex(double r, double j)
        {
            _r = r;
            _j = j;
        } // Complex

        /**
         * Display the current Complex as a String, for usage in println( ) or writing
         * the complex into a file.

         */
        override public String ToString()
        {
            StringBuilder sb = new StringBuilder().Append(_r);
            if (_j >= 0)
                sb.Append('+');
            else
                sb.Append('-');
            return sb.Append(_j).Append('j').ToString();
        } // toString

        /**
         * Return the real number.
         * 
         * @date 19.11.2010 13:23:34
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @return real number of this complex number
         */
        public double getReal()
        {
            return _r;
        } // getReal( )

        /**
         * Return the imaginary number.
         * 
         * @date 19.11.2010 13:23:51
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @return imaginary number of this complex number
         */
        public double getImag()
        {
            return _j;
        } // getImag

        /**
         * Set the real number.
         * 
         * @date 23.11.2010 18:44:52
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param r
         *          the real number
         */
        public void setReal(double r)
        {
            _r = r;
        } // setReal

        /**
         * Set the imaginary number.
         * 
         * @date 23.11.2010 18:45:16
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param j
         *          the imaginary number
         */
        public void setImag(double j)
        {
            _j = j;
        } // setImag

        /**
         * Add to real number.
         * 
         * @date 23.11.2010 18:49:57
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param r
         *          the real number
         */
        public void addReal(double r)
        {
            _r += r;
        } // addReal

        /**
         * Add to imaginary number.
         * 
         * @date 23.11.2010 18:50:23
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param j
         *          the imaginary number
         */
        public void addImag(double j)
        {
            _j += j;
        } // addImag

        /**
         * multiply scalar to real number.
         * 
         * @date 23.11.2010 18:53:27
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param s
         *          scalar
         */
        public void mulReal(double s)
        {
            _r *= s;
        } // mulReal

        /**
         * multiply scalar to imaginary number.
         * 
         * @date 23.11.2010 18:54:48
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param s
         *          scalar
         */
        public void mulImag(double s)
        {
            _j *= s;
        } // mulImag

        /**
         * Calculate the magnitude of the complex number.
         */
        public double getMag()
        {
            return Math.Sqrt(_r * _r + _j * _j);
        } // getMag( )

        /**
         * Calculates the angle phi of a complex number.
         * @return angle of this complex number
         */
        public double getPhi()
        {
            if (_r == 0.0 && _j == 0)
                return 0.0;
            double phi = Util.RadianToDegree(Math.Atan(Math.Abs(_j / _r)));
            if (_r >= 0.0 && _j >= 0.0 ) // 1. quadrant
                return phi;
            if (_r <= 0.0 && _j >= 0.0 ) // 2. quadrant
                return 180.0 - phi;
            if (_r <= 0.0 && _j <= 0.0 ) // 3. quadrant
                return phi + 180.0;
            if (_r >= 0.0 && _j <= 0.0 ) // 4. quadrant
                return 360.0 - phi;
            return Util.RadianToDegree(Math.Atan(Math.Abs(_j / _r)));
        } // getPhi( )

        /**
         * Returns the stored values as new double array: [ real, imag ].
         * 
         * @date 19.11.2010 13:25:38
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @return returns stored values as array [ real, imag ]
         */
        public double[] toArr()
        {
            double[] arr = { _r, _j };
            return arr;
        } // toArr

        /**
         * Returns the conjugate complex number of this complex number.
         * 
         * @date 19.11.2010 19:36:52
         * @author Thomas Leduc
         * @return new object of Complex keeping the result
         */
        public Complex conjugate()
        {
            return new Complex(_r, -_j);
        } // conjugate

        /**
         * Add another complex number to this one and return.
         * 
         * @date 19.11.2010 13:25:55
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param c
         *          complex number
         * @return new object of Complex keeping the result
         */
        public Complex add(Complex c)
        {
            return new Complex(_r + c._r, _j + c._j);
        } // add

        /**
         * Subtract another complex number from this one.
         * 
         * @date 19.11.2010 13:27:05
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param c
         *          complex number
         * @return new object of Complex keeping the result
         */
        public Complex sub(Complex c)
        {
            return new Complex(_r - c._r, _j - c._j);
        } // sub

        /**
         * Multiply this complex number times another one.
         * 
         * @date 19.11.2010 13:27:36
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param c
         *          complex number
         * @return new object of Complex keeping the result
         */
        public Complex mul(Complex c)
        {
            return new Complex(_r * c._r - _j * c._j, _r * c._j + _j * c._r);
        } // mul

        /**
         * Multiply this complex number times a scalar.
         * 
         * @date 19.11.2010 13:28:03
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         * @param s
         *          scalar
         * @return new object of Complex keeping the result
         */
        public Complex mul(double s)
        {
            return new Complex(_r * s, _j * s);
        } // mul

        /**
         * Divide this complex number by another one.
         * 
         * @param c
         *          complex number
         * @return new object of Complex keeping the result
         */
        public Complex div(Complex c)
        {
            return mul(c.conjugate()).div(c._r * c._r + c._j * c._j);
        } // div

        /**
         * Divide this complex number by a scalar.
         * 
         * @date 19.11.2010 13:29:49
         * @author Thomas Leduc
         * @param s
         *          scalar
         * @return new object of Complex keeping the result
         */
        public Complex div(double s)
        {
            return mul(1.0 / s);
        } // div

        /**
         * Generates a hash code for this object.
         * 
         * @date 19.11.2010 19:42:39
         * @author Thomas Leduc
         * @see java.lang.Object#hashCode()
         */
        override public int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            long temp;
            temp = BitConverter.DoubleToInt64Bits(_j);
            result = prime * result + (int)(temp ^ (uint)(temp >> 32));
            temp = BitConverter.DoubleToInt64Bits(_r);
            result = prime * result + (int)(temp ^ (uint)(temp >> 32));
            return result;
        } // hashCode  

        /**
         * Compare this Complex number with another one.
         * 
         * @date 19.11.2010 13:30:35
         * @author Thomas Leduc
         * @see java.lang.Object#equals(java.lang.Object)
         */
        override public bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            Complex other = (Complex)obj;
            if (BitConverter.DoubleToInt64Bits(_j) != BitConverter.DoubleToInt64Bits(other._j))
                return false;
            if (BitConverter.DoubleToInt64Bits(_r) != BitConverter.DoubleToInt64Bits(other._r))
                return false;
            return true;
        } // equals

        /**
         * Print this complex number to console.
         * 
         * @date 19.11.2010 13:31:16
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         */
        public void show()
        {
            if (_j < 0)
                Console.WriteLine(getReal() + " - j" + Math.Abs(getImag()));
    else
                Console.WriteLine(getReal() + " + j" + getImag());
        } // show

        /**
         * Print this complex number to console with an identifier before.
         * @param ident
         *          string to label this complex number
         */
        public void Show(String ident)
        {
            if (_j < 0)
                Console.WriteLine(ident + ": " + getReal() + " - j"
                    + Math.Abs(getImag()));
    else
                Console.WriteLine(ident + ": " + getReal() + " + j" + getImag());
        } // show

        /**
         * Print magnitude to console out.

         */
        public void showMag()
        {
            Console.WriteLine(getMag());
        } // showMag

        /**
         * Print angle to console out.
         * 
         * @date 19.11.2010 13:32:33
         * @author Christian Scheiblich (cscheiblich@gmail.com)
         */
        public void showPhi()
        {
            Console.WriteLine(getPhi());
        } // showPhi

    } // class
}
