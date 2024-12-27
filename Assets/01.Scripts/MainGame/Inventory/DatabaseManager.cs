using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 데이터베이스
public class DatabaseManager : MonoBehaviour
{
    #region 싱글톤
    public static DatabaseManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public List<Item> itemList = new List<Item>(); //전체 아이템 리스트
    public List<Item> playerItemList = new List<Item>(); //플레이어 아이템 리스트
    public List<Item> enemyItemList = new List<Item>(); //적 아이템 리스트

    private void Start()
    {
        itemList.Add(new Item(10001, "근미래 디바이스", Item.ItemType.Special));
        itemList.Add(new Item(10002, "뜨거운 열정", Item.ItemType.Special));
        itemList.Add(new Item(10003, "가쓰오부시", Item.ItemType.Normal));
        itemList.Add(new Item(10004, "토핑 파", Item.ItemType.Normal));
        itemList.Add(new Item(10005, "폭탄치즈", Item.ItemType.Normal));

        playerItemList.Add(new Item(10002, "뜨거운 열정", Item.ItemType.Special));
    }
}
