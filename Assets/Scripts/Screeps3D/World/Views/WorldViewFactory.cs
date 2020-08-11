using Assets.Scripts.Screeps3D.World.Overlay;
using Assets.Scripts.Screeps3D.World.Views;
using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class WorldViewFactory : BaseSingleton<WorldViewFactory>
    {
        private static Transform _parent;

        private const string Path = "Prefabs/WorldView/";

        private Dictionary<string, Stack<WorldView>> _pools = new Dictionary<string, Stack<WorldView>>();

        public static WorldView GetInstance(WorldViewData data)
        {
            if (_parent == null)
            {
                _parent = new GameObject("WorldViews").transform;
            }

            WorldView view = Instance.GetFromPool(data.Type);

            if (view == null)
            {
                var go = PrefabLoader.Load(string.Format("{0}{1}", Path, data.Type), _parent);
                view = go.GetComponent<WorldView>();
            }

            //view.gameObject.name = overlay.Name;
            //view.transform.localPosition = overlay.Position;
            view.Init(data);
            return view;
        }

        private WorldView GetFromPool(string type)
        {
            var pool = GetPool(type);
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            else
            {
                return null;
            }
        }
        private Stack<WorldView> GetPool(string type)
        {
            if (!_pools.ContainsKey(type))
            {
                _pools[type] = new Stack<WorldView>();
            }
            return _pools[type];
        }

        public void AddToPool(WorldView view)
        {
            var pool = GetPool(view.Data.Type);
            pool.Push(view);
        }

    }
}