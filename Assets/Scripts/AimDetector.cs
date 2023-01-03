using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimDetector : MonoBehaviour
{
    public List<GameObject> Enemies = new List<GameObject>();
    public GameObject ClosestEnemy;
    public float ClosestEnemyRange = 20;
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    public void DetectEnemy(float duration)
    {
        if (duration > 0)
            StartCoroutine(StartDetecting_Routine(duration));
        else
        {
            collider.enabled = true;
            Enemies = new List<GameObject>();
            ClosestEnemyRange = 20;
        }
        
    }

    private IEnumerator StartDetecting_Routine(float duration)
    {
        collider.enabled = true;
        Enemies = new List<GameObject>();
        ClosestEnemyRange = 20;
        yield return new WaitForSeconds(duration);
        StopDetecting();
    }

    public void StopDetecting()
    {
        ClosestEnemy = null;
        Enemies.Clear();
        collider.enabled = false;
    }

    private void Update()
    {
        if (SwordSoul.GameManager.Player.State.HasFlag(PlayerState.Jumping) || SwordSoul.GameManager.Player.State.HasFlag(PlayerState.Attacking))
        {
            Debug.Log("je fait des truc");
            foreach (GameObject enemy in Enemies)
            {
                float dist = Vector3.Distance(SwordSoul.GameManager.Player.transform.position, enemy.transform.position);
                if (dist < ClosestEnemyRange && ClosestEnemy == null && enemy.GetComponent<MonsterController>().Settings.Life > 0)
                {
                    ClosestEnemyRange = dist;
                    Debug.Log(ClosestEnemyRange);
                    ClosestEnemy = enemy;
                    Debug.Log(ClosestEnemy.name);
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!Enemies.Contains(other.gameObject) && other.gameObject.tag == "Mob" && SwordSoul.GameManager.Player.State.HasFlag(PlayerState.Jumping) ||
            other.gameObject.tag == "Mob" && SwordSoul.GameManager.Player.State.HasFlag(PlayerState.Attacking))
        {
            Enemies.Add(other.gameObject);
        }
    }
}
