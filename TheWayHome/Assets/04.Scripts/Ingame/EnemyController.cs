using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    Rigidbody2D rigid;
    Animator animator;
    BoxCollider2D box;
    NoticePlayer noticePlayer;  // �ڽ� ������Ʈ�� ������Ʈ

    Vector3 originLocalScale;   // ��������Ʈ flipX��� localScale�� ������ ���� ���� ���� �������� �����صд�.
                                // (���� ray�� ���� ���̹Ƿ� ��������� ���� �ݶ��̴��� �� ���̴�. flipX�� �ϸ� �ڽ� ��ü�� ���̴��� �ȵ��������Ƿ� localScale�� ������ ��

    public int nextMove;    // �̵� ���� ����
    public int enemySpeed;  // ���� �̵� �ӵ�
    public int enemyDefaultSpeed = 2;   // �⺻ ���� �ӵ�
    public int enemyPlayerFindSpeed = 5;    // �÷��̾� �߽߰� �ӵ�
    public int enemyDamage; // �� ���ݷ�

    public bool isFindPlayer;   // �÷��̾ ���̿� ���� ���, ����� Ž�����̴��� ���� ��� true
    RaycastHit2D rayHit;

    [SerializeField] private GameObject alertUI;    // �÷��̾ Ž���ϱ� �����ϸ� Ȱ��ȭ, Ž�� ������ ����� ��Ȱ��ȭ(Ž���ð��� 0���� ũ�� Ȱ��ȭ, Ž���ð��� 0�̵Ǹ� ��Ȱ��ȭ)
    [SerializeField] private GameObject alertGage;  // �����̴�ó�� Ȱ���� ���� ������Ʈ.
    [SerializeField] private TextMesh alertSign; // �⺻ ?, �߰� �� !
    [SerializeField] private GameObject discoverOutline;    // �߰� �� Ȱ��ȭ�� �׵θ� ȿ��
    // ��������Ʈ scaleY�� �����ؼ� �����̴�ó�� ���̰� �Ѵ�.
    private Vector3 notFindGage = new Vector3(1, 0, 1);    // �⺻ ������(0)
    private Vector3 findGage = new Vector3(1, 1, 1);   // �߰� ������(1)
    private float scaleY;   // alertGage�� ������ y��. �ð� ����� ���� �������� Ű���� �����̴� ȿ���� �ش�.
    
    public float findTime;  // ������ �÷��̾ Ž���ϴµ� �ɸ��� �ð�.
    public float time;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        noticePlayer = GetComponentInChildren<NoticePlayer>();  // �ڽ� ������Ʈ�� ������Ʈ�� ����

        originLocalScale = this.transform.localScale;

        Invoke("Think", 5f);
    }

    private void Start()
    {
        // �ʱ�ȭ
        scaleY = 0;
        time = 0;
    }

    private void Update()
    {     
        if(GameManager.Instance.player.GetComponent<PlayerController>().isHide)
        {
            isFindPlayer = false;
        }

        NoticeFromBehind(); // �� �ڿ��� �÷��̾ �ӹ��� ��� �÷��̾ �ν��ϰ� �i�ư��� �Լ�(����, ���帮�� ������ �� ����)

        if(alertGage.activeSelf)
        {
            // �ؽ�Ʈ �¿���� ���� -> enemy�� localScale�� ���(�⺻����)�̸� �ؽ�Ʈ�� �⺻, �ƴϸ� �ؽ�Ʈ�� ����.
            alertSign.transform.localScale = this.transform.localScale.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }

        // �� ������� Ž�� ���̴��ȿ� �÷��̾ ������ ��
        if (noticePlayer.isInNoticeRange)
        {
            alertUI.SetActive(true);    // �÷��̾� Ž�� UI Ȱ��ȭ.

            // Ž�� UI�� Ȱ�� �����϶��� �ð��� �����̴��� ������Ʈ�Ѵ�.
            if(alertUI.activeSelf)
            {
                time += Time.deltaTime;
                scaleY = time / findTime;

                // Ž�� ������ ������Ʈ
                alertGage.transform.localScale = new Vector3(1, scaleY, 1);

                if (scaleY >= 1)
                    scaleY = 1;

                if (time >= findTime)
                    time = findTime;
            }        
        }

        

        // �÷��̾� Ž�� �Ұ� ������ �� (�� ������� Ž�� ���̴����� ����� ��, ����� ray���� ���� ��)
        if(!isFindPlayer)
        { 
            if (rayHit.collider == null)
            {
                time -= Time.deltaTime;
                scaleY = time / findTime;

                // Ž�� ������ ������Ʈ
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
        Move(); // ���� üũ    
        
        FindPlayer();   // �÷��̾� üũ
        // �̵� �ӵ�
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y) * enemySpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �÷��̾�� �ε��� ��쿡�� Ž�� �ߴٰ� ���� ��� �÷��̾� �������� �޷�����.
        if(collision.gameObject.tag == "Player")
        {
            // Ž�� UI Ȱ��ȭ
            DiscoverPlayer();
            
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
            enemySpeed = enemyPlayerFindSpeed;  // �÷��̾� �߽߰� 2������� �̵�
            Debug.Log("�÷��̾�� �浹!");

            // �÷��̾ ���ʿ� �ִ� ���
            if (collision.transform.position.x < rigid.position.x)
            {
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = -1;
                isFindPlayer = false;
            }
            // �÷��̾ �����ʿ� �ִ� ���
            else if (collision.transform.position.x > rigid.position.x)
            {
                this.transform.localScale = originLocalScale;
                nextMove = 1;
                isFindPlayer = false;
            }
        }
    }

    // ��� �Լ�
    void Think()
    {
        // ���� �̵� ����
        nextMove = Random.Range(-1, 2);

        // Sprite FlipX
        if (nextMove != 0)
        {
            if(nextMove == -1)
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
            else
                this.transform.localScale = originLocalScale;
        }
         
        // ���� �̵� �ֱ�
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

    // ���� üũ
    private void Move()
    {
        enemySpeed = enemyDefaultSpeed; // �̵� �ÿ� �⺻ �ӵ��� �̵�
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHitPlatform = Physics2D.Raycast(frontVec, Vector3.down, 3, LayerMask.GetMask("Platform"));
        if (rayHitPlatform.collider == null)
        {
            Turn();
        }
    }

    // �÷��̾� Ž��
    void FindPlayer()
    {
        Debug.DrawRay(rigid.position, this.transform.localScale != originLocalScale ? Vector3.left : Vector3.right, new Color(0, 1, 0));
        rayHit = Physics2D.Raycast(rigid.position + new Vector2(0f, -1.5f), this.transform.localScale != originLocalScale ? Vector3.left : Vector3.right, 6, LayerMask.GetMask("Player"));

        // �÷��̾ ���̿� �������
        if (rayHit.collider != null && !isFindPlayer)
        {
            DiscoverPlayer();
           
            CancelInvoke();
            animator.SetBool("isRun", true);

            enemySpeed = enemyPlayerFindSpeed;  // �÷��̾� �߽߰� 2������� �̵�

            // �÷��̾ ���ʿ� �ִ� ���
            if (rayHit.collider.transform.position.x < rigid.position.x)
            {
                nextMove = -1;
            }
            // �÷��̾ �����ʿ� �ִ� ���
            else if (rayHit.collider.transform.position.x > rigid.position.x)
            {
                nextMove = 1;
            }
        }
        else if(rayHit.collider == null)
        {
            
            // Ž�� UI ��Ȱ��ȭ
            //MissPlayer();
            isFindPlayer = false;

            animator.SetBool("isRun", false);
            if(nextMove != 0)
                animator.SetBool("isWalk", true);
            else
                animator.SetBool("isWalk", false);
        }         
    }

    // �ڽ� ������Ʈ�� noticeRange���� �÷��̾ n�� �̻� �ӹ��� �߰� �Ǿ��� �� �÷��̾�� �����ϴ� �Լ�
    void NoticeFromBehind()
    {
        if (noticePlayer.isInNoticeRange)
            isFindPlayer = true;        
        else
            isFindPlayer= false;

        // �ڽ� ������Ʈ���� �÷��̾ Ž���ߴٸ�
        if (noticePlayer.isNotice)
        {
            DiscoverPlayer();
            
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
           
            enemySpeed = enemyPlayerFindSpeed;  // �÷��̾� �߽߰� 2������� �̵�
            Debug.Log("�÷��̾� �߰�!");

            // �ڿ��� ���������Ƿ� �÷��̾�� ������ �� �ڿ� �ִ�. 
            // ������ �ٶ󺸴� �� -> �������� ����
            if (this.transform.localScale == originLocalScale)
            {
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = -1;  // ����
                isFindPlayer = false;
                noticePlayer.isNotice = false;  // �ٽ� �ڽ� ������Ʈ�� �÷��׸� false��
            }
            // ���� �ٶ󺸴� �� -> ���������� ����
            else
            {
                this.transform.localScale = originLocalScale;
                nextMove = 1;   // ����
                isFindPlayer = false;
                noticePlayer.isNotice = false;  // �ٽ� �ڽ� ������Ʈ�� �÷��׸� false��
            }
        }
    }

    // �÷��̾� �߰�
    private void DiscoverPlayer()
    {
        // Ž�� UI Ȱ��ȭ
        alertUI.SetActive(true);
        alertGage.transform.localScale = findGage;
        alertSign.text = "!";
        discoverOutline.SetActive(true);
        isFindPlayer = true;
        time = findTime;
    }

    // �÷��̾� ��ħ
    private void MissPlayer()
    {
        // Ž�� UI ��Ȱ��ȭ
        alertGage.transform.localScale = notFindGage;  // ���̿� ��Ҵ� -> ���鿡�� �ٶ� ���̹Ƿ� �ٷ� �߰��� ������ ����?
        alertSign.text = "?";
        discoverOutline.SetActive(false);
        alertUI.SetActive(false);
        isFindPlayer = false;
        time = 0;
    }
}
