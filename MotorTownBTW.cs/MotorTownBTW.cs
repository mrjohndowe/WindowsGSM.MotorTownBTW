using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
    public class MotorTownBTW : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.MotorTownBTW",
            author = "MrJohnDowe",
            description = "WindowsGSM plugin for supporting MotorTownBTW Dedicated Server",
            version = "2025.23.10.1704-alpha", // format "YYYY.MM.DD.HHMM"
            url = "https://github.com/mrjohndowe/WindowsGSM.MotorTownBTW.git",
            color = "#053beeff"
        };

        // - Standard Constructor and properties
        public MotorTownBTW(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;


        // {
        // AppIdnParam = $"{AppId} {customParam}";
        // }
        // private readonly ServerConfig _serverData;
        // public string ErrorMessage, NoticeMessage;

        // - Game server Fixed variables
        public override string StartPath => "MotorTown\\Binaries\\Win64\\MotorTownServer-Win64-Shipping.exe";
        public string FullName = "MotorTownBTW Dedicated Server";
        public bool AllowsEmbedConsole = true; // Allowed but Game currently does not support it as in doesn't let you use server console only here for info.
        public int PortIncrements = 3;
        public object QueryMethod = new A2S(); // Query method should be A2S for this game server

        //web interface info TODO Still looking into using the REST API interface.
        public string WebAPIPort = "27016";

        // - Game server default values
        public string Port = "7777"; //Not sure if this is even change able.
        public string QueryPort = "27015";
        public string Defaultmap = "Jeju_World";
        public string Maxplayers = "200";
        public string Additional = "-log -useperfthreads";

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => false; // Click Login and add your login info to txt file keep this safe.
        public override string AppId => "2223650 -beta test -betapassword motortowndedi";

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {

            string sampleConfigPath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, "DedicatedServerConfig_Sample.json");
            File.Copy(sampleConfigPath, GetConfigPath());

            UpdateConfig();
        }


        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            string shipExePath = Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            UpdateConfig();

            StringBuilder sb = new StringBuilder();
            sb.Append($"{_serverData.ServerMap}?listen? -server ");
            sb.Append($"{_serverData.ServerParam} ");
            sb.Append($" -MultiHome={_serverData.ServerIP}"); //maybe change to online IP
            sb.Append($" -Port={_serverData.ServerPort} ");
            sb.Append($" -QueryPort={_serverData.ServerQueryPort} ");
            //



            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = shipExePath,
                    Arguments = sb.ToString(),
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Set up Redirect Input and Output to WindowsGSM Console if EmbedConsole is on
            if (serverData.EmbedConsole)
            {
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                var serverConsole = new ServerConsole(_serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
            }

            // Start Process
            try
            {
                p.Start();
                if (_serverData.EmbedConsole)
                {
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                }
                return p;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null; // return null if fail to start
            }
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            var exePath = p.MainModule.FileName;
            var configPath = exePath.Replace(StartPath, "");
            var configs = GetServerConfigs(Path.Combine(configPath, "DedicatedServerConfig.json"));
            var enabled = (bool)configs["bEnableHostWebAPIServer"];
            var port = (int)configs["HostWebAPIServerPort"];
            var password = (string)configs["HostWebAPIServerPassword"];

            if (enabled)
            {
                await ShutDownCountDown(port, password);
            }

            await Task.Run(async () =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle); // Gracefull?
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                p.WaitForExit(2000);
                if (!p.HasExited)
                    p.Kill();
            });
        }

        private string GetConfigPath()
        {
            return Functions.ServerPath.GetServersServerFiles(_serverData.ServerID, "DedicatedServerConfig.json");
        }

        private JObject GetServerConfigs(string configPath = null)
        {
            if (configPath == null)
                configPath = GetConfigPath();

            var json = File.ReadAllText(configPath);
            var configs = JObject.Parse(json);
            return configs;
        }

        private void SaveConfigFile(JObject config)
        {
            File.WriteAllText(GetConfigPath(), config.ToString());
        }

        private JObject UpdateConfig()
        {

            var configs = GetServerConfigs();
            configs["ServerName"] = _serverData.ServerName;
            configs["MaxPlayers"] = Int32.Parse(_serverData.ServerMaxPlayer);
            SaveConfigFile(configs);
            return configs;
        }

        private async Task ShutDownCountDown(int port, string password)
        {
            int countDown = 60;
            ServerSendMessage(port, password, $"Server restarting in {countDown} seconds");

            while (countDown > 0)
            {
                if (countDown == 30 && countDown <= 10)
                {
                    ServerSendMessage(port, password, $"Server restarting in {countDown} seconds");
                }
                countDown--;
                await Task.Delay(1000);
            }
        }

        private void ServerSendMessage(int port, string password, string message)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string remoteUrl = $"http://localhost:{port}/chat?password={password}&message={message}";
                    webClient.UploadString(remoteUrl, "POST", "{}");
                }
            }
            catch
            {
                //ignore
            }
        }
    }
}
