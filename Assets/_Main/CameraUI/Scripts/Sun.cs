using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
	public class Sun : MonoBehaviour 
	{
        float timeScale;

        private void Start()
        {
            timeScale = FindObjectOfType<DayNightCycle>().GetTimeScale() * 360;
        }

        void Update ()
        {
            float angleThisFrame = Time.deltaTime * timeScale;

            transform.RotateAround(transform.position, Vector3.forward, angleThisFrame);

        }

	}
}
