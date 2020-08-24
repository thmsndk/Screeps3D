using UnityEngine;

namespace Screeps_API
{
    public class ScreepsUser
    {
        public readonly string UserId;
        public readonly string Username;
        public readonly int Cpu;
        public readonly Texture2D Badge;
        public readonly bool IsNpc;

        public ScreepsUser(string userId, string username, int cpu, Texture2D badge, bool isNpc)
        {
            this.UserId = userId;
            this.Username = username;
            this.Cpu = cpu;
            this.Badge = badge;
            this.IsNpc = isNpc;
        }

        public Color BadgeColor1 { get; internal set; }
        public Color BadgeColor2 { get; internal set; }
        public Color BadgeColor3 { get; internal set; }
    }
}