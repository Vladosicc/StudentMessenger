using System.Net.Sockets;

using NetDataLibrary.NetworkStatuses;

using Console = Server_Console.ServerConsole;

namespace Server_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ServerObject server = new ServerObject();// создаем сервер
            //Console.WriteLine($"Start server\nListen on 127.0.0.1:8888");
            //server.ListenAsync().Wait();
            Server_Net server = null;
            try
            {
                server = new Server_Net("127.0.0.1", 8888);
                if (server.ServerStatus == ServerStatuses.Initialized)
                {
                    server.OpenConnections();
                }  
                while (true)
                {
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteTo("Ошибка. " + ex.Message);
                Console.ReadKey();
            }
            finally
            {
                server?.Dispose();
            }
        }
    }
}