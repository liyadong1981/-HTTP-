using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HttpPostTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        string url = "";
        string filePath = "";
        int tryCount = 0;
        int spac = 0;
        int SuccessCount = 0;
        int FailCount = 0;
        string result = "";
        FileStream fst;
        int index = 0;
        //打开
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                txtFilePath.Text = openFileDialog1.FileName;
        }
        //发送
        private void button2_Click_1(object sender, EventArgs e)
        {
            msg.Text = "";
            successC.Text = "0";
            responseTime.Text = "";
            SuccessCount = 0;
            FailCount = 0;
            result = "";

            url = txtUrl.Text;
            filePath = txtFilePath.Text;
            int tn = (int)numThreads.Value;
            tryCount = (int)numTrys.Value;
            spac = int.Parse(txtSpac.Text);

            //使用cp提供的下行包发送测试
            if (filePath != "")
            {
                fst = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader2 = new StreamReader(fst, Encoding.Default);
                result = "[" + DateTime.Now.ToString() + "]\r\n发送数据包：\r\n" + reader2.ReadToEnd() +
                    "\r\n\r\n\r\n\r\n\r\n";
                reader2.Close();
            }


            for (int i = 0; i < tn; i++)
            {
                Thread td = new Thread(new ThreadStart(ThreadFun));
                td.Start();
            }



            //while ((SuccessCount + FailCount) < tn * tryCount)
            //{
            //    //Thread.Sleep(10);
            //    msg.Text = result;
            //}
            //msg.Text += "\r\n-------------------------------------------------------------";
            //msg.Text += "\r\n成功：" + SuccessCount;
            //msg.Text += "\r\n失败：" + FailCount;
            //msg.Text += "\r\n成功率：" + string.Format("{0:p}", ((double)SuccessCount / (SuccessCount + FailCount)));
        }

        public void ThreadFun()
        {
            for (int j = 0; j < tryCount; j++)
            {
                PostOneData();
                Thread.Sleep(spac);
            }
        }

        public string ConvertFromBase64String(string strOri, Encoding encodeing)
        {
            if (strOri == null || strOri.Length == 0)
            {
                return strOri;
            }

            strOri = strOri.Replace(" ", "+");
            byte[] buff = Convert.FromBase64String(strOri);
            return encodeing.GetString(buff);
        }
        public void PostOneData()
        {
            Stopwatch st = new Stopwatch();
            bool success = false;
            try
            {
                index++;
                // string sendData = "";
                //使用cp提供的下行包发送测试

                st.Start();
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                StreamReader reader1 = new StreamReader(fs, Encoding.Default);
                byte[] postData = System.Text.Encoding.UTF8.GetBytes(reader1.ReadToEnd());
                reader1.Close();
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded"; //
                myRequest.ContentLength = postData.Length;
                Stream newStream = myRequest.GetRequestStream();
                //发送数据
                newStream.Write(postData, 0, postData.Length);
                newStream.Close();

                //获取返回的结果
                HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                //if (retStr.Length>0)
                if (response.StatusCode == HttpStatusCode.OK)
                    success = true;
                string retStr = reader.ReadToEnd();
                result += "[" + DateTime.Now.ToString() + "]\r\n接收到数据包：\r\n" + retStr + "\r\n\r\n";
                reader.Close();
                response.Close();
                st.Stop();
            }
            catch
            {
                success = false;
            }
            if (success)
                SuccessCount++;
            else
                FailCount++;
            msg.Text = result;
            successC.Text = SuccessCount.ToString();
            responseTime.Text += st.ElapsedMilliseconds + "\r\n";
        }

        //private void button2_Click_1(object sender, EventArgs e)
        //{

        //}
    }
}
