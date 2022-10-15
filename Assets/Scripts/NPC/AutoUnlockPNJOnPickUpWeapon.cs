using UnityEngine;
using UnityEngine.AI;

public class AutoUnlockPNJOnPickUpWeapon : MonoBehaviour
{
    public int NPCID;
    public int NPCStateAfterUnlock;
    public bool SpecificWeapon = false;
    public int UnlockWeaponID;
    public WeaponType UnlockWeaponType;

    void Start()
    {
        SwordSoul.Events.PlayerPickupWeapon += Events_PlayerPickupWeapon;
    }

    private void Events_PlayerPickupWeapon(int weaponID, WeaponType type)
    {
        if (type == UnlockWeaponType && (UnlockWeaponID == weaponID && SpecificWeapon || !SpecificWeapon))
        {
            SwordSoul.Events.PlayerPickupWeapon -= Events_PlayerPickupWeapon;
            GetComponent<NavMeshObstacle>().enabled = false;
            GetComponent<Animator>().SetBool("ObstacleIsDestroy", true);
            SwordSoul.Events.Fire_SetNPCState(NPCID, NPCStateAfterUnlock);
        }
    }
}