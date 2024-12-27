using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//턴 정보
public class TurnData
{
    public static int gameTurn = 5; //남은 게임 턴
    public static int movingCount = 0; //남은 플레이어 이동 횟수

    public static int playerFinalScore = 0; //플레이어 최종점수
    public static int enemyFinalScore = 0; //적 최종점수

    public enum Turn { ItemSetup, Player, Enemy, ItemDistribution }; //턴 목록
    public static Turn currentTurn; //현재 턴
}
