using System;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public WeaponType Type;
    public int ID;
    public GameObject Prefab;
    public float Cooldown = 1f;
    public int Damages = 1;
}