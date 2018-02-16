using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    class Class1
    {
        private static double[] A;
        private static int masterInd = 0;
        static private object threadLock = new object();

        /* DFT function to apply DFT */
        public static double[] DFT(double[] S)
        {
            A = new double[S.Length];
            double[] tempreal = new double[S.Length];
            double[] tempimag = new double[S.Length];
            double N = S.Length, real, imag;
            masterInd = 0;
            int f = masterInd;

            while (f <= N - 1)
            {
                lock (threadLock)
                    if (masterInd <= N - 1)
                        f = masterInd++;
                    else break;
                real = 0;
                imag = 0;
                for (int t = 0; t <= N - 1; t++)
                {
                    real += S[t]  *Math.Cos(2 * Math.PI * t * f / N);
                    imag -= S[t]  *Math.Sin(2 * Math.PI*  t * f / N);
                    tempreal[t] = real;
                    tempimag[t] = imag;
                }
                A[f] = Math.Sqrt(real * real + imag * imag) / N;
            }
            return A;
        }

        /* IDFT function to apply IDFT */
        public static double[] IDFT(double[] A)
        {
            double[] S = new double[A.Length];
            double N = A.Length, real, imag;

            for (int t = 0; t < N; t++)
            {
                real = 0;
                imag = 0;
                for (int f = 0; f < N; f++)
                {
                    real += A[f] * Math.Cos(2 * Math.PI * t * f / N);
                    imag += A[f] * Math.Sin(2 * Math.PI * t * f / N);
                }
                S[t] = (real - imag) / N;
            }
            return S;
        }
    }
}
