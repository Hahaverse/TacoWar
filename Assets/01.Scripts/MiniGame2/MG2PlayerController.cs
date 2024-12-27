using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG2PlayerController : MonoBehaviour
{
    MiniGameManager minimanager;
    private void Start()
    {
        minimanager = FindObjectOfType<MiniGameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) //OK입력
        {
            minimanager.CheckPlayerAnswer(true);
        }
        else if (Input.GetKeyDown(KeyCode.Q)) //NO입력
        {
            minimanager.CheckPlayerAnswer(false);
        }
    }
}
