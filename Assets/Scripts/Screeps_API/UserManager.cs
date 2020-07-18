using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps_API
{
    public class UserManager
    {
        private Dictionary<string, ScreepsUser> _users = new Dictionary<string, ScreepsUser>();

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

            var id = data["_id"].str;
            
            if (_users.ContainsKey(id)) return _users[id];

            Texture2D badge = null;
            var isNpc = false;
            var badgeData = data["badge"];
            if (badgeData != null)
            {
                badge = ScreepsAPI.Badges.Generate(badgeData);
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
                cpu = (int) cpuData.n;
            }

            var user = new ScreepsUser(id, username, cpu, badge, isNpc);
            _users[id] = user;
            return user;
        }

        internal ScreepsUser GetUserByName(string username)
        {
            return _users.SingleOrDefault(u => u.Value.Username == username).Value;
        }
    }
}