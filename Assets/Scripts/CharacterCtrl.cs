using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCtrl : MonoBehaviour
{
    // variable pour le deplacement
    public float speed = 6.0f;
    public float rotateSpeed = 90.0f;
    public float gravity = 20.0f;
    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded;
    // Dash
    public float maxDashTime = 1.0f;
    public float dashSpeed = 1.0f;
    public float dashStoppingSpeed = 0.1f;
    private float currentDashTime;
    private bool isDashing;
    // JumpAttack
    private Vector3 jumpDirection = Vector3.zero;
    public float maxJumpTime = 1.0f;
    public float JumpSpeed = 1.0f;
    public float JumpStoppingSpeed = 0.1f;
    private float currentJumpTime;
    [HideInInspector]
    public bool isJumping;
    private GameObject jumpAttakTrigger;
    private SphereCollider jumpAttakColl;
    // les composants
    private CharacterController controller;
    private Animator anim;
    private AudioSource audioSource;
    // SFX
    public AudioClip[] swordSfx;
    // Spells
    public GameObject FireShield;
    public GameObject[] Spells;
    public bool isAttacking;
    void Start()
    {
        // on recupere le composant sur le perso
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        FireShield.SetActive(false);
        currentDashTime = maxDashTime;
        jumpAttakTrigger = GameObject.Find("JumpAttakTrigger");
        jumpAttakColl = jumpAttakTrigger.GetComponent<SphereCollider>();
    }
    void FixedUpdate()
    {
        baseMouvement();
        attack();
        spell();
        Dash();
        JumpAttak();
    }
    public void baseMouvement()
    {
        if (isGrounded)
        {
            anim.SetFloat("walkSpeed", controller.velocity.magnitude);
            // calcul du vecteur direction du mouvement
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
        }
        // on applique la graviter sur le perso
        moveDirection.y -= gravity * Time.deltaTime;
        Physics.SyncTransforms();
        // on applique le mouvement
        var flags = controller.Move(moveDirection * Time.deltaTime);
        // gestion de la rotaiton du perso 
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime, 0);
        // detection du sol
        isGrounded = CollisionFlags.CollidedBelow != 0;
    }
    public void attack()
    {
        if (Input.GetMouseButtonDown(0) && isAttacking == false && isDashing == false)
        {
            StartCoroutine("AttackCooldown");
            audioSource.PlayOneShot(swordSfx[Random.Range(0, 3)]);
            anim.SetTrigger("attack");
        }
    }
    public void spell()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject go = Instantiate(Spells[0],  transform.position, Quaternion.identity);
            Destroy(go, 3);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject go = Instantiate(Spells[1], transform.position, Quaternion.identity);
            Destroy(go, 3);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            FireShield.SetActive(true);
            StartCoroutine("FireShieldCooldown");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject go = Instantiate(Spells[2], transform.position, Quaternion.identity);
            Destroy(go, 3);
        }
    }
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == false && isJumping == false)
        {
            currentDashTime = 0.0f;
            StartCoroutine("DashCooldown");
        }
        if (currentDashTime < maxDashTime)
        {
            moveDirection *= dashSpeed;
            currentDashTime += dashStoppingSpeed;
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
    public void JumpAttak()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isJumping == false)
        {
            jumpAttakColl.radius = 0.5f;
            jumpAttakColl.isTrigger = false;
            currentJumpTime = 0.0f;
            anim.SetTrigger("JumpAttak");
            isJumping = true;
            StartCoroutine("JumpCooldown");
        }
    }
    IEnumerator FireShieldCooldown()
    {
        yield return new WaitForSeconds(10);
        FireShield.SetActive(false);
    }
    IEnumerator DashCooldown()
    {

        isDashing = true;
        yield return new WaitForSeconds(1);
        isDashing = false;
    }
    IEnumerator AttackCooldown()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1);
        isAttacking = false;
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(0.4f);
        while (currentJumpTime < maxJumpTime)
        {
            jumpDirection = new Vector3(0, 1, 1);
            jumpDirection = transform.TransformDirection(jumpDirection);
            jumpDirection *= JumpSpeed;
            currentJumpTime += JumpStoppingSpeed;
            controller.Move(jumpDirection * Time.deltaTime);
        }
        yield return new WaitForSeconds(0.3f);
        jumpAttakColl.isTrigger = true;
        while (jumpAttakColl.radius < 1.6f)
        {
            jumpAttakColl.radius += 0.2f;
        }
        yield return new WaitForSeconds(1);
        isJumping = false;
    }
}
