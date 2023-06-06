using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageData  // 스테이지 정보를 담는 클래스. Json으로 만들어줄 코드는 클래스 형태여야 한다.
{
    public int stageNumber;

    public StageData(int stageNumber)
    {
        this.stageNumber = stageNumber;
    }
}

public class DataManager : MonoBehaviour
{
    // 싱글톤 데이터 매니저
    public static DataManager instance;

    // 스테이지 정보를 저장할 경로
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

        // 싱글톤 데이터 매니저 인스턴스가 처음 생성될 때 경로를 지정
        savePath = Application.persistentDataPath + "/stageData.json";
    }

    // 스테이지 정보를 저장하는 함수(문자열 -> 제이슨)
    public void SaveStageData(int stageNumber)
    {
        StageData data = new StageData(stageNumber);
        string jsonData = JsonUtility.ToJson(data);

        // 파일에 데이터 저장
        File.WriteAllText(savePath, jsonData);

        Debug.Log(stageNumber + "가 저장되었습니다.");
    }

    // 저장된 스테이지 정보를 불러오는 함수(제이슨 -> 문자열)
    public int LoadStageData()
    {
        // 저장된 스테이지가 있을 경우
        if(File.Exists(savePath))
        {
            // 파일에서 데이터 읽기
            string jsonData = File.ReadAllText(savePath);
            StageData data = JsonUtility.FromJson<StageData>(jsonData);

            Debug.Log(data.stageNumber + "를 불러옵니다.");

            return data.stageNumber;
        }

        // 저장된 스테이지 정보가 없을 경우 기본값 반환
        else
        {
            Debug.Log("저장된 정보가 없습니다. 튜토리얼을 시작합니다.");

            return 1;   // 스테이지 1(튜토리얼)
        }
    }
}
