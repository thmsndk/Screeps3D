using UnityEngine;
using System.Collections.Generic;
using System;
using Common;

namespace Screeps3D.Rooms.Views
{
    public class TerrainView : MonoBehaviour, IRoomViewComponent
    {
        private Room _room;
        [SerializeField] private MeshFilter _swampMesh = default;
        [SerializeField] private MeshFilter _wallMesh = default;

        private bool _hasTerrainData;
        private string _terrain;
        private bool _hasMapData;
        private List<Vector2> _lairPositions;
        private List<Vector2> _roadPositions;

        private int _x;
        private int _y;
        private bool[,] _wallPositions;
        private bool[,] _swampPositions;
        private bool isInitialized;
        private bool isInitializing;
        public void Init(Room room)
        {
            _room = room;
            _hasTerrainData = false;
            _hasMapData = false;

            TerrainFinder.Instance.Find(room, OnTerrainData);
            room.MapStream.OnData += OnMapData;
            _lairPositions = new List<Vector2>();
            _roadPositions = new List<Vector2>();
            enabled = true;
            isInitialized = false;
        }
        private void OnMapData(JSONObject data)
        {
            _hasMapData = true;
            if (data["k"] != null)
            {
                foreach (var numArray in data["k"].list)
                {
                    var x = (int)numArray.list[0].n;
                    var y = (int)numArray.list[1].n;
                    _lairPositions.Add(new Vector2((float)x, (float)y));
                }
            }
            if (data["r"] != null)
            {
                foreach (var numArray in data["r"].list)
                {
                    var x = (int)numArray.list[0].n;
                    var y = (int)numArray.list[1].n;
                    _roadPositions.Add(new Vector2((float)x, (float)y));
                }
            }
        }
        private void OnTerrainData(string terrain)
        {
            _terrain = terrain;
            _hasTerrainData = true;
        }

        private void Update()
        {
            if (isInitialized)
                return;
            if (!_hasTerrainData || !_hasMapData)
                return;
            if (!isInitializing)
            {
                isInitializing = true;
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

            isInitializing = false;
            isInitialized = true;
            Scheduler.Instance.Add(Deform);
        }

        private void generateWalls1()
        {
            const float wallConstant = 0.5f;
            const float wallRandom = 0.5f;
            const float lairDeformRange = 1.0f;
            const float lairConstant = 0.35f;
            const float lairRandom = 0.05f;
            const float tunnelConstant = 0.05f;
            const float tunnelRandom = 0.0f;

            // walls
            var vertices = _wallMesh.mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                var point = vertices[i];
                if (point.x < 0 || point.x > 50 || point.z < 0 || point.z > 50)
                    continue;

                var xf = point.x;
                var x = (int)point.x;
                if (x < 0 || x >= _wallPositions.GetLength(0))
                    continue;

                var yf = 50.0 - point.z;
                var y = 49 - (int)point.z;
                if (y < 0 || y >= _wallPositions.GetLength(1))
                    continue;

                if (!_wallPositions[x, y])
                    continue;

                bool isLair = false;
                foreach (var pos in _lairPositions)
                    if (Math.Abs(pos.x + 0.5 - xf) < lairDeformRange && Math.Abs(pos.y + 0.5 - yf) < lairDeformRange)
                    {
                        isLair = true;
                        break;
                    }
                bool isTunnel = false;
                if (i % 6 != 0)
                {
                    foreach (var pos in _roadPositions)
                    {
                        if (pos.x == x && pos.y == y)
                        {
                            isTunnel = true;
                            break;
                        }
                    }
                }

                if (isLair)
                    vertices[i] = new Vector3(point.x, lairConstant + UnityEngine.Random.value * lairRandom, point.z);
                else if (isTunnel)
                    vertices[i] = new Vector3(point.x, tunnelConstant + UnityEngine.Random.value * tunnelRandom, point.z);
                else
                    vertices[i] = new Vector3(point.x, wallConstant + UnityEngine.Random.value * wallRandom, point.z);
            }
            _wallMesh.mesh.vertices = vertices;
            _wallMesh.mesh.RecalculateNormals();
        }
        private float getRandom(int x, int z)
        {
            var seed = ((int)_room.Position.x + x) * 1000 + ((int)_room.Position.z + z);
            UnityEngine.Random.InitState(seed);
            return UnityEngine.Random.value;
        }
        private float getY(int x, int z, int depth)
        {
            const float wallDepth = 0.5f;
            const float wallRandom = 0.5f;
            const float wallStep = 0.1f;
            const float wallConstant = 0.5f;

            float Y = wallConstant
                + (float)Math.Round(getRandom(x, z) * wallRandom / wallStep) * wallStep
                + depth * wallDepth;
            return Y;
        }

        private void generateWalls2()
        {
            var wallCount = 0;
            for (int x = 0; x < 50; ++x)
                for (int y = 0; y < 50; ++y)
                    if (_wallPositions[x, y])
                        ++wallCount;

            var wallDepth = new int[50, 50];
            const int someHighNumber = 50;
            for (int y = 1; y < 50; ++y)
                for (int x = 1; x < 50; ++x)
                    wallDepth[x, y] = someHighNumber;
            for (int i = 0; i < 50; ++i)
            {
                wallDepth[0, i] = 1;
                wallDepth[49, i] = 1;
                wallDepth[i, 0] = 1;
                wallDepth[i, 49] = 1;
            }

            for (int y = 1; y < 49; ++y)
                for (int x = 1; x < 49; ++x)
                {
                    var z = 49 - y;
                    if (!_wallPositions[x, y])
                        wallDepth[x, z] = 0;
                    else
                    {
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x, z + 1] + 1);
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x - 1, z] + 1);
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x - 1, z + 1] + 1);
                        wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x + 1, z + 1] + 1);
                    }
                }
            for (int y = 48; y > 0; --y)
                for (int x = 48; x > 0; --x)
                {
                    var z = 49 - y;
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x, z - 1] + 1);
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x + 1, z] + 1);
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x + 1, z - 1] + 1);
                    wallDepth[x, z] = Math.Min(wallDepth[x, z], wallDepth[x - 1, z - 1] + 1);
                }

            const int quadsPerWall = 5;
            int vertCount = wallCount * 4 * quadsPerWall;
            int triangleCount = wallCount * 6 * quadsPerWall;

            var vertices = new Vector3[vertCount];
            var uv = new Vector2[vertCount];
            var triangles = new int[triangleCount];

            var index = 0;
            var tIndex = 0;
            for (int x = 0; x < 50; ++x)
                for (int y = 0; y < 50; ++y)
                {
                    if (_wallPositions[x, y])
                    {
                        var z = 49 - y;
                        var wallY1 = getY((int)_room.Position.x + x, (int)_room.Position.z + z, wallDepth[x, z]);
                        var wallY2 = wallY1;
                        var wallY3 = wallY1;
                        var wallY4 = wallY1;

                        vertices[index] = new Vector3(x, wallY1, z);
                        vertices[index + 1] = new Vector3(x, wallY2, z + 1);
                        vertices[index + 2] = new Vector3(x + 1, wallY3, z);
                        vertices[index + 3] = new Vector3(x + 1, wallY4, z + 1);
                        uv[index] = new Vector2(x, z);
                        uv[index + 1] = new Vector2(x, z + 1);
                        uv[index + 2] = new Vector2(x + 1, z);
                        uv[index + 3] = new Vector2(x + 1, z + 1);
                        triangles[tIndex] = index;
                        triangles[tIndex + 1] = index + 1;
                        triangles[tIndex + 2] = index + 2;
                        triangles[tIndex + 3] = index + 3;
                        triangles[tIndex + 4] = index + 2;
                        triangles[tIndex + 5] = index + 1;
                        index += 4;
                        tIndex += 6;

                        // sideTriangles
                        vertices[index] = new Vector3(x, wallY2, z + 1);
                        vertices[index + 1] = new Vector3(x, wallY1, z);
                        vertices[index + 2] = new Vector3(x, 0, z + 1);
                        vertices[index + 3] = new Vector3(x, 0, z);
                        uv[index] = new Vector2(x, z);
                        uv[index + 1] = new Vector2(x, z + 1);
                        uv[index + 2] = new Vector2(x + 1, z);
                        uv[index + 3] = new Vector2(x + 1, z + 1);
                        triangles[tIndex] = index;
                        triangles[tIndex + 1] = index + 1;
                        triangles[tIndex + 2] = index + 2;
                        triangles[tIndex + 3] = index + 3;
                        triangles[tIndex + 4] = index + 2;
                        triangles[tIndex + 5] = index + 1;
                        index += 4;
                        tIndex += 6;

                        vertices[index] = new Vector3(x, wallY2, z + 1);
                        vertices[index + 1] = new Vector3(x, 0, z + 1);
                        vertices[index + 2] = new Vector3(x + 1, wallY4, z + 1);
                        vertices[index + 3] = new Vector3(x + 1, 0, z + 1);
                        uv[index] = new Vector2(x, z);
                        uv[index + 1] = new Vector2(x, z + 1);
                        uv[index + 2] = new Vector2(x + 1, z);
                        uv[index + 3] = new Vector2(x + 1, z + 1);
                        triangles[tIndex] = index;
                        triangles[tIndex + 1] = index + 1;
                        triangles[tIndex + 2] = index + 2;
                        triangles[tIndex + 3] = index + 3;
                        triangles[tIndex + 4] = index + 2;
                        triangles[tIndex + 5] = index + 1;
                        index += 4;
                        tIndex += 6;
                        //
                        vertices[index] = new Vector3(x + 1, wallY3, z);
                        vertices[index + 1] = new Vector3(x + 1, wallY4, z + 1);
                        vertices[index + 2] = new Vector3(x + 1, 0, z);
                        vertices[index + 3] = new Vector3(x + 1, 0, z + 1);
                        uv[index] = new Vector2(x, z);
                        uv[index + 1] = new Vector2(x, z + 1);
                        uv[index + 2] = new Vector2(x + 1, z);
                        uv[index + 3] = new Vector2(x + 1, z + 1);
                        triangles[tIndex] = index;
                        triangles[tIndex + 1] = index + 1;
                        triangles[tIndex + 2] = index + 2;
                        triangles[tIndex + 3] = index + 3;
                        triangles[tIndex + 4] = index + 2;
                        triangles[tIndex + 5] = index + 1;
                        index += 4;
                        tIndex += 6;
                        //
                        vertices[index] = new Vector3(x + 1, wallY3, z);
                        vertices[index + 1] = new Vector3(x + 1, 0, z);
                        vertices[index + 2] = new Vector3(x, wallY1, z);
                        vertices[index + 3] = new Vector3(x, 0, z);
                        uv[index] = new Vector2(x, z);
                        uv[index + 1] = new Vector2(x, z + 1);
                        uv[index + 2] = new Vector2(x + 1, z);
                        uv[index + 3] = new Vector2(x + 1, z + 1);
                        triangles[tIndex] = index;
                        triangles[tIndex + 1] = index + 1;
                        triangles[tIndex + 2] = index + 2;
                        triangles[tIndex + 3] = index + 3;
                        triangles[tIndex + 4] = index + 2;
                        triangles[tIndex + 5] = index + 1;
                        index += 4;
                        tIndex += 6;
                    }
                }

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            _wallMesh.mesh = mesh;
        }

        private void Deform()
        {
            // change to generateWalls2();
            generateWalls1();

            const float swampConstant = 0.3f;
            const float swampRandom = 0.0f;

            // swamps
            var vertices = _swampMesh.mesh.vertices;
            for (var i = 0; i < vertices.Length; i++)
            {
                var point = vertices[i];
                if (point.x < 0 || point.x > 50 || point.z < 0 || point.z > 50)
                    continue;

                var x = (int)point.x;
                if (x < 0 || x >= _swampPositions.GetLength(0))
                    continue;

                var y = 49 - (int)point.z;
                if (y < 0 || y >= _swampPositions.GetLength(1))
                    continue;

                if (!_swampPositions[x, y])
                    continue;

                vertices[i] = new Vector3(point.x, swampConstant + UnityEngine.Random.value * swampRandom, point.z);
            }
            _swampMesh.mesh.vertices = vertices;
            _swampMesh.mesh.RecalculateNormals();

            _wallPositions = null;
            _swampPositions = null;
        }
    }
}