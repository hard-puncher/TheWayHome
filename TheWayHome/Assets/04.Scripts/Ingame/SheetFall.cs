using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetFall : MonoBehaviour
{
    private Rigidbody2D rigid;
    private BoxCollider2D box;

    private Vector3 startPosition;    // ���� ��ġ

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        // ������ ������ٰ� �ٽ� ������ ���� ���� ó�� ���� ��ġ�� �����Ѵ�.
        startPosition = this.transform.position;
    }

    // Ȱ��ȭ �ɶ� ���� ��ġ�� �����ϰ�, �߷��� �ٽ� 0���� �����.
    private void OnEnable()
    {
        this.gameObject.transform.position = startPosition; // ���� �� ��ġ
        this.rigid.gravityScale = 0f;   // �߷� ����x
        this.rigid.rotation = 0f;   // ���� ���� ����ġ
        this.rigid.constraints = RigidbodyConstraints2D.FreezeAll;  // �����̼� ����
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Invoke("DelayToFall", 1.5f);    
        }

        if(collision.gameObject.tag == "Ground")
        {
            DisableSheet(); // ���� ������ ��� �ٷ� ��Ȱ��ȭ
        }
    }

    // ��� ���� ��� �Ŀ� �������� �ϴ� �Լ�
    private void DelayToFall()
    {
        // �÷��̾�� ���� ��� �߷��� �༭ �������� �Ѵ�.
        this.rigid.gravityScale = 5f;
        // ������ ���� �и��� ������ �����ϱ� ���� freezePositionY�� true -> false�� ����� ������ �� �ְ� �Ѵ�.
        this.rigid.constraints = RigidbodyConstraints2D.None;   // ���� ���� Ǯ����ٰ�
        this.rigid.constraints = RigidbodyConstraints2D.FreezeRotation; // z�� �������ְ�
        this.rigid.constraints = RigidbodyConstraints2D.FreezePositionX;    // x �������ָ� y�� Ǯ����.

        Invoke("DisableSheet", 2f);
    }

    // ������ �� ������ ��Ȱ��ȭ �ϴ� �Լ�
    private void DisableSheet()
    {
        this.gameObject.SetActive(false);

        Invoke("RepositionSheet", 2f);
    }

    // ������ �ٽ� �����ϴ� �Լ�
    private void RepositionSheet()
    {
        this.gameObject.SetActive(true);
    }
}
