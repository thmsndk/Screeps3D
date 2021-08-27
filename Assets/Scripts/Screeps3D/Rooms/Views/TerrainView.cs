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
        [SerializeField] private MeshFilter _terrainMesh = default;

        private bool _hasTerrainData;
        private string _terrain;
        private bool _hasMapData;
        private List<Vector2Int> _lairPositions;
        private List<Vector2Int> _roadPositions;
        private List<Vector2Int> _sourcePositions;
        private List<Vector2Int> _controllerPositions;
        private List<Vector2Int> _mineralPositions;
        private List<Vector2Int> _powerBankPositions;

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
            _lairPositions = new List<Vector2Int>();
            _roadPositions = new List<Vector2Int>();
            _sourcePositions = new List<Vector2Int>();
            _controllerPositions = new List<Vector2Int>();
            _mineralPositions = new List<Vector2Int>();
            _powerBankPositions = new List<Vector2Int>();
            enabled = true;
            isInitialized = false;
        }
        private void OnMapData(JSONObject data)
        {
            _hasMapData = true;
            Action<JSONObject, List<Vector2Int>> addPositions = (JSONObject jsonPositions, List<Vector2Int> positions) =>
            {
                foreach (var numArray in jsonPositions.list)
                {
                    var x = (int)numArray.list[0].n;
                    var y = (int)numArray.list[1].n;
                    positions.Add(new Vector2Int(x, y));
                }
            };
            if (data["k"] != null)
            {
                addPositions(data["k"], _lairPositions);
            }
            if (data["r"] != null)
            {
                addPositions(data["r"], _roadPositions);
            }
            if (data["c"] != null)
            {
                addPositions(data["c"], _controllerPositions);
            }
            if (data["s"] != null)
            {
                addPositions(data["s"], _sourcePositions);
            }
            if (data["m"] != null)
            {
                addPositions(data["m"], _mineralPositions);
            }
            if (data["pb"] != null)
            {
                addPositions(data["pb"], _powerBankPositions);
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
            const int maxDepth = 3;
            const float wallDepth = 2.0f;
            const float wallRandom = 1.0f;
            const float wallStep = 0.25f;
            const float wallConstant = 0.0f;

            float Y = wallConstant
                + (float)Math.Round(getRandom(x, z) * wallRandom / wallStep) * wallStep
                + Math.Min(maxDepth, depth) * wallDepth;
            return Y;
        }

        private void generateWallsAndTerrain()
        {
            // calculate wall depth
            var wallDepth = new int[50, 50];
            const int someHighNumber = 50;
            for (int y = 0; y < 50; ++y)
                for (int x = 0; x < 50; ++x)
                    wallDepth[x, y] = someHighNumber;

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

            for (int y = 1; y < 49; ++y)
                for (int x = 1; x < 49; ++x)
                {
                    if (wallDepth[x, y] != wallDepth[x + 1, y]
                        && wallDepth[x, y] != wallDepth[x - 1, y]
                        && wallDepth[x, y] != wallDepth[x, y + 1]
                        && wallDepth[x, y] != wallDepth[x, y - 1])
                        --wallDepth[x, y];
                }

            // calculate wall heights
            var wallHeight = new float[50, 50];
            for (int x = 0; x < 50; ++x)
                for (int y = 0; y < 50; ++y)
                {
                    if (_wallPositions[x, y])
                    {
                        var z = 49 - y;
                        wallHeight[x, y] = getY((int)_room.Position.x + x, (int)_room.Position.z + z, wallDepth[x, z]);
                    }
                    else
                    {
                        wallHeight[x, y] = -0.6f;
                    }
                }
            foreach (var pos in _sourcePositions)
            {
                wallHeight[pos.x, pos.y] = 3.75f;
            }
            foreach (var pos in _lairPositions)
            {
                for (int x = pos.x - 1; x <= pos.x + 1; ++x)
                    for (int y = pos.y - 1; y <= pos.y + 1; ++y)
                        if (x >= 0 && x <= 49 && y >= 0 && y <= 49)
                            wallHeight[x, y] = Math.Min(wallHeight[x, y], 0.35f + getRandom(x, y) * 0.05f);
            }
            foreach (var pos in _mineralPositions)
            {
                for (int x = pos.x - 1; x <= pos.x + 1; ++x)
                    for (int y = pos.y - 1; y <= pos.y + 1; ++y)
                        if (x >= 0 && x <= 49 && y >= 0 && y <= 49)
                            wallHeight[x, y] = Math.Min(wallHeight[x, y], 0.5f + getRandom(x, y) * 0.25f);
            }
            foreach (var pos in _controllerPositions)
            {
                for (int x = pos.x - 1; x <= pos.x + 1; ++x)
                    for (int y = pos.y - 1; y <= pos.y + 1; ++y)
                        if (x >= 0 && x <= 49 && y >= 0 && y <= 49)
                            wallHeight[x, y] = Math.Min(wallHeight[x, y], .35f + getRandom(x, y) * 0.5f);
            }
            foreach (var pos in _powerBankPositions)
            {
                for (int x = pos.x - 1; x <= pos.x + 1; ++x)
                    for (int y = pos.y - 1; y <= pos.y + 1; ++y)
                        if (x >= 0 && x <= 49 && y >= 0 && y <= 49)
                            wallHeight[x, y] = Math.Min(wallHeight[x, y], 0.45f + getRandom(x, y) * 0.25f);
            }


            // make mesh
            var wallCount = 50 * 50;
            // for (int x = 0; x < 50; ++x)
            //     for (int y = 0; y < 50; ++y)
            //         if (_wallPositions[x, y])
            //             ++wallCount;
            const int quadsPerWall = 5;
            int wallVertCount = wallCount * 4 * quadsPerWall;
            int wallTriangleCount = wallCount * 6 * quadsPerWall;

            var wallVertices = new Vector3[wallVertCount];
            var wallUV = new Vector2[wallVertCount];
            var wallTriangles = new int[wallTriangleCount];

            var wallIndex = 0;
            var wallTriIndex = 0;

            Action<Vector3, Vector3, Vector3, Vector3> addWallQuad = (Vector3 A, Vector3 B, Vector3 C, Vector3 D) =>
            {
                wallVertices[wallIndex] = A;
                wallVertices[wallIndex + 1] = B;
                wallVertices[wallIndex + 2] = C;
                wallVertices[wallIndex + 3] = D;
                wallUV[wallIndex] = new Vector2(0, 0);
                wallUV[wallIndex + 1] = new Vector2(0, 1);
                wallUV[wallIndex + 2] = new Vector2(1, 0);
                wallUV[wallIndex + 3] = new Vector2(1, 1);
                wallTriangles[wallTriIndex] = wallIndex;
                wallTriangles[wallTriIndex + 1] = wallIndex + 1;
                wallTriangles[wallTriIndex + 2] = wallIndex + 2;
                wallTriangles[wallTriIndex + 3] = wallIndex + 3;
                wallTriangles[wallTriIndex + 4] = wallIndex + 2;
                wallTriangles[wallTriIndex + 5] = wallIndex + 1;
                wallIndex += 4;
                wallTriIndex += 6;
            };

            const int quadsPerTerrain = 5;

            var terrainVertices = new Vector3[wallVertCount];
            var terrainUV = new Vector2[wallVertCount];
            var terrainTriangles = new int[wallTriangleCount];

            var terrainIndex = 0;
            var terrainTriIndex = 0;
            Action<Vector3, Vector3, Vector3, Vector3> addTerrainQuad = (Vector3 A, Vector3 B, Vector3 C, Vector3 D) =>
            {
                terrainVertices[terrainIndex] = A;
                terrainVertices[terrainIndex + 1] = B;
                terrainVertices[terrainIndex + 2] = C;
                terrainVertices[terrainIndex + 3] = D;
                terrainUV[terrainIndex] = new Vector2(0, 0);
                terrainUV[terrainIndex + 1] = new Vector2(0, 1);
                terrainUV[terrainIndex + 2] = new Vector2(1, 0);
                terrainUV[terrainIndex + 3] = new Vector2(1, 1);
                terrainTriangles[terrainTriIndex] = terrainIndex;
                terrainTriangles[terrainTriIndex + 1] = terrainIndex + 1;
                terrainTriangles[terrainTriIndex + 2] = terrainIndex + 2;
                terrainTriangles[terrainTriIndex + 3] = terrainIndex + 3;
                terrainTriangles[terrainTriIndex + 4] = terrainIndex + 2;
                terrainTriangles[terrainTriIndex + 5] = terrainIndex + 1;
                terrainIndex += 4;
                terrainTriIndex += 6;
            };

            for (int x = 0; x < 50; ++x)
                for (int y = 0; y < 50; ++y)
                {
                    if (_wallPositions[x, y])
                    {
                        float h = wallHeight[x, y];
                        float h2;
                        var z = 49 - y;

                        addWallQuad(
                            new Vector3(x, h, z),
                            new Vector3(x, h, z + 1),
                            new Vector3(x + 1, h, z),
                            new Vector3(x + 1, h, z + 1));

                        h2 = x > 0 ? wallHeight[x - 1, y] : -0.6f;
                        addWallQuad(
                            new Vector3(x, h, z + 1),
                            new Vector3(x, h, z),
                            new Vector3(x, h2, z + 1),
                            new Vector3(x, h2, z));
                        h2 = x < 49 ? wallHeight[x + 1, y] : -0.6f;
                        addWallQuad(
                            new Vector3(x + 1, h, z),
                            new Vector3(x + 1, h, z + 1),
                            new Vector3(x + 1, h2, z),
                            new Vector3(x + 1, h2, z + 1));

                        h2 = y > 0 ? wallHeight[x, y - 1] : -0.6f;
                        addWallQuad(
                            new Vector3(x + 1, h, z + 1),
                            new Vector3(x, h, z + 1),
                            new Vector3(x + 1, h2, z + 1),
                            new Vector3(x, h2, z + 1));
                        h2 = y < 49 ? wallHeight[x, y + 1] : -0.6f;
                        addWallQuad(
                            new Vector3(x, h, z),
                            new Vector3(x + 1, h, z),
                            new Vector3(x, h2, z),
                            new Vector3(x + 1, h2, z));
                    }
                    else if (!_swampPositions[x, y])
                    {
                        float h = UnityEngine.Random.Range(-0.05f, 0.02f);
                        float h2 = -0.3f;
                        var z = 49 - y;

                        addTerrainQuad(
                            new Vector3(x, h, z),
                            new Vector3(x, h, z + 1),
                            new Vector3(x + 1, h, z),
                            new Vector3(x + 1, h, z + 1));

                        addTerrainQuad(
                            new Vector3(x, h, z + 1),
                            new Vector3(x, h, z),
                            new Vector3(x, h2, z + 1),
                            new Vector3(x, h2, z));
                        addTerrainQuad(
                            new Vector3(x + 1, h, z),
                            new Vector3(x + 1, h, z + 1),
                            new Vector3(x + 1, h2, z),
                            new Vector3(x + 1, h2, z + 1));
                        addTerrainQuad(
                            new Vector3(x + 1, h, z + 1),
                            new Vector3(x, h, z + 1),
                            new Vector3(x + 1, h2, z + 1),
                            new Vector3(x, h2, z + 1));
                        addTerrainQuad(
                            new Vector3(x, h, z),
                            new Vector3(x + 1, h, z),
                            new Vector3(x, h2, z),
                            new Vector3(x + 1, h2, z));
                    }
                }

            Mesh wallMesh = new Mesh();
            wallMesh.Clear();
            wallMesh.vertices = wallVertices;
            wallMesh.uv = wallUV;
            wallMesh.triangles = wallTriangles;
            wallMesh.RecalculateNormals();
            _wallMesh.mesh = wallMesh;

            Mesh terrainMesh = new Mesh();
            terrainMesh.Clear();
            terrainMesh.vertices = terrainVertices;
            terrainMesh.uv = terrainUV;
            terrainMesh.triangles = terrainTriangles;
            terrainMesh.RecalculateNormals();
            _terrainMesh.mesh = terrainMesh;
        }

        private void generateTerrain()
        {
            const float terrainSwampHole = -0.3f;
            const float swampRandom = 0.0f;

            // swamps
            var vertices = _terrainMesh.sharedMesh.vertices;
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

                vertices[i] = new Vector3(point.x, terrainSwampHole + UnityEngine.Random.value * swampRandom, point.z);
            }
            _terrainMesh.sharedMesh.vertices = vertices;
            _terrainMesh.sharedMesh.RecalculateNormals();
        }

        private void Deform()
        {
            // generateSwamps();
            // generateTerrain();
            // generateWalls1();
            generateWallsAndTerrain();

            _wallPositions = null;
            _swampPositions = null;
        }
    }
}