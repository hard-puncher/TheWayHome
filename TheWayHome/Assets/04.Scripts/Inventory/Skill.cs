using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "New Skill/skill")]
public class Skill : ScriptableObject
{
    // ��ų ����
    public enum ItemType
    {
        DoubleJump,
        WallClimb,
        AirDash
    }

    public string skillName; // ��ų�� �̸�
    public string skillDesc; // ��ų�� ����
    public ItemType skillType;   // ��ų ����
    public Sprite skillImage;    // ��ų�� �̹���
    public GameObject skillPrefab;   // ��ų�� ������
}
