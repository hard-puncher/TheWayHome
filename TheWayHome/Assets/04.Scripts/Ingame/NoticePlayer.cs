using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticePlayer : MonoBehaviour
{
    public bool isInNoticeRange;    // Ž�� ���� �ȿ� ��� �Դ��� ����
    public bool isNotice;   // ���� Ž�� �ߴ��� ����

    public float noticeTime;    // Ž�� ������ �ӹ����� ���� ���� �÷��̾ �˾������µ� �ɸ��� �ð�
    public float time;

    private void Update()
    {
        // Ž�� ���� �ȿ� ������ true�̸� �ð��� ����� ����
        if(isInNoticeRange)
        {
            time += Time.deltaTime;
            // �˾������µ� �ɸ��� �ð����� Ŀ����
            if(time >= noticeTime)
            {
                isNotice = true;    // isNotice�� true�� ����� �θ� ��ü�� EnemyController���� �߰��� �����ϰ� �Ѵ�.
                // ���� �ʱ�ȭ
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
        // �÷��̾ ���� ���°� �ƴϰ�, ���帮�� ���� ���� ���� Ž�� �ð� ����� �����Ѵ�. -> ���� ����, ���帮�� ���� ���� ���� �ε����ų� ray�� �°ų� ���� �ʴ� �̻� Ž������ ����.
        if(collision.gameObject.tag == "Player" 
            && !collision.gameObject.GetComponent<PlayerController>().isHide
            && !collision.gameObject.GetComponent<PlayerController>().isCrawl
            && !isInNoticeRange)
        {
            isInNoticeRange = true; // �ð��� ����ϵ��� Ž�� ���� �ȿ� ������ true�� �Ѵ�.
            Debug.Log("�� Ž�� ���� �ȿ� ���Խ��ϴ�. �ð� ����� �����մϴ�.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInNoticeRange = false;    // Ž�� ������ ����� false�� �ؼ� �ð� ����� �����.
            Debug.Log("�� Ž�� ������ ������ϴ�.. �ð� ����� �����մϴ�.");
        }
    }
}
