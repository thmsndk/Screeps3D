using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    internal class CreepView : ObjectView
    {
        [SerializeField] private Renderer _badge = default;
        [SerializeField] private Renderer _creepCore = default;
        [SerializeField] private Renderer _doritoCore = default;
        [SerializeField] private Transform _rotationRoot = default;
        [SerializeField] private Light _underLight = default;

        [SerializeField] private Renderer _wingLeft;
        [SerializeField] private Renderer _wingRight;
        [SerializeField] private Renderer _horse;

        private Quaternion _rotTarget;
        private Vector3 _posTarget;
        private Vector3 _posRef;
        private Creep _creep;
        private bool _dead;

        private Color32 _initialUnderlightColor;

        private void Awake()
        {
            _initialUnderlightColor = _underLight.color;
        }

        private void setWings(bool setWings) {
            float v = setWings ? 0.2f : 15f;
                _wingRight.material.SetFloat("Magic", v);
                _wingLeft.material.SetFloat("Magic", v);
        }
        private void setHorse(bool setHorse) {
            float v = setHorse ? 0.2f : 15f;
                _horse.material.SetFloat("Magic", v);
        }

        private void setDorito(bool isDorito) {
            if(isDorito) {
                _doritoCore.gameObject.SetActive(true);
                _creepCore.gameObject.SetActive(false);
                _underLight.color = Color.red;
            } else {
                _creepCore.gameObject.SetActive(true);
                _doritoCore.gameObject.SetActive(false);
                _underLight.color = _initialUnderlightColor;
            }
        }

        internal override void Load(RoomObject roomObject)
        {            
            base.Load(roomObject);
            _creep = roomObject as Creep;

            if (_creep != null)
            {
                // Allows you to find the specific creep by search
                this.name = $"Creep:{_creep.Name}";
            }

            if (_creep?.Owner?.Badge == null) {
                Debug.LogError("A creep with no owner?");
            } else {
                _badge.materials[0].SetTexture("EmissionTexture", _creep?.Owner?.Badge);
                _badge.materials[0].SetFloat("EmissionStrength", .1f);
            }
            
            setDorito(_creep.Owner.UserId == Constants.InvaderUserId
                      || _creep.Owner.UserId == Constants.SourceKeeperUserId);

            // HORSE
            // do not forget to do reposition in .blend files ! 
            // to uncomment:
            setWings(false);
            setHorse(false);
            // to comment:
            // if (_creep.Owner.Username == "Tigga" || _creep.Owner.Username == "Geir1983") {
            //     setWings(true);
            //     setHorse(false);
            // } else {
            //     setWings(false);
            //     setHorse(true);
            // }

            _rotTarget = transform.rotation;
            _posTarget = roomObject.Position;

            ScaleCreepSize();
        }

        private void ScaleCreepSize()
        {
            var percentage = _creep.Body.Parts.Count / 50f;
            if(percentage == 0) {
                _dead = true;
            }

            var minVisibility = 0.001f; /*to keep it visible and selectable*/
            var maxVisibility = 1f;

            // http://james-ramsden.com/map-a-value-from-one-number-scale-to-another-formula-and-c-code/
            float minimum = Mathf.Log(minVisibility);
            float maximum = Mathf.Log(maxVisibility);

            // Scale the visibility in such a way that a lot of the model is rendered above 25 body parts
            float current = Mathf.Log(percentage == 0 ? minVisibility : percentage);

            // Map range to visibility range
            var visibility = minVisibility + (maxVisibility - minVisibility) * ((current - minimum) / (maximum - minimum));

            _vis.SetVisibility(visibility, true);
        }

        internal override void Delta(JSONObject data)
        {
            base.Delta(data);
            _underLight.intensity = _creep.Hits / _creep.HitsMax  * 0.1f;

            var posDelta = _posTarget - RoomObject.Position;

            if (posDelta.sqrMagnitude > .01)
            {
                _posTarget = RoomObject.Position;
            }

            ScaleCreepSize();
        }

        private void Update()
        {            
            if (_creep == null)
                return;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _posTarget, ref _posRef, .5f);

            if(_creep.ActionTarget.HasValue) {               
                // creep does something, keep it rotated towards target
                Vector3 relativePos = _rotationRoot.position - (Vector3)_creep.ActionTarget;
                if (relativePos != Vector3.zero) {
                    Quaternion tRotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    _rotationRoot.rotation = tRotation;
                }
            } else {
                // keep rotation towards move direction
                _rotationRoot.transform.rotation = Quaternion.Slerp(_rotationRoot.transform.rotation, _creep.Rotation, 
                    Time.deltaTime * 5);
            }

        }
    }
}