using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class ObjectViewFactory : BaseSingleton<ObjectViewFactory>
    {
        [SerializeField] private Transform _objectParent = default;
        private Dictionary<string, Stack<ObjectView>> _pools = new Dictionary<string, Stack<ObjectView>>();
        private string _path = "Prefabs/RoomObjects/";

        public ObjectView NewView(RoomObject roomObject)
        {
            if (roomObject.Type == null)
            {
                Debug.LogError("found null type, is there a caching problem?");
                return null;
            }
            var view = GetFromPool(roomObject.Type);
            if (!view)
            {
                view = NewInstance(roomObject.Type);
            }
            return view;
        }

        private ObjectView NewInstance(string type)
        {
            var go = PrefabLoader.Load(string.Format("{0}{1}", _path, type));
            if (go == null)
                return null;
            var view = go.GetComponent<ObjectView>();
            view.transform.SetParent(_objectParent);
            view.Init();
            return view;
        }

        private ObjectView GetFromPool(string type)
        {
            var pool = GetPool(type);
            if (pool.Count > 0)
            {
                return pool.Pop();
            } else
            {
                return null;
            }
        }

        private Stack<ObjectView> GetPool(string type)
        {
            if (!_pools.ContainsKey(type))
            {
                _pools[type] = new Stack<ObjectView>();
            }
            return _pools[type];
        }

        public void AddToPool(ObjectView objectView)
        {
            var pool = GetPool(objectView.RoomObject.Type);
            pool.Push(objectView);
        }
    }
}