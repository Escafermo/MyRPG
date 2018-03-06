using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
using System;

namespace RPG.Characters    
{
	public class SpecialAbilities : MonoBehaviour 
	{
        AudioSource audioSource;

        [SerializeField] SpecialAbilityConfig[] abilities;
        [SerializeField] AudioClip[] outOfEnergySounds;

        [SerializeField] Image energyOrb;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float energyRecoverPerSecond = 1f;
        // TODO out of energy sound

        float currrentEnergy;

        void Start ()
        {
            audioSource = GetComponent<AudioSource>();

            currrentEnergy = maxEnergyPoints;
            UpdateEnergyBar();

            AttachInitialAbilities();
        }

        void Update()
        {
            if (currrentEnergy < maxEnergyPoints)
            {
                RecoverEnergy();
                UpdateEnergyBar();
            }
        }

        // Because we provided default value "null", it becomes OPTIONAL to have a target:
        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var specialAbilities = GetComponent<SpecialAbilities>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyCost <= currrentEnergy) // TODO read from specialized scriptable object
            {
                ConsumeEnergy(energyCost);
                //var abilityParameters = new AbilityUseParameters(target);
                //abilities[abilityIndex].Use(abilityParameters);

                abilities[abilityIndex].Use(target);
            }
            else
            {
                PlayRandomNoEnergySound();
            }
        }


        public float energyAsPercentage // Getter
        {
            get
            {
                return currrentEnergy / (float)maxEnergyPoints;
            }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        public void RecoverEnergy()
        {
            var pointsToAdd = energyRecoverPerSecond * Time.deltaTime;
            currrentEnergy = Mathf.Clamp(currrentEnergy + pointsToAdd, 0, maxEnergyPoints);
        }

        //public bool IsEnergyAvailable(float amount)
        //{
        //    return amount <= currentEnergyPoints;
        //}

        public void ConsumeEnergy (float amount)
        {
            float newEnergyPoints = currrentEnergy - amount;
            currrentEnergy = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);

            UpdateEnergyBar();
        }

        void PlayRandomNoEnergySound()
        {
            AudioClip audioClip = outOfEnergySounds[UnityEngine.Random.Range(0, outOfEnergySounds.Length)];
            audioSource.PlayOneShot(audioClip);
        }


        void AttachInitialAbilities()
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                abilities[i].AttachAbilityTo(gameObject);
            }
        }

        // TODO remove magic numbers below
        void UpdateEnergyBar()
        {
            if (energyOrb)
            {
                energyOrb.fillAmount = energyAsPercentage;
            }
        }

    }
}
