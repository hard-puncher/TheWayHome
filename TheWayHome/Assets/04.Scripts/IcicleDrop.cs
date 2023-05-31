using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleDrop : MonoBehaviour
{
    BoxCollider2D box;
    Rigidbody2D rigid;

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

        // 레이에 플레이어가 닿은 경우(고드름 밑으로 플레이어가 지나간 경우)
        if (rayHit.collider != null)
        {
            // Gravity scale을 0 -> 3로 만들어 떨어뜨린다.
            rigid.gravityScale = 3f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            gameObject.SetActive(false);
        if(rigid.velocity.y < 0 && collision.gameObject.tag == "Ground")
            gameObject.SetActive(false);
    }

    void OnDisable()
    {
        rigid.gravityScale = 0f;
    }
}
