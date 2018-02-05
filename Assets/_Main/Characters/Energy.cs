using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour 
	{
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergytPoints = 100f;
        [SerializeField] float energyPerHit;

        float currentEnergyPoints;

        CameraRaycaster cameraRaycaster = null;

        void Start ()
        {
            currentEnergyPoints = maxEnergytPoints;
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyEnergySpent += OnMiddleMouseClick;
        }

        void OnMiddleMouseClick(RaycastHit raycastHit, int layerHit)
        {
            float newEnergyPoints = currentEnergyPoints - energyPerHit;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergytPoints);
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            float xValue = -(energyAsPercentage / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        public float energyAsPercentage
        {
            get
            {
                return currentEnergyPoints / (float)maxEnergytPoints;
            }
        }


    }
}
