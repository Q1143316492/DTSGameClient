using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Util;

namespace CWLEngine.GameImpl.Controller
{
    public class TPSCameraController : MonoBehaviour
    {
        private GameObject target = null;
        private Transform baseTarget;

        public float distance = 5.0f;

        public float angleYLimitMax = 90;
        public float angleYLimitMin = -40;

        public float angleYSpeed = 5.0f;    // angleYOffset 的变换速度

        private float angleYOffset = 0.0f;  // 相机看向角色目标点的视角 与 水平面的角度

        private Vector3 cameraOffset = Vector3.zero;

        private bool isInit = false;
        
        void Start()
        {
            MemeryCacheMgr.Instance.Set("TPSCameraControllerRadius", distance);
        }

        public void Init()
        {
            LockPlayer("PlayerA");
        }

        public void LockPlayer(string name = "PlayerA")
        {
            target = GameObject.Find(name);
            baseTarget = Tools.FindChildrenTransform(target, "camera_lookat");

            if (target != null && baseTarget != null)
            {
                isInit = true;
            }
        }

        void LateUpdate()
        {
            if (!isInit || OperationMode.Instance.IsLock())
            {
                return;
            }

            baseTarget = Tools.FindChildrenTransform(target, "camera_lookat");
            gameObject.transform.position = baseTarget.position + cameraOffset;
            gameObject.transform.LookAt(baseTarget.position);
        }

        public void MoveUp(float distance)
        {
            angleYOffset -= distance;
            if (angleYOffset > angleYLimitMax) angleYOffset = angleYLimitMax;
            if (angleYOffset < angleYLimitMin) angleYOffset = angleYLimitMin;
        }

        public void MoveDown(float distance)
        {
            angleYOffset += distance;
            if (angleYOffset > angleYLimitMax) angleYOffset = angleYLimitMax;
            if (angleYOffset < angleYLimitMin) angleYOffset = angleYLimitMin;
        }

        private void MouseXYMove()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            angleYOffset -= mouseY * angleYSpeed;
            if (angleYOffset > angleYLimitMax) angleYOffset = angleYLimitMax;
            if (angleYOffset < angleYLimitMin) angleYOffset = angleYLimitMin;
        }

        public void FixedUpdate()
        {
            if (!isInit || OperationMode.Instance.IsLock())
            {
                return;
            }
        }

        void Update()
        {
            if (!isInit || OperationMode.Instance.IsLock())
            {
                return;
            }
            
            float radius = distance * Mathf.Cos(angleYOffset * Mathf.PI / 180.0f);
        
            cameraOffset = -target.transform.forward * radius;
            cameraOffset.y = distance * Mathf.Sin(angleYOffset * Mathf.PI / 180.0f);
            cameraOffset += target.transform.right * 1;  // 这个偏移是为了相机在角色右后方看向目标

            MemeryCacheMgr.Instance.Set("TPSCameraControllerRadius", radius);
            //cameraOffset = Matrix4x4.Rotate(Quaternion.Euler(0, Mathf.Atan2(2, radius) * 180 / Mathf.PI, 0)).MultiplyPoint(cameraOffset);

            MouseXYMove();
        }
    }
}

