using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //싱글톤
    #region 싱글톤
    public static GameManager Instance;
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

    public bool isEventActive = false; //이벤트 활성화 상태

    public List<Item> itemRewardList = new List<Item>(); //보상리스트
    public List<Vector2Int> rewardTiles = new List<Vector2Int>(); //보상타일 리스트

    public Text turnText; //턴텍스트

    private void Update()
    {
        if (turnText != null)
        {
            turnText.text = TurnData.gameTurn.ToString();
        }
    }
}
