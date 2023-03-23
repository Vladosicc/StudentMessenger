using Microsoft.AspNetCore.SignalR.Client;

namespace Client
{
	public partial class OldPage : ContentPage
	{
		int count = 0;
		HubConnection connection;

		public OldPage()
		{
			InitializeComponent();
			connection = new HubConnectionBuilder().WithUrl("https://localhost:9678/chat").Build();
			connection.On<string, string>("Receive", (user, message) =>
				Dispatcher.Dispatch(() =>
				{
					var newMess = $"{user}: {message}";

				}));
		}

		private void OnCounterClicked(object sender, EventArgs e)
		{
			count++;

			if (count == 1)
				CounterBtn.Text = $"Clicked {count} time";
			else
				CounterBtn.Text = $"Clicked {count} times";

			SemanticScreenReader.Announce(CounterBtn.Text);
		}

        private void ContentPage_Loaded(object sender, EventArgs e)
        {

        }
    }

}

