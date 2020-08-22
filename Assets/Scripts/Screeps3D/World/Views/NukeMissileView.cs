using Common;
using Screeps_API;
using Screeps3D.Rooms;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class NukeMissileView : MonoBehaviour, IWorldOverlayViewComponent
    {
        public NukeMissileOverlay Overlay { get; private set; }
        [SerializeField] private GameObject _nuke;
        [SerializeField] private ParticleSystem _nukeTrail;
        [SerializeField] private ParticleSystem _bigBadaBoom;
        [SerializeField] private ParticleSystem _launchSmoke;
        [SerializeField] private NukeMissileArchRenderer nukeArcRenderer;

        private bool initialized = false;

        private bool nukeExploded = false;
        private bool nukeLaunched = true;

        private bool badgeSet = false;
        private bool launchLocationSet = false;


        private void setBadge()
        {
            if (!badgeSet)
            {
                var launchRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(Overlay.Shard, Overlay.LaunchRoomName);
                if (launchRoomInfo != null)
                {
                    Renderer r = _nuke.GetComponentInChildren<Renderer>();
                    r.materials[3].SetTexture("EmissionTexture", launchRoomInfo.User?.Badge);
                    r.materials[3].SetFloat("EmissionStrength", .1f);
                    badgeSet = true;
                }
            }
        }

        private void setLaunchLocation(Room room, JSONObject roomData)
        {
            if (!launchLocationSet)
            {
                var nuker = room.Objects.SingleOrDefault(ro => ro.Value.Type == Constants.TypeNuker);

                // if nuker present, shift arc renderer start point to it
                if (nuker.Value != null)
                {
                    nukeArcRenderer.point1.transform.position = nuker.Value.Position;
                    _launchSmoke.transform.position += nukeArcRenderer.point1.transform.position + new Vector3(0, 0.6f, 0);
                    nukeArcRenderer.RenderArc();
                }
                // regardless - position was set (default or nuker) - unsubscribe from unpack
                launchLocationSet = true;
                Overlay.LaunchRoom.RoomUnpacker.OnUnpack -= setLaunchLocation;
            }
        }

        private void playLaunchEffect(float progress)
        {
            if (progress < 0.015 && !_launchSmoke.isPlaying)
            {
                _launchSmoke.Play();
            }
            if (progress >= 0.015 && _launchSmoke.isPlaying)
            {
                _launchSmoke.Stop();
            }
        }

        private void landNukeEffect(float progress)
        {
            if (progress >= 1f && !nukeExploded)
            {
                _nukeTrail.Stop();
                _bigBadaBoom.Play();
                nukeExploded = true;
                nukeArcRenderer.Show(false);
            }
        }

        public void Init(object overlay)
        {
            waitingOnLaunchLocation = false;
            lastProgress = 0f;

            //_bigBadaBoom.Stop();
            _launchSmoke.Stop();
            Overlay = overlay as NukeMissileOverlay;
            simulatedProgress = 0f;

            // do we have a launchroom? what if we first acquire the launchroom later?, should this be in update?
            if (Overlay.LaunchRoom != null)
            {
                nukeArcRenderer.point1.transform.position = Overlay.LaunchRoom.Position + new Vector3(25, 0, 25); // Center of the room, because we do not know where the nuke is, could perhaps scan for it and correct it?
                //_launchSmoke.transform.position = nukeArcRenderer.point1.transform.position;
                _launchSmoke.transform.position += new Vector3(0, 0.6f, 0);
            }
            ////var launchRoomText = nukeArcRenderer.point1.GetComponentInChildren<TMP_Text>();
            ////launchRoomText.text = "";//launcRoom.Name;

            if (Overlay.ImpactRoom != null)
            {
                nukeArcRenderer.point2.transform.position = Overlay.ImpactPosition;
                //_bigBadaBoom.transform.position = Overlay.ImpactPosition;
            }

            ////var point2Text = nukeArcRenderer.point2.GetComponentInChildren<TMP_Text>();
            ////point2Text.text = ""; //$"{progress*100}%";

            nukeArcRenderer.Show(true);

            initialized = true;
        }


        private void MoveMissleAlongArc(float progress)
        {
            if (progress > 1 || progress < 0)
            {
                Debug.LogWarning($"{this.name} progress is out of range 0..1 = {progress}");
                return;
            }

            _nuke.transform.position = nukeArcRenderer.CalculateArcPoint(progress);
            var nextPoint = nukeArcRenderer.CalculateArcPoint(progress + 0.001f);
            _nuke.transform.LookAt(nextPoint);
        }

        private bool waitingOnLaunchLocation = false;
        private float lastProgress;

        private float simulatedProgress;
        private void Update()
        {
            if (Overlay == null)
            {
                return;
            }

            if (!initialized)
            {
                return;
            }

            if (!launchLocationSet && !waitingOnLaunchLocation)
            {
                if (Overlay.LaunchRoom != null && Overlay.LaunchRoom.RoomUnpacker != null)
                {
                    Overlay.LaunchRoom.RoomUnpacker.OnUnpack += setLaunchLocation;
                    waitingOnLaunchLocation = true;
                }
            }
            setBadge();

            // TODO: should we simulate movement / progress in between nukemonitor updates so the misile moves "smoothly"? this neeeds to be in update then. and not sure calling arcRenderer.Progress works, we then need a "targetProgress" or something like that, could let us inspire by creep movement between ticks
            // TODO: should perhaps move this calculation so progress is updated on each tick? and not each rendering?
            var timeSinceLastUpdate = DateTime.Now - Overlay.ShardInfo.TimeUpdated;
            var ticksSinceLastUpdate = (long)Math.Floor(timeSinceLastUpdate.TotalMilliseconds / (double)Overlay.ShardInfo.AverageTick);

            var time = Overlay.ShardInfo.Time + ticksSinceLastUpdate;

            var initialLaunchTick = Math.Max(Overlay.LandingTime - Constants.NUKE_TRAVEL_TICKS, 0);

            float progress = (float)(time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;
            ////// For Debug purposes
            //////if (launchLocationSet)
            //////{
            ////    simulatedProgress += simulatedProgress < 0.015f ? 0.00001f : 0.001f;
            //////}
            ////progress = simulatedProgress;
            if (lastProgress != progress)
            {
                lastProgress = progress;

                playLaunchEffect(progress);

                MoveMissleAlongArc(progress);

                // nuke explosion and stopping of nukeSmoke particle effect, disabling the ArcRenderer also
                landNukeEffect(progress);
            }
            // quadratic curves tend to be far more exciting
            // make it fast at launch, spending most time in the middle, and gain more and more speed towards impact so it "lands" with a bang?
            gameObject.name = $"nukeMissile:{this.Overlay.Id}:{Overlay?.LaunchRoom?.Name}->{Overlay?.ImpactRoom?.Name} {progress * 100}%";
        }
    }
}