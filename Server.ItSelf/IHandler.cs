namespace Server.ItSelf
{
    public interface IHandler
    {
        void Handle(Stream stream, Request request);
    }
}