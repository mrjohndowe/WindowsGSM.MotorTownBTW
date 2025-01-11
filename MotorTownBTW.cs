using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
//using Newtonsoft.Json.Linq;

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
            version = "2025.01.11.1221", // format "YYYY.MM.DD.HHMM"
            url = "https://github.com/TheRealSarcasmO/WindowsGSM.MotorTownBTW",
            color = "#34FFeb"
        };

        // - Standard Constructor and properties
        public MotorTownBTW(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string ErrorMessage, NoticeMessage;

        // - Game server Fixed variables
        public override string StartPath => "MotorTown\\Binaries\\Win64\\MotorTownServer-Win64-Shipping.exe";
        public string FullName = "MotorTownBTW Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 3;
        public object QueryMethod = new A2S(); // Query method should be A2S for this game server

        // - Game server default values
        public string Port = "27015";
        public string QueryPort = "27016";
        public string Defaultmap = "jeju_world";
        public string Maxplayers = "200";
        public string Additional = "Jeju_World?listen? -server -log -useperfthreads";

        //web interface info TODO
        public string WebAPIPort = "8080";

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => false; // Click Login and add your login info to txt file keep this safe.
        public string AppId = "2223650";

        // public string custom = "-beta test -betapassword motortowndedi";
        //
        //public string customParam = " -beta test -betapassword motortowndedi";

        public async Task<Process> Install()
        {
            var steamCMD = new Installer.SteamCMD();
            Process p = await steamCMD.Install(_serverData.ServerID, string.Empty, AppId, loginAnonymous);
            Error = steamCMD.Error;

            return p;
        }

        public async Task<Process> Update(bool validate = true, string custom = null)
        {
            if (custom == null)
                custom = " -beta test -betapassword motortowndedi";
            var (p, error) = await Installer.SteamCMD.UpdateEx(_serverData.ServerID, AppId, validate, custom: custom,loginAnonymous: loginAnonymous);
            Error = error;
            return p;
        }

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
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");                 //Trying to grace-fully exit
                p.WaitForExit(20000);
            });
        }
    }
}
