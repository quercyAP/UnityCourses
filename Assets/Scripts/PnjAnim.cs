using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnjAnim : MonoBehaviour
{
    public GameObject obstacle1;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        if (obstacle1 == null)
        {
            anim.SetBool("ObstacleIsDestroy", true);
        }
    }
}
