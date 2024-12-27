using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    public List<GameObject> rulePanels;
    public int currentIndex = 0; //현재 UI 인덱스

    public void BeginRules()
    {
        //룰 시작
        ShowCurrentPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShowPrePanel();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ShowNextPanel();
        }
    }

    void ShowCurrentPanel()
    {
        for(int i=0; i < rulePanels.Count; i++)
        {
            rulePanels[i].SetActive(i == currentIndex);
        }
    }

    void ShowPrePanel()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowCurrentPanel();
        }
    }

    void ShowNextPanel()
    {
        if (currentIndex < rulePanels.Count - 1)
        {
            currentIndex++;
            ShowCurrentPanel();
        }
        else
        {
            ExitRules();
        }
    }

    void ExitRules() //룰설명 종료
    {
        foreach (GameObject panel in rulePanels) //UI 비활성화
        {
            panel.SetActive(false);
        }

        //ReadyGo
        MiniGameManager gamemanager = FindObjectOfType<MiniGameManager>();
        if(gamemanager!= null)
        {
            gamemanager.StartCountdown(); //시작
        }
        //RuleManager 자체 비활성화
        gameObject.SetActive(false);
    }
}
