using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Slider playerHPBar; // 플레이어 체력바

    // 플레이어 체력
    public float playerMaxHP = 100f;  // 플레이어 최대 체력(100초)
    public float playerCurHP; // 플레이어 현재 체력
  
    private void Awake()
    {
        if(instance == null)
        {
            // 이 클래스의 인스턴스가 탄생했을 때 전역변수 instance에 게임매니저 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            instance = this;

            // 씬 전환이 되더라도 파괴되지 않게한다.
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 만약 씬 이동이 되었는데 그 씬에도 게임매니저가 존재한다면
            // 새로운 씬의 게임매니저를 파괴한다.
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
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

    // 옵션 버튼
    public void OnIngameOptionButtonClicked()
    {
        // 옵션 UI 활성화
    }

    // 로비 버튼
    public void OnIngameLobbyButtonClicked()
    {
        // 로비 귀환 UI 활성화
    }
}
