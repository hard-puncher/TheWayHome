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
            // ���� ���� ���� �ε����� �����´�.
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // ���� ���� ���� �ε����� ���� ���� �ε����� 1�� ���Ѵ�.
            int nextSceneIndex = currentSceneIndex + 1;
            // ���� ���� �ε��Ѵ�.
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
