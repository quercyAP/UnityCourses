using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public List<WeaponData> Weapons;

    void Awake()
    {
        SwordSoul.ItemsManager = this;
    }

    public WeaponData GetWeaponData(int weaponID, WeaponType type)
    {
        foreach (WeaponData data in Weapons)
        {
            if (data.ID == weaponID && data.Type == type)
                return data;
        }
        return null;
    }
}