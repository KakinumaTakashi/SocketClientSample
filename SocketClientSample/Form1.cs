using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace SocketClientSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TcpClient tcpClient = new TcpClient("192.168.56.1", 8888);
            NetworkStream networkStream = tcpClient.GetStream();

            Encoding encoding = Encoding.UTF8;
            byte[] sendBytes = encoding.GetBytes("KakinumaTakashi" + '\n');

            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Close();
        }
    }
}
