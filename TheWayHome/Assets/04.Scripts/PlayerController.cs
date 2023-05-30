using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    SpriteRenderer sprite;
    Animator animator;

    [Space(10f)]
    [Header("Status")]
    public float maxSpeed;
    public float jumpPower;
    public bool isJump;
    public float dashDelay;
    public bool canDash = true;
    public bool isDash;
    public float dashPower;
    public float maxClimbSpeed;
    public bool isFall; // ���� ����� �� rigid.velocity�� ������ �𸣰����� ������ �Ǿ� fall �ִϸ��̼��� ����Ǵ� ���� �߻�, ���� ����� ���� isFall�� false�� �ؼ� �ذ�

    // Wall Climb
    public bool isWallTouch;    // ���� �´��� üũ
    public bool canWallClimb;    // �� ���� üũ

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // ���� ���� �� ����ī�޶� ã�Ƽ� �÷��̾� �ڽ����� ����
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.SetParent(gameObject.transform);

        // ���� ���۽� ���ȭ���� �÷��̾� �ڽ����� ����
        GameObject backGround_Day = GameObject.FindGameObjectWithTag("BG_Day");
        backGround_Day.transform.SetParent(gameObject.transform);
        GameObject backGround_Night = GameObject.FindGameObjectWithTag("BG_Night");
        backGround_Night.transform.SetParent(gameObject.transform);

        // ���� ���� �� ���� �ð��� 6~18�ö�� �� �����, 18~6�ö�� �� ����� Ȱ��ȭ�Ѵ�.
        Debug.Log(DateTime.Now.Hour);
        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 18)
        {        
            backGround_Day.SetActive(true);
            backGround_Night.SetActive(false);
        }
        else if ((DateTime.Now.Hour >= 18 && DateTime.Now.Hour <= 24)
            || DateTime.Now.Hour < 6)
        {
            backGround_Day.SetActive(false);
            backGround_Night.SetActive(true);
        }

        // ���� �� �÷��̾� ��ġ ����
        this.transform.position = new Vector3(-9, -3.2f, 0);
    }

    void Update()
    {
        // canDash�� false �̸� �ð��� �����Ͽ� ��Ÿ�� 3�ʰ� ������ �뽬�� ����������.
        if(!canDash)
        {
            dashDelay += Time.deltaTime;
            if(dashDelay >= 3f)
            {
                dashDelay = 0f;
                canDash = true;
            }
        }

        Jump();
        FlipX();
        RunAnim();
        SneakAnim();    
    }

    void FixedUpdate()
    {
        MoveHor();
        FallAnim();
        WallClimb();         
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isDash = true;
        isFall = false;

        // Land(����)
        if(collision.gameObject.tag == "Ground")
        {
            isJump = false;
            animator.SetBool("isFall", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isFall = true;
        isDash = false;
    }

    // Dash ������ ���� �ִ�ӵ��� 2�� �÷� ������ �̵�
    void Dash()
    {
        // ���߿� �� ���� ���� �뽬�� ����.
        if (!isDash)
            return;
        maxSpeed = 9f;
        Invoke("DashOff", 0.833f);  // Dash �ִϸ��̼� ���̸�ŭ invoke�� �ɾ��ش�.
    }

    void DashOff()
    {
        maxSpeed = 4.5f;
    }

    void Jump()
    {
        // Jump: Space�� ������ �����Ѵ�.
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            // Jump �ִϸ��̼�
            animator.SetBool("isJump", true);
            isJump = true;
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

                // Dash: Run ���¿��� canDash�϶� shift�� ������ ����
                if (canDash && Input.GetKeyDown(KeyCode.LeftShift))
                {
                    animator.SetTrigger("isDash");
                    canDash = false;
                    if(!isFall)
                        Dash(); // maxSpeed�� �Ͻ������� �÷� ������ �޸��� ��ó�� ���̰��ϴ� �Լ�
                }
            }

            // Run -> Idle
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                animator.SetBool("isRun", false);
        }
    }

    void SneakAnim()
    {
        // Crouch(��ũ����) & GetUp : �Ʒ�Ű�� ������ ��ũ����, �Ʒ�Ű�� ����ä �¿�Ű�� ������ ����.
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // ��ũ�� ���¿��� ���� ��θ� ���� �� �ֵ��� �ݶ��̴� ũ��� �������� �����Ѵ�.
            capsule.offset = new Vector2(capsule.offset.x, -0.05f);
            capsule.size = new Vector2(capsule.size.x, 0.08f);

            animator.SetBool("isRun", false);
            // ��ũ�� ���¿��� maxSpeed�� ���ҽ�Ų��.
            maxSpeed = 1.5f;
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
            // �ݶ��̴� �������
            capsule.offset = new Vector2(capsule.offset.x, -0.025f);
            capsule.size = new Vector2(capsule.size.x, 0.13f);

            // maxSpeed �������
            maxSpeed = 4.5f;
            animator.SetBool("isCrouch", false);
            animator.SetBool("isSneak", false);
        }
    }

    void MoveHor()
    {
        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
    }

    void FallAnim()
    {
        // �߶����� ����(�ӵ��� ����)
        if (rigid.velocity.y < 0 && isFall)
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

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position - new Vector2(0, 0.3f), sprite.flipX == true ? Vector3.left : Vector3.right, 0.6f, LayerMask.GetMask("Platform"));

        // ���� ������ ���
        if (rayHit.collider != null)
        {
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
            // �ݶ��̴� �������
            capsule.direction = CapsuleDirection2D.Horizontal;
            capsule.size = new Vector2(0.11f, capsule.size.y);

            canWallClimb = false;
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
        }
    }
}
