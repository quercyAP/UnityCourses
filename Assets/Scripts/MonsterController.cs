using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    public Transform Emitter;
    public float AttackRadius = 0.5f;
    public MobSettings Settings;
    public GameObject HitParticles;
    public GameObject DiedParticles;
    public Slider LifeBar;
    private NavMeshAgent agent;
    private Animator animator;
    private bool canAttack = true;
    private Vector3 initialPos; // position d'origin du monstre 

    private void Start()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        initialPos = transform.position;
        LifeBar.transform.parent.GetComponent<Canvas>().worldCamera = SwordSoul.GameManager.Camera;
        Settings.Life = Settings.BaseLife;
        LifeBar.minValue = 0;
        LifeBar.maxValue = Settings.BaseLife;
        LifeBar.DOValue(Settings.Life, 0.5f);
        agent.Warp(transform.position);
    }

    private void Update()
    {
        // distance entre le monstre et le joeur
        float distance = Vector3.Distance(transform.position, SwordSoul.GameManager.Player.transform.position);

        // si le joueur est visible mais pas a porter, on s'approche du joeur
        if ((distance < Settings.DetectDistance) && (distance > Settings.AttakDistance))
            agent.destination = SwordSoul.GameManager.Player.transform.position;

        // si le joeur est a porter, qu'on peut attaker et que le mob est vivant, on peut attaquer le joueur
        if ((distance <= Settings.AttakDistance) && canAttack)
        {
            StartCoroutine(AttackCooldown_Routine());
        }

        // si le joueur est trop loin on retourne a initialpos
        if (distance > Settings.DetectDistance)
            agent.destination = initialPos;
    }

    private IEnumerator AttackCooldown_Routine()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(Settings.AttackCooldown);
        canAttack = true;
    }

    public void Monster_Attack()
    {
        // get player direction
        Vector3 playerDirection = SwordSoul.GameManager.Player.transform.position - transform.position;
        // raycast to player
        RaycastHit[] hits = Physics.SphereCastAll(Emitter.position, AttackRadius, playerDirection, AttackRadius);
        // check if raycast hit player
        foreach (RaycastHit hit in hits)
        {
            // if the raycast hit the player
            if (hit.collider.gameObject == SwordSoul.GameManager.Player.gameObject)
            {
                SwordSoul.Events.Fire_MonsterHitPlayer(Settings);
                return;
            }
        }
    }

    public void TakeDamages(int damages, float knockStrength)
    {
        // loose life
        Settings.Life -= damages;
        LifeBar.DOValue(Settings.Life, 0.5f);

        // hitted particles
        GameObject vfx = Instantiate(HitParticles, transform.position, Quaternion.identity);
        Destroy(vfx, 2f);

        // we just die
        if (Settings.Life <= 0)
        {
            animator.SetBool("Died", true);
            // on lance l'animation et les particules
            GameObject go = Instantiate(DiedParticles, transform.position, Quaternion.identity);
            Destroy(go, 3f);
            Destroy(this);
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(gameObject, 2f); // TODO : fade disapear
        }
        else
        {
            animator.SetTrigger("Hitted");
            // le monstre recul
            Vector3 direction = transform.position - SwordSoul.GameManager.Player.transform.position;
            StartCoroutine(KnockBack(direction * knockStrength, 0.5f));
        }
    }

    IEnumerator KnockBack(Vector3 direction, float duration)
    {
        float enlapsed = 0f;
        //agent.enabled = false;
        while (enlapsed < duration)
        {
            agent.Move(Vector3.Lerp(direction, Vector3.zero, enlapsed / duration));
            yield return null;
            enlapsed += Time.deltaTime;
        }
        //agent.enabled = true;
    }
}

[Serializable]
public class MobSettings
{
    [Range(2, 100)]
    public float DetectDistance = 10; // distance de detection du joueur 
    public float AttakDistance = 2.2f; // distance d'attaque
    public float AttackCooldown = 1f;
    public int Damage = 1;
    public int BaseLife = 3;
    [HideInInspector]
    public int Life;
}