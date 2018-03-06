using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
	public class Sun : MonoBehaviour 
	{
		void Update ()
        {
            float timeScale = FindObjectOfType<DayNightCycle>().GetTimeScale() * 360;

            float angleThisFrame = Time.deltaTime * timeScale;

            transform.RotateAround(transform.position, Vector3.forward, angleThisFrame);

        }
	}
}
