using System;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
//using Newtonsoft.Json.Linq;
using NLog;

namespace WindowsGSM.Plugins
{
    public class MotorTownBTW : SteamCMDAgent
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // - Standard Constructor and properties
        public MotorTownBTW(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
        private readonly ServerConfig _serverData;

        // - Game server Fixed variables
        public override string StartPath => "MotorTown\\Binaries\\Win64\\MotorTownServer-Win64-Shipping.exe";
        public string FullName = "MotorTownBTW Dedicated Server";
        public bool AllowsEmbedConsole = true; // I hope
        public int PortIncrements = 3;
        public object QueryMethod = new A2S(); // Query method should be A2S for this game server

        // - Game server default values
        public string Port = "27015";
        public string QueryPort = "27016";
        public string Defaultmap = "jeju_world";
        public string Maxplayers = "200";
        public string Additional = "Jeju_World?listen? -server -log -useperfthreads";

        //web interface info TODO
        public string WebAPIPort = "27014"; //will change sooner or later probably

        // - Settings properties for SteamCMD installer
        public override bool loginAnonymous => false; // Click Login and add your login info to txt file keep this safe.
        public string AppId = "2223650";
        public string customParam = " -beta test -betapassword motortowndedi";
        public string AppIdnParam = "${AppId} {customParam}";

        public async Task<Process> Install()
        {
            var steamCMD = new Installer.SteamCMD();

            try
            {
                Process p = await steamCMD.Install(_serverData.ServerID, string.Empty, AppIdnParam, loginAnonymous);
                logger.Info("MotorTownBTW server installed successfully.");
                return p;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to install MotorTownBTW server.");
                MessageBox.Show($"Error installing MotorTownBTW server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public async Task<Process> Update(bool validate = true, string custom = null)
        {
            if (string.IsNullOrEmpty(custom)) // Injecting updated argument for install
                custom = customParam;

            try
            {
                var (p, error) = await Installer.SteamCMD.UpdateEx(_serverData.ServerID, AppId, validate, custom: custom, loginAnonymous: loginAnonymous);
                logger.Info("MotorTownBTW server updated successfully.");
                return p;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to update MotorTownBTW server.");
                MessageBox.Show($"Error updating MotorTownBTW server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            try
            {
                //TODO
                logger.Info("MotorTownBTW server configuration created successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to create MotorTownBTW server configuration.");
                MessageBox.Show($"Error creating MotorTownBTW server configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            try
            {
                //TODO
                logger.Info("MotorTownBTW server started successfully.");
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to start MotorTownBTW server.");
                MessageBox.Show($"Error starting MotorTownBTW server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            try
            {
                await Task.Run(() =>
                {
                    Functions.ServerConsole.SetMainWindow(p.MainWindowHandle);
                    Functions.ServerConsole.SendWaitToMainWindow("^c");                 //Trying to gracefully exit
                    p.WaitForExit(20000);
                });
                logger.Info("MotorTownBTW server stopped successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to stop MotorTownBTW server.");
                MessageBox.Show($"Error stopping MotorTownBTW server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
