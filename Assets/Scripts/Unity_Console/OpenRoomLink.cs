using Screeps3D.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// https://deltadreamgames.com/unity-tmp-hyperlinks/
namespace Assets.Scripts.Screeps3D
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class OpenRoomLink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private RoomChooser _chooser;
        private TextMeshProUGUI m_TextMeshPro;

        private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

        private bool isHoveringObject;
        private int selectedLink = -1;
        private int m_lastIndex = -1;

        private void Awake()
        {
            m_TextMeshPro = GetComponent<TextMeshProUGUI>();
            var roomChooserGameObject = GameObject.FindGameObjectWithTag("RoomChooser");
            _chooser = roomChooserGameObject.GetComponent<RoomChooser>();
        }

        void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        void OnDisable()
        {
            // UnSubscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }

        void ON_TEXT_CHANGED(UnityEngine.Object obj)
        {
            if (obj == m_TextMeshPro)
            {
                // Update cached vertex data.
                m_cachedMeshInfoVertexData = m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // http://digitalnativestudios.com/forum/index.php?topic=838.0
            // ScreenSpace overlay doesn't use a Camera so you need to pass null for the Camera.

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, null);
            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                //Application.OpenURL(linkInfo.GetLinkID());
                //Debug.Log("link!!" + linkInfo.GetLinkID());
                var linkId = linkInfo.GetLinkID();
                Debug.Log(linkId);
                var shardAndRoom = linkId.Split('/');
                var shard = shardAndRoom[0];
                var room = shardAndRoom[1];
                if (!string.IsNullOrEmpty(shard))
                {
                    _chooser.OnSelectedShardChanged(shard);
                }
                _chooser.GetAndChooseRoom(room);

            }
        }

        void LateUpdate()
        {
            if(isHoveringObject) {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, null);
                if (linkIndex != -1 /*&& linkIndex != selectedLink*/)
                {
                    selectedLink = linkIndex;
                    SetLinkToColor(linkIndex, UnityEngine.Random.ColorHSV(38f / 359f, 38.5f / 359f, 0.46f, 0.50f, 0.89f, 0.99f)); // sand/yellow
                    // TODO: change cursor to indicate that it is clickable, e.g. a hand
                    
                }

            }
            else
            {
                // Taken from TMP_TxtSelector_B it aint really working properly though, do we need the swapping stuff??
                // Restore any character that may have been modified
                //if (m_lastIndex != -1)
                //{
                    //RestoreCachedVertexAttributes(0);
                    //m_lastIndex = -1;
                //}
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // isHovering object
            isHoveringObject = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHoveringObject = false;
        }

        // https://deltadreamgames.com/unity-tmp-hyperlinks/
        List<Color32[]> SetLinkToColor(int linkIndex, Color32 color)
        {
            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

            var oldVertColors = new List<Color32[]>(); // store the old character colors

            for (int i = 0; i < linkInfo.linkTextLength; i++)
            { // for each character in the link string
                int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
                var charInfo = m_TextMeshPro.textInfo.characterInfo[characterIndex];
                int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
                int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

                Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32; // the colors for this character
                oldVertColors.Add(vertexColors.ToArray());

                if (charInfo.isVisible)
                {
                    vertexColors[vertexIndex + 0] = color;
                    vertexColors[vertexIndex + 1] = color;
                    vertexColors[vertexIndex + 2] = color;
                    vertexColors[vertexIndex + 3] = color;
                }
            }

            // Update Geometry
            m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            return oldVertColors;
        }

        void RestoreCachedVertexAttributes(int index)
        {
            if (index == -1 || index > m_TextMeshPro.textInfo.characterCount - 1) return;

            // Get the index of the material / sub text object used by this character.
            int materialIndex = m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;

            // Get the index of the first vertex of the selected character.
            int vertexIndex = m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;

            // Restore Vertices
            // Get a reference to the cached / original vertices.
            Vector3[] src_vertices = m_cachedMeshInfoVertexData[materialIndex].vertices;

            // Get a reference to the vertices that we need to replace.
            Vector3[] dst_vertices = m_TextMeshPro.textInfo.meshInfo[materialIndex].vertices;

            // Restore / Copy vertices from source to destination
            dst_vertices[vertexIndex + 0] = src_vertices[vertexIndex + 0];
            dst_vertices[vertexIndex + 1] = src_vertices[vertexIndex + 1];
            dst_vertices[vertexIndex + 2] = src_vertices[vertexIndex + 2];
            dst_vertices[vertexIndex + 3] = src_vertices[vertexIndex + 3];

            // Restore Vertex Colors
            // Get a reference to the vertex colors we need to replace.
            Color32[] dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;

            // Get a reference to the cached / original vertex colors.
            Color32[] src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;

            // Copy the vertex colors from source to destination.
            dst_colors[vertexIndex + 0] = src_colors[vertexIndex + 0];
            dst_colors[vertexIndex + 1] = src_colors[vertexIndex + 1];
            dst_colors[vertexIndex + 2] = src_colors[vertexIndex + 2];
            dst_colors[vertexIndex + 3] = src_colors[vertexIndex + 3];

            // Restore UV0S
            // UVS0
            Vector2[] src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
            Vector2[] dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
            dst_uv0s[vertexIndex + 0] = src_uv0s[vertexIndex + 0];
            dst_uv0s[vertexIndex + 1] = src_uv0s[vertexIndex + 1];
            dst_uv0s[vertexIndex + 2] = src_uv0s[vertexIndex + 2];
            dst_uv0s[vertexIndex + 3] = src_uv0s[vertexIndex + 3];

            // UVS2
            Vector2[] src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
            Vector2[] dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
            dst_uv2s[vertexIndex + 0] = src_uv2s[vertexIndex + 0];
            dst_uv2s[vertexIndex + 1] = src_uv2s[vertexIndex + 1];
            dst_uv2s[vertexIndex + 2] = src_uv2s[vertexIndex + 2];
            dst_uv2s[vertexIndex + 3] = src_uv2s[vertexIndex + 3];


            // Restore last vertex attribute as we swapped it as well
            int lastIndex = (src_vertices.Length / 4 - 1) * 4;

            // Vertices
            dst_vertices[lastIndex + 0] = src_vertices[lastIndex + 0];
            dst_vertices[lastIndex + 1] = src_vertices[lastIndex + 1];
            dst_vertices[lastIndex + 2] = src_vertices[lastIndex + 2];
            dst_vertices[lastIndex + 3] = src_vertices[lastIndex + 3];

            // Vertex Colors
            src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;
            dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;
            dst_colors[lastIndex + 0] = src_colors[lastIndex + 0];
            dst_colors[lastIndex + 1] = src_colors[lastIndex + 1];
            dst_colors[lastIndex + 2] = src_colors[lastIndex + 2];
            dst_colors[lastIndex + 3] = src_colors[lastIndex + 3];

            // UVS0
            src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
            dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
            dst_uv0s[lastIndex + 0] = src_uv0s[lastIndex + 0];
            dst_uv0s[lastIndex + 1] = src_uv0s[lastIndex + 1];
            dst_uv0s[lastIndex + 2] = src_uv0s[lastIndex + 2];
            dst_uv0s[lastIndex + 3] = src_uv0s[lastIndex + 3];

            // UVS2
            src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
            dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
            dst_uv2s[lastIndex + 0] = src_uv2s[lastIndex + 0];
            dst_uv2s[lastIndex + 1] = src_uv2s[lastIndex + 1];
            dst_uv2s[lastIndex + 2] = src_uv2s[lastIndex + 2];
            dst_uv2s[lastIndex + 3] = src_uv2s[lastIndex + 3];

            // Need to update the appropriate 
            m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }


    }
}
