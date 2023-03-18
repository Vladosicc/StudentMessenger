using System.Collections.ObjectModel;

using NetDataLibrary.NetworkData;

namespace NetDataLibrary
{

    public class Client_ChatUser
    {
        public ushort ClientID { get; set; }

        public string UserName { get; set; }

        public ObservableCollection<MessageData> Messages { get; set; }

        public Client_ChatUser(ushort clID, string userName, ObservableCollection<MessageData> messages = null)
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




}

