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
        short a = 0;

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
                if (a > 10)
                {
                    a = 0;
                }
                else a++;

            }
        }
        private void Monitoring_Update()
        {

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    switch(a){
                        case 0:
                            //textBox1.Text = Txcommand();
                            break;
                        case 1:
                            //label2.Text = Sbcommand(0,0,1);
                            break;
                        case 2:
                            //textBox_te.Text = PLcommand(1);
                            break;
                        case 3:
                            //label2.Text = Gvcommand();
                            break;
                        case 4:
                            //textBox1.Text = Lccom();
                            break;
                        case 5:
                            textBox_te.Text = Tecommand(0,1,0,1,0,1);
                            break;
                        case 6:
                            break;
                        case 7:
                            break;
                        case 8:
                            break;
                        case 9:
                            break;
                        default:
                            break;                            
                    }
                }
                ));
            }
            else
            {
                Console.WriteLine("Wait");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void text(object sender, PaintEventArgs e)
        {

        }
        string Sbcommand(byte bid0, byte bid1, byte En) //Sb - Enable or disable data blocks > It Work!
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
                // 서버로 데이터 보내기
                byte[] buffer = new byte[8];
                buffer[0] = 02;
                buffer[1] = 08;
                buffer[2] = 0x53;//S
                buffer[3] = 0x62;//b
                buffer[4] = bid0;
                buffer[5] = bid1;
                buffer[6] = En;
                //bcc 붙이기------------------------------------------------------------
                //bcc구하기(단계별 xor) 참고:https://bcc.beyerleinf.de/
                byte[] bTemp1 = buffer;
                int iTemp1 = 0;
                //1. 처음부터 끝까지 xor 계산 
                for (byte i = 0; i < bTemp1.Length; i++)
                {
                    iTemp1 = iTemp1 ^ (int)bTemp1[i];
                    //Console.WriteLine("Temp1: " + iTemp1);
                }
                buffer[7] = 59;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                buffer[7] = (byte)iTemp1;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                //buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC
                byte[] responseBytes = new byte[1024];

                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Sb-send:" + buffer[2] +","+ buffer[3] + "," + buffer[4] + "," + buffer[5] + "," + buffer[6] + "," + buffer[7]);
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                Console.WriteLine("서버로부터 받은 응답: " +responseBytes[2] + "," + responseBytes[3] + "," + responseBytes[4] + "," + responseBytes[5] + "," + responseBytes[6] + "," + responseBytes[7]);

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

        string Tecommand(byte Vx0, byte Vx1, byte Vy0, byte Vy1, byte Va0, byte Va1) //Te -Read the distance from the virtual line sensor to the path with the heading error, the last encountered tag and the recommended velocity
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
                // 서버로 데이터 보내기
                byte[] buffer = {0x02,0x11,0x54,0x65,0,0,0,0,0,0,0 };
                buffer[0] = 0x02;//stx
                buffer[1] = 11;//dec 11
                buffer[2] = 0x54;//(byte)Convert.ToInt32('T');//T
                buffer[3] = 0x65;// (byte)Convert.ToInt32('e');//e
                buffer[4] = Vx0;//Vx0 X component of the vehicle speed in vehicle coordinates, -5000 to 5000 mm/s.
                buffer[5] = Vx1;//Vx1
                buffer[6] =  Vy0;//Vy0 Y component of the vehicle speed in vehicle coordinates, -5000 to 5000 mm/s.
                buffer[7] =Vy0;//Vy1
                buffer[8] =Va0;//Va0 Angular velocity of the vehicle, C000 to 4000h bdeg/s.
                buffer[9] = Va1;//Va1
                                 //bcc 붙이기------------------------------------------------------------
                                 //bcc구하기(단계별 xor) 참고:https://bcc.beyerleinf.de/
                byte[] bTemp1 = buffer;
                int iTemp1 = 0;
                //1. 처음부터 끝까지 xor 계산 
                for (byte i = 0; i < bTemp1.Length; i++)
                {
                    iTemp1 = iTemp1 ^ bTemp1[i];
                    Console.WriteLine("Temp1: " + iTemp1);
                }
                buffer[10] = (byte)iTemp1;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC

                byte[] responseBytes = new byte[1024];
                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Te-send:" + buffer[2] + "," + buffer[3] + "," + buffer[4] + "," + buffer[5] + "," + buffer[6] + "," + buffer[7] + "," + buffer[8] + "," + buffer[9] + "," + buffer[10]);
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                if(responseBytes[2]==80 && responseBytes[3] == 69)
                {
                    Console.WriteLine("Error! class:" +  responseBytes[5] + " Detail:" + responseBytes[7]);
                    response = "Error Te! class:" + responseBytes[5] + " Detail:" + responseBytes[7];
                }
                else
                {
                    response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                    Console.WriteLine("서버로부터 받은 응답: " + response +"=" + responseBytes[4] + "," + responseBytes[5] + "," + responseBytes[7] + "," + responseBytes[8]);
                }
               
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


        string Lccom() //Lc – Cancel Lf command >it work
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
                // 서버로 데이터 보내기 
                byte[] buffer = {0x02,0x05,0x4C,0x63,0x28};
                buffer[0] = 0x02;//STX: Start value (02H).
                buffer[1] = 0x05;
                buffer[2] = 0x4C;//L -Command: Identified by 2 ASCII characters.(1)
                buffer[3] = 0X63;//c -Command: Identified by 2 ASCII characters.(2)
                buffer[4] = 0x28;//BCC

                byte[] responseBytes = new byte[1024];
                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Lc-send." + buffer[2] + "," + buffer[3] + "," + buffer[4]);
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                Console.WriteLine("Recive: " + responseBytes[0] + "," + responseBytes[1] + "," + responseBytes[2] + "," + responseBytes[3] + "," + responseBytes[4] );


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
        //Tx=Abort the current patn
        string Txcommand() //Tx - Abort the current path > 작동 잘됨
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
                string message = "2";//"클라이언트에서 보낼 메시지"; 02 05 4c 63 28 = 
                // 서버로 데이터 보내기 
                byte[] buffer = { 0x02, 0x05, 0x54, 0x78, 0x2B };
                //buffer[0] = 02;//STX: Start value (02H).
                ////buffer[1] = 08;
                //buffer[2] = 76;//Command: Identified by 2 ASCII characters.(1)
                //buffer[3] = 99;//Command: Identified by 2 ASCII characters.(2)
                //buffer[4] = 28;//BCC
                //buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC
                byte[] responseBytes = new byte[1024];

                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("메시지를 보냈습니다." + buffer.Length);
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("대기중..");
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                Console.WriteLine("Recive: " + responseBytes[0] + "," + responseBytes[1] + "," + responseBytes[2] + "," + responseBytes[3] + "," + responseBytes[4] + "," + responseBytes[5] + "," + responseBytes[6] + "," + responseBytes[7] + "," + responseBytes[8] + "," + responseBytes[9] + "," + responseBytes[10]);
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

        string Gvcommand() // Get communication protocol version 잘됨
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
                // 서버로 데이터 보내기 
                byte[] buffer = { 0x02, 0x05, 0x47, 0x76, 0x36 };
                //buffer[0] = 02;//STX: Start value (02H).
                ////buffer[1] = 08;
                //buffer[2] = 76;//Command: Identified by 2 ASCII characters.(1)
                //buffer[3] = 99;//Command: Identified by 2 ASCII characters.(2)
                //buffer[4] = 28;//BCC
                //buffer[1] = (byte)buffer.Length; //Message length: Number of bytes in message, including STX and BCC
                byte[] responseBytes = new byte[1024];
                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Gv-send" + buffer.Length);
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead) + " Revision:" + responseBytes[4] + "," + responseBytes[5] + "," + responseBytes[6] + "," + responseBytes[7] + " Protocol Version:" + responseBytes[8] + "," + responseBytes[9] + "," + responseBytes[10] + "," + responseBytes[11];
                Console.WriteLine("Recive: " + responseBytes[0] + "," + responseBytes[1] + "," + responseBytes[2] + "," + responseBytes[3] + ",Revision:" + responseBytes[4] + "," + responseBytes[5] + "," + responseBytes[6] + "," + responseBytes[7] + ",Protocol Version:" + responseBytes[8] + "," + responseBytes[9] + "," + responseBytes[10] + "," + responseBytes[11]);
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
        string MessageStructure(string command, byte[] Datablock)
        {
            /* STX(1byte) + Message Length(1) + Command(2) + Datablock(n byte) + Bcc(1)
             * STX: Start value (02H)=(0x02).
             * Message length: Number of bytes in message, including STX and BCC.
             * Command: Identified by 2 ASCII characters.
             * Data block: All data needed in the message.
             * BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
             */
            //stx 선언 (Message length까지 임의로 추가)-----------------------------
            byte[] Message = {0x02, 0x02};
            //command 붙이기------------------------------------------------------
            byte[] AddCommand = Encoding.ASCII.GetBytes(command);
            Message = Message.Concat(AddCommand).ToArray();
            //Data block 붙이기-----------------------------------------------------
            if (command == "Tx" || command == "Gv" || command == "Lc") ;
            else
            {
                Message = Message.Concat(Datablock).ToArray();
            }
            // Message length 업데이트----------------------------------------------
            Message[1] = (byte)Message.Length;
            Message[1]++;
            Console.WriteLine("SandB: " + Message[0] + "," + Message[1] + "," + Message[2] + "," + Message[3] );

            //bcc 붙이기------------------------------------------------------------
            //bcc구하기(단계별 xor) 참고:https://bcc.beyerleinf.de/
            byte[] bTemp1 = Message;
            int iTemp1 = 0;
            //1. 처음부터 끝까지 xor 계산 
            for (byte i = 0; i < bTemp1.Length; i++) 
            {
                iTemp1 = iTemp1 ^ (int)bTemp1[i];
                Console.WriteLine("Temp: " + iTemp1 );
            }
            byte[] iTemp4 = {(byte)iTemp1 };
            Message = Message.Concat(iTemp4).ToArray();
            Console.WriteLine("Sand: " + Message[0] + ","+Message[1]+"," + Message[2] + "," + Message[3] + "," + Message[4] + "=" + Message.Length);
            
            //반환
            string response = Encoding.ASCII.GetString(Message,0, Message.Length);
            Console.WriteLine("Response: " + response);

            return response;
        }

        string PLcommand(byte Mapid) //PL - Select the current map > it work
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
                // 서버로 데이터 보내기
                byte[] buffer = {0x02,0x06,0,0,0,0 };
                  buffer[2] = 0x50;//P
                buffer[3] = 0x4C;//L
                buffer[4] = Mapid;
                //bcc 붙이기------------------------------------------------------------
                //bcc구하기(단계별 xor) 참고:https://bcc.beyerleinf.de/
                byte[] bTemp1 = buffer;
                int iTemp1 = 0;
                //1. 처음부터 끝까지 xor 계산 
                for (byte i = 0; i < bTemp1.Length; i++)
                {
                    iTemp1 = iTemp1 ^ (int)bTemp1[i];
                    //Console.WriteLine("Temp1: " + iTemp1);
                }
                buffer[5] = (byte)iTemp1;//BCC: Checksum calculated by performing a byte-oriented XOR operation over all the bytes in the message packet, including STX
                byte[] responseBytes = new byte[1024];

                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("PL-send:" + buffer[2] + "," + buffer[3] + "," + buffer[4] + "," + buffer[5] );
                // 서버로부터 응답 받기
                Console.WriteLine("대기중.");
                int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
                if (responseBytes[2] == 80 && responseBytes[3] == 69)
                {
                    Console.WriteLine("Error! class:" + responseBytes[5] + " Detail:" + responseBytes[7]);
                    response = "Error! class:" + responseBytes[5] + " Detail:" + responseBytes[7];
                }
                else
                {
                    response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
                    Console.WriteLine("서버로부터 받은 응답: " + responseBytes[2] + "," + responseBytes[3] + "," + responseBytes[4] + "," + responseBytes[5]);
                }

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
        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
