﻿using UnityEngine;

namespace Screeps3D {
    internal class CreepView : ObjectView {

        [SerializeField] private ScreepsAPI api;
        
        private Quaternion rotTarget;
        private Vector3 posTarget;
        private Vector3 posRef;
        
        internal override void Init(RoomObject roomObject) {
            base.Init(roomObject);
            GetComponent<Renderer>().material.mainTexture = api.Me.badge;
            rotTarget = transform.rotation;
            posTarget = transform.localPosition;
        }

        internal override void Delta(JSONObject data) {
            base.Delta(data);
            var newPos = new Vector3(RoomObject.X, transform.localPosition.y, 49 - RoomObject.Y);
            var posDelta = posTarget - newPos;
            if (posDelta.sqrMagnitude > .01) {
                posTarget = newPos;
                rotTarget = Quaternion.LookRotation(posDelta);
            }
        }

        private void Update() {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, posTarget, ref posRef, .5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotTarget, Time.deltaTime * 5);
        }
    }
}