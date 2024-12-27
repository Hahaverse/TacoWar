using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aeEnemyAI : MonoBehaviour
{
    public float minDelay = 0.05f;
    public float maxDelay = 0.1f;

    bool canAct = true; //적 행동 가능한지 체크
    MiniGameManager minimanager;

    private void Awake()
    {
        minimanager = FindObjectOfType<MiniGameManager>();
    }
    private void Update()
    {
        if (!canAct) return; //행동가능 체크

        Plate plateScript = minimanager.GetEnemyPlate();

        // 접시가 목표에 도달했는지 확인
        if (plateScript != null && plateScript.IsReadyForAction())
        {
            StartCoroutine(MakeDecision(plateScript));
        }
    }

    IEnumerator MakeDecision(Plate plate)
    {
        canAct = false;

        //판단 지연
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        //판단
        bool correctAnswer = plate.objectCount == 10; //정답 체크
        bool isCorrect = Random.Range(0, 100) >= 30;   //30% 확률로 오답

        if (!isCorrect)
        {
            correctAnswer = !correctAnswer; //오답처리
        }

        minimanager.CheckEnemyAnswer(correctAnswer);
        canAct = true;
    }
}
