using System;
using System.Threading.Tasks;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using System.Threading;
using System.IO;

namespace ApiClientCodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread.Sleep(5000);
                System.Net.WebClient wclient = new System.Net.WebClient();
                var document = OpenApiDocument.FromJsonAsync(wclient.DownloadString("https://localhost:44313/swagger/v1/swagger.json")).Result;
                var settings = new CSharpClientGeneratorSettings
                {
                    ClassName = "Client",
                    CSharpGeneratorSettings =
                    {
                        Namespace = "ApiLib",
                        JsonLibrary=NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson
                    },
                    WrapResponses=true,
                    ResponseClass="ApiResponse"
                };

                var generator = new CSharpClientGenerator(document, settings);
                var code = generator.GenerateFile();

                //replace double
                code = code.Replace("double", "decimal");
                //replace status code
                code = code.Replace("status_ == 200", "status_ == 200 || status_ == 201 || status_ == 204");

                //replace permissions
                int indexPermBegin = code.IndexOf("enum Permissions\n");
                int indexPermEnd = 0;
                string forReplace = "";
                for (int i = indexPermBegin; i < code.Length; i++)
                {
                    forReplace += code[i];
                    if (code[i] == '}')
                    {
                        indexPermEnd = i;
                        break;
                    }
                }

                string appPath = Directory.GetCurrentDirectory();

                //read source
                string sourcePath = Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..\..\Shared\Entities\Permissions.cs");
                string source = File.ReadAllText(sourcePath);
                string forWrite = "";
                for (int i = source.IndexOf("enum Permissions"); i < source.Length; i++)
                {
                    forWrite += source[i];
                    if (source[i] == '}')
                    {
                        indexPermEnd = i;
                        break;
                    }
                }

                //replace
                code = code.Replace(forReplace, forWrite);
                string pathToClassFile = Path.GetFullPath(Directory.GetCurrentDirectory() + @"\..\..\..\..\ApiClient\Client.cs");
                File.WriteAllText(pathToClassFile, code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
