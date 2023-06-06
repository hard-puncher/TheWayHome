using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 싱글톤으로 만들어 플레이어 정보와 체력, UI를 관리한다.
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    // 게임매니저 인스턴스에 접근할 수 있는 프로퍼티, static이므로 다른 클래스에서 마음껏 호출 가능하다.
    // ("프로퍼티"란 선언한 변수의 값을 외부에서 접근할 수 있도록 하면서 동시에 캡슐화를 지원하는 언어의 한 요소이다.)
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    // 플레이어 프리팹
    public GameObject[] characters;

    // 선택한 플레이어
    public GameObject player;

    // UI
    [SerializeField]
    private GameObject pauseGroupUI;    // 일시정지 시 활성화되는 버튼그룹

    [SerializeField]
    private GameObject soundGroupUI;   // 일시정지-사운드 옵션 선택 시 활성화

    [SerializeField]
    private GameObject lobbyGroupUI;   // 일시정지-로비 선택 시 활성화

    [SerializeField]
    private Slider playerHPBar; // 플레이어 체력바

    [SerializeField]
    private Slider bgmSlider;   // bgm 슬라이더
    [SerializeField]
    private Slider seSlider;    // se 슬라이더

    // 플레이어 체력
    public float playerMaxHP = 100f;  // 플레이어 최대 체력(100초)
    public float playerCurHP; // 플레이어 현재 체력
  
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 인게임 씬으로 이동 시 로비 씬에서 설정한 사운드 볼륨및 슬라이더 값에 맞게 조절.
        bgmSlider.value = SoundManager.instance.bgmSliderValue;
        SoundManager.instance.SetBgmVolume(SoundManager.instance.bgmSoundVolume);
        seSlider.value = SoundManager.instance.seSliderValue;
        SoundManager.instance.SetSeVolume(SoundManager.instance.seSoundVolume);

        // 로비 BGM 멈춤
        SoundManager.instance.StopBGM("Lobby");
        // 인게임 BGM 재생
        SoundManager.instance.PlayBGM("Ingame");

        // 저장된 선택한 캐릭터 인덱스 가져오기
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);

        if(selectedCharacterIndex != -1 && selectedCharacterIndex < characters.Length)
        {
            // 선택한 캐릭터 인덱스에 해당하는 캐릭터 생성
            player = Instantiate(characters[selectedCharacterIndex]);
        }

        // 체력 초기화
        playerCurHP = playerMaxHP;
    }

    private void Update()
    {
        DecreaseHPOverTime();
        UpdateHPBar();
    }

    // 실시간 체력 감소
    private void DecreaseHPOverTime()
    {
        playerCurHP -= Time.deltaTime;      

        if (playerCurHP < 0f)
            playerCurHP = 0f;
    }

    // 슬라이더 갱신
    private void UpdateHPBar()
    {
        playerHPBar.value = playerCurHP / playerMaxHP;
    }

    // 일시정지 버튼
    public void OnPauseButtonClicked()
    {
        // 시간을 멈추고 메뉴를 활성화한다.
        Time.timeScale = 0f;
        pauseGroupUI.SetActive(true);
    }

    // 계속하기 버튼
    public void OnIngameContinueButtonClicked()
    {
        // 메뉴를 비활성화하고 다시 시간이 흐른다.
        pauseGroupUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // 사운드 옵션 버튼
    public void OnIngameSoundButtonClicked()
    {
        // 일시정지 그룹 비활성화
        pauseGroupUI.SetActive(false);
        // 사운드 옵션 UI 활성화
        soundGroupUI.SetActive(true);
    }

    // 로비 버튼
    public void OnIngameLobbyButtonClicked()
    {
        // 일시정지 그룹 비활성화
        pauseGroupUI.SetActive(false);
        // 로비 귀환 UI 활성화
        lobbyGroupUI.SetActive(true);
    }

    // 로비 버튼 하위 그룹
    // 로비 Yes 클릭 시
    public void OnLobbyYesButtonClicked()
    {
        // 현재 스테이지를 문자열로 받는다.
        string currentStage = SceneManager.GetActiveScene().name;
        Debug.Log(currentStage);
        // 문자열에서 현재 스테이지의 번호를 추출한다.
        int currentStageNumber = int.Parse(currentStage.Substring(currentStage.Length - 1));
        // 현재 스테이지 정보를 저장한다.
        DataManager.instance.SaveStageData(currentStageNumber);

        // 다시 시간이 흐르게 함(로비로 돌아갔다가 다시 시작할 경우 멈추는 현상 발생
        Time.timeScale = 1f;

        // 로비 BGM 재생
        SoundManager.instance.PlayBGM("Lobby");

        // 로비 이동
        SceneManager.LoadScene("Lobby");
        
        // 인게임 BGM 멈춤
        //SoundManager.instance.StopBGM("Ingame");
    }

    // 로비 No 클릭 or 닫기 버튼 클릭 시
    public void OnLobbyNoButtonClicked()
    {
        // 로비 귀환 UI 비활성화
        lobbyGroupUI.SetActive(false);
        // 일시정지 그룹 활성화
        pauseGroupUI.SetActive(true);
    }

    // 사운드 옵션 창 닫기 버튼 클릭 시
    public void OnSoundQuitButtonClicked()
    {
        // 옵션 비활성화
        soundGroupUI.SetActive(false);
        // 일시정지 그룹 활성화
        pauseGroupUI.SetActive(true);
    }

    // 버튼 클릭 사운드 함수(인스펙터-OnClick에 추가)
    public void PlayButtonClickSound()
    {
        SoundManager.instance.PlaySE("ButtonClick");
    }
}
