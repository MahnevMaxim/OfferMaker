using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace API
{
    public class L
    {
        /// <summary>
        /// Log writer
        /// </summary>
        /// <param name="log_string"></param>
        /// <param name="exc"></param>
        public static void LW(string log_string, Exception exc = null)
        {
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
                using (StreamWriter sw = File.AppendText(@"log.txt"))
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

        /// <summary>
        /// Clear log
        /// </summary>
        public static void Clear()
        {
            try
            {
                //стираем лог
                StreamWriter sw = new StreamWriter(@"log.txt", false, Encoding.UTF8);
                sw.WriteLine("");
                sw.Close();
            }
            catch { }
        }
    }
}
