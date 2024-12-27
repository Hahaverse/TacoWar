using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Charactor : MonoBehaviour
{
    public Vector2Int currentPosition; //현재 위치 (배열 인덱스)

    //이동 관련 설정
    protected float distance = 1.04f; //블럭 간 거리
    protected float height = 0.5f; //점프 높이
    protected float duration = 0.8f; //이동 시간
    protected bool isMoving = false; //이동 상태

    //이동 거리 벡터
    protected Vector3 disW, disA, disS, disD;

    //회전 방향 벡터
    protected Vector3 rotW, rotA, rotS, rotD;

    protected TurnManager turnmanager;
    protected MapManager mapmanager;
    protected EventManager eventmanager;

    protected virtual void Start()
    {
        turnmanager = FindObjectOfType<TurnManager>();
        mapmanager = FindObjectOfType<MapManager>();
        eventmanager = FindObjectOfType<EventManager>();
        InitializeMoveToVectors();
    }

    void InitializeMoveToVectors() //이동 거리, 회전 방향 설정
    {
        disW = new Vector3(0, 0, distance);
        disA = new Vector3(-distance, 0, 0);
        disS = new Vector3(0, 0, -distance);
        disD = new Vector3(distance, 0, 0);

        rotW = new Vector3(360, 0, 0);
        rotA = new Vector3(0, 0, 360);
        rotS = new Vector3(-360, 0, 0);
        rotD = new Vector3(0, 0, -360);
    }

    protected void UpdatePosition(Vector3 pos, Vector3 rot) //위치 이동 함수
    {
        Debug.Log(currentPosition);

        isMoving = true;

        //점프 + 구르기 모션
        Sequence moveSequence = DOTween.Sequence();

        //DoTweening으로 다음 칸으로 점프  (목표 위치, 높이, 점프 횟수, 시간)
        moveSequence.Append(transform.DOLocalJump(pos, height, 1, duration));

        //DoRotate로 구르기
        moveSequence.Join(transform.DORotate(rot, duration, RotateMode.WorldAxisAdd));

        //이동상태 해제
        moveSequence.OnComplete(() =>
        {
            isMoving = false;
        });
    }
}
