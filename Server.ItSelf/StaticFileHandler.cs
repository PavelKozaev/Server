using System.Net;

namespace Server.ItSelf
{
    internal record Request(string Path, HttpMethod Method);

    internal static class RequestParser
    {
        public static Request Parse(string header)
        {
            var split = header.Split(" ");
            return new Request(split[1], GetMethod(split[0]));
        }

        private static HttpMethod GetMethod(string method)
        {
            if (method == "GET")
                return HttpMethod.Get;
            return HttpMethod.Post;
            
        }
    }

    internal static class ResponseWriter
    {
        public static void WriteStatus(HttpStatusCode code, Stream stream)
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            writer.WriteLine($"HTTP/1.0 {(int)code} {code}");
            writer.WriteLine();
        }
    }

    public class StaticFileHandler : IHandler
    {
        private readonly string _path;
        public StaticFileHandler(string path)
        {
            _path = path;            
        }
        public void Handle(Stream networkStream)
        {
            using (var reader = new StreamReader(networkStream))
            using (var writer = new StreamWriter(networkStream))
            {
                var firstLine = reader.ReadLine();
                for (string line = null; line != string.Empty; line = reader.ReadLine()) ;

                var request = RequestParser.Parse(firstLine);

                var filePath = Path.Combine(_path, request.Path.Substring(1));

                if (!File.Exists(filePath))
                {
                    //TODO: write 404
                    ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
                }
                else
                {
                    ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(networkStream);
                    }
                }

                Console.WriteLine(filePath);                
            }
        }
    }
}