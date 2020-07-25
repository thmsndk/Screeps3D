using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace Screeps_API
{
    public class ScreepsSocket : MonoBehaviour
    {
        private WebSocket Socket { get; set; }
        private Dictionary<string, Action<JSONObject>> _subscriptions = new Dictionary<string, Action<JSONObject>>();
        private Queue<MessageEventArgs> _messages = new Queue<MessageEventArgs>();

        public Action<EventArgs> OnOpen;
        public Action<CloseEventArgs> OnClose;
        public Action<MessageEventArgs> OnMessage;
        public Action<ErrorEventArgs> OnError;

        private void OnDestroy()
        {
            UnsubAll();
            if (Socket != null) Socket.Close();
        }

        public void Connect()
        {
            if (Socket != null)
            {
                Socket.Close();
            }

            var protocol = ScreepsAPI.Cache.Address.Ssl ? "wss" : "ws";
            var port = ScreepsAPI.Cache.Type == SourceProviderType.Official ? "" : string.Format(":{0}", ScreepsAPI.Cache.Address.Ssl ? "443" : ScreepsAPI.Cache.Address.Port);

            var path = ScreepsAPI.Cache.Address.Path;
            if (path.StartsWith("/") && path.EndsWith("/"))
            {
                path = path.Substring(1);
            }

            var url = string.Format("{0}://{1}{2}{3}/socket/websocket", protocol, ScreepsAPI.Cache.Address.HostName, port, path);
            Debug.Log(url);

            Socket = new WebSocket(url);

            Socket.OnOpen += Open;
            Socket.OnError += Error;
            Socket.OnMessage += Message;
            Socket.OnClose += Close;
            Socket.Connect();
        }

        private void Open(object sender, EventArgs e)
        {
            try
            {
                Debug.Log("Socket Open");
                Socket.Send(string.Format("auth {0}", ScreepsAPI.Http.Token));
                if (OnOpen != null) OnOpen.Invoke(e);
            } catch (Exception exception)
            {
                Debug.Log("Exception in ScreepSocket.OnOpen");
                Debug.LogException(exception);
            }
        }

        private void Close(object sender, CloseEventArgs e)
        {
            try
            {
                Debug.Log(string.Format("Socket Closing: {0}", e.Reason));
                if (OnClose != null) OnClose.Invoke(e);
            } catch (Exception exception)
            {
                Debug.Log("Exception in ScreepSocket.OnClose");
                Debug.LogException(exception);
            }
        }

        private void Message(object sender, MessageEventArgs e)
        {
            _messages.Enqueue(e);
        }

        private void Error(object sender, ErrorEventArgs e)
        {
            try
            {
                Debug.Log(string.Format("Socket Error: {0}", e.Message));
                if (OnError != null) OnError.Invoke(e);
            } catch (Exception exception)
            {
                Debug.Log("Exception in ScreepSocket.OnError");
                Debug.LogException(exception);
            }
        }

        private void Update()
        {
            while (_messages.Count > 0)
            {
                ProcessMessage(_messages.Dequeue());
            }
        }

        private void ProcessMessage(MessageEventArgs e)
        {
            if (e == null || e.Data == null)
            {
                Debug.Log("recieved null data from server");
                return;
            }
            
            if (e.Data.Substring(0, 5) == "[\"err")
                Debug.Log(e.Data);
            
            if (e.Data.Substring(0, 3) == "gz:")
                Debug.Log(e.Data);
            
            // Debug.Log(string.Format("Socket Message: {0}", e.Data));
            var parse = e.Data.Split(' ');
            if (parse.Length == 3 && parse[0] == "auth" && parse[1] == "ok")
            {
                Debug.Log("socket auth success");
            }
            var json = new JSONObject(e.Data);
            FindSubscription(json);
            if (OnMessage != null) OnMessage.Invoke(e);
        }

        private void FindSubscription(JSONObject json)
        {
            var list = json.list;
            if (list == null || list.Count < 2 || !list[0].IsString || !_subscriptions.ContainsKey(list[0].str))
            {
                return;
            }
            if(list[0].str.IndexOf("room:")==0)
            {
                findAndUpdateTimeInRoomSubscription(list[1]);
            }
            _subscriptions[list[0].str].Invoke(list[1]);
        }

        private void findAndUpdateTimeInRoomSubscription(JSONObject json)
        {
            if(!json.HasField("gameTime") || json["gameTime"] == null)
            {
                return;
            }
            long currentTime = (long)json["gameTime"].n;
            ScreepsAPI.Instance.UpdateTime(currentTime);
        }

        public void Subscribe(string path, Action<JSONObject> callback)
        {
            //Debug.Log("subscribing " + path);
            Socket.Send(string.Format("subscribe {0}", path));
            _subscriptions[path] = callback;
        }

        public void Unsub(string path, bool remove = true)
        {
            //Debug.Log("unsub " + path);
            Socket.Send(string.Format("unsubscribe {0}", path));
            if (remove)
                _subscriptions.Remove(path);
        }

        private void UnsubAll()
        {
            if (Socket == null) return;
            foreach (var kvp in _subscriptions)
            {
                Unsub(kvp.Key, false);
            }
            _subscriptions.Clear();
        }

        public void Disconnect()
        {
            UnsubAll();
            Socket.Close();
        }
    }
}