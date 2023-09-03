using Server.ItSelf;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerHost host = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
            host.StartV2();
        }
    }
}