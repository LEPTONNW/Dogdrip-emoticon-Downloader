using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 개드립콘_파싱툴
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static string HTMLSorce;
        public static List<string> List = new List<string>();
        public static string selected = "";
        public static string fileName = "";
        public static int count1 = 0;
        public static int count2 = 1;
        public static int count3 = 0;
        public static int tmpi = 1;
        public static DirectoryInfo di;
        public static string[] temp4;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Clear();

                HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(textBox1.Text.Trim());

                HttpWebResponse oGetResponse = (HttpWebResponse)oRequest.GetResponse();

                StreamReader oStreamReader = new StreamReader(oGetResponse.GetResponseStream());

                string strHtml = oStreamReader.ReadToEnd();

                HTMLSorce = strHtml;

                string[] temp = HTMLSorce.Split(new string[] { "sicker" }, StringSplitOptions.None);
                string[] temp2 = temp[1].Split(new string[] { "not_logged_in" }, StringSplitOptions.None);
                string[] temp3 = temp2[0].Split(new string[] { "<li>" }, StringSplitOptions.None);


                HTMLSorce = "";

                foreach (string a in temp3)
                {
                    try
                    {

                        if(a.Contains("(.") == true)
                        {
                            temp4 = a.Split(new string[] { "(." }, StringSplitOptions.None);
                            HTMLSorce += temp4[1];
                        }
                        else if(a.Contains("(//") == true)
                        {
                            string[] tt = a.Split(new string[] { "(//" }, StringSplitOptions.None);
                            temp4 = tt[1].Split(new string[] { "rc.dogdrip.net" }, StringSplitOptions.None);
                            HTMLSorce += temp4[1];
                            //MessageBox.Show(temp4[1].ToString());
                        }


                    }
                    catch (Exception av) 
                    { 
                        MessageBox.Show(av.Message);
                    }
                }

                string[] temp4_1 = HTMLSorce.Split(new string[] { "</li>" }, StringSplitOptions.None);
                HTMLSorce = "";
                foreach (string a in temp4_1)
                {
                    HTMLSorce += a;
                }


                string[] temp4_2 = HTMLSorce.Split(new string[] { ");" + '\u0022' + "></div>" }, StringSplitOptions.None);
                count1 = temp4_2.Length;

                HTMLSorce = "";

                foreach (string a in temp4_2)
                {

                    if (count2 > count1 - 1)
                    {

                    }
                    else
                    {
                        string temp5 = "https://rc.dogdrip.net" + a.Trim();
                        string temp5_1 = "jpg";
                        string temp5_2 = "gif";
                        string temp5_3 = "png";
                         
                        bool temp6 = temp5.Contains(temp5_1);
                        bool temp7 = temp5.Contains(temp5_2);
                        bool temp8 = temp5.Contains(temp5_3);

                        if (temp6 == true)
                        {
                            string url = temp5;

                            //fileName = textBox2.Text + @"\" + tmpi;
                            if (count3 == 0)
                            {
                                if (File.Exists(fileName + ".jpg"))
                                {
                                    //di = new DirectoryInfo(textBox2.Text);
                                    FileInfo[] F = di.GetFiles();
                                    for (int i = 0; i < F.Length; i++)
                                    {
                                        string[] ntmp = F[i].ToString().Split('.');
                                        if (tmpi < Convert.ToInt32(ntmp[0]))
                                        {
                                            tmpi = Convert.ToInt32(ntmp[0]);

                                        }
                                    }
                                    tmpi = tmpi + 1;
                                    //fileName = textBox2.Text + @"\" + tmpi;
                                    count3 = 1;
                                }
                            }

                            //MessageBox.Show(tmpi.ToString());
                            fileName += ".jpg";


                            //DownloadRemoteImageFile(url, fileName);

                            listBox1.Items.Add(url);
                        }
                        else if (temp7 == true)
                        {
                            string url = temp5;

                            //fileName = textBox2.Text + @"\" + tmpi;
                            if (count3 == 0)
                            {
                                if (File.Exists(fileName + ".gif"))
                                {
                                    //di = new DirectoryInfo(textBox2.Text);
                                    FileInfo[] F = di.GetFiles();
                                    for (int i = 0; i < F.Length; i++)
                                    {
                                        string[] ntmp = F[i].ToString().Split('.');
                                        if (tmpi < Convert.ToInt32(ntmp[0]))
                                        {
                                            tmpi = Convert.ToInt32(ntmp[0]);

                                        }
                                    }
                                    tmpi = tmpi + 1;
                                    //fileName = textBox2.Text + @"\" + tmpi;
                                    count3 = 1;
                                }
                            }
                            fileName += ".gif";


                            //DownloadRemoteImageFile(url, fileName);

                            listBox1.Items.Add(url);
                        }
                        else if (temp8 == true)
                        {
                            string url = temp5;

                            //fileName = textBox2.Text + @"\" + tmpi;
                            if (count3 == 0)
                            {
                                if (File.Exists(fileName + ".png"))
                                {
                                    //di = new DirectoryInfo(textBox2.Text);
                                    FileInfo[] F = di.GetFiles();
                                    for (int i = 0; i < F.Length; i++)
                                    {
                                        string[] ntmp = F[i].ToString().Split('.');
                                        if (tmpi < Convert.ToInt32(ntmp[0]))
                                        {
                                            tmpi = Convert.ToInt32(ntmp[0]);

                                        }
                                    }
                                    tmpi = tmpi + 1;
                                    //fileName = textBox2.Text + @"\" + tmpi;
                                    count3 = 1;
                                }
                            }
                            fileName += ".png";


                            //DownloadRemoteImageFile(url, fileName);
                            listBox1.Items.Add(url);
                        }

                        //HTMLSorce += a.Trim();
                    }
                    count2++;
                    tmpi++;

                }
                //Clipboard.SetText(HTMLSorce);

                count2 = 1;
                tmpi = 1;
                count3 = 0;
                textBox1.Text = "";
            }
            catch { MessageBox.Show("개드립 주소가 아닙니다."); }
        }

        private bool DownloadRemoteImageFile(string uri, string fileName) //이미지 다운로드 모듈
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            bool bImage = response.ContentType.StartsWith("image",
                StringComparison.OrdinalIgnoreCase);
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                bImage)
            {
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string temp = listBox1.SelectedItem.ToString();

            try
            {
                Process.Start("chrome", temp);
            }
            catch
            {
                Process.Start(temp);
            }
        }
        public static int co = 0;
        public static int co2 = 0;
        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (selected == "")
                { MessageBox.Show("경로부터 지정하세요"); }
                else
                {
                    co = listBox1.Items.Count;

                    foreach (string a in listBox1.Items)
                    {
                        if(co2 < co)
                        {
                            co2++;
                            status.Text = "status : " + (co2.ToString()) + "번 다운로드 중...";
                        }
                        else
                        {
                            co2 = 0;
                            status.Text = "status : " + (co.ToString()) + "번 다운로드 중...";
                        }

                        string ReplaceResult = a.Trim().Replace("https://rc.dogdrip.net/", "");
                        ReplaceResult = ReplaceResult.Replace("/", "");
                        ReplaceResult = ReplaceResult.Replace("dvs", "");
                        //MessageBox.Show(a);
                        DownloadRemoteImageFile(a, Path.Combine(textBox2.Text + @"\" + ReplaceResult));
                    }
                    co = 0;
                    status.Text = "status : 대기";
                    MessageBox.Show("다운로드 완료");
                }
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            selected = dialog.SelectedPath;
            textBox2.Text = selected;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
