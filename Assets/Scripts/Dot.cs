using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Transform trans;

    float groundY = -3f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trans = GetComponent<Transform>();
    }

    void Update()
    {
        //if the dot is underground, make it invisible
        spriteRenderer.enabled = trans.position.y > groundY;
    }
}
