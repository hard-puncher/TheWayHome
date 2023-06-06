using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// �̱������� ����� �÷��̾� ������ ü��, UI�� �����Ѵ�.
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    // ���ӸŴ��� �ν��Ͻ��� ������ �� �ִ� ������Ƽ, static�̹Ƿ� �ٸ� Ŭ�������� ������ ȣ�� �����ϴ�.
    // ("������Ƽ"�� ������ ������ ���� �ܺο��� ������ �� �ֵ��� �ϸ鼭 ���ÿ� ĸ��ȭ�� �����ϴ� ����� �� ����̴�.)
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

    // �÷��̾� ������
    public GameObject[] characters;

    // ������ �÷��̾�
    public GameObject player;

    // UI
    [SerializeField]
    private GameObject pauseGroupUI;    // �Ͻ����� �� Ȱ��ȭ�Ǵ� ��ư�׷�

    [SerializeField]
    private GameObject soundGroupUI;   // �Ͻ�����-���� �ɼ� ���� �� Ȱ��ȭ

    [SerializeField]
    private GameObject lobbyGroupUI;   // �Ͻ�����-�κ� ���� �� Ȱ��ȭ

    [SerializeField]
    private Slider playerHPBar; // �÷��̾� ü�¹�

    [SerializeField]
    private Slider bgmSlider;   // bgm �����̴�
    [SerializeField]
    private Slider seSlider;    // se �����̴�

    // �÷��̾� ü��
    public float playerMaxHP = 100f;  // �÷��̾� �ִ� ü��(100��)
    public float playerCurHP; // �÷��̾� ���� ü��
  
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // �ΰ��� ������ �̵� �� �κ� ������ ������ ���� ������ �����̴� ���� �°� ����.
        bgmSlider.value = SoundManager.instance.bgmSliderValue;
        SoundManager.instance.SetBgmVolume(SoundManager.instance.bgmSoundVolume);
        seSlider.value = SoundManager.instance.seSliderValue;
        SoundManager.instance.SetSeVolume(SoundManager.instance.seSoundVolume);

        // �κ� BGM ����
        SoundManager.instance.StopBGM("Lobby");
        // �ΰ��� BGM ���
        SoundManager.instance.PlayBGM("Ingame");

        // ����� ������ ĳ���� �ε��� ��������
        int selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);

        if(selectedCharacterIndex != -1 && selectedCharacterIndex < characters.Length)
        {
            // ������ ĳ���� �ε����� �ش��ϴ� ĳ���� ����
            player = Instantiate(characters[selectedCharacterIndex]);
        }

        // ü�� �ʱ�ȭ
        playerCurHP = playerMaxHP;
    }

    private void Update()
    {
        DecreaseHPOverTime();
        UpdateHPBar();
    }

    // �ǽð� ü�� ����
    private void DecreaseHPOverTime()
    {
        playerCurHP -= Time.deltaTime;      

        if (playerCurHP < 0f)
            playerCurHP = 0f;
    }

    // �����̴� ����
    private void UpdateHPBar()
    {
        playerHPBar.value = playerCurHP / playerMaxHP;
    }

    // �Ͻ����� ��ư
    public void OnPauseButtonClicked()
    {
        // �ð��� ���߰� �޴��� Ȱ��ȭ�Ѵ�.
        Time.timeScale = 0f;
        pauseGroupUI.SetActive(true);
    }

    // ����ϱ� ��ư
    public void OnIngameContinueButtonClicked()
    {
        // �޴��� ��Ȱ��ȭ�ϰ� �ٽ� �ð��� �帥��.
        pauseGroupUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // ���� �ɼ� ��ư
    public void OnIngameSoundButtonClicked()
    {
        // �Ͻ����� �׷� ��Ȱ��ȭ
        pauseGroupUI.SetActive(false);
        // ���� �ɼ� UI Ȱ��ȭ
        soundGroupUI.SetActive(true);
    }

    // �κ� ��ư
    public void OnIngameLobbyButtonClicked()
    {
        // �Ͻ����� �׷� ��Ȱ��ȭ
        pauseGroupUI.SetActive(false);
        // �κ� ��ȯ UI Ȱ��ȭ
        lobbyGroupUI.SetActive(true);
    }

    // �κ� ��ư ���� �׷�
    // �κ� Yes Ŭ�� ��
    public void OnLobbyYesButtonClicked()
    {
        // ���� ���������� ���ڿ��� �޴´�.
        string currentStage = SceneManager.GetActiveScene().name;
        Debug.Log(currentStage);
        // ���ڿ����� ���� ���������� ��ȣ�� �����Ѵ�.
        int currentStageNumber = int.Parse(currentStage.Substring(currentStage.Length - 1));
        // ���� �������� ������ �����Ѵ�.
        DataManager.instance.SaveStageData(currentStageNumber);

        // �ٽ� �ð��� �帣�� ��(�κ�� ���ư��ٰ� �ٽ� ������ ��� ���ߴ� ���� �߻�
        Time.timeScale = 1f;

        // �κ� BGM ���
        SoundManager.instance.PlayBGM("Lobby");

        // �κ� �̵�
        SceneManager.LoadScene("Lobby");
        
        // �ΰ��� BGM ����
        //SoundManager.instance.StopBGM("Ingame");
    }

    // �κ� No Ŭ�� or �ݱ� ��ư Ŭ�� ��
    public void OnLobbyNoButtonClicked()
    {
        // �κ� ��ȯ UI ��Ȱ��ȭ
        lobbyGroupUI.SetActive(false);
        // �Ͻ����� �׷� Ȱ��ȭ
        pauseGroupUI.SetActive(true);
    }

    // ���� �ɼ� â �ݱ� ��ư Ŭ�� ��
    public void OnSoundQuitButtonClicked()
    {
        // �ɼ� ��Ȱ��ȭ
        soundGroupUI.SetActive(false);
        // �Ͻ����� �׷� Ȱ��ȭ
        pauseGroupUI.SetActive(true);
    }

    // ��ư Ŭ�� ���� �Լ�(�ν�����-OnClick�� �߰�)
    public void PlayButtonClickSound()
    {
        SoundManager.instance.PlaySE("ButtonClick");
    }
}
