using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
            author = "TheRealSarcasmO",
            description = "WindowsGSM plugin for supporting MotorTownBTW Dedicated Server",
            version = "2025.01.11.1919-alpha", // format "YYYY.MM.DD.HHMM"
            url = "https://github.com/TheRealSarcasmO/WindowsGSM.MotorTownBTW",
            color = "#34FFeb"
        };

        // - Standard Constructor and properties
        public MotorTownBTW(ServerConfig serverData) : base(serverData) => base.serverData = serverData;
        
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

        //web interface info TODO
        public string WebAPIPort = "27014";

        // - Game server default values
        public string Port = "27015";
        public string QueryPort = "27016";
        public string Defaultmap = "Jeju_world";
        public string Maxplayers = "200";
        public string Additional = "-log -useperfthreads";

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => false; // Click Login and add your login info to txt file keep this safe.
        public override string AppId => "2223650 -beta test -betapassword motortowndedi";
        //   public string customParam = " -beta test -betapassword motortowndedi";
        //   public string AppIdnParam;

        //        public async Task<Process> Install()
        //       {
        //          var steamCMD = new Installer.SteamCMD();
        //         Process p = await steamCMD.Install(_serverData.ServerID, string.Empty, AppIdnParam, true, loginAnonymous);  // Thanks for reminding me Raziel7893
        //        Debug.WriteLine(steamCMD.Error);
        //
        //           return p;
        //      }

        //        public async Task<Process> Update(bool validate = true, string custom = null)
        //      {
        //        if (custom == null)
        //          custom = " -beta test -betapassword motortowndedi";
        //    var (p, error) = await Installer.SteamCMD.UpdateEx(_serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);  // Thanks for reminding me Raziel7893
        //  Debug.WriteLine(error);
        //return p;
        //}

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            //TODO
        }

        // - Start server function, return its Process to WindowsGSM
        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            string shipExePath = Functions.ServerPath.GetServersServerFiles(serverData.ServerID, StartPath);
            if (!File.Exists(shipExePath))
            {
                Error = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"{serverData.ServerMap}?listen -server ");
            sb.Append($"{serverData.ServerParam} ");
            //sb.Append($" -MultiHome={_serverData.ServerIP}"); //maybe change to online IP
            //sb.Append($" -Port={_serverData.ServerPort} ");
            //sb.Append($" -QueryPort={_serverData.ServerQueryPort} ");
            //


            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = false,
                    WorkingDirectory = ServerPath.GetServersServerFiles(serverData.ServerID),
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
                var serverConsole = new ServerConsole(serverData.ServerID);
                p.OutputDataReceived += serverConsole.AddOutput;
                p.ErrorDataReceived += serverConsole.AddOutput;
            }

            // Start Process
            try
            {
                p.Start();
                if (serverData.EmbedConsole)
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
            await Task.Run(() =>
            {
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");                 //Trying to grace-fully exit
                 
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                 
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                Functions.ServerConsole.SendWaitToMainWindow("^c");
                p.WaitForExit(20000);
            });
        }
    }
}
