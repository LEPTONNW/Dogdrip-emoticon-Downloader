using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 개드립콘파싱툴
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //20250630 v1.2
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

        public static string[] 리스트상단제거;
        public static string[] 리스트하단제거;
        private async void button1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("시작");
            try
            {
                button1.Enabled = false;

                if (textBox1.Text == "" || String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("주소를 입력후 눌러주세요");
                    button1.Enabled = true;
                    return;
                }

                listBox1.Items.Clear();

                await extractHTMLAsync();
                status.Text = "Status : 로딩완료";


                
                if (HTMLSorce.Contains("data-dnv-loaded=\"false\""))
                {
                    리스트상단제거 = HTMLSorce.Split(new string[] { "<div class=\"dogcon_file_list\" data-dnv-loaded=\"false\" data-dnv-processed=\"true\"" }, StringSplitOptions.None);
                }
                else
                {
                    리스트상단제거 = HTMLSorce.Split(new string[] { "<div class=\"dogcon_file_list\" data-dnv-processed=\"true\"" }, StringSplitOptions.None);
                }

                
                if (리스트상단제거[1].Contains("<div class=\"dogcon_buy not_logged_in\">"))
                {
                    리스트하단제거 = 리스트상단제거[1].Split(new string[] { "<div class=\"dogcon_buy not_logged_in\">" }, StringSplitOptions.None);
                }
                else if (리스트상단제거[1].Contains("<div class=\"dogcon-loading-spinner dogcon-overlay dogcon-spinner-top\">"))
                {
                    리스트하단제거 = 리스트상단제거[1].Split(new String[] { "<div class=\"dogcon-loading-spinner dogcon-overlay dogcon-spinner-top\">" }, StringSplitOptions.None);
                }
                else
                {
                    리스트하단제거 = 리스트상단제거[1].Split(new String[] { "</ul>" }, StringSplitOptions.None);
                }

                
                

                string[] 리스트텍스트가공 = 리스트하단제거[0].Split(new string[] { "/dvs" }, StringSplitOptions.None);


                foreach (string 가공1 in 리스트텍스트가공.Skip(1))
                {
                    String temp = "";
                    if (가공1.Contains("\" alt=\""))
                    {
                        temp = "https://rc.dogdrip.net/dvs" + 가공1.Split(new String[] { "\" alt=\"" }, StringSplitOptions.None)[0];


                        listBox1.Items.Add(temp);
                    }
                    else
                    {
                        temp = "https://rc.dogdrip.net/dvs" + 가공1.Split(new string[] { ");\"></div>" }, StringSplitOptions.None)[0];

                        if (temp.Contains("data-dnv-applied"))
                        {
                            listBox1.Items.Add(temp.Trim().Split(new string[] { ");\"" }, StringSplitOptions.None)[0]);
                        }
                        else
                        {
                            listBox1.Items.Add(temp);
                        }
                    }
                }
                

                textBox1.Text = "";
                button1.Enabled = true;
                status.Text = "Status : 대기";
            }
            catch (Exception ea)
            {
                MessageBox.Show(ea.Message);
                //MessageBox.Show("개드립 주소가 아닙니다."); 
                //Clipboard.SetText(HTMLSorce); 

            }
        }


        //Javascript 가 완전히 로드 된 후에 HTML 소스를 가져오게 하는 메서드
        private async Task extractHTMLAsync()
        {
            status.Text = "Status : 스크립트 로딩중...";

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();

            //크롬 사용자로 위장
            await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
            {
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36",
                ["Accept-Language"] = "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7",
                ["Referer"] = "https://www.google.com/"
            });


            // 모든 JS/네트워크 리소스가 멈출 때까지 대기
            await page.GotoAsync(textBox1.Text.Trim(), new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 60000// 60초까지 기다릴 수 있음
            });

            HTMLSorce = await page.ContentAsync();  // JS 실행 후 HTML 전체를 추출

            //Clipboard.SetText(HTMLSorce);
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

                        string ReplaceResult = a.Trim().Replace("https://rc.dogdrip.net/dvs", "");
                        ReplaceResult = ReplaceResult.Replace("/", "");

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
