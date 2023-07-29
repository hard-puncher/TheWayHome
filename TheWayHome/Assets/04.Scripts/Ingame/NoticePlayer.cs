using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticePlayer : MonoBehaviour
{
    public bool isInNoticeRange;    // 탐지 범위 안에 들어 왔는지 여부
    public bool isNotice;   // 적을 탐지 했는지 여부

    public float noticeTime;    // 탐지 범위에 머무르는 동안 적이 플레이어를 알아차리는데 걸리는 시간
    public float time;

    private void Update()
    {
        // 탐지 범위 안에 들어옴이 true이면 시간을 계산을 시작
        if(isInNoticeRange)
        {
            time += Time.deltaTime;
            // 알아차리는데 걸리는 시간보다 커지면
            if(time >= noticeTime)
            {
                isNotice = true;    // isNotice를 true로 만들어 부모 객체인 EnemyController에서 추격을 시작하게 한다.
                // 조건 초기화
                time = 0f;
                //isInNoticeRange = false;
            }
        }
        else
        {
            time -= Time.deltaTime;
            if (time <= 0)
                time = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 은신 상태가 아니고, 엎드리고 있지 않을 때만 탐지 시간 계산을 시작한다. -> 은신 상태, 엎드리고 있을 때는 적과 부딪히거나 ray에 맞거나 하지 않는 이상 탐지되지 않음.
        if(collision.gameObject.tag == "Player" 
            && !collision.gameObject.GetComponent<PlayerController>().isHide
            && !collision.gameObject.GetComponent<PlayerController>().isCrawl
            && !isInNoticeRange)
        {
            isInNoticeRange = true; // 시간을 계산하도록 탐지 범위 안에 들어옴을 true로 한다.
            Debug.Log("적 탐지 범위 안에 들어왔습니다. 시간 계산을 시작합니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInNoticeRange = false;    // 탐지 범위를 벗어나면 false로 해서 시간 계산을 멈춘다.
            Debug.Log("적 탐지 범위를 벗어났습니다.. 시간 계산을 종료합니다.");
        }
    }
}
