using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp2
{
    public partial class Form1
    {
        uint length;
        IntPtr testData;
        byte[] Y;


        /* Link function in DLL. */
        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr InitializeRecord();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CloseRecord();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StartRecord();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StartPlay();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool EndRecord();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopPlay();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetData();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint GetLength();

        [DllImport("myDll.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SetData(uint length);

        /* Clicks record button to start record */
        private void Record_Click(object sender, EventArgs e)
        {
            StartRecord();
        }

        /* Clicks end button to finish record */
        private void End_Click(object sender, EventArgs e)
        {
            if (EndRecord())
            {
                length = GetLength();
                testData = GetData();
                drawRecord();
                newSamplesFromChart1 = null;
            }
        }

        /* draws new record on both charts */
        private void drawRecord()
        {
            if (length > 0)
            {
                chart1.Series.Clear();
                chart2.Series.Clear();

                var series1 = chart1.Series.Add("wave");
                var series2 = chart2.Series.Add("frequency");

                chart1.Series["wave"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart2.Series["frequency"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                Y = new byte[length];

                Marshal.Copy(testData, Y, 0, (int)length);

                for (uint read = 0; read < length; read++)
                {
                    chart1.Series["wave"].Points.AddXY(read, Y[read] - 128);
                }

                var chartArea1 = chart1.ChartAreas[series1.ChartArea];
                var chartArea2 = chart2.ChartAreas[series2.ChartArea];

                chartArea1.CursorX.AutoScroll = true;

                // let's zoom to [0,blockSize] (e.g. [0,100])
                chartArea1.AxisX.ScaleView.Zoomable = false;
                chartArea2.AxisX.ScaleView.Zoomable = false;

                int position = 0;
                int size = 50000;
                chartArea1.AxisX.ScaleView.Zoom(position, size);

                // disable zoom-reset button (only scrollbar's arrows are available)
                chartArea1.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                
                // set scrollbar small change to blockSize (e.g. 100)
                chartArea1.AxisX.ScaleView.SmallScrollSize = 5;
                chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
                chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                chart2.ChartAreas[0].CursorX.AutoScroll = false;
                chart2.ChartAreas[0].AxisX.Enabled = AxisEnabled.True;
                chart2.ChartAreas[0].AxisY.Enabled = AxisEnabled.True;
                chart2.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
                chart2.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
                chart1.ChartAreas[0].AxisX.Maximum = chart1.Series["wave"].Points.Count;
                chart1.ChartAreas[0].AxisY.Maximum = 200;
                chart1.ChartAreas[0].AxisY.Minimum = -200;
                chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.Maximum;
                chart2.ChartAreas[0].AxisX.Minimum = 0;
            }
        }

        /* updates edited data to replace the original one */
        private void updateData()
        {
            if(newSamplesFromChart1 != null)
            {
                byte[] newData = new byte[newSamplesFromChart1.Count];
                for (int i = 0; i < newSamplesFromChart1.Count; i++)
                {
                    newData[i] = (byte)(newSamplesFromChart1[i] - 128);
                }
                uint updatedLength = SetData((uint)newData.Length);
                testData = GetData();
                Marshal.Copy(newData, 0, testData, newData.Length);
                testData = GetData();
                Y = new byte[newData.Length];
                Marshal.Copy(testData, Y, 0, newData.Length);
            }
        }

        /* clicks play button to start playing record */
        private void Play_Click(object sender, EventArgs e)
        {
            updateData();
            StartPlay();
        }
    }
}