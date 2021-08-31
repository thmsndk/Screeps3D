using Assets.Scripts.Screeps_API;
using Screeps3D;
using System;
using System.Collections.Generic;

namespace Screeps_API
{
    public class ScreepsServer : IScreepsServer
    {
        public string AuthToken { get; set; }

        public string Key { get; set; }
        public string Name { get; set; }
        public Address Address { get; } = new Address();
        public Credentials Credentials { get; } = new Credentials();

        public IScreepsServerMetaData Meta { get; set; }

        public ScreepsServer(string key)
        {
            this.Key = key;
            this.Meta = new ScreepsServerMetaData();
        }

        public ScreepsServer(string key, SS3UnifiedCredentials.SS3UnifiedCredentialsServer server): this(key)
        {
            this.Address.HostName = server.Host;
            this.Address.Port = server.Port;

            if (server.Secure.HasValue)
            {
                this.Address.Ssl = server.Secure.Value;
            }

            if (string.IsNullOrEmpty(server.Port))
            {
                this.Address.Port = this.Address.Ssl ? "443" : "21025";
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

        public bool? Online { get; set; }

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
    public class ScreepsServerMetaData : IScreepsServerMetaData
    {
        
        public int Users { get; set; }
        public string Version { get; set; }
        public int LikeCount { get; set; }

        public List<string> ShardNames { get; set; } = new List<string>();
        public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();

        public ScreepsUser Me { get; set; }

        public double GlobalControlLevel { get; set; }
        public WorldStatus WorldStatus { get; set; }
    }


    public class Credentials
    {
        public string Token;
        public string Email;
        public string Password;

        public bool HasCredentials
        {
            get
            {
                return !string.IsNullOrEmpty(Token) || !string.IsNullOrEmpty(Email) &&
                       !string.IsNullOrEmpty(Email);
            }
        }
    }

    public class Address
    {
        public bool Ssl;
        public string HostName;
        public string Port;
        public string Path = "/";

        public string Http(string path = "")
        {
            if (path.StartsWith("/") && Path.EndsWith("/"))
            {
                path = path.Substring(1);
            }

            var protocol = Ssl ? "https" : "http";
            var port = HostName.ToLowerInvariant() == "screeps.com" ? "" : string.Format(":{0}", this.Port);
            var url = string.Format("{0}://{1}{2}{3}{4}", protocol, HostName, port, this.Path, path);
            //Debug.Log(url);
            return url;
        }
    }

}