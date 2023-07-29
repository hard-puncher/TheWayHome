using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleDrop : MonoBehaviour
{
    BoxCollider2D box;
    Rigidbody2D rigid;

    public int icicleDamage;
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        // RayCast for Icicle Drop to player
        Debug.DrawRay(transform.position, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Vector3.down, 5f, LayerMask.GetMask("Player"));

        // ���̿� �÷��̾ ���� ���(��帧 ������ �÷��̾ ������ ���)
        if (rayHit.collider != null)
        {
            // Gravity scale�� 0 -> 3�� ����� ����߸���.
            rigid.gravityScale = 3f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.gameObject.GetComponent<PlayerController>().isInvincible)
        {
            // �ǰ� ���� ���
            SoundManager.instance.PlaySE("Hit");

            gameObject.SetActive(false);

            // �÷��̾� ��������ȭ
            collision.gameObject.GetComponent<PlayerController>().Invincible();

            collision.gameObject.GetComponent<Animator>().SetTrigger("isHurt");

            GameManager.Instance.DecreaseHP(icicleDamage);
            //GameManager.Instance.playerCurHP -= icicleDamage;
        }
             
        if(rigid.velocity.y < 0 && collision.gameObject.tag == "Ground")
        {
            // ȿ���� ���
            SoundManager.instance.PlaySE("IcicleDrop");
            gameObject.SetActive(false);
        }           
    }

    void OnDisable()
    {
        rigid.gravityScale = 0f;
    }
}
