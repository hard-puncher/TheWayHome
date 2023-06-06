using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class Sound
{
    public string name; // ȿ���� or ����� �̸�
    public AudioClip clip;  // ȿ���� or �����
}

public class SoundManager : MonoBehaviour
{
    // �̱��� ���� �Ŵ���
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

    public Sound[] effectSounds;    // ȿ���� ����� Ŭ����
    public Sound[] bgmSounds;   // ����� ����� Ŭ����

    public AudioSource audioSourceBGM;  // ����� �����(BGM�� �ѹ��� �Ѱ ����ǹǷ� �迭X)
    public AudioSource[] audioSourceSE; // ȿ���� �����(ȿ������ �ѹ��� ������ ����� �� �����Ƿ� �迭)

    public string[] playSoundName;  // ��� ���� ȿ���� �̸� �迭

    // ���� ����
    // ����� �ͼ�
    public AudioMixer audioMixer;

    // �� ��ȯ�� �Ǿ ������ ���� ������ �����ǵ��� ������ ����
    public float bgmSliderValue;    // bgm �����̴� ��
    public float bgmSoundVolume;    // bgm ����
    public float seSliderValue;     // se �����̴� ��
    public float seSoundVolume;     // se ����

    private void Start()
    {
        playSoundName = new string[audioSourceSE.Length];   // ȿ���� ������� ����ŭ �ʱ�ȭ

        // �κ� BGM ���
        PlayBGM("Lobby");

        bgmSliderValue = 1;
        bgmSoundVolume = 1;
        seSliderValue = 1;
        seSoundVolume = 1;
    }

    // ȿ���� ��� �Լ�
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
                Debug.Log("��� ���� AudioSource�� ��� ���Դϴ�.");
                return;
            }
        }
        Debug.Log(_name + "���尡 SoundManager�� ��ϵ��� �ʾҽ��ϴ�.");
    }

    // ����� ��� �Լ�
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
        Debug.Log(_name + "���尡 SoundManager�� ��ϵ��� �ʾҽ��ϴ�.");
    }

    // ����� ���� �Լ�
    public void StopBGM(string _name)
    {
        audioSourceBGM.Stop();
    }

    // ��� ���� ��� ȿ���� ���� �Լ�
    public void StopAllSE()
    {
        for(int i = 0; i < audioSourceSE.Length; i++)
        {
            audioSourceSE[i].Stop();
        }
    }

    // ȿ���� ���� �Լ�
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
        Debug.Log("��� ����" + _name + "���尡 �����ϴ�.");
    }

    // BGM ���� ����
    public void SetBgmVolume(float value)
    {
        // �ε巯�� ������ ���� �α� ���� �� ����
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    // SE ���� ����
    public void SetSeVolume(float value)
    {
        audioMixer.SetFloat("SE", Mathf.Log10(value) * 20);
    }
}
