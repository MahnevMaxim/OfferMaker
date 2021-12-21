using System;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Text;
using System.IO;

namespace ApiPublish
{
    class Program
    {
        #region Config

        static string host = "ftp://194.58.102.194";
        static string user = "root";
        static string pass = "9XM$*9|Y**&v";
        static string pubDir = @"C:\Users\Maxxx\source\repos\API\ApiPublish\bin\Debug\net5.0\pub";
        static string projectFilePath = @"C:\Users\Maxxx\source\repos\API\API\API.csproj";
        static string remoteDir = "/var/www/kip";
        static int dots = 75;

        #endregion Config

        static FtpClient ftpClient;

        static void Main(string[] args)
        {
            if(Directory.Exists(pubDir))
            {
                Directory.Delete(pubDir, true);
            }
            string res;
            res = RunScript(@"dotnet publish " + projectFilePath + " -o " + pubDir + " -r linux-x64 --self-contained false --configuration Release");
            Console.WriteLine(res);
            ftpClient = new FtpClient(host, user, pass);
            SendDirectoryRecursive(pubDir, remoteDir);
        }

        static private string RunScript(string scriptText)
        {
            // create Powershell runspace

            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it

            runspace.Open();

            // create a pipeline and feed it the script text

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptText);

            // add an extra command to transform the script
            // output objects into nicely formatted strings

            // remove this line to get the actual objects
            // that the script returns. For example, the script

            // "Get-Process" returns a collection
            // of System.Diagnostics.Process instances.

            pipeline.Commands.Add("Out-String");

            // execute the script

            var results = pipeline.Invoke();

            // close the runspace

            runspace.Close();

            // convert the script result into a single string

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            return stringBuilder.ToString();
        }

        static private void SendDirectoryRecursive(string dirPath, string uploadPath)
        {
            string[] files = Directory.GetFiles(dirPath, "*.*");
            string[] subDirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                Write(file, uploadPath);
                ftpClient.Upload(uploadPath + "/" + Path.GetFileName(file), file);
            }

            foreach (string subDir in subDirs)
            {
                ftpClient.CreateDirectory(uploadPath + "/" + Path.GetFileName(subDir));
                SendDirectoryRecursive(subDir, uploadPath + "/" + Path.GetFileName(subDir));
            }
        }

        static void Write(string first, string second)
        {
            string res = first.Split("\\pub\\")[1];
            int lenght = res.Length;
            if (lenght < dots)
            {
                int addEmptyCount = dots - lenght;
                string additional = "";
                for (int i = 0; i < addEmptyCount; i++)
                {
                    additional += "."; ;
                }
                Console.WriteLine(res + "..." + additional + second);
            }
            else
            {
                Console.WriteLine(res + "..." + second);
            }
        }
    }
}
