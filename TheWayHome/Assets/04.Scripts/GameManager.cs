using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �̱������� ����� �÷��̾� ������ ü��, UI�� �����Ѵ�.
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    // UI
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
        // ü�� �ʱ�ȭ
        playerCurHP = playerMaxHP;
    }

    // ���ӸŴ��� �ν��Ͻ��� ������ �� �ִ� ������Ƽ, static�̹Ƿ� �ٸ� Ŭ�������� ������ ȣ�� �����ϴ�.
    // ("������Ƽ"�� ������ ������ ���� �ܺο��� ������ �� �ֵ��� �ϸ鼭 ���ÿ� ĸ��ȭ�� �����ϴ� ����� �� ����̴�.)
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
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
}
