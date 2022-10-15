using System;

public class Events
{
    public event Action<int, WeaponType> PlayerPickupWeapon;
    public event Action<int, int> SetNPCState;
    public event Action<MobSettings> MonsterHitPlayer;
    public event Action<MobSettings> PlayerHitMonster;

    public void Fire_PlayerPickupWeapon(int weaponID, WeaponType type)
    {
        PlayerPickupWeapon?.Invoke(weaponID, type);
    }

    public void Fire_SetNPCState(int NPCID, int StateID)
    {
        SetNPCState?.Invoke(NPCID, StateID);
    }

    public void Fire_MonsterHitPlayer(MobSettings monster)
    {
        MonsterHitPlayer?.Invoke(monster);
    }

    public void Fire_PlayerHitMonster(MobSettings monster)
    {
        PlayerHitMonster?.Invoke(monster);
    }
}