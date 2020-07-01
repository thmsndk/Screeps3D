using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class CreepBodyView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer _rend = default;
        private Creep _creep;
        private Texture2D _texture;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            if (_texture == null)
            {
                InitTexture();
            }
            _creep = roomObject as Creep;
            UpdateView();
        }

        private void InitTexture()
        {
            _texture = new Texture2D(50, 1);
            _texture.filterMode = FilterMode.Point;
            //_rend.material.mainTexture = _texture;
            //_rend.material.SetTexture("_MainTex", _texture);
            // _rend.material.SetTexture("_BaseColorMap", _texture);
            _rend.material.SetTexture("BodyMap", _texture);
            
        }

        public void Delta(JSONObject data)
        {
            var bodyObj = data["body"];
            if (bodyObj == null)
                return;
            UpdateView();
        }

        public void Unload(RoomObject roomObject)
        {
        }

        private void UpdateView()
        {
            var frontIndex = 0;
            for (var i = 0; i < PartCount("ranged_attack"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.RangedAttack);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("attack"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Attack);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("heal"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Heal);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("work"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Work);
                frontIndex++;
            }
            for (var i = 0; i < PartCount("claim"); i++)
            {
                _texture.SetPixel(frontIndex, 0, Constants.CreepBodyPartColors.Claim);
                frontIndex++;
            }

            var backIndex = 0;
            for (; backIndex < PartCount("move"); backIndex++)
            {
                _texture.SetPixel(49 - backIndex, 0, Constants.CreepBodyPartColors.Move);
            }

            var toughAlpha = Mathf.Min(PartCount("tough") / 10f, 1);
            var toughColor = new Color(1, 1, 1, toughAlpha);
            for (; frontIndex < 49 - (backIndex - 1); frontIndex++)
            {
                _texture.SetPixel(frontIndex, 0, toughColor);
            }
            _texture.Apply();
        }

        private int PartCount(string type)
        {
            var count = 0;
            foreach (var part in _creep.Body.Parts)
            {
                if (part.Type == type && part.Hits > 0)
                {
                    count++;
                }
            }
            return count;
        }
    }
}