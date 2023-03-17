using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NetDataLibrary;

using Newtonsoft.Json;

namespace Client_WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppNetwork Network { get; set; } = new AppNetwork();

        public ClientsData ClData { get; set; } = new ClientsData();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            LB_Main_Chats.ItemsSource = ClData.Clients;     
        }

        private void UI_NetStatusChanged(ServerStatus status, IPNetData data)
        {
            Dispatcher.Invoke(() => UI_ChangeStatus(status, data));
        }

        private void UI_ChangeStatus(ServerStatus status, IPNetData data)
        {
            if (data != null)
            {
                switch (status)
                {
                    case ServerStatus.Connected:
                        {
                            Title = "Мессенджер - Клиент (подключено). IP: " + data.ToString();
                            break;
                        }
                    case ServerStatus.TryToConnect:
                        {
                            Title = "Мессенджер - Клиент (попытка подключения). IP: " + data.ToString();
                            break;
                        }
                    case ServerStatus.Disconnected:
                        {
                            Title = "Мессенджер - Клиент (отключён). IP: " + data.ToString();
                            break;
                        }
                    case ServerStatus.Aborted:
                        {
                            Title = "Мессенджер - Клиент (оборвано). IP: " + data.ToString();
                            break;
                        }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TryConnectToServer();
        }

        private void TryConnectToServer()
        {
            if (Network.IsEmpty)
            {
                Window_IPEntering w_Ip = new Window_IPEntering();
                w_Ip.ShowDialog();
                if (w_Ip.DialogResult == true)
                {
                    try
                    {
                        Network = AppNetwork.OpenConnectionTo(w_Ip.IpPort);
                        Network.OnNetworkStatusChanged += UI_NetStatusChanged;
                        Network.OnClientStatusChanged += Network_OnClientStatusChanged;
                        Network.OnClientReceivedMessage += Network_OnClientReceivedMessage;
                        Network.OpenSocketWithData(w_Ip.UserName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка" + ex.Message, "Ошибка.");
                    }
                }
            }
        }

        private void Network_OnClientReceivedMessage(ushort recepientID, ushort receiverID, string message)
        {
            ClData.AddMessageToChat(recepientID, receiverID, message);
        }

        private void Network_OnClientStatusChanged(ushort recID, string userLogin, ClientStatus status)
        {
            if (status == ClientStatus.Connected)
            {
                try
                {
                    ClData.AddClient(new Client(recID, userLogin));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка." + ex.Message, "Произошла ошибка.");
                }
            }
            else if (status == ClientStatus.Disconnected)
            {
                try
                {
                    if (!ClData.RemoveClient(recID))
                    {
                        MessageBox.Show("Ошибка при удалении клиента с ID - " + recID + ". ", "Произошла ошибка.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка. " + ex.Message, "Произошла ошибка.");
                }

            }
        }

        private void LB_Main_Chats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = LB_Main_Chats.SelectedIndex;
            try
            {
                var client = ClData.FindClientWithIndex(index);
                if (client != null)
                {
                    LB_Main_Messages.ItemsSource = client.Messages;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка. " + ex.Message);
            }
            
        }

        private void MI_Server_ChangeServ_Click(object sender, RoutedEventArgs e)
        {
            DisconnectClient();
            TryConnectToServer();
        }

        private void MI_Server_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            DisconnectClient();
        }

        private void DisconnectClient()
        {
            Network.DisconnectClient();
            Network.OnNetworkStatusChanged -= UI_NetStatusChanged;
            Network.OnClientStatusChanged -= Network_OnClientStatusChanged;
            Network.OnClientReceivedMessage -= Network_OnClientReceivedMessage;
        }
    }

    public class ClientsData
    {
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>();

        public ClientsData ()
        {
            Clients = new ObservableCollection<Client>();
        }

        public void AddClient(Client client)
        {
            if (FindClientWithID(client.ClientID) == null)
            {
                Clients.Add(client);
            }
            else
                throw new Exception("Пользователь с ID = " + client.ClientID + "уже существует.");
        }

        public bool RemoveClient(ushort clID)
        {
            var client = FindClientWithID(clID);
            if (client != null)
            {
                return Clients.Remove(client);
            }
            else
                throw new Exception("Пользователь с ID " + clID + "не был найден.");
        }

        public Client FindClientWithID(ushort clID)
        {
            if (Clients == null)
            {
                Clients = new ObservableCollection<Client>();
            }
            if (Clients.Count == 0)
                return null;
            return Clients.FirstOrDefault(cl => cl.ClientID == clID);
        }

        public void AddMessageToChat (ushort recepientID, ushort receiverID, string message)
        {
            var client = FindClientWithID(recepientID);
            if (client != null)
            {
                client.AddMessage(recepientID, receiverID, message);
            }
        }

        public Client FindClientWithIndex(int index)
        {
            if (index < Clients.Count)
            {
                return Clients[index];
            }
            else
                throw new Exception("Невозможно найти клиента с выбранным индексом.");
        }

    }

    public class IPNetData
    {
        public string IP { get; set; } = null;

        public ushort? Port { get; set; } = null;

        public bool IsEmpty 
        { 
            get
            {
                return IP == null;
            }
        }

        public IPNetData (string ipPort)
        {
            var val = ipPort.Split(':');
            if (val.Length == 2)
            {
                if (ushort.TryParse(val[1], out ushort _port))
                {
                    Port = _port;
                    var ipVal = val[0].Split('.');
                    if (ipVal.Length == 4)
                    {
                        foreach (var ipDig in ipVal)
                        {
                            if (!byte.TryParse(ipDig, out byte _))
                                throw new Exception("IP имеет некорректный формат [0-255].[0-255].[0-255].[0-255]");
                        }
                        IP = val[0];
                    }
                    else
                        throw new Exception("IP имеет некорректный формат [0-255].[0-255].[0-255].[0-255]");
                }
                else
                {
                    throw new Exception("Порт должен быть от [0-65535]");
                }

            }
            else
                throw new Exception("Комбинация [IP:Port] неверна..");
        }

        public IPNetData (string ip, ushort port)
        {
            IP = ip;    
            Port = port;
        }

        public override string ToString()
        {
            return IP + ":" + Port;
        }

    }

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
                //OnNetworkStatusChanged?.Invoke(ServerStatus.TryToConnect, MainData);
                SocketClient.BeginConnect(IPAddress.Parse(MainData.IP), (int) MainData.Port, new AsyncCallback(ConnectCallback), userName);
            }
        }

        private async void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                SocketClient.EndConnect(ar);
                var userName = (string)ar.AsyncState;
                SendOperationInfo(CodeOperations.Connect, userName);
                MemStream = new MemoryStream(new byte[StreamCapacity], 0, StreamCapacity, true, true);
                OnNetworkStatusChanged?.Invoke(ServerStatus.Connected, MainData);
                StartReceive(ar);
            }
            catch (Exception ex)
            {
                DisconnectClient(ex);
            }
        }

        private void SendOperationInfo(CodeOperations operation, object netdata)
        {
            var nd = new NetData()
            {
                CodeOperation = operation,
                Data = netdata
            };
            string val = JsonConvert.SerializeObject(nd);
            try
            {
                SocketClient.Client.Send(Encoding.Default.GetBytes(val));
            }
            catch (Exception ex)
            {
                DisconnectClient(ex);
            }
        }

        public void DisconnectClient(Exception ex = null)
        {
            if (ex != null)
                OnNetworkStatusChanged?.Invoke(ServerStatus.Aborted, MainData);
            else
                OnNetworkStatusChanged?.Invoke(ServerStatus.Disconnected, MainData);
            SocketClient?.Dispose();
        }

        private void StartReceive(IAsyncResult ar)
        {
            SocketClient.Client.BeginReceive(MemStream.GetBuffer(), 0, MemStream.GetBuffer().Length, SocketFlags.None, out SocketError sockErr, new AsyncCallback(ReceivedData), ar.AsyncState);
        }

        private void ReceivedData(IAsyncResult ar)
        {
            TcpClient connect = (TcpClient)ar.AsyncState;
            int bytesRead = connect.Client.EndReceive(ar, out SocketError errorcode);
            if (bytesRead > 0 && errorcode == SocketError.Success)
            {
                //StartReceive(ar);
            }
            else
                DisconnectClient();
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

    public enum ServerStatus
    {
        Disconnected, Aborted, Connected, TryToConnect
    }

}
