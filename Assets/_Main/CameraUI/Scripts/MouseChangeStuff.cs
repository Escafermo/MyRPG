using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class MouseChangeStuff : MonoBehaviour
    {

        [SerializeField] Transform target;
        Vector3 localCameraPosition;

        // Pan
        [SerializeField] float yMinLimit = 20f;
        [SerializeField] float yMaxLimit = 40f;
        [SerializeField] float xMinLimit = -5f;
        [SerializeField] float xMaxLimit = 5f;

        float distance = 2.0f;
        float xSpeed = 10.0f;
        float ySpeed = 10.0f;
        float distanceMin = 10f;
        float distanceMax = 10f;
        float smoothTime = 3f;
        float rotationYAxis = 0.0f;
        float rotationXAxis = 0.0f;
        float panVelocityX = 0.0f;
        float panVelocityY = 0.0f;

        //Zoom
        [SerializeField] Vector3 startingPosition = new Vector3(0f, 9f, 10f);
        [SerializeField] float minCameraDistY = 3f;
        [SerializeField] float maxCameraDistY = 9f;
        [SerializeField] float minCameraDistZ = 4f;
        [SerializeField] float maxCameraDistZ = 10f;

        float zoomValue = 0f;
        float zoomVelocity = 0f;
        float zoomCap = 0.5f;

        void Start()
        {
            localCameraPosition = startingPosition;
          

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
                panVelocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                panVelocityY += ySpeed * Input.GetAxis("Mouse Y") * distance * 0.02f;
            }

            rotationYAxis += panVelocityX;
            rotationXAxis -= panVelocityY;
            rotationXAxis = ClampAngle(rotationXAxis, xMinLimit, xMaxLimit);
            rotationYAxis = ClampAngle(rotationYAxis, yMinLimit, yMaxLimit);

            Quaternion toRotation = Quaternion.Euler(-rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            transform.rotation = rotation;

            panVelocityX = Mathf.Lerp(panVelocityX, 0, Time.deltaTime * smoothTime);
            panVelocityY = Mathf.Lerp(panVelocityY, 0, Time.deltaTime * smoothTime);

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
                zoomVelocity += Input.GetAxis("Mouse ScrollWheel") * zoomCap;
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