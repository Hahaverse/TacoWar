using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EventManager : MonoBehaviour
{
    DialogueManager dialoguemanager;

    private void Start()
    {
        dialoguemanager = FindObjectOfType<DialogueManager>();
    }

    public void HandleEncounter(Charactor charactor, MapTile targetTile) //캐릭터 만남 이벤트 총괄
    {
        if (targetTile.hasPlayer && charactor is Enemy)
        {
            //적이 플레이어가 있는 칸으로 갔을 때 이벤트
            //EncounterToEnemy("EnemyToPlayer2");
            Debug.Log("적이 플레이어에게 갑니다.");
        }
        else if (targetTile.hasEnemy && charactor is PlayerController)
        {
            Enemy enemy = targetTile.GetEnemy(); //적 객체 가져오기
            EncounterToPlayer(enemy.OnEncounter());

            Debug.Log(enemy.OnEncounter());
            Debug.Log("플레이어가 적에게 갑니다.");
        }
        else
        {
            //다른 상황 (예를 들어 에너미-에너미)
        }
    }

    void EncounterToPlayer(string DialoguePath) //적이 플레이어와 만난 이벤트
    {
        //다이얼로그 UI 등장

        dialoguemanager.ShowDialogue("타코야끼", DialoguePath);

        //
    }

    void EncounterToEnemy(string DialoguePath) //적이 플레이어에게 간 이벤트
    {
        dialoguemanager.ShowDialogue("타코야끼", DialoguePath);
    }
}