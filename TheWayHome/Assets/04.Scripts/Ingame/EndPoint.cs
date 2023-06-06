using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EndPoint : MonoBehaviour
{
    // ��������Ʈ�� ������ ���� ������ �̵�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("��������Ʈ ����! ���� ���������� �̵�.");
            // ���� ���������� ���ڿ��� �޴´�.
            string currentStage = SceneManager.GetActiveScene().name;
            // ���ڿ����� ���� ���������� ��ȣ�� �����Ѵ�.
            int currentStageNumber = int.Parse(currentStage.Substring(currentStage.Length - 1));
            // �ش� ���������� Ŭ���������Ƿ�, ���� ���������� �����Ѵ�.
            int nextStageNumber = currentStageNumber + 1;
            // ���� ���������� ���ڿ��� �����.
            string nextStage = "Stage" + nextStageNumber;
            // ���� ���������� �ε��Ѵ�.
            SceneManager.LoadScene(nextStage);
            // �������� ���� ����
            DataManager.instance.SaveStageData(nextStageNumber);
        }
    }
}
