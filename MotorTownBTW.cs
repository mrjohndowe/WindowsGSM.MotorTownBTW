using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using Newtonsoft.Json;

namespace WindowsGSM.Plugins
{
    public class MotorTownBTW : SteamCMDAgent // SteamCMDAgent is used because MotorTownBTW relies on SteamCMD for installation and update process
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.MotorTownBTW", // WindowsGSM.MotorTownBTW
            author = "TheRealSarcasmO",
            description = "🧩 WindowsGSM plugin for supporting MotorTown Behind The Wheel Dedicated Server (Coding inspired by battleduck)",
            version = "0.1",
            url = "https://github.com/TheRealSarcasmO/WindowsGSM.MotorTownBTW", // Github repository link (Best practice)
            color = "#9eff99" // Color Hex
        };

        // - Standard Constructor and properties
        public MotorTownBTW(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData; // Store server start metadata, such as start ip, port, start param, etc

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true; // MotorTownBTW requires to login steam account to install the server, so loginAnonymous = false
        public override string AppId => "2223650"; // Game server appId, MotorTownBTW is 2223650

        // - Game server Fixed variables
        public override string StartPath => @"Binaries\Win64\MotorTownServer-Win64-Shipping.exe"; // Game server start path, for MotorTownBTW, it is MotorTownServer-Win64-Shipping.exe
        public string FullName = "MotorTownBTW Dedicated Server"; // Game server FullName
        public bool AllowsEmbedConsole = true;  // Does this server support output redirect? (hoping this is or becomes a thing)
        public int PortIncrements = 2; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = new UT3(); // Query method should be use on current server type. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        // - Game server default values
        public string Port = "27015"; // Default port
        public string QueryPort = "27016"; // Default query port
        public string Defaultmap = "jeju_world"; // Default map name
        public string Maxplayers = "64"; // Default maxplayers
        public string Additional = "jeju_world?listen? -server -log -useperfthreads"; // Additional server start parameter

        // - API Settings
        private string apiPassword = "supersecret";
        private string apiPort = "27017";

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG() { }

        private void LoadConfig()
        {
            // Read the DedicatedServerConfig.json file to get API settings
            // Assuming we have a method ReadConfig to read the JSON file
            var config = ReadConfig("DedicatedServerConfig.json");
            apiPassword = config.HostWebAPIServerPassword;
            apiPort = config.HostWebAPIServerPort ?? "8080";
        }

        private async Task<string> GetApiResponse(string endpoint)
        {
            string url = $"http://localhost:{apiPort}/{endpoint}?password={apiPassword}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<int> GetPlayerCountAsync()
        {
            LoadConfig();
            string responseBody = await GetApiResponse("player/count");

            dynamic data = JsonConvert.DeserializeObject(responseBody);
            if (data.succeeded)
            {
                return data.data.num_players;
            }
            else
            {
                throw new Exception(data.message);
            }
        }

        // Add more methods for other API calls such as KickPlayer, BanPlayer, etc.

        // - Install server function
        public override async Task<Process> Install()
        {
            var steamCMD = new Process
            {
                StartInfo =
                {
                    FileName = "steamcmd.exe",
                    Arguments = $"+login anonymous +force_install_dir {ServerPath.GetServersServerFiles(_serverData.ServerID)} +app_update {AppId} -beta test -betapassword motortow
                    ndedi validate +exit",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            steamCMD.OutputDataReceived += (sender, args) => base.ServerConsole += $"{args.Data}\n";
            steamCMD.Start();
            steamCMD.BeginOutputReadLine();
            await steamCMD.WaitForExitAsync();

            return steamCMD;
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            // Prepare start parameter
            var param = new StringBuilder();
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerPort) ? string.Empty : $" -port={_serverData.ServerPort}");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerName) ? string.Empty : $" -name=\"{_serverData.ServerName}\"");
            param.Append(string.IsNullOrWhiteSpace(_serverData.ServerParam) ? string.Empty : $" {_serverData.ServerParam}");

            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
                    Arguments = param.ToString()
                },
                EnableRaisingEvents = true
            };

            // Start Process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                base.Error = e.Message;
                return null; // return null if fail to start
            }
        }

        // - Stop server function
        public async Task Stop(Process p) => await Task.Run(() => { p.Kill(); });
    }
}
