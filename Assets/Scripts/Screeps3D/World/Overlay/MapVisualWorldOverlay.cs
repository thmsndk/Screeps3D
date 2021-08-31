using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Common.SettingsManagement;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.World.Views;
using Common;
using Screeps_API;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using TMPro;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // On official, we subscribe to each shard. 
    // Should probably only render visual for selected shard, unless we do intershard visuals? 

    public class MapVisualWorldOverlay : MonoBehaviour
    {
        private static Shader _lineShader;
        private static Transform _parent;

        private Queue<JSONObject> queue = new Queue<JSONObject>();

        private Dictionary<string, WorldView> _views = new Dictionary<string, WorldView>();

        [SerializeField] private Material _AlphaCutMaterial = default;

        private static event EventHandler<bool> OnEnableMapVisuals;

        private static bool _ENABLE_MAP_VISUALS = false;

        [Setting("Overlay/(WIP) Map Visuals - Can cause crashes with large visuals", "Enable")]
        private static bool ENABLE_INTEGRATION
        {
            get => _ENABLE_MAP_VISUALS; set
            {
                _ENABLE_MAP_VISUALS = value;

                OnEnableMapVisuals?.Invoke(null, _ENABLE_MAP_VISUALS);
            }
        }

        // TODO: we will initialize a list of OwnerControllerLevelViewData
        public MapVisualWorldOverlay()
        {

            // TODO: we want to initialize 1 view, this view is then responsible for world wide rendering
            // inside this view, we might want object pooling, to render "sub views" over each room, in this case an OwnerControllerLevelPrefab for each room
            // we should be able to control how many of theese sub views we render, for example based on distance from player.
            // other kind of world views would be some sort of strategic view, intel, map visuals and such.


            // TODO: for each view, or well actually inside the view
            //_label.text = string.Format("{0}", _selected.Owner.Username);
            //_badge.sprite = Sprite.Create(_selected.Owner.Badge,
            //    new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));
        }

        private void Start()
        {
            _lineShader = Shader.Find("HDRP/Lit");
            _parent = new GameObject("MapVisual").transform;
            _parent.SetParent(this.gameObject.transform);

            if (ScreepsAPI.Server.Official)
            {
                // TODO: Handle shard change. 
                ScreepsAPI.Socket.Subscribe(string.Format("mapVisual:{0}/{1}", ScreepsAPI.Me.UserId, PlayerPosition.Instance.ShardName), RecieveData);
            }
            else
            {
                ScreepsAPI.Socket.Subscribe(string.Format("mapVisual:{0}", ScreepsAPI.Me.UserId), RecieveData);
            }
        }

        private void RecieveData(JSONObject data)
        {
            queue.Enqueue(data);
            //Debug.LogError(this.GetInstanceID() + " MapVisuals:RecieveData:" + queue.Count);
        }

        public const float OverlayDistance = 200;

        public const float OverlayHeight = 10;


        private enum LookingDirection
        {
            North,
            East,
            South,
            West
        }

        private LookingDirection _lookingDirection = LookingDirection.North;

        private void Update()
        {
            if (!_ENABLE_MAP_VISUALS)
            {
                foreach (Transform child in _parent.transform)
                {
                    child.gameObject.SetActive(false);
                }
                return;
            }

            //Debug.LogError(this.GetInstanceID() + "MapVisuals:Update:" + queue.Count);
            if (queue.Count > 0)
            {
                UnpackMapVisuals(queue.Dequeue());
            }

            //Debug.Log(CameraRig.Position);
            //Debug.Log(PlayerPosition.Instance.transform.position);
            //Debug.Log(Vector3.Angle(CameraRig.Position, PlayerPosition.Instance.transform.position));
            //Debug.Log(CameraRig.Rotation);
            // dpending if you rotate left or right, the y value will be negative or positive
            // it increases and increases and increases in either direction
            if (CameraRig.Rotation.y >= -50 && CameraRig.Rotation.y <= 50)
            {
                _lookingDirection = LookingDirection.North;
            }
            else if (CameraRig.Rotation.y >= 50 && CameraRig.Rotation.y <= 145)
            {
                _lookingDirection = LookingDirection.East;
            }
            else if (CameraRig.Rotation.y >= 145 || CameraRig.Rotation.y <= 225)
            {
                _lookingDirection = LookingDirection.South;
            }
            else if (CameraRig.Rotation.y >= 225 || CameraRig.Rotation.y <= 320)
            {
                _lookingDirection = LookingDirection.West;
            }
            //Debug.Log(_lookingDirection);

            //LoadViewsByPlayerPosition();
        }

        private void UnpackMapVisuals(JSONObject data)
        {
            // debug to allow testing
            ////if (_parent.transform.childCount > 0)
            ////{
            ////    return;
            ////}

            // https://en.wikipedia.org/wiki/JSON_streaming is used for map visuals it appears to be newline delimted.
            // 
            //data = new JSONObject(data.str)

            //Debug.LogError("UnpackMapVisuals: " + data.ToString());
            var list = data?.str?.Replace("\\\"", "\"")?.Split(new string[] { "}\\n" }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
            var hashes = new List<string>();
            foreach (var visual in list)
            {
                

                //Debug.LogError($"visual: {visual}");
                var item = new JSONObject(visual + "}" /* add end of json object back */);
                //Debug.LogError($"{item.ToString()}");
                var type = item["t"]; // l = line, c = circle, r = rectangle ,p = poly, t = text

                // TODO: generate a unique hash for the visuals, if we already render it, no need to generate a new one.
                var hash = GenerateVisualHash(item);

                var existingVisual = _parent.Find(hash);

                if (existingVisual != null)
                {
                    //Debug.LogError($"Adding {hash} to list and skipping");
                    hashes.Add(hash);
                    // Re-enable visual in case map visuals was toggled off
                    if (!existingVisual.gameObject.activeSelf)
                    {
                        existingVisual.gameObject.SetActive(true);
                    }
                    continue;
                }

                // TODO: being able to generate a visual room object is something that should be extracted out and utilized for room visuals as well
                // We might need a general purpose "View" factory, that ObjectViewFactory and WorldViewFactory could inherit from
                switch (type.str)
                {
                    case "l": // line

                        DrawLine(hash, item);

                        break;
                    case "c": // circle
                        DrawCircle(hash, item);

                        break;
                    case "r": // rectangle
                        DrawRectangle(hash, item);
                        break;
                    case "p": // poly
                        DrawPoly(hash, item);
                        break;
                    case "t": // text
                        DrawText(hash, item);
                        break;
                }

                //Debug.LogError($"Adding {hash} to list");
                hashes.Add(hash);
            }

            // clear old visuals not present in this unpack.
            foreach (Transform child in _parent.transform)
            {
                if (!hashes.Contains(child.name))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private string GenerateVisualHash(JSONObject item)
        {
            // loop all keys, string values together? in case of another object(style) return that data as well?
            var hash = new StringBuilder();

            foreach (var key in item.keys)
            {
                var value = item[key];

                if (value.IsObject)
                {
                    // TODO: recursive
                    continue;
                }

                if (value.IsString)
                {
                    hash.Append(value.str);
                    continue;
                }

                if (value.IsNumber)
                {
                    hash.Append(value.n);
                    continue;
                }
            }

            return hash.ToString();
        }

        private void DrawText(string hash, JSONObject item)
        {

            // TODO: text does not really scale the same way as in the official client.

            var x = item["x"];
            var y = item["y"];
            var n = item["n"];
            var text = item["text"];

            var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
            var pos = PosUtility.Convert((int)x.n, (int)y.n, room); // The position object of the top-left corner.


            var style = item["s"]; // optional
            // color string Font color in the following format: #ffffff (hex triplet). Default is #ffffff.
            var color = style != null ? style["color"] : null;
            // fontFamily string The font family, default is sans-serif
            // fontSize number The font size in game coordinates, default is 10
            var fontSize = style != null ? style["fontSize"] : null;
            // fontStyle string The font style ('normal', 'italic' or 'oblique')
            // fontVariant string The font variant ('normal' or 'small-caps')
            // stroke string Stroke color in the following format: #ffffff (hex triplet). Default is undefined (no stroke).
            var stroke = style != null ? style["stroke"] : null; 
            // strokeWidth number Stroke width, default is 0.15.
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            // backgroundColor string Background color in the following format: #ffffff (hex triplet). Default is undefined (no background). When background is enabled, text vertical align is set to middle (default is baseline).
            // backgroundPadding number Background rectangle padding, default is 2.
            // align string Text align, either center, left, or right. Default is center.
            // opacity number Opacity value, default is 0.5.
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5

            var go = new GameObject(hash);
            go.transform.SetParent(_parent);
            go.transform.Rotate(new Vector3(90, 0, 0));

            var tmpText = go.AddComponent<TextMeshPro>();
            
            //Debug.LogError(item.ToString());
            //Debug.LogError(text.ToString());
            tmpText.text = ScreepsConsole.RemoveEscapes(text.str);
            tmpText.ForceMeshUpdate();
            var renderedValues = tmpText.GetRenderedValues(false);
            //Debug.LogError(tmpText.textBounds.center.y);
            go.transform.position = pos + new Vector3(0, OverlayHeight, renderedValues.y*2);
            //Debug.LogError(tmpText.text);
            tmpText.fontSize = (fontSize != null ? (int)fontSize.n : 10) * 8; // what if the text is longer than "Target"
            tmpText.autoSizeTextContainer = true;
            tmpText.color = GetColor(color);

            var strokeColor = GetColor(stroke);
            
            // TODO: Figure out how to render svg opacity "properly" :S
            string svgOpacity = (opacity != null && !string.IsNullOrEmpty(opacity.str)) ? opacity.str : "0.5";
        }

        private void DrawPoly(string hash, JSONObject item)
        {
            // TODO: might want to add other polygon renderings for the testcases to verify if the magic numbers work as intended in most cases.
            var pointsObject = item["points"];

            var style = item["s"]; // optional
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var points = new List<Vector3>();
            var minX = 10f;
            var maxX = 10f;
            var minZ = 10f;
            var maxZ = 10f;

            foreach (var point in pointsObject.list)
            {
                var x = point["x"];
                var y = point["y"];
                var n = point["n"];

                var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
                var pointPos = PosUtility.Convert((int)x.n, (int)y.n, room);
                points.Add(pointPos);

                if (pointPos.x < minX)
                {
                    minX = pointPos.x;
                }

                if (pointPos.x > maxX)
                {
                    maxX = pointPos.x;
                }

                if (pointPos.z < minZ)
                {
                    minZ = pointPos.z;
                }

                if (pointPos.z > maxZ)
                {
                    maxZ = pointPos.z;
                }
            }

            var width = Math.Abs(minX - maxX) - 37.5f; // somewhat random magical values, should probably find a better solution
            var height = Math.Abs(minZ - maxZ) - 12.5f;

            var go = new GameObject(hash, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(_parent);

            // TODO: find width and height?
            var pos = points.FirstOrDefault();
            go.transform.position = pos + new Vector3(-width, OverlayHeight, 0);


            string svgStrokeColor = GetSvgColor(stroke);
            string svgStroke = string.IsNullOrEmpty(svgStrokeColor) ? "" : $" stroke=\"{svgStrokeColor}\"";

            // TODO: stroke-width

            string svgFillColor = GetSvgColor(fill);
            string svgFill = string.IsNullOrEmpty(svgFillColor) ? "" : $" fill=\"{svgFillColor}\"";

            // TODO: Figure out how to render svg opacity "properly" :S
            string svgOpacity = (opacity != null && !string.IsNullOrEmpty(opacity.str)) ? opacity.str : "0.5";

            string svgFillOpacity = $" fill-opacity=\"{svgOpacity}\""; //0..1
            string svgStrokeOpacity = $" stroke-opacity=\"{svgOpacity}\""; //0..1 // TODO: this basicly hides the line :thinking:
            // wonder if fill="rgba(124,240,10,0.5)" works better :thinking:

            // https://forum.unity.com/threads/vector-graphics-preview-package.529845/page-2
            // https://stackoverflow.com/questions/62044261/svg-opacity-in-unity
            // perhaps we need a giant canvas, or at least a canvas, to embed the gameobject onto? :thinking:

            // TODO: we have an odd issue where the spaces between lines gets further and further
            string svgLineStyle = GetSvgLineStyle(lineStyle);
            
            // TODO: Points are relative to 0,0 (top left) how do we translate that ?
            string svgPointsList = string.Join(" ", points.Select(p => $"{Math.Abs(p.x)},{Math.Abs(p.z)}"));
            string svgPoints = $"points=\"{svgPointsList}\"";

            string xml = $@"<svg height=""{height * 2}"" width=""{width * 2}"">
             <g transform=""rotate(180, 100, 100)"">
              <polygon {svgPoints}{svgStroke}{svgFill}{svgLineStyle}{svgFillOpacity}{svgStrokeOpacity} />
             </g>
            </svg>";

            //Debug.LogError(xml);

            var meshFilter = go.GetComponent<MeshFilter>();

            // TODO: could generate a mesh that is actually the correct shape :thinking:
            Mesh mesh = GenerateQuad(width, height);

            meshFilter.mesh = mesh;

            // TODO: figure out how we disable lighting, maybe it should be an unlit shader? nope, culling does not work ....
            SetTexture(go, xml);
        }


        private void DrawRectangle(string hash, JSONObject item)
        {
            var x = item["x"];
            var y = item["y"];
            var n = item["n"];
            var w = item["w"];
            var h = item["h"];

            var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
            var pos = PosUtility.Convert((int)x.n, (int)y.n, room); // The position object of the top-left corner.

            var style = item["s"]; // optional
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject(hash, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(_parent);
            // required, what if they are null? does screeps crash?
            var width = w != null ? w.n : 25; 
            var height = h != null ? h.n : 25;
            //Debug.LogError($"{width} {height}");
            go.transform.position = pos + new Vector3(0, OverlayHeight, -height);


            string svgStrokeColor = GetSvgColor(stroke);
            string svgStroke = string.IsNullOrEmpty(svgStrokeColor) ? "" : $" stroke=\"{svgStrokeColor}\"";

            // TODO: stroke-width

            string svgFillColor = GetSvgColor(fill);
            string svgFill = string.IsNullOrEmpty(svgFillColor) ? "" : $" fill=\"{svgFillColor}\"";

            // TODO: Figure out how to render svg opacity "properly" :S
            string svgOpacity = (opacity != null && !string.IsNullOrEmpty(opacity.str)) ? opacity.str : "0.5";
            
            string svgFillOpacity = $" fill-opacity=\"{svgOpacity}\""; //0..1
            string svgStrokeOpacity = $" stroke-opacity=\"{svgOpacity}\""; //0..1 // TODO: this basicly hides the line :thinking:
            // wonder if fill="rgba(124,240,10,0.5)" works better :thinking:

            // https://forum.unity.com/threads/vector-graphics-preview-package.529845/page-2
            // https://stackoverflow.com/questions/62044261/svg-opacity-in-unity
            // perhaps we need a giant canvas, or at least a canvas, to embed the gameobject onto? :thinking:

            // TODO: we have an odd issue where the spaces between lines gets further and further
            string svgLineStyle = GetSvgLineStyle(lineStyle);

            string xml = $@"<svg height=""{width * 2}"" width=""{height * 2}"">
              <rect width=""{width}"" height=""{height}"" {svgStroke}{svgFill}{svgLineStyle}{svgFillOpacity}{svgStrokeOpacity} />
            </svg>";

            //Debug.LogError(xml);

            var meshFilter = go.GetComponent<MeshFilter>();

            // TODO: generate it in such a way that we can position the quad and it is relative to the position (center i think)
            Mesh mesh = GenerateQuad(width, height);

            meshFilter.mesh = mesh;

            // TODO: figure out how we disable lighting, maybe it should be an unlit shader? nope, culling does not work ....
            SetTexture(go, xml);
        }

        private void DrawCircle(string hash, JSONObject item)
        {
            var x = item["x"];
            var y = item["y"];
            var n = item["n"];

            var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
            var pos = PosUtility.Convert((int)x.n, (int)y.n, room);

            var style = item["s"]; // optional
            var radiusObject = style != null ? style["radius"] : null; // number, default is 10
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject(hash, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(_parent);

            //float lineWidth = 1f;
            float radius = radiusObject != null ? radiusObject.n : 10f;
            go.transform.position = pos + new Vector3(-radius, OverlayHeight, -radius);
            // TODO: figure out proper positions

            var w = radius*2;
            var h = radius*2;

            string svgStrokeColor = GetSvgColor(stroke);
            string svgStroke = string.IsNullOrEmpty(svgStrokeColor) ? "" : $" stroke=\"{svgStrokeColor}\"";

            // TODO: strokes are having a bad time being rendered, I might need to make a test project and submit a bug report to unity :thinking:
            if (strokeWidth == null || !float.TryParse(strokeWidth.str,out var svgStrokeWidth))
            {
                svgStrokeWidth = 0.5f;
            }
            svgStrokeWidth *= 3;

            string svgStrokeWidthAttribute = $" stroke-width=\"{svgStrokeWidth.ToString(System.Globalization.CultureInfo.InvariantCulture)}\"";

            string svgFillColor = GetSvgColor(fill);
            string svgFill = string.IsNullOrEmpty(svgFillColor) ? "" : $" fill=\"{svgFillColor}\"";

            // TODO: Figure out how to render svg opacity "properly" :S
            string svgOpacity = (opacity != null && !string.IsNullOrEmpty(opacity.str)) ? opacity.str : "0.5";
            string svgFillOpacity = $" fill-opacity=\"{svgOpacity}\""; //0..1
            string svgStrokeOpacity = $" stroke-opacity=\"{svgOpacity}\""; //0..1 // TODO: this basicly hides the line :thinking:

            // TODO: we have an odd issue where the spaces between lines gets further and further
            string svgLineStyle = GetSvgLineStyle(lineStyle);

            string xml = $@"<svg height=""{w}"" width=""{h}"">
              <circle cx=""{radius}"" cy=""{radius}"" r=""{radius}""{svgStroke}{svgFill}{svgLineStyle}{svgFillOpacity}{svgStrokeOpacity}{svgStrokeWidthAttribute} />
            </svg>";

            Debug.LogError(xml);

            var meshFilter = go.GetComponent<MeshFilter>();

            // TODO: generate it in such a way that we can position the quad and it is relative to the position (center i think)
            Mesh mesh = GenerateQuad(w, h);

            meshFilter.mesh = mesh;
            SetTexture(go, xml);
        }

        private void DrawLine(string hash, JSONObject item)
        {
            var x1 = item["x1"]; // X
            var y1 = item["y1"]; // Y
            var n1 = item["n1"]; // roomName
            var room1 = RoomManager.Instance.Get(n1.str, PlayerPosition.Instance.ShardName);
            var pos1 = PosUtility.Convert((int)x1.n, (int)y1.n, room1) + Vector3.up * OverlayHeight;
            var x2 = item["x2"]; // X
            var y2 = item["y2"]; // Y
            var n2 = item["n2"]; // roomName

            var room2 = RoomManager.Instance.Get(n2.str, PlayerPosition.Instance.ShardName);
            var pos2 = PosUtility.Convert((int)x2.n, (int)y2.n, room2) + Vector3.up * OverlayHeight;

            var style = item["s"]; // optional
            var width = style != null ? style["width"] : null; // number, default is 0.1
            var color = style != null ? style["color"] : null; // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject(hash, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(_parent);

            var w = Math.Abs(pos1.x - pos2.x);
            var h = Math.Abs(pos1.z - pos2.z);
            go.transform.position = room1.Position + new Vector3(-(w) + 25, OverlayHeight, 25);

            string svgHexColor = GetSvgColor(color);

            // TODO: we have an odd issue where the spaces between lines gets further and further
            string svgLineStyle = GetSvgLineStyle(lineStyle);

            string xml = $@"<svg height=""{w}"" width=""{h}"">
              <line x1=""0"" y1=""0"" x2=""{w}"" y2=""{h}"" {svgLineStyle} style=""stroke:{svgHexColor};stroke-width:2"" />
            </svg>";

            //Debug.LogError(xml);

            var meshFilter = go.GetComponent<MeshFilter>();

            Mesh mesh = GenerateQuad(w, h);

            meshFilter.mesh = mesh;
            SetTexture(go, xml);
        }

        private void SetTexture(GameObject go, string xml)
        {
            var texture = Texturize(xml);

            var rend = go.GetComponent<MeshRenderer>();
            var mat = new Material(_AlphaCutMaterial);
            rend.receiveShadows = false;

            mat.SetTexture(ShaderKeys.HDRPLit.Texture, texture);
            rend.material = mat;
        }

        private static string GetSvgLineStyle(JSONObject lineStyle)
        {
            var svgLineStyle = "";

            if (lineStyle == null)
            {
                return svgLineStyle;
            }

            // TODO: we have an odd issue where dash-array stretches further and further the longer it runs
            switch (lineStyle.str)
            {
                case "dashed":
                    svgLineStyle = " stroke-dasharray=\"5,5\"";
                    break;
                case "dotted":
                    svgLineStyle = " stroke-dasharray=\"2,2\"";
                    break;
            }

            return svgLineStyle;
        }

        private static Color GetColor(JSONObject json)
        {
            var colorString = json != null ? json.str : string.Empty;

            if (string.IsNullOrEmpty(colorString))
            {
                // hex color code, default is #ffffff
                return Color.white;
            }

            if (!colorString.StartsWith("#"))
            {
                var colorInfo = ColorToHex.Parse(colorString);
                if (colorInfo != null)
                {
                    colorString = colorInfo.Hex;
                }
            }

            if (ColorUtility.TryParseHtmlString(colorString, out var color))
            {
                return color;
            }

            return Color.white;
        }

        private static string GetSvgColor(JSONObject color)
        {
            var svgHexColor = color != null ? color.str : string.Empty;

            if (string.IsNullOrEmpty(svgHexColor))
            {
                // hex color code, default is #ffffff
                return "#ffffff";
            }

            if (!svgHexColor.StartsWith("#") && svgHexColor != "transparent")
            {
                var colorInfo = ColorToHex.Parse(svgHexColor);
                if (colorInfo != null)
                {
                    svgHexColor = colorInfo.Hex;
                }
            }

            return svgHexColor;
        }

        private static Mesh GenerateQuad(float w, float h)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(w, 0, 0),
                new Vector3(0, 0, h),
                new Vector3(w, 0, h)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                  new Vector2(0, 0),
                  new Vector2(1, 0),
                  new Vector2(0, 1),
                  new Vector2(1, 1)
            };
            mesh.uv = uv;
            return mesh;
        }

        private static Texture2D Texturize(string xml)
        {
            // From BadgeManager
            using (var reader = new StringReader(xml))
            {
                var sceneInfo = SVGParser.ImportSVG(reader);

                var geometry = VectorUtils.TessellateScene(sceneInfo.Scene, new VectorUtils.TessellationOptions
                {
                    // https://docs.unity3d.com/Packages/com.unity.vectorgraphics@1.0/manual/index.html
                    // Theese options needs to be set, else it fails
                    StepDistance = 0.1f, // This gives a smoothe circle, gotta figure out how high we can go.
                    SamplingStepSize = 100f,
                    MaxCordDeviation = 0.5f,
                    MaxTanAngleDeviation = 0.1f
                });

                var sprite = VectorUtils.BuildSprite(geometry, 1, VectorUtils.Alignment.Center, Vector2.zero, 0, true);

                // var mat = new Material(Shader.Find("Unlit/VectorGradient"));
                var mat = new Material(Shader.Find("Unlit/Vector"));

                return VectorUtils.RenderSpriteToTexture2D(sprite, 1024, 1024, mat);
            }
        }

        ////private void LoadViewsByPlayerPosition()
        ////{
        ////    var playerPosition = PlayerPosition.Instance.transform.position;
        ////    foreach (var col in Physics.OverlapSphere(playerPosition, OverlayDistance, 1 << 10))
        ////    {
        ////        var roomView = col.GetComponent<RoomView>();
        ////        if (roomView == null || roomView.Room == null || Vector3.Distance(playerPosition, roomView.Room.Position) > OverlayDistance)
        ////        {
        ////            continue;
        ////        }

        ////        var roomInfo = MapStatsUpdater.Instance.GetRoomInfo(PlayerPosition.Instance.ShardName, roomView.Room.RoomName);

        ////        if (roomInfo == null)
        ////        {
        ////            continue;
        ////        }

        ////        if (!_views.TryGetValue(roomInfo.RoomName, out var view))
        ////        {
        ////            _views[roomInfo.RoomName] = null; // Add it with a null value to prevent repated queueing

        ////            Scheduler.Instance.Add(() =>
        ////            {
        ////                var room = RoomManager.Instance.Get(roomInfo.RoomName, PlayerPosition.Instance.ShardName);
        ////                var data = new OwnerControllerLevelData(room, roomInfo);
        ////                var o = WorldViewFactory.GetInstance(data);
        ////                o.name = $"{roomInfo.RoomName}:RoomOwnerInfoView";
        ////                _views[roomInfo.RoomName] = o;
        ////            });
        ////        }

        ////        if (view != null)
        ////        {
        ////        // TODO: trigger an update?

        ////        }
        ////    }
        ////}
    }

    ////public class OwnerControllerLevelData : WorldViewData
    ////{
    ////    public OwnerControllerLevelData(Room room, RoomInfo roomInfo)
    ////    {
    ////        Type = "RoomOwnerInfo";
    ////        Room = room;
    ////        RoomInfo = roomInfo;
    ////    }

    ////    public Room Room { get; }
    ////    public RoomInfo RoomInfo { get; }
    ////}
}

/* We seem to be recieving a "list" of objects, that are not seperated.
             * 
             * {
	                "t": "l",
	                "x1": 25,
	                "y1": 25,
	                "n1": "W8N1",
	                "x2": 25,
	                "y2": 25,
	                "n2": "W8N2",
	                "s": {
		                "color": "#ff0000",
		                "lineStyle": "dashed"
	                }
                }{
	                "t": "c",
	                "x": 25,
	                "y": 25,
	                "n": "W2N4"
                }{
	                "t": "c",
	                "x": 25,
	                "y": 25,
	                "n": "W8N1",
	                "s": {
		                "fill": "transparent",
		                "radius": 500,
		                "stroke": "#ff0000"
	                }
                }{
	                "t": "r",
	                "x": 13,
	                "y": 2,
	                "n": "W1N4",
	                "w": 11,
	                "h": 11,
	                "s": {
		                "fill": "transparent",
		                "stroke": "#ff0000"
	                }
                }{
	                "t": "p",
	                "points": [{
			                "x": 25,
			                "y": 25,
			                "n": "W8N1"
		                }, {
			                "x": 25,
			                "y": 25,
			                "n": "W8N2"
		                }, {
			                "x": 20,
			                "y": 21,
			                "n": "W1N1"
		                }
	                ],
	                "s": {
		                "fill": "aqua"
	                }
                }{
	                "t": "t",
	                "text": "Target💥",
	                "x": 11,
	                "y": 14,
	                "n": "W2N4",
	                "s": {
		                "color": "#FF0000",
		                "fontSize": 10
	                }
                }

             * */
