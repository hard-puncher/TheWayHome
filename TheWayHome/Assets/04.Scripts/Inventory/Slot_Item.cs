using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_Item : MonoBehaviour
{
    private Inventory inventory;

    public Item item;   // ȹ���� ������
    public int itemCount;   // ȹ���� �������� ����
    public Image itemImage; // �������� �̹���

    public string itemName; // ������ �̸�
    public string itemDesc; // ������ ����

    [SerializeField] private TextMeshProUGUI text_Count;
    //[SerializeField] private GameObject go_CountImage;

    private void Start()
    {
        inventory = GameObject.Find("InventoryGroup").GetComponent<Inventory>();
    }

    // ������ �̹����� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // �κ��丮�� ���ο� ������ ���� �߰�
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        // ����� �̸��� �޾ƿ���
        itemName = _item.itemName;
        itemDesc = _item.itemDesc;

        // ��� �������� ��쿡�� ������ �Ⱥ��̰� ���ش�.
        if(item.itemType != Item.ItemType.Equipment)
        {
            //go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "";
            //go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    // �ش� ������ ������ ���� ������Ʈ
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        // ������ 0�����̸� ���� ����
        if (itemCount <= 0)
            ClearSlot();
    }

    // �ش� ���� �ϳ� ����
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        //go_CountImage.SetActive(false);
    }

    // ���� Ŭ�� �� �ش� ������ ������ �̸���, ������ ������ ������ ���� �Լ�
    public void ViewItemInfo()
    {
        inventory.selectedItemName.text = itemName;
        inventory.selectedItemDesc.text = itemDesc;
        inventory.selectedItemImage.sprite = itemImage.sprite;
    }
}
