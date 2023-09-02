using Server.ItSelf;

namespace Server
{
    class Programm
    {
        static void Main(string[] args)
        {
            ServerHost host = new ServerHost(new StaticFileHandler(Path.Combine(Environment.CurrentDirectory, "www")));
            host.Start();
        }
    }
}