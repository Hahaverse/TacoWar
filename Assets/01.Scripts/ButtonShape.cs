using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//버튼 모양대로 다듬기
public class ButtonShape : MonoBehaviour
{
    public float AlphaThreshold = 0.1f;

    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
    }
}
