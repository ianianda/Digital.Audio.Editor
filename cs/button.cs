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
        int i = 0;
        int lengthforSave;

        int chunkID;
        int fileSize;
        int riffType;
        int fmtID;
        int fmtSize;
        int fmtCode;
        int channels;
        int sampleRate;
        int byteRate;
        int fmtBlockAlign;
        int bitDepth;
        int fmtExtraSize;
        int dataID;
        int bytes;
        byte[] byteArray;
        int bytesForSamp;
        int samps;
        float[] asFloat;
        List<short> lDataList = new List<short>();
        List<short> rDataList = new List<short>();
        float[] L;
        /* Clicks open button. */
        private void open_Click(object sender, EventArgs e)
        {
            int blockSize = 1000;
            string filename;

            float[] R;

            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Wave File (*.wav)|*.wav;";
            if (open.ShowDialog() != DialogResult.OK) return;
            filename = open.FileName;

            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    // chunk 0
                    chunkID = reader.ReadInt32();
                    fileSize = reader.ReadInt32();
                    riffType = reader.ReadInt32();

                    // chunk 1
                    fmtID = reader.ReadInt32();
                    fmtSize = reader.ReadInt32(); // bytes for this chunk
                    fmtCode = reader.ReadInt16();
                    channels = reader.ReadInt16();
                    sampleRate = reader.ReadInt32();
                    byteRate = reader.ReadInt32();
                    fmtBlockAlign = reader.ReadInt16();
                    bitDepth = reader.ReadInt16();

                    //for (int i = 0; i < fmtSize / fmtBlockAlign; i++)
                    //{
                    //    lDataList.Add((short)reader.ReadUInt16());
                    //    rDataList.Add((short)reader.ReadUInt16());
                    //}

                    if (fmtSize == 18)
                    {
                        // Read any extra values
                        fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }

                    // chunk 2
                    dataID = reader.ReadInt32();
                    bytes = reader.ReadInt32();

                    // DATA!
                    byteArray = reader.ReadBytes(bytes);

                    bytesForSamp = bitDepth / 8;
                    samps = bytes / bytesForSamp;


                    asFloat = null;
                    switch (bitDepth)
                    {
                        case 64:
                            double[]
                            asDouble = new double[samps];
                            Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
                            asFloat = Array.ConvertAll(asDouble, e1 => (float)e1);
                            break;
                        case 32:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
                            break;
                        case 16:
                            Int16[]
                            asInt16 = new Int16[samps];
                            Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e1 => (float)e1); /* e1 => e1 / (float)Int16.MaxValue*/
                            break;
                        default:
                            break;
                            //return false;
                    }

                    switch (channels)
                    {
                        case 1:
                            L = asFloat;
                            R = null;
                            //return true;
                            break;
                        case 2:
                            L = new float[samps];
                            R = new float[samps];
                            for (int i = 0, s = 0; i < samps; i++)
                            {
                                L[i] = asFloat[s++];
                                R[i] = asFloat[s++];
                            }
                            break;
                        //return true;
                        default:
                            break;
                            //return false;
                    }
                }
            }
            catch
            {
                //Debug.Log("...Failed to load note: " + filename);
                //return false;
                //left = new float[ 1 ]{ 0f };
            }

            chart1.Series.Clear();
            chart2.Series.Clear();

            var series1 = chart1.Series.Add("wave");
            var series2 = chart2.Series.Add("frequency");
            chart1.Series["wave"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart2.Series["frequency"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            chart1.Series["wave"].ChartArea = "ChartArea1";

            byte[] buffer = new byte[20];
            bigBuffer.Clear();

            for (int i = 0; i < L.Length; i++)
            {
                chart1.Series["wave"].Points.AddXY(i, L[i]);
            }

            var chartArea1 = chart1.ChartAreas[series1.ChartArea];
            var chartArea2 = chart2.ChartAreas[series2.ChartArea];

            chartArea1.CursorX.AutoScroll = true;

            int position = 0;
            int size = blockSize;
            chartArea1.AxisX.ScaleView.Zoom(position, size);
            chartArea1.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            
            // set scrollbar small change to blockSize (e.g. 100)
            chartArea1.AxisX.ScaleView.SmallScrollSize = blockSize;
            
            chart1.ChartAreas[0].AxisX.Maximum = chart1.Series["wave"].Points.Count;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = chart1.ChartAreas[0].AxisX.Maximum / 10;
            chartArea1.AxisX.ScaleView.Zoomable = false;
        }

        List<DataPoint> newselectedPoints;
        int startPosition;
        int endPosition;
        double[] copiedPoints;

        /* clicks copy button to copy the selected area points */
        private void copy_Click(object sender, EventArgs e) //copy
        {
            newselectedPoints = selectedPoints;
            copiedPoints = new double[newselectedPoints.Count];
            for (int i = 0; i < newselectedPoints.Count; i++)
            {
                copiedPoints[i] = newselectedPoints[i].YValues[0];
            }
            startPosition = (int)chart1.ChartAreas[0].CursorX.SelectionStart;
            endPosition = (int)chart1.ChartAreas[0].CursorX.SelectionEnd;
        }

        List<double> newSamplesFromChart1;

        /* Clicks paste button to paste selected points to a new area */
        private void paste_Click(object sender, EventArgs e) //paste
        {
            delete_Click(sender, e);
            DataPointCollection wholePoints = chart1.Series["wave"].Points;
            double[] tempData = new double[wholePoints.Count + copiedPoints.Length];
            for (int i = 0; i < startPosition; i++)
            {
                tempData[i] = wholePoints[i].YValues[0];
            }

            for (int i = 0; i < copiedPoints.Length; i++)
            {
                tempData[i + startPosition] = copiedPoints[i];
            }

            for (int i = startPosition + copiedPoints.Length, j = startPosition; i < tempData.Length; i++, j++)
            {
                tempData[i] = wholePoints[j].YValues[0];
            }

            chart1.Series["wave"].Points.Clear();

            for (int i = 0; i < tempData.Length; i++)
            {
                chart1.Series["wave"].Points.AddXY(i, tempData[i]);
            }


            List<double> buffer = new List<double>();

            for (int i = 0; i < tempData.Length; i++)
            {
                buffer.Add(tempData[i]);
            }
            newSamplesFromChart1 = buffer;
        }

        /* Clicks cut button to cut off an area*/
        private void cut_Click(object sender, EventArgs e) // cut
        {
            copy_Click(sender, e);
            delete_Click(sender, e);
        }

        /* Clicks delete button */
        private void delete_Click(object sender, EventArgs e) // deletes points on the time domain chart
        {
            DataPointCollection wholePoints = chart1.Series["wave"].Points;
            startPosition = (int)chart1.ChartAreas[0].CursorX.SelectionStart;
            endPosition = (int)chart1.ChartAreas[0].CursorX.SelectionEnd;
            List<double> buffer = new List<double>();
            lengthforSave = buffer.Count();

            for (int a = 0; a < startPosition; a++)
            {
                buffer.Add(wholePoints[a].YValues[0]);
            }

            for (int b = endPosition; b < wholePoints.Count; b++)
            {
                buffer.Add(wholePoints[b].YValues[0]);
            }

            chart1.Series["wave"].Points.Clear();
            for (int i = 0; i < buffer.Count; i++)
            {
                chart1.Series["wave"].Points.AddXY(i, buffer[i]);
            }
            newSamplesFromChart1 = buffer;
        }
    }
}