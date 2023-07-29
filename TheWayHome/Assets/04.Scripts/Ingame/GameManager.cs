using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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

    public GameObject gameOverUI;  // �÷��̾� ��� �� Ȱ��ȭ

    [SerializeField]
    private Slider playerHPBar; // �÷��̾� ü�¹�

    [SerializeField]
    private Slider bgmSlider;   // bgm �����̴�
    [SerializeField]
    private Slider seSlider;    // se �����̴�

    public GameObject backGround_Day;
    public GameObject backGround_Night;

    // ü�� �� ü�� UI
    [SerializeField] private GameObject go_HpUI;    // hp�� grid layout group���� ������ �θ� ��ü.
    [SerializeField] private GameObject hpOriginalPref; // ������ �Ǵ� hp ������.
    public int curHp;
    public int maxHp;
    [SerializeField] private Image[] hpImages;  // �̹��� �迭(ĳ���� �� ü�� �ѷ���ŭ �ʱ�ȭ)
    [SerializeField] private Sprite hpDefault;    // �⺻ ��Ʈ �̹���
    [SerializeField] private Sprite hpDecrease;   // ü�� ���� �� ��Ʈ �̹���

    private void Awake()
    {
        instance = this;     
    }

    private void Start()
    {
        // ���� ���� �� ���� �ð��� 6~18�ö�� �� �����, 18~6�ö�� �� ����� Ȱ��ȭ�Ѵ�.
        Debug.Log("���� �ð��� " + DateTime.Now.Hour);
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

        if (selectedCharacterIndex != -1 && selectedCharacterIndex < characters.Length)
        {
            // ������ ĳ���� �ε����� �ش��ϴ� ĳ���� ����
            player = Instantiate(characters[selectedCharacterIndex]);
        }

        // ĳ���ͺ� ���� ü������ �ʱ�ȭ.
        maxHp = player.GetComponent<PlayerController>().maxHp;
        curHp = maxHp;
        hpImages = new Image[maxHp];    // �̹����� ������ ĳ������ maxHp�� �°� �ʱ�ȭ.

        for (int i = 0; i < hpImages.Length; i++)
        {
            hpImages[i] = Instantiate(hpOriginalPref, go_HpUI.transform).GetComponent<Image>();
            hpImages[i].sprite = hpDefault; // �⺻ ��Ʈ �̹����� �ʱ�ȭ
        }
    }

    private void Update()
    {
        backGround_Day.transform.position = player.transform.position;
        backGround_Night.transform.position = player.transform.position;
    }

    // ü�� ���� �Լ�(���� ���� ��)
    public void IncreaseHP(int _item)
    {
        curHp += _item;

        for (int i = 0; i < maxHp; i++)
        {
            if (i < curHp)
            {
                hpImages[i].sprite = hpDefault;
            }
            else
            {
                hpImages[i].sprite = hpDecrease;
            }
        }
    }

    // ü�� ���� �Լ�(��, ��ֹ��� �ǰ� �� ȣ��)
    public void DecreaseHP(int _damage)
    {
        curHp -= _damage;

        for(int i = 0; i < maxHp; i++)
        {
            if(i < curHp)
            {
                hpImages[i].sprite = hpDefault;
            }
            else
            {
                hpImages[i].sprite = hpDecrease;
            }
        }
    }

    // �Ͻ����� ��ư
    public void OnPauseButtonClicked()
    {
        // ���ӿ��� ui�� ��Ȱ��ȭ �����϶���
        if(!gameOverUI.activeSelf)
        {
            // �ð��� ���߰� �޴��� Ȱ��ȭ�Ѵ�.
            Time.timeScale = 0f;
            pauseGroupUI.SetActive(true);
        }     
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

    // ���� ���� - �絵�� ��ư
    public void OnRetryButtonClicked()
    {
        // ���� ���������� ���ڿ��� �޴´�.
        string currentStage = SceneManager.GetActiveScene().name;

        // ���ڿ����� ���� ���������� ��ȣ�� �����Ѵ�.
        int currentStageNumber = int.Parse(currentStage.Substring(currentStage.Length - 1));

        // ���� ���������� �ε��Ѵ�.
        SceneManager.LoadScene("Stage" + currentStageNumber);

        Time.timeScale = 1f;
    }
}
