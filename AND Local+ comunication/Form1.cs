using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AND_Local__comunication
{    
    public partial class Form1 : Form
    {
        public TCPcomm commTcp { get; private set; }
        private Thread monitoring;
        private string QsysMessage;

        public Form1()
        {
            InitializeComponent();
            Awake();
        }
        void Awake()
        {

            //Thread - UI update
            if (monitoring == null)
            {
                monitoring = new Thread(Run);
                monitoring.IsBackground = true;
                monitoring.Start();
            }
        }
        void Run()
        {
            while (true)
            {
                Monitoring_Update();
                Thread.Sleep(50);//50ms
                                  // 50밀리초 동안 대기 Thread.Sleep(50) 메서드를 사용하여 50밀리초마다 대기하도록 설정했으므로, 전송 주기는 20Hz

            }
        }
        private void Monitoring_Update()
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    textBox1.Text = "Recive=" + QsysMessage;
                    label2.Text = "Recive=" + QsysMessage;
                }
                ));
            }
            else
            {

            }
            TCPcall();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void text(object sender, PaintEventArgs e)
        {

        }
        string TCPcall()
        {
            string serverIP = "192.168.0.102";
            int port = 1566; // ANT 포트 번호1566
            string response="Non";
            try
            {
                //TCP 서버에 연결
                TcpClient client = new TcpClient(serverIP, port);
                // 네트워크 스트림 생성
                NetworkStream stream = client.GetStream();
                string message = "8Sb0113b" + Environment.NewLine;//"클라이언트에서 보낼 메시지"; 02 08 53 62 00 01 01 3B
                // 서버로 데이터 보내기
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                buffer[0] = 02;
                //buffer[1] = 08;
                buffer[2] = 83;
                buffer[3] = 98;
                buffer[4] = 00;
                buffer[5] = 01;
                buffer[6] = 01;
                //buffer[7] = 59;
                int i = (int)buffer.Length;
                Console.WriteLine("??"+i);
                buffer[i] = 59;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC
                byte[] responseBytes = new byte[1024];

                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("메시지를 보냈습니다.");
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("대기중..");
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                QsysMessage = response;
                Console.WriteLine("서버로부터 받은 응답: " + response);

                // 연결 종료
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return response;
        }

        string TCPcalltest()
        {
            string serverIP = "192.168.0.102";
            int port = 1566; // ANT 포트 번호1566
            string response = "Non";
            try
            {
                //TCP 서버에 연결
                TcpClient client = new TcpClient(serverIP, port);
                // 네트워크 스트림 생성
                NetworkStream stream = client.GetStream();
                string message = "8Sb0113b" + Environment.NewLine;//"클라이언트에서 보낼 메시지"; 02 08 53 62 00 01 01 3B
                // 서버로 데이터 보내기
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                buffer[0] = 02;
                //buffer[1] = 08;
                buffer[2] = 83;
                buffer[3] = 98;
                buffer[4] = 00;
                buffer[5] = 01;
                buffer[6] = 01;
                buffer[7] = 59;

                buffer[7] = 59;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC

                List<byte> list = new List<byte>();
                list.AddRange(buffer);


                byte[] responseBytes = new byte[1024];
                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("메시지를 보냈습니다.");
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("대기중..");
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                QsysMessage = response;
                Console.WriteLine("서버로부터 받은 응답: " + response);

                // 연결 종료
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return response;
        }


        string TWcall()
        {
            string serverIP = "192.168.0.102";
            int port = 1566; // ANT 포트 번호1566
            string response = "Non";
            try
            {
                //TCP 서버에 연결
                TcpClient client = new TcpClient(serverIP, port);
                // 네트워크 스트림 생성
                NetworkStream stream = client.GetStream();
                string message = "8Sb0113b" + Environment.NewLine;//"클라이언트에서 보낼 메시지"; 02 11 54 77 01 01 01 01 01 01 30
                // 서버로 데이터 보내기 
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                buffer[0] = 02;//STX: Start value (02H).
                //buffer[1] = 08;
                buffer[2] = 83;//Command: Identified by 2 ASCII characters.(1)
                buffer[3] = 98;//Command: Identified by 2 ASCII characters.(2)
                buffer[4] = 00;//Data block: All data needed in the message
                buffer[5] = 01;//Data block: All data needed in the message
                buffer[6] = 01;//Data block: All data needed in the message
                buffer[(byte)buffer.Length+1] = 59;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC
                byte[] responseBytes = new byte[1024];

                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("메시지를 보냈습니다.");
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("대기중..");
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                QsysMessage = response;
                Console.WriteLine("서버로부터 받은 응답: " + response);

                // 연결 종료
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return response;
        }
    }
}
