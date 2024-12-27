using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : Charactor
{
    protected override void Start()
    {
        base.Start();

        //초기 위치 설정
        currentPosition = new Vector2Int(4, 6);
        mapmanager.SetHasPlayerOnTile(currentPosition, true); // 플레이어 위치 맵에 설정
    }

    //플레이어 턴에 움직이기
    public void StartTurn(int count)
    {
        TurnData.movingCount = count; //초기화
        StartCoroutine(PlayerInput()); //플레이어 이동 허용
    }

    IEnumerator PlayerInput()
    {
        while (TurnData.movingCount > 0)
        {
            //이벤트 중 입력 차단
            if (GameManager.Instance.isEventActive)
            {
                yield return null;
                continue;
            }

            //중복 입력 방지
            if (!isMoving) 
            {
                //플레이어 이동
                if (Input.GetKeyDown(KeyCode.W))
                {
                    MoveTo(currentPosition + Vector2Int.down, disW, rotW);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    MoveTo(currentPosition + Vector2Int.left, disA, rotA);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    MoveTo(currentPosition + Vector2Int.up, disS, rotS);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    MoveTo(currentPosition + Vector2Int.right, disD, rotD);
                }
            }
            yield return null;
        }
        Debug.Log("플레이어 턴이 끝나야 합니다.");
        turnmanager.EndTurn();
    }

    void MoveTo(Vector2Int newPosition, Vector3 dir, Vector3 rot) //칸 이동 함수
    {
        //맵 경계 확인
        if (!mapmanager.CheckBounds(newPosition)) return;

        //적 확인
        if(mapmanager.TryMoveToTile(newPosition, this))
        {
            currentPosition = newPosition;

            //실제 좌표 이동
            Vector3 nextPos = transform.position + dir;
            UpdatePosition(nextPos, rot);

            //남은 턴 수정
            TurnData.movingCount--;
            Debug.Log("남은 이동 횟수: " + TurnData.movingCount);
        }
        else
        {
            return;
        }
    }

    void UpdateTileState(Vector2Int oldPos, Vector2Int newPos) //타일 정보 갱신
    {
        mapmanager.SetHasPlayerOnTile(oldPos, false);
        mapmanager.SetHasPlayerOnTile(newPos, true);
        currentPosition = newPos;
    }

    public void RandomUpdatePosition() //랜덤 위치 지정
    {
        Vector2Int newPosition;
        if (mapmanager == null) return;
        do
        {
            newPosition = new Vector2Int(
                Random.Range(0, mapmanager.width),
                Random.Range(0, mapmanager.height)
            );
        }
        while (mapmanager.GetHasPlayerOnTile(newPosition) == true && mapmanager.GetHasEnemyOnTile(newPosition) == true);

        UpdateTileState(currentPosition, newPosition);

        //위치 지정
        Vector3 position = new Vector3(newPosition.x - (mapmanager.width - 1) * distance, 0,
            ((mapmanager.height - 1) - newPosition.y) * distance);

        currentPosition = newPosition;

        isMoving = true;
        //DoTweening으로 다음 칸으로 점프  (목표 위치, 높이, 점프 횟수, 시간)
        transform.DOLocalJump(position, height*5, 1, duration);
        isMoving = false;
    }
}
