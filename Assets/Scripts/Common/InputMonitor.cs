using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common
{
    public class InputMonitor : BaseSingleton<InputMonitor>
    {
        public static bool IsDragging { get; private set; }
        public static bool InputFieldActive { get; private set; }
        public static bool OverUI { get; private set; }
        public static bool Shift { get; private set; }
        public static bool Control { get; private set; }
        public static bool Alt { get; private set; }

        private bool _isDragOriginUI;
        private Vector3 _dragOrigin;
        private bool _isMouseDown;
        private int _dragSensitivity = 10;

        private void Update()
        {
            if (!EventSystem.current)
                throw new Exception("InputMonitor expecting an EventSystem in the scene");
            
            IsDragging = CheckDragging(); 
            InputFieldActive = CheckInput();
            OverUI = CheckOverUI();

            MonitorModifiers();
            MonitorClick();
        }

        private void MonitorModifiers()
        {
            Shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            Control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            Alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }

        private bool CheckDragging()
        {
            return _isMouseDown && Vector3.Distance(_dragOrigin, Input.mousePosition) > _dragSensitivity;
        }

        private bool CheckInput()
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                var input = EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
                if (input && input.isFocused)
                    return true;
            }
            return false;
        }

        private bool CheckOverUI()
        {
            if (IsDragging)
            {
                return _isDragOriginUI;
            }

            return EventSystem.current.IsPointerOverGameObject();
        }

        private void MonitorClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _dragOrigin = Input.mousePosition;
                _isMouseDown = true;
                _isDragOriginUI = EventSystem.current.IsPointerOverGameObject();
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isMouseDown = false;
            }
        }
    }
}