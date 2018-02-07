using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class DayNightCycle : MonoBehaviour
    {

        [Tooltip("Number of minutes per second")]
        [SerializeField]
        float timeScale;

        void Update()
        {

            float angleThisFrame = Time.deltaTime / 360 * timeScale;

            transform.RotateAround(transform.position, Vector3.forward, angleThisFrame);

        }
    }
}
