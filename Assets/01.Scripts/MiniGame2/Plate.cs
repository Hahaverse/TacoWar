using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plate : MonoBehaviour
{
    public GameObject objectPrefab; //접시 위에 올라갈 오브젝트
    public int objectCount; //접시 위 오브젝트 갯수

    Vector3 targetPosition; //이동 목적지
    float moveSpeed; //이동속도
    bool isreached = false; //목적지 도달 여부

    Vector3[] positions = new Vector3[]
    {
        new Vector3(-0.268f, -0.305f, -0.075f),
        new Vector3(0.15f,-0.298f,-0.229f),
        new Vector3(0.317f,-0.297f,0.223f),
        new Vector3(-0.138f,-0.305f,0.354f),
        new Vector3(0.362f,-0.291f,-0.086f),
        new Vector3(0.092f,-0.294f,0.385f),
        new Vector3(-0.273f,-0.283f,0.211f),
        new Vector3(0.001f,-0.298f,-0.365f),
        new Vector3(0.237f,-0.305f,0.045f),
        new Vector3(-0.020f,-0.288f,-0.014f)
    };

    public void Initialize(Vector3 target, float speed)
    {
        targetPosition = target;
        moveSpeed = speed;

        //8개 또는 10개의 오브젝트 생성
        objectCount = Random.Range(0, 2) == 0 ? 8 : 10;
        SpawnObjects(objectCount);

        //이동 제어
        MoveToTarget();
    }

    void MoveToTarget() //목적지까지 이동
    {
        transform.DOMove(targetPosition, 1f / moveSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            isreached = true; //목적지에 도달하고 판단 확인
        });
    }

    void SpawnObjects(int count) //그릇에 아이템 생성
    {
        List<int> usedlist = new List<int>(); //생성위치 확인하는 리스트
        for(int i = 0; i < count; i++)
        {
            int randomIndex;

            //사용되지 않은 좌표를 찾기 위한 루프
            do
            {
                randomIndex = Random.Range(0, positions.Length);
            }
            while (usedlist.Contains(randomIndex));

            usedlist.Add(randomIndex);

            // 오브젝트 생성
            Vector3 spawnPosition = positions[randomIndex];
            GameObject obj = Instantiate(objectPrefab, transform.position + spawnPosition, Quaternion.identity, transform);

            // Y축 랜덤 회전
            obj.transform.Rotate(0, Random.Range(0f, 360f), 0);
        }
    }

    public bool IsReadyForAction() //접시 목적지 도달 여부 반환
    {
        return isreached;
    }

    public void Accept() //승낙 애니메이션
    {
        float animationTime = 0.5f / moveSpeed; // 이동 속도의 절반
        StartCoroutine(PlayAcceptAnimation(animationTime));
    }

    public void Decline(bool isPlayer) //거절 애니메이션
    {
        float animationTime = 0.5f / moveSpeed; // 이동 속도의 절반
        StartCoroutine(PlayDeclineAnimation(isPlayer, animationTime));
    }

    IEnumerator PlayAcceptAnimation(float duration) // OK움직임
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveY(transform.position.y + 1, duration / 2))
            .Join(transform.DORotate(new Vector3(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), duration / 2))
            .Append(transform.DOMoveY(transform.position.y - 1, duration / 2))
            .Join(transform.DORotate(new Vector3(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), duration / 2));

        yield return sequence.WaitForCompletion();

        //파괴
        Destroy(gameObject);
        DOTween.Kill(transform);
    }

    IEnumerator PlayDeclineAnimation(bool isPlayer, float duration) //NO움직임
    {
        //NO 애니메이션 처리
        if (isPlayer)
        {
            //플레이어의 경우 X축 -4로 이동
            transform.DOMoveX(transform.position.x - 4, duration);
        }
        else
        {
            //적의 경우 X축 +4로 이동
            transform.DOMoveX(transform.position.x + 4, duration);
        }

        yield return new WaitForSeconds(duration);

        //파괴
        Destroy(gameObject);
        DOTween.Kill(transform);
    }

    IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); //접시 파괴
    }
}
