using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // 필요한 컴포넌트
    Rigidbody2D rigid;
    Animator animator;
    BoxCollider2D box;
    NoticePlayer noticePlayer;  // 자식 오브젝트의 컴포넌트

    Vector3 originLocalScale;   // 스프라이트 flipX대신 localScale로 뒤집기 위해 원래 로컬 스케일을 저장해둔다.
                                // (앞은 ray로 감지 중이므로 뒤통수에만 감지 콜라이더를 달 것이다. flipX로 하면 자식 객체인 레이더는 안뒤집어지므로 localScale로 뒤집는 것

    public int nextMove;    // 이동 방향 변수
    public int enemySpeed;  // 실제 이동 속도
    public int enemyDefaultSpeed = 2;   // 기본 걸음 속도
    public int enemyPlayerFindSpeed = 5;    // 플레이어 발견시 속도
    public int enemyDamage; // 적 공격력

    public bool isFindPlayer;   // 플레이어가 레이에 닿은 경우, 뒤통수 탐지레이더에 닿은 경우 true
    RaycastHit2D rayHit;

    [SerializeField] private GameObject alertUI;    // 플레이어를 탐지하기 시작하면 활성화, 탐지 범위를 벗어나면 비활성화(탐지시간이 0보다 크면 활성화, 탐지시간이 0이되면 비활성화)
    [SerializeField] private GameObject alertGage;  // 슬라이더처럼 활용할 게임 오브젝트.
    [SerializeField] private TextMesh alertSign; // 기본 ?, 발견 시 !
    [SerializeField] private GameObject discoverOutline;    // 발견 시 활성화할 테두리 효과
    // 스프라이트 scaleY를 조정해서 슬라이더처럼 보이게 한다.
    private Vector3 notFindGage = new Vector3(1, 0, 1);    // 기본 게이지(0)
    private Vector3 findGage = new Vector3(1, 1, 1);   // 발각 게이지(1)
    private float scaleY;   // alertGage의 스케일 y값. 시간 경과에 따라 스케일을 키워서 슬라이더 효과를 준다.
    
    public float findTime;  // 적마다 플레이어를 탐지하는데 걸리는 시간.
    public float time;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        noticePlayer = GetComponentInChildren<NoticePlayer>();  // 자식 오브젝트의 컴포넌트를 연결

        originLocalScale = this.transform.localScale;

        Invoke("Think", 5f);
    }

    private void Start()
    {
        // 초기화
        scaleY = 0;
        time = 0;
    }

    private void Update()
    {     
        if(GameManager.Instance.player.GetComponent<PlayerController>().isHide)
        {
            isFindPlayer = false;
        }

        NoticeFromBehind(); // 적 뒤에서 플레이어가 머무를 경우 플레이어를 인식하고 쫒아가는 함수(은신, 엎드리기 상태일 때 제외)

        if(alertGage.activeSelf)
        {
            // 텍스트 좌우반전 방지 -> enemy의 localScale이 양수(기본상태)이면 텍스트도 기본, 아니면 텍스트도 반전.
            alertSign.transform.localScale = this.transform.localScale.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }

        // 적 뒤통수의 탐지 레이더안에 플레이어가 들어왔을 때
        if (noticePlayer.isInNoticeRange)
        {
            alertUI.SetActive(true);    // 플레이어 탐지 UI 활성화.

            // 탐지 UI가 활성 상태일때만 시간과 슬라이더를 업데이트한다.
            if(alertUI.activeSelf)
            {
                time += Time.deltaTime;
                scaleY = time / findTime;

                // 탐지 게이지 업데이트
                alertGage.transform.localScale = new Vector3(1, scaleY, 1);

                if (scaleY >= 1)
                    scaleY = 1;

                if (time >= findTime)
                    time = findTime;
            }        
        }

        

        // 플레이어 탐지 불가 상태일 때 (적 뒤통수의 탐지 레이더에서 벗어낫을 때, 앞통수 ray범위 밖일 때)
        if(!isFindPlayer)
        { 
            if (rayHit.collider == null)
            {
                time -= Time.deltaTime;
                scaleY = time / findTime;

                // 탐지 게이지 업데이트
                alertGage.transform.localScale = new Vector3(1, scaleY, 1);

                if (scaleY <= 0)
                    scaleY = 0;

                if (time <= 0)
                {
                    time = 0;
                    MissPlayer();
                }
            }
        }         
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
            // 탐지 UI 활성화
            DiscoverPlayer();
            
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
            enemySpeed = enemyPlayerFindSpeed;  // 플레이어 발견시 2배속으로 이동
            Debug.Log("플레이어와 충돌!");

            // 플레이어가 왼쪽에 있는 경우
            if (collision.transform.position.x < rigid.position.x)
            {
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = -1;
                isFindPlayer = false;
            }
            // 플레이어가 오른쪽에 있는 경우
            else if (collision.transform.position.x > rigid.position.x)
            {
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
            DiscoverPlayer();
           
            CancelInvoke();
            animator.SetBool("isRun", true);

            enemySpeed = enemyPlayerFindSpeed;  // 플레이어 발견시 2배속으로 이동

            // 플레이어가 왼쪽에 있는 경우
            if (rayHit.collider.transform.position.x < rigid.position.x)
            {
                nextMove = -1;
            }
            // 플레이어가 오른쪽에 있는 경우
            else if (rayHit.collider.transform.position.x > rigid.position.x)
            {
                nextMove = 1;
            }
        }
        else if(rayHit.collider == null)
        {
            
            // 탐지 UI 비활성화
            //MissPlayer();
            isFindPlayer = false;

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
        if (noticePlayer.isInNoticeRange)
            isFindPlayer = true;        
        else
            isFindPlayer= false;

        // 자식 오브젝트에서 플레이어를 탐지했다면
        if (noticePlayer.isNotice)
        {
            DiscoverPlayer();
            
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
           
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

    // 플레이어 발견
    private void DiscoverPlayer()
    {
        // 탐지 UI 활성화
        alertUI.SetActive(true);
        alertGage.transform.localScale = findGage;
        alertSign.text = "!";
        discoverOutline.SetActive(true);
        isFindPlayer = true;
        time = findTime;
    }

    // 플레이어 놓침
    private void MissPlayer()
    {
        // 탐지 UI 비활성화
        alertGage.transform.localScale = notFindGage;  // 레이에 닿았다 -> 정면에서 바라본 것이므로 바로 발각된 것으로 간주?
        alertSign.text = "?";
        discoverOutline.SetActive(false);
        alertUI.SetActive(false);
        isFindPlayer = false;
        time = 0;
    }
}
