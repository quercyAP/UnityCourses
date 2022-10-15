using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMgr : MonoBehaviour
{
    public GameObject hitParticles;
    public GameObject diedParticles;
    public Animator anim;
    public int life = 3;
    private SphereCollider coll; // collider gloabal du mob
    private GameObject character; // le GO du Perso
    public Rigidbody rb;
    public float KnockStrength = 1;

    private void Awake()
    {
        character = GameObject.Find("RPGHeroHP");
    }
    private void OnTriggerEnter(Collider other)
    {

        // si le mob a ete toucher par lépée
        if (other.gameObject.name == "Sword")
        {
            if (life == 0 && !anim.GetBool("Died"))
            {
                // Gain d'xp
                GameManager.Instance.xp += 100;
                // on lance l'animation et les particules
                anim.SetBool("Died", true);
                Destroy(this.gameObject, 3);
                GameObject go1 = Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(go1, 2);
                GameObject go = Instantiate(diedParticles, transform.position, Quaternion.identity);
                Destroy(go, 3);
                // on set up le loot des potion
                if (Random.Range(0, 100) > 50)
                {
                    GameObject loot = Instantiate(GameManager.Instance.lootSlime[Random.Range(0, 1)], transform.position, Quaternion.Euler(270, 0, 0));
                    loot.transform.position += Vector3.up;
                    loot.name = "FioleLoot";
                }
            }
            else if (life > 0 && character.GetComponent<CharacterCtrl>().isAttacking == true)
            {
                // le monstre recul
                Vector3 direction = transform.position - character.transform.position;
                rb.AddForce(direction.normalized * KnockStrength, ForceMode.Impulse);
                // on lance l'animation + les particule et il perd de la vie,  
                anim.SetTrigger("Hitted");
                life--;
                GameObject go = Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(go, 2);
            }
        }
        // si le mob a ete toucher par le saut
        if (other.gameObject.name == "JumpAttakTrigger")
        {
            if (life > 0 && character.GetComponent<CharacterCtrl>().isJumping == true)
            {
                // le monstre recul
                Vector3 direction = transform.position - character.transform.position;
                rb.AddForce(direction.normalized * KnockStrength, ForceMode.Impulse);
                // on lance l'animation + les particule et il perd de la vie, 
                anim.SetTrigger("Hitted");
                life--;
                GameObject go = Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(go, 2);
            }
            if (life == 0 && !anim.GetBool("Died"))
            {
                // Gain d'xp
                GameManager.Instance.xp += 100;
                // on lance l'animation et les particules
                anim.SetBool("Died", true);
                Destroy(this.gameObject, 3);
                GameObject go1 = Instantiate(hitParticles, transform.position, Quaternion.identity);
                Destroy(go1, 2);
                GameObject go = Instantiate(diedParticles, transform.position, Quaternion.identity);
                Destroy(go, 3);
                // on set up le loot des potion
                if (Random.Range(0, 100) > 50)
                {
                    GameObject loot = Instantiate(GameManager.Instance.lootSlime[Random.Range(0, 1)], transform.position, Quaternion.Euler(270, 0, 0));
                    loot.transform.position += Vector3.up;
                    loot.name = "FioleLoot";
                }
            }
        }
    }

    private void Start()
    {
        // on recupere le collider global du monster
        coll = GetComponent<SphereCollider>();
    }

    private void Update()
    {   
        // On desactive le collider glabal pour qu'il ne soit plus affecter par les attaque
        if (anim.GetBool("Died"))
        {
            coll.enabled = false;
        }
    }
}
