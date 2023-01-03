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
    public AimDetector jumpAimDetetector;
    public float JumpSpeed = 5f;
    public float JumpCooldown = 1f;
    public float JumpMonsterKnockBack = 4f;
    private bool canJump = true;

    // SwordSlash
    public GameObject Slash;
    public MonsterCollisionDetector SlashCollider;
    public float SlashSpeed = 2f;
    public float SlashDuration = 2f;
    public float SlashMonsterKnockBack = 0f;
    public AnimationCurve SlashSpeedCurve;
    private bool canSlash = true;

    // SFX
    private AudioSource audioSource;
    public AudioClip[] swordSfx;

    // Attack
    public AimDetector AttackAimDetector;
    public float AimSpeed;
    public int clickCount = 0;
    #endregion

    void Start()
    {
        // on recupere le composant sur le perso
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        canDash = true;
        State = PlayerState.Idle;
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
        checkAnimatorState();
        SwordComboCollision();
    }

    void LateUpdate()
    {
        BaseMouvement();
    }

    public void BaseMouvement()
    {

        // calcul du vecteur direction du mouvement
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;
        // on applique le mouvement
        // gestion de la rotaiton du perso 
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);

        if (!State.HasFlag(PlayerState.Jumping) && !State.HasFlag(PlayerState.Attacking))
        {
            agent.Move(moveDirection * Time.deltaTime);
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
        if (Input.GetMouseButtonDown(0) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing) &&
            !State.HasFlag(PlayerState.Slashing))
        {
            OnClick();
        }

        // dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash &&
            !State.HasFlag(PlayerState.Attacking) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing) &&
            !State.HasFlag(PlayerState.Slashing))
        {
            StartCoroutine(Dash());
        }

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && canJump &&
            !State.HasFlag(PlayerState.Attacking) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing) &&
            !State.HasFlag(PlayerState.Slashing))
        {
            Jump();
        }
        // SwordSlash
        if (Input.GetMouseButtonDown(1) && canSlash &&
            !State.HasFlag(PlayerState.Attacking) &&
            !State.HasFlag(PlayerState.Jumping) &&
            !State.HasFlag(PlayerState.Dashing) &&
            !State.HasFlag(PlayerState.Slashing))
        {
            SwordSlash();
        }
    }
    #endregion

    #region SwordSlash
    private void SwordSlash()
    {
        canSlash = false;
        State |= PlayerState.Slashing;
        anim.SetTrigger("SwordSlash");
    }
    IEnumerator SwordSlashFx()
    {
        GameObject SwordSlash = Instantiate(Slash);
        SlashCollider = SwordSlash.GetComponent<MonsterCollisionDetector>();
        SwordSlash.transform.position = transform.position;
        SwordSlash.transform.forward = transform.forward;
        float enlapsed = 0f;
        while (enlapsed < SlashDuration)
        {
            SlashCollider.DetectCollisions(SlashDuration);
            SwordSlash.transform.Translate(SwordSlash.transform.forward * Time.deltaTime * SlashSpeed * SlashSpeedCurve.Evaluate(enlapsed / SlashDuration), Space.World);
            yield return null;
            enlapsed += Time.deltaTime;
        }
        Destroy(SwordSlash);
    }
    private void SwordSlash_End()
    {
        State &= ~PlayerState.Slashing;
        canSlash = true;
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
        Animator animator = GetComponent<Animator>();
        animator.speed = 2f;
        // end dash
        yield return new WaitForSeconds(DashDuration);
        animator.speed = 1;
        speed = initialSpeed;
        State &= ~PlayerState.Dashing;
        // dash cooldown
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }
    #endregion

    #region Attack
    private void checkAnimatorState()
    {
        anim.SetInteger("ClickCount", clickCount);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            SwordCollider.StopDetecting();
            AttackAimDetector.StopDetecting();
            clickCount = 0;
            State &= ~PlayerState.Attacking;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > anim.GetCurrentAnimatorStateInfo(0).length / anim.GetCurrentAnimatorStateInfo(0).speed &&
            anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            anim.SetBool("hit1", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > anim.GetCurrentAnimatorStateInfo(0).length / anim.GetCurrentAnimatorStateInfo(0).speed &&
            anim.GetCurrentAnimatorStateInfo(0).IsName("hit2"))
        {
            anim.SetBool("hit1", false);
            anim.SetBool("hit2", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > anim.GetCurrentAnimatorStateInfo(0).length / anim.GetCurrentAnimatorStateInfo(0).speed &&
            anim.GetCurrentAnimatorStateInfo(0).IsName("hit3"))
        {
            anim.SetBool("hit2", false);
            anim.SetBool("hit3", false);
        }

    }
    private void SwordComboCollision()
    {
        if (!State.HasFlag(PlayerState.Attacking) && anim.GetCurrentAnimatorStateInfo(0).IsName("hit1"))
        {
            SwordCollider.DetectCollisions(0);
            AttackAimDetector.DetectEnemy(0);
            State |= PlayerState.Attacking;
        }
        if (State.HasFlag(PlayerState.Attacking) && AttackAimDetector.ClosestEnemy != null)
        {
            var maxDistance = Vector3.Distance(transform.position, AttackAimDetector.ClosestEnemy.transform.position);
            var targetRotation = Quaternion.LookRotation(AttackAimDetector.ClosestEnemy.transform.position - transform.position);
            if (maxDistance > 2f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (rotateSpeed / 2) * Time.deltaTime);
                transform.position += transform.forward * AimSpeed * Time.deltaTime;
            }
        }
    }
    private void OnClick()
    {
        clickCount++;
        if (clickCount == 1)
        {
            anim.SetBool("hit1", true);
        }
        clickCount = Mathf.Clamp(clickCount, 0, 3);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < anim.GetCurrentAnimatorStateInfo(0).length / anim.GetCurrentAnimatorStateInfo(0).speed - 0.2f &&
            clickCount >= 2)
        {
            anim.SetBool("hit2", true);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < anim.GetCurrentAnimatorStateInfo(0).length / anim.GetCurrentAnimatorStateInfo(0).speed - 0.2f &&
            clickCount >= 3)
        {
            anim.SetBool("hit3", true);
        }
    }
    #endregion

    #region Jump
    private void Jump()
    {
        speed *= 0.5f;
        anim.SetTrigger("JumpAttak");
        State |= PlayerState.Jumping;
        jumpAimDetetector.DetectEnemy(1f);
        canJump = false;
    }
    IEnumerator Jump_Start()
    {
        speed = baseSpeed * 0.75f;
        while (State.HasFlag(PlayerState.Jumping))
        {
            if (jumpAimDetetector.ClosestEnemy != null)
            {
                var maxDistance = Vector3.Distance(transform.position, jumpAimDetetector.ClosestEnemy.transform.position);
                var targetRotation = Quaternion.LookRotation(jumpAimDetetector.ClosestEnemy.transform.position - transform.position);
                if (maxDistance > 2f && targetRotation.eulerAngles.y > 75f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
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
        monster.TakeDamages(EquipedWeapon.Damages, transform.position, SwordAttackMonsterKnockBack);
    }

    public void DoJumpDammage(MonsterController monster)
    {
        monster.TakeDamages(1, transform.position, JumpMonsterKnockBack);
    }

    public void DoSlashDammage(MonsterController monster)
    {
        monster.TakeDamages(2, monster.transform.position, SlashMonsterKnockBack);
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
    Jumping = 8,
    Slashing = 16
}