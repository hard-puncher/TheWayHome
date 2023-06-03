using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class EndPoint : MonoBehaviour
{
    // 엔드포인트에 닿으면 다음 씬으로 이동
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("엔드포인트 도달! 다음 스테이지로 이동.");
            // 현재 씬의 빌드 인덱스를 가져온다.
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // 다음 씬의 빌드 인덱스는 현재 씬의 인덱스에 1을 더한다.
            int nextSceneIndex = currentSceneIndex + 1;
            // 다음 씬을 로드한다.
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
