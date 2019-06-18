using Screeps3D.Menus.Options;
using System;
using UnityEngine;

namespace Common
{
    public class CameraRig : BaseSingleton<CameraRig>
    {
        [SerializeField] private Transform _boom;
        [SerializeField] private Transform _pivot;
        [SerializeField] private int _rigLayer;
        [SerializeField] private float _defaultZoom;
        [SerializeField] private float _defaultAngle;
        [SerializeField] private float _zoomSpeed = 5;
        [SerializeField] private float _minZoom = 1;
        [SerializeField] private float _maxZoom = 400;

        public Action OnTargetReached;

        public static Vector3 Rotation
        {
            get { return Instance._targetRotation; }
            set { Instance._targetRotation = value; }
        }

        public static Vector3 Position
        {
            get { return Instance._targetPosition; }
            set { Instance._targetPosition = value; }
        }

        public static float Zoom
        {
            get { return Instance._targetZoom; }
            set { Instance._targetZoom = value; }
        }
        
        public static bool ClickToPan { get; set; }
        
        private float _targetZoom;
        private Vector3 _targetRotation;
        private Vector3 _targetPosition;
        private float _zoomRef;
        private Vector3 _posRef;
        private float _keyboardSpeed = 5;
        private Vector3 _clickPos;

        private void Start()
        {
            _targetZoom = _defaultZoom;
            _targetRotation = Vector3.right * _defaultAngle;
        }

        private void Update()
        {
            MouseControl();
            KeyboardControl();
            
            UpdateZoom();
            UpdateRotation();
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (transform.position != _targetPosition)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _posRef, .1f);

                // TODO - should position be checked with some delta?
                if (transform.position == _targetPosition)
                {
                    if (OnTargetReached != null) OnTargetReached.Invoke();
                }
            }
            
        }

        private void UpdateZoom()
        {
            var pos = _boom.localPosition;
            pos.z = Mathf.SmoothDamp(pos.z, -_targetZoom, ref _zoomRef, 0.2f);
            _boom.localPosition = pos;
        }

        private void UpdateRotation()
        {
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, 0, 90);
            var target = Quaternion.Euler(_targetRotation.x, _targetRotation.y, 0);
            // transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime);
            _pivot.rotation = target;
        }

        private void KeyboardControl()
        {
            if (InputMonitor.InputFieldActive)
                return;

            KeyboardPosition();
            KeyboardRotation();
        }

        private void KeyboardPosition()
        {
            var cameraRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            var heightFactor = Mathf.Log10(-_boom.localPosition.z + 1);
            _targetPosition += cameraRotation * input * _keyboardSpeed * heightFactor * 10 * Time.deltaTime;
        }

        private void KeyboardRotation()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                _targetRotation.y += 25 * _keyboardSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.E))
            {
                _targetRotation.y -= 25 * _keyboardSpeed * Time.deltaTime;
            }
        }

        private void MouseControl()
        {
            MouseRotation();
            
            if (InputMonitor.OverUI)
                return;
            
            MouseZoom();
            if (ClickToPan)
                MousePosition();
        }

        private void MousePosition()
        {
            if (!Input.GetMouseButton(0))
            {
                _clickPos = Vector3.zero;
                return;
            }
                
            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics.Raycast(ray, out hitInfo, 1000, 1 << _rigLayer);
            if (hit)
            {
                var localPoint = hitInfo.point - transform.position;
                if (_clickPos == Vector3.zero)
                    _clickPos = localPoint;

                if (InputMonitor.IsDragging)
                {
                    var delta = _clickPos - localPoint;
                    _clickPos = localPoint;
                    _targetPosition += delta;
                }
            }
        }

        private void MouseZoom()
        {
            _targetZoom -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed * _targetZoom / 2;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        private void MouseRotation()
        {
            if (!Input.GetMouseButton(1))
                return;

            _targetRotation.y += Input.GetAxis("Mouse X") * 5;
            _targetRotation.x -= Input.GetAxis("Mouse Y") * 5;
        }
    }
}