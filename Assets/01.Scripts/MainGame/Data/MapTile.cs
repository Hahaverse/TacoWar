using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapTile
{
    public bool hasPlayer { get; set; } //칸에 플레이어가 있는지 여부
    public Enemy enemy { get; private set; } //칸에 있는 적 객체
    public bool hasEnemy => enemy != null; //적의 존재 여부 자체 체크

    public Action tileEvent; //칸 이벤트

    public MapTile()
    {
        hasPlayer = false;
        enemy = null;

        tileEvent = null;
    }

    Renderer renderer;
    public Renderer GetRenderer() //렌더러
    {
        if (renderer == null)
        {
            renderer = GameObject.Find("TileGameObject").GetComponent<Renderer>();
        }
        return renderer;
    }

    public void SetEnemy(Enemy newEnemy) //적 배치
    {
        enemy = newEnemy;
    }

    public Enemy GetEnemy() { //적 반환
        return enemy;

    }

    public void RemoveEnemy() //적 제거
    {
        enemy = null;
    }
}
