﻿using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;

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
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected");
                var _ = ProcessClientAsync(client);
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
            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = await reader.ReadLineAsync();
                    Console.WriteLine($"Request: {firstLine}");
                    for (string line = null; line != string.Empty; line = await reader.ReadLineAsync()) ;

                    var request = RequestParser.Parse(firstLine);
                    Console.WriteLine($"Parsed request: {JsonConvert.SerializeObject(request)}");
                    await _handler.HandleAsync(stream, request);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}