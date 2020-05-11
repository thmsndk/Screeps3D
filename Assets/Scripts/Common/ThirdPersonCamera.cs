using UnityEngine;
using UnityEngine.EventSystems;

namespace Common
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private float _zoomSpeed = 10;
        [SerializeField] private Transform _followTransform = default;
        private Vector3 _posRef;
        private Quaternion _camRotation;
        private float _zoom;
        private float _xRot = -135;
        private float yRot = 0;
        private Vector3 tether;
        private Vector3 tetherRef;
        private Vector3 height;

        // Use this for initialization
        void Start()
        {
            if (_followTransform)
            {
                Init();
            } else
            {
                enabled = false;
            }
        }

        private void Init()
        {
            _zoom = Vector3.Distance(transform.position, _followTransform.position);
            height = Vector3.up * 1;
            tether = _followTransform.position + height;
            enabled = true;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var tetherGarget = _followTransform.position + height;
            if (Vector3.Distance(tetherGarget, tether) > .1f)
            {
                tether = Vector3.SmoothDamp(tether, tetherGarget, ref tetherRef, .1f);
            }

            _zoom -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;

            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(1))
            {
                _xRot -= Input.GetAxis("Mouse Y") * 5;
                yRot += Input.GetAxis("Mouse X") * 5;
            }

            var camRotation = Quaternion.Euler(_xRot, yRot, 0);
            var target = tether + camRotation * Vector3.forward * _zoom;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _posRef, .2f);
            transform.rotation = Quaternion.LookRotation(tether - transform.position);
        }

        public void FollowTransform(Transform tran)
        {
            _followTransform = tran;
            Init();
        }
    }
}