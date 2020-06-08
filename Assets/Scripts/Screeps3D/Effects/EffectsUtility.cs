using Common;
using Screeps3D.RoomObjects;
using UnityEngine;

namespace Screeps3D.Effects
{
    public static class EffectsUtility
    {
        public static void Beam(RoomObject origin, JSONObject target, BeamConfig config)
        {
            var startPos = origin.View.transform.position + new Vector3(0, config.StartHeight, 0);
            var endPos = PosUtility.Convert(target, origin.Room) + new Vector3(0, config.EndHeight, 0);
            Beam(startPos, endPos, config.Color);
        }
        
        public static void Attack(RoomObject origin, Vector3 targetPos) {
            var go = PoolLoader.Load(AttackEffect.PATH);
            var effect = go.GetComponent<AttackEffect>();
            effect.Load(origin, targetPos);
        }

        public static void Beam(Vector3 startPos, Vector3 endPos, Color color)
        {
            var go = PoolLoader.Load(BeamEffect.PATH);
            var effect = go.GetComponent<BeamEffect>();
            effect.Load(startPos, endPos, color);
        }

        public static void Speech(RoomObject creep, string message, bool isPublic = true)
        {
            var go = PoolLoader.Load(SpeechEffect.PATH);
            var effect = go.GetComponent<SpeechEffect>();
            effect.Load(creep, message, isPublic);
        }

        /// <summary>
        /// Render a creep teleporting away
        /// </summary>
        /// <param name="origin"></param>
        public static void Teleport(RoomObject origin)
        {
            var go = PoolLoader.Load(TeleportEffect.PATH);
            var effect = go.GetComponent<TeleportEffect>();
            effect.Load(origin); // TODO: need to unload if creep exists / portals
        }

        /// <summary>
        /// Render a creep spawning from a teleport
        /// </summary>
        /// <param name="position"></param>
        public static void TeleportSpawn(Vector3 position)
        {
            var go = PoolLoader.Load(TeleportEffect.PATH);
            var effect = go.GetComponent<TeleportEffect>();
            effect.Load(position);
        }

        /// <summary>
        /// Render an electric explosion around a creep
        /// </summary>
        /// <param name="origin"></param>
        public static void ElectricExplosion(RoomObject origin)
        {
            var go = PoolLoader.Load(ElectricExplosionEffect.PATH);
            var effect = go.GetComponent<ElectricExplosionEffect>();
            effect.Load(origin); // TODO: need to unload if creep exists / portals
        }

        /// <summary>
        /// Render a nuke explosion at target
        /// </summary>
        /// <param name="position"></param>
        public static void NukeExplosion(Vector3 position)
        {
            var go = PoolLoader.Load(NukeExplosionEffect.PATH);
            var effect = go.GetComponent<NukeExplosionEffect>();
            effect.Load(position);
        }
    }
    
    public class BeamConfig
    {
        public Color Color { get; private set; }
        public float StartHeight { get; private set; }
        public float EndHeight { get; private set; }

        public BeamConfig(Color color, float startHeight, float endHeight)
        {
            Color = color;
            StartHeight = startHeight;
            EndHeight = endHeight;
        }
    }
}