using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ��� ��ũ��Ʈ
public class Hide : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    BoxCollider2D boxCollider;

    private bool nowHide;   // OnTriggerStay���� �ѹ��� ȣ��� �� �ֵ��� ���� �÷���

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // �÷��̾ ���󹰿� ��������
    private void OnTriggerStay2D(Collider2D collision)
    {
        // �÷��̾ ���� ����� ��, nowHide�� false�� ���� �����Ѵ�.
        if (collision.gameObject.tag == "Player" && !collision.GetComponent<PlayerController>().isJump && !nowHide)
        {
            Debug.Log("���� ���Դϴ�.");
            // ���� �߿� ������ ���� ���� isHide�� true�� �Ѵ�.
            collision.GetComponent<PlayerController>().isHide = true;
            // Trigger�� �ٲ㵵 �߶����� �ʵ��� �߷��� 0���� �Ѵ�.
            collision.GetComponent<Rigidbody2D>().gravityScale = 0f;
            // �����߿� ���� ����� �� �ֵ��� Trigger���·� �Ѵ�.
            collision.GetComponent<CapsuleCollider2D>().isTrigger = true;
            // ���İ� ����
            collision.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
            // ���� �Ұ��� ���� ���̾��ũ ��� �����Ѵ�.
            collision.gameObject.layer = 0;
            // ���� �Ϸ� ������ nowHide�� true�� �ٲ㼭, OnTriggerStay�� ���������� ȣ��Ǵ� ���� �����Ѵ�.
            nowHide = true;
        }
    }
    
    // ������ �������
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // nowHide�� false�� �ٲ㼭, �ٽ� ������ �� �ְ� �Ѵ�.
            nowHide = false;
            Debug.Log("���� �����մϴ�.");
            // ���� ����
            collision.GetComponent<PlayerController>().isHide = false;
            // Ʈ���� ����
            collision.GetComponent<CapsuleCollider2D>().isTrigger = false;
            // �߷� �������
            collision.GetComponent<Rigidbody2D>().gravityScale = 2f;
            // ���İ� 1
            collision.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            // ���̾� �������.
            collision.gameObject.layer = 3;
        }
    }
}
