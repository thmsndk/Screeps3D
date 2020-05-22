using Screeps_API;
using Screeps3D.Effects;
using TMPro;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class NukeMissileView : MonoBehaviour, IWorldOverlayViewComponent
    {
        public NukeMissileOverlay Overlay { get; private set; }
        [SerializeField] private Renderer _nuke;        

        private NukeMissileArchRenderer arcRenderer;

        private bool initialized = false;

        private bool nukeExploded = false;

        private bool badgeSet = false;

        public void Init(WorldOverlay overlay)
        {
            Overlay = overlay as NukeMissileOverlay;

            this.arcRenderer = this.gameObject.GetComponentInChildren<NukeMissileArchRenderer>();

            // do we have a launchroom? what if we first acquire the launchroom later?, should this be in update?
            if (Overlay.LaunchRoom != null)
            {
                arcRenderer.point1.transform.position = Overlay.LaunchRoom.Position + new Vector3(25, 0, 25); // Center of the room, because we do not know where the nuke is, could perhaps scan for it and correct it?
                
            }
            var launchRoomText = arcRenderer.point1.GetComponentInChildren<TMP_Text>();
            launchRoomText.text = "";//launcRoom.Name;

            if (Overlay.ImpactRoom != null)
            {
                arcRenderer.point2.transform.position = Overlay.ImpactPosition;
            }

            var point2Text = arcRenderer.point2.GetComponentInChildren<TMP_Text>();
            point2Text.text = ""; //$"{progress*100}%";
            arcRenderer.Progress(Overlay.Progress - 0.3f); // give a little smoke trail when initialized
            //arcRenderer.Progress(Overlay.Progress); // TODO: render progress on selection panel when you select the missile.

            initialized = true;
            // spawn nuke explosion for testing purposes, not sure it belongs on the missile view? :shrugh: belongs in an "onTick" event or something
            //EffectsUtility.NukeExplosition(Overlay.ImpactPosition);

            
            
        }

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

            if (!badgeSet)
            {
                var launchRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(Overlay.Shard, Overlay.LaunchRoomName);
                if (launchRoomInfo != null)
                {
                    _nuke.materials[3].SetColor("EmissionColor", new Color(0.7f, 0.7f, 0.7f, 1f));
                    _nuke.materials[3].SetTexture("EmissionTexture", launchRoomInfo.User?.Badge);
                    _nuke.materials[3].SetFloat("EmissionStrength", 4);
                    badgeSet = true;
                }
            }
            float decayEmission = 2f + Mathf.Abs(Mathf.Sin(Time.time)) * 3;
            _nuke.materials[3].SetFloat("EmissionStrength", decayEmission);

            // TODO: should we simulate movement / progress in between nukemonitor updates so the misile moves "smoothly"? this neeeds to be in update then. and not sure calling arcRenderer.Progress works, we then need a "targetProgress" or something like that, could let us inspire by creep movement between ticks
            // TODO: should perhaps move this calculation so progress is updated on each tick? and not each rendering?
            float progress = (float)(ScreepsAPI.Time - Overlay.InitialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;
            // TODO: the nuke position progress should be at the tip of the nuke
            arcRenderer.Progress(progress);

            // quadratic curves tend to be far more exciting
            // make it fast at launch, spending most time in the middle, and gain more and more speed towards impact so it "lands" with a bang?

            gameObject.name = $"nukeMissile:{this.Overlay.Id}:{Overlay?.LaunchRoom?.Name}->{Overlay?.ImpactRoom?.Name} {progress * 100}%";

            // explosion progress check should be landTime and current tick
            if (progress >= 1f && !nukeExploded)
            {
                // TODO: nuke effect should be slower, and last longer, when you wait 50k ticks for an explosion, it should be GRAND!!!
                EffectsUtility.NukeExplosion(Overlay.ImpactPosition);
                nukeExploded = true;
                arcRenderer.enabled = false;
            }

        }
    }
}