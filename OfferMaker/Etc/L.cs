using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;

namespace OfferMaker
{
    class L
    {
        public static void LW(string log_string, Exception exc = null)
        {
            DateTime date = DateTime.UtcNow;
            string time = String.Format("[{0:H:mm:ss:fff}] ", date);
            string text = time + " ";
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

        public static void LW(Exception ex) => LW("", ex);

        public static void ShowMessage(Exception ex)
        {
            string text = DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToLongTimeString() + "\n\r\n\r" + ex.ToString();// тут хорошо бы его отформатировать, скажем, в XML, добавить данные о времени, дате, железе и софте... 
            text += "\n\r\n\rСтек вызовов:\n\r " + ex.StackTrace + "\n\r\n\rМетод:\n\r\n\r" + ex.TargetSite;
            try
            {
                string mess = "Произошла ошибка. \n\n" + text;
                System.Windows.MessageBox.Show(mess, "Ошибка");
                LW(mess);
            }
            catch (Exception ex2)
            {
                System.Windows.MessageBox.Show("Произошла ошибка.\n\n" + ex2.ToString(), "\nОшибка");
            }
        }

        public static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowMessage((Exception)e.ExceptionObject);
            System.Windows.Forms.Application.Exit();
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowMessage(e.Exception);
            System.Windows.Forms.Application.Exit();
        }

        public static void Trace(
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                LW("member name: " + memberName + "\n" + "file path: " + sourceFilePath + "\n" + "line number: " + sourceLineNumber);
            }
            catch { }
        }
    }
}
