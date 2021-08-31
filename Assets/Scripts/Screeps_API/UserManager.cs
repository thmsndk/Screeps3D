using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps_API
{
    public class UserManager
    {
        private Dictionary<string, ScreepsUser> _users = new Dictionary<string, ScreepsUser>();

        private object _lockObj = new object();

        public ScreepsUser GetUser(string id)
        {
            if (id == null)
                return null;
            if (_users.ContainsKey(id))
            {
                return _users[id];
            } else
            {
                return null;
            }
        }

        internal void Reset()
        {
            _users = new Dictionary<string, ScreepsUser>();
        }
        internal ScreepsUser CacheUser(JSONObject data)
        {
            return CacheUser(null, data);
        }

        internal ScreepsUser CacheUser(string id, JSONObject data)
        {
            lock (_lockObj)
            {
                // Handle using GetUserByName
                if (data["user"] != null)
                {
                    data = data["user"];
                }

                // TODO: xxscreeps does not have _id, and the id is actually the key when unpacking room objects
                if (id == null)
                {
                    var idProperty = data["_id"];
                    if (idProperty != null && !idProperty.IsNull)
                    {
                        id = idProperty.str;
                    }
                }

                if (_users.ContainsKey(id)) return _users[id];

                Texture2D badge = null;
                var isNpc = false;
                var badgeData = data["badge"];
                SvgParams badgeParams = null;
                if (badgeData != null)
                {
                    badge = ScreepsAPI.Badges.Generate(badgeData, out badgeParams);
                }
                else
                {
                    isNpc = true;
                    badge = ScreepsAPI.Badges.Invader;
                }

                var username = "unknown";
                var nameData = data["username"];
                if (nameData != null)
                {
                    username = nameData.str;
                }

                var cpu = 10;
                var cpuData = data["cpu"];
                if (cpuData != null)
                {
                    cpu = (int)cpuData.n;
                }

                var user = new ScreepsUser(id, username, cpu, badge, isNpc);

                if (badgeParams != null)
                {
                    if (ColorUtility.TryParseHtmlString(badgeParams.color1, out var color1))
                    {
                        user.BadgeColor1 = color1;
                    }

                    if (ColorUtility.TryParseHtmlString(badgeParams.color2, out var color2))
                    {
                        user.BadgeColor2 = color2;
                    }

                    if (ColorUtility.TryParseHtmlString(badgeParams.color3, out var color3))
                    {
                        user.BadgeColor3 = color3;
                    }
                }

                _users[id] = user;
                return user; 
            }
        }

        internal ScreepsUser GetUserByName(string username)
        {
            return _users.SingleOrDefault(u => u.Value.Username == username).Value;
        }
    }
}