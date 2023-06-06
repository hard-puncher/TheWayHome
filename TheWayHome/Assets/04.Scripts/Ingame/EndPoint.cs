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
            // 현재 스테이지를 문자열로 받는다.
            string currentStage = SceneManager.GetActiveScene().name;
            // 문자열에서 현재 스테이지의 번호를 추출한다.
            int currentStageNumber = int.Parse(currentStage.Substring(currentStage.Length - 1));
            // 해당 스테이지는 클리어했으므로, 다음 스테이지를 저장한다.
            int nextStageNumber = currentStageNumber + 1;
            // 다음 스테이지를 문자열로 만든다.
            string nextStage = "Stage" + nextStageNumber;
            // 다음 스테이지를 로드한다.
            SceneManager.LoadScene(nextStage);
            // 스테이지 정보 저장
            DataManager.instance.SaveStageData(nextStageNumber);
        }
    }
}
