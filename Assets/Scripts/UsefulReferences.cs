using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulReferences
{
    /// <summary>
    /// Returns the dimensions of the camera view frustum
    /// </summary>
    public struct CameraViewFrustum
    {
        public static float x = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).x * 2;
        public static float y = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)).y * 2;
    }

    public static GameObject ballGO;
    public static GameObject flagGO;

    public static GameControlScript gcsScript;    
    public static Ball ballScript;

    public static void Initialize()
    {
        ballScript = GameObject.FindObjectOfType<Ball>();
        gcsScript = GameObject.FindObjectOfType<GameControlScript>();
        ballGO = ballScript.gameObject;
        flagGO = GameObject.Find("Flag");
    }
}
