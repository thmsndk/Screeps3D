using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Screeps3D;
using UnityEngine;
using UnityEngine.Networking;

namespace Screeps_API
{
    public class ScreepsHTTP : MonoBehaviour
    {
        public string Token { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestMethod"></param>
        /// <param name="path"></param>
        /// <param name="body"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError">If you sbuscribe to onError, the request will no longer re-auth and call the endpoint again, you will be responsible for doing that.</param>
        /// <param name="timeout"></param>
        /// <param name="skipAuth"></param>
        /// <param name="noNotification">If true UI notification will not be displayed on error.</param>
        /// <returns></returns>
        public IEnumerator<UnityWebRequestAsyncOperation> Request(string requestMethod, string path, RequestBody body = null,
            Action<string> onSuccess = null, Action onError = null, int timeout = 0, bool skipAuth = false, bool noNotification = false)
        {
            // TODO: all theese requests needs to be queued to not hit request limits
            //Debug.Log(string.Format("HTTP: attempting {0} to {1}", requestMethod, path));
            UnityWebRequest www;
            var fullPath = path.StartsWith("/api") ? ScreepsAPI.Cache.Address.Http(path) : path;
            if (requestMethod == UnityWebRequest.kHttpVerbGET)
            {
                if (body != null)
                {
                    fullPath = fullPath + body.ToQueryString();
                }
                www = UnityWebRequest.Get(fullPath);
            }
            else if (requestMethod == UnityWebRequest.kHttpVerbPOST)
            {
                www = new UnityWebRequest(fullPath, "POST");
                if (body != null)
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(body.ToString());
                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
            }
            else
            {
                var message = $"HTTP: request method {requestMethod} unrecognized";
                Debug.Log(message);
                throw new Exception(message);
            }

            Action<UnityWebRequest> onComplete = (UnityWebRequest outcome) =>
            {
                var responseHeaders = outcome.GetResponseHeaders();

                if (responseHeaders != null)
                {
                    var rateLimitHeaders = responseHeaders.Where(x => x.Key.StartsWith("X-RateLimit"));
                    var rateLimitDebug = new StringBuilder();

                    if (rateLimitHeaders.Any())
                    {
                        foreach (var header in rateLimitHeaders)
                        {
                            rateLimitDebug.AppendLine($"{header.Key}: {header.Value}");
                        }

                        Debug.Log(rateLimitDebug.ToString());
                    }
                }

                if (outcome.isNetworkError || outcome.isHttpError)
                {
                    // TODO: rate limit https://github.com/screepers/node-screeps-api/blob/4e15d49c45e9b5cc3808122db6597c2d45537cb5/src/RawAPI.js#L389-L405
                    if (!noNotification)
                    {
                        NotifyText.Message(string.Format("HTTP: error ({1}), reason: {0}", outcome.error,
                            outcome.responseCode));
                    }
                    Debug.Log(string.Format("HTTP: error ({1}), reason: {0}", outcome.error, outcome.responseCode));
                    if (onError != null)
                    {
                        onError();
                    }
                    else
                    {
                        if (skipAuth)
                        {
                            Request(requestMethod, path, body, onSuccess);
                        }
                        else
                        {
                            Auth((reply) =>
                            {
                                Request(requestMethod, path, body, onSuccess);
                            }, () =>
                            {
                                ScreepsAPI.Instance.AuthFailure();
                            });
                        }
                    }
                }
                else
                {
                    // Debug.Log(string.Format("HTTP: success, data: \n{0}", outcome.downloadHandler.text));
                    if (outcome.downloadHandler.text.Contains("token"))
                    {
                        var reply = new JSONObject(outcome.downloadHandler.text);
                        var token = reply["token"];
                        if (token != null)
                        {
                            Token = token.str;
                            Debug.Log($"HTTP: found a token! {Token}");
                        }
                    }

                    onSuccess?.Invoke(outcome.downloadHandler.text);
                }
            };

            www.timeout = timeout;

            var request = SendRequest(www, onComplete);

            StartCoroutine(request);

            return request;
        }

        private IEnumerator<UnityWebRequestAsyncOperation> SendRequest(UnityWebRequest www, Action<UnityWebRequest> onComplete)
        {
            if (Token != null)
            {
                www.SetRequestHeader("X-Token", Token);
                www.SetRequestHeader("X-Username", Token);
            }
            yield return www.SendWebRequest();
            onComplete(www);
        }

        public void Auth(Action<string> onSuccess, Action onError = null, bool noNotification = false)
        {
            if (!string.IsNullOrEmpty(ScreepsAPI.Cache.Credentials.Token))
            {
                Token = ScreepsAPI.Cache.Credentials.Token;
                Request("GET", "/api/auth/me", null, onSuccess, onError, noNotification: noNotification);
            }
            else
            {
                var body = new RequestBody();
                body.AddField("email", ScreepsAPI.Cache.Credentials.Email);
                body.AddField("password", ScreepsAPI.Cache.Credentials.Password);
                Request("POST", "/api/auth/signin", body, onSuccess, onError, noNotification: noNotification);
            }
        }

        public void GetUser(Action<string> onSuccess, bool noNotification = false)
        {
            Request("GET", "/api/auth/me", null, onSuccess, noNotification: noNotification);
        }

        public void ConsoleInput(string message, bool noNotification = false)
        {
            var body = new RequestBody();
            body.AddField("expression", message);
            body.AddField("shard", "shard0");
            Request("POST", "/api/user/console", body, noNotification: noNotification);
        }

        public void GetRoom(string roomName, string shard, Action<string> callback, bool noNotification = false)
        {
            var body = new RequestBody();
            body.AddField("room", roomName);
            body.AddField("encoded", "0");
            body.AddField("shard", shard);

            Request("GET", "/api/game/room-terrain", body, callback, noNotification: noNotification);
        }

        public void GetRooms(string userId, Action<string> onSuccess, bool noNotification = false)
        {
            var body = new RequestBody();
            body.AddField("id", userId);
            Request("GET", "/api/user/rooms", body, onSuccess, noNotification: noNotification);
        }

        public void GetServerList(Action<string> onSuccess, Action onError, bool noNotification = false)
        {
            Request("POST", "https://screeps.com/api/servers/list", onSuccess: onSuccess, onError: onError, skipAuth: true, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GetVersion(Action<string> onSuccess, Action onError, bool noNotification = false)
        {
            // this call does not require authentication, and thus we only need the hostname
            return Request("GET", "/api/version", onSuccess: onSuccess, onError: onError, timeout: 2, skipAuth: true, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GetMapStats(List<string> rooms, string shard, string statName, Action<string, string> onSuccess, bool noNotification = false)
        {
            /*
             https://github.com/screepers/node-screeps-api/blob/HEAD/docs/Endpoints.md
             [POST] https://screeps.com/api/game/map-stats
                post data: { rooms: [ <room name> ], statName: "..."}
                statName can be "owner0", "claim0", or a stat (see the enumeration above) followed by an interval
                if it is owner0 or claim0, there's no separate stat block in the response
                response: { ok, stats: { <room name>: { status, novice, own: { user, level }, <stat>: [ { user, value } ] } }, users: { <user's _id>: { _id, username, badge: { type, color1, color2, color3, param, flip } } } }
                status and novice are as per the room-status call
                level can be 0 to indicate a reserved room
                */

            var body = new RequestBody();
            var jsonRooms = JSONObject.Create(JSONObject.Type.ARRAY);
            rooms.ForEach((room) => jsonRooms.Add(room));
            body.AddField("rooms", jsonRooms);
            body.AddField("statName", statName);
            body.AddField("shard", shard);

            Action<string> onRequestSuccess = (json) =>
            {
                onSuccess(shard, json);
            };

            return Request("POST", "/api/game/map-stats", body, onSuccess: onRequestSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GenerateUniqueFlagName(string shard, Action<string> onSuccess, bool noNotification = false)
        {
            // POST https://screeps.com/api/game/gen-unique-flag-name
            /* body
             * {"shard":"shard3"}
             * response
             * {"ok":1,"name":"Flag1"}
             */
            var body = new RequestBody();
            body.AddField("shard", shard);

            return Request("POST", "/api/game/gen-unique-flag-name", body, onSuccess: onSuccess, noNotification: noNotification);

        }

        public IEnumerator<UnityWebRequestAsyncOperation> CreateFlag(string shard, string room, int x, int y, string name, int color, int secondaryColor, Action<string> onSuccess, bool noNotification = false)
        {
            //  POST https://screeps.com/api/game/create-flag
            // request: {"x":38,"y":23,"name":"Flag1","color":10,"secondaryColor":7,"room":"E19S38","shard":"shard3"}
            // response: {"ok":1,"result":{"n":1,"nModified":1,"ok":1},"connection":{"id":16,"host":"dhost3.srv.screeps.com","port":25270},"message":{"parsed":true,"index":75,"raw":{"type":"Buffer","data":[75,0,0,0,210,79,229,77,233,142,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"data":{"type":"Buffer","data":[75,0,0,0,210,79,229,77,233,142,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"bson":{},"opts":{"promoteLongs":true,"promoteValues":true,"promoteBuffers":false},"length":75,"requestId":1306873810,"responseTo":6065897,"responseFlags":8,"cursorId":"0","startingFrom":0,"numberReturned":1,"documents":[{"n":1,"nModified":1,"ok":1}],"cursorNotFound":false,"queryFailure":false,"shardConfigStale":false,"awaitCapable":true,"promoteLongs":true,"promoteValues":true,"promoteBuffers":false,"hashedName":"8cf87ebd96d4f56356284e048c6646c112baf617"}}

            var body = new RequestBody();
            body.AddField("shard", shard);
            body.AddField("room", room);
            body.AddField("name", name);
            body.AddField("x", x);
            body.AddField("y", y);
            body.AddField("color", color);
            body.AddField("secondaryColor", secondaryColor);

            return Request("POST", "/api/game/create-flag", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> ChangeFlagColor(string shard, string room, string name, int color, int secondaryColor, Action<string> onSuccess, bool noNotification = false)
        {
            // POST https://screeps.com/api/game/change-flag-color
            // Request: {"room":"E19S38","shard":"shard3","name":"Flag1","color":10,"secondaryColor":7}
            // Response: {"ok":1,"result":{"n":1,"nModified":1,"ok":1},"connection":{"id":4,"host":"dhost3.srv.screeps.com","port":25270},"message":{"parsed":true,"index":75,"raw":{"type":"Buffer","data":[75,0,0,0,186,216,193,77,139,130,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"data":{"type":"Buffer","data":[75,0,0,0,186,216,193,77,139,130,92,0,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"bson":{},"opts":{"promoteLongs":true,"promoteValues":true,"promoteBuffers":false},"length":75,"requestId":1304549562,"responseTo":6062731,"responseFlags":8,"cursorId":"0","startingFrom":0,"numberReturned":1,"documents":[{"n":1,"nModified":1,"ok":1}],"cursorNotFound":false,"queryFailure":false,"shardConfigStale":false,"awaitCapable":true,"promoteLongs":true,"promoteValues":true,"promoteBuffers":false,"hashedName":"8cf87ebd96d4f56356284e048c6646c112baf617"}}

            var body = new RequestBody();
            body.AddField("shard", shard);
            body.AddField("room", room);
            body.AddField("name", name);
            body.AddField("color", color);
            body.AddField("secondaryColor", secondaryColor);

            return Request("POST", "/api/game/change-flag-color", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> RemoveFlag(string shard, string room, string name, Action<string> onSuccess, bool noNotification = false)
        {
            //  POST https://screeps.com/api/game/remove-flag
            // request: {"room":"E19S38","shard":"shard3","name":"Flag2"}
            // response: {"ok":1,"result":{"n":1,"nModified":1,"ok":1},"connection":{"id":8,"host":"dhost3.srv.screeps.com","port":25270},"message":{"parsed":true,"index":75,"raw":{"type":"Buffer","data":[75,0,0,0,142,100,226,151,150,40,22,2,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"data":{"type":"Buffer","data":[75,0,0,0,142,100,226,151,150,40,22,2,1,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,39,0,0,0,16,110,0,1,0,0,0,16,110,77,111,100,105,102,105,101,100,0,1,0,0,0,1,111,107,0,0,0,0,0,0,0,240,63,0]},"bson":{},"opts":{"promoteLongs":true,"promoteValues":true,"promoteBuffers":false},"length":75,"requestId":-1746770802,"responseTo":35006614,"responseFlags":8,"cursorId":"0","startingFrom":0,"numberReturned":1,"documents":[{"n":1,"nModified":1,"ok":1}],"cursorNotFound":false,"queryFailure":false,"shardConfigStale":false,"awaitCapable":true,"promoteLongs":true,"promoteValues":true,"promoteBuffers":false,"hashedName":"8cf87ebd96d4f56356284e048c6646c112baf617"}}

            var body = new RequestBody();
            body.AddField("shard", shard);
            body.AddField("room", room);
            body.AddField("name", name);

            return Request("POST", "/api/game/remove-flag", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> CheckUniqueFlag(string shard, string name, Action<string> onSuccess, bool noNotification = false)
        {
            // POST https://screeps.com/api/game/check-unique-flag-name
            // Request: {"name":"Flag1","shard":"shard3"}
            // Response: {"error":"name exists"} || {"ok":1}

            var body = new RequestBody();
            body.AddField("shard", shard);
            body.AddField("name", name);

            return Request("POST", "/api/game/check-unique-flag-name", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> CreateConstructionsite(string shard, string room, int x, int y, string structureType, Action<string> onSuccess, bool noNotification = false)
        {
            return CreateConstructionsite(shard, room, x, y, structureType, null, onSuccess, noNotification);
        }
        public IEnumerator<UnityWebRequestAsyncOperation> CreateConstructionsite(string shard, string room, int x, int y, string structureType, string name, Action<string> onSuccess, bool noNotification = false)
        {
            //  POST https://screeps.com/api/game/create-construction
            // request: {"room":"E19S35","shard":"shard3","name":"spawnName","structureType":"container","x":23,"y":29}
            // response: {"ok":1,"_id":"5ee3cde6b4b7d3fac1e6e2d8"}

            var body = new RequestBody();
            body.AddField("shard", shard);
            body.AddField("room", room);
            body.AddField("structureType", structureType);
            if (!string.IsNullOrEmpty(name))
            {
                body.AddField("name", name);
            }
            body.AddField("x", x);
            body.AddField("y", y);

            return Request("POST", "/api/game/create-construction", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GetWorldStatus(Action<string> onSuccess, bool noNotification = false)
        {
            /*
             https://github.com/screepers/node-screeps-api/blob/HEAD/docs/Endpoints.md
             [GET] https://screeps.com/api/user/world-status
                {
                    "ok": 1,
                    "status": "empty"
                }
                status can be lost, empty or normal, lost is when you loose all your spawns, empty is when you have respawned and not placed your spawn yet.
                */

            return Request("GET", "/api/user/world-status", onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> Respawn(Action<string> onSuccess, bool noNotification = false)
        {
            //  POST https://screeps.com/api/user/respawn
            var body = new RequestBody();
            body.Add(new JSONObject()); // an empty object is required in the request

            return Request("POST", "/api/user/respawn", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GetRespawnProhibitedRooms(Action<string> onSuccess, bool noNotification = false)
        {
            /*
             [GET] https://screeps.com/api/user/respawn-prohibited-rooms
                response: {
                    "ok": 1,
                    "rooms": []
                }
                */

            return Request("GET", "/api/user/respawn-prohibited-rooms", onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> PlaceSpawn(string shard, string room, int x, int y, string name, Action<string> onSuccess, bool noNotification = false)
        {
            //  POST https://screeps.com/api/game/place-spawn
            // request: {"room":"E19S35","shard":"shard3","name":"spawnName","x":23,"y":29}
            // response: {"ok":1,"_id":"5ee3cde6b4b7d3fac1e6e2d8"}

            var body = new RequestBody();
            body.AddField("shard", shard);
            body.AddField("room", room);

            if (!string.IsNullOrEmpty(name))
            {
                body.AddField("name", name);
            }

            body.AddField("x", x);
            body.AddField("y", y);

            return Request("POST", "/api/game/place-spawn", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> AddObjectIntent(string shard, string room, string name, JSONObject intent, Action<string> onSuccess, bool noNotification = false)
        {
            //  POST https://screeps.com/api/game/add-object-intent
            /* request: {
                        "room": "E19S38",
	                    "shard": "shard3",
	                    "_id": "room",
	                    "name": "destroyStructure",
	                    "intent": [{
			                    "id": "5e7ff81a7e00f81474101903",
			                    "roomName": "E19S38",
			                    "user": "5a44e109ac5a5f1d0146916e"

                            }
	                    ]
                    }*/

            // response: 

            var body = new RequestBody();
            body.AddField("room", room);
            body.AddField("shard", shard);
            body.AddField("_id", "room");
            body.AddField("name", name);
            body.AddField("intent", intent);

            Debug.LogError(body.ToString());

            return Request("POST", "/api/game/add-object-intent", body, onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> DestroyStructureObjectIntent(string shard, string room, string id, string userId, Action<string> onSuccess, bool noNotification = false)
        {
            var list = new JSONObject();
            var intent = new JSONObject();
            list.Add(intent);

            intent.AddField("id", id);
            intent.AddField("roomName", room);
            intent.AddField("user", userId);


            return AddObjectIntent(shard, room, "destroyStructure", list, onSuccess);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GetUserByName(string username, Action<string> onSuccess, bool noNotification = false)
        {
            // https://screeps.com/api/user/find?username=thmsn
            // {"ok":1,"user":{"_id":"5a44e109ac5a5f1d0146916e","username":"thmsn","badge":{"type":16,"color1":"#e026e3","color2":"#060606","color3":"#020202","param":54,"flip":false},"gcl":72599362}}

            return Request("GET", $"/api/user/find?username={username}", onSuccess: onSuccess, noNotification: noNotification);
        }

        public IEnumerator<UnityWebRequestAsyncOperation> GetUsersRoomsByUserId(string userId, Action<string> onSuccess, bool noNotification = false)
        {
            // https://screeps.com/api/user/rooms?id=5a44e109ac5a5f1d0146916e
            // {"ok":1,"shards":{"shard0":[],"shard1":[],"shard2":[],"shard3":["E18S37","E19S36","E19S38"]},"reservations":{"shard0":[],"shard1":[],"shard2":[],"shard3":[]}}
            return Request("GET", $"/api/user/find?username={userId}", onSuccess: onSuccess, noNotification: noNotification);
        }



        /* Experimental */
        public IEnumerator<UnityWebRequestAsyncOperation> GetExperimentalNukes(Action<string> onSuccess)
        {
            /*
             // https://screeps.com/api/experimental/nukes
             // for PS it requires screepsmod-admin-utils or another mod that implements the endpoint
            */

            return Request("GET", "/api/experimental/nukes", onSuccess: onSuccess);
        }

        public IEnumerator GetRoomTexture(string shard, string roomName, Action<Texture> response)
        {
            var roomTextureUrl = $"https://d3os7yery2usni.cloudfront.net/map/{shard}/{roomName}.png";

            if (ScreepsAPI.Cache.Type != SourceProviderType.Official)
            {
                // Private servers runs with a different url.
                roomTextureUrl = ScreepsAPI.Cache.Address.Http($"/assets/map/{roomName}.png");
            }

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(roomTextureUrl);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                //Texture myTexture = DownloadHandlerTexture.GetContent(www);
                response(myTexture);
            }
        }
    }
}