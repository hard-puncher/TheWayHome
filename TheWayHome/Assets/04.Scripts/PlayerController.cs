using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float dashPower;
    public float maxClimbSpeed;

    // Wall Climb
    public bool isWallTouch;    // ���� �´��� üũ
    public bool isWallClimb;    // �� ���� üũ

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        // Land(����) �ִϸ��̼�
        if(collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isFall", false);
            animator.SetTrigger("isLand");
            isJump = false;
        }

        if(collision.gameObject.tag == "Wall")
        {
            isWallTouch = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            isWallTouch = false;
            animator.SetBool("isWallGrab", false);
            animator.SetBool("isWallClimb", false);
        }
    }

    // Dash ������ ���� �ִ�ӵ��� 2�� �÷� ������ �̵�
    void Dash()
    {
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
        // �߶����� ����(�ӵ��� ����) (���� �ȴ��������)
        if (rigid.velocity.y < 0 && !isWallTouch)
        {
            // Fall �ִϸ��̼�
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }
    }

    void WallClimb()
    {
        // ���� ���� ����(isWallTouch)�̸� isWallClimb�� true�� �Ǿ� ��Ÿ�Ⱑ ����������.
        if (isWallTouch)
        {
            isWallClimb = true;

            // ���� ���� �����ε� �ӷ��� 0�̰ų� ���� -> �̲������� ���̰ų� ���� ���� ����
            if (rigid.velocity.y <= 0)
            {
                // Wall Grab �ִϸ��̼�
                animator.SetBool("isWallGrab", true);
                animator.SetBool("isWallClimb", false);
            }

            // ���� ���� �����ε� �ӷ��� ��� -> ��Ÿ�� �ö󰡴� ��
            else if (rigid.velocity.y > 0)
            {
                // Wall Climb �ִϸ��̼�
                animator.SetBool("isWallClimb", true);
                animator.SetBool("isWallGrab", false);
            }
        }
        // ���� �������� ��
        else
        {
            isWallClimb = false;
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
        }

        // isWallClimb�� true�϶� ����Ű�� ������ ����Ÿ�� ���� �ö󰣴�.
        if (Input.GetKey(KeyCode.UpArrow) && isWallClimb)
        {
            rigid.AddForce(Vector2.up, ForceMode2D.Impulse);

            if (rigid.velocity.y > maxClimbSpeed)
                rigid.velocity = new Vector2(rigid.velocity.x, maxClimbSpeed);
        }
    }
}
