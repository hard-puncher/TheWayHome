using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class Sound
{
    public string name; // 효과음 or 배경음 이름
    public AudioClip clip;  // 효과음 or 배경음
}

public class SoundManager : MonoBehaviour
{
    // 싱글톤 사운드 매니저
    public static SoundManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public Sound[] effectSounds;    // 효과음 오디오 클립들
    public Sound[] bgmSounds;   // 배경음 오디오 클립들

    public AudioSource audioSourceBGM;  // 배경음 재생기(BGM은 한번에 한곡만 재생되므로 배열X)
    public AudioSource[] audioSourceSE; // 효과음 재생기(효과음은 한번에 여러개 재생될 수 있으므로 배열)

    public string[] playSoundName;  // 재생 중인 효과음 이름 배열

    // 사운드 조절
    // 오디오 믹서
    public AudioMixer audioMixer;

    // 씬 전환이 되어도 설정한 볼륨 정보가 유지되도록 저장할 변수
    public float bgmSliderValue;    // bgm 슬라이더 값
    public float bgmSoundVolume;    // bgm 볼륨
    public float seSliderValue;     // se 슬라이더 값
    public float seSoundVolume;     // se 볼륨

    private void Start()
    {
        playSoundName = new string[audioSourceSE.Length];   // 효과음 재생기의 수만큼 초기화

        // 로비 BGM 재생
        PlayBGM("Lobby");

        bgmSliderValue = 1;
        bgmSoundVolume = 1;
        seSliderValue = 1;
        seSoundVolume = 1;
    }

    // 효과음 재생 함수
    public void PlaySE(string _name)
    {
        for(int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for(int j = 0; j < audioSourceSE.Length; j++)
                {
                    if (!audioSourceSE[j].isPlaying)
                    {
                        audioSourceSE[j].clip = effectSounds[i].clip;
                        audioSourceSE[j].Play();
                        playSoundName[j] = effectSounds[i].name;
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용 중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    // 배경음 재생 함수
    public void PlayBGM(string _name)
    {
        for(int i = 0; i < bgmSounds.Length; i++)
        {
            if(_name == bgmSounds[i].name)
            {
                audioSourceBGM.clip = bgmSounds[i].clip;
                audioSourceBGM.Play();
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    // 배경음 멈춤 함수
    public void StopBGM(string _name)
    {
        audioSourceBGM.Stop();
    }

    // 재생 중인 모든 효과음 멈춤 함수
    public void StopAllSE()
    {
        for(int i = 0; i < audioSourceSE.Length; i++)
        {
            audioSourceSE[i].Stop();
        }
    }

    // 효과음 멈춤 함수
    public void StopSE(string _name)
    {
        for(int i = 0; i < audioSourceSE.Length; i++)
        {
            if (playSoundName[i] ==  _name)
            {
                audioSourceSE[i].Stop();
                break;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }

    // BGM 볼륨 조절
    public void SetBgmVolume(float value)
    {
        // 부드러운 조절을 위해 로그 연산 값 전달
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    // SE 볼륨 조절
    public void SetSeVolume(float value)
    {
        audioMixer.SetFloat("SE", Mathf.Log10(value) * 20);
    }
}
