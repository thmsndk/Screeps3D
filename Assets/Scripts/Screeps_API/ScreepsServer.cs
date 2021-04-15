using System;
using System.Collections.Generic;

namespace Screeps_API
{
    public class ScreepsServer
    {
        public string Name { get; set; }

        public Address Address = new Address();
        public Credentials Credentials = new Credentials();

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