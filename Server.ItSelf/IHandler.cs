namespace Server.ItSelf
{
    public interface IHandler
    {
        void Handle(Stream stream, Request request);
        Task HandleAsync(Stream stream, Request request);
    }
}