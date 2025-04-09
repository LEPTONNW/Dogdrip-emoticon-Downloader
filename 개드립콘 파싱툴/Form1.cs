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
using System.Threading;
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
            MessageBox.Show("시작");
            try
            {
                listBox1.Items.Clear();

                HttpWebRequest oRequest = (HttpWebRequest)WebRequest.Create(textBox1.Text.Trim());

                HttpWebResponse oGetResponse = (HttpWebResponse)oRequest.GetResponse();

                StreamReader oStreamReader = new StreamReader(oGetResponse.GetResponseStream());

                string strHtml = oStreamReader.ReadToEnd();

                HTMLSorce = strHtml;



                string[] 리스트상단제거 = HTMLSorce.Split(new string[] { "sticker_file_list\">" }, StringSplitOptions.None);

                string[] 리스트하단제거 = 리스트상단제거[1].Split(new string[] { "<div>" }, StringSplitOptions.None);
                string 리스트텍스트가공 = 리스트하단제거[0].Split(new string[] { "<div class=\"sticker_buy" }, StringSplitOptions.None)[0].Replace("<ul>", "").Replace("</ul>", "");

                string[] 리스트배열저장 = 리스트텍스트가공.Split(new string[] { "<div class=\"stk" }, StringSplitOptions.None);
                List<String> 리스트가공배열 = new List<String>();

                foreach (string s in 리스트배열저장)
                {
                    리스트가공배열.Add(s);
                }
                리스트가공배열.RemoveAt(0); //첫번째 빈칸 제거

                List<String> 리스트가공배열2 = new List<string>();

                foreach (string s in 리스트가공배열)
                {

                    string temp = "";
                    if (s.Contains("rc.dogdrip.net"))
                    {
                        temp = s.Split(new string[] { "rc.dogdrip.net" }, StringSplitOptions.None)[1].Split(new string[] { ");\">" }, StringSplitOptions.None)[0];
                    }
                    else
                    {
                        temp = "/" + s.Split(new string[] { "./" }, StringSplitOptions.None)[1].Split(new string[] { ");\">" }, StringSplitOptions.None)[0];
                    }

                    리스트가공배열2.Add(temp.Trim());

                }


                
                foreach (string a in 리스트가공배열2)
                {
                    String url = "https://rc.dogdrip.net" + a;
                    listBox1.Items.Add(url);
                }

                textBox1.Text = "";
            }
            catch(Exception ea) { MessageBox.Show("개드립 주소가 아닙니다."); }
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
            Thread th1 = new Thread(new ThreadStart(Download));
            th1.Start();
        }

        public void Download()
        {
            try
            {
                if (selected == "")
                { MessageBox.Show("경로부터 지정하세요"); }
                else
                {
                    co = listBox1.Items.Count;

                    double temp = (100 / co);
                    int progress = Convert.ToInt32(Math.Ceiling(temp));

                    int progresstemp = 0;

                    foreach (string a in listBox1.Items)
                    {
                        if (co2 < co)
                        {
                            co2++;
                            status.Text = "status : " + (co2.ToString()) + "번 다운로드 중...";
                        }
                        else
                        {
                            co2 = 0;
                            status.Text = "status : " + (co.ToString()) + "번 다운로드 중...";
                        }

                        if (progress > 90)
                        {
                            progressBar1.Value = 100;
                        }
                        else
                        {
                            progresstemp += progress;
                            progressBar1.Value = progresstemp;
                        }

                        string ReplaceResult = a.Trim().Replace("https://rc.dogdrip.net/", "");
                        ReplaceResult = ReplaceResult.Replace("/", "");
                        ReplaceResult = ReplaceResult.Replace("dvs", "");

                        DownloadRemoteImageFile(a, Path.Combine(textBox2.Text + @"\" + ReplaceResult));
                    }

                    //초기화
                    co = 0;
                    co2 = 0;
                    temp = 0;
                    progress = 0;
                    progresstemp = 0;
                    progressBar1.Value = 0;

                    status.Text = "status : 대기";

                    Process.Start(Path.Combine(textBox2.Text)); //다운로드 완료된 폴더 열기
                    MessageBox.Show(new Form { TopMost = true }, "다운로드 완료");
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
