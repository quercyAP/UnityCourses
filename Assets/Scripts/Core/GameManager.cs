using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public CharacterController Player;

    private void Awake()
    {
        SwordSoul.GameManager = this;
    }

    // Caracteristique Perso
    public int xp = 0;
    public int force = 10;
    public int def = 8;

    public int or = 0;
    // Loot
    public GameObject[] lootSlime;

}