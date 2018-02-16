using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApp2
{
    public partial class Form1
    {
        /* Clicks convolution button. */
        private void filter_Click(object sender, EventArgs e)
        {
            /* Gets the whole points from graphs. */
            DataPointCollection wholePoints1 = chart1.Series["wave"].Points;
            DataPointCollection wholePoints2 = chart2.Series["frequency"].Points;

            // low pass filter
            int startP = (int)chart2.ChartAreas[0].CursorX.SelectionStart;
            int endP = (int)chart2.ChartAreas[0].CursorX.SelectionEnd;

            if (startP > endP) // selects area from right to left
            {
                endP = startP;
            }

            double[] tempFilter = new double[wholePoints2.Count];

            // Filter {1, 1, 1, .., 1, 0, 0, 0 .... , 0, 0, 0, 1, 1, 1 ... 1, 1}
            for (int i = 0; i < tempFilter.Length; i++)
            {
                if (i < endP || i > tempFilter.Length - endP + 1)
                {
                    tempFilter[i] = 1;
                }
                else
                {
                    tempFilter[i] = 0;
                }
            }

            double[] filterT = Class1.IDFT(tempFilter); // applies IDFT to filter

            double[] tempResult = new double[wholePoints1.Count];

            for (int i = 0; i < tempResult.Length; i++) // does train and track technique
            {
                for (int j = 0; j < filterT.Length; j++)
                {
                    if (i + j < tempResult.Length)
                    {
                        tempResult[i] += wholePoints1[i + j].YValues[0] * filterT[j];
                    }
                }
            }

            chart1.Series["wave"].Points.Clear(); // clears out points on time domain chart

            for (int i = 0; i < tempResult.Length; i++) // adds new points to time domain chart
            {
                chart1.Series["wave"].Points.AddXY(i, tempResult[i]);
            }

            double[] tempDFTSelection = new double[tempResult.Length / 10];
            for (int i = 0; i < tempDFTSelection.Length; i++) // truncates samples
            {
                tempDFTSelection[i] = tempResult[i];
            }

            double[] result = Class1.DFT(tempDFTSelection); // applies DFT to samples.
            chart2.Series["frequency"].Points.Clear(); // clears out points on frequency chart
            for (int i = 0; i < result.Length; i++) // adds new points to frequency chart
            {
                chart2.Series["frequency"].Points.AddXY(i, result[i]);
            }
        }
    }
}