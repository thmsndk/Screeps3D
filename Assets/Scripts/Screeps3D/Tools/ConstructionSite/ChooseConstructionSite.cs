using Common;
using Screeps3D;
using Screeps3D.Player;
using Screeps3D.RoomObjects;
using Screeps3D.Rooms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using RoomObjectConstructionSite = Screeps3D.RoomObjects.ConstructionSite;

namespace Assets.Scripts.Screeps3D.Tools.ConstructionSite
{

    /// <summary>
    /// Responsible for showing available construction sites for placement
    /// </summary>
    //[ExecuteInEditMode]
    public class ChooseConstructionSite : BaseSingleton<ChooseConstructionSite>
    {
        [SerializeField] private ConstructionSiteItem prefab = default;
        [SerializeField] private GameObject popup = default;
        [SerializeField] private ToggleGroup constructionSites = default;

        public Action<string> OnConstructionSiteChange;

        private Room playerPositionRoom;

        private const int AVAILABLE = 2500; // 2500 seems to be an indicator of not showing available amount.

        private readonly Dictionary<string, ConstructionSiteSpecification> CONTROLLER_STRUCTURES = new Dictionary<string, ConstructionSiteSpecification>
            {

                { "spawn",              new ConstructionSiteSpecification("Spawn","Spawns creeps using energy contained in the room spawns and extensions",new List<int>{0, 1, 1, 1, 1, 1, 1, 2, 3 } )},
                { "extension",          new ConstructionSiteSpecification("Extension","Contains additional energy wich can be used by spawns for spawning bigger creeps",new List<int>{0, 0, 5, 10, 20, 30, 40, 50, 60 } )},
                { "link",               new ConstructionSiteSpecification("Link","Remotely transfers energy to another Link in the same room",new List<int>{0, 0, 0, 0, 0, 2, 3, 4, 6 } )},
                { "road",               new ConstructionSiteSpecification("Road","Decreases movement cost. Decays over time and requires repair",new List<int>{AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE } )},
                { "constructedWall",    new ConstructionSiteSpecification("Wall","Blocks movement of all creeps. Requires repair after construction",new List<int>{0, 0, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE } )},
                { "rampart",            new ConstructionSiteSpecification("Rampart","Defends creeps and structures on the same tile and blocks enemy movement. Decays over time and requires repair",new List<int>{0, 0, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE, AVAILABLE } )},
                { "storage",            new ConstructionSiteSpecification("Storage",$"Stores up to {1000000} resource units",new List<int>{0, 0, 0, 0, 1, 1, 1, 1, 1 } )},
                { "tower",              new ConstructionSiteSpecification("Tower","Remotely attacks or heals any creep in the room, or repairs a structure.",new List<int>{0, 0, 0, 1, 1, 2, 2, 3, 6 } )},
                { "observer",           new ConstructionSiteSpecification("Observer","Provides visibility into a distant froom from your script",new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 1 } )},
                { "powerSpawn",         new ConstructionSiteSpecification("Power Spawn","Spawn power creeps with special unique powers",new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 1 } )},
                { "extractor",          new ConstructionSiteSpecification("Extractor","Allows to mine a mineral deposit.",new List<int>{0, 0, 0, 0, 0, 0, 1, 1, 1 } )},
                { "terminal",           new ConstructionSiteSpecification("Terminal","Sends any resources to a Terminal in another room",new List<int>{0, 0, 0, 0, 0, 0, 1, 1, 1 } )},
                { "lab",                new ConstructionSiteSpecification("Lab","Produces mineral compounds and boosts creeps.",new List<int>{0, 0, 0, 0, 0, 0, 3, 6, 10 } )},
                { "container",          new ConstructionSiteSpecification("Container",$"Stores up to {2000} resource units. Decays over time and requires repair",new List<int>{5, 5, 5, 5, 5, 5, 5, 5, 5 } )},
                { "nuker",              new ConstructionSiteSpecification("Nuker","Launches a nuke to a distant room dealing huge damage to the landing area",new List<int>{0, 0, 0, 0, 0, 0, 0, 0, 1 } )},
                { "factory",            new ConstructionSiteSpecification("Factory","Produces trade commodities",new List<int>{0, 0, 0, 0, 0, 0, 0, 1, 1 } )},
            };

        public override void Awake()
        {
            Debug.Log("choose Constructionsite Awake");
            base.Awake();

            InitializeSpecificationItems();
        }

        private void OnEnable()
        {
            popup?.gameObject?.SetActive(true);

            PlayerPosition.Instance.OnRoomChange += OnRoomChange;
            
            // register for ticks/delta or room updates
            playerPositionRoom = PlayerPosition.Instance.Room;
            playerPositionRoom.ObjectStream.OnData += OnRoomData;

            UpdateAvailable();

            StartCoroutine(SetDeferredContentHeight());

            constructionSites.SetAllTogglesOff();
        }

        private void OnRoomChange()
        {
            playerPositionRoom.ObjectStream.OnData -= OnRoomData;

            playerPositionRoom = PlayerPosition.Instance.Room;

            playerPositionRoom.ObjectStream.OnData += OnRoomData;

            UpdateAvailable();
        }

        private void OnRoomData(JSONObject obj)
        {
            UpdateAvailable();
        }

        private void UpdateAvailable()
        {
            foreach (var item in CONTROLLER_STRUCTURES)
            {
                UpdateAvailable(item.Key);
            }
        }

        private void UpdateAvailable(string type)
        {
            // TODO: Do we want to indicate that some of the constructed objects are not yours?
            var controller = PlayerPosition.Instance.Room.Objects.SingleOrDefault(o => o.Value.Type == Constants.TypeController).Value as Controller;
            var rcl = controller?.Level ?? 0;

            var currentAmount = PlayerPosition.Instance.Room.Objects.Count(o => o.Value.Type == type);
            var currentConstructionSites = PlayerPosition.Instance.Room.Objects.Where(o => o.Value.Type == Constants.TypeConstruction).Count(o => (o.Value as RoomObjectConstructionSite).StructureType == type);

            var specification = CONTROLLER_STRUCTURES[type];
            var maxAmount = specification.CONTROLLER_STRUCTURES[rcl];
            var nextRclUpgrade = specification.CONTROLLER_STRUCTURES.FindIndex(rcl, x => x > maxAmount);

            specification.ConstructionSiteItem.SetAvailable(currentAmount + currentConstructionSites, maxAmount, maxAmount == AVAILABLE, controller, nextRclUpgrade);
        }

        private void OnDisable()
        {
            popup?.gameObject?.SetActive(false);
            playerPositionRoom.ObjectStream.OnData -= OnRoomData;
        }

        private void InitializeSpecificationItems()
        {
            foreach (Transform child in constructionSites.transform)
            {
                var toggle = child.GetComponent<Toggle>();
                toggle?.onValueChanged.RemoveAllListeners();

                Destroy(child.gameObject);
            }

            foreach (var site in CONTROLLER_STRUCTURES)
            {
                var newSite = Instantiate(prefab, constructionSites.transform);
                newSite.name = site.Key;

                newSite.SetName(site.Value.Name);
                newSite.SetDescription(site.Value.Description);
                newSite.SetAvailable(UnityEngine.Random.Range(0, 2500), UnityEngine.Random.Range(0, 2500));

                var toggle = newSite.GetComponent<Toggle>();
                toggle.group = constructionSites;
                toggle.onValueChanged.AddListener(isOn => ToggleInput(toggle, isOn, site.Key));

                site.Value.ConstructionSiteItem = newSite;
            }
        }

        private IEnumerator SetDeferredContentHeight()
        {
            yield return new WaitForSeconds(0.1f); // gotta yield to let content size fitter run
            SetContentHeight();
        }

        private void SetContentHeight()
        {
            var constructionSitesRect = constructionSites.GetComponent<RectTransform>();
            var contentRect = constructionSites.transform.parent.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, constructionSitesRect.sizeDelta.y);

            constructionSitesRect.anchoredPosition = new Vector3(constructionSitesRect.anchoredPosition.x, constructionSitesRect.sizeDelta.y / 2f);
        }

        private void ToggleInput(Toggle toggle, bool isOn, string type)
        {
            if (isOn)
            {
                OnConstructionSiteChange?.Invoke(type);
            }
        }

        private class ConstructionSiteSpecification
        {
            public string Name { get; set; }
            public string Description { get; set; }

            public List<int> CONTROLLER_STRUCTURES { get; set; }
            public ConstructionSiteItem ConstructionSiteItem { get; internal set; }

            public ConstructionSiteSpecification(string name, string description, List<int> rclRequirements)
            {
                this.Name = name;
                this.Description = description;
                this.CONTROLLER_STRUCTURES = rclRequirements;
            }
        }
    }
}
