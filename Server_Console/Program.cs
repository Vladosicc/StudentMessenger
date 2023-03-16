using System.Net.Sockets;

namespace Server_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerObject server = new ServerObject();// создаем сервер
            Console.WriteLine($"Start server\nListen on 127.0.0.1:8888");
            server.ListenAsync().Wait();
        }
    }
}