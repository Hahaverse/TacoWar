using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DiceRoll : MonoBehaviour
{
    float speed = 0.1f; //초 당 회전 속도
    Vector3[] faceRotation = new Vector3[6];  //각 면에 대한 회전값
    Tween rotationTween;

    private void Start()
    {
        faceRotation[0] = new Vector3(0, 180f, 0); //1
        faceRotation[1] = new Vector3(-90f, 0, 0);//2
        faceRotation[2] = new Vector3(0, -90f, 0);//3
        faceRotation[3] = new Vector3(0, 90f, 0);//4
        faceRotation[4] = new Vector3(90f, 0, 0);//5
        faceRotation[5] = new Vector3(0, 0, 0); //6
    }

    //주사위 굴리기
    public void RollDIce()
    {
        if (rotationTween != null && rotationTween.IsActive()) return; //회전 중이면 무시

        transform.rotation = Quaternion.Euler(Vector3.zero);

        // 랜덤 회전 축 생성
        Vector3 randomAxis = new Vector3(
            Random.Range(-1f, 1f), // X축
            Random.Range(-1f, 1f), // Y축
            Random.Range(-1f, 1f)  // Z축
        ).normalized;

        //회전 시작
        rotationTween = transform.DORotate(randomAxis * 360f, speed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental) // 무한 반복
            .SetEase(Ease.Linear); // 일정한 속도
    }

    //주사위 결과 발표
    public int StopRolling()
    {
        int result = Random.Range(1, 7); //1~6 중 랜덤한 숫자 뽑기
        Debug.Log(result);
        if (rotationTween != null && rotationTween.IsActive())
        {
            rotationTween.Kill(); // 회전 애니메이션 종료
            rotationTween = null; // Tween 객체 초기화
            transform.DORotate(faceRotation[result - 1], 0.5f).SetEase(Ease.OutBounce); //알맞게 주사위 회전
        }

        //결과 반환
        return result;
    }
}
