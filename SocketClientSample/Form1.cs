using SocketServerSample.util;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketClientSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Thread reciveThread;
        private System.Timers.Timer senderTimer;

        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private int count = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            Log.d("Form1_Load");

            // ソケットオープン
            tcpClient = new TcpClient("192.168.56.1", 8888);
            networkStream = tcpClient.GetStream();

            // 受信スレッド
            reciveThread = new Thread(reciver);
            reciveThread.Start();
            Log.i("受信スレッド起動");

            // 送信タイマー
            senderTimer = new System.Timers.Timer(1000);
            senderTimer.Elapsed += senderTimer_Elapsed;
            senderTimer.Start();
            Log.i("送信タイマー起動");


            //Encoding encoding = Encoding.UTF8;

            ////サーバーから送られたデータを受信する
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            //byte[] resBytes = new byte[256];
            //int resSize = 0;
            //do
            //{
            //    //データの一部を受信する
            //    resSize = networkStream.Read(resBytes, 0, resBytes.Length);
            //    //Readが0を返した時はサーバーが切断したと判断
            //    if (resSize == 0)
            //    {
            //        Console.WriteLine("サーバーが切断しました。");
            //        break;
            //    }
            //    //受信したデータを蓄積する
            //    memoryStream.Write(resBytes, 0, resSize);
            //    //まだ読み取れるデータがあるか、データの最後が\nでない時は、
            //    // 受信を続ける
            //} while (networkStream.DataAvailable || resBytes[resSize - 1] != '\n');
            ////受信したデータを文字列に変換
            //string resMsg = encoding.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            //memoryStream.Close();
            ////末尾の\nを削除
            //resMsg = resMsg.TrimEnd('\n');
            //Console.WriteLine(resMsg);


            //byte[] sendBytes = encoding.GetBytes(resMsg + '\n');

            //networkStream.Write(sendBytes, 0, sendBytes.Length);
            //networkStream.Close();
        }

        /// <summary>
        /// 送信タイマースレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void senderTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Log.d("senderTimer_Elapsed");

            Encoding encoding = Encoding.UTF8;

            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            byte[] sendBytes = encoding.GetBytes(timestamp + " " + count + '\n');

            networkStream.Write(sendBytes, 0, sendBytes.Length);
            //networkStream.Close();
        }

        /// <summary>
        /// 受信スレッド
        /// </summary>
        private void reciver()
        {
            Log.d("reciver");

            Encoding encoding = Encoding.UTF8;
            MemoryStream memoryStream = null;

            try
            {
                while (true)
                {
                    //サーバーから送られたデータを受信する
                    memoryStream = new MemoryStream();
                    byte[] resBytes = new byte[256];
                    int resSize = 0;
                    do
                    {
                        //データの一部を受信する
                        resSize = networkStream.Read(resBytes, 0, resBytes.Length);
                        //Readが0を返した時はサーバーが切断したと判断
                        //if (resSize == 0)
                        //{
                        //    Console.WriteLine("サーバーが切断しました。");
                        //    break;
                        //}
                        //受信したデータを蓄積する
                        memoryStream.Write(resBytes, 0, resSize);
                        //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                        // 受信を続ける
                    } while (networkStream.DataAvailable || resBytes[resSize - 1] != '\n');
                    //受信したデータを文字列に変換
                    string resMsg = encoding.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    memoryStream.Close();
                    memoryStream = null;
                    //末尾の\nを削除
                    resMsg = resMsg.TrimEnd('\n');
                    Log.i("受信データ:" + resMsg);
                    File.AppendAllText("SocketClientSample.log", resMsg + "\n");
                }
            }
            catch (ThreadAbortException e)
            {
                if (memoryStream != null) memoryStream.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.d("Form1_FormClosed");

            // 受信スレッド停止要求
            reciveThread.Abort();
            // 送信スレッド停止
            senderTimer.Stop();

            networkStream.Close();
            tcpClient.Close();
        }
    }
}
