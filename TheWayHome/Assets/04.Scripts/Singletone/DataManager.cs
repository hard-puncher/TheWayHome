using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageData  // �������� ������ ��� Ŭ����. Json���� ������� �ڵ�� Ŭ���� ���¿��� �Ѵ�.
{
    public int stageNumber;

    public StageData(int stageNumber)
    {
        this.stageNumber = stageNumber;
    }
}

public class DataManager : MonoBehaviour
{
    // �̱��� ������ �Ŵ���
    public static DataManager instance;

    // �������� ������ ������ ���
    private string savePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);

        // �̱��� ������ �Ŵ��� �ν��Ͻ��� ó�� ������ �� ��θ� ����
        savePath = Application.persistentDataPath + "/stageData.json";
    }

    // �������� ������ �����ϴ� �Լ�(���ڿ� -> ���̽�)
    public void SaveStageData(int stageNumber)
    {
        StageData data = new StageData(stageNumber);
        string jsonData = JsonUtility.ToJson(data);

        // ���Ͽ� ������ ����
        File.WriteAllText(savePath, jsonData);

        Debug.Log(stageNumber + "�� ����Ǿ����ϴ�.");
    }

    // ����� �������� ������ �ҷ����� �Լ�(���̽� -> ���ڿ�)
    public int LoadStageData()
    {
        // ����� ���������� ���� ���
        if(File.Exists(savePath))
        {
            // ���Ͽ��� ������ �б�
            string jsonData = File.ReadAllText(savePath);
            StageData data = JsonUtility.FromJson<StageData>(jsonData);

            Debug.Log(data.stageNumber + "�� �ҷ��ɴϴ�.");

            return data.stageNumber;
        }

        // ����� �������� ������ ���� ��� �⺻�� ��ȯ
        else
        {
            Debug.Log("����� ������ �����ϴ�. Ʃ�丮���� �����մϴ�.");

            return 1;   // �������� 1(Ʃ�丮��)
        }
    }
}
