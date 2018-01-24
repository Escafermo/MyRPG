using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChangeStuff : MonoBehaviour
{
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
    private float currentViewValue = 2f;
    private Vector3 localCameraPosition;

    [SerializeField] float yZoomInMax;
    [SerializeField] float yZoomOutMax;
    [SerializeField] float translationValue;

    // Use this for initialization
    void Start()
    {
        localCameraPosition = new Vector3(0, 5f, 6f);

        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    void LateUpdate()
    {
        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(-rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            //distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
            //RaycastHit hit;
            //if (Physics.Linecast(target.position, transform.position, out hit))
            //{
            //distance -= hit.distance;
            //}
            //Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            //Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            //transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

            transform.position = target.transform.position;
        }

        // Handles zooming:
        Vector3 zoomOut = new Vector3(0, translationValue, translationValue);
        Vector3 zoomIn = new Vector3(0, -translationValue, -translationValue);
        transform.GetComponentInChildren<Camera>().transform.localPosition = localCameraPosition;

        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0  && localCameraPosition.y < yZoomOutMax)
        {
            localCameraPosition += zoomOut;
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && localCameraPosition.y > yZoomInMax)
        {
            localCameraPosition += zoomIn;
        }

    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
