using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Caracteristique Perso
    public int xp = 0;
    public int force = 10;
    public int def = 8;

    public int or = 0;
    // Loot
    public GameObject[] lootSlime;

}
