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
    public bool isWallTouch;    // 벽과 맞닿음 체크
    public bool isWallClimb;    // 벽 오름 체크

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
        // Land(착지) 애니메이션
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

    void Jump()
    {
        // Jump: Space를 누르면 점프한다.
        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            // Jump 애니메이션
            animator.SetBool("isJump", true);
            isJump = true;
        }
    }

    void FlipX()
    {
        // 스프라이트 flipX
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            sprite.flipX = true;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            sprite.flipX = false;
    }

    void RunAnim()
    {
        // 엎드리거나 기어가고 있지 않을 때만 Run 애니메이션 재생한다.
        if (!Input.GetKey(KeyCode.DownArrow))
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
    }

    void SneakAnim()
    {
        // Crouch(웅크리기) & GetUp : 아래키를 누르면 웅크리고, 아래키를 누른채 좌우키를 누르면 기어간다.
        if (Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetBool("isRun", false);
            // 웅크린 상태에선 maxSpeed를 감소시킨다.
            maxSpeed = 1.5f;
            // Crouch 애니메이션
            animator.SetBool("isCrouch", true);
            // Sneak(기어가기)
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

        // 아래키를 떼면 Crouch, Sneak 상태를 해제하고 Idle 상태로 돌아간다.
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            // maxSpeed 원래대로
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
        // 추락중일 때는(속도가 음수) (벽과 안닿았을때만)
        if (rigid.velocity.y < 0 && !isWallTouch)
        {
            // Fall 애니메이션
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }
    }

    void WallClimb()
    {
        // 벽과 닿은 상태(isWallTouch)이면 isWallClimb이 true가 되어 벽타기가 가능해진다.
        if (isWallTouch)
        {
            isWallClimb = true;

            // 벽과 닿은 상태인데 속력이 0이거나 음수 -> 미끄러지는 중이거나 벽과 닿은 상태
            if (rigid.velocity.y <= 0)
            {
                // Wall Grab 애니메이션
                animator.SetBool("isWallGrab", true);
                animator.SetBool("isWallClimb", false);
            }

            // 벽과 닿은 상태인데 속력이 양수 -> 벽타고 올라가는 중
            else if (rigid.velocity.y > 0)
            {
                // Wall Climb 애니메이션
                animator.SetBool("isWallClimb", true);
                animator.SetBool("isWallGrab", false);
            }
        }
        // 벽과 떨어졌을 때
        else
        {
            isWallClimb = false;
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
        }

        // isWallClimb이 true일때 위쪽키를 누르면 벽을타고 위로 올라간다.
        if (Input.GetKey(KeyCode.UpArrow) && isWallClimb)
        {
            rigid.AddForce(Vector2.up, ForceMode2D.Impulse);

            if (rigid.velocity.y > maxClimbSpeed)
                rigid.velocity = new Vector2(rigid.velocity.x, maxClimbSpeed);
        }
    }
}
