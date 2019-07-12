using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public static class PoolLoader
    {

        private static Dictionary<string, Stack<GameObject>> _pools;
        private static Transform _parent;

        public static void Init()
        {
            _pools = new Dictionary<string, Stack<GameObject>>();
            _parent = new GameObject("pooledObjects").transform;
        }

        private static Stack<GameObject> GetStack(string path)
        {
            if (!_pools.ContainsKey(path))
            {
                _pools[path] = new Stack<GameObject>();
            }
            return _pools[path];
        }

        private static GameObject Instantiate(string path)
        {
            var prefab = PrefabLoader.Look(path);
            if (!prefab)
                throw new Exception(string.Format("no resource found at path: {0}", path));
            return UnityEngine.Object.Instantiate(prefab, _parent);
        }

        public static void Preload(string path, int count)
        {
            var stack = GetStack(path);

            for (var i = 0; i < count; i++)
            {
                stack.Push(Instantiate(path));
            }
        }

        public static GameObject Load(string path)
        {
            var stack = GetStack(path);

            if (stack.Count == 0)
            {
                stack.Push(Instantiate(path));
            }

            return stack.Pop();
        }

        public static void Return(string path, GameObject go)
        {
            if (!_pools.ContainsKey(path))
            {
                throw new Exception(path + " returned pooled object to unused pool");
            }

            _pools[path].Push(go);
        }
    }
}