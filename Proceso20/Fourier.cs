// Code to implement decently performing FFT for complex and real valued
// signals. See www.lomont.org for a derivation of the relevant algorithms 
// from first principles. Copyright Chris Lomont 2010. 
// This code and any ports are free for all to use for any reason as long 
// as this header is left in place.

// Nota Provig: En los programas solo se utiliza hasta el momento la rutina RealFFT,
// la cual solamente se ha modificado en que devuelve un arreglo de double.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proceso20
{
    class Fourier
    {
        /// <summary>
        /// Compute the forward or inverse Fourier Transform of data, with 
        /// data containing complex valued data as alternating real and 
        /// imaginary parts. The length must be a power of 2.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        public void FFT(double[] data, bool forward)
        {
            int n = data.Length;
            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0)
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2");
            n /= 2;    // n is the number of samples

            Reverse(data, n); // bit index data reversal

            // do transform: so single point transforms, then doubles, etc.
            double sign = forward ? 1 : -1;
            int mmax = 1;
            while (n > mmax)
            {
                int istep = 2 * mmax;
                double theta = sign * Math.PI / mmax;
                double wr = 1, wi = 0;
                double wpr = Math.Cos(theta);
                double wpi = Math.Sin(theta);
                for (int m = 0; m < istep; m += 2)
                {
                    for (int k = m; k < 2 * n; k += 2 * istep)
                    {
                        int j = k + istep;
                        double tempr = wr * data[j] - wi * data[j + 1];
                        double tempi = wi * data[j] + wr * data[j + 1];
                        data[j] = data[k] - tempr;
                        data[j + 1] = data[k + 1] - tempi;
                        data[k] = data[k] + tempr;
                        data[k + 1] = data[k + 1] + tempi;
                    }
                    double t = wr; // trig recurrence
                    wr = wr * wpr - wi * wpi;
                    wi = wi * wpr + t * wpi;
                }
                mmax = istep;
            }

            // inverse scaling in the backward case
            if (!forward)
            {
                double scale = 1.0 / n;
                for (int i = 0; i < 2 * n; ++i)
                    data[i] *= scale;
            }
        }

        /// <summary>
        /// Compute the forward or inverse Fourier Transform of data, with data
        /// containing complex valued data as alternating real and imaginary 
        /// parts. The length must be a power of 2. This method caches values 
        /// and should be slightly faster on repeated uses than then FFT method. 
        /// It is also slightly more accurate.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        public void TableFFT(double[] data, bool forward)
        {
            int n = data.Length;
            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0)
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2"
                    );
            n /= 2;    // n is the number of samples

            Reverse(data, n); // bit index data reversal

            // make table if needed
            if ((cosTable == null) || (cosTable.Count != n))
                Initialize(n);

            // do transform: so single point transforms, then doubles, etc.
            double sign = forward ? 1 : -1;
            int mmax = 1;
            int tptr = 0;
            while (n > mmax)
            {
                int istep = 2 * mmax;
                double theta = sign * Math.PI / mmax;
                for (int m = 0; m < istep; m += 2)
                {
                    double wr = cosTable[tptr];
                    double wi = sign * sinTable[tptr++];
                    for (int k = m; k < 2 * n; k += 2 * istep)
                    {
                        int j = k + istep;
                        double tempr = wr * data[j] - wi * data[j + 1];
                        double tempi = wi * data[j] + wr * data[j + 1];
                        data[j] = data[k] - tempr;
                        data[j + 1] = data[k + 1] - tempi;
                        data[k] = data[k] + tempr;
                        data[k + 1] = data[k + 1] + tempi;
                    }
                }
                mmax = istep;
            }

            // copy out with optional scaling
            if (!forward)
            {
                double scale = 1.0 / n;
                for (int i = 0; i < 2 * n; ++i)
                    data[i] *= scale;
            }
        }

        /// <summary>
        /// Compute the forward or inverse Fourier Transform of data, with 
        /// data containing real valued data only. The output is complex 
        /// valued after the first two entries, stored in alternating real 
        /// and imaginary parts. The first two returned entries are the real 
        /// parts of the first and last value from the conjugate symmetric 
        /// output, which are necessarily real. The length must be a power 
        /// of 2.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        /// // ***** indican modificaciones al original
        public double[] RealFFT(double[] data, bool forward)  //***** devuelve double[]
        {
            int i; // *****
            int n = data.Length; // # of real inputs, 1/2 the complex length 
            double[] dd; //*****

            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0)
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2"
                    );

            double sign = -1;
            if (forward)
            { // do packed FFT. This can be changed to FFT to save memory
                TableFFT(data, forward);
                sign = 1;
            }

            double theta = sign * 2 * Math.PI / n;
            double wpr = Math.Cos(theta);
            double wpi = Math.Sin(theta);
            double wjr = wpr;
            double wji = wpi;
            for (int j = 1; j <= n / 4; ++j) // ****
            {
                int k = n / 2 - j;
                double tnr = data[2 * k];
                double tni = data[2 * k + 1];
                double tjr = data[2 * j];
                double tji = data[2 * j + 1];

                double e = (tjr + tnr);
                double f = (tji - tni);
                double a = (tjr - tnr) * wji;
                double d = (tji + tni) * wji;
                double b = (tji + tni) * wjr;
                double c = (tjr - tnr) * wjr;

                // compute entry y[j]
                data[2 * j] = 0.5 * (e + sign * (a + b));
                data[2 * j + 1] = 0.5 * (f - sign * (c - d));

                // compute entry y[k]
                data[2 * k] = 0.5 * (e - sign * (a + b));
                data[2 * k + 1] = 0.5 * (sign * (-c + d) - f);

                double temp = wjr;
                // todo - allow more accurate version here? make option?
                wjr = wjr * wpr - wji * wpi;
                wji = temp * wpi + wji * wpr;
            }

            if (forward)
            {
                // compute final y0 and y_{N/2}, store data[0], data[1]
                double temp = data[0];
                data[0] += data[1];
                data[1] = temp - data[1];
            }
            else
            {
                double temp = data[0]; // unpack the y[j], then invert FFT
                data[0] = 0.5 * (temp + data[1]);
                data[1] = 0.5 * (temp - data[1]);
                // do packed FFT. This can be changed to FFT to save memory
                TableFFT(data, false);
            }


            dd = new double[data.Length]; // *****
            dd[0] = Math.Abs(data[0]); // *****
            for (i = 1; i < data.Length - 1; i++) dd[i + 1] = Math.Abs(data[i]);  // *****
            dd[data.Length - 1] = Math.Abs(data[1]);

            return (dd);
        }

        /// <summary>
        /// Devuelve solamente la amplitud del espectro como valores reales.
        /// </summary>
        /// <param name="data">The complex data stored as alternating real 
        /// and imaginary parts</param>
        /// <param name="forward">true for a forward transform, false for 
        /// inverse transform</param>
        /// <returns></returns>
        public double[] RealFFTAmpli(double[] data, bool forward)  //***** devuelve double[]
        {
            int i,j,jj,k,kk; // *****
            int n = data.Length; // # of real inputs, 1/2 the complex length 
            double[] dd; //*****

            // checks n is a power of 2 in 2's complement format
            if ((n & (n - 1)) != 0)
                throw new ArgumentException(
                    "data length " + n + " in FFT is not a power of 2"
                    );

            double sign = -1;
            if (forward)
            { // do packed FFT. This can be changed to FFT to save memory
                TableFFT(data, forward);
                sign = 1;
            }

            double theta = sign * 2 * Math.PI / n;
            double wpr = Math.Cos(theta);
            double wpi = Math.Sin(theta);
            double wjr = wpr;
            double wji = wpi;
            for (j = 1; j <= n / 4; ++j) // ****
            {
                k = n / 2 - j;
                double tnr = data[2 * k];
                double tni = data[2 * k + 1];
                double tjr = data[2 * j];
                double tji = data[2 * j + 1];

                double e = (tjr + tnr);
                double f = (tji - tni);
                double a = (tjr - tnr) * wji;
                double d = (tji + tni) * wji;
                double b = (tji + tni) * wjr;
                double c = (tjr - tnr) * wjr;

                // compute entry y[j]
                data[2 * j] = 0.5 * (e + sign * (a + b));
                data[2 * j + 1] = 0.5 * (f - sign * (c - d));

                // compute entry y[k]
                data[2 * k] = 0.5 * (e - sign * (a + b));
                data[2 * k + 1] = 0.5 * (sign * (-c + d) - f);

                double temp = wjr;
                // todo - allow more accurate version here? make option?
                wjr = wjr * wpr - wji * wpi;
                wji = temp * wpi + wji * wpr;
            }

            if (forward)
            {
                // compute final y0 and y_{N/2}, store data[0], data[1]
                double temp = data[0];
                data[0] += data[1];
                data[1] = temp - data[1];
            }
            else
            {
                double temp = data[0]; // unpack the y[j], then invert FFT
                data[0] = 0.5 * (temp + data[1]);
                data[1] = 0.5 * (temp - data[1]);
                // do packed FFT. This can be changed to FFT to save memory
                TableFFT(data, false);
            }

            // La modificacion siguiente, devuelve los valores de amplitud del espectro
            kk = 1+(int)(data.Length / 2.0);
            dd = new double[kk]; // *****
            dd[0] = Math.Abs(data[0]); // *****
            jj = 1;
            for (i = 2; i < data.Length-1; i+=2)
            {
                dd[jj++] = Math.Abs(data[i]);  // *****                  
            }
            dd[jj] = Math.Abs(data[1]);

            return (dd);
        }

        #region Internals

        /// <summary>
        /// Call this with the size before using the TableFFT version
        /// Fills in tables for speed. Done automatically in TableFFT
        /// </summary>
        /// <param name="size">The size of the FFT in samples</param>
        void Initialize(int size)
        {
            // NOTE: if you do not use garbage collected languages 
            // like C# or Java be sure to free these correctly
            cosTable = new List<double>();
            sinTable = new List<double>();

            // forward pass
            int n = size;
            int mmax = 1;
            while (n > mmax)
            {
                int istep = 2 * mmax;
                double theta = Math.PI / mmax;
                double wr = 1, wi = 0;
                double wpi = Math.Sin(theta);
                // compute in a slightly slower yet more accurate manner
                double wpr = Math.Sin(theta / 2);
                wpr = -2 * wpr * wpr;
                for (int m = 0; m < istep; m += 2)
                {
                    cosTable.Add(wr);
                    sinTable.Add(wi);
                    double t = wr;
                    wr = wr * wpr - wi * wpi + wr;
                    wi = wi * wpr + t * wpi + wi;
                }
                mmax = istep;
            }
        }

        /// <summary>
        /// Swap data indices whenever index i has binary 
        /// digits reversed from index j, where data is
        /// two doubles per index.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="n"></param>
        void Reverse(double[] data, int n)
        {
            // bit reverse the indices. This is exercise 5 in section 
            // 7.2.1.1 of Knuth's TAOCP the idea is a binary counter 
            // in k and one with bits reversed in j
            int j = 0, k = 0; // Knuth R1: initialize
            int top = n / 2;  // this is Knuth's 2^(n-1)
            while (true)
            {
                // Knuth R2: swap - swap j+1 and k+2^(n-1), 2 entries each
                double t = data[j + 2];
                data[j + 2] = data[k + n];
                data[k + n] = t;
                t = data[j + 3];
                data[j + 3] = data[k + n + 1];
                data[k + n + 1] = t;
                if (j > k)
                { // swap two more
                    // j and k
                    t = data[j];
                    data[j] = data[k];
                    data[k] = t;
                    t = data[j + 1];
                    data[j + 1] = data[k + 1];
                    data[k + 1] = t;
                    // j + top + 1 and k+top + 1
                    t = data[j + n + 2];
                    data[j + n + 2] = data[k + n + 2];
                    data[k + n + 2] = t;
                    t = data[j + n + 3];
                    data[j + n + 3] = data[k + n + 3];
                    data[k + n + 3] = t;
                }
                // Knuth R3: advance k
                k += 4;
                if (k >= n)
                    break;
                // Knuth R4: advance j
                int h = top;
                while (j >= h)
                {
                    j -= h;
                    h /= 2;
                }
                j += h;
            } // bit reverse loop
        }

        /// <summary>
        /// Precomputed sin/cos tables for speed
        /// </summary>
        List<double> cosTable;
        List<double> sinTable;

        #endregion


    }
}
