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
    public class OpenRoomLink : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private RoomChooser _chooser;
        private TextMeshProUGUI pTextMeshPro;

        private void Awake()
        {
            pTextMeshPro = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // http://digitalnativestudios.com/forum/index.php?topic=838.0
            // ScreenSpace overlay doesn't use a Camera so you need to pass null for the Camera.

            int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, null);
            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

                // open the link id as a url, which is the metadata we added in the text field
                //Application.OpenURL(linkInfo.GetLinkID());
                //Debug.Log("link!!" + linkInfo.GetLinkID());
                var linkId = linkInfo.GetLinkID();
                _chooser.GetAndChooseRoom(linkId);

            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // isHovering object
            var isHoveringOver = TMP_TextUtilities.IsIntersectingRectTransform(pTextMeshPro.rectTransform, Input.mousePosition, Camera.main);
        }

        List<Color32[]> SetLinkToColor(int linkIndex, Color32 color)
        {
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            var oldVertColors = new List<Color32[]>(); // store the old character colors

            for (int i = 0; i < linkInfo.linkTextLength; i++)
            { // for each character in the link string
                int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
                var charInfo = pTextMeshPro.textInfo.characterInfo[characterIndex];
                int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
                int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

                Color32[] vertexColors = pTextMeshPro.textInfo.meshInfo[meshIndex].colors32; // the colors for this character
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
            pTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

            return oldVertColors;
        }
    }
}
