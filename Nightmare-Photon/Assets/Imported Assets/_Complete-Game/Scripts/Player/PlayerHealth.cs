using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

namespace CompleteProject
{
    public class PlayerHealth : MonoBehaviourPunCallbacks
    {
        public int startingHealth = 100;                            // The amount of health the player starts the game with.
        public int currentHealth;                                   // The current health the player has.
        public Slider healthSlider;                                 // Reference to the UI's health bar.
        public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
        public AudioClip deathClip;                                 // The audio clip to play when the player dies.
        public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


        Animator anim;                                              // Reference to the Animator component.
        AudioSource playerAudio;                                    // Reference to the AudioSource component.
        PlayerMovement playerMovement;                              // Reference to the player's movement.
        PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
        bool isDead;                                                // Whether the player is dead.
        bool damaged;                                               // True when the player gets damaged.

        //### Multiplayer
        public ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
        PhotonView photonView;

        public GameObject canvasHUD;

        public Text playerScoreText;

        public RuntimeAnimatorController animatorController;
        public AudioClip hurtClip;

        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            playerAudio = GetComponent <AudioSource> ();
            playerMovement = GetComponent <PlayerMovement> ();
            playerShooting = GetComponentInChildren <PlayerShooting> ();

            // Set the initial health of the player.
            currentHealth = startingHealth;

            //hitParticles = GetComponentInChildren<ParticleSystem>(); //###Meu
            photonView = GetComponent<PhotonView>();

            if (!photonView.IsMine)
            {
                canvasHUD.SetActive(false);
            }
        }


        void Update ()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            // If the player has just been damaged...
            if(damaged)
            {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else
            {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }


        public void TakeDamage (int amount, Vector3 hitPoint, Player origin)  
        {
            photonView.RPC("TakeDamageNetwork", RpcTarget.All, amount, hitPoint, origin);
        }

        [PunRPC]
        public void TakeDamageNetwork(int amount, Vector3 hitPoint, Player origin) 
        {
            //SCORE
            origin.AddScore(amount);  

            // Set the position of the particle system to where the hit was sustained.
            hitParticles.transform.position = hitPoint; //##MEU

            // And play the particles.
            hitParticles.Play(); //##Meu

            // Set the damaged flag so the screen will flash.
            damaged = true;

            // Reduce the current health by the damage amount.
            currentHealth -= amount;

            // Set the health bar's value to the current health.
            healthSlider.value = currentHealth;

            // Play the hurt sound effect.
            playerAudio.Play();

            // If the player has lost all it's health and the death flag hasn't been set yet...
            if (currentHealth <= 0 && !isDead)
            {
                //SCORE
                origin.AddScore(amount);  

                // ... it should die.
                Death();
            }
        }

        void Death ()
        {
            // Set the death flag so this function won't be called again.
            isDead = true;

            // Turn off any remaining shooting effects.
            playerShooting.DisableEffects ();

            // Tell the animator that the player is dead.
            anim.SetTrigger ("Die");

            GetComponent<CapsuleCollider>().isTrigger = true; //#multiplayer

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;

            StartCoroutine(DeathEffect(2f));
        }

        public void RestartLevel ()
        {
            // Reload the level that is currently loaded.
            //SceneManager.LoadScene (0);
        }


        IEnumerator DeathEffect(float time)
        {
            yield return new WaitForSeconds(time);

            GetComponent<Rigidbody>().isKinematic = true;
            transform.Translate(new Vector3(0, -300f, 0) * 2.5f * Time.deltaTime);
            StartCoroutine(WaitForRespawn(3f));
        }

        IEnumerator WaitForRespawn(float time)
        {
            yield return new WaitForSeconds(time);
            Respawn();
        }

        void Respawn()
        {
            isDead = false;
            playerAudio.clip = hurtClip;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<CapsuleCollider>().isTrigger = false;
            anim.runtimeAnimatorController = null;
            anim.runtimeAnimatorController = animatorController;

            Transform[] spawnPlayer = GameObject.Find("_GC").GetComponent<GameControllerGamePlay>().spawnPLayer;
            int i = Random.Range(0, spawnPlayer.Length);
            
            transform.position = spawnPlayer[i].position;
            transform.rotation = new Quaternion(0, 0, 0, 0);
            currentHealth = startingHealth;
            healthSlider.value = currentHealth;

            playerMovement.enabled = true;
            playerShooting.enabled = true;
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if(photonView.Owner.ActorNumber == targetPlayer.ActorNumber)
            {
                object temp;
                changedProps.TryGetValue("score", out temp);
                playerScoreText.text = "Score: " + (int)temp;
            }  
        }
    }
}