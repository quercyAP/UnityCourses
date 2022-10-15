using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] Prefabs;
    public float MinDelay;
    public float MaxDelay;
    private GameObject monster;

    IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(MinDelay, MaxDelay));

            if (monster == null)
            {
                GameObject prefab = Prefabs[Random.Range(0, Prefabs.Length)];
                monster = Instantiate(prefab);
                monster.transform.position = transform.position;
                monster.transform.parent = transform.parent;
            }
        }
    }
}