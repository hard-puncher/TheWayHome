using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer sprite;
    BoxCollider2D box;

    // �ݶ��̴� ������ ������ ���� ����(�������� �̵��Ҷ��� offsetX�� �������������)
    Vector2 originOffset;
    Vector2 leftOffset;

    public int nextMove;    // �̵� ���� ����
    public int enemySpeed;  // ���� �̵� �ӵ�
    public int enemyDefaultSpeed = 2;   // �⺻ ���� �ӵ�
    public int enemyPlayerFindSpeed = 5;    // �÷��̾� �߽߰� �ӵ�
    public int enemyDamage; // �� ���ݷ�

    public bool isFindPlayer;   // �÷��̾ ���̿� ���� ��� TRUE

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
        // �ݶ��̴� ������ ����
        box.offset = sprite.flipX == true ? leftOffset : originOffset;

        Move(); // ���� üũ    
        FindPlayer();   // �÷��̾� üũ
        // �̵� �ӵ�
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y) * enemySpeed;
    }

    // ��� �Լ�
    void Think()
    {
        // ���� �̵� ����
        nextMove = Random.Range(-1, 2);

        // �ִϸ��̼�
        animator.SetBool("isWalk", nextMove == 0 ? false : true);
        
        // Sprite FlipX
        if (nextMove != 0)
            sprite.flipX = nextMove == -1;

        // ���� �̵� �ֱ�
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
        Debug.DrawRay(rigid.position, sprite.flipX == true ? Vector3.left : Vector3.right, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position + new Vector2(0f, -1.5f), sprite.flipX == true ? Vector3.left : Vector3.right, 6, LayerMask.GetMask("Player"));

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
            animator.SetBool("isRun", false);
    }
}
