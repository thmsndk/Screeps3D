using Assets.Scripts.Screeps_API;
using System;
using System.Collections.Generic;

namespace Screeps_API
{
    public class ScreepsServer
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public Address Address = new Address();
        public Credentials Credentials = new Credentials();

        public ScreepsServer(string key)
        {
            this.Key = key;
        }

        public ScreepsServer(string key, SS3UnifiedCredentials.SS3UnifiedCredentialsServer server)
        {
            this.Address.HostName = server.Host;
            this.Address.Port = server.Port;

            if (server.Secure.HasValue)
            {
                this.Address.Ssl = server.Secure.Value;
            }

            this.Key = key;
            this.Name = server.Name ?? key;

            // TODO: What if they have supplied a path, but it is not equal to the bools?
            if (server.Ptr.HasValue && server.Ptr.Value)
            {
                this.Address.Path = "/ptr";
            }

            if (server.Season.HasValue && server.Season.Value)
            {
                this.Address.Path = "/season";
            }

            // Assist with merging
            if (server.Host.ToLowerInvariant().EndsWith("screeps.com"))
            {
                this.Official = true;

                this.Name = $"Screeps.com";
                if (this.Address.Path == "/ptr")
                {
                    this.Name = $"PTR " + this.Name;
                }

                if (this.Address.Path == "/season")
                {
                    this.Name = $"SEASONAL " + this.Name;
                }
            }

            this.Credentials.Token = server.Token;
            this.Credentials.Email = server.Username;
            this.Credentials.Password = server.Password;
        }

        /// <summary>
        /// A bool indicating if it is an official server or not.
        /// </summary>
        public bool Official { get; internal set; }

        public bool HasCredentials
        {
            get
            {
                return Credentials.HasCredentials;
            }
        }
    }

    /// <summary>
    /// Primarly consists of data from api/version, but additional custom data about a server is stored here.
    /// </summary>
    public class ScreepsServerMetaData
    {
        public bool? Online { get; internal set; }
        public int Users { get; internal set; }
        public string Version { get; internal set; }
        public int LikeCount { get; set; }

        public List<string> ShardNames { get; internal set; }
    }

}