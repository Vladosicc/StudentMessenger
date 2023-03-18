using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Client_WPF.Network;
using NetDataLibrary;
using NetDataLibrary.NetworkStatuses;

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
                            UI_ConnectClient();
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
                            UI_ConnectClient();
                            break;
                        }
                    case ServerStatus.Aborted:
                        {
                            Title = "Мессенджер - Клиент (оборвано). IP: " + data.ToString();
                            UI_ConnectClient();
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
            if (Network.IsConnected == null  || !Network.IsConnected.Value)
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
                    Dispatcher.Invoke(() => Clients_AddNewClient(new Client_ChatUser(recID, userLogin)));
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
                    if (!Dispatcher.Invoke(() => Clients_RemoveClient(recID)))
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

        private void Clients_AddNewClient(Client_ChatUser user)
        {
            ClData.AddClient(user);
        }

        private bool Clients_RemoveClient(ushort recID)
        {
            return ClData.RemoveClient(recID);
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
                    TB_Message.IsEnabled = true;
                    B_Message_Send.IsEnabled = true;
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
            UI_DisconnClient();
        }

        private void UI_DisconnClient()
        {
            MI_Home_Server.IsEnabled = true;
            MI_Server_ChangeServ.IsEnabled = true;
            MI_Server_Disconnect.IsEnabled = false;
        }

        private void UI_ConnectClient()
        {
            //MI_Home_Server.IsEnabled = true;
            MI_Server_ChangeServ.IsEnabled = true;
            MI_Server_Disconnect.IsEnabled = true;
        }

    }

    public class ClientsData
    {
        public ObservableCollection<Client_ChatUser> Clients { get; set; } = new ObservableCollection<Client_ChatUser>();

        public ClientsData ()
        {
            Clients = new ObservableCollection<Client_ChatUser>();
        }

        public void AddClient(Client_ChatUser client)
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

        public Client_ChatUser FindClientWithID(ushort clID)
        {
            if (Clients == null)
            {
                Clients = new ObservableCollection<Client_ChatUser>();
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

        public Client_ChatUser FindClientWithIndex(int index)
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

    public enum ServerStatus
    {
        Disconnected, Aborted, Connected, TryToConnect
    }

}
