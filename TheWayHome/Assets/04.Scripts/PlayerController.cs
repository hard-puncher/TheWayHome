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
        // canDash가 false 이면 시간을 누적하여 쿨타임 3초가 지나면 대쉬가 가능해진다.
        if(!canDash)
        {
            dashDelay += Time.deltaTime;
            if(dashDelay >= 3f)
            {
                dashDelay = 0f;
                canDash = true;
            }
        }

        // Jump: Space를 누르면 점프한다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            // Jump 애니메이션
            animator.SetBool("isJump", true);
        }
            
        // 스프라이트 flipX
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            sprite.flipX = true;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            sprite.flipX = false;

        // 엎드리거나 기어가고 있지 않을 때만 Run 애니메이션 재생한다.
        if(!Input.GetKey(KeyCode.DownArrow))
        {
            // Idle -> Run
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                animator.SetBool("isRun", true);

                // Dash: Run 상태에서 canDash일때 shift를 누르면 실행
                if (canDash && Input.GetKeyDown(KeyCode.LeftShift))
                {
                    animator.SetTrigger("isDash");
                    canDash = false;
                    Dash(); // maxSpeed를 일시적으로 늘려 빠르게 달리는 것처럼 보이게하는 함수
                }        
            }
                
            // Run -> Idle
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                animator.SetBool("isRun", false);
        }      

        // Crouch(웅크리기) & GetUp : 아래키를 누르면 웅크리고, 아래키를 누른채 좌우키를 누르면 기어간다.
        if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isRun", false);
            // 웅크린 상태에선 maxSpeed를 감소시킨다.
            maxSpeed = 1.5f;
            // Crouch 애니메이션
            animator.SetBool("isCrouch", true);
            // Sneak(기어가기)
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
           
        // 아래키를 떼면 Crouch, Sneak 상태를 해제하고 Idle 상태로 돌아간다.
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            // maxSpeed 원래대로
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


        // 추락중일 때는(속도가 음수)
        if (rigid.velocity.y < 0)
        {
            // Fall 애니메이션
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }    
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Land(착지) 애니메이션
        if(collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isFall", false);
            animator.SetTrigger("isLand");
        }
    }

    // Dash 상태일 때는 최대속도를 2배 늘려 빠르게 이동
    void Dash()
    {
        maxSpeed = 9f;
        Invoke("DashOff", 0.833f);  // Dash 애니메이션 길이만큼 invoke를 걸어준다.
    }

    void DashOff()
    {
        maxSpeed = 4.5f;
    }
}
