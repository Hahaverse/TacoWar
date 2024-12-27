using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; //플레이어 transform
    public float fixedY = 1.9f; //Y고정

    private void LateUpdate()
    {
        if (player != null)
        {
            //플레이어 추적
            transform.position = new Vector3(player.position.x, player.position.y + 1.9f, player.position.z - 2.1f);
        }
    }
}
