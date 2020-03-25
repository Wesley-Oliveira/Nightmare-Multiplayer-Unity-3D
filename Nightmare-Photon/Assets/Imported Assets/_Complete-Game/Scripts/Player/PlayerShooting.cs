using UnityEngine;
using System.Collections;
using UnitySampleAssets.CrossPlatformInput;
using Photon.Pun; // ## Multiplayer

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.


        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
		public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

        PhotonView photonView;// ## Multiplayer

        void Awake ()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem> ();
            gunLine = GetComponent <LineRenderer> ();
            gunAudio = GetComponent<AudioSource> ();
            gunLight = GetComponent<Light> ();
            //faceLight = GetComponentInChildren<Light> ();

            photonView = GetComponent<PhotonView>(); // ## Multiplayer
        }

        void Update ()
        {
            ShootUpdate();
        }

        void ShootUpdate()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
            if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot();
            }
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            /*
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects();
            }
            */
        }

        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot ()
        {
            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;
            
            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
            {
                PlayerHealth playerHealth = shootHit.collider.GetComponent <PlayerHealth> ();

                if(playerHealth != null && photonView.IsMine)
                {                  
                    playerHealth.TakeDamage(damagePerShot, shootHit.point, photonView.Owner);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                //gunLine.SetPosition (1, shootHit.point); // ### meu
                ShootEffect(shootHit.point); // ## Multiplayer
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                //gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range); // ### meu
                ShootEffect(shootRay.origin + shootRay.direction * range); // ## Multiplayer
            }
        }

        void ShootEffect(Vector3 hitPoint)
        {
            photonView.RPC("ShootEffectNetwork", RpcTarget.All, hitPoint);
        }

        [PunRPC]
        void ShootEffectNetwork(Vector3 hitPoint)
        {
            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the lights.
            gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            //FINAL
            gunLine.SetPosition(1, hitPoint);

            //Aguardar para desabilitar
            StartCoroutine(WaitAndDisableEffects(timeBetweenBullets * effectsDisplayTime));

        }

        private IEnumerator WaitAndDisableEffects(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            DisableEffects();
        }
    }
}