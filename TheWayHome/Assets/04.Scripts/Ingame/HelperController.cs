using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperController : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;
    Animator animator;
    BoxCollider2D box;
    Vector3 leftLocalScale;
    Vector3 originLocalScale;

    NoticePlayer noticePlayer;  // �ڽ� ������Ʈ�� ������Ʈ
    [SerializeField] private GameObject fishPrefab;
    [SerializeField] private float dropPower;    // ������ ��ӵɶ� ���� ƨ��� �Ŀ�


    // �ݶ��̴� ������ ������ ���� ����(�������� �̵��Ҷ��� offsetX�� �������������)
    Vector2 originOffset;
    Vector2 leftOffset;

    public int nextMove;    // �̵� ���� ����
    public int helperSpeed;  // ���� �̵� �ӵ�
    public int helperDefaultSpeed = 2;   // �⺻ ���� �ӵ�
    public int helperPlayerFindSpeed = 5;    // �÷��̾� �߽߰� �ӵ�

    public bool isFindPlayer;   // �÷��̾ ���̿� ���� ��� TRUE
    public bool isTouchPlayer;  // �÷��̾�� ������ ��� true
   
    RaycastHit2D rayHit;

    [SerializeField] private GameObject alertUI;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        noticePlayer = GetComponentInChildren<NoticePlayer>();  // �ڽ� ������Ʈ�� ������Ʈ�� ����

        originLocalScale = this.transform.localScale;
        leftLocalScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);

        originOffset = new Vector2(box.offset.x, box.offset.y);
        leftOffset = new Vector2(-box.offset.x, box.offset.y);

        Invoke("Think", 5f);
    }

    private void Update()
    {
        if(!isTouchPlayer)
        {
            if (nextMove == 1)
                this.transform.localScale = originLocalScale;
            else if (nextMove == -1)
                this.transform.localScale = leftLocalScale;
        }

        NoticeFromBehind(); // �� �ڿ��� �÷��̾ �ӹ��� ��� �÷��̾ �ν��ϰ� �i�ư��� �Լ�(����, ���帮�� ������ �� ����)
    }


    void FixedUpdate()
    {
        if(!isTouchPlayer)
        {
            Move(); // ���� üũ    
            FindPlayer();   // �÷��̾� üũ
                            // �̵� �ӵ�
            rigid.velocity = new Vector2(nextMove, rigid.velocity.y) * helperSpeed;
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �÷��̾�� �ε��� ��쿡�� Ž�� �ߴٰ� ���� �����.
        if (collision.gameObject.tag == "Player")
        {
            alertUI.SetActive(true);
            rigid.velocity = Vector2.zero;
            isTouchPlayer = true;
            CancelInvoke();
            animator.SetBool("isWalk", false);
            
            isFindPlayer = true;
            helperSpeed = 0;
            Debug.Log("Helper�� �����߽��ϴ�!");

            // �÷��̾ ���ʿ� �ִ� ���
            if (collision.transform.position.x < rigid.position.x)
            {
                //sprite.flipX = true;
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = 0;
                isFindPlayer = false;
            }
            // �÷��̾ �����ʿ� �ִ� ���
            else if (collision.transform.position.x > rigid.position.x)
            {
                //sprite.flipX = false;
                this.transform.localScale = originLocalScale;
                nextMove = 0;
                isFindPlayer = false;
            }

            // ���� ���
            GameObject fish = Instantiate(fishPrefab);
            fish.transform.position = gameObject.transform.position;
            fish.GetComponent<Rigidbody2D>().AddForce(Vector2.up * dropPower, ForceMode2D.Impulse); // ���� ��ӽ� ���� Ƣ���ٰ� �������� ȿ��
            StartCoroutine(FadeAway());
        }
    }

    // ���� �ְ� ����� �� ������� ������� �ϴ� �ڷ�ƾ.
    IEnumerator FadeAway()
    {
        while (spriteRenderer.color.a > 0)
        {
            spriteRenderer.color -= new Color(1, 1, 1, Time.deltaTime);
            yield return null;
        }

        if(spriteRenderer.color.a <= 0)
            gameObject.SetActive(false);
    }

    // ��� �Լ�
    void Think()
    {
        // ���� �̵� ����
        nextMove = Random.Range(-1, 2);

        // Sprite FlipX
        if (nextMove != 0)
        {
            if (nextMove == -1)
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
        helperSpeed = helperDefaultSpeed; // �̵� �ÿ� �⺻ �ӵ��� �̵�
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
            alertUI.SetActive(true);
            CancelInvoke();
            animator.SetBool("isWalk", true);
            isFindPlayer = true;
            helperSpeed = helperPlayerFindSpeed;  // �÷��̾� �߽߰� ���� �ӵ��� �̵�
            Debug.Log("�÷��̾� �߰�!");

            // �÷��̾ ���ʿ� �ִ� ���
            if (rayHit.collider.transform.position.x < rigid.position.x)
            {
                nextMove = -1;
                isFindPlayer = false;
            }
            // �÷��̾ �����ʿ� �ִ� ���
            else if (rayHit.collider.transform.position.x > rigid.position.x)
            {
                nextMove = 1;
                isFindPlayer = false;
            }
        }
        else
        {
            alertUI.SetActive(false);
            if (nextMove != 0)
                animator.SetBool("isWalk", true);
            else
                animator.SetBool("isWalk", false);
        }
    }

    // �ڽ� ������Ʈ�� noticeRange���� �÷��̾ n�� �̻� �ӹ��� �߰� �Ǿ��� �� �÷��̾�� �����ϴ� �Լ�
    void NoticeFromBehind()
    {
        // �ڽ� ������Ʈ���� �÷��̾ Ž���ߴٸ�
        if (noticePlayer.isNotice)
        {
            alertUI.SetActive(true);
            CancelInvoke();
            animator.SetBool("isWalk", true);
            
            isFindPlayer = true;
            helperSpeed = helperPlayerFindSpeed;  // �÷��̾� �߽߰� ���� �ӵ��� �̵�
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
}
