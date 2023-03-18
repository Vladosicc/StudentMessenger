using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Client_WPF.Windows;
using NetDataLibrary.NetworkData;
using NetDataLibrary.NetworkStatuses;
using NetDataLibrary.Sockets;
using NetDataLibrary.UserInfos;

namespace Client_WPF.Network
{
    public class AppNetwork
    {
        const int StreamCapacity = 1024;

        public delegate void NetworkStatusChanged(ServerStatus status, IPNetData data);

        public event NetworkStatusChanged OnNetworkStatusChanged;

        public delegate void ClientStatusChanged(ushort recID, string userLogin, ClientStatus status);

        public event ClientStatusChanged OnClientStatusChanged;

        public delegate void ClientReceivedMessage(ushort recepientID, ushort receiverID, string message);

        public event ClientReceivedMessage OnClientReceivedMessage;

        private TcpClient SocketClient { get; set; } = new TcpClient();

        public IPNetData MainData { get; set; }

        public bool? IsConnected
        {
            get
            {
                if (MainData == null || SocketClient == null)
                    return null;
                return
                    SocketClient?.Connected;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (MainData == null)
                    return true;
                return MainData.IsEmpty;
            }
        }

        public AppNetwork()
        {

        }

        MemoryStream MemStream = null;
        private string UserName;

        private AppNetwork(IPNetData data)
        {
            MainData = data;
            //OpenSocketWithData(userName);
        }

        private AppNetwork(string ipPort)
        {
            MainData = new IPNetData(ipPort);
            //OpenSocketWithData(userName);
        }

        private AppNetwork(string ip, ushort port)
        {
            MainData = new IPNetData(ip, port);
            //OpenSocketWithData(userName);
        }

        public async Task OpenSocketWithData(string userName)
        {
            if (IsConnected.HasValue && !IsConnected.Value)
            {
                UserName = userName;
                //OnNetworkStatusChanged?.Invoke(ServerStatus.TryToConnect, MainData);
                SocketClient.BeginConnect(IPAddress.Parse(MainData.IP), (int)MainData.Port, new AsyncCallback(ConnectCallback), SocketClient);
            }
        }

        private async void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                
                SocketClient.EndConnect(ar);
                //SocketClient = ar.AsyncState as TcpClient;
                //var userName = (string)ar.AsyncState;
                SendOperationInfo(CodeOperations.ConnectClient, UserName);
                //MemStream = new MemoryStream(new byte[StreamCapacity], 0, StreamCapacity, true, true);
                OnNetworkStatusChanged?.Invoke(ServerStatus.Connected, MainData);
                StartReceive(SocketClient.Client);
            }
            catch (Exception ex)
            {
                DisconnectClient(ex);
            }
        }

        private async Task StartReceive(Socket sock)
        {
            await Task.Run(
            async () =>
            {
                try
                {
                    if (!sock.Connected)
                        return;
                    string request = SocketFunction.GetFullStringUnicode(sock);
                    //Console.WriteTo(DateTime.Now.ToLongTimeString() + " Command: " + request);
                    await DoTasks(sock, request);
                }
                catch (Exception ex)
                {
                    DisconnectClient(ex);
                    MessageBox.Show("Ошибка. " + ex.Message);
                }
            });
            if (!sock.Connected)
                return;
            StartReceive(sock);
        }

        private void SendOperationInfo(CodeOperations operation, object netdata)
        {
            NetData nd = new NetData()
            {
                CodeOperation = operation,
                Data = netdata
            };
            byte[] val = JsonNetworkConvert.SerializeObjectToSend(nd);
            try
            {
                SocketClient.Client?.Send(val);
            }
            catch (Exception ex)
            {
                if (operation != CodeOperations.DisconnectClient)
                    DisconnectClient(ex);
            }
        }

        public void DisconnectClient(Exception ex = null)
        {
            SendOperationInfo(CodeOperations.DisconnectClient, null);
            if (ex != null)
                OnNetworkStatusChanged?.Invoke(ServerStatus.Aborted, MainData);
            else
                OnNetworkStatusChanged?.Invoke(ServerStatus.Disconnected, MainData);
            SocketClient?.Dispose();
        }


        private async Task DoTasks(Socket sock, string Request)
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
                        string servInfo = netData.Data.ToString();
                        ServerUserInfo userInfo = JsonNetworkConvert.DeserializeObject(servInfo, typeof(ServerUserInfo)) as ServerUserInfo;
                        if (userInfo == null)
                            return;
                        OnClientStatusChanged?.Invoke(userInfo.UserIndex, userInfo.UserName, ClientStatus.Connected);
                        break;
                    }
                case CodeOperations.LoadChat:
                    {
                        break;
                    }
                case CodeOperations.SendMessage:
                    {
                        MessageData message = netData.Data as MessageData;
                        //SendMessageToReceiver(user, message);
                        break;
                    }
                case CodeOperations.DisconnectClient:
                    {
                        string servInfo = netData.Data.ToString();
                        ServerUserInfo userInfo = JsonNetworkConvert.DeserializeObject(servInfo, typeof(ServerUserInfo)) as ServerUserInfo;
                        OnClientStatusChanged?.Invoke(userInfo.UserIndex, userInfo.UserName, ClientStatus.Disconnected);
                        break;
                    }
                default:
                    {
                        //SendDataToUser(user, TaskError);
                        //SendDataToUser(user, Encoding.Unicode.GetBytes("Undefined command"));
                        //s.Send(TaskError);
                        //s.Send(Encoding.Unicode.GetBytes("Undefined command"));
                        return;
                    }

            }
        }

        public static AppNetwork OpenConnectionTo(IPNetData data, string userName)
        {
            return new AppNetwork(data);
        }

        public static AppNetwork OpenConnectionTo(string ipPort)
        {
            return new AppNetwork(ipPort);
        }


    }

}
