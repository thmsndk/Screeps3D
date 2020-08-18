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
using System.Linq;

namespace Assets.Scripts.Screeps3D.Menus.RecentBattlesListPopup
{
    [Serializable]
    public class OnBattleSelected : UnityEvent<Warpath.WarpathRoom> { }

    //Inherit from TableViewCell instead of MonoBehavior to use the GameObject
    //containing this component as a cell in a TableView
    public class RecentBattlesListPopupListItemCell : TableViewCell
    {
        public TextMeshProUGUI Shard;

        public Image Progress;
        public TMP_Text ClassificationLabel;

        public Text LastPvpTime;
        public Text TicksAgo;

        public Image RoomTexture;
        public TextMeshProUGUI Room;

        public BadgeAndLabel DefendersPrefab;
        public GameObject DefendersContainer;

        public BadgeAndLabel AttackersPrefab;
        public GameObject AttackersContainer;

        public OnBattleSelected onSelected;

        private Warpath.WarpathRoom battle;
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

        internal void SetCellItem(Warpath.WarpathRoom battle)
        {
            /*
             room images:
              var urlString = "https://d3os7yery2usni.cloudfront.net/map/\(shard)/\(name).png"
                if shard == "privSrv" {
                    urlString = "\(serverUrl)/assets/map/\(name).png"
                }
             */

            this.battle = battle;

            Shard.text = this.battle.Shard.StartsWith("shard") ? this.battle.Shard.Replace("shard", "shard ") : this.battle.Shard;

            RoomTexture.material.mainTexture = this.battle.RoomTexture;

            Room.text = RoomLink.FormatTMPLink(this.battle.Shard, this.battle.RoomName, this.battle.RoomName);
            //LaunchTime.text = $"Tick {this.battle.InitialLaunchTick.ToString()}";

            // TODO: we need to queue a map-stats lookup if we can't find it. but what about rate limits?
            //var launchRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(this.battle.Shard, this.battle.LaunchRoom?.RoomName);

            //LaunchRoomOwner.SetOwner(this.battle.Attackers.FirstOrDefault()); // TODO: support for rendering a list of battles

            //ImpactRoomTexture.material.mainTexture = this.battle.ImpactRoomTexture;
            //ImpactRoom.text = RoomLink.FormatTMPLink(this.battle.Shard, this.battle.RoomName, this.battle.RoomName);

            //ImpactTime.text = $"Tick {this.battle.LandingTime.ToString()}";

            //var impactRoomInfo = MapStatsUpdater.Instance.GetRoomInfo(this.battle.Shard, this.battle.ImpactRoom?.RoomName);

            foreach (Transform child in DefendersContainer.transform)
            {
                Destroy(child.gameObject);
            }

            var defender = Instantiate(DefendersPrefab, DefendersContainer.transform);
            defender.SetOwner(this.battle.Defender);

            var scale = 1.2f;
            this.DefendersContainer.transform.localScale = new Vector3(scale, scale, 1f);

            foreach (Transform child in AttackersContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var user in this.battle.Attackers)
            {
                var attacker = Instantiate(AttackersPrefab, AttackersContainer.transform);
                attacker.SetOwner(user);
            }

            var attackerCount = this.battle.Attackers.Count;

            if (attackerCount >= 5)
            {
                // TODO: 5 attackers needs another column.
                scale = 0.4f;
            }
            else if (attackerCount == 4)
            {
                scale = 0.6f;
            }
            else if (attackerCount == 3)
            {
                scale = 0.8f;
            }

            this.AttackersContainer.transform.localScale = new Vector3(scale, scale, 1f);

            LastPvpTime.text = this.battle.LastPvpTime.ToString();
            UpdateTicksAgo();

            ClassificationLabel.text = $"Class {(int)this.battle.Classification}";
            Progress.fillAmount = (float)this.battle.Classification / 6f;
        }

        private void UpdateTicksAgo()
        {
            if (this.battle.ShardInfo != null)
            {
                TicksAgo.gameObject.SetActive(true);

                var shardTime = this.battle.ShardInfo.Time;

                if (shardTime > this.battle.LastPvpTime)
                {
                    var ticksAgo = (shardTime - this.battle.LastPvpTime).ToString();
                    TicksAgo.text = $"({ticksAgo} Ticks ago)";
                }
            }
            else
            {
                TicksAgo.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (battle != null)
            {
                UpdateTicksAgo();

                // calculate estimated time based on average tickrate.
                //var timeSinceLastUpdate = DateTime.Now - battle.ShardInfo.TimeUpdated;
                //var ticksSinceLastUpdate = (long)Math.Floor(timeSinceLastUpdate.TotalMilliseconds / (double)battle.ShardInfo.AverageTick);

                //var time = battle.ShardInfo.Time + ticksSinceLastUpdate;


                //var initialLaunchTick = Math.Max(battle.LandingTime - Constants.NUKE_TRAVEL_TICKS, 0);
                //var progress = (float)(time - initialLaunchTick) / Constants.NUKE_TRAVEL_TICKS;
                //Progress.fillAmount = progress;// / 100f;

                //var ticksLeft = battle.LandingTime - time; // TODO: this does not work for other shards
                //TicksLeft.text = $"Ticks remaining {ticksLeft.ToString()}";

                //var impactTimeSpan = battle.EtaEarly - DateTime.Now;
                //ImpactRealTime.text = $"{Environment.NewLine}{impactTimeSpan.Days:D2}d {impactTimeSpan.Hours:D2}h {impactTimeSpan.Minutes:D2}m {impactTimeSpan.Seconds:D2}s";
            }
        }

        public void GoToRoom()
        {
            _chooser.OnSelectedShardChanged(battle.Shard);
            _chooser.GetAndChooseRoom(battle.RoomName);
        }
    }
}
