# WindowsGSM.MotorTownBTW
🧩 WindowsGSM plugin that provides MotorTownh Behind The Wheel Dedicated server support!

# WindowsGSM Installation: 
1. Download  WindowsGSM https://windowsgsm.com/ 
2. Create a Folder at a Location where you want all Servers to be Installed and Run.
4. Drag WindowsGSM.Exe into the previously created folder and execute it.

# Plugin Installation:
1. Download [Latest](https://github.com/TheRealSarcasmO/WindowsGSM.MotorTownBTW) release
2. Extract then Move **MotorTownBTW.cs** folder to **plugins** folder
3. OR Press on the Puzzle Icon on the bottom left side, install this plugin by navigating to it and select the Zip File.
4. Click **[RELOAD PLUGINS]** button or restart WindowsGSM
5. Navigate "Servers" and Click "Install Game Server" and find "American Truck Simulator Dedicated Server [AmericanTruckSimulator.cs]

### The Game
🕹️ https://store.steampowered.com/app/1369670/Motor_Town_Behind_The_Wheel/

### Dedicated server info
🖥️ https://steamdb.info/app/2223650/info/


###// Not used yet but here in case it is used in the future
#### Registering Game Server Login Token (GSLT)
##### To navigate this on WGSM click the Edit config button and you will see the Server GSLT field
1. Generate a token for your game server [View](http://steamcommunity.com/dev/managegameservers) 
2. App ID of the dedicated server (2223650)
3. Memo (text stored with the account, just shown here to help you keep track)
4. Paste the Login Token on your WGSM GSLT text field.

### NOTE
- max_players this can't be more than 232ish I think 
- When you Install/Start the server you need to copy 

## Set up
1. Run the game, not the server, and exit
2. It will Generate DedicatedServerConfig_Sample.json in the server folder
3. Select your newly installed server and click Browse then click Server Files
4. Click on DedicatedServerConfig.json and open it to edit it.
5. Edit server name, player counts, etc.
6. Add your Admins if you want using UniqueNetID = SteamID64 and then their "Nickname" = MotorTown username
7. Then "save as"  DedicatedServerConfig.json
8. Start the Server

## How to copy client save files to the dedicated server
Copy client save folder to server save folder
Client save folder: %appdata%..\Local\MotorTown\Saved\SaveGames
Server save folder: <Dedicated Server Local Files Folder>\Saved\SaveGames
(Note that only the first character's world is compatible with the dedicated server)

### Note
Dedicated Server is 'black console screen and no input is available'
If you are using a router, you need to Port Forward Port 7777 and 27015 (UDP/TCP)

### Steam Cloud?
No (At least for now)

### Should I copy SaveGames from/to User Folder?
It's optional.
The client uses 'User' Folder and Dedicated server uses its installed Folder
You can copy from/to if you want.
But only the first character's world is used by the Dedicated server

#Official ReadMe.txt
```
[How to Run Dedicated Server]
1. Edit DedicatedServerConfig_Sample.json
   <ServerName>: This will be shown at server list
   <ServerMessage>: This will be shown to player after connect. Use \n for a New Line
   <MaxPlayers>: Set max player number. Recommend 10 for average system. 20 for high performance system.
   <Admins>: Use this to give admin to players 
             To aquire UniqueNetId and Nickname
               A. Open Server using game client
               B. Add admin to players
               C. Open game config file(%appdata%..\Local\MotorTown\Saved\Config\Windows\GameUserSettings)
               D. Search 'Admins'
2. Rename DedicatedServerConfig_Sample.json to DedicatedServerConfig.json
3. Launch Dedicated Server using RunDedicatedServer.bat or from Steam Library(Steam Login Required)

[Save Files Path]
Saved\SaveGames
(You can copy your Client Game Save files into this folder from '%appdata%..\Local\MotorTown\Saved\SaveGames')
```
Since you are using this plugin you don't have to worry about most of that since you launch from here.

## SO you need to contact me?
### https://discord.gg/rrsQEHUpX2

# License
//TODO - This project is licensed under the MIT License - see the <a href="https://github.com/TheRealSarcasmO/WindowsGSM.MotorTownBTW/blob/main/LICENSE">LICENSE.md</a> file for details
