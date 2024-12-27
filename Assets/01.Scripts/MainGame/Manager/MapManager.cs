using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int width = 5;
    public int height = 7;
    public bool isEvent;
    MapTile[,] map;

    EventManager eventmanager;

    //MapTile getter
    public MapTile GetTile(Vector2Int pos)
    {
        if (!CheckBounds(pos))
        {
            Debug.LogError("존재하지 않는 영역입니다.");
            return null;
        }
        return map[pos.x, pos.y];
    }

    //MapTile의 setter
    public void SetHasPlayerOnTile(Vector2Int pos, bool hasplayer)
    {
        if (CheckBounds(pos)) map[pos.x, pos.y].hasPlayer = hasplayer;
    }

    public void SetHasEnemyOnTile(Vector2Int pos, Enemy enemy)
    {
        if (CheckBounds(pos))
        {
            MapTile targetTile = GetTile(pos);
            targetTile.SetEnemy(enemy);
        }
    }

    //MapTile의 getter
    public bool GetHasPlayerOnTile(Vector2Int pos)
    {
        if (CheckBounds(pos)) return map[pos.x, pos.y].hasPlayer;
        else return false;
    }
    public bool GetHasEnemyOnTile(Vector2Int pos)
    {
        if (CheckBounds(pos))
        {
            MapTile targetTile = GetTile(pos);
            return targetTile.hasEnemy;
        }
        return false;
    }

    private void Awake()
    {
        //맵 초기화
        map = new MapTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new MapTile();
            }
        }
    }
    void Start()
    {
        eventmanager = FindObjectOfType<EventManager>();
    }

    public bool CheckBounds(Vector2Int position) //맵 경계 확인
    {
        return position.x > -1 && position.x < width && position.y > -1 && position.y < height;
    }

    public bool TryMoveToTile(Vector2Int targetPos, Charactor charactor) //이동 처리 함수
    {
        MapTile targetTile = GetTile(targetPos);

        if (targetTile.hasPlayer || targetTile.hasEnemy) //타일에 캐릭터가 존재하는지 체크
        {
            eventmanager.HandleEncounter(charactor, targetTile); //만남 처리
            isEvent = true;
            return false;
        }

        //이동 가능하면 타일 정보 갱신
        UpdateTileState(charactor.currentPosition, targetPos, charactor);
        charactor.currentPosition = targetPos; //캐릭터의 현재 위치 갱신

        return true;
    }

    public void UpdateTileState(Vector2Int oldPos, Vector2Int newPos, Charactor charactor) //타일 상태 갱신
    {
        MapTile oldTile = GetTile(oldPos);
        MapTile newTile = GetTile(newPos);

        if (charactor is Enemy enemy)
        {
            oldTile.RemoveEnemy();
            newTile.SetEnemy(enemy);
        }
        else if (charactor is PlayerController)
        {
            oldTile.hasPlayer = false;
            newTile.hasPlayer = true;
        }
    }
}
