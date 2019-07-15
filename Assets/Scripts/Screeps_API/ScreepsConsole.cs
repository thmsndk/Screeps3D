using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps_API
{
    public class ScreepsConsole : MonoBehaviour
    {

        public Action<ConsoleMessage> OnConsoleMessage;
        public Action<string> OnConsoleError;
        public Action<string> OnConsoleResult;
        
        private Queue<JSONObject> queue = new Queue<JSONObject>();

        internal List<IConsoleClientAbuse> consoleClientAbuses = new List<IConsoleClientAbuse>();

        private void Start()
        {
            ScreepsAPI.OnConnectionStatusChange += OnConnectionStatusChange;
            consoleClientAbuses.Add(new FontColor());
            consoleClientAbuses.Add(new RoomLink());
        }

        public void Input(string javascript)
        {
            ScreepsAPI.Http.ConsoleInput(AddEscapes(javascript));
        }

        private void OnConnectionStatusChange(bool connected)
        {
            if (connected)
            {
                ScreepsAPI.Socket.Subscribe(string.Format("user:{0}/console", ScreepsAPI.Me.UserId), RecieveData);
            }
        }

        private void RecieveData(JSONObject obj)
        {
            queue.Enqueue(obj);
        }

        private void Update()
        {
            if (queue.Count == 0)
                return;
            UnpackData(queue.Dequeue());
        }

        private void UnpackData(JSONObject obj)
        {
            var messages = obj["messages"];
            if (messages != null)
            {
                var log = messages["log"];
                if (log != null && OnConsoleMessage != null)
                {
                    foreach (var msgData in log.list)
                    {
                        var message = new ConsoleMessage(msgData.str);

                        foreach (var abuser in consoleClientAbuses)
                        {
                           abuser.Abuse(message);
                        }

                        OnConsoleMessage(message);
                    }
                }
                var results = messages["results"];
                if (results != null && OnConsoleResult != null)
                {
                    foreach (var resultsData in results.list)
                    {
                        OnConsoleResult(RemoveEscapes(resultsData.str));
                    }
                }
            }

            var errorData = obj["error"];
            if (errorData != null && OnConsoleError != null)
            {
                OnConsoleError(RemoveEscapes(errorData.str));
            }

            ScreepsAPI.Instance.IncrementTime();
        }

        private static string AddEscapes(string str)
        {
            str = str.Replace("\"", "\\\"");
            return str;
        }

        private static string RemoveEscapes(string str)
        {
            str = str.Replace("\\n", "\n");
            str = str.Replace("\\\\", "\\");
            // handle "html" <a href=\"...
            str = str.Replace("\\\"", "\"");
            return str;
        }

        public class ConsoleMessage
        {
            public ConsoleMessage(string message)
            {
                RawMessage = message;
                Message = RemoveEscapes(message);
            }
            public string Message { get; set; }
            public string RawMessage { get; set; }

            public static implicit operator string(ConsoleMessage message) { return message.Message.ToString(); }
        }
    }
}