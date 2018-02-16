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
    public partial class Form1 : Form
    {
        bool zoom = false;

        List<byte> bigBuffer = new List<byte>();
        public Form1()
        {
            InitializeComponent();

            /* Sets layout performance for both charts. */
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chart1.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
            chart1.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            chart1.ChartAreas[0].AxisX.Title = "Time";
            chart1.ChartAreas[0].AxisY.Title = "Amplitude";

            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
            chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorX.AutoScroll = false;
            chart2.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
            chart2.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
            chart2.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
            chart2.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            chart2.ChartAreas[0].AxisX.Title = "Time";
            chart2.ChartAreas[0].AxisY.Title = "Amplitude";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeRecord(); // calls record function in dll.
        }

        /* Clicks DFT button. */
        private void button8_Click(object sender, EventArgs e)
        {
            double[] hello = new double[buffer5.Count]; // gets the same size array of time domain chart.
            
            for (int i = 0; i < buffer5.Count; i++) // copies all points from time domain
            {
                hello[i] = buffer5[i];
            }

            double[] result = Class1.DFT(hello); // applies dft on time domain points.
            chart2.Series["frequency"].Points.Clear(); // clears all points on frequency domain chart.
            for (int i = 0; i < result.Length; i++) // redraws frequency domain chart 
            {
                chart2.Series["frequency"].Points.AddXY(i, result[i]);
            }
        }

        /* enables to zoom scrollbars */
        private void EnableZoom(object sender, EventArgs e)
        {
            zoom = !zoom;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = zoom;
        }

        /* resets chart */
        private void button10_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
        }
    }
}