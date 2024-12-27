using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonHelper : MonoBehaviour
{
    public static T[] FromJson<T>(string json)
    {
        // JSON 데이터를 배열로 래핑
        string wrappedJson = $"{{\"items\":{json}}}";
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.items;
    }

    // 배열을 JSON 문자열로 변환하는 함수
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T> { items = array };
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    // 보기 좋은 포맷으로 변환하는 ToJson 함수
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T> { items = array };
        return UnityEngine.JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
