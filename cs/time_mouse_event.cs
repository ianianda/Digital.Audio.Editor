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
        /* Moves mouse on time domain chart. */
        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.X < 20) //enables to move right
                {
                    if (chart1.ChartAreas[0].AxisX.ScaleView.Position - 1000 >= 0)
                        chart1.ChartAreas[0].AxisX.ScaleView.Position -= 1000;
                }
                else if (e.X > chart1.Size.Width - 200) // enables to move left
                {
                    if (chart1.ChartAreas[0].AxisX.ScaleView.Position + 1000 <= chart1.ChartAreas[0].AxisX.Maximum)
                        chart1.ChartAreas[0].AxisX.ScaleView.Position += 1000;
                }
                chart1.Refresh();
            }
        }

        Point mdown = Point.Empty; // selected point is null. (x, y)
        List<DataPoint> selectedPoints; // same as above
        List<double> buffer5;
        List<double> dftArray;

        /* Presses down mouse on time domain chart. */
        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            mdown = e.Location;
            selectedPoints = new List<DataPoint>(); // select what points in that area
            dftArray = new List<double>();
            DataPointCollection wholePoints = chart1.Series["wave"].Points;
            int duration = endPosition - startPosition; // gets the length of selected area
        }

        /* Presses up mouse on frequency domain chart. */
        private void chart1_MouseUp(object sender, MouseEventArgs e)
        {
            Axis ax = chart1.ChartAreas[0].AxisX;
            Axis ay = chart1.ChartAreas[0].AxisY;
            startPosition = (int)chart1.ChartAreas[0].CursorX.SelectionStart;
            endPosition = (int)chart1.ChartAreas[0].CursorX.SelectionEnd;

            for (int i = startPosition; i < endPosition; i++) // adds selected points to another datapoint list.
            {
                selectedPoints.Add(chart1.Series[0].Points[i]);
            }
            
            DataPointCollection wholePoints = chart1.Series["wave"].Points; // gets all points from time domain chart.

            buffer5 = new List<double>();  // stores all selected points on time domain chart.
            for (int i = startPosition; i < endPosition; i++)
            {
                buffer5.Add(wholePoints[i].YValues[0]);
            }
        }
    }
}