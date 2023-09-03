using System.Net.Sockets;
using System.Net;
using System.Reflection.PortableExecutable;

namespace Server.ItSelf
{
    public class ServerHost
    {
        private readonly IHandler _handler;
        public ServerHost(IHandler handler)
        {

            _handler = handler;

        }
        public void StartV1()
        {
            Console.WriteLine("Server Started V1");
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                try
                {
                    using (var client = listener.AcceptTcpClient())
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var firstLine = reader.ReadLine();
                        for (string line = null; line != string.Empty; line = reader.ReadLine()) ;

                        var request = RequestParser.Parse(firstLine);
                        _handler.Handle(stream, request);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }                
            }
        }

        public void StartV2()
        {
            Console.WriteLine("Server Started V2");
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                ProcessClient(client);                
            }
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Server Started Async");
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    await ProcessClientAsync(client);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }                
            }
        }

        private void ProcessClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    using (client)
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var firstLine = reader.ReadLine();
                        for (string line = null; line != string.Empty; line = reader.ReadLine()) ;

                        var request = RequestParser.Parse(firstLine);
                        _handler.Handle(stream, request);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex);
                }                
            });            
        }

        private async Task ProcessClientAsync(TcpClient client)
        {            
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = await reader.ReadLineAsync();
                    for (string line = null; line != string.Empty; line = await reader.ReadLineAsync()) ;

                    var request = RequestParser.Parse(firstLine);
                    await _handler.HandleAsync(stream, request);
                }            
        }
    }
}