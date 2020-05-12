using UnityEngine;

namespace Common
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotation;
        [SerializeField] private bool _mutateRandomly = default;
        private Vector3 _mutator;

        // Update is called once per frame
        void Update()
        {
            transform.rotation = transform.rotation * Quaternion.Euler(_rotation * Time.deltaTime);
            if (_mutateRandomly && Random.value < .1)
            {
                _mutator.x += (Random.value * .01f) - .005f;
                _mutator.y += (Random.value * .01f) - .005f;
                _mutator.z += (Random.value * .01f) - .005f;
                _mutator.x = Mathf.Clamp(_mutator.x, -.1f, .1f);
                _mutator.y = Mathf.Clamp(_mutator.y, -.1f, .1f);
                _mutator.z = Mathf.Clamp(_mutator.z, -.1f, .1f);
                _rotation = _rotation + _mutator;
                _rotation.x = Mathf.Clamp(_rotation.x, -100f, 100f);
                _rotation.y = Mathf.Clamp(_rotation.y, -100f, 100f);
                _rotation.z = Mathf.Clamp(_rotation.z, -100f, 100f);
            }
        }
    }
}