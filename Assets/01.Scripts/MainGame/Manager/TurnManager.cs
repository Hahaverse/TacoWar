using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public RectTransform playerBtnUI; //버튼 UI
    public RectTransform playerRenderUI; //렌더 UI
    public GameObject playerDiceUI; //주사위 UI
    public GameObject playerDiceBtnUI; //주사위버튼 UI

    public Camera playerCamera; //플레이어 턴 카메라
    public Camera otherCamera; //나머지 카메라

    public CanvasGroup fade; //페이드

    List<TurnData.Turn> turnOrder; //턴 순서 리스트
    int currentTurnIndex = 0; //현재 턴 리스트
    int itemsPerTurn = 3; //매 턴 생성할 아이템 수

    PlayerController playerController;
    DiceRoll diceroll;

    MapManager mapmanager;
    InventoryManager inventorymanager;

    //임시
    Enemy enemy;
    bool isEnemyTurn = false; //적 턴 진행 상태

    private void Start()
    {
        Debug.Log("TurnManager Start 실행됨");
        playerController = FindObjectOfType<PlayerController>();
        diceroll = FindObjectOfType<DiceRoll>();
        enemy = FindObjectOfType<Enemy>();
        mapmanager = FindObjectOfType<MapManager>();
        inventorymanager = FindObjectOfType<InventoryManager>();

        //턴 초기화
        turnOrder = new List<TurnData.Turn>
        {
            TurnData.Turn.ItemSetup,
            TurnData.Turn.Player,
            TurnData.Turn.Enemy,
            TurnData.Turn.ItemDistribution
        };

        ChangeTurn(turnOrder[currentTurnIndex]); //시작 턴: 아이템 설정
    }

    void ChangeTurn(TurnData.Turn newTurn) //턴 설정 함수
    {
        Debug.Log(newTurn);
        Debug.Log($"isEnemyTurn 상태: {isEnemyTurn}");
        TurnData.currentTurn = newTurn; //현재 턴 변경
        switch (newTurn)
        {
            case TurnData.Turn.ItemSetup: //아이템 설정 턴
                HighlightRandomTiles();
                break;
            case TurnData.Turn.Player: //플레이어 턴이면
                SetActiveCamera(playerCamera);
                StartPlayerTurn(); //플레이어 턴 시작
                break;
            case TurnData.Turn.Enemy:
                SetActiveCamera(otherCamera);
                StartEnemyTurn();
                break;
            case TurnData.Turn.ItemDistribution:
                DistributeItems(); //아이템 지급 단계
                break;
            default:
                break;
        }
    }

    void SetActiveCamera(Camera activeCamera) //카메라 전환
    {
        if (playerCamera != null) playerCamera.gameObject.SetActive(false);
        if (otherCamera != null) otherCamera.gameObject.SetActive(false);

        activeCamera.gameObject.SetActive(true);
    }

    void StartPlayerTurn() //플레이어 턴 시작
    { 
        //카메라 이동 필요
        Debug.Log("플레이어의 턴");
        //UI 등장
        MoveUI(playerBtnUI, 0, 0.5f);
        MoveUI(playerRenderUI, 0, 0.5f);
    }

    void StartEnemyTurn() //에너미 턴 시작
    {
        Debug.Log("적의 턴");
        isEnemyTurn = true;
        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        yield return StartCoroutine(enemy.TakeTurn()); //적의 턴 실행
        yield return new WaitForSeconds(0.5f); //대기
        isEnemyTurn = false;
        EndTurn(); //다음 턴으로 전환
    }

    public void EndTurn() //턴 종료
    {
        Debug.Log("턴 종료");

        //다음 턴 전환
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

        if (currentTurnIndex == 0) //한 턴 사이클 완료
        {
            TurnData.gameTurn--; //게임 턴 감소
            if (TurnData.gameTurn <= 0)
            {
                Debug.Log("게임 종료");
                TurnData.playerFinalScore = DatabaseManager.Instance.playerItemList.Sum(item => item.itemCount);
                TurnData.enemyFinalScore = DatabaseManager.Instance.enemyItemList.Sum(item => item.itemCount);

                StartCoroutine(FadeManager.Instance.FadeIn(fade, 2f));
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                return;
            }
        }

        ChangeTurn(turnOrder[currentTurnIndex]);

    }

    void HighlightRandomTiles() //랜덤 아이템 칸 설정
    {
        itemsPerTurn = Random.Range(2, 5); //3~4개 정도 지정

        HashSet<Vector2Int> selectedTiles = new HashSet<Vector2Int>(); //
        while (selectedTiles.Count < itemsPerTurn)
        {
            Vector2Int randomPosition = new Vector2Int(
                Random.Range(0, mapmanager.width),
                Random.Range(0, mapmanager.height)
            );

            if (!selectedTiles.Contains(randomPosition))
            {
                selectedTiles.Add(randomPosition);

                // 아이템 생성
                int randomItemID = Random.Range(10003, 10006);
                int randomCount = Random.Range(2, 4);
                Item newItem = new Item(randomItemID, "아이템", Item.ItemType.Normal, randomCount);

                // 리스트에 추가
                GameManager.Instance.itemRewardList.Add(newItem);
                GameManager.Instance.rewardTiles.Add(randomPosition);
            }

        }
        EndTurn();
    }
    //IEnumerator ChangeTileColor(MapTile tile, Color targetColor, float duration)
    //{
    //    Renderer tileRenderer = tile.GetRenderer(); // MapTile에서 Renderer를 가져오는 메서드
    //    if (tileRenderer == null) yield break;

    //    Material tileMaterial = tileRenderer.material;
    //    Color initialColor = tileMaterial.color;
    //    float timeElapsed = 0;

    //    while (timeElapsed < duration)
    //    {
    //        timeElapsed += Time.deltaTime;
    //        tileMaterial.color = Color.Lerp(initialColor, targetColor, timeElapsed / duration); // 점진적으로 색 변경
    //        yield return null;
    //    }

    //    tileMaterial.color = targetColor; // 최종 색상 설정
    //}

    void DistributeItems() //아이템 지급 단계
    {
        for (int i = 0; i < GameManager.Instance.rewardTiles.Count; i++)
        {
            Vector2Int tilePos = GameManager.Instance.rewardTiles[i];
            Item rewardItem = GameManager.Instance.itemRewardList[i];

            MapTile tile = mapmanager.GetTile(tilePos);

            if (tile.hasPlayer)
            {
                inventorymanager.AddItem(DatabaseManager.Instance.playerItemList, rewardItem.itemID, rewardItem.itemCount);
                Debug.Log($"플레이어가 {rewardItem.itemCount}개의 {rewardItem.itemID} 아이템을 받았습니다.");
            }
            else if (tile.hasEnemy)
            {
                inventorymanager.AddItem(DatabaseManager.Instance.enemyItemList, rewardItem.itemID, rewardItem.itemCount);
                Debug.Log($"적이 {rewardItem.itemCount}개의 {rewardItem.itemID} 아이템을 받았습니다.");
            }

            //// 타일 색상을 검은색으로 부드럽게 변경
            //if (tile != null)
            //{
            //    StartCoroutine(ChangeTileColor(tile, Color.black, 1f)); // 1초 동안 검은색으로 변경
            //}
        }

        // 리스트 초기화
        GameManager.Instance.itemRewardList.Clear();
        GameManager.Instance.rewardTiles.Clear();

        EndTurn(); // 다음 턴으로 전환
    }

    public void DiceBtn() //주사위 등장 버튼 이벤트
    {
        if (isEnemyTurn) return;

        MoveUI(playerBtnUI, -700f, 0.5f); //버튼 UI 퇴장

        playerDiceUI.SetActive(true); //주사위UI 등장
        playerDiceBtnUI.SetActive(true);//주사위버튼 등장

        diceroll.RollDIce(); //주사위 굴리기
    }

    public void DiceResult() //주사위 멈추기 버튼
    {
        playerDiceBtnUI.SetActive(false);//주사위버튼 비활성화

        StartCoroutine(DisappearDiceUI()); //주사위UI 비활성화

        int diceResult = diceroll.StopRolling();
        playerController.StartTurn(diceResult); //플레이어의 턴 무빙
    }

    void MoveUI(RectTransform rect, float posX, float duration) //UI 등장, 퇴장 (대상, 위치, 시간)
    {
        rect.DOAnchorPosX(posX, duration).SetEase(Ease.InOutQuad);
    }

    IEnumerator DisappearDiceUI()
    {
        yield return new WaitForSeconds(1.5f);
        MoveUI(playerRenderUI, -700f, 0.5f); //캐릭터 UI 퇴장
        playerDiceUI.SetActive(false);
    }
}
