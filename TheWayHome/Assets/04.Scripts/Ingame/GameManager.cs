using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Slider playerHPBar; // �÷��̾� ü�¹�

    // �÷��̾� ü��
    public float playerMaxHP = 100f;  // �÷��̾� �ִ� ü��(100��)
    public float playerCurHP; // �÷��̾� ���� ü��
  
    private void Awake()
    {
        if(instance == null)
        {
            // �� Ŭ������ �ν��Ͻ��� ź������ �� �������� instance�� ���ӸŴ��� �ν��Ͻ��� ������� �ʴٸ�, �ڽ��� �־��ش�.
            instance = this;

            // �� ��ȯ�� �Ǵ��� �ı����� �ʰ��Ѵ�.
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // ���� �� �̵��� �Ǿ��µ� �� ������ ���ӸŴ����� �����Ѵٸ�
            // ���ο� ���� ���ӸŴ����� �ı��Ѵ�.
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
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

    // �ɼ� ��ư
    public void OnIngameOptionButtonClicked()
    {
        // �ɼ� UI Ȱ��ȭ
    }

    // �κ� ��ư
    public void OnIngameLobbyButtonClicked()
    {
        // �κ� ��ȯ UI Ȱ��ȭ
    }
}
