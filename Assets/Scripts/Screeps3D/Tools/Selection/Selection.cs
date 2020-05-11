using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Screeps3D.Menus.Options;
using Screeps3D.RoomObjects;
using Screeps3D.RoomObjects.Views;
using UnityEngine;

namespace Screeps3D.Tools.Selection
{
    public class Selection : BaseSingleton<Selection>
    {
        [SerializeField] private GameObject _circlePrefab = default;
        [SerializeField] private GameObject _labelPrefab = default;

        public static GameObject CircleTemplate;
        public static GameObject LabelTemplate;

        private Vector3 _startPosition;
        private bool _isSelecting;
        private readonly Dictionary<string, RoomObject> _selections = new Dictionary<string, RoomObject>();
        
        public Dictionary<string, RoomObject> Selections { get { return _selections; }}

        public Action<RoomObject> OnSelect;
        public Action<RoomObject> OnDeselect;

        private void Start()
        {
            CircleTemplate = _circlePrefab;
            LabelTemplate = _labelPrefab;
        }

        private void Update()
        {
            var dragging = IsDragging();

            ObjectView rayTarget = null;
            if (!dragging && !InputMonitor.OverUI)
            {
                rayTarget = Rayprobe();
                SelectionOutline.DrawOutline(rayTarget);
            }

            if (Input.GetMouseButtonDown(0) && !InputMonitor.OverUI)
            {
                _isSelecting = true;
                _startPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isSelecting = false;
                
                if (!InputMonitor.Control && !dragging && !InputMonitor.OverUI)
                    DeselectAll();
                
                if (dragging && MultiSelect.IsOn)
                {
                    SelectBoxedObjects();
                } 
                else if (!InputMonitor.OverUI && rayTarget)
                {
                    ToggleSelection(rayTarget.RoomObject);
                }
            }
        }

        private void OnGUI()
        {
            if (!IsDragging() || !MultiSelect.IsOn) return; // Early
            SelectionBox.DrawSelectionBox(_startPosition, Input.mousePosition);
        }

        private bool IsDragging()
        {
            return _isSelecting && (Input.mousePosition - _startPosition).magnitude > 5;
        }

        private ObjectView Rayprobe()
        {
            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics.Raycast(ray, out hitInfo);
            if (!hit) return null; // Early

            return hitInfo.transform.gameObject.GetComponent<ObjectView>();
        }

        public void ToggleSelection(RoomObject obj)
        {
            if (obj == null)
            {
                Debug.LogError("RoomObject is null the objectview is missing a reference to RoomObject, perhaps Load() has not yet been run?");
                return;
            }

            if (_selections.ContainsKey(obj.Id))
            {
                DeselectObject(obj);
            } 
            else
            {
                SelectObject(obj);
            }
        }

        public void DeselectObject(RoomObject obj)
        {
            if (!_selections.ContainsKey(obj.Id)) return; // Early

            if (obj.View)
            {
                var selectionView = obj.View.GetComponent<SelectionView>();
                if (selectionView)
                {
                    selectionView.Dispose();
                }
            }

            _selections.Remove(obj.Id);
            if (OnDeselect != null)
                OnDeselect(obj);
        }

        private void DeselectAll()
        {
            _selections.Values.ToList().ForEach(obj => DeselectObject(obj));
        }

        public void SelectObject(RoomObject obj)
        {
            if (!_selections.ContainsKey(obj.Id))
            {
                _selections.Add(obj.Id, obj);
                if (obj.View)
                {
                    obj.View.gameObject.AddComponent<SelectionView>();
                }

                obj.OnShow += Obj_OnShow;

                OnSelect?.Invoke(obj);
            }
        }

        private void Obj_OnShow(RoomObject roomObject, bool show)
        {
            if (!show)
            {
                DeselectObject(roomObject);
            }
        }

        private void SelectBoxedObjects()
        {
            foreach (var kvp in ObjectManager.Instance.RoomObjects)
            {
                if (kvp.Value.View == null)
                    continue;
                var withinBounds = SelectionBox.IsWithinSelectionBox(kvp.Value.View);
                if (withinBounds)
                    SelectObject(kvp.Value);
            }
        }
    }
}