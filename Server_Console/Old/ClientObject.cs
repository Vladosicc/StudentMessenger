//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace Server_Console
//{
//    internal class ClientObject
//    {
//        protected internal string Id { get; } = Guid.NewGuid().ToString();

//        Socket client;
//        ServerObject server; // объект сервера


//        public ClientObject(Socket tcpClient, ServerObject serverObject)
//        {
//            client = tcpClient;
//            server = serverObject;
//        }

//        public async Task ProcessAsync()
//        {
//            try
//            {
//               // получаем имя пользователя
//               // string? userName = await Reader.ReadLineAsync();
//               // string? message = $"{userName} вошел в чат";
//               // посылаем сообщение о входе в чат всем подключенным пользователям
//               //await server.BroadcastMessageAsync(message, Id);
//               // Console.WriteLine(message);
//               // в бесконечном цикле получаем сообщения от клиента
//               // while (true)
//               // {
//               //     try
//               //     {
//               //         message = await Reader.ReadLineAsync();
//               //         if (message == null) continue;
//               //         message = $"{userName}: {message}";
//               //         Console.WriteLine(message);
//               //         await server.BroadcastMessageAsync(message, Id);
//               //     }
//               //     catch
//               //     {
//               //         message = $"{userName} покинул чат";
//               //         Console.WriteLine(message);
//               //         await server.BroadcastMessageAsync(message, Id);
//               //         break;
//               //     }
//               // }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//            finally
//            {
//                // в случае выхода из цикла закрываем ресурсы
//                server.RemoveConnection(Id);
//            }
//        }
//        // закрытие подключения
//        protected internal void Close()
//        {
//            client.Close();
//        }
//    }
//}
