using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Console = Server_Console.ServerConsole;

using NetDataLibrary;
using Server_Console.UserData;
using NetDataLibrary.Sockets;
using Newtonsoft.Json;
using NetDataLibrary.NetworkStatuses;
using NetDataLibrary.NetworkData;
using System.Net;

namespace Server_Console
{
    internal class Server_Net : IDisposable
    {
        public string IP { get; private set; }

        public int Port { get; private set; }

        public ServerStatuses ServerStatus { get; internal set; }

        private TcpListener MainListener { get; set; }

        private ChatUserData ServerUsers { get; set; } = new ChatUserData();

        public Server_Net(string ip, int port)
        {
            IP = ip;
            Port = port;
            ServerStatus = ServerStatuses.Initialized;
            Console.WriteTo("Сервер был инициализирован.");
            Console.WriteTo("IP: " + IP + ":" + Port);
        }


        public void Dispose()
        {
            MainListener.Stop();
        }

        internal void OpenConnections()
        {
            if (ServerStatus != ServerStatuses.Initialized)
                throw new Exception("Ошибка. ");
            BeginAcceptingUsers();
        }

        private void BeginAcceptingUsers()
        {
            if (MainListener == null)
            {
                MainListener = new TcpListener(IPAddress.Any, Port);
                MainListener.Start();
                Console.Title = "Чат-сервер: (ожидание пользователей) - " + IP + ":" + Port;
                Console.WriteTo("Ожидание подключений от пользователей.");
            }
            MainListener.BeginAcceptSocket(new AsyncCallback(ClientAccepted), this);
        }

        private void ClientAccepted(IAsyncResult ar)
        {
            try
            {
                var sock = MainListener.EndAcceptSocket(ar);
                if (sock != null)
                {
                    ChatUser us = ServerUsers.AddUser(sock);
                    Console.WriteTo("Был подключен пользователь (ID = " + us.Info.UserIndex + ")");
                    BeginReceiveDataFrom(us);
                }
                BeginAcceptingUsers();
            }
            catch (Exception ex)
            {
                Console.WriteTo("Ошибка. ", ex.Message);
            }
        }

        private async Task BeginReceiveDataFrom(ChatUser us)
        {
            await Task.Run(
            async () =>
            {
                try
                {
                    if (!us.Socket.Connected)
                        return;
                    string request = SocketFunction.GetFullStringUnicode(us.Socket);
                    //Console.WriteTo(DateTime.Now.ToLongTimeString() + " Command: " + request);
                    await DoTasks(us, request);
                }
                catch (Exception ex)
                {
                    ServerUsers.RemoveUserBy(us.Info.UserIndex);
                    Console.WriteTo("Пользователь [" + us.Info.UserName + "] был отключён из чата..");

                    //Console.WriteTo(ex.Message);
                }
            });
            await BeginReceiveDataFrom(us);
        }

        private async Task DoTasks(ChatUser user, string Request)
        {
            byte[] TaskEndingSuccessfully = BitConverter.GetBytes(0);
            byte[] TaskError = BitConverter.GetBytes(-1);

            CodeOperations typeOperation = CodeOperations.Undefined;
            NetData netData = JsonNetworkConvert.DeserializeObject(Request, typeof(NetData)) as NetData;
            if (netData == null)
                return;
            typeOperation = netData.CodeOperation;
            switch (typeOperation)
            {
                case CodeOperations.ConnectClient:
                    {
                        string userName = netData.Data as string;
                        user.Info.ChangeUserName(userName);
                        //ServerUsers.ChangeUserName(user, userName);
                        string value = $"Пользователь [{userName}] подключился и отправил свои данные на сервер..";
                        Console.WriteTo(value);
                        //string userInfo = JsonNetworkConvert.SerializeObject(user.Info);
                        ServerUsers.SendDataAboutConnectedUsersTo(user);
                        break;
                    }
                case CodeOperations.LoadChat:
                    {
                        break;
                    }
                case CodeOperations.SendMessage:
                    {
                        MessageData message = netData.Data as MessageData;
                        SendMessageToReceiver(user, message);
                        break;
                    }
                case CodeOperations.DisconnectClient:
                    {
                        Console.WriteTo("Пользователь (" + user.Info.UserName + ") вышел из чата..");
                        string userInfo = JsonNetworkConvert.SerializeObject(user.Info);
                        NetData nd = new NetData()
                        {
                            CodeOperation = CodeOperations.DisconnectClient,
                            Data = userInfo
                        };
                        byte[] bytes = JsonNetworkConvert.SerializeObjectToSend(nd);
                        SendDataToAllUsers(bytes);
                        ServerUsers.RemoveUserBy(user.Info.UserIndex);
                        break;
                    }
                default:
                    {
                        SendDataToUser(user, TaskError);
                        SendDataToUser(user, Encoding.Unicode.GetBytes("Undefined command"));
                        //s.Send(TaskError);
                        //s.Send(Encoding.Unicode.GetBytes("Undefined command"));
                        return;
                    }

            }
        }

        private void SendMessageToReceiver(ChatUser userFrom, MessageData? message)
        {
            ServerUsers.SendMessageToUser(userFrom, message);
        }

        private void SendDataToAllUsers(byte[] request, ChatUser sender = null)
        {
            ServerUsers.SendDataToAllUsers(request, sender);
        }

        private void SendDataToUser(ChatUser user, byte[] data)
        {
            ServerUsers.SendDataToUser(user, data);
        }

        private void SendDataToUser(ChatUser user, string request)
        {
            ServerUsers.SendDataToUser(user, request);
        }

    }
}
