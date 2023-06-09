using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    Rigidbody2D rigid;
    Animator animator;
    BoxCollider2D box;
    NoticePlayer noticePlayer;  // �ڽ� ������Ʈ�� ������Ʈ

    Vector3 originLocalScale;   // ��������Ʈ flipX��� localScale�� ������ ���� ���� ���� �������� �����صд�.
                                // (���� ray�� ���� ���̹Ƿ� ��������� ���� �ݶ��̴��� �� ���̴�. flipX�� �ϸ� �ڽ� ��ü�� ���̴��� �ȵ��������Ƿ� localScale�� ������ ��

    // �ݶ��̴� ������ ������ ���� ����(�������� �̵��Ҷ��� offsetX�� �������������)
    Vector2 originOffset;
    Vector2 leftOffset;

    public int nextMove;    // �̵� ���� ����
    public int enemySpeed;  // ���� �̵� �ӵ�
    public int enemyDefaultSpeed = 2;   // �⺻ ���� �ӵ�
    public int enemyPlayerFindSpeed = 5;    // �÷��̾� �߽߰� �ӵ�
    public int enemyDamage; // �� ���ݷ�

    public bool isFindPlayer;   // �÷��̾ ���̿� ���� ��� TRUE

    RaycastHit2D rayHit;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        noticePlayer = GetComponentInChildren<NoticePlayer>();  // �ڽ� ������Ʈ�� ������Ʈ�� ����

        originLocalScale = this.transform.localScale;

        originOffset = new Vector2(box.offset.x, box.offset.y);
        leftOffset = new Vector2(-box.offset.x, box.offset.y);

        Invoke("Think", 5f);
    }

    private void Update()
    {
        NoticeFromBehind(); // �� �ڿ��� �÷��̾ �ӹ��� ��� �÷��̾ �ν��ϰ� �i�ư��� �Լ�(����, ���帮�� ������ �� ����)
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
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
            isFindPlayer = true;
            enemySpeed = enemyPlayerFindSpeed;  // �÷��̾� �߽߰� 2������� �̵�
            Debug.Log("�÷��̾� �߰�!");

            // �÷��̾ ���ʿ� �ִ� ���
            if (collision.transform.position.x < rigid.position.x)
            {
                //sprite.flipX = true;
                this.transform.localScale = new Vector3(-originLocalScale.x, originLocalScale.y, originLocalScale.z);
                nextMove = -1;
                isFindPlayer = false;
            }
            // �÷��̾ �����ʿ� �ִ� ���
            else if (collision.transform.position.x > rigid.position.x)
            {
                //sprite.flipX = false;
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
            CancelInvoke();
            animator.SetBool("isRun",  true);
            isFindPlayer = true;
            enemySpeed = enemyPlayerFindSpeed;  // �÷��̾� �߽߰� 2������� �̵�
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
        // �ڽ� ������Ʈ���� �÷��̾ Ž���ߴٸ�
        if (noticePlayer.isNotice)
        {
            CancelInvoke();
            animator.SetBool("isWalk", false);
            animator.SetBool("isRun", true);
            isFindPlayer = true;
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
}
