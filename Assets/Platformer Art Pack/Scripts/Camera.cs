using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    internal static Camera main;
    public GameObject player;
    public float offset;

    internal Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        transform.position = new Vector2 (player.transform.position.x, player.transform.position.y + offset);
    }
}
