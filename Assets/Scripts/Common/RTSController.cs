using UnityEngine;

namespace Common
{
    public class RTSController : MonoBehaviour
    {
        [SerializeField] private Collider _dragCollider = default;
        private Vector3 _posLastFrame;
        private bool _dragging;

        // Update is called once per frame
        void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                _dragging = false;
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider == _dragCollider)
                {
                    Debug.Log("hit!");
                    if (_dragging)
                    {
                        var delta = _posLastFrame - hitInfo.point;
                        transform.position += delta;
                    }
                    _posLastFrame = hitInfo.point;
                    _dragging = true;
                }
            }
        }
    }
}