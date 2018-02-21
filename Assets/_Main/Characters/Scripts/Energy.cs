using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour 
	{
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float energyRecoverPerSecond = 1f;

        float currentEnergyPoints;

        void Start ()
        {
            currentEnergyPoints = maxEnergyPoints;
        }

        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                RecoverEnergy();
                UpdateEnergyBar();
            }
        }

        public void RecoverEnergy()
        {
            var pointsToAdd = energyRecoverPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy (float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);

            UpdateEnergyBar();
        }
        
        // TODO remove magic numbers below
        private void UpdateEnergyBar()
        {
            float xValue = -(energyAsPercentage / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        public float energyAsPercentage
        {
            get
            {
                return currentEnergyPoints / (float)maxEnergyPoints;
            }
        }


    }
}
