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
                        JsonLibrary=NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson,
                        
                    }
                };

                var generator = new CSharpClientGenerator(document, settings);
                var code = generator.GenerateFile();

                //replace double
                code = code.Replace("double", "decimal");
                //replace status code
                code = code.Replace("status_ == 200", "status_ == 200 || status_ == 201 || status_ == 204");

                //replace permissions
                int indexPermBegin = code.IndexOf("Permissions");
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

                //read source
                string source = File.ReadAllText(@"C:\Users\Maxxx\source\repos\API\Shared\Entities\Permissions.cs");
                string forWrite = "";
                for (int i = source.IndexOf("Permissions"); i < source.Length; i++)
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

                string pathToClassFole = @"C:\Users\Maxxx\source\repos\API\ApiClient\Client.cs";
                File.WriteAllText(pathToClassFole, code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
