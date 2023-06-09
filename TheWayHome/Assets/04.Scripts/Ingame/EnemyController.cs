using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // 필요한 컴포넌트
    Rigidbody2D rigid;
    Animator animator;
    BoxCollider2D box;
    NoticePlayer noticePlayer;  // 자식 오브젝트의 컴포넌트

    Vector3 originLocalScale;   // 스프라이트 flipX대신 localScale로 뒤집기 위해 원래 로컬 스케일을 저장해둔다.
                                // (앞은 ray로 감지 중이므로 뒤통수에만 감지 콜라이더를 달 것이다. flipX로 하면 자식 객체인 레이더는 안뒤집어지므로 localScale로 뒤집는 것

    // 콜라이더 오프셋 조절을 위한 변수(왼쪽으로 이동할때는 offsetX를 반전시켜줘야함)
    Vector2 originOffset;
    Vector2 leftOffset;

    public int nextMove;    // 이동 방향 변수
    public int enemySpeed;  // 실제 이동 속도
    public int enemyDefaultSpeed = 2;   // 기본 걸음 속도
    public int enemyPlayerFindSpeed = 5;    // 플레이어 발견시 속도
    public int enemyDamage; // 적 공격력

    public bool isFindPlayer;   // 플레이어가 레이에 닿은 경우 TRUE

    RaycastHit2D rayHit;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        noticePlayer = GetComponentInChildren<NoticePlayer>();  // 자식 오브젝트의 컴포넌트를 연결

        originLocalScale = this.transform.localScale;

        originOffset = new Vector2(box.offset.x, box.offset.y);
        leftOffset = new Vector2(-box.offset.x, box.offset.y);

        Invoke("Think", 5f);
    }

    private void Update()
    {
        NoticeFromBehind(); // 적 뒤에서 플레이어가 머무를 경우 플레이어를 인식하고 쫒아가는 함수(은신, 엎드리기 상태일 때 제외)
    }


    void FixedUpdate()
    {
        Move(); // 지형 체크    
        FindPlayer();   // 플레이어 체크
        // 이동 속도
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y) * enemySpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어와 부딪힌 경우에도 탐지 했다고 보고 즉시 플레이어 방향으로 달려간다.
        if(collision.gameObject.tag == "Player")
        {
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
            isFindPlayer = true;
            enemySpeed = enemyPlayerFindSpeed;  // 플레이어 발견시 2배속으로 이동
            Debug.Log("플레이어 발견!");

            // 플레이어가 왼쪽에 있는 경우
            if (collision.transform.position.x < rigid.position.x)
            {
                //sprite.flipX = true;
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = -1;
                isFindPlayer = false;
            }
            // 플레이어가 오른쪽에 있는 경우
            else if (collision.transform.position.x > rigid.position.x)
            {
                //sprite.flipX = false;
                this.transform.localScale = originLocalScale;
                nextMove = 1;
                isFindPlayer = false;
            }
        }
    }

    // 재귀 함수
    void Think()
    {
        // 다음 이동 방향
        nextMove = Random.Range(-1, 2);

        // Sprite FlipX
        if (nextMove != 0)
        {
            if(nextMove == -1)
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
            else
                this.transform.localScale = originLocalScale;
        }
         
        // 다음 이동 주기
        float nextThinkTime = Random.Range(5f, 10f); 
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove = -nextMove;
        if (nextMove == -1)
            this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
        else
            this.transform.localScale = originLocalScale;
     
        CancelInvoke();
        Invoke("Think", 5f);
    }

    // 지형 체크
    private void Move()
    {
        enemySpeed = enemyDefaultSpeed; // 이동 시엔 기본 속도로 이동
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHitPlatform = Physics2D.Raycast(frontVec, Vector3.down, 3, LayerMask.GetMask("Platform"));
        if (rayHitPlatform.collider == null)
        {
            Turn();
        }
    }

    // 플레이어 탐색
    void FindPlayer()
    {
        Debug.DrawRay(rigid.position, this.transform.localScale != originLocalScale ? Vector3.left : Vector3.right, new Color(0, 1, 0));
        rayHit = Physics2D.Raycast(rigid.position + new Vector2(0f, -1.5f), this.transform.localScale != originLocalScale ? Vector3.left : Vector3.right, 6, LayerMask.GetMask("Player"));

        // 플레이어가 레이에 닿았으면
        if (rayHit.collider != null && !isFindPlayer)
        {
            CancelInvoke();
            animator.SetBool("isRun",  true);
            isFindPlayer = true;
            enemySpeed = enemyPlayerFindSpeed;  // 플레이어 발견시 2배속으로 이동
            Debug.Log("플레이어 발견!");
            
            // 플레이어가 왼쪽에 있는 경우
            if (rayHit.collider.transform.position.x < rigid.position.x)
            {
                nextMove = -1;
                isFindPlayer = false;
            }
            // 플레이어가 오른쪽에 있는 경우
            else if (rayHit.collider.transform.position.x > rigid.position.x)
            {
                nextMove = 1;
                isFindPlayer = false;
            }
        }
        else
        {
            animator.SetBool("isRun", false);
            if(nextMove != 0)
                animator.SetBool("isWalk", true);
            else
                animator.SetBool("isWalk", false);
        }         
    }

    // 자식 오브젝트인 noticeRange에서 플레이어가 n초 이상 머물러 발각 되었을 때 플레이어에게 돌진하는 함수
    void NoticeFromBehind()
    {
        // 자식 오브젝트에서 플레이어를 탐지했다면
        if (noticePlayer.isNotice)
        {
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
            isFindPlayer = true;
            enemySpeed = enemyPlayerFindSpeed;  // 플레이어 발견시 2배속으로 이동
            Debug.Log("플레이어 발견!");

            // 뒤에서 감지했으므로 플레이어는 무조건 적 뒤에 있다. 
            // 오른쪽 바라보는 중 -> 왼쪽으로 돌격
            if (this.transform.localScale == originLocalScale)
            {
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = -1;  // 왼쪽
                isFindPlayer = false;
                noticePlayer.isNotice = false;  // 다시 자식 오브젝트의 플래그를 false로
            }
            // 왼쪽 바라보는 중 -> 오른쪽으로 돌격
            else
            {
                this.transform.localScale = originLocalScale;
                nextMove = 1;   // 왼쪽
                isFindPlayer = false;
                noticePlayer.isNotice = false;  // 다시 자식 오브젝트의 플래그를 false로
            }
        }
    }
}
