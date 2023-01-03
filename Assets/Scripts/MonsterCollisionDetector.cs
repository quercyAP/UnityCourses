using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterCollisionDetector : MonoBehaviour
{
    public UnityEvent<MonsterController> OnCollide;
    public HashSet<Collider> Collided = new HashSet<Collider>();
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    private IEnumerator StartDetecting_Routine(float duration)
    {
        collider.enabled = true;
        Collided = new HashSet<Collider>();
        yield return new WaitForSeconds(duration);
        StopDetecting();
    }

    public void DetectCollisions(float duration)
    {
        if (duration > 0)
            StartCoroutine(StartDetecting_Routine(duration));
        else
        {
            collider.enabled = true;
            Collided = new HashSet<Collider>();
        }
    }


    public void StopDetecting()
    {
        Collided.Clear();
        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterController mc = other.GetComponent<MonsterController>();
        if (mc != null)
        {
            Collided.Add(other);
            OnCollide?.Invoke(mc);
        }   
    }
}