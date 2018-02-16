using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1
    {
        public void save_Click(object sender, EventArgs e) //save
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "*.wav|*.wav";
            if (saveFile.ShowDialog() != DialogResult.OK) return;


            FileStream fs = (FileStream)saveFile.OpenFile();

            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                try
                {
                    bw.Write(chunkID);
                    bw.Write(lengthforSave);
                    bw.Write(riffType);
                    bw.Write(fmtID);
                    bw.Write(fmtSize);
                    bw.Write(fmtCode);
                    bw.Write(channels);
                    bw.Write(sampleRate);
                    bw.Write(byteRate);
                    bw.Write(fmtBlockAlign);
                    bw.Write(bitDepth);
                    bw.Write(fmtExtraSize);
                    bw.Write(dataID);
                    bw.Write(bytes);
                    if (byteArray != null)
                    {
                        bw.Write(byteArray);
                    }
                    if (bytesForSamp != null)
                    {
                        bw.Write(bytesForSamp);
                    }
                    if (samps != null)
                    {
                        bw.Write(samps);
                    }
                    int temp = 0;
                    //bw.Write(asFloat);
                    if (fmtBlockAlign != 0)
                    {
                        temp = lengthforSave / fmtBlockAlign;
                    }
                    for (int i = 0; i < temp; i++)
                    {
                        if (i < lDataList.Count())
                        {
                            bw.Write((ushort)lDataList[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }

                        if (i < rDataList.Count)
                        {
                            bw.Write((ushort)rDataList[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                    }
                }
                finally
                {
                    if (bw != null)
                    {
                        bw.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }
        }
    }
}
