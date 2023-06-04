using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엄폐 기능 스크립트
public class Hide : MonoBehaviour
{
    // 필요한 컴포넌트
    BoxCollider2D boxCollider;

    private bool nowHide;   // OnTriggerStay에서 한번만 호출될 수 있도록 세울 플래그

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // 플레이어가 엄폐물에 숨었을때
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 땅에 닿았을 때, nowHide가 false일 때만 실행한다.
        if (collision.gameObject.tag == "Player" && !collision.GetComponent<PlayerController>().isJump && !nowHide)
        {
            Debug.Log("엄폐 중입니다.");
            // 엄폐 중엔 점프를 막기 위해 isHide를 true로 한다.
            collision.GetComponent<PlayerController>().isHide = true;
            // Trigger로 바꿔도 추락하지 않도록 중력을 0으로 한다.
            collision.GetComponent<Rigidbody2D>().gravityScale = 0f;
            // 엄폐중엔 적이 통과할 수 있도록 Trigger상태로 한다.
            collision.GetComponent<CapsuleCollider2D>().isTrigger = true;
            // 알파값 조절
            collision.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
            // 추적 불가를 위해 레이어마스크 잠시 변경한다.
            collision.gameObject.layer = 0;
            // 엄폐 완료 했으면 nowHide를 true로 바꿔서, OnTriggerStay가 지속적으로 호출되는 것을 방지한다.
            nowHide = true;
        }
    }
    
    // 엄폐물을 벗어났을때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // nowHide를 false로 바꿔서, 다시 엄폐할 수 있게 한다.
            nowHide = false;
            Debug.Log("엄폐를 해제합니다.");
            // 점프 가능
            collision.GetComponent<PlayerController>().isHide = false;
            // 트리거 해제
            collision.GetComponent<CapsuleCollider2D>().isTrigger = false;
            // 중력 원래대로
            collision.GetComponent<Rigidbody2D>().gravityScale = 2f;
            // 알파값 1
            collision.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
            // 레이어 원래대로.
            collision.gameObject.layer = 3;
        }
    }
}
