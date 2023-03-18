namespace NetDataLibrary.UserInfos
{
    public class ClientUserInfo
    {
        public string UserName { get; set; }

    }

    public class ServerUserInfo
    {
        public ServerUserInfo(ushort index)
        {
            UserIndex = index;
        }

        public string UserName { get; set; }

        public ushort UserIndex { get; set; }

        public void ChangeUserName(string name)
        {
            UserName = name;
        }
    }
}
