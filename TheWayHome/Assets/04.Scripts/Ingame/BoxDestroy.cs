using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroy : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private int boxHp = 3;  // 3번 때리면 파괴되도록 한다.

    [SerializeField]
    private GameObject fishPrefab;
    [SerializeField]
    private float dropPower;    // 생선이 드롭될때 위로 튕기는 파워

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 고양이의 공격과 닿으면 상자 내구도를 1씩 감소시킨다.
        if(collision.gameObject.tag == "AttackRange")
        {
            // 박스 히트 사운드 재생
            SoundManager.instance.PlaySE("BoxHit");

            boxHp--;
            // 내구도가 0이되면 애니메이션을 재생한 후 비활성화하고, 생선 프리팹을 그자리에 생성한다.
            if(boxHp <= 0)
            {
                // 박스 파괴 사운드 재생
                SoundManager.instance.PlaySE("BoxDestroy");

                animator.SetTrigger("isOpened");
                Invoke("BoxDisable", 0.8f); // 애니메이션의 시간만큼 기다렸다가 비활성화
            }
        }
    }

    // 박스 비활성화 및 생선 프리팹 생성
    private void BoxDisable()
    {
        // 50%의 확률로 생선을 드롭한다. 박스는 둘다 비활성화한다.
        int fishOrNot = Random.Range(0, 10);
        if(fishOrNot >= 0 && fishOrNot < 5)
        {
            GameObject fish = Instantiate(fishPrefab);
            fish.transform.position = gameObject.transform.position;
            gameObject.SetActive(false);    // 상자 비활성화
            fish.GetComponent<Rigidbody2D>().AddForce(Vector2.up * dropPower, ForceMode2D.Impulse); // 생선 드롭시 위로 튀었다가 떨어지는 효과
        }
        else if(fishOrNot >= 5 && fishOrNot < 10)
        {
            gameObject.SetActive(false);    // 상자 비활성화
        }        
    }
}
