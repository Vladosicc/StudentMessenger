using System.Net;
using System.Net.Sockets;

using Microsoft.AspNetCore.SignalR.Client;

namespace Client;

public partial class MainPage : ContentPage
{
    HubConnection mainConnect;
	public MainPage()
	{
		InitializeComponent();
	}

    private void B_SendMessage_Clicked(object sender, EventArgs e)
    {

    }

    private void B_ConnectToServer_Clicked(object sender, EventArgs e)
    {
        string ip = TB_IPInput.Text;
        string[] values = ip.Split(":");
        if (values.Length < 2)
        {
            L_ErrorsData.Text = "Ошибка. Введите IP вида 192.168.0.100:{port}";
            return;
        }
        if (IPAddress.TryParse(values[0], out IPAddress ipVal))
        {
            if (short.TryParse(values[1], out short port))
            {
                mainConnect = new HubConnectionBuilder().WithUrl(string.Format("http://{0}/chat", ip)).Build();
                mainConnect.Closed += MainConnect_Closed;
                TryConnectToServer();
            }
            
        }
    }

    private Task MainConnect_Closed(Exception arg)
    {
        L_ErrorsData.Text = arg.Message;
        B_ConnectToServer.IsEnabled = true;
        return Task.CompletedTask;
    }

    private async Task TryConnectToServer()
    {
        if (mainConnect != null)
        {
            try
            {
                B_ConnectToServer.IsEnabled = false;
                TB_IPInput.IsEnabled = false;
                await mainConnect.StartAsync();
                B_SendMessage.IsEnabled = true;
                TB_LoginInput.IsEnabled = true;
                TB_MessageInput.IsEnabled = true;
            }
            catch (SocketException ex)
            {        
                if (ex.SocketErrorCode == SocketError.ConnectionAborted)
                {
                    L_ErrorsData.Text = "Ошибка... Невозможно подключиться к серверу.";
                }
                B_ConnectToServer.IsEnabled = true;
                TB_IPInput.IsEnabled = true;
                B_SendMessage.IsEnabled = false;
                TB_LoginInput.IsEnabled = false;
                TB_MessageInput.IsEnabled = false;
            }
            catch (HttpRequestException ex)
            {
                switch (ex.StatusCode)
                {
                    case null:
                        {
                            L_ErrorsData.Text = "Ошибка... Невозможно подключиться к серверу.";
                            break;
                        }
                    case HttpStatusCode.Forbidden:
                        {
                            L_ErrorsData.Text = "Ошибка... Невозможно подключиться к серверу.";
                            break;
                        }
                }
                
                B_ConnectToServer.IsEnabled = true;
                TB_IPInput.IsEnabled = true;
                B_SendMessage.IsEnabled = false;
                TB_LoginInput.IsEnabled = false;
                TB_MessageInput.IsEnabled = false;

            }
        }
    }
}