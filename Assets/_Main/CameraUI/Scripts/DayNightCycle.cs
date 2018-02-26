using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class DayNightCycle : MonoBehaviour
    {
        float timeScale = 0.001f; // Needs to be 0.001f

        [Tooltip("Number of minutes per second")]
        [SerializeField] GameObject skydome;
        float pos;
        float thisSky = 0f;
        Vector2 vector;
        float time;

        void Start()
        {
            skydome.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(thisSky, 0));
            time = Time.time;
        }


        void Update()
        {
            vector += OffsetValue();
            time = Time.time / 1000; // Neds to be /1000
            skydome.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", vector);
        }

        Vector2 OffsetValue()
        {
            pos = Mathf.Lerp(0, 1, Time.deltaTime * timeScale);
            Vector2 vector = new Vector2(pos, 0);

            if (pos == 1)
            {
                time = 0;
            }

            return vector;
        }
    }
}
