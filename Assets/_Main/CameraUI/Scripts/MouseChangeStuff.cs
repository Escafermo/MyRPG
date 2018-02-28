using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class MouseChangeStuff : MonoBehaviour
    {
        // Handles ZOOM and PAN

        public Transform target;
        public float distance = 2.0f;
        public float xSpeed = 20.0f;
        public float ySpeed = 20.0f;
        public float yMinLimit = -90f;
        public float yMaxLimit = 90f;
        public float distanceMin = 10f;
        public float distanceMax = 10f;
        public float smoothTime = 2f;
        float rotationYAxis = 0.0f;
        float rotationXAxis = 0.0f;
        float velocityX = 0.0f;
        float velocityY = 0.0f;

        float zoomValue = 0f;
        float zoomVelocity = 0f;
        float small = 0.5f;
        float minCameraDistY = 4f;
        float maxCameraDistY = 10f;
        float minCameraDistZ = 5.5f;
        float maxCameraDistZ = 11.5f;
        private Vector3 localCameraPosition;

        void Start()
        {
            localCameraPosition = new Vector3(0, 4f, 5.5f);
          

            Vector3 angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;
 
            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().freezeRotation = true;
            }
        }

        void LateUpdate()
        {
            if (target)
            {
                MoveCameraAroundPlayer();
            }

            HandleZoom();

        }

        private void MoveCameraAroundPlayer()
        {
            if (Input.GetMouseButton(1))
            {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * distance * 0.02f;
            }

            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);

            Quaternion toRotation = Quaternion.Euler(-rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            transform.rotation = rotation;

            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

            transform.position = target.transform.position;

            //transform.rotation = Quaternion.Lerp(rotation, target.transform.rotation, Time.deltaTime * 10);

            //rotation = Quaternion.Lerp(rotation, target.transform.rotation, (1f * Time.deltaTime));
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        private void HandleZoom()
        {
            transform.GetComponentInChildren<Camera>().transform.localPosition = localCameraPosition;

            if (Input.GetAxis("Mouse ScrollWheel") != 0 && IsLocalPositionOk())
            {
                zoomVelocity += Input.GetAxis("Mouse ScrollWheel") * small;
            }
            else if (IsLocalPositionToZeroZoomValue()) // If passed limits
            {
                zoomValue = 0f;
            }
            zoomValue += zoomVelocity;
            localCameraPosition -= new Vector3(0, zoomValue, zoomValue);
            ClampAndLerpZoom();
        }
        
        private void ClampAndLerpZoom()
        {
            localCameraPosition.y = Mathf.Clamp(localCameraPosition.y, minCameraDistY, maxCameraDistY);
            localCameraPosition.z = Mathf.Clamp(localCameraPosition.z, minCameraDistZ, maxCameraDistZ);
            zoomValue = Mathf.Lerp(zoomValue, 0, Time.deltaTime * 2f);
            zoomVelocity = Mathf.Lerp(zoomVelocity, 0, Time.deltaTime * 500f);
        }

        private bool IsLocalPositionOk()
        {
            return localCameraPosition.y >= minCameraDistY && localCameraPosition.y <= maxCameraDistY && localCameraPosition.z >= minCameraDistZ && localCameraPosition.z <= maxCameraDistZ;
        }

        private bool IsLocalPositionToZeroZoomValue()
        {
            return localCameraPosition.y <= minCameraDistY || localCameraPosition.y >= maxCameraDistY || localCameraPosition.z <= minCameraDistZ || localCameraPosition.z >= maxCameraDistZ;
        }

    }
}