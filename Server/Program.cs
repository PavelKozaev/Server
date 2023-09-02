using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Programm
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            var client = listener.AcceptTcpClient();
            using (var stream = client.GetStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    while (true)
                    {
                        Console.WriteLine(reader.ReadLine());
                    }
                }
            }
        }
    }
}