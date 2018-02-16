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
        /* Move mouse on frequency chart and enable to scroll the chart */
        private void chart2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left) //left mouse button pressed
            {
                if (e.X < 20) // enables to move the chart to right
                {
                    if (chart2.ChartAreas[0].AxisX.ScaleView.Position - 1000 >= 0)
                        chart2.ChartAreas[0].AxisX.ScaleView.Position -= 1000;
                }
                else if (e.X > chart2.Size.Width - 200) // enables to move the chart to left
                {
                    if (chart2.ChartAreas[0].AxisX.ScaleView.Position + 1000 <= chart2.ChartAreas[0].AxisX.Maximum)
                        chart2.ChartAreas[0].AxisX.ScaleView.Position += 1000;
                }

                chart2.Refresh();
            }
        }
    }
}