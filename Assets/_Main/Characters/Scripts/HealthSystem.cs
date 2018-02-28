using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
	public class HealthSystem : MonoBehaviour 
	{
        const string DEATH_TRIGGER = "Death";
        const string HIT_TRIGGER = "Hit";

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthOrbImage;
        [SerializeField] AudioClip[] arrayOfHitClips;
        [SerializeField] AudioClip[] arrayOfDeathClips;
        [SerializeField] float deathVanishSeconds = 3f;

        public float currentHealthPoints;
        float timeLastCharacterHit = 0f;

        Animator myAnimator;
        AudioSource audioSource;
        CharacterMovement characterMovement;


        
        private void Start()
        {
            myAnimator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<CharacterMovement>();

            currentHealthPoints = maxHealthPoints;
        }




        private void Update()
        {
            UpdateHealthBar();
        }


        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }


        public void TakeDamage(float damageAmount)
        {
            bool characterDies = (currentHealthPoints - damageAmount <= 0); // Must ask before Reducing Health
            bool isTimeToGetHit = (Time.time - timeLastCharacterHit) > UnityEngine.Random.Range(2f, 5f); // Always take damage, but only play animation and hit sound after 2f-5f delay

            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damageAmount, 0f, maxHealthPoints);

            if (damageAmount > 0 && isTimeToGetHit) // Stop hit sound when heal - TODO find better solution
            {
                myAnimator.SetTrigger(HIT_TRIGGER);
                PlayRandomHitSound();
                timeLastCharacterHit = Time.time;
            }
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }


        IEnumerator KillCharacter()
        {
            StopAllCoroutines();
            characterMovement.CharacterDeath();
            myAnimator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<Player>();
            if (playerComponent && playerComponent.isActiveAndEnabled) // Relying on lazy evaluation (first evaluation = isPlayerComponent, if false does not look at second evaluation)
            {
                PlayRandomDeathSound();

                yield return new WaitForSecondsRealtime(audioSource.clip.length + 2f);

                SceneManager.LoadScene(0);
            }
            else // Assume is enemy TODO reconsider for other NPCs
            {
                DestroyObject(gameObject, deathVanishSeconds);


            }

            
        }



        void UpdateHealthBar()
        {
            if (healthOrbImage) // Enemies may not have health orb to update
            {
                healthOrbImage.fillAmount = healthAsPercentage;
            }
        }

        public void Heal(float healAmount)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healAmount, 0f, maxHealthPoints);
        }


        /////////////////////////////////////////////////////////////////////

        private void PlayRandomHitSound()
        {
            var clip = arrayOfHitClips[UnityEngine.Random.Range(0, arrayOfHitClips.Length)];
            audioSource.PlayOneShot(clip); // Play on top of other sounds
        }

        private void PlayRandomDeathSound()
        {
            audioSource.clip = arrayOfDeathClips[UnityEngine.Random.Range(0, arrayOfDeathClips.Length)];
            audioSource.Play(); // Stop all other sounds
        }

    }
}
