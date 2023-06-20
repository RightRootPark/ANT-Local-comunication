using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class TCPcomm
{
    public void TCPcall(byte i)
    {
        string serverIP = "127.0.0.1";
        int port = 1702; // Q-sys 포트 번호1702
        try
        {
            //TCP 서버에 연결
            TcpClient client = new TcpClient(serverIP, port);
            // 네트워크 스트림 생성
            NetworkStream stream = client.GetStream();
            string message = "sg" + Environment.NewLine;//"클라이언트에서 보낼 메시지";
                                                        // 서버로 데이터 보내기
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            byte[] responseBytes = new byte[1024];

            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine("메시지를 보냈습니다.");
            // 서버로부터 응답 받기
            Console.WriteLine("대기중.");
            int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("대기중..");
            string response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
            Console.WriteLine("서버로부터 받은 응답: " + response);

            // 연결 종료
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
    public string stringtest(string a)
    {
        string serverIP = "127.0.0.1";
        int port = 1702; // Q-sys 포트 번호1702
        try
        {
            //TCP 서버에 연결
            TcpClient client = new TcpClient(serverIP, port);
            // 네트워크 스트림 생성
            NetworkStream stream = client.GetStream();
            string message = "sg" + Environment.NewLine;//"클라이언트에서 보낼 메시지";
                                                        // 서버로 데이터 보내기
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            byte[] responseBytes = new byte[1024];

            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine("메시지를 보냈습니다.");
            // 서버로부터 응답 받기
            Console.WriteLine("대기중.");
            int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("대기중..");
            string response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);
            Console.WriteLine("서버로부터 받은 응답: " + response);

            // 연결 종료
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        return a;
    }
   
}
/*위 코드에서는 BitConverter.GetBytes() 메서드를 사용하여 int 데이터를 바이트 배열로 변환하고,
 * BitConverter.ToString() 메서드를 사용하여 바이트 배열을 16진수 문자열로 변환합니다.
 * 이후 Encoding.UTF8.GetBytes() 메서드를 사용하여 16진수 문자열을 바이트 배열로 변환하고,
 * NetworkStream.Write() 메서드를 사용하여 데이터를 전송합니다.
 */
