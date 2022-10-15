using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAi : MonoBehaviour
{
    [Range(2, 100)]
    public float detectDistance = 10; // distance de detection du joueur 
    public float attakDistance = 2.2f; // distance d'attaque
    public int Damage = 1;
    Vector3 initialPos; // position d'origin du monstre 
    public SphereCollider col; // collider d'attaque du monstre
    Transform hero; // reference vers le perso joueur
    public bool canAttack = true;
    public NavMeshAgent agent;
    public Animator anim;

    private void Start()
    {
        initialPos = transform.position;
        hero = GameObject.Find("RPGHeroHP").transform;
    }

    private void Update()
    {
        // distance entre le monstre et le joeur
        float distance = Vector3.Distance(transform.position, hero.position);
        if ((distance < detectDistance) && (distance > attakDistance) && !anim.GetBool("Died"))// si le joueur est visible mais pas a porter
        {
            // on s'approche du joeur
            agent.destination = hero.position;

        }
        if ((distance <= attakDistance) && canAttack && !anim.GetBool("Died")) // si le joeur est a porter, qu'on peut attaker et que le mob est vivant
        {
            // on peut attaquer le joueur
            canAttack = false;
            GetComponent<Animator>().SetTrigger("Attack");
            StartCoroutine(AttackPlayer());
        }
        if (distance > detectDistance && !anim.GetBool("Died")) // si le joueur est trop loin on retourne a initialpos
        {
            agent.destination = initialPos;
        }
    }

    IEnumerator AttackPlayer()
    {
        col.enabled = true; // on active le collider d'attaque 
        yield return new WaitForSeconds(1); // on patiente 1s
        col.enabled = false;// on desactive le collider
        yield return new WaitForSeconds(1); 
        canAttack = true; // on peut attaquer  
    }
    // on affiche le distance de detection
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectDistance);
    }
}
