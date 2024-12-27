using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public GameObject spotlight;
    public Text resultText;

    private void Start()
    {
        StartCoroutine(Result());
    }

    IEnumerator Result()
    {
        yield return new WaitForSeconds(3f);

        if (TurnData.enemyFinalScore > TurnData.playerFinalScore) //적 승리 시
        {
            spotlight.SetActive(true);
            spotlight.transform.position = new Vector3(2f, 10f, 0);
            yield return new WaitForSeconds(1f);
            resultText.text = "오징어자식에게 패배하고 말았다...";
        }
        else if (TurnData.playerFinalScore > TurnData.enemyFinalScore)
        {
            spotlight.SetActive(true);
            spotlight.transform.position = new Vector3(-2f, 10f, 0);
            yield return new WaitForSeconds(1f);
            resultText.text = "최고의 타코야끼가 되었다!";
        }
        else
        {
            spotlight.SetActive(true);
            spotlight.transform.position = new Vector3(0f, 10f, 0);
            yield return new WaitForSeconds(1f);
            resultText.text = "동점이다!";
        }
    }
}
