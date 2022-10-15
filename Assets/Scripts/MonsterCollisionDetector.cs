using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterCollisionDetector : MonoBehaviour
{
    public UnityEvent<MonsterController> OnCollide;
    private HashSet<Collider> collided = new HashSet<Collider>();
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    private IEnumerator StartDetecting_Routine(float duration)
    {
        collider.enabled = true;
        collided = new HashSet<Collider>();
        yield return new WaitForSeconds(duration);
        StopDetecting();
    }

    public void DetectCollisions(float duration)
    {
        StartCoroutine(StartDetecting_Routine(duration));
    }


    public void StopDetecting()
    {
        collider.enabled = false;
        collided.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!collided.Contains(other))
        {
            MonsterController mc = other.GetComponent<MonsterController>();
            if (mc != null)
            {
                collided.Add(other);
                OnCollide?.Invoke(mc);
            }
        }
    }
}