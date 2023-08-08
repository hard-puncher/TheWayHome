using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Inventory inventory;

    // �ʿ��� ������Ʈ
    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    SpriteRenderer sprite;
    Animator animator;

    [Header("Flag")]
    public bool isJump; // ���� ������ ����
    public bool isGround;   // �ٴڿ� �ִ��� ����
    public bool canDash = true;
    public bool isDash; // ������ �뽬 ������
    public bool nowDash;    // �뽬 ���ΰ�?
    public bool isFall; // ���� ����� �� rigid.velocity�� ������ �𸣰����� ������ �Ǿ� fall �ִϸ��̼��� ����Ǵ� ���� �߻�, ���� ����� ���� isFall�� false�� �ؼ� �ذ�
    public bool isAttack;   // ���������� üũ
    public bool canAttack;  // �����ֱⰡ ���ƿԴ��� üũ
    public bool isDie;
    public bool isInvincible;   // �������� üũ, true�϶� �ǰݴ����� �ʴ´�.
    public bool isHide; // ���� ���������� üũ, �������϶� ���� X
    public bool isCrawl;    // ���帮�� �ִ��� ����(���帮�� ������ �� ����� ������ ���� �� �˾������� ���ϰ� �Ұ�)

    // Wall Climb
    private RaycastHit2D rayHit;
    public bool isWallTouch;    // ���� �´��� üũ
    public bool canWallClimb;   // isWallTouch�� true�� �Ǹ� canWallClimb�� true�� �Ǿ� ���� ���� �� �ִ�.
   
    // �Ӹ� ���� �÷����� �´���ִ��� üũ
    private RaycastHit2D rayHitSneak;

    [Space(10f)]
    [Header("Status By Character")]
    public float defaultSpeed = 6f;  // ĳ���ͺ� �̵��ӵ�
    public float dashSpeed = 12f; // ĳ���ͺ� �뽬�ӵ�
    public float crawlSpeed = 3f;    // ĳ���ͺ� ���� �ӵ�
    public float applySpeed; // ������ ����� �ӵ�
   
    public float jumpPower; // ������
    
    public float dashDelay; // ĳ���ͺ� �뽬�ֱ�
    public float dashPower; // ĳ���ͺ� �뽬�Ŀ�(defaultSpeed�� ������ ��)
    public float maxClimbSpeed; // ĳ���ͺ� �� ���� �ӵ�
   
    public float attackDelay;   // ĳ���ͺ� ���� �ֱ�
    
    public float knockBackPower;    // ĳ���ͺ� �˹���ϴ� ����

    // ĳ���ͺ� �ִ� ü��.
    public int maxHp;
    
    [Space(10f)]
    [Header("Common")] 
    public GameObject attackRange;  // ���� ���� ���ӿ�����Ʈ
    public float defaultFriction = 0.03f;   // �⺻ ������(����� �ε巴�� �ö󰡴� ����)
    public float wallSlideFriction = 0.3f; // ���� ����� �� ������(�� �̲������� �ʰԲ�)
    public int jumpCount;   // ���� ���� ���� Ƚ��
    public int maxJumpCount;    // �ִ� ���� ���� Ƚ��(�⺻ 2ȸ)
   
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        inventory = GameObject.Find("InventoryGroup").GetComponent<Inventory>();
    }
    
    // ���� ������ ĳ���Ͱ� Ȱ��ȭ�� ��
    private void OnEnable()
    {
        // ���� ���� �� ����ī�޶� ã�Ƽ� �÷��̾� �ڽ����� ����
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.SetParent(gameObject.transform);
        
        // ���� �� �÷��̾� ��ġ ����
        this.transform.position = new Vector3(-15, -3.2f, 0);
    }

    void Update()
    {
        if (isGround)
        {
            animator.SetBool("isFall", false);
        }
            
        CheckDie();

        // �뽬 ������ ����
        if (!canDash)
        {
            dashDelay += Time.deltaTime;
            if(dashDelay >= 3f)
            {
                dashDelay = 0f;
                canDash = true;
            }
        }
        // ���� ������ ����
        if(!canAttack)
        {
            attackDelay += Time.deltaTime;
            if(attackDelay >= 1f)
            {
                attackDelay = 0f;
                canAttack = true;
            }
        }

        if (!isDie)
        {
            Attack();
            if (!isHide && !isCrawl)
                Jump();
            FlipX();
            RunAnim();
            SneakAnim();
        }        
    }

    void FixedUpdate()
    {
        if(!isDie)
        {
            MoveHor();   
            WallClimb();
        }
        FallAnim(); // ��Ÿ�ٰ� �׾��� ��� �������� �ؾ��ϹǷ� FallAnim()�� isDie�� ���� �����Ѵ�.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isDash = true;
        isFall = false;

        // ��(��)�� ����� ��
        if(collision.gameObject.tag == "Ground")
        {
            isJump = false;
            isGround = true;
            jumpCount = 0;
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }

        // �������� ���ǰ� ����� ��
        if(collision.gameObject.tag == "FallingSheet")
        {
            isJump = false;
            isGround = true;
            jumpCount = 0;
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }

        // �������°� �ƴϸ鼭 ���� �ε����� ��
        if (collision.gameObject.tag == "Enemy" && !isInvincible)
        {
            // �ǰ� ���� ���
            SoundManager.instance.PlaySE("Hit");

            // �� ���ݷ¸�ŭ ü�� ����
            GameManager.Instance.DecreaseHP(collision.gameObject.GetComponent<EnemyController>().enemyDamage);

            CheckDie();

            // �˹� -> �ε��� ���� x��ǥ�� �÷��̾�� �����ʿ� ������ �������� ƨ���, ���ʿ� ������ ���������� ƨ���.
            rigid.AddForce(collision.gameObject.GetComponent<Rigidbody2D>().position.x > rigid.position.x ? Vector2.left * knockBackPower : Vector2.right * knockBackPower, ForceMode2D.Impulse);

            // ��������ȭ
            Invincible();

            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);

            animator.SetTrigger("isHurt");
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "FallingSheet")
        {
            isFall = true;
            isDash = false;
            isGround = false;
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ Ʈ���� ���¿��� ���� �浹 -> ���� �����̹Ƿ� �߶� ������ ���� �ӵ� 0
        if (collision.gameObject.tag == "Ground")
        {
            jumpCount = 0;
            isGround = true;
            rigid.velocity = Vector2.zero;
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }

        if (collision.gameObject.tag == "DeadZone")
        {
            rigid.velocity = new Vector2(0, 0); // y�ӵ��� 0���� ������ ������ fall �ִϸ��̼��� ����ȴ�.
            rigid.isKinematic = true;   // �߶� �����.
            GameManager.Instance.DecreaseHP(GameManager.Instance.curHp);
            Debug.Log("�� �ٱ����� ���������ϴ�. ���� �����Դϴ�.");
            CheckDie();
        }

        // ���� �����۰� �浹 �� isGround�� true�� �ؼ� 2�������� �ǰ� ��.
        if(collision.gameObject.tag == "JumpStone")
        {
            isGround = true;
            if (jumpCount <= 0)
                return;
            jumpCount--;
        }
    }

    // ���� ����
    public void Invincible()
    {
        sprite.color = new Color(1, 1, 1, 0.4f);
        isInvincible = true;
     
        Invoke("OffInvincible", 1f);    // 1�ʰ� ����
    }
    // ���� ���� ����
    public void OffInvincible()
    {
        sprite.color = new Color(1, 1, 1, 1f);
        isInvincible = false;
    }
    // ��� üũ
    public void CheckDie()
    {
        if (GameManager.Instance.curHp <= 0 && !isDie)
        {
            isDie = true;

            // ���� ������ ���·� ����� ��츸 ���ڸ����� �����.
            if(isGround && !isWallTouch)
            {
                //rigid.velocity = Vector2.zero;
                capsule.enabled = false;
                rigid.isKinematic = true;
            }

            // �ٸ� �ִϸ��̼� �� ��ž
            animator.SetBool("isCrouch", false);
            animator.SetBool("isSneak", false);
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
            animator.SetBool("isFall", false);

            animator.SetTrigger("isDie");

            Invoke("StopGame", 2f);
        }
    }

    private void StopGame()
    {
        Time.timeScale = 0f;
        GameManager.Instance.gameOverUI.SetActive(true);    // ���ӿ��� ui Ȱ��ȭ
    }

    // ����
    private void Attack()
    {
        if(canAttack && Input.GetKeyDown(KeyCode.LeftControl))
        {
            canAttack = false;
            isAttack = true;
            animator.SetTrigger("isAttack");
            animator.SetBool("isFall", false);
            attackRange.SetActive(true);
            // ������ �� �� �÷��̾� ��������Ʈ flipX�� True�̸� attackRange�� �ݶ��̴��� ��¦ ���ʿ�, False�̸� �����ʿ� �״�� Ȱ��ȭ�Ѵ�.
            attackRange.GetComponent<BoxCollider2D>().offset = sprite.flipX == true ? new Vector2(-0.18f, 0f) : new Vector2(0f, 0f);
            Invoke("AttackOff", 0.5f);
        }
    }

    private void AttackOff()
    {
        isAttack = false;
        attackRange.SetActive(false);
    }

    // Dash ������ ���� �ִ�ӵ��� 2�� �÷� ������ �̵�
    void Dash()
    {
        nowDash = true;
        // �뽬 ���� ���
        SoundManager.instance.PlaySE("Dash");
        applySpeed = dashSpeed;  // ���� �ӵ��� �뽬 �ӵ���
        Invoke("DashOff", 0.833f);  // Dash �ִϸ��̼� ���̸�ŭ invoke�� �ɾ��ش�.
    }

    void DashOff()
    {
        nowDash = false;
        applySpeed = defaultSpeed;
    }

    // ����
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            // Jump �ִϸ��̼�
            animator.SetBool("isJump", true);
            animator.SetBool("isFall", false);

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void FlipX()
    {
        // ��������Ʈ flipX
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            sprite.flipX = true;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            sprite.flipX = false;
    }

    void RunAnim()
    {
        // ���帮�ų� ���� ���� ���� ���� Run �ִϸ��̼� ����Ѵ�.
        if (!Input.GetKey(KeyCode.DownArrow))
        {
            // Idle -> Run
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                animator.SetBool("isRun", true);

                // ���߿� �� ������, ���� ���� ��, ���� ���� ���� �뽬 X
                // Dash: Run ���¿��� canDash�϶� shift�� ������ ����
                if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && !isFall && !isCrawl && !isJump)
                {
                    animator.SetTrigger("isDash");
                    canDash = false;
                    // ���� ��� �ְ� ���帮�ų� ���� ���� �ƴ� ���� �뽬 ����
                    if(isGround && rayHitSneak.collider == null)
                        Dash(); // defaultSpeed�� �Ͻ������� �÷� ������ �޸��� ��ó�� ���̰��ϴ� �Լ�
                }
            }

            // Run -> Idle
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                animator.SetBool("isRun", false);
        }
    }

    void SneakAnim()
    {
        // ��� ���� �� �Ͼ�� ������ �����ϱ� ���� �÷��̾� ���� �������� ���̸� ����, ���� �÷����� �ִٸ� �Ͼ �� ���� �Ѵ�.
        Debug.DrawRay(rigid.position, Vector3.up, new Color(0, 1, 0));

        rayHitSneak = Physics2D.Raycast(rigid.position, Vector3.up, 0.6f, LayerMask.GetMask("Platform"));

        if(rayHitSneak.collider != null)
        {
            isCrawl = true; // ���帮�� ������ true�� �Ѵ�.

            // ��ũ�� ���¿��� ���� ��θ� ���� �� �ֵ��� �ݶ��̴� ũ��� �������� �����Ѵ�.
            capsule.offset = new Vector2(capsule.offset.x, -0.05f);
            capsule.size = new Vector2(capsule.size.x, 0.08f);

            animator.SetBool("isRun", false);

            // ��ũ�� ���¿��� applySpeed�� crawlSpeed��
           // if(!nowDash)
            applySpeed = crawlSpeed;

            // Crouch �ִϸ��̼�
            animator.SetBool("isCrouch", true);
            // Sneak(����)      
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {   
                animator.SetBool("isCrouch", false);
                animator.SetBool("isSneak", true);
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                animator.SetBool("isCrouch", true);
                animator.SetBool("isSneak", false);
            }
        }
        else
        {
            isCrawl = false;

            // �ݶ��̴� �������
            capsule.offset = new Vector2(capsule.offset.x, -0.025f);
            capsule.size = new Vector2(capsule.size.x, 0.13f);

            animator.SetBool("isCrouch", false);
            animator.SetBool("isSneak", false);
            if(!nowDash)
                applySpeed = defaultSpeed;
        }

        // Crouch(��ũ����) & GetUp : �Ʒ�Ű�� ������ ��ũ����, �Ʒ�Ű�� ����ä �¿�Ű�� ������ ����.
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isCrawl = true; // ���帮�� ������ true�� �Ѵ�.

            // ��ũ�� ���¿��� ���� ��θ� ���� �� �ֵ��� �ݶ��̴� ũ��� �������� �����Ѵ�.
            capsule.offset = new Vector2(capsule.offset.x, -0.05f);
            capsule.size = new Vector2(capsule.size.x, 0.08f);

            animator.SetBool("isRun", false);
            // ��ũ�� ���¿��� applySpeed�� crawlSpeed��
            applySpeed = crawlSpeed;
            // Crouch �ִϸ��̼�
            animator.SetBool("isCrouch", true);
            // Sneak(����)      
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                animator.SetBool("isCrouch", false);
                animator.SetBool("isSneak", true);
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                animator.SetBool("isCrouch", true);
                animator.SetBool("isSneak", false);
            }
        }

        // �Ʒ�Ű�� ���� Crouch, Sneak ���¸� �����ϰ� Idle ���·� ���ư���.
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isCrawl = false;

            // �ݶ��̴� �������
            capsule.offset = new Vector2(capsule.offset.x, -0.025f);
            capsule.size = new Vector2(capsule.size.x, 0.13f);

            // applySpeed �������
            applySpeed = defaultSpeed;
            animator.SetBool("isCrouch", false);
            animator.SetBool("isSneak", false);
        }
    }

    void MoveHor()
    {
        // ���� �߿� �̵����� �ʴ´�.
        if (isAttack)
            return;

        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > applySpeed) //Right Max Speed
            rigid.velocity = new Vector2(applySpeed, rigid.velocity.y);
        else if (rigid.velocity.x < applySpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(applySpeed * (-1), rigid.velocity.y);
        
    }

    void FallAnim()
    {
        // �߶����� ����(�ӵ��� ����)
        if (rigid.velocity.y < 0 && isFall && !isAttack && !isDie)
        {
            // Fall �ִϸ��̼�
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }
    }

    void WallClimb()
    {
        // RayCast for Wall Climb
        Debug.DrawRay(rigid.position - new Vector2(0, 0.3f), sprite.flipX == true ? Vector3.left : Vector3.right, new Color(0, 1, 0));

        rayHit = Physics2D.Raycast(rigid.position - new Vector2(0, 0.3f), sprite.flipX == true ? Vector3.left : Vector3.right, 0.6f, LayerMask.GetMask("Platform"));

        // ���� ������ ���
        if (rayHit.collider != null)
        {
            // õõ�� �̲��������� ������ up
            rigid.sharedMaterial.friction = wallSlideFriction;

            isWallTouch = true;
            // ��������Ʈ�� ���� �����ǵ��� ������� �𷺼��� �����Ѵ�.
            capsule.direction = CapsuleDirection2D.Vertical;
            capsule.size = new Vector2(0.06f, capsule.size.y);


            // ���� ���� �����ε� �ӷ��� 0�̰ų� ���� -> �̲������� ���̰ų� ���� ���� ����
            if (rigid.velocity.y <= 0)
            {
                // Wall Grab �ִϸ��̼�
                animator.SetBool("isWallGrab", true);
                animator.SetBool("isWallClimb", false);

                // ���� ����� canWallClimb�� true�� ������ְ� �̶����� ���� ���� �� �ִ�.
                canWallClimb = true;
            }

            // ���� ���� �����ε� �ӷ��� ��� -> ��Ÿ�� �ö󰡴� ��
            else if (rigid.velocity.y > 0)
            {
                // Wall Climb �ִϸ��̼�
                animator.SetBool("isWallClimb", true);
                animator.SetBool("isWallGrab", false);
            }

            // canWallClimb�� true�϶� ����Ű�� ������ ����Ÿ�� ���� �ö󰣴�.
            if (Input.GetKey(KeyCode.UpArrow) && canWallClimb)
            {
                rigid.AddForce(Vector2.up, ForceMode2D.Impulse);

                if (rigid.velocity.y > maxClimbSpeed)
                    rigid.velocity = new Vector2(rigid.velocity.x, maxClimbSpeed);
            }
        }
        else
        {
            // ������ �������
            rigid.sharedMaterial.friction = defaultFriction;

            isWallTouch = false;
            // �ݶ��̴� �������
            capsule.direction = CapsuleDirection2D.Horizontal;
            capsule.size = new Vector2(0.11f, capsule.size.y);

            canWallClimb = false;
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
        }
    }
}
