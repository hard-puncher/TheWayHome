using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroy : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private int boxHp = 3;  // 3�� ������ �ı��ǵ��� �Ѵ�.

    [SerializeField]
    private GameObject fishPrefab;
    [SerializeField]
    private float dropPower;    // ������ ��ӵɶ� ���� ƨ��� �Ŀ�

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // ������� ���ݰ� ������ ���� �������� 1�� ���ҽ�Ų��.
        if(collision.gameObject.tag == "AttackRange")
        {
            // �ڽ� ��Ʈ ���� ���
            SoundManager.instance.PlaySE("BoxHit");

            boxHp--;
            // �������� 0�̵Ǹ� �ִϸ��̼��� ����� �� ��Ȱ��ȭ�ϰ�, ���� �������� ���ڸ��� �����Ѵ�.
            if(boxHp <= 0)
            {
                // �ڽ� �ı� ���� ���
                SoundManager.instance.PlaySE("BoxDestroy");

                animator.SetTrigger("isOpened");
                Invoke("BoxDisable", 0.8f); // �ִϸ��̼��� �ð���ŭ ��ٷȴٰ� ��Ȱ��ȭ
            }
        }
    }

    // �ڽ� ��Ȱ��ȭ �� ���� ������ ����
    private void BoxDisable()
    {
        // 50%�� Ȯ���� ������ ����Ѵ�. �ڽ��� �Ѵ� ��Ȱ��ȭ�Ѵ�.
        int fishOrNot = Random.Range(0, 10);
        if(fishOrNot >= 0 && fishOrNot < 5)
        {
            GameObject fish = Instantiate(fishPrefab);
            fish.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);    // ���� ��Ȱ��ȭ
            fish.GetComponent<Rigidbody2D>().AddForce(Vector2.up * dropPower, ForceMode2D.Impulse); // ���� ��ӽ� ���� Ƣ���ٰ� �������� ȿ��
        }
        else if(fishOrNot >= 5 && fishOrNot < 10)
        {
            gameObject.SetActive(false);    // ���� ��Ȱ��ȭ
        }        
    }
}
