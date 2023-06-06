using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    // BGM ���� ����
    public void SetBgmVolume(float value)
    {
        // �ε巯�� ������ ���� �α� ���� �� ����
        SoundManager.instance.audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
        // �� ��ȯ�� �Ǿ ������ ���� �����ǵ��� �̱��� ��ü�� ���� �Ŵ����� ���� ������ ����
        SoundManager.instance.bgmSoundVolume = value;
        SoundManager.instance.bgmSliderValue = value;
    }

    // SE ���� ����
    public void SetSeVolume(float value)
    {
        // �ε巯�� ������ ���� �α� ���� �� ����
        SoundManager.instance.audioMixer.SetFloat("SE", Mathf.Log10(value) * 20);
        // �� ��ȯ�� �Ǿ ������ ���� �����ǵ��� �̱��� ��ü�� ���� �Ŵ����� ���� ������ ����
        SoundManager.instance.seSoundVolume = value;
        SoundManager.instance.seSliderValue = value;
    }
}
