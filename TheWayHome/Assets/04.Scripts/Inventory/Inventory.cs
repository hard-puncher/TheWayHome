using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // ������ �κ��丮
    [SerializeField] private GameObject inventory_Item;
    // ��ų �κ��丮
    [SerializeField] private GameObject inventory_Skill;

    // ������ �κ��丮�� ���Ե� �迭
    public Slot_Item[] slots_Item;
    // ��ų �κ��丮�� ���Ե� �迭

    // ������ �κ��丮 - ������ �������� �̸���
    public TextMeshProUGUI selectedItemName;
    // ������ �κ��丮 - ������ �������� �����
    public TextMeshProUGUI selectedItemDesc;
    // ������ �κ��丮 - ������ �������� �̹���
    public Image selectedItemImage;
    
    private void Start()
    {
        // ������ �κ��丮�� ���Ե� ����
        slots_Item = inventory_Item.GetComponentsInChildren<Slot_Item>();
        // ��ų �κ��丮�� ���Ե� ����

    }

    private void Update()
    {
        // ������ �κ��丮 on/off
        if (Input.GetKeyDown(KeyCode.I))
        {
            ItemInventoryOnOff();
        }
        // ��ų �κ��丮 on/off
        if(Input.GetKeyDown(KeyCode.K))
        {
            SkillInventoryOnOff();
        }
    }

    // ������ �κ��丮 ��ư
    public void ItemInventoryOnOff()
    {
        if(inventory_Item.activeSelf)
            inventory_Item.SetActive(false);
        else
        {
            if(inventory_Skill.activeSelf)
                inventory_Skill.SetActive(false);
            inventory_Item.SetActive(true);

            // ������ Ŭ������ ���� �� �⺻������ ������� ǥ�õǴ� ������ ���� 0���� ���� ������ �⺻���� ����Ѵ�.
            if (slots_Item[0].item != null)
            {
                selectedItemName.text = slots_Item[0].itemName;
                selectedItemDesc.text = slots_Item[0].itemDesc;
                selectedItemImage.sprite = slots_Item[0].itemImage.sprite;
            }
        }          
    }

    // ��ų �κ��丮 ��ư
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

    // ������ �����ϱ� (�������� �ֿ��� �� ȣ��)
    public void AcquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for(int i = 0; i < slots_Item.Length; i++)
            {
                if (slots_Item[i].item != null)
                {
                    // ���� ������ �������� �ִ��� �˻�, ������ ������ ���� +
                    if (slots_Item[i].item.itemName == _item.itemName)
                    {
                        slots_Item[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        // ���� ������ �������� ���ٸ� �������� ������ ���ο� ������ ����
        for(int i = 0; i < slots_Item.Length; i++)
        {
            // �� ������ ã�Ҵٸ� �ش� ���Կ� ���ο� ������ �߰�
            if (slots_Item[i].item == null)
            {
                slots_Item[i].AddItem(_item, _count);
                return;
            }
        }
    }
}
