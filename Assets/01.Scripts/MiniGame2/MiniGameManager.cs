using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    //스코어 텍스트
    public Text playerScoreText;
    public Text enemyScoreText;

    //타이머 텍스트
    public Text timerText;

    //제한 시간
    public float gameDuration = 30f;
    float timeLeft;

    //그릇
    public GameObject platePrefab;
    public float plateSpeed = 3f;
    float playerPlateSpeed;
    float enemyPlateSpeed;

    //플레이어 그릇 스폰
    public Transform PlayerSpawnPoint;
    public Transform PlayerTargetPoint;

    //적 그릇 스폰
    public Transform EnemySpawnPoint;
    public Transform EnemyTargetPoint;

    //카운트 IMAGE
    public CanvasGroup readyImage;
    public CanvasGroup goImage;
    public CanvasGroup finishImage;

    //종합 UI
    public GameObject UICanvas;

    //페이드
    public CanvasGroup fade;

    //스포트라이트
    public GameObject spotlight;

    //스코어
    int playerScore = 0;
    int enemyScore = 0;

    //현재 접시 정보
    GameObject currentPlayerPlate;
    GameObject currentEnemyPlate;

    bool gameRunning = false; //게임 진행 상황
    bool canPlayerSpawnPlate = true; //플레이어 접시 생성 가능 여부
    bool canEnemySpawnPlate = true; //적 접시 생성 가능 여부

    void Start()
    {
        timerText.text = "";
        //초기 접시 속도 설정
        playerPlateSpeed = plateSpeed;
        enemyPlateSpeed = plateSpeed;

        StartCoroutine(StartGamePre()); //게임 시작 준비
    }

    void Update()
    {
        if (gameRunning) //게임 중이면
        {
            //타이머 업데이트
            timeLeft -= Time.deltaTime;
            timerText.text = $"{Mathf.Max(0, timeLeft):F1}";

            if (timeLeft <= 0)
            {
                EndGame(); //게임 종료
            }

        }
        //스코어 표시
        playerScoreText.text = $"{ playerScore}";
        enemyScoreText.text = $"{enemyScore}";
    }

    public void StartGame() //게임 시작
    {
        //게임 시작 설정
        gameRunning=true;
        timeLeft = gameDuration;

        //첫 접시 생성
        SpawnPlayerPlate();
        SpawnEnemyPlate();

    }

    void EndGame() //게임 종료
    {
        StartCoroutine(EndGameCoroutine());
    }

    public void StartCountdown() //스타트 카운트다운
    {
        UICanvas.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator StartGamePre() //게임 시작 전
    {
        //흰 화면에서 페이드 인
        yield return StartCoroutine(FadeManager.Instance.FadeIn(fade, 1f));

        //설명 로직 실행
        FindObjectOfType<RuleManager>().BeginRules();
    }

    IEnumerator CountdownCoroutine() //카운트다운
    {
        //레디
        readyImage.gameObject.SetActive(true);
        yield return StartCoroutine(FadeManager.Instance.FadeOut(readyImage, 0.5f));
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(FadeManager.Instance.FadeIn(readyImage, 0.5f));
        yield return new WaitForSeconds(0.2f);

        //고
        goImage.gameObject.SetActive(true);
        yield return StartCoroutine(FadeManager.Instance.FadeOut(goImage, 0.5f));
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(FadeManager.Instance.FadeIn(goImage, 0.5f));
        yield return new WaitForSeconds(0.2f);

        StartGame();
    }

    IEnumerator EndGameCoroutine() //게임 종료 로직
    {
        gameRunning = false;
        //FINISH 활성화
        finishImage.gameObject.SetActive(true);
        //FINISH 페이드아웃과 동시에 비활성화
        yield return StartCoroutine(FadeManager.Instance.FadeOut(finishImage, 1f));
        finishImage.gameObject.SetActive(false);

        //UI비활성화
        UICanvas.SetActive(false);
        RenderSettings.ambientLight = Color.black;

        //조명 효과
        yield return new WaitForSeconds(2f);
        Vector3 spotlightPosition = WinnerPos();
        SetWinnerSpotlight(spotlightPosition);

        //종료
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeManager.Instance.FadeIn(fade, 1f));

        //결과 처리
        PassWinerInfo();
    }

    Vector3 WinnerPos() //승자 위치 결정
    {
        if (playerScore > enemyScore) return new Vector3(-2f, 10f, 4f);
        else if (playerScore < enemyScore) return new Vector3(2f, 10f, 4f);
        else return new Vector3(0, 10f, 4f); //동점
    }

    void SetWinnerSpotlight(Vector3 pos) //스포트라이트 설정
    {
        spotlight.SetActive(true);
        spotlight.transform.position = pos;
    }

    void PassWinerInfo() //우승자 정보 저장
    {
        TurnData.playerFinalScore += playerScore;
        TurnData.enemyFinalScore += enemyScore;

        if (playerScore > enemyScore) TurnData.playerFinalScore += 5;
        else if (enemyScore > playerScore) TurnData.enemyFinalScore += 5;

        SceneManager.LoadScene(2); //최종 씬 이동
    }

    public Plate GetEnemyPlate() //적 접시 반환
    {
        if (currentEnemyPlate == null) return null;
        return currentEnemyPlate.GetComponent<Plate>();
    }

    public void CheckPlayerAnswer(bool isAccept) //플레이어 정답 처리
    {
        if (!gameRunning || currentPlayerPlate == null) return; //입력 불가 상황

        Plate plateScript = currentPlayerPlate.GetComponent<Plate>();
        if (plateScript == !plateScript.IsReadyForAction()) return; // 접시가 준비되지 않은 경우 무시

        //정답 체크
        bool correctAnswer = (plateScript.objectCount == 8); //8개일 경우 승낙이 정답

        if (isAccept) //승낙
        {
            plateScript.Accept();
        }
        else //거절
        {
            plateScript.Decline(true);
        }

        // 오답 처리
        if ((isAccept && !correctAnswer) || (!isAccept && correctAnswer)) // 선택이 오답인 경우
        {
            StartCoroutine(PlayerPenalty()); // 패널티 실행
        }
        else
        {
            playerScore++; // 정답 시 스코어 증가
            playerPlateSpeed += 0.5f; // 속도 증가
        }

        currentPlayerPlate = null;
        StartCoroutine(nextPlate(PlayerSpawnPoint, PlayerTargetPoint, true));

    }

    public void CheckEnemyAnswer(bool isAccept) //적 정답 처리
    {
        if (!gameRunning || currentEnemyPlate == null) return;

        Plate plateScript = currentEnemyPlate.GetComponent<Plate>();
        if (plateScript==null || !plateScript.IsReadyForAction()) return; // 접시가 준비되지 않으면 무시

        // 접시 위 오브젝트 수에 따른 정답 기준
        bool correctAnswer = (plateScript.objectCount == 10);

        if (isAccept)
        {
            plateScript.Accept(); // 승낙 애니메이션
        }
        else
        {
            plateScript.Decline(false); // 거절 애니메이션 실행
        }

        if((isAccept&&!correctAnswer) || (!isAccept && correctAnswer)) //오답 실행
        {
            StartCoroutine(EnemyPenalty());
        }
        else //정답 실행
        {
            //정답 처리
            enemyScore++;
            enemyPlateSpeed += 0.5f;//속도 조절
        }
        currentEnemyPlate = null;
        StartCoroutine(nextPlate(EnemySpawnPoint, EnemyTargetPoint, false));
    }
    IEnumerator PlayerPenalty() //플레이어 패널티
    {
        canPlayerSpawnPlate = false;
        yield return new WaitForSeconds(1f);
        canPlayerSpawnPlate = true;
    }
    IEnumerator EnemyPenalty() //에너미 패널티
    {
        canEnemySpawnPlate = false;
        yield return new WaitForSeconds(1f);
        canEnemySpawnPlate = true;
    }

    IEnumerator nextPlate(Transform spawnPoint, Transform targetPoint, bool isPlayer) //다음 접시 꺼내기
    {
        // 1초 딜레이 후 새 접시 생성
        yield return new WaitForSeconds(1f);

        if (isPlayer && canPlayerSpawnPlate)
        {
            currentPlayerPlate = Instantiate(platePrefab, spawnPoint.position, Quaternion.identity);
            currentPlayerPlate.GetComponent<Plate>().Initialize(targetPoint.position, playerPlateSpeed);
        }
        else if (!isPlayer && canEnemySpawnPlate)
        {
            currentEnemyPlate = Instantiate(platePrefab, spawnPoint.position, Quaternion.identity);
            currentEnemyPlate.GetComponent<Plate>().Initialize(targetPoint.position, enemyPlateSpeed);
        }
    }

    void SpawnPlayerPlate() //플레이어 접시 소환
    {
        if (!canPlayerSpawnPlate) return;

        currentPlayerPlate = Instantiate(platePrefab, PlayerSpawnPoint.position, Quaternion.identity);
        currentPlayerPlate.GetComponent<Plate>().Initialize(PlayerTargetPoint.position, plateSpeed);
    }

    void SpawnEnemyPlate() //적 접시 소환
    {
        if (!canEnemySpawnPlate) return;

        currentEnemyPlate = Instantiate(platePrefab, EnemySpawnPoint.position, Quaternion.identity);
        currentEnemyPlate.GetComponent<Plate>().Initialize(EnemyTargetPoint.position, plateSpeed);
    }
}
