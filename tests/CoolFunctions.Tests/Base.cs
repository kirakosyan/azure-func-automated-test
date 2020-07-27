using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace CoolFunctions.Tests
{
    [TestClass]
    public class Base : IDisposable
    {
        private static Process _funcHostProcess;
        public static HttpClient Client = new HttpClient();
        public const int Port = 7001;

        [AssemblyInitialize]
        public static void Init(TestContext testContext)
        {
            string buildFolder = "Release";
#if DEBUG
            buildFolder = "Debug";
#endif
            var dir = Directory.GetCurrentDirectory();

            var dotnetExePath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                @"\dotnet\dotnet.exe";
            // var dotnetExecutablePath = @"%ProgramFiles%\dotnet\dotnet.exe";
            var functionHostPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                @"\npm\node_modules\azure-functions-core-tools\bin\func.dll";
            var functionApplicationPath = dir + @$"\..\..\..\..\..\sources\CoolFunctions\bin\{buildFolder}\netcoreapp3.1";
            var functionAppFolder = Path.GetRelativePath(Directory.GetCurrentDirectory(), functionApplicationPath);

            if (!Directory.Exists(functionApplicationPath))
            {
                throw new FileNotFoundException(functionApplicationPath);
            }

            if (!Directory.Exists(functionAppFolder))
            {
                throw new FileNotFoundException(functionAppFolder);
            }

            _funcHostProcess = new Process
            {
                StartInfo =
                {
                    FileName = dotnetExePath,
                    Arguments = $"\"{functionHostPath}\" start -p {Port}",
                    WorkingDirectory = functionAppFolder
                }
            };

            var success = _funcHostProcess.Start();
            if (!success)
            {
                throw new InvalidOperationException("Could not start Azure Functions host.");
            }

            Client.BaseAddress = new Uri($"http://localhost:{Port}");

            Thread.Sleep(10000);
        }

        public virtual void Dispose()
        {
            if (!_funcHostProcess.HasExited)
            {
                _funcHostProcess.Kill();
            }

            _funcHostProcess.Dispose();
        }
    }
}
