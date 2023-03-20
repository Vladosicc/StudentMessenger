using System.Net.Sockets;

using NetDataLibrary;
using NetDataLibrary.NetworkData;
using NetDataLibrary.Sockets;
using NetDataLibrary.UserInfos;

using Newtonsoft.Json;

namespace Server_Console.UserData
{
    public class ChatUser
    {
        public Socket Socket { get; set; } 

        public ServerUserInfo Info { get; set; }

        public ChatUser(Socket sock, ushort index)
        {
            Socket = sock;
            Info = new ServerUserInfo(index);
        }

        private ChatUser()
        {

        }

        public void SendData(string request)
        {
            byte[] data = JsonNetworkConvert.SerializeObjectToSend(request);
            SendData(data);
        }
        
        public void SendData(byte[] data)
        {
            if (Socket.Connected)
                Socket.Send(data);
        }
        
        public void SendMessage(MessageData message)
        {
            NetData nd = new NetData()
            {
                CodeOperation = CodeOperations.SendMessage,
                Data = message
            };
            SendData(JsonNetworkConvert.SerializeObjectToSend(nd));
        }
        
    }

    public class ChatUserData
    {
        static ushort ChatUserIndex = 1;

        private List<ChatUser> Users { get; set; }

        public ChatUser AddUser(Socket sock)
        {
            if (Users == null)
                Users = new List<ChatUser>();
            try
            {
                ChatUser res = new ChatUser(sock, ChatUserIndex++);
                Users.Add(res);
                return res;
            }
            catch 
            {
                throw;
            }
        }

        public ChatUser FindUserBy(Socket sock)
        {
            if (sock == null || CheckUsersNullable())
                return null;
            return Users.FirstOrDefault(chus => chus.Socket == sock);
        } 

        private bool CheckUsersNullable()
        {
            if (Users == null || Users.Count == 0)
                return true;
            else
                return false;
        }

        public ChatUser FindUserBy(int index)
        {
            if (CheckUsersNullable())
                return null;
            return Users.FirstOrDefault(chus => chus.Info.UserIndex == index);
        }

        public bool RemoveUserBy(Socket sock)
        {
            var chUser = FindUserBy(sock);
            if (chUser != null)
                return Users.Remove(chUser);
            else
                return false;
        }

        public bool RemoveUserBy(int index)
        {
            var chUser = FindUserBy(index);
            if (chUser != null)
                return Users.Remove(chUser);
            else
                return false;
        }

        public void ChangeUserName(ChatUser user, string? userName)
        {
            if (!CheckUsersNullable())
            {
                var chUser = FindUserBy(user.Info.UserIndex);
                chUser.Info.ChangeUserName(userName);
            }
        }
        

        public void SendDataToAllUsers(byte[] data, ChatUser sender = null)
        {
            if (!CheckUsersNullable())
            {
                foreach (var user in Users)
                {
                    if (sender != null)
                    {
                        if (sender.Info.UserIndex != user.Info.UserIndex)
                            SendDataToUser(user, data);
                    }
                    else
                        SendDataToUser(user, data);
                    
                }
            }
        }

        public void SendDataToUser(ChatUser user, byte[] data)
        {
            user.SendData(data); 
        }
        
        public void SendDataToUser(ChatUser user, string request)
        {
            user.SendData(request);
        }
        
        public void SendMessageToUser(ChatUser userFrom, MessageData? message)
        {
            if (userFrom != null)
            {
                ChatUser user = FindUserBy(message.ReceiverID);
                if (user != null)
                    user.SendMessage(message);
            }
        }

        internal void SendDataAboutConnectedUsersTo(ChatUser user)
        {
            foreach (var usInfo in Users)
            {

                if (user.Info.UserIndex != usInfo.Info.UserIndex)
                {
                    string usJsonInf = JsonNetworkConvert.SerializeObject(usInfo.Info);
                    NetData nd = new NetData()
                    {
                        CodeOperation = CodeOperations.ConnectClient,
                        Data = usJsonInf
                    };
                    byte[] req = JsonNetworkConvert.SerializeObjectToSend(nd);
                    SendDataToUser(user, req);
                }
                else
                {
                    string usJsonInfo = JsonNetworkConvert.SerializeObject(user.Info);
                    NetData nd = new NetData()
                    {
                        CodeOperation = CodeOperations.ConnectClient,
                        Data = usJsonInfo
                    };
                    byte[] req = JsonNetworkConvert.SerializeObjectToSend(nd);
                    SendDataToUser(usInfo, req);
                }

            }
        }
    }

}