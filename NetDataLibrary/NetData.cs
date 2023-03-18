using System.Collections.ObjectModel;

namespace NetDataLibrary
{
    public class NetData
    {
        public CodeOperations CodeOperation { get; set; }

        public object Data { get; set; }

    }

    public enum CodeOperations
    {
        Connect, SendMessage, LoadChat, Disconnect,
        Undefined
    }

    public enum ClientStatus
    {
        Connected, Disconnected
    }

    public class Client
    {
        public ushort ClientID { get; set; }

        public string UserName { get; set; }

        public ObservableCollection<MessageData> Messages { get; set; }

        public Client(ushort clID, string userName, ObservableCollection<MessageData> messages = null)
        {
            ClientID = clID;
            UserName = userName;
            if (messages == null)
            {
                Messages = new ObservableCollection<MessageData>();
            }
            else
                Messages = messages;
        }

        public override string ToString()
        {
            return UserName;
        }

        public void AddMessage(ushort recepientID, ushort receiverID, string message)
        {
            Messages.Add(new MessageData()
            {
                Message = message,
                ReceiverID = receiverID,
                RecepientID = recepientID
            });
        }

    }

    public class MessageData
    {
        public ushort RecepientID { get; set; }

        public ushort ReceiverID { get; set; }

        public string Message { get; set; }

    }

}
