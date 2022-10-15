using UnityEngine;

public class PickableWeapon : MonoBehaviour
{
    public WeaponType Type;
    public int WeaponID;
    public Vector3 rotationSpeed;

    private void Start()
    {
        WeaponData data = SwordSoul.ItemsManager.GetWeaponData(WeaponID, Type);
        GameObject weapon = Instantiate(data.Prefab);
        weapon.transform.SetParent(transform);
        weapon.transform.localScale = Vector3.one * 1.5f;
        weapon.transform.position = transform.GetChild(0).position;
        weapon.transform.rotation = transform.GetChild(0).rotation;
        Destroy(transform.GetChild(0).gameObject);
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == SwordSoul.GameManager.Player.gameObject)
        {
            SwordSoul.GameManager.Player.PickUpWeapon(WeaponID, Type);
            Destroy(gameObject);
        }
    }
}

public enum WeaponType
{
    Sword,
    Shield
}