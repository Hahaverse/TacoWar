using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemID; //아이템 ID
    public string itemName; //아이템 이름
    public int itemCount; //아이템 갯수
    public ItemType itemType; //아이템 타입

    public enum ItemType //아이템 타입
    {
        Special,
        Normal
    }

    public Item(int _itemID, string _itemName, ItemType _itemType,int _itemCount = 1)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemType = _itemType;
        itemCount = _itemCount;
    }
}
