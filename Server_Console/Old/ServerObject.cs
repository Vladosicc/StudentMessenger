//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using System.Net.Http;
//using NetDataLibrary;
//using Newtonsoft.Json;

//namespace Server_Console
//{
//    internal class ServerObject
//    {
//        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005);
//        Socket listener = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // сервер для прослушивания
//        List<ClientObject> clients = new List<ClientObject>(); // все подключения

//        protected internal void RemoveConnection(string id)
//        {
//            // получаем по id закрытое подключение
//            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
//            // и удаляем его из списка подключений
//            if (client != null) clients.Remove(client);
//            client?.Close();
//        }

//        // прослушивание входящих подключений
//        protected internal static async Task ListenAsync()
//        {
//            try
//            {
//                //Слушаем клиента
//                listener.Bind(ip);
//                listener.Listen(0);
//                Console.WriteLine("Сервер запущен. Ожидание подключений...");

//                while (true)
//                {
//                    Socket s = listener.Accept();
//                    try
//                    {
//                        ClientObject clientObject = new ClientObject(s, this);
//                        clients.Add(clientObject);
//                        Task.Run(clientObject.ProcessAsync);
//                        //Получили запрос
//                        ReciveMessage(s);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine(ex);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            finally
//            {
//                Disconnect();
//            }
//        }

//        static async Task ReciveMessage(Socket s)
//        {
//            await Task.Run(async () =>
//            {
//                try
//                {
//                    string request = SocketFunction.GetFullStringUnicode(s);
//                    Console.WriteLine(DateTime.Now.ToShortTimeString() + " Command: " + request);
//                    await DoTasks(s, request);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.Message);
//                }
//            });
//        }

//        ClientObject FindClientObjectBySocket(Socket sock)
//        {
//            if (sock == null || clients.Count == 0)
//                return null;
//            return clients.FirstOrDefault(s => s.)

//        }

//        static async Task DoTasks(Socket s, string Request)
//        {
//            byte[] TaskEndingSuccessfully = BitConverter.GetBytes(0);
//            byte[] TaskError = BitConverter.GetBytes(-1);

//            CodeOperations typeOperation = CodeOperations.Undefined;
//            string JSData = Request;
//            NetData nd = JsonConvert.DeserializeObject(JSData) as NetData;
//            if (nd == null)
//                return;
//            typeOperation = nd.CodeOperation;
//            switch (typeOperation)
//            {
//                case CodeOperations.ConnectClient:
//                {
//                    string userName = nd.Data as string;
//                    break;
//                }
//                default:
//                {
//                    s.Send(TaskError);
//                    s.Send(Encoding.Unicode.GetBytes("Undefined command"));
//                    return;
//                }

//            }
//        }

//        // отключение всех клиентов
//        protected internal void Disconnect()
//        {
//            foreach (var client in clients)
//            {
//                client.Close(); //отключение клиента
//            }
//            listener.Close(); //остановка сервера
//        }
//    }
//}
