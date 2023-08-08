using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "New Skill/skill")]
public class Skill : ScriptableObject
{
    // 스킬 유형
    public enum ItemType
    {
        DoubleJump,
        WallClimb,
        AirDash
    }

    public string skillName; // 스킬의 이름
    public string skillDesc; // 스킬의 설명
    public ItemType skillType;   // 스킬 유형
    public Sprite skillImage;    // 스킬의 이미지
    public GameObject skillPrefab;   // 스킬의 프리팹
}
