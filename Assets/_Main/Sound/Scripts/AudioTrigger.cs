using UnityEngine;

namespace RPG.Core
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] AudioClip clip;
        //[SerializeField] int layerFilter;
        [SerializeField] float playerDistanceTrigger = 2f;
        [SerializeField] bool isOneTimeOnly = true;

        bool hasPlayed = false;
        AudioSource audioSource;
        GameObject player;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;

            player = GameObject.FindGameObjectWithTag("Player");
            //SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            //sphereCollider.isTrigger = true;
            //sphereCollider.radius = triggerRadius;

            //gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= playerDistanceTrigger)
            {
                RequestPlayAudioClip();
            }
        }
        //void OnTriggerEnter(Collider other)
        //{
        //    if (other.gameObject.layer == layerFilter)
        //    {
        //        RequestPlayAudioClip();
        //    }
        //}

        void RequestPlayAudioClip()
        {
            if (isOneTimeOnly && hasPlayed)
            {
                return;
            }
            else
            {
                audioSource.clip = clip;
                audioSource.PlayOneShot(clip);
                hasPlayed = true;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 255f, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, playerDistanceTrigger);
        }
    }
}
