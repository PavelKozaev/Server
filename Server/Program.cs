using Server.ItSelf;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServerHost host = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
            await host.StartAsync();
        }
    }
}