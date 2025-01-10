using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using Newtonsoft.Json.Linq;

namespace WindowsGSM.Plugins
{
    public class MotorTownBTW : SteamCMDAgent
    {
        // - Plugin Details
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.MotorTownBTW",
            author = "TheRealSarcasmO",
            description = "WindowsGSM plugin for supporting MotorTownBTW Dedicated Server",
            version = "2025.01.08.1725", // format "YYYY.MM.DD.HHMM"
            url = "https://github.com/TheRealSarcasmO/WindowsGSM.MotorTownBTW",
            color = "#34FFeb"
        };

        // - Standard Constructor and properties
        public MotorTownBTW(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string ErrorMessage, NoticeMessage;


        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => true;
        public string AppId = "2223650";
        string custom = "-beta test -betapassword motortowndedi";

        // - Server STeam Install 
        public async Task<Process> Install()
        {
            AppId = $"{AppId} ";
            var steamCMD = new Installer.SteamCMD();
            Process p = await steamCMD.Install(_serverData.ServerID, string.Empty, AppId, true, loginAnonymous);
            Error = steamCMD.Error;
            return p;
        }

        // - Game server Fixed variables
        public override string StartPath => "\\Binaries\\Win64\\MotorTownServer-Win64-Shipping.exe";
        public string FullName = "MotorTownBTW Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 3;
        public object QueryMethod = new A2S(); // Query method should be A2S for this game server


        // - Game server default values
        public string Port = "27015";
        public string QueryPort = "27016";
        public string Defaultmap = "jeju_world";
        public string Maxplayers = "200";
        public string Additional = "jeju_world?listen? -server -log -useperfthreads";

        //web interface info TODO
        public string WebAPIPort = "8080";




        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            //TODO
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            return null;
        }


        // - Stop server function
        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                p.WaitForExit(20000);
            });
        }
    }
}
