using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObserver : MonoBehaviour
{
    private const float MAX_RANDOM_DELTA = 360;
    private const float TARGET_DELAY = 2;

    private float _nextTarget;
    private Quaternion[] _rotations;
    private Quaternion _target;

    // Use this for initialization
    void Start()
    {
        int amount = 10;
        _rotations = new Quaternion[amount];
        var initial = transform.rotation.eulerAngles;
        for (var i = 0; i < amount; i++)
        {
            _rotations[i] = Quaternion.Euler(Random.Range(-300.0f, -220.0f), Random.Range(-300.0f, -220.0f), initial.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindNewTarget();
        RotateTowardTarget();
    }

    private void RotateTowardTarget()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, _target, Time.deltaTime);
    }

    private void FindNewTarget()
    {
        if (_nextTarget > Time.time) return;
        _nextTarget = Time.time + TARGET_DELAY;
        _target = _rotations[(int) (_rotations.Length * Random.value)];
    }
}