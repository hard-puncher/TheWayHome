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
    
    // 옵션 메뉴 그룹(시작 메뉴-옵션 선택 시 활성화)
    public GameObject optionGroupUI;

    // 종료 메뉴 그룹(시작 메뉴-종료 선택 시 활성화)
    public GameObject quitGroupUI;

    // 계정 리셋 그룹(시작메뉴-옵션-계정 리셋 선택 시 활성화)
    public GameObject accountResetGroupUI;

    // 시작 메뉴 하위의 UI 활성화 시 시작 메뉴 버튼들과의 상호작용을 방지하기 위한 플래그
    private bool isLowerGroupUIOn; 
   
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
        if (isLowerGroupUIOn)
            return;

        // 시작메뉴를 비활성화하고 캐릭터 선택 UI를 활성화한다.
        menuGroupUI.SetActive(false);
        characterSelectionUI.SetActive(true);
    }

    // 이어하기: 시작 메뉴-continue버튼 클릭 시
    public void OnContinueButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;
    }

    // 옵션: 시작 메뉴-option버튼 클릭 시
    public void OnOptionButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;

        optionGroupUI.SetActive(true);  // ui 활성화
        isLowerGroupUIOn = true;    // 시작메뉴 그룹과의 상호작용 방지
    }

    // 종료: 시작 메뉴-quit버튼 클릭 시
    public void OnQuitButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;

        quitGroupUI.SetActive(true);    // ui 활성화
        isLowerGroupUIOn = true;    // 시작메뉴 그룹과의 상호작용 방지
    }

    // 캐릭터 선택창에서 뒤로가기 버튼 클릭 시
    public void OnBackButtonClicked()
    {
        characterSelectionUI.SetActive(false);
        menuGroupUI.SetActive(true);
    }

    // 게임시작 버튼 -> 캐릭터 선택
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

    // 옵션 버튼 하위 그룹
    // BGM 음량 조절 슬라이더
    public void BgmControl()
    {

    }

    // 효과음 음량 조절 슬라이더
    public void SeControl()
    {

    }

    // 계정 초기화 버튼 클릭 시
    public void OnAccountResetButtonClicked()
    {
        optionGroupUI.SetActive(false); // 옵션 창 비활성화
        accountResetGroupUI.SetActive(true);    // 계정 초기화 창 활성화
    }

    // 계정 초기화 Yes 클릭
    public void OnAccountResetYesButtonClicked()
    {
        // 모든 세이브 데이터 초기화

        // 계정 초기화 창 비활성화
        accountResetGroupUI.SetActive(false);

        // 옵션 창 활성화
        optionGroupUI.SetActive(true);
    }

    // 계정 초기화 No 클릭 or 계정 초기화 창 닫기 클릭 시
    public void OnAccountResetNoButtonClicked()
    {
        accountResetGroupUI.SetActive(false);   // 계정 초기화 창 비활성화
        optionGroupUI.SetActive(true);  // 옵션 창 활성화
    }

    // 옵션 창 닫기 버튼 클릭 시
    public void OnOptionQuitButtonClicked()
    {
        optionGroupUI.SetActive(false);
        isLowerGroupUIOn = false;
    }

    // 종료 버튼 하위 그룹
    // 종료 Yes 클릭 시
    public void OnQuitYesButtonClicked()
    {
        Quit();
    }

    // 종료 No 클릭 시 or 종료 창 닫기 버튼 클릭 시
    public void OnQuitNoButtonClicked()
    {
        quitGroupUI.SetActive(false);
        isLowerGroupUIOn = false;
    }

    // 버튼 클릭 사운드 함수(인스펙터-OnClick에 추가)
    public void PlayButtonClickSound()
    {
        SoundManager.instance.PlaySE("ButtonClick");
    }

    // 유니티 에디터에서도 종료할 수 있도록 만든 종료 함수
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    // 플레이모드 종료
#endif
        Application.Quit();
    }
}
