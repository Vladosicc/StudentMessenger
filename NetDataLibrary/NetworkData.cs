using System.Text;
using Newtonsoft.Json;

namespace NetDataLibrary.NetworkData
{
    public static class JsonNetworkConvert
    {

        public static string SerializeObject(object? jsonData)
        {
            return JsonConvert.SerializeObject(jsonData);
        }

        public static object? DeserializeObject(string jsonData, Type type)
        {
            return JsonConvert.DeserializeObject(jsonData, type);
        }

        public static object? DeserializeObject(byte[] netBytes, Type type)
        {
            return DeserializeObject(Encoding.Unicode.GetString(netBytes), type);
        }

        public static byte[] SerializeObjectToSend(object? jsonData)
        {
            return Encoding.Unicode.GetBytes(SerializeObject(jsonData));
        }

    }

    public class MessageData
    {
        public ushort RecepientID { get; set; }

        public ushort ReceiverID { get; set; }

        public string Message { get; set; }

    }

    public class NetData
    {
        public CodeOperations CodeOperation { get; set; }

        public object Data { get; set; }

    }

    public enum CodeOperations
    {
        ConnectClient, SendMessage, LoadChat, DisconnectClient,
        Undefined
    }

}
