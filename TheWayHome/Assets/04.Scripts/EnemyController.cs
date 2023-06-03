using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // 필요한 컴포넌트
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer sprite;
    BoxCollider2D box;

    // 콜라이더 오프셋 조절을 위한 변수(왼쪽으로 이동할때는 offsetX를 반전시켜줘야함)
    Vector2 originOffset;
    Vector2 leftOffset;

    public int nextMove;    // 이동 방향 변수
    public int enemySpeed;  // 실제 이동 속도
    public int enemyDefaultSpeed = 2;   // 기본 걸음 속도
    public int enemyPlayerFindSpeed = 5;    // 플레이어 발견시 속도
    public int enemyDamage; // 적 공격력

    public bool isFindPlayer;   // 플레이어가 레이에 닿은 경우 TRUE

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();

        originOffset = new Vector2(box.offset.x, box.offset.y);
        leftOffset = new Vector2(-box.offset.x, box.offset.y);

        Invoke("Think", 5f);
    }

    
    void FixedUpdate()
    {
        // 콜라이더 오프셋 조절
        box.offset = sprite.flipX == true ? leftOffset : originOffset;

        Move(); // 지형 체크    
        FindPlayer();   // 플레이어 체크
        // 이동 속도
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y) * enemySpeed;
    }

    // 재귀 함수
    void Think()
    {
        // 다음 이동 방향
        nextMove = Random.Range(-1, 2);

        // 애니메이션
        animator.SetBool("isWalk", nextMove == 0 ? false : true);
        
        // Sprite FlipX
        if (nextMove != 0)
            sprite.flipX = nextMove == -1;

        // 다음 이동 주기
        float nextThinkTime = Random.Range(5f, 10f); 
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove = -nextMove;
        sprite.flipX = nextMove == -1;

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
        Debug.DrawRay(rigid.position, sprite.flipX == true ? Vector3.left : Vector3.right, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + new Vector2(0f, -1.5f), sprite.flipX == true ? Vector3.left : Vector3.right, 6, LayerMask.GetMask("Player"));

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
            animator.SetBool("isRun", false);
    }
}
