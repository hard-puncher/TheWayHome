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
    public float dashDelay;
    public bool canDash = true;
    public float dashPower;

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

        // Jump: Space�� ������ �����Ѵ�.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            // Jump �ִϸ��̼�
            animator.SetBool("isJump", true);
        }
            
        // ��������Ʈ flipX
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            sprite.flipX = true;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            sprite.flipX = false;

        // ���帮�ų� ���� ���� ���� ���� Run �ִϸ��̼� ����Ѵ�.
        if(!Input.GetKey(KeyCode.DownArrow))
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

        // Crouch(��ũ����) & GetUp : �Ʒ�Ű�� ������ ��ũ����, �Ʒ�Ű�� ����ä �¿�Ű�� ������ ����.
        if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isRun", false);
            // ��ũ�� ���¿��� maxSpeed�� ���ҽ�Ų��.
            maxSpeed = 1.5f;
            // Crouch �ִϸ��̼�
            animator.SetBool("isCrouch", true);
            // Sneak(����)
            if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                animator.SetBool("isCrouch", false);
                animator.SetBool("isSneak", true);
            }

            if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
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

    void FixedUpdate()
    {
        // Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) //Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);


        // �߶����� ����(�ӵ��� ����)
        if (rigid.velocity.y < 0)
        {
            // Fall �ִϸ��̼�
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }    
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Land(����) �ִϸ��̼�
        if(collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isFall", false);
            animator.SetTrigger("isLand");
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
}
