using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Charactor
{
    protected PlayerController playercontroller;

    Vector2Int lastDirection = Vector2Int.zero;

    protected override void Start()
    {
        base.Start();
        playercontroller = FindObjectOfType<PlayerController>();
        InitializePosition();
    }

    protected void InitializePosition() //적의 랜덤한 초기 위치 설정
    {
        if (mapmanager == null) return;
        do
        {
            currentPosition = new Vector2Int(
                Random.Range(0, mapmanager.width),
                Random.Range(0, mapmanager.height)
            );
        }
        while (mapmanager.GetHasPlayerOnTile(currentPosition) == true);
        Debug.Log("적의 초기 위치: " + currentPosition);

        //월드 좌표 싱크
        transform.position = new Vector3(
            (currentPosition.x - (mapmanager.width - 1)) * distance, 0,
            ((mapmanager.height - 1) - currentPosition.y) * distance);

        //타일에 적 등록
        mapmanager.SetHasEnemyOnTile(currentPosition, this);
    }

    public IEnumerator TakeTurn() //적의 턴 패턴
    {
        Debug.Log("적의 턴 시작");

        //주사위 결과 (임시)
        int roll = Random.Range(1, 7);

        //정해진 턴 동안 움직임
        for (int i = 0; i < roll; i++)
        {
            //목적지 정하기
            Vector2Int targetPosition = playercontroller.currentPosition;
            Debug.Log("적의 최종 목적지 : " + targetPosition);

            //타겟 계산
            Vector2Int direction = GetMoveDirection(targetPosition); //상하좌우 결정
            Vector2Int nextPosition = currentPosition + direction; //다음 좌표 인덱스 결정

            //타겟 장소에 다른 캐릭터 있는지 체크
            if (mapmanager.TryMoveToTile(nextPosition, this))
            {
                UpdateTileState(currentPosition, nextPosition);
                yield return Moveto(direction);

            }
        }

        Debug.Log("에너미 턴 종료");
    }

    Vector2Int GetMoveDirection(Vector2Int targetPos) //방향 결정 함수
    {
        bool rand = Random.Range(0, 2) == 0;

        if (rand)
        {
            //x축 먼저
            if (targetPos.x < currentPosition.x) return Vector2Int.left; //왼쪽
            else if (targetPos.x > currentPosition.x) return Vector2Int.right; //오른쪽
            else if (targetPos.y > currentPosition.y) return Vector2Int.up; //아래
            else if (targetPos.y < currentPosition.y) return Vector2Int.down; //위
        }
        else
        {
            //y축 먼저
            if (targetPos.y > currentPosition.y) return Vector2Int.up; //아래
            else if (targetPos.y < currentPosition.y) return Vector2Int.down; //위
            else if (targetPos.x < currentPosition.x) return Vector2Int.left; //왼쪽
            else if (targetPos.x > currentPosition.x) return Vector2Int.right; //오른쪽
        }
        return Vector2Int.zero;
    }

    void UpdateTileState(Vector2Int oldPos, Vector2Int newPos) //타일 정보 갱신
    {
        mapmanager.SetHasEnemyOnTile(oldPos, null);
        mapmanager.SetHasEnemyOnTile(newPos, this);
        currentPosition = newPos;
    }

    IEnumerator Moveto(Vector2Int dir) //칸 이동 함수
    {
        Vector3 rot; //이동 각도
        Vector3 dis; //이동거리

        if (dir == Vector2Int.up)
        {
            rot = rotS;
            dis = disS;
        }
        else if (dir == Vector2Int.down)
        {
            rot = rotW;
            dis = disW;
        }
        else if (dir == Vector2Int.right)
        {
            rot = rotD;
            dis = disD;
        }
        else if (dir == Vector2Int.left)
        {
            rot = rotA;
            dis = disA;
        }
        else
        {
            rot = Vector3.zero;
            dis = Vector3.zero;
        }

        //직전과 이동방향이 다르면 몸체 회전
        if (dir != lastDirection)
        {
            Vector3 rotY = GetRotation(dir);
            transform.DORotate(rotY, duration, RotateMode.Fast)
                        .SetEase(Ease.OutCubic)
                        .WaitForCompletion();
            yield return new WaitForSeconds(0.5f);
        }

        UpdatePosition(transform.position + dis, rot);
        lastDirection = dir;

        //이동 대기
        yield return new WaitForSeconds(duration);
    }

    Vector3 GetRotation(Vector2Int dir) //회전 최소각 구하기
    {
        float targetRotationY = 0;

        if (dir == Vector2Int.up) targetRotationY = 180f;
        else if (dir == Vector2Int.down) targetRotationY = 0f;
        else if (dir == Vector2Int.left) targetRotationY = -90f;
        else if (dir == Vector2Int.right) targetRotationY = 90f;

        return new Vector3(transform.eulerAngles.x, targetRotationY, transform.eulerAngles.z);
    }

    public virtual string OnEncounter() //이벤트
    {
        return null; //반환값 = 다음 씬
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
