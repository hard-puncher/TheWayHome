using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{
    // 아이템 유형
    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        ETC
    }

    public string itemName; // 아이템의 이름
    public string itemDesc; // 아이템의 설명
    public ItemType itemType;   // 아이템 유형
    public Sprite itemImage;    // 아이템의 이미지
    public GameObject itemPrefab;   // 아이템의 프리팹
}
