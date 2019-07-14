using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using TMPro;
using Screeps3D.Player;

public class CompassHandler : MonoBehaviour
{
    public float numberOfPixelsNorthToNorth;
    //public GameObject target;
    Vector3 startPosition;
    float rationAngleToPixel;
    public RawImage compass;
    //public SVGImage compass;
    public Transform player;

    public TMP_Text roomName;

    void Start()
    {
        startPosition = transform.position;
        rationAngleToPixel = numberOfPixelsNorthToNorth / 360f;
    }

    void Update()
    {
        //Vector3 perp = Vector3.Cross(Vector3.forward, player.transform.forward);
        //float dir = Vector3.Dot(perp, Vector3.up);

        //var plane2d = Vector3.Angle(player.transform.forward, Vector3.forward);
        //var plane3d = Vector3.Angle(new Vector3(player.transform.forward.x, 0f, player.transform.forward.z), Vector3.forward);

        //compass.transform.position = startPosition + (new Vector3(plane2d * Mathf.Sign(dir) * rationAngleToPixel, 0, 0));

        //compass.transform.position = startPosition + new Vector3(1f, 0f, 0f); //+ new Vector3(player.localEulerAngles.y / 360f, 0);

        // RawImage
        compass.uvRect = new Rect(player.localEulerAngles.y / 360f, 0, 1, 1);

        if (roomName != null)
        {
            roomName.text = PlayerPosition.Instance.RoomName;
        }
    }
}
