namespace Client_WPF.Network
{
    public class SocketData
    {
        public string IP { get; set; }

        public int Port { get; set; } = 13868;

        /// <summary>
        /// Used in future
        /// </summary>
        public int BufferSize { get; set; } = 512;

    }
}