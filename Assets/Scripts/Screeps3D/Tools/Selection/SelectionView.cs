using System.Collections.Generic;
using Screeps3D.RoomObjects;
using Screeps3D.RoomObjects.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Screeps3D.Tools.Selection
{
    [DisallowMultipleComponent]
    internal class SelectionView : MonoBehaviour
    {
        private static readonly Dictionary<string, float> CircleSizes = new Dictionary<string, float>
        {
            // Prefab default 1.5
            {"extension", 1.0f}
        };

        private string _type;
        private GameObject _circle;
        private GameObject _label;
        private Stack<GameObject> _labelPool = new Stack<GameObject>();
        private Stack<GameObject> _circlePool = new Stack<GameObject>();
        private DecalProjector _decalProjector;

        public ObjectView Selected { get; private set; }

        private void Start()
        {
            Selected = gameObject.GetComponent<ObjectView>();
            _type = Selected.RoomObject.Type;
            _circle = CreateCircle();
            _label = CreateLabel();
        }

        public void Dispose()
        {
            if (_circle != null)
            {
                _circle.SetActive(false);
                _circlePool.Push(_circle);
            }
            ;
            if (_label != null)
            {
                _label.SetActive(false);
                _labelPool.Push(_label);
            }
            Destroy(this);
        }

        private GameObject CreateLabel()
        {
            var nameObj = Selected.RoomObject as INamedObject;
            if (nameObj == null)
                return null; // Early

            GameObject label;
            if (_labelPool.Count > 0)
            {
                label = _labelPool.Pop();
                label.SetActive(true);
            } else
            {
                label = Instantiate(Tools.Selection.Selection.LabelTemplate);
            }
            label.transform.SetParent(Selected.gameObject.transform);
            label.transform.localPosition = new Vector3(0, label.gameObject.transform.lossyScale.y + 1, 0);
            var textMesh = label.GetComponent<TextMeshPro>();
            textMesh.text = nameObj.Name;
            textMesh.enabled = true;
            return label;
        }

        private void BillboardLabel()
        {
            _label.transform.rotation = Camera.main.transform.rotation;
        }

        private void Update()
        {
            if (_label != null) BillboardLabel();
            if (_circle != null) FadeInCircle();
        }

        private void FadeInCircle()
        {
            var color = _decalProjector.material.color;
            if (color.a >= 1)
            {
                return;
            }
            color.a += Time.deltaTime / .2f;
            _decalProjector.material.color = color;
        }

        private GameObject CreateCircle()
        {
            GameObject go;
            if (_circlePool.Count > 0)
            {
                go = _circlePool.Pop();
                go.SetActive(true);
            } 
            else
            {
                go = Instantiate(Tools.Selection.Selection.CircleTemplate);
            }
            _decalProjector = go.GetComponent<DecalProjector>();
            var color = _decalProjector.material.color;
            color.a = 0;
            _decalProjector.material.color = color;
            go.transform.SetParent(Selected.gameObject.transform, false);
            if (CircleSizes.ContainsKey(_type))
            {
                _decalProjector.size = new Vector3(CircleSizes[_type], CircleSizes[_type], _decalProjector.size.z);
            }
            return go;
        }
    }
}