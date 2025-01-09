using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Query;
using Newtonsoft.Json.Linq;
using WindowsGSM.GameServer.Engine;

namespace WindowsGSM.Plugins
{
    public class MotorTownBTW
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
        public MotorTownBTW(ServerConfig serverData) => _serverConfig = serverData;
        private readonly ServerConfig _serverConfig;
        public string ErrorMessage, NoticeMessage;

        // - Game server Fixed variables
        public string FullName = "MotorTownBTW Dedicated Server";
        public bool AllowsEmbedConsole = true;
        public int PortIncrements = 2;
        public object QueryMethod = new UT3();

        // - Game server default values
        public string Port = "27015";
        public string QueryPort = "27016";
        public string Defaultmap = "jeju_world";
        public string Maxplayers = "200";
        public string Additional = "jeju_world?listen? -server -log -useperfthreads";

        // - SteamCMD install settings
        public bool loginAnonymous = false;
        public string AppId = "2223650";

        // - Create a default cfg for the game server after installation
        public async void CreateServerCFG()
        {
            string configFilePath = Functions.ServerPath.GetServersServerFiles(_serverConfig.ServerID, "DedicatedServerConfig.json");
            string sampleConfigFilePath = Functions.ServerPath.GetServersServerFiles(_serverConfig.ServerID, "DedicatedServerConfig_Sample.json");

            if (File.Exists(sampleConfigFilePath))
            {
                var sampleConfig = JObject.Parse(File.ReadAllText(sampleConfigFilePath));

                if (!File.Exists(configFilePath))
                {
                    File.WriteAllText(configFilePath, sampleConfig.ToString());
                }
                else
                {
                    var currentConfig = JObject.Parse(File.ReadAllText(configFilePath));

                    foreach (var property in sampleConfig.Properties())
                    {
                        if (!currentConfig.ContainsKey(property.Name))
                        {
                            currentConfig.Add(property.Name, property.Value);
                        }
                    }

                    File.WriteAllText(configFilePath, currentConfig.ToString());
                }
            }
        }

        // - Start server function, return its Process to WindowsGSM
        public async Task<Process> Start()
        {
            string shipExePath = Functions.ServerPath.GetServersServerFiles(_serverConfig.ServerID, "MotorTown\\Binaries\\Win64\\MotorTownServer-Win64-Shipping.exe");

            if (string.IsNullOrWhiteSpace(shipExePath) || !File.Exists(shipExePath))
            {
                ErrorMessage = $"{Path.GetFileName(shipExePath)} not found ({shipExePath})";
                return null;
            }

            string param = string.IsNullOrWhiteSpace(_serverConfig.ServerMap) ? string.Empty : _serverConfig.ServerMap;
            param += "?listen";
            param += string.IsNullOrWhiteSpace(_serverConfig.ServerName) ? string.Empty : $"?SessionName=\"{_serverConfig.ServerName}\"";
            param += string.IsNullOrWhiteSpace(_serverConfig.ServerIP) ? string.Empty : $"?MultiHome={_serverConfig.ServerIP}";
            param += string.IsNullOrWhiteSpace(_serverConfig.ServerPort) ? string.Empty : $"?Port={_serverConfig.ServerPort}";
            param += string.IsNullOrWhiteSpace(_serverConfig.ServerMaxPlayer) ? string.Empty : $"?MaxPlayers={_serverConfig.ServerMaxPlayer}";
            param += string.IsNullOrWhiteSpace(_serverConfig.ServerQueryPort) ? string.Empty : $"?QueryPort={_serverConfig.ServerQueryPort}";
            param += $"{_serverConfig.ServerParam} -server -log";

            Process p = new Process
            {
                StartInfo =
                {
                    FileName = shipExePath,
                    Arguments = param,
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
            p.Start();

            return p;
        }

        // - Stop server function
        public async Task Stop(Process p)
        {
            await Task.Run(() =>
            {
                p.Kill();
            });
        }

        // - Install server function
        public async Task<Process> Install()
        {
            // Ensure that the ServerID and paths are not null or empty
            if (string.IsNullOrWhiteSpace(_serverConfig.ServerID))
            {
                ErrorMessage = "ServerID is null or empty";
                return null;
            }

            // Manually create the necessary directory structure
            string serversServerFilesPath = Functions.ServerPath.GetServersServerFiles(_serverConfig.ServerID);
            Console.WriteLine($"serversServerFilesPath: {serversServerFilesPath}"); // Debug statement
            if (string.IsNullOrWhiteSpace(serversServerFilesPath))
            {
                ErrorMessage = "serversServerFilesPath is null or empty";
                return null;
            }

            string steamAppsPath = Path.Combine(serversServerFilesPath, "steamapps");
            Console.WriteLine($"steamAppsPath: {steamAppsPath}"); // Debug statement
            if (string.IsNullOrWhiteSpace(steamAppsPath))
            {
                ErrorMessage = "steamAppsPath is null or empty";
                return null;
            }

            Directory.CreateDirectory(steamAppsPath);

            string forceInstallDir = Functions.ServerPath.GetServersServerFiles(_serverConfig.ServerID);
            Console.WriteLine($"forceInstallDir: {forceInstallDir}"); // Debug statement
            string appId = AppId;  // Use the AppId from the class
            Console.WriteLine($"appId: {appId}"); // Debug statement
            string customParam = "-beta test -betapassword motortowndedi";

            // Ensure paths are not null or empty
            if (string.IsNullOrWhiteSpace(forceInstallDir) || string.IsNullOrWhiteSpace(appId))
            {
                ErrorMessage = "Install directory or AppId is null or empty";
                return null;
            }

            // Construct the parameter string
            string param = $"+force_install_dir \"{forceInstallDir}\" +login anonymous +app_update {appId} {customParam} validate +exit";
            Console.WriteLine($"param: {param}"); // Debug statement

            // Ensure steamcmd.exe is available
            string steamCmdExe = Path.Combine(ServerPath.GetBin("steamcmd"), "steamcmd.exe");
            Console.WriteLine($"steamCmdExe: {steamCmdExe}"); // Debug statement
            if (string.IsNullOrWhiteSpace(steamCmdExe) || !File.Exists(steamCmdExe))
            {
                ErrorMessage = "steamcmd.exe not found";
                return null;
            }

            // Start the SteamCMD process
            Process process = new Process
            {
                StartInfo =
                {
                    FileName = steamCmdExe,
                    Arguments = param,
                    WorkingDirectory = Path.GetDirectoryName(steamCmdExe),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.Start();

            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode != 0)
            {
                ErrorMessage = $"SteamCMD process exited with code {process.ExitCode}";
                return null;
            }

            return process;
        }

        // - Update server function
        public async Task<Process> Update(bool validate = false, string custom = null)
        {
            var steamCMDAgent = new WindowsGSM.GameServer.Engine.SteamCMDAgent(_serverConfig);
            Process p = await steamCMDAgent.Update(validate, custom);
            ErrorMessage = steamCMDAgent.Error;

            return p;
        }

        // - Check if the installation is successful
        public bool IsInstallValid()
        {
            var steamCMDAgent = new WindowsGSM.GameServer.Engine.SteamCMDAgent(_serverConfig);
            return steamCMDAgent.IsInstallValid();
        }

        // - Check if the directory contains the necessary files for import
        public bool IsImportValid(string path)
        {
            var steamCMDAgent = new WindowsGSM.GameServer.Engine.SteamCMDAgent(_serverConfig);
            return steamCMDAgent.IsImportValid(path);
        }

        // - Get Local server version
        public string GetLocalBuild()
        {
            var steamCMDAgent = new WindowsGSM.GameServer.Engine.SteamCMDAgent(_serverConfig);
            return steamCMDAgent.GetLocalBuild();
        }

        // - Get Latest server version
        public async Task<string> GetRemoteBuild()
        {
            var steamCMDAgent = new WindowsGSM.GameServer.Engine.SteamCMDAgent(_serverConfig);
            return await steamCMDAgent.GetRemoteBuild();
        }

        // - Check installation validity
        public bool CheckInstallationValidity()
        {
            var steamCMDAgent = new SteamCMDAgent(_serverConfig);
            return steamCMDAgent.IsInstallValid();
        }
    }
}
