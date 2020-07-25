using Assets.Scripts.Common;
using Assets.Scripts.Common.SettingsManagement;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace Assets.Scripts.Screeps3D
{
    public class GoToRoomEventArgs
    {
        public GoToRoomEventArgs(string roomName, int seconds)
        {
            RoomName = roomName;
            Seconds = seconds;
        }

        public string RoomName { get; set; }
        public float Seconds { get; set; }
    }

    public class TwitchClient : BaseSingleton<TwitchClient>
    {
        private static event EventHandler<bool> OnEnableIntegration;

        private static bool _ENABLE_INTEGRATION = false;

        [Setting("Twitch/General", "Enable")]
        private static bool ENABLE_INTEGRATION
        {
            get => _ENABLE_INTEGRATION; set
            {
                if (CmdArgs.ForceEnableTwitch)
                {
                    Debug.Log("Twitch is force enabled by -twitch argument");
                    _ENABLE_INTEGRATION = true;
                }
                else
                {
                    _ENABLE_INTEGRATION = value;
                }

                OnEnableIntegration?.Invoke(null, _ENABLE_INTEGRATION);
            }
        }

        [Setting("Twitch/General", "Token", secret: true)]
        private static string BOT_TOKEN;
        [Setting("Twitch/General", "Channel")]
        private static string CHANNEL;
        [Setting("Twitch/General", "Username")]
        private static string USERNAME;

        // TODO: not making this a setting you can configure, cause if it is updated from twitch, how do we know the settingname? it is not stored either
        //[Setting("Twitch/General", "!info")]
        //private static string STREAM_INFO; 
        private const string PP_TWITCH_INFO = "twitch:stream:info";

        public Client client;


        public event EventHandler<GoToRoomEventArgs> OnGoToRoom;

        private void Awake()
        {
            OnEnableIntegration += TwitchClient_OnEnableIntegration;
        }

        private void TwitchClient_OnEnableIntegration(object sender, bool enabled)
        {
            if (enabled)
            {
                InitializeAndConnect(CHANNEL, BOT_TOKEN, USERNAME);
            }
            else
            {
                client?.Disconnect();
            }

            // TODO: handle disconnect
        }

        private void Start()
        {
            // Script should be running always, this is handled in editor settings though, setting it like this is not reccomended.
            // Application.runInBackground = true;
        }

        private void OnDestroy()
        {
            Debug.Log("TwitchClient destroyed?");
            if (client != null)
            {
                client.OnConnected -= Client_OnConnected;
                client.OnJoinedChannel -= Client_OnJoinedChannel;
                client.OnLeftChannel -= Client_OnLeftChannel;
                client.OnDisconnected -= Client_OnDisconnected;
                client.OnConnectionError -= Client_OnConnectionError;
                client.OnUserTimedout -= Client_OnUserTimedout;


                client.OnChatCommandReceived -= Client_OnChatCommandReceived;

                client.OnMessageReceived -= Client_OnMessageReceived;
                client.Disconnect(); 
            }
        }

        public void SendTwitchMessage(string message)
        {
            client.SendMessage(client.JoinedChannels[0], message);
        }

        private void InitializeAndConnect(string channel, string accessToken, string username)
        {
            try
            {
                if (client == null)
                {
                    bool settingsValid = true;
                    var sb = new StringBuilder();

                    if (string.IsNullOrEmpty(channel))
                    {
                        settingsValid = false;
                        sb.AppendLine($"<size=40><b>Missing Channelname</b></size>");
                    }

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        settingsValid = false;
                        sb.AppendLine($"<size=40><b>Missing token</b></size>");
                    }

                    if (string.IsNullOrEmpty(username))
                    {
                        settingsValid = false;
                        sb.AppendLine($"<size=40><b>Missing username</b></size>");
                    }

                    if (!settingsValid)
                    {

                        NotifyText.Message(sb.ToString(), Color.red);
                        return;
                    }

                    var credentials = new ConnectionCredentials(username, accessToken);
                    client = new Client();
                    client.Initialize(credentials, channel);

                    client.OnConnected += Client_OnConnected;
                    client.OnJoinedChannel += Client_OnJoinedChannel;
                    client.OnLeftChannel += Client_OnLeftChannel;
                    client.OnDisconnected += Client_OnDisconnected;
                    client.OnConnectionError += Client_OnConnectionError;
                    client.OnUserTimedout += Client_OnUserTimedout;


                    client.OnChatCommandReceived += Client_OnChatCommandReceived;

                    client.OnMessageReceived += Client_OnMessageReceived;

                    client.DisableAutoPong = false;
                }

                client.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }
        }

        private void Client_OnUserTimedout(object sender, TwitchLib.Client.Events.OnUserTimedoutArgs e)
        {
            Debug.Log($"The bot {e.UserTimeout.Username} timeout {e.UserTimeout.TimeoutReason}");
        }

        private void Client_OnConnectionError(object sender, TwitchLib.Client.Events.OnConnectionErrorArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} connection err {e.Error.Message}");
        }

        private void Client_OnDisconnected(object sender, TwitchLib.Communication.Events.OnDisconnectedEventArgs e)
        {
            Debug.Log($"The bot just disconncted the channel!?!??!?!");
        }

        private void Client_OnLeftChannel(object sender, TwitchLib.Client.Events.OnLeftChannelArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} just left the channel: {e.Channel}");
        }

        private void Client_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
            client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
        }

        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

            if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
            {
                Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
            }
        }

        /*
         * You should also be aware of the restrictions of a bot. To prevent from spam bots and bot misuse, 
         * Twitch have placed restrictions on how many messages can be sent every 30 seconds, how many whisper can be sent, and how many new users can be whispered by a bot. 
         * You can put a request in to Twitch to have your bot upgraded to a “known bot” status that will improve these limits somewhat. 
         * One thing that can sometimes help viewer receive whispers from the bot is if the viewer follows the bot and/or whispers the bot first. 

         */

        private void Client_OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
        {
            Debug.Log($"{e.Command.CommandText}: {e.Command.ArgumentsAsString}");
            // TODO: !help with a list of commands
            // !info that spits out a link to the event page.
            // allow moderators to !setinfo and persist link, could be link to info page :thinking: // can probably jsut hardcode a list of users for start.
            // TODO: camera actions !topdown !zoom !rotate !follow
            switch (e.Command.CommandText)
            {
                case "help":
                    client.SendMessage(client.JoinedChannels[0], "TODO: add a fancy help/command system :P !goto ");
                    break;
                case "room":
                case "goto":
                    // Raise goto event, and let RoomChooser listen for it.
                    // TODO: validate we are actually connected to the server :smirk:
                    var arguments = e.Command.ArgumentsAsList;
                    if (arguments.Count > 0)
                    {
                        var userName = e.Command.ChatMessage.Username;
                        var roomName = arguments[0];
                        int seconds = 30;
                        var message = $"{userName} told me to go to {roomName}";
                        if (arguments.Count >= 2)
                        {
                            int.TryParse(arguments[1], out seconds);
                            message += $" for {seconds}";
                        }

                        NotifyText.Message(message, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
                        OnGoToRoom?.Invoke(this, new GoToRoomEventArgs(roomName, seconds));
                    }
                    break;
                case "server":
                    // tell what server we are on
                    break;
                case "tick":
                    // tell what tick we are on? :thinking:
                    break;
                case "connect":
                    // press the connect button in case the stream has disconnected.
                    break;
                case "warpath":
                    // message the chat with pvp battles / conflicts should this perhaps be something running on a timer?
                    //Apparently loan is utilized https://www.leagueofautomatednations.com/vk/battles.json

                    break;
                case "pvp":
                    // message the chat with pvp battles from the experimental endpoint.

                    break;
                case "rcl":
                    // List combined RCL rankings.
                    break;
                case "nextbattle":
                    // goes to the next "interesting" battle, e.g. classifications from warpath.

                    break;
                case "info":
                    // goes to the next "interesting" battle, e.g. classifications from warpath.
                    var info = PlayerPrefs.GetString(PP_TWITCH_INFO, string.Empty);
                    if (!string.IsNullOrEmpty(info))
                    {
                        SendTwitchMessage(info);
                    }

                    break;
                ////case "setinfo":
                ////    var allowed = new[] // TODO: detect mods & people allowed
                ////    {
                ////        "thmsndk",
                ////        "ags131"
                ////    };

                ////    if (allowed.Contains(e.Command.ChatMessage.Username))
                ////    {
                ////        PlayerPrefs.SetString(PP_TWITCH_INFO, e.Command.ArgumentsAsString);
                ////    }
                ////    break;
                default:
                    break;
                    // send a random message at a timer mentioning the channel and the warpath channels
            }
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Username != USERNAME)
            {
                //Debug.Log($"{e.ChatMessage.Username}: {e.ChatMessage.Message}");
            }
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    client.SendMessage(client.JoinedChannels[0], "This is a test message from the bot / 3D client");
            //}
        }
    }
}
