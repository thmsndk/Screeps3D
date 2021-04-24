using System.Collections.Generic;

namespace Screeps_API
{
    public interface IScreepsServer
    {
        string Key { get; set; }

        string Name { get; set; }
        
        bool Official { get; }

        public Address Address { get; }

        public Credentials Credentials { get; }

        bool? Online { get; set; }

        public IScreepsServerMetaData Meta { get; }
        
        public bool HasCredentials { get; }
    }

    public interface IScreepsServerMetaData
    {
        int LikeCount { get; set; }
        List<string> ShardNames { get; set; }
        int Users { get; set; }
        string Version { get; set; }
        // raw point: "useNativeAuth": false, ??

        // TODO: features (mods) screepsmod-mongo, screepsmod-auth, screepsmod-admin-utils market
    }
}