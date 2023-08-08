using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Inventory inventory;

    // 필요한 컴포넌트
    Rigidbody2D rigid;
    CapsuleCollider2D capsule;
    SpriteRenderer sprite;
    Animator animator;

    [Header("Flag")]
    public bool isJump; // 점프 중인지 여부
    public bool isGround;   // 바닥에 있는지 여부
    public bool canDash = true;
    public bool isDash; // 점프중 대쉬 방지용
    public bool nowDash;    // 대쉬 중인가?
    public bool isFall; // 벽에 닿았을 때 rigid.velocity가 왜인진 모르겠으나 음수가 되어 fall 애니메이션이 실행되는 현상 발생, 벽과 닿았을 때는 isFall을 false로 해서 해결
    public bool isAttack;   // 공격중인지 체크
    public bool canAttack;  // 공격주기가 돌아왔는지 체크
    public bool isDie;
    public bool isInvincible;   // 무적상태 체크, true일땐 피격당하지 않는다.
    public bool isHide; // 현재 엄폐중인지 체크, 엄폐중일땐 점프 X
    public bool isCrawl;    // 엎드리고 있는지 여부(엎드리고 있으면 적 뒤통수 범위에 있을 때 알아차리지 못하게 할것)

    // Wall Climb
    private RaycastHit2D rayHit;
    public bool isWallTouch;    // 벽과 맞닿음 체크
    public bool canWallClimb;   // isWallTouch가 true가 되면 canWallClimb이 true가 되어 벽을 오를 수 있다.
   
    // 머리 위에 플랫폼이 맞닿아있는지 체크
    private RaycastHit2D rayHitSneak;

    [Space(10f)]
    [Header("Status By Character")]
    public float defaultSpeed = 6f;  // 캐릭터별 이동속도
    public float dashSpeed = 12f; // 캐릭터별 대쉬속도
    public float crawlSpeed = 3f;    // 캐릭터별 포복 속도
    public float applySpeed; // 실제로 적용될 속도
   
    public float jumpPower; // 점프력
    
    public float dashDelay; // 캐릭터별 대쉬주기
    public float dashPower; // 캐릭터별 대쉬파워(defaultSpeed에 곱해줄 값)
    public float maxClimbSpeed; // 캐릭터별 벽 오름 속도
   
    public float attackDelay;   // 캐릭터별 공격 주기
    
    public float knockBackPower;    // 캐릭터별 넉백당하는 정도

    // 캐릭터별 최대 체력.
    public int maxHp;
    
    [Space(10f)]
    [Header("Common")] 
    public GameObject attackRange;  // 공격 범위 게임오브젝트
    public float defaultFriction = 0.03f;   // 기본 마찰력(계단을 부드럽게 올라가는 정도)
    public float wallSlideFriction = 0.3f; // 벽에 닿았을 때 마찰력(잘 미끄러지지 않게끔)
    public int jumpCount;   // 남은 점프 가능 횟수
    public int maxJumpCount;    // 최대 점프 가능 횟수(기본 2회)
   
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        inventory = GameObject.Find("InventoryGroup").GetComponent<Inventory>();
    }
    
    // 게임 시작후 캐릭터가 활성화될 때
    private void OnEnable()
    {
        // 게임 시작 시 메인카메라를 찾아서 플레이어 자식으로 연결
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.SetParent(gameObject.transform);
        
        // 시작 시 플레이어 위치 지정
        this.transform.position = new Vector3(-15, -3.2f, 0);
    }

    void Update()
    {
        if (isGround)
        {
            animator.SetBool("isFall", false);
        }
            
        CheckDie();

        // 대쉬 딜레이 측정
        if (!canDash)
        {
            dashDelay += Time.deltaTime;
            if(dashDelay >= 3f)
            {
                dashDelay = 0f;
                canDash = true;
            }
        }
        // 공격 딜레이 측정
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
        FallAnim(); // 벽타다가 죽었을 경우 떨어지긴 해야하므로 FallAnim()은 isDie일 때도 실행한다.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isDash = true;
        isFall = false;

        // 땅(벽)과 닿았을 때
        if(collision.gameObject.tag == "Ground")
        {
            isJump = false;
            isGround = true;
            jumpCount = 0;
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }

        // 떨어지는 장판과 닿았을 때
        if(collision.gameObject.tag == "FallingSheet")
        {
            isJump = false;
            isGround = true;
            jumpCount = 0;
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }

        // 무적상태가 아니면서 적과 부딪혔을 때
        if (collision.gameObject.tag == "Enemy" && !isInvincible)
        {
            // 피격 사운드 재생
            SoundManager.instance.PlaySE("Hit");

            // 적 공격력만큼 체력 감소
            GameManager.Instance.DecreaseHP(collision.gameObject.GetComponent<EnemyController>().enemyDamage);

            CheckDie();

            // 넉백 -> 부딪힌 적의 x좌표가 플레이어보다 오른쪽에 있으면 왼쪽으로 튕기고, 왼쪽에 있으면 오른쪽으로 튕긴다.
            rigid.AddForce(collision.gameObject.GetComponent<Rigidbody2D>().position.x > rigid.position.x ? Vector2.left * knockBackPower : Vector2.right * knockBackPower, ForceMode2D.Impulse);

            // 무적상태화
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
        // 플레이어가 트리거 상태에서 땅과 충돌 -> 엄폐 상태이므로 추락 방지를 위해 속도 0
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
            rigid.velocity = new Vector2(0, 0); // y속도를 0으로 만들지 않으면 fall 애니메이션이 재생된다.
            rigid.isKinematic = true;   // 추락 멈춘다.
            GameManager.Instance.DecreaseHP(GameManager.Instance.curHp);
            Debug.Log("맵 바깥으로 떨어졌습니다. 게임 오버입니다.");
            CheckDie();
        }

        // 점프 아이템과 충돌 시 isGround를 true로 해서 2단점프가 되게 함.
        if(collision.gameObject.tag == "JumpStone")
        {
            isGround = true;
            if (jumpCount <= 0)
                return;
            jumpCount--;
        }
    }

    // 무적 상태
    public void Invincible()
    {
        sprite.color = new Color(1, 1, 1, 0.4f);
        isInvincible = true;
     
        Invoke("OffInvincible", 1f);    // 1초간 무적
    }
    // 무적 상태 해제
    public void OffInvincible()
    {
        sprite.color = new Color(1, 1, 1, 1f);
        isInvincible = false;
    }
    // 사망 체크
    public void CheckDie()
    {
        if (GameManager.Instance.curHp <= 0 && !isDie)
        {
            isDie = true;

            // 땅에 착지한 상태로 사망한 경우만 그자리에서 멈춘다.
            if(isGround && !isWallTouch)
            {
                //rigid.velocity = Vector2.zero;
                capsule.enabled = false;
                rigid.isKinematic = true;
            }

            // 다른 애니메이션 올 스탑
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
        GameManager.Instance.gameOverUI.SetActive(true);    // 게임오버 ui 활성화
    }

    // 공격
    private void Attack()
    {
        if(canAttack && Input.GetKeyDown(KeyCode.LeftControl))
        {
            canAttack = false;
            isAttack = true;
            animator.SetTrigger("isAttack");
            animator.SetBool("isFall", false);
            attackRange.SetActive(true);
            // 공격을 할 때 플레이어 스프라이트 flipX가 True이면 attackRange의 콜라이더는 살짝 왼쪽에, False이면 오른쪽에 그대로 활성화한다.
            attackRange.GetComponent<BoxCollider2D>().offset = sprite.flipX == true ? new Vector2(-0.18f, 0f) : new Vector2(0f, 0f);
            Invoke("AttackOff", 0.5f);
        }
    }

    private void AttackOff()
    {
        isAttack = false;
        attackRange.SetActive(false);
    }

    // Dash 상태일 때는 최대속도를 2배 늘려 빠르게 이동
    void Dash()
    {
        nowDash = true;
        // 대쉬 사운드 재생
        SoundManager.instance.PlaySE("Dash");
        applySpeed = dashSpeed;  // 적용 속도는 대쉬 속도로
        Invoke("DashOff", 0.833f);  // Dash 애니메이션 길이만큼 invoke를 걸어준다.
    }

    void DashOff()
    {
        nowDash = false;
        applySpeed = defaultSpeed;
    }

    // 점프
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            // Jump 애니메이션
            animator.SetBool("isJump", true);
            animator.SetBool("isFall", false);

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
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

                // 공중에 떠 있을때, 기어가고 있을 때, 점프 중일 때는 대쉬 X
                // Dash: Run 상태에서 canDash일때 shift를 누르면 실행
                if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && !isFall && !isCrawl && !isJump)
                {
                    animator.SetTrigger("isDash");
                    canDash = false;
                    // 땅에 닿아 있고 엎드리거나 포복 중이 아닐 때만 대쉬 가능
                    if(isGround && rayHitSneak.collider == null)
                        Dash(); // defaultSpeed를 일시적으로 늘려 빠르게 달리는 것처럼 보이게하는 함수
                }
            }

            // Run -> Idle
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                animator.SetBool("isRun", false);
        }
    }

    void SneakAnim()
    {
        // 통로 기어가기 중 일어서는 오류를 방지하기 위해 플레이어 기준 위쪽으로 레이를 쏴서, 위에 플랫폼이 있다면 일어날 수 없게 한다.
        Debug.DrawRay(rigid.position, Vector3.up, new Color(0, 1, 0));

        rayHitSneak = Physics2D.Raycast(rigid.position, Vector3.up, 0.6f, LayerMask.GetMask("Platform"));

        if(rayHitSneak.collider != null)
        {
            isCrawl = true; // 엎드리고 있음을 true로 한다.

            // 웅크린 상태에선 좁은 통로를 지날 수 있도록 콜라이더 크기와 오프셋을 조절한다.
            capsule.offset = new Vector2(capsule.offset.x, -0.05f);
            capsule.size = new Vector2(capsule.size.x, 0.08f);

            animator.SetBool("isRun", false);

            // 웅크린 상태에선 applySpeed를 crawlSpeed로
           // if(!nowDash)
            applySpeed = crawlSpeed;

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
        else
        {
            isCrawl = false;

            // 콜라이더 원래대로
            capsule.offset = new Vector2(capsule.offset.x, -0.025f);
            capsule.size = new Vector2(capsule.size.x, 0.13f);

            animator.SetBool("isCrouch", false);
            animator.SetBool("isSneak", false);
            if(!nowDash)
                applySpeed = defaultSpeed;
        }

        // Crouch(웅크리기) & GetUp : 아래키를 누르면 웅크리고, 아래키를 누른채 좌우키를 누르면 기어간다.
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isCrawl = true; // 엎드리고 있음을 true로 한다.

            // 웅크린 상태에선 좁은 통로를 지날 수 있도록 콜라이더 크기와 오프셋을 조절한다.
            capsule.offset = new Vector2(capsule.offset.x, -0.05f);
            capsule.size = new Vector2(capsule.size.x, 0.08f);

            animator.SetBool("isRun", false);
            // 웅크린 상태에선 applySpeed를 crawlSpeed로
            applySpeed = crawlSpeed;
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
            isCrawl = false;

            // 콜라이더 원래대로
            capsule.offset = new Vector2(capsule.offset.x, -0.025f);
            capsule.size = new Vector2(capsule.size.x, 0.13f);

            // applySpeed 원래대로
            applySpeed = defaultSpeed;
            animator.SetBool("isCrouch", false);
            animator.SetBool("isSneak", false);
        }
    }

    void MoveHor()
    {
        // 공격 중엔 이동하지 않는다.
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
        // 추락중일 때는(속도가 음수)
        if (rigid.velocity.y < 0 && isFall && !isAttack && !isDie)
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

        rayHit = Physics2D.Raycast(rigid.position - new Vector2(0, 0.3f), sprite.flipX == true ? Vector3.left : Vector3.right, 0.6f, LayerMask.GetMask("Platform"));

        // 벽을 마주한 경우
        if (rayHit.collider != null)
        {
            // 천천히 미끄러지도록 마찰력 up
            rigid.sharedMaterial.friction = wallSlideFriction;

            isWallTouch = true;
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
            // 마찰력 원래대로
            rigid.sharedMaterial.friction = defaultFriction;

            isWallTouch = false;
            // 콜라이더 원래대로
            capsule.direction = CapsuleDirection2D.Horizontal;
            capsule.size = new Vector2(0.11f, capsule.size.y);

            canWallClimb = false;
            animator.SetBool("isWallClimb", false);
            animator.SetBool("isWallGrab", false);
        }
    }
}
