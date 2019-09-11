using System;
using System.Collections.Generic;

namespace Screeps_API
{
    [Serializable]
    public class ServerCache
    {
        public string Name { get; set; }

        public Address Address = new Address();
        public Credentials Credentials = new Credentials();
        [Obsolete("We should not cache terrain on disk")] // can't remove it without breaking deserilization, have to figure out how to write some migration
        public Dictionary<string, string> Terrain = new Dictionary<string, string>();
        public bool SaveCredentials;

        public int LikeCount { get; set; }
        public bool MMO { get; internal set; }
        public bool? Online { get; internal set; }
        public int Users { get; internal set; }
        public string Version { get; internal set; }
        public bool HasCredentials
        {
            get
            {
                return !string.IsNullOrEmpty(Credentials.Token) || (!string.IsNullOrEmpty(Credentials.Email) && !string.IsNullOrEmpty(Credentials.Email));
            }
        }

        public bool Selected { get; internal set; }
    }
}