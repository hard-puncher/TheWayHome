using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾ ������ ������ ü���� 30 ȸ��
        if(collision.gameObject.tag == "Player")
        {
            GameManager.Instance.playerCurHP += 30f;
            // ü���� �ִ� ü�� �̻����� ȸ���� �� ���� ����
            if (GameManager.Instance.playerCurHP > GameManager.Instance.playerMaxHP)
                GameManager.Instance.playerCurHP = GameManager.Instance.playerMaxHP;
            gameObject.SetActive(false);
        }
    }
}
