using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // 아이템 인벤토리
    [SerializeField] private GameObject inventory_Item;
    // 스킬 인벤토리
    [SerializeField] private GameObject inventory_Skill;

    // 아이템 인벤토리의 슬롯들 배열
    public Slot_Item[] slots_Item;
    // 스킬 인벤토리의 슬롯들 배열

    // 아이템 인벤토리 - 선택한 아이템의 이름란
    public TextMeshProUGUI selectedItemName;
    // 아이템 인벤토리 - 선택한 아이템의 설명란
    public TextMeshProUGUI selectedItemDesc;
    // 아이템 인벤토리 - 선택한 아이템의 이미지
    public Image selectedItemImage;
    
    private void Start()
    {
        // 아이템 인벤토리의 슬롯들 연결
        slots_Item = inventory_Item.GetComponentsInChildren<Slot_Item>();
        // 스킬 인벤토리의 슬롯들 연결

    }

    private void Update()
    {
        // 아이템 인벤토리 on/off
        if (Input.GetKeyDown(KeyCode.I))
        {
            ItemInventoryOnOff();
        }
        // 스킬 인벤토리 on/off
        if(Input.GetKeyDown(KeyCode.K))
        {
            SkillInventoryOnOff();
        }
    }

    // 아이템 인벤토리 버튼
    public void ItemInventoryOnOff()
    {
        if(inventory_Item.activeSelf)
            inventory_Item.SetActive(false);
        else
        {
            if(inventory_Skill.activeSelf)
                inventory_Skill.SetActive(false);
            inventory_Item.SetActive(true);

            // 슬롯을 클릭하지 않을 시 기본적으로 설명란에 표시되는 정보는 슬롯 0번에 대한 정보를 기본으로 출력한다.
            if (slots_Item[0].item != null)
            {
                selectedItemName.text = slots_Item[0].itemName;
                selectedItemDesc.text = slots_Item[0].itemDesc;
                selectedItemImage.sprite = slots_Item[0].itemImage.sprite;
            }
        }          
    }

    // 스킬 인벤토리 버튼
    public void SkillInventoryOnOff()
    {
        if(inventory_Skill.activeSelf)
            inventory_Skill.SetActive(false);
        else
        {
            if(inventory_Item.activeSelf)
                inventory_Item.SetActive(false);
            inventory_Skill.SetActive(true);
        }            
    }

    // 아이템 습득하기 (아이템을 주웠을 때 호출)
    public void AcquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for(int i = 0; i < slots_Item.Length; i++)
            {
                if (slots_Item[i].item != null)
                {
                    // 같은 종류의 아이템이 있는지 검사, 있으면 아이템 개수 +
                    if (slots_Item[i].item.itemName == _item.itemName)
                    {
                        slots_Item[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        // 같은 종류의 아이템이 없다면 아이템을 저장할 새로운 슬롯을 마련
        for(int i = 0; i < slots_Item.Length; i++)
        {
            // 빈 슬롯을 찾았다면 해당 슬롯에 새로운 아이템 추가
            if (slots_Item[i].item == null)
            {
                slots_Item[i].AddItem(_item, _count);
                return;
            }
        }
    }
}
