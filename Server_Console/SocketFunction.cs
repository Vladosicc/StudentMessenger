using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace safeboard_2_service.MySocket
{
    /// <summary>
    /// Вспомогательный класс для облегчения работы с сокетом
    /// </summary>
    public static class SocketFunction
    {
        public static string[] GetStringUnicodeArray(this Socket s)
        {
            var data = s.GetFullStringUnicode();
            return data.Split("\n");
        }

        public static int[] GetInt32Array(this Socket s, int count)
        {
            List<int> tmp = new List<int>();
            while (count != 0)
            {
                tmp.Add(s.GetInt32());
                count--;
            }
            return tmp.ToArray();
        }

        public static int[] GetInt32Array(this Socket s)
        {
            List<int> tmp = new List<int>();
            do
            {
                tmp.Add(s.GetInt32());
            }
            while (s.Available > 0);
            return tmp.ToArray();
        }

        public static int GetInt32(this Socket s)
        {
            byte[] data = new byte[sizeof(int)];
            s.Receive(data, sizeof(int), SocketFlags.None);
            return BitConverter.ToInt32(data, 0);
        }

        public static double GetDouble(this Socket s)
        {
            byte[] data = new byte[sizeof(double)];
            s.Receive(data, sizeof(double), SocketFlags.None);
            return BitConverter.ToDouble(data, 0);
        }

        public static char GetChar(this Socket s)
        {
            byte[] data = new byte[sizeof(char)];
            s.Receive(data, sizeof(char), SocketFlags.None);
            return BitConverter.ToChar(data, 0);
        }

        public static string GetStringUnicode(this Socket s, int sizeString)
        {
            string res = "";
            for (int i = 0; i < sizeString; i++)
            {
                res += GetChar(s);
            }
            return res;
        }

        public static string GetFullStringUnicode(this Socket s)
        {
            //получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных

            do
            {
                bytes = s.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (s.Available > 0);

            return builder.ToString();
        }

        public static string GetFullStringUTF8(this Socket s)
        {
            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных

            do
            {
                bytes = s.Receive(data);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (s.Available > 0);

            return builder.ToString();
        }

        public static string GetStringUTF32(this Socket s)
        {
            // получаем сообщение
            StringBuilder builder = new StringBuilder();
            int bytes = 0; // количество полученных байтов
            byte[] data = new byte[256]; // буфер для получаемых данных

            do
            {
                bytes = s.Receive(data);
                builder.Append(Encoding.UTF32.GetString(data, 0, bytes));
            }
            while (s.Available > 0);

            return builder.ToString();
        }
    }
}
