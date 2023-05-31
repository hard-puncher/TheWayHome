using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroy : MonoBehaviour
{
    [SerializeField]
    private int boxHp = 3;  // 3번 때리면 파괴되도록 한다.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 고양이의 공격과 닿으면 상자 내구도를 1씩 감소시킨다.
    }
}
