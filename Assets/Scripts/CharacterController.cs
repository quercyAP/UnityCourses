using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    #region Variables
    public PlayerState State;

    // life
    [HideInInspector]
    private float life = 0;
    public Slider LifeBar;
    public float LifeBase = 10;

    // weapon
    public TrailRenderer SwordTrailFX;
    public Transform SwordRootAnchor;
    public MonsterCollisionDetector SwordCollider;
    public Transform ShieldRootAnchor;
    public WeaponData EquipedWeapon { get; private set; }
    public float SwordAttackMonsterKnockBack = 4f;
    private GameObject swordGameObject;
    private GameObject shieldGameObject;

    // Movments
    private NavMeshAgent agent;
    private Animator anim;
    public float speed = 6.0f;
    private float baseSpeed = 6.0f;
    public float rotateSpeed = 90.0f;
    private Vector3 moveDirection = Vector3.zero;

    // Dash
    public float DashDuration = 0.33f;
    public float dashSpeedThreshold = 2.0f;
    public float DashCooldown = 1f;
    private bool canDash = true;

    // Jump
    public GameObject JumpVFXPrefab;
    public MonsterCollisionDetector JumpCollider;
    public float JumpSpeed = 5f;
    public float JumpCooldown = 1f;
    public float JumpMonsterKnockBack = 4f;
    private bool canJump = true;

    // SFX
    private AudioSource audioSource;
    public AudioClip[] swordSfx;

    // Spells
    public GameObject[] Spells;
    public float AttackDuration = 0.33f;
    public float AttackCooldown = 1f;
    private bool canAttack = true;
    #endregion

    void Start()
    {
        // on recupere le composant sur le perso
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        canDash = true;
        State = PlayerState.Idle;
        SwordTrailFX.enabled = false;
        SwordSoul.Events.MonsterHitPlayer += Events_MonsterHitPlayer;
        life = LifeBase;
        baseSpeed = speed;
    }

    #region Monsters
    private void Events_MonsterHitPlayer(MobSettings monster)
    {
        life -= monster.Damage;
        LifeBar.DOValue(life / LifeBase, 0.5f);
        if (life <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SwordSoul.GameManager.Camera.DOShakeRotation(0.18f, 8, 5, 8);
        }
    }
    #endregion

    #region Movements and Inputs
    void Update()
    {
        Inputs();
    }

    void LateUpdate()
    {
        BaseMouvement();
    }

    public void BaseMouvement()
    {
        if (State.HasFlag(PlayerState.Dashing))
            return;

        // calcul du vecteur direction du mouvement
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;
        // on applique le mouvement
        agent.Move(moveDirection * Time.deltaTime);
        // gestion de la rotaiton du perso 
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);

        if (!State.HasFlag(PlayerState.Jumping))
        {
            float walkSpeed = moveDirection.magnitude;
            anim.SetFloat("walkSpeed", walkSpeed);
            // set walking state
            if (walkSpeed > 0)
                State |= PlayerState.Moving;
            else
                State &= ~PlayerState.Moving;
        }
    }

    public void Inputs()
    {
        // sword attack
        if (Input.GetMouseButtonDown(0) && canAttack &&
            !State.HasFlag(PlayerState.Attacking) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing))
        {
            StartCoroutine(Attack());
        }

        // dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash &&
            !State.HasFlag(PlayerState.Attacking) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing))
        {
            StartCoroutine(Dash());
        }

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && canJump &&
            !State.HasFlag(PlayerState.Attacking) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing))
        {
            Jump();
        }
    }
    #endregion

    #region Dash
    IEnumerator Dash()
    {
        // start dash
        State |= PlayerState.Dashing;
        canDash = false;
        float initialSpeed = speed;
        speed = speed * dashSpeedThreshold;
        // end dash
        yield return new WaitForSeconds(DashDuration);
        speed = initialSpeed;
        State &= ~PlayerState.Dashing;
        // dash cooldown
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }
    #endregion

    #region Attack
    IEnumerator Attack()
    {
        audioSource.PlayOneShot(swordSfx[Random.Range(0, 3)]);
        anim.SetTrigger("attack");
        State |= PlayerState.Attacking;
        canAttack = false;
        SwordTrailFX.enabled = true;
        SwordCollider.DetectCollisions(AttackDuration + AttackCooldown);
        yield return new WaitForSeconds(AttackDuration);
        State &= ~PlayerState.Attacking;
        yield return new WaitForSeconds(AttackCooldown);
        SwordTrailFX.enabled = false;
        canAttack = true;
    }
    #endregion

    #region Jump
    private void Jump()
    {
        speed *= 0.5f;
        anim.SetTrigger("JumpAttak");
        State |= PlayerState.Jumping;
        canJump = false;
    }

    IEnumerator Jump_Start()
    {
        speed = baseSpeed * 0.75f;
        while (State.HasFlag(PlayerState.Jumping))
        {
            transform.position += transform.forward * JumpSpeed * Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Jump_Grounded()
    {
        State &= ~PlayerState.Jumping;

        GameObject vfx = Instantiate(JumpVFXPrefab);
        vfx.transform.position = transform.position;
        SwordSoul.GameManager.Camera.DOShakeRotation(0.18f, 8, 5, 8);

        JumpCollider.DetectCollisions(0.25f);
        speed = 0f;
        float enlapsed = 0f;
        while (enlapsed < JumpCooldown)
        {
            speed = Mathf.Lerp(0, baseSpeed, enlapsed / JumpCooldown);
            yield return null;
            enlapsed += Time.deltaTime;
        }
        speed = baseSpeed;
        Destroy(vfx);
        canJump = true;
    }
    #endregion

    #region Dammages
    public void DoAttackDamage(MonsterController monster)
    {
        Debug.Log("Sword attack");
        monster.TakeDamages(EquipedWeapon.Damages, SwordAttackMonsterKnockBack);
    }

    public void DoJumpDammage(MonsterController monster)
    {
        Debug.Log("Jump attack");
        monster.TakeDamages(1, JumpMonsterKnockBack);
    }
    #endregion

    #region PickUp
    public void PickUpWeapon(int weaponID, WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:
                EquipSword(weaponID);
                break;

            case WeaponType.Shield:
                EquipShield(weaponID);
                break;
        }

        // fire equiped event
        SwordSoul.Events.Fire_PlayerPickupWeapon(weaponID, type);
    }

    private void EquipSword(int swordID)
    {
        // destroy old weapon if not null
        if (swordGameObject != null)
            Destroy(swordGameObject);

        // get weapon data from items manager
        EquipedWeapon = SwordSoul.ItemsManager.GetWeaponData(swordID, WeaponType.Sword);

        // instantiate weapon
        swordGameObject = Instantiate(EquipedWeapon.Prefab);
        // get weapon anchor child
        swordGameObject.transform.SetParent(SwordRootAnchor.GetChild(swordID));
        // set default position / rotation / scale
        swordGameObject.transform.localPosition = Vector3.zero;
        swordGameObject.transform.localRotation = Quaternion.identity;
        swordGameObject.transform.localScale = Vector3.one;
    }

    private void EquipShield(int shieldID)
    {
        // destroy old weapon if not null
        if (shieldGameObject != null)
            Destroy(shieldGameObject);

        // instantiate weapon
        shieldGameObject = Instantiate(SwordSoul.ItemsManager.GetWeaponData(shieldID, WeaponType.Shield).Prefab);
        // get weapon anchor child
        shieldGameObject.transform.SetParent(ShieldRootAnchor.GetChild(shieldID));
        // set default position / rotation / scale
        shieldGameObject.transform.localPosition = Vector3.zero;
        shieldGameObject.transform.localRotation = Quaternion.identity;
        shieldGameObject.transform.localScale = Vector3.one;
    }
    #endregion
}

public enum PlayerState
{
    Idle = 0,
    Moving = 1,
    Attacking = 2,
    Dashing = 4,
    Jumping = 8
}