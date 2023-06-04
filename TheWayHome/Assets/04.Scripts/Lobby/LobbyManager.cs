using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LobbyManager : MonoBehaviour
{
    // 시작 메뉴 그룹
    public GameObject menuGroupUI;
    // 캐릭터 선택 창
    public GameObject characterSelectionUI;
    // 캐릭터 선택 후 활성화되는 게임 시작 버튼
    public GameObject gameStartButton;
    // 선택한 캐릭터 번호
    private int selectedCharacterIndex = -1;

    private void Awake()
    {
        // 게임 시작 시 현실 시간이 6~18시라면 낮 배경을, 18~6시라면 밤 배경을 활성화한다.
        GameObject backGround_Day = GameObject.FindGameObjectWithTag("BG_Day");     
        GameObject backGround_Night = GameObject.FindGameObjectWithTag("BG_Night");
        
        Debug.Log(DateTime.Now.Hour);
        if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 18)
        {
            backGround_Day.SetActive(true);
            backGround_Night.SetActive(false);
        }
        else if ((DateTime.Now.Hour >= 18 && DateTime.Now.Hour <= 24)
            || DateTime.Now.Hour < 6)
        {
            backGround_Day.SetActive(false);
            backGround_Night.SetActive(true);
        }
    }

    // 게임시작: 시작 메뉴-play버튼 클릭 시
    public void OnPlayButtonClicked()
    {
        // 시작메뉴를 비활성화하고 캐릭터 선택 UI를 활성화한다.
        menuGroupUI.SetActive(false);
        characterSelectionUI.SetActive(true);
    }

    // 이어하기: 시작 메뉴-continue버튼 클릭 시
    public void OnContinueButtonClicked()
    {

    }

    // 옵션: 시작 메뉴-option버튼 클릭 시
    public void OnOptionButtonClicked()
    {

    }

    // 종료: 시작 메뉴-quit버튼 클릭 시
    public void OnQuitButtonClicked()
    {

    }

    // 캐릭터 선택
    public void OnCharacterSelected(int characterIndex)
    {
        // 선택한 캐릭터 인덱스 저장
        selectedCharacterIndex = characterIndex;

        // 게임 시작 버튼 활성화
        gameStartButton.SetActive(true);
    }

    public void OnGameStartButtonClicked()
    {
        // 캐릭터를 선택했다면
        if(selectedCharacterIndex != -1)
        {
            // 선택한 캐릭터 인덱스를 인게임 씬으로 전달
            SceneManager.LoadScene("Stage0");
            // 선택한 캐릭터 정보를 기기에 저장
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        }
    }
}
