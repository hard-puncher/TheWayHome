using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFish : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 생선을 먹으면 체력을 30 회복
        if(collision.gameObject.tag == "Player")
        {
            // 생선 섭취 사운드 재생
            int random = Random.Range(0, 2);
            if (random == 0)
                SoundManager.instance.PlaySE("GetFood1");
            else
                SoundManager.instance.PlaySE("GetFood2");

            GameManager.Instance.curHp += 1;
            GameManager.Instance.IncreaseHP(1);
            // 체력은 최대 체력 이상으로 회복할 수 없게 제한
            if (GameManager.Instance.curHp > GameManager.Instance.maxHp)
                GameManager.Instance.curHp = GameManager.Instance.maxHp;
            gameObject.SetActive(false);
        }
    }
}
