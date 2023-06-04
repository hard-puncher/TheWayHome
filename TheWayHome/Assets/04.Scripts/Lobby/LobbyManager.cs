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
    // ĳ���� ���� �� Ȱ��ȭ�Ǵ� ���� ���� ��ư
    public GameObject gameStartButton;
    // ������ ĳ���� ��ȣ
    private int selectedCharacterIndex = -1;

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

    // ���ӽ���: ���� �޴�-play��ư Ŭ�� ��
    public void OnPlayButtonClicked()
    {
        // ���۸޴��� ��Ȱ��ȭ�ϰ� ĳ���� ���� UI�� Ȱ��ȭ�Ѵ�.
        menuGroupUI.SetActive(false);
        characterSelectionUI.SetActive(true);
    }

    // �̾��ϱ�: ���� �޴�-continue��ư Ŭ�� ��
    public void OnContinueButtonClicked()
    {

    }

    // �ɼ�: ���� �޴�-option��ư Ŭ�� ��
    public void OnOptionButtonClicked()
    {

    }

    // ����: ���� �޴�-quit��ư Ŭ�� ��
    public void OnQuitButtonClicked()
    {

    }

    // ĳ���� ����
    public void OnCharacterSelected(int characterIndex)
    {
        // ������ ĳ���� �ε��� ����
        selectedCharacterIndex = characterIndex;

        // ���� ���� ��ư Ȱ��ȭ
        gameStartButton.SetActive(true);
    }

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
}
