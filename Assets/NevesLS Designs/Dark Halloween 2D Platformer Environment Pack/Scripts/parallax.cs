using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    private float startPos;
    private GameObject cam;
    [SerializeField] private float parallaxEffect;


    void Start()
    {
        cam = GameObject.Find("Main Camera");
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.x);
        

    }
}
