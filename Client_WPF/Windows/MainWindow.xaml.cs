using System;
using System.Collections.Generic;
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

namespace Client_WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppNetwork Network { get; set; } = new AppNetwork();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Network.OnNetworkStatusChanged += UI_NetStatusChanged;
        }

        private void UI_NetStatusChanged(ServerStatus status, IPNetData data)
        {
            UI_ChangeStatus(status, data);
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
            if (Network.IsEmpty)
            {
                Window_IPEntering w_Ip = new Window_IPEntering();
                w_Ip.ShowDialog();
                if (w_Ip.DialogResult == true)
                {
                    try
                    {
                        Network = AppNetwork.OpenConnectionTo(w_Ip.IpPort);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка" + ex.Message, "Ошибка.");
                    }
                }
            }
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
                return IP != null;
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
        public delegate void NetworkStatusChanged(ServerStatus status, IPNetData data);

        public event NetworkStatusChanged OnNetworkStatusChanged;

        private TcpClient SocketClient { get; set; } = new TcpClient();

        public IPNetData MainData { get; set; }

        public AppNetwork()
        {
                
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

        private AppNetwork(IPNetData data)
        {
            MainData = data;
        }

        private AppNetwork(string ipPort)
        {
            MainData = new IPNetData(ipPort);
            OpenSocketWithData();
        }

        private AppNetwork(string ip, ushort port)
        {
            MainData = new IPNetData(ip, port);
            OpenSocketWithData();
        }

        private void OpenSocketWithData()
        { 
            if (IsConnected.HasValue && !IsConnected.Value)
            {
                OnNetworkStatusChanged?.Invoke(ServerStatus.TryToConnect, MainData);
                SocketClient.BeginConnect(IPAddress.Parse(MainData.IP), (int) MainData.Port, new AsyncCallback(ConnectCallback), null);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            SocketClient.EndConnect(ar);
            OnNetworkStatusChanged?.Invoke(ServerStatus.Connected, MainData);
        }

        public static AppNetwork OpenConnectionTo(IPNetData data)
        {
            return new AppNetwork(data);
        }

        public static AppNetwork OpenConnectionTo(string ipPort)
        {
            return new AppNetwork(ipPort);
        }

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

    }

    public enum ServerStatus
    {
        Disconnected, Aborted, Connected, TryToConnect
    }
}
