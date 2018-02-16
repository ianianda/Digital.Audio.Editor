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

namespace WindowsFormsApp2
{
    public partial class Form1
    {
        /* Clicks windowing button. */
        private void windowing_Click(object sender, EventArgs e)
        {
            DataPointCollection wholePoints = chart1.Series["wave"].Points; // gets all the points on the time domain chart.
            double[] selectedPoints = new double[endPosition - startPosition + 1]; // gets all the points in the selected area.

            for (int i = 0; i < endPosition - startPosition + 1; i++)
            {
                selectedPoints[i] = wholePoints[i].YValues[0]; // stores all selected points.
            }

            double[] weighted = new double[selectedPoints.Length]; // stores points from windowing.

            for (int i = 0; i < weighted.Length; i++) // applies triangle windowing.
            {
                weighted[i] = (1 - Math.Abs(((double)i - ((weighted.Length - 1) / 2)) / (weighted.Length - 1) / 2)) * selectedPoints[i];
            }

            double[] weightedA = Class1.DFT(weighted); // applies DFT after windowing.
            chart2.Series["frequency"].Points.Clear(); //clears out all points on frequency domain.

            for (int i = 0; i < weightedA.Length; i++) // draws new points on the frequency domain.
            {
                chart2.Series["frequency"].Points.AddXY(i, weightedA[i]);
            }
        }
    }
}