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

namespace Client_WPF
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

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class NetworkData
    {
        public string IP { get; set; } 

        public ushort Port { get; set; } 

        public NetworkData (string ipPort)
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

        public NetworkData (string ip, ushort port)
        {
            IP = ip;    
            Port = port;
        }

    }

    public class AppNetwork
    {
        public delegate void NetworkStatusChanged(ServerStatus status, NetworkData data);

        public event NetworkStatusChanged OnNetworkStatusChanged;

        private TcpClient SocketClient { get; set; } = new TcpClient();

        public NetworkData MainData { get; set; }

        public AppNetwork()
        {
                
        }

        private AppNetwork(NetworkData data)
        {
            MainData = data;
        }

        private AppNetwork(string ipPort)
        {
            MainData = new NetworkData(ipPort);
            OpenSocketWithData();
        }

        private AppNetwork(string ip, ushort port)
        {
            MainData = new NetworkData(ip, port);
            OpenSocketWithData();
        }

        private void OpenSocketWithData()
        { 
            if (IsConnected.HasValue && !IsConnected.Value)
            {
                OnNetworkStatusChanged?.Invoke(ServerStatus.TryToConnect, MainData);
                SocketClient.BeginConnect(IPAddress.Parse(MainData.IP), MainData.Port, new AsyncCallback(ConnectCallback), null);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            SocketClient.EndConnect(ar);
            OnNetworkStatusChanged?.Invoke(ServerStatus.Connected, MainData);
        }

        public static AppNetwork OpenConnectionTo(NetworkData data)
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
