using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string speaker;
    public string message; //대화 내용
}

[System.Serializable]
public class DialogueWrapper
{
    public string enemyName; // 적의 이름
    public Dialogue[] dialogues; // 대사 배열
}
