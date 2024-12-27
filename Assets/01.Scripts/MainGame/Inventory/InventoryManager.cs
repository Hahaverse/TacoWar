using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventory; //인벤토리 패널
    public List<Image> itemSlots; //아이템 슬롯 리스트
    public List<Text> itemNames; //아이템 이름 리스트
    public List<Text> itemCounts; //아이템 개수 텍스트

    private void Start()
    {
        //인벤토리 패널 기본 비활성화
        inventory.SetActive(false);
        UpdateInventoryUI();
    }

    public void ToggleInventory() //인벤토리 패널 활/비활
    {
        Debug.Log("버튼 클릭");
        inventory.SetActive(!inventory.activeSelf);
        if (inventory.activeSelf)
        {
            UpdateInventoryUI();
        }
    }

    void UpdateInventoryUI() //정보 업데이트
    {
        List<Item> playerItems = DatabaseManager.Instance.playerItemList; //플레이어 아이템 리스트

        //슬롯 초기화
        for (int i = 0; i < itemSlots.Count; i++)
        {
            int itemID = 10001 + i; // 슬롯과 ID 매핑
            Item currentItem = playerItems.Find(item => item.itemID == itemID);

            if (currentItem != null && currentItem.itemCount > 0)
            {
                itemSlots[i].color = Color.white;
                itemCounts[i].text = currentItem.itemCount.ToString();
                itemNames[i].text = currentItem.itemName;
            }
            else
            {
                itemSlots[i].color = Color.black;
                itemCounts[i].text = "-";
                itemNames[i].text = "???";
            }
        }
    }

    public void AddItem(List<Item> inventory, int _itemID, int _count = 1) //아이템 추가
    {
        for(int i=0; i < DatabaseManager.Instance.itemList.Count; i++) //아이템 검색
        {
            if (_itemID == DatabaseManager.Instance.itemList[i].itemID)
            {
                for(int j=0; j < inventory.Count; j++) //소지품 확인
                {
                    if (inventory[j].itemID == _itemID) //있으면 증가
                    {
                        inventory[j].itemCount += _count;
                        return;
                    }
                    inventory.Add(DatabaseManager.Instance.itemList[i]); //없으면 추가
                    inventory[inventory.Count - 1].itemCount = _count;
                    return;
                }
            }
        }
    }

    // Special 아이템을 랜덤으로 선택하여 반환하는 함수
    public Item GetRandomSpecialItem(List<Item> itemList)
    {
        List<Item> specialItems = itemList.Where(item => item.itemType == Item.ItemType.Special && item.itemCount > 0).ToList();
        if (specialItems.Count == 0) return null;

        int randomIndex = Random.Range(0, specialItems.Count);
        return specialItems[randomIndex];
    }

    // 아이템 제거 함수
    public void RemoveItem(List<Item> itemList, int itemID, int count)
    {
        Item item = itemList.FirstOrDefault(i => i.itemID == itemID);
        if (item != null)
        {
            item.itemCount -= count;
            if (item.itemCount <= 0)
            {
                itemList.Remove(item);
            }
        }
    }
}
