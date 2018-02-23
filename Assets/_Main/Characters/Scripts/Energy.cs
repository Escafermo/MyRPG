using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour 
	{
        [SerializeField] Image energyImage;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float energyRecoverPerSecond = 1f;

        float currentEnergyPoints;

        void Start ()
        {
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBar();
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
            energyImage.fillAmount = energyAsPercentage;
        }

        public float energyAsPercentage // Getter
        {
            get
            {
                return currentEnergyPoints / (float)maxEnergyPoints;
            }
        }


    }
}
