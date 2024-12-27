using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    #region ½Ì±ÛÅæ
    public static FadeManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public IEnumerator FadeOut(CanvasGroup fadeCanvas, float duration) //ÆäÀÌµå ¾Æ¿ô (0->1)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        fadeCanvas.alpha = 1f;
    }

    public IEnumerator FadeIn(CanvasGroup fadeCanvas, float duration) //ÆäÀÌµå ÀÎ (1->0)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }
        fadeCanvas.alpha = 0f;
    }
}
