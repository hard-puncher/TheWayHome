using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LobbyManager : MonoBehaviour
{
    // ���� �޴� �׷�
    public GameObject menuGroupUI;
    
    // ĳ���� ���� â
    public GameObject characterSelectionUI;
    
    // ó������ -> ĳ���� ���� �� Ȱ��ȭ�Ǵ� ���� ���� ��ư
    public GameObject gameStartButton;

    // �̾��ϱ� -> ĳ���� ���� �� Ȱ��ȭ�Ǵ� �̾��ϱ� ��ư
    public GameObject continueStartButton;

    // ó�����͸� ��������, �̾��ϱ⸦ ���������� üũ�ؼ� ĳ���͸� ����� ��, ù ���������� ���� ��ư�� Ȱ��ȭ ���� ���� ���������� ���� ��ư�� Ȱ��ȭ ���� ������ ����.
    public bool isContinueClicked = false;

    // ������ ĳ���� ��ȣ
    private int selectedCharacterIndex = -1;
    
    // �ɼ� �޴� �׷�(���� �޴�-�ɼ� ���� �� Ȱ��ȭ)
    public GameObject optionGroupUI;

    // ���� �޴� �׷�(���� �޴�-���� ���� �� Ȱ��ȭ)
    public GameObject quitGroupUI;

    // ���� ���� �׷�(���۸޴�-�ɼ�-���� ���� ���� �� Ȱ��ȭ)
    public GameObject accountResetGroupUI;

    // ���� �޴� ������ UI Ȱ��ȭ �� ���� �޴� ��ư����� ��ȣ�ۿ��� �����ϱ� ���� �÷���
    private bool isLowerGroupUIOn;

    // ���� �����̴�
    public Slider bgmSlider;
    public Slider seSlider;
   
    private void Awake()
    {
        // ���� ���� �� ���� �ð��� 6~18�ö�� �� �����, 18~6�ö�� �� ����� Ȱ��ȭ�Ѵ�.
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

    private void Start()
    {
        // �κ� ������ �̵� �� �ΰ��� ������ ������ ���� ������ �����̴� ���� �°� ����.
        bgmSlider.value = SoundManager.instance.bgmSliderValue;
        SoundManager.instance.SetBgmVolume(SoundManager.instance.bgmSoundVolume);
        seSlider.value = SoundManager.instance.seSliderValue;
        SoundManager.instance.SetSeVolume(SoundManager.instance.seSoundVolume);
    }

    // ���ӽ���: ���� �޴�-play��ư Ŭ�� ��
    public void OnPlayButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;

        // ���۸޴��� ��Ȱ��ȭ�ϰ� ĳ���� ���� UI(ó������ ����)�� Ȱ��ȭ�Ѵ�.
        menuGroupUI.SetActive(false);
        characterSelectionUI.SetActive(true);
    }

    // �̾��ϱ�: ���� �޴�-continue��ư Ŭ�� ��
    public void OnContinueButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;

        // �̾��ϱ� ��ư�� �������Ƿ� true�� �ٲ��ش�.
        isContinueClicked = true;

        // ���۸޴��� ��Ȱ��ȭ�ϰ� ĳ���� ���� UI(�̾��ϱ� ����)�� Ȱ��ȭ�Ѵ�.
        menuGroupUI.SetActive(false);
        characterSelectionUI.SetActive(true);
    }

    // �ɼ�: ���� �޴�-option��ư Ŭ�� ��
    public void OnOptionButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;

        optionGroupUI.SetActive(true);  // ui Ȱ��ȭ
        isLowerGroupUIOn = true;    // ���۸޴� �׷���� ��ȣ�ۿ� ����
    }

    // ����: ���� �޴�-quit��ư Ŭ�� ��
    public void OnQuitButtonClicked()
    {
        if (isLowerGroupUIOn)
            return;

        quitGroupUI.SetActive(true);    // ui Ȱ��ȭ
        isLowerGroupUIOn = true;    // ���۸޴� �׷���� ��ȣ�ۿ� ����
    }

    // ĳ���� ����â���� �ڷΰ��� ��ư Ŭ�� ��
    public void OnBackButtonClicked()
    {
        characterSelectionUI.SetActive(false);
        menuGroupUI.SetActive(true);
    }

    // ó������ -> ���ӽ��� ��ư -> ĳ���� ����
    public void OnCharacterSelected(int characterIndex)
    {
        // ������ ĳ���� �ε��� ����
        selectedCharacterIndex = characterIndex;

        // ���� ���� ��ư Ȱ��ȭ(�̾��ϱ⸦ ������ ������ �̾��ϱ� ��ư�� Ȱ��ȭ�ϰ�, ó�����͸� ������ ������ ó������ ��ư�� Ȱ��ȭ�Ѵ�.)
        if (!isContinueClicked)
            gameStartButton.SetActive(true);
        else
            continueStartButton.SetActive(true);
    }

    // ó������ -> ĳ���� ���� -> ���� ��ŸƮ ��ư Ŭ�� ��
    public void OnGameStartButtonClicked()
    {
        // ĳ���͸� �����ߴٸ�
        if(selectedCharacterIndex != -1)
        {
            // ������ ĳ���� �ε����� �ΰ��� ������ ����
            SceneManager.LoadScene("Stage0");
            // ������ ĳ���� ������ ��⿡ ����
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        }
    }

    // �̾��ϱ� -> ĳ���� ���� -> �̾��ϱ� ��ư Ŭ�� ��
    public void OnGameContinueButtonClicked()
    {
        // ĳ���͸� �����ߴٸ�
        if (selectedCharacterIndex != -1)
        {
            // �ٽ� �κ�� ���� ���� ���� �÷��׸� �ʱ�ȭ
            isContinueClicked = false;
            // ����� �ְ� �������� ���� �޾ƿ���
            int saveStage = DataManager.instance.LoadStageData();
            // ����� �� �ε�
            SceneManager.LoadScene("Stage" + saveStage);
            // ������ ĳ���� ������ ��⿡ ����
            PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
        }
    }

    // ���� �ʱ�ȭ ��ư Ŭ�� ��
    public void OnAccountResetButtonClicked()
    {
        optionGroupUI.SetActive(false); // �ɼ� â ��Ȱ��ȭ
        accountResetGroupUI.SetActive(true);    // ���� �ʱ�ȭ â Ȱ��ȭ
    }

    // ���� �ʱ�ȭ Yes Ŭ��
    public void OnAccountResetYesButtonClicked()
    {
        // ��� ���̺� ������ �ʱ�ȭ
        DataManager.instance.DeleteAllSaveData();

        // ���� �ʱ�ȭ â ��Ȱ��ȭ
        accountResetGroupUI.SetActive(false);

        // �ɼ� â Ȱ��ȭ
        optionGroupUI.SetActive(true);
    }

    // ���� �ʱ�ȭ No Ŭ�� or ���� �ʱ�ȭ â �ݱ� Ŭ�� ��
    public void OnAccountResetNoButtonClicked()
    {
        accountResetGroupUI.SetActive(false);   // ���� �ʱ�ȭ â ��Ȱ��ȭ
        optionGroupUI.SetActive(true);  // �ɼ� â Ȱ��ȭ
    }

    // �ɼ� â �ݱ� ��ư Ŭ�� ��
    public void OnOptionQuitButtonClicked()
    {
        optionGroupUI.SetActive(false);
        isLowerGroupUIOn = false;
    }

    // ���� ��ư ���� �׷�
    // ���� Yes Ŭ�� ��
    public void OnQuitYesButtonClicked()
    {
        Quit();
    }

    // ���� No Ŭ�� �� or ���� â �ݱ� ��ư Ŭ�� ��
    public void OnQuitNoButtonClicked()
    {
        quitGroupUI.SetActive(false);
        isLowerGroupUIOn = false;
    }

    // ��ư Ŭ�� ���� �Լ�(�ν�����-OnClick�� �߰�)
    public void PlayButtonClickSound()
    {
        SoundManager.instance.PlaySE("ButtonClick");
    }

    // ����Ƽ �����Ϳ����� ������ �� �ֵ��� ���� ���� �Լ�
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;    // �÷��̸�� ����
#endif
        Application.Quit();
    }
}
