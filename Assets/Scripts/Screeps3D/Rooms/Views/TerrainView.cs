using UnityEngine;
using System.Collections.Generic;
using System;
using Common;

namespace Screeps3D.Rooms.Views
{
    public class TerrainView : MonoBehaviour, IRoomViewComponent
    {
        [SerializeField] private MeshFilter _swampMesh;
        [SerializeField] private MeshFilter _wallMesh;

        private string _terrain;
        private List<Vector2> _lairPositions;
        
        private int _x;
        private int _y;
        private bool[,] _wallPositions;
        private bool[,] _swampPositions;
        private bool needsUpdate;
        private bool isDuringUpdate;
        private bool isDuringDeform;

        public void Init(Room room)
        {
            TerrainFinder.Instance.Find(room, InitRender);
            _lairPositions = new List<Vector2>();
            enabled = true;
            needsUpdate = false;
            isDuringUpdate = false;
            isDuringDeform = false;
        }

        public void addLair(int x, int y)
        {
            if (_lairPositions.Exists(l => l.x == x && l.y == y))
                return;
            _lairPositions.Add(new Vector2((float)x, (float)y));
            needsUpdate = true;
        }

        private void InitRender(string terrain)
        {
            this._terrain = terrain;
            needsUpdate = true;
        }

        private void Update()
        {
            if (_terrain == null)
                return;
            if (isDuringDeform)
                return;
            if (!needsUpdate && !isDuringUpdate)
                return;
            if (!isDuringUpdate)
            {
                needsUpdate = false;
                isDuringUpdate = true;
                _wallPositions = new bool[50, 50];
                _swampPositions = new bool[50, 50];
                _x = 0;
                _y = 0;
            }
            
            var time = Time.time;
            for (; _x < 50; _x++)
            {
                for (; _y < 50; _y++)
                {
                    _swampPositions[_x, _y] = false;
                    _wallPositions[_x, _y] = false;

                    var unit = _terrain[_x + _y * 50];
                    if (unit == '0' || unit == '1')
                    {
                    }
                    if (unit == '2' || unit == '3')
                    {
                        _swampPositions[_x, _y] = true;
                    }
                    if (unit == '1' || unit == '3')
                    {
                        _wallPositions[_x, _y] = true;
                    }
                    if (Time.time - time > .001f)
                    {
                        return;
                    }
                }
                _y = 0;
            }
            
            isDuringUpdate = false;
            isDuringDeform = true;
            Scheduler.Instance.Add(Deform);
        }

        private void Deform()
        {
            const float wallConstant = 0.5f;
            const float wallRandom = 0.5f;
            const float swampConstant = 0.3f;
            const float swampRandom = 0.0f;
            const float lairDeformRange = 1.0f;
            const float lairConstant = 0.35f;
            const float lairRandom = 0.05f;
            
            // walls
            var vertices = _wallMesh.mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                var point = vertices[i];
                if (point.x < 0 || point.x > 50 || point.z < 0 || point.z > 50)
                    continue;
                
                var xf =  point.x;
                var x = (int) point.x;
                if (x >= _wallPositions.GetLength(0))
                    continue;
                
                var yf = 50.0 - point.z;
                var y = 49 - (int) point.z;
                if (y >= _wallPositions.GetLength(1))
                    continue;
                
                if (!_wallPositions[x,y])
                    continue;

                bool isLair = false;
                foreach (var lairPos in _lairPositions)
                    if (Math.Abs(lairPos.x + 0.5 - xf) < lairDeformRange && Math.Abs(lairPos.y + 0.5 - yf) < lairDeformRange)
                    {
                        isLair = true;
                        break;
                    }

                if (isLair)
                    vertices[i] = new Vector3(point.x, lairConstant + UnityEngine.Random.value * lairRandom, point.z);
                else
                    vertices[i] = new Vector3(point.x, wallConstant + UnityEngine.Random.value * wallRandom, point.z);
            }
            _wallMesh.mesh.vertices = vertices;
            _wallMesh.mesh.RecalculateNormals();
            
            // swamps
            vertices = _swampMesh.mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                var point = vertices[i];
                if (point.x < 0 || point.x > 50 || point.z < 0 || point.z > 50)
                    continue;

                var x = (int) point.x;
                if (x >= _swampPositions.GetLength(0))
                    continue;

                var y = 49 - (int) point.z;
                if (y >= _swampPositions.GetLength(1))
                    continue;
                
                if (!_swampPositions[x,y])
                    continue;

                vertices[i] = new Vector3(point.x, swampConstant + UnityEngine.Random.value * swampRandom, point.z);
            }
            _swampMesh.mesh.vertices = vertices;
            _swampMesh.mesh.RecalculateNormals();
            
            _wallPositions = null;
            _swampPositions = null;
            isDuringDeform = false;
        }
    }
}