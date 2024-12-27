using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    //대화창 UI
    public GameObject DialogueUI;
    public Text nameText;
    public Text dialogueText;
    public string dialogueFilePath;

    //선택지버튼 UI
    public GameObject choiceUI;
    public Button acceptBtn;
    public Button declineBtn;

    //페이드
    public CanvasGroup fade;

    //주사위
    public GameObject playerDice;
    public GameObject enemyDice;

    string enemyName; //적 이름
    Dialogue[] dialogues; //대사 정보
    int currentIndex = 0; //대화 진행 상태
    bool isFight = true; //싸우기로 결정했는지 의사 체크
    string nextScene; //넘어갈 씬 이름

    void ResetState() //상태 초기화
    {
        currentIndex = 0;
        isFight = false;
        nextScene = null;
        DialogueUI.SetActive(false);
        choiceUI.SetActive(false);
    }
    public void ShowDialogue(string playerName, string path) //대화 시작
    {
        ResetState();
        LoadDialogue(path);
        StartCoroutine(ShowDialogueCoroutine(playerName));
    }

    void LoadDialogue(string dialogueFile) //대화 파일 불러오기
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(dialogueFile); //파일 위치 결정

        if (jsonFile != null)
        {
            string json = jsonFile.text;
            DialogueWrapper wrapper = JsonUtility.FromJson<DialogueWrapper>(json);
            enemyName = wrapper.enemyName; // 적 이름 저장
            dialogues = wrapper.dialogues; // 대사 배열 저장
            
            Debug.Log("파일 불러오기 성공");
        }
        else Debug.LogError("대화 파일이 존재하지 않습니다.");
    }

    IEnumerator ShowDialogueCoroutine(string playerName) //대화 로직
    {
        GameManager.Instance.isEventActive = true; //이벤트 시작
        DialogueUI.SetActive(true);
        isFight = true;

        while (currentIndex < dialogues.Length)
        {
            Dialogue currentDialogue = dialogues[currentIndex]; //대사 불러오기

            if (currentDialogue.speaker == "ENEMY") //적의 대사로 설정
            {
                nameText.text = enemyName;
                dialogueText.text = currentDialogue.message; //대사 출력
            }
            else if (currentDialogue.speaker == "PLAYER") //플레이어의 대사로 설정
            {
                nameText.text = playerName;
                dialogueText.text = currentDialogue.message; //대사 출력
            }
            else if (currentDialogue.speaker == "CHOICE") //선택지 시작
            {
                yield return StartCoroutine(HandleChoice(currentDialogue)); //선택지 표시
                continue;
            }

            // 키 입력 처리를 한 번만 하도록 보장
            bool hasProcessedInput = false;
            while (!hasProcessedInput)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hasProcessedInput = true; // 입력 처리 완료
                    currentIndex++; // 다음 대사로 이동
                }
                yield return null; // 다음 프레임까지 대기
            }
        }
        if (isFight) //플레이어가 싸우기로 했다면
        {
            StartCoroutine(FightCoroutine());
        }

        DialogueUI.SetActive(false);
        GameManager.Instance.isEventActive = false; //이벤트 종료
    }

    IEnumerator FightCoroutine()
    {
        //주사위 활성화
        playerDice.SetActive(true);
        enemyDice.SetActive(true);

        DiceRoll playerDiceRoll = playerDice.GetComponent<DiceRoll>();
        DiceRoll enemyDiceRoll = enemyDice.GetComponent<DiceRoll>();

        //주사위 굴리기
        playerDiceRoll.RollDIce();
        enemyDiceRoll.RollDIce();

        yield return new WaitForSeconds(2.5f); // 2.5초 대기

        // 주사위 멈추기 및 결과 가져오기
        int playerResult = playerDiceRoll.StopRolling();
        int enemyResult = enemyDiceRoll.StopRolling();

        DialogueUI.SetActive(true);
        dialogueText.text = $"플레이어: {playerResult} vs 적: {enemyResult}";

        playerDice.SetActive(false);
        enemyDice.SetActive(false);

        yield return new WaitForSeconds(1f); // 1초 대기
        DialogueUI.SetActive(false);
        dialogueText.text = "";

        if (playerResult > enemyResult)
        {
            Enemy enemy = FindObjectOfType<Enemy>();
            enemy.RandomUpdatePosition();
        }
        else if (playerResult < enemyResult)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.RandomUpdatePosition();
        }
        else
        {
            Debug.Log("아무 일도 일어나지 않습니다.");
        }
    }

    IEnumerator HandleChoice(Dialogue choiceDialogue)
    {
        choiceUI.SetActive(true); //선택지 UI 표시

        //버튼 리스너 초기화 (추후 분리)
        acceptBtn.onClick.RemoveAllListeners();
        declineBtn.onClick.RemoveAllListeners();

        //결정 플래그
        bool isAccepted = false;
        bool isDeclined = false;

        acceptBtn.onClick.AddListener(() => //승낙 버튼 리스너
        {
            isAccepted = true;
        });

        declineBtn.onClick.AddListener(() => //거절 버튼 리스너
        {
            isDeclined = true;
        });

        yield return new WaitUntil(() => isAccepted || isDeclined); //선택 완료까지 대기

        //선택 마무리
        choiceUI.SetActive(false);

        //선택 결과 처리
        if (isAccepted)
        {
            nextScene = choiceDialogue.message;
            currentIndex++;
        }
        else if (isDeclined)
        {
            isFight = false;
            dialogueText.text = "(그만두자.)";
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            currentIndex = dialogues.Length;
        }
    }
}
