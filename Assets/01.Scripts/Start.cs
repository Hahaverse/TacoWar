using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour
{
    //게임 시작버튼 함수
    public void StartGame()
    {
        SceneManager.LoadScene(0);
        Debug.Log("호출됨?");
    }
}
