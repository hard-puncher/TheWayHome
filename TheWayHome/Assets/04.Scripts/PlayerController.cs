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
    public bool isFall; // 벽에 닿았을 때 rigid.velocity가 왜인진 모르겠으나 음수가 되어 fall 애니메이션이 실행되는 현상 발생, 벽과 닿았을 때는 isFall을 false로 해서 해결

    // Wall Climb
    public bool isWallTouch;    // 벽과 맞닿음 체크
    public bool canWallClimb;    // 벽 오름 체크

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 게임 시작 시 메인카메라를 찾아서 플레이어 자식으로 연결
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.SetParent(gameObject.transform);

        // 게임 시작시 배경화면을 플레이어 자식으로 연결
        GameObject backGround_Day = GameObject.FindGameObjectWithTag("BG_Day");
        backGround_Day.transform.SetParent(gameObject.transform);
        GameObject backGround_Night = GameObject.FindGameObjectWithTag("BG_Night");
        backGround_Night.transform.SetParent(gameObject.transform);

        // 게임 시작 시 현실 시간이 6~18시라면 낮 배경을, 18~6시라면 밤 배경을 활성화한다.
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

        // 시작 시 플레이어 위치 지정
        this.transform.position = new Vector3(-9, -3.2f, 0);
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
        isDash = true;
        isFall = false;

        // Land(착지)
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

    // Dash 상태일 때는 최대속도를 2배 늘려 빠르게 이동
    void Dash()
    {
        // 공중에 떠 있을 때는 대쉬를 멈춤.
        if (!isDash)
            return;
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
                    if(!isFall)
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
            // 웅크린 상태에선 좁은 통로를 지날 수 있도록 콜라이더 크기와 오프셋을 조절한다.
            capsule.offset = new Vector2(capsule.offset.x, -0.05f);
            capsule.size = new Vector2(capsule.size.x, 0.08f);

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
            // 콜라이더 원래대로
            capsule.offset = new Vector2(capsule.offset.x, -0.025f);
            capsule.size = new Vector2(capsule.size.x, 0.13f);

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
        // 추락중일 때는(속도가 음수)
        if (rigid.velocity.y < 0 && isFall)
        {
            // Fall 애니메이션
            animator.SetBool("isJump", false);
            animator.SetBool("isFall", true);
        }
    }

    void WallClimb()
    {
        // RayCast for Wall Climb
        Debug.DrawRay(rigid.position - new Vector2(0, 0.3f), sprite.flipX == true ? Vector3.left : Vector3.right, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position - new Vector2(0, 0.3f), sprite.flipX == true ? Vector3.left : Vector3.right, 0.6f, LayerMask.GetMask("Platform"));

        // 벽을 마주한 경우
        if (rayHit.collider != null)
        {
            // 스프라이트가 벽에 밀착되도록 사이즈와 디렉션을 조절한다.
            capsule.direction = CapsuleDirection2D.Vertical;
            capsule.size = new Vector2(0.06f, capsule.size.y);

            // 벽과 닿은 상태인데 속력이 0이거나 음수 -> 미끄러지는 중이거나 벽과 닿은 상태
            if (rigid.velocity.y <= 0)
            {
                // Wall Grab 애니메이션
                animator.SetBool("isWallGrab", true);
                animator.SetBool("isWallClimb", false);

                // 벽과 닿고나서 canWallClimb을 true로 만들어주고 이때부터 벽을 오를 수 있다.
                canWallClimb = true;
            }

            // 벽과 닿은 상태인데 속력이 양수 -> 벽타고 올라가는 중
            else if (rigid.velocity.y > 0)
            {
                // Wall Climb 애니메이션
                animator.SetBool("isWallClimb", true);
                animator.SetBool("isWallGrab", false);
            }

            // canWallClimb이 true일때 위쪽키를 누르면 벽을타고 위로 올라간다.
            if (Input.GetKey(KeyCode.UpArrow) && canWallClimb)
            {
                rigid.AddForce(Vector2.up, ForceMode2D.Impulse);

                if (rigid.velocity.y > maxClimbSpeed)
                    rigid.velocity = new Vector2(rigid.velocity.x, maxClimbSpeed);
            }
        }
        else
        {
            // 콜라이더 원래대로
            capsule.direction = CapsuleDirection2D.Horizontal;
            capsule.size = new Vector2(0.11f, capsule.size.y);

            canWallClimb = false;
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
        }
    }
}
