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
            GameManager.Instance.playerCurHP += 30f;
            // 체력은 최대 체력 이상으로 회복할 수 없게 제한
            if (GameManager.Instance.playerCurHP > GameManager.Instance.playerMaxHP)
                GameManager.Instance.playerCurHP = GameManager.Instance.playerMaxHP;
            gameObject.SetActive(false);
        }
    }
}
