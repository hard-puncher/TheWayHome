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
            // ���� ���� ���� ���
            int random = Random.Range(0, 2);
            if (random == 0)
                SoundManager.instance.PlaySE("GetFood1");
            else
                SoundManager.instance.PlaySE("GetFood2");

            GameManager.Instance.curHp += 1;
            GameManager.Instance.IncreaseHP(1);
            // ü���� �ִ� ü�� �̻����� ȸ���� �� ���� ����
            if (GameManager.Instance.curHp > GameManager.Instance.maxHp)
                GameManager.Instance.curHp = GameManager.Instance.maxHp;
            gameObject.SetActive(false);
        }
    }
}
