using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheetFall : MonoBehaviour
{
    private Rigidbody2D rigid;
    private BoxCollider2D box;

    private Vector3 startPosition;    // 장판 위치

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        // 장판이 사라졌다가 다시 생성될 때를 위해 처음 시작 위치를 저장한다.
        startPosition = this.transform.position;
    }

    // 활성화 될때 원래 위치에 생성하고, 중력을 다시 0으로 만든다.
    private void OnEnable()
    {
        this.gameObject.transform.position = startPosition; // 장판 원 위치
        this.rigid.gravityScale = 0f;   // 중력 영향x
        this.rigid.rotation = 0f;   // 장판 각도 원위치
        this.rigid.constraints = RigidbodyConstraints2D.FreezeAll;  // 로테이션 고정
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Invoke("DelayToFall", 1.5f);    
        }

        if(collision.gameObject.tag == "Ground")
        {
            DisableSheet(); // 땅에 떨어진 경우 바로 비활성화
        }
    }

    // 밟고 나서 잠시 후에 떨어지게 하는 함수
    private void DelayToFall()
    {
        // 플레이어와 닿은 경우 중력을 줘서 떨어지게 한다.
        this.rigid.gravityScale = 5f;
        // 장판이 위로 밀리는 현상을 방지하기 위해 freezePositionY를 true -> false로 만들어 떨어질 수 있게 한다.
        this.rigid.constraints = RigidbodyConstraints2D.None;   // 먼저 전부 풀어줬다가
        this.rigid.constraints = RigidbodyConstraints2D.FreezeRotation; // z축 고정해주고
        this.rigid.constraints = RigidbodyConstraints2D.FreezePositionX;    // x 고정해주면 y만 풀린다.

        Invoke("DisableSheet", 2f);
    }

    // 떨어진 후 장판을 비활성화 하는 함수
    private void DisableSheet()
    {
        this.gameObject.SetActive(false);

        Invoke("RepositionSheet", 2f);
    }

    // 장판을 다시 생성하는 함수
    private void RepositionSheet()
    {
        this.gameObject.SetActive(true);
    }
}
