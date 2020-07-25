using Assets.Scripts.Common;
using Assets.Scripts.Common.SettingsManagement;
using Screeps3D.Menus.Options;
using System;
using UnityEngine;

namespace Common
{
    public class IdleCamera : BaseSingleton<IdleCamera>
    {
        [SerializeField] private CameraRig _cameraRig = default;
        private Vector3 lastRotation;
        private float lastZoom;
        private bool wasIdle;        
        private float secondsToBeIdle;

        [Setting("Gameplay/Camera", "Enable CameraIdle", "Enable / Disable camera idle")]
        private static bool _enableCameraIdle = true;

        [Setting("Gameplay/Camera", "Minimum Minutes", "")]
        private static float _minimumIdleMinutes = 1f;
        [Setting("Gameplay/Camera", "Maximum Minutes", "")]
        private static float _maximumIdleMinutes = 5f;

        private void Start()
        {
            lastRotation = CameraRig.Rotation;
            lastZoom = CameraRig.Zoom;
            
            // Check if we're allowing the idle system
            _enableCameraIdle = !CmdArgs.DisableCameraIdle;
            Debug.Log($"Camera Idle: {_enableCameraIdle}");

            if (_enableCameraIdle)
            {
                RandomizeIdleTime();
            }
        }

        private void Update()
        {
            if (_enableCameraIdle)
            {
                var idle = Time.time - InputMonitor.LastAction > secondsToBeIdle;

                if (!idle)
                {

                    if (wasIdle)
                    {
                        RestoreRotationAndZoom();

                        wasIdle = false;

                        RandomizeIdleTime();
                    }

                    // Update when not idle
                    CacheRotationZoom();

                    return;
                }

                if (!wasIdle)
                {
                    wasIdle = true;

                    // Update lastRotation when going idle
                    CacheRotationZoom();

                    // Rotate to a 45 degree angle
                    // Well I have no idea how to reset to "topdown" and then rotate it 45 degrees.
                    //CameraRig.PivotRotation = initialRotation.Value;
                    _cameraRig.SetTargetRotation(new Vector2(0, 35)); // This rotates the view based on where the camera is.... so you can end up in the bottom of the map

                    _cameraRig.SetTargetZoom(30);
                }

                // should we enable / disable the script  based on idle?

                float speed = 1;

                _cameraRig.SetTargetRotation(new Vector2(5 * speed * Time.deltaTime, 0));
            }
        }

        private void RandomizeIdleTime()
        {
            secondsToBeIdle = UnityEngine.Random.Range(
                            60f * _minimumIdleMinutes /* 1 Minutes */,
                            60f * _maximumIdleMinutes /* 5 Minutes */);
        }

        private void RestoreRotationAndZoom()
        {
            if (lastRotation != CameraRig.Rotation)
            {
                CameraRig.Rotation = lastRotation;
            }

            if (lastZoom != CameraRig.Zoom)
            {
                CameraRig.Zoom = lastZoom;
            }
        }

        private void CacheRotationZoom()
        {
            lastRotation = CameraRig.Rotation;
            lastZoom = CameraRig.Zoom;
        }
    }
}