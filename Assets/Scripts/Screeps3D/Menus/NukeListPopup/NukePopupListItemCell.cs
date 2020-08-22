using UnityEngine;
using System.Collections;
using Tacticsoft;
using UnityEngine.UI;
using Screeps_API;
using System;
using UnityEngine.Events;
using Screeps3D.World.Views;
using Screeps3D;
using TMPro;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Screeps3D.Rooms;

namespace Assets.Scripts.Screeps3D.Menus.NukeListPopup
{
    [Serializable]
    public class OnNukeSelected : UnityEvent<NukeMonitor.NukeData> { }

    //Inherit from TableViewCell instead of MonoBehavior to use the GameObject
    //containing this component as a cell in a TableView
    public class NukePopupListItemCell : TableViewCell
    {
        public TextMeshProUGUI Shard;

        public Image Progress;

        public Text ImpactRealTime;

        public Image LaunchRoomTexture;
        public TextMeshProUGUI LaunchRoom;
        public Text LaunchTime;

        public BadgeAndLabel LaunchRoomOwner;

        public Image ImpactRoomTexture;
        public TextMeshProUGUI ImpactRoom;
        public Text ImpactTime;

        public BadgeAndLabel ImpactRoomOwner;

        public Text TicksLeft;
        public Text ETAEarly;
        public Text ETALate;


        public OnNukeSelected onSelected;

        private NukeMonitor.NukeData nuke;
        private Image buttonImage;

        private RoomChooser _chooser;

        private void Awake()
        {
            var roomChooserGameObject = GameObject.FindGameObjectWithTag("RoomChooser");
            _chooser = roomChooserGameObject.GetComponent<RoomChooser>();
        }

        void Start()
        {
            buttonImage = GetComponent<Image>();
        }

        //public void Selected()
        //{
        //    if (onSelected != null)
        //    {
        //        onSelected.Invoke(nuke);
        //    }
        //}

        internal void SetCellItem(NukeMonitor.NukeData nuke)
        {
            /*
             room images:
              var urlString = "https://d3os7yery2usni.cloudfront.net/map/\(shard)/\(name).png"
                if shard == "privSrv" {
                    urlString = "\(serverUrl)/assets/map/\(name).png"
                }
             */

            this.nuke = nuke;

            Shard.text = nuke.Shard.StartsWith("shard") ? nuke.Shard.Replace("shard", "shard ") : nuke.Shard;

            LaunchRoomTexture.material.mainTexture = nuke.LaunchRoomTexture;

            LaunchRoom.text = RoomLink.FormatTMPLink(nuke.Shard, nuke.LaunchRoom?.RoomName, nuke.LaunchRoom?.RoomName);
            LaunchTime.text = $"Tick {nuke.InitialLaunchTick.ToString()}"; 

            // TODO: we need to queue a map-stats lookup if we can't find it. but what about rate limits?
            var launchRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(nuke.Shard, nuke.LaunchRoom?.RoomName);

            LaunchRoomOwner.SetOwner(launchRoomInfo?.User);

            ImpactRoomTexture.material.mainTexture = nuke.ImpactRoomTexture;
            ImpactRoom.text = RoomLink.FormatTMPLink(nuke.Shard, nuke.ImpactRoom?.RoomName, nuke.ImpactRoom?.RoomName);

            ImpactTime.text = $"Tick {nuke.LandingTime.ToString()}";

            var impactRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(nuke.Shard, nuke.ImpactRoom?.RoomName);

            ImpactRoomOwner.SetOwner(impactRoomInfo?.User);

            ETAEarly.text = nuke.EtaEarly.ToString();
            ETALate.text = nuke.EtaLate.ToString();
        }

        private void Update()
        {
            if (nuke != null)
            {
                // calculate estimated time based on average tickrate.
                var timeSinceLastUpdate = DateTime.Now - nuke.ShardInfo.TimeUpdated;
                var ticksSinceLastUpdate = (long)Math.Floor(timeSinceLastUpdate.TotalMilliseconds / (double)nuke.ShardInfo.AverageTick);

                var time = nuke.ShardInfo.Time + ticksSinceLastUpdate;

                var initialLaunchTick = Math.Max(nuke.LandingTime - Constants.NUKE_TRAVEL_TICKS, 0);
                var progress = (float)(time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;
                Progress.fillAmount = progress;// / 100f;

                var ticksLeft = (nuke.LandingTime - time);
                TicksLeft.text = $"Ticks remaining {ticksLeft.ToString()}";

                var impactTimeSpan = nuke.EtaEarly - DateTime.Now;
                ImpactRealTime.text = $"{Environment.NewLine}{impactTimeSpan.Days:D2}d {impactTimeSpan.Hours:D2}h {impactTimeSpan.Minutes:D2}m {impactTimeSpan.Seconds:D2}s"; 
            }
        }

        public void GoToLaunchRoom()
        {
            _chooser.OnSelectedShardChanged(nuke.Shard);
            _chooser.GetAndChooseRoom(nuke.LaunchRoomName);
        }

        public void GoToImpactRoom()
        {
            _chooser.OnSelectedShardChanged(nuke.Shard);
            _chooser.GetAndChooseRoom(nuke.ImpactRoomName);
        }
    }
}
