using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    // BGM 볼륨 조절
    public void SetBgmVolume(float value)
    {
        // 부드러운 조절을 위해 로그 연산 값 전달
        SoundManager.instance.audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
        // 씬 전환이 되어도 설정한 값이 유지되도록 싱글톤 객체인 사운드 매니저의 전역 변수에 저장
        SoundManager.instance.bgmSoundVolume = value;
        SoundManager.instance.bgmSliderValue = value;
    }

    // SE 볼륨 조절
    public void SetSeVolume(float value)
    {
        // 부드러운 조절을 위해 로그 연산 값 전달
        SoundManager.instance.audioMixer.SetFloat("SE", Mathf.Log10(value) * 20);
        // 씬 전환이 되어도 설정한 값이 유지되도록 싱글톤 객체인 사운드 매니저의 전역 변수에 저장
        SoundManager.instance.seSoundVolume = value;
        SoundManager.instance.seSliderValue = value;
    }
}
