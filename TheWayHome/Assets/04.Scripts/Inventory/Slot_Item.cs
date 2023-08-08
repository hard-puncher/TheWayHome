using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot_Item : MonoBehaviour
{
    private Inventory inventory;

    public Item item;   // 획득한 아이템
    public int itemCount;   // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지

    public string itemName; // 아이템 이름
    public string itemDesc; // 아이템 설명

    [SerializeField] private TextMeshProUGUI text_Count;
    //[SerializeField] private GameObject go_CountImage;

    private void Start()
    {
        inventory = GameObject.Find("InventoryGroup").GetComponent<Inventory>();
    }

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        // 설명과 이름도 받아오기
        itemName = _item.itemName;
        itemDesc = _item.itemDesc;

        // 장비 아이템의 경우에만 개수를 안보이게 해준다.
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

    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        // 개수가 0이하이면 슬롯 삭제
        if (itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        //go_CountImage.SetActive(false);
    }

    // 슬롯 클릭 시 해당 슬롯의 아이템 이름과, 아이템 설명을 우측에 띄우는 함수
    public void ViewItemInfo()
    {
        inventory.selectedItemName.text = itemName;
        inventory.selectedItemDesc.text = itemDesc;
        inventory.selectedItemImage.sprite = itemImage.sprite;
    }
}
