using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace API
{
    public class Log
    {
        static string defaultLogPath = "log.txt";

        public static void Write(Exception exc) => Write("", exc);

        /// <summary>
        /// Log writer
        /// </summary>
        /// <param name="log_string"></param>
        /// <param name="exc"></param>
        public static void Write(string log_string, Exception exc = null, string logPath = null)
        {
            if (logPath == null)
                logPath = defaultLogPath;

            DateTime date = DateTime.UtcNow;
            string text = "";
            if (exc != null)
            {
                text += log_string + "\n" + exc.ToString() + "\n" + exc.StackTrace;
            }
            else
            {
                text += log_string + "\n";
            }

            try
            {
                using (StreamWriter sw = File.AppendText(logPath))
                {
                    sw.WriteLine(text);
                    Console.WriteLine(text);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Исключение при доступе к логу\n" + ex.ToString());
            }
        }

        internal static void Write(string message, string logPath) => Write("", null, logPath);

        internal static void EfWrite(string message) => Write(message, null, "ef_log.txt");

        /// <summary>
        /// Clear log
        /// </summary>
        internal static void Clear(string logPath = null)
        {
            try
            {
                if (logPath == null)
                    logPath = defaultLogPath;

                //стираем лог
                StreamWriter sw = new StreamWriter(logPath, false, Encoding.UTF8);
                sw.WriteLine("");
                sw.Close();
            }
            catch { }
        }
    }
}
