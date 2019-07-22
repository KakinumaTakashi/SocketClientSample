using System;
using System.Diagnostics;

namespace SocketServerSample.util
{
    public static class Log
    {
        public static string i(string msg)
        {
            return write("I", msg);
        }

        public static string w(string msg)
        {
            return write("W", msg);
        }

        public static string e(string msg)
        {
            return write("E", msg);
        }

        public static string d(string msg)
        {
#if DEBUG
            return write("D", msg);
#else
            return "";
#endif
        }

        private static string write(string level, string msg)
        {
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            string log = String.Format("{0} [{1:D5}-{2:D5}]/{3}: {4}", timestamp, processId, threadId, level, msg);
            Console.WriteLine(log);

            return log;
        }
    }
}
