using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Console
{
    internal static class ServerConsole
    {
        public static string Title
        {
            get
            {
                return Console.Title;
            }
            internal set
            {
                Console.Title = value;
            }
        }


        internal static ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        internal static void WriteTo(string v)
        {
            Console.WriteLine(v);
        }

        internal static void WriteTo(params string[] v)
        {
            WriteTo(string.Join(string.Empty, v));
        }

    }
}
