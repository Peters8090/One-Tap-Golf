using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inpiration: trajectoryScript by Mace Barihi (https://drive.google.com/file/d/1btUPxj8gRHZto4aIpKbzl7unsh-PB0Md/view)
/// </summary>
public class Ball : MonoBehaviour
{
    internal Rigidbody2D rb;
    internal Vector3 startPos = Vector3.zero;

    /// <summary>
    /// Child count of Trajectory Dots gameObject
    /// </summary>
    int dotCount;

    /// <summary>
    /// When the ball is already clicked, this var is set to true and next "ball shot" is possible in another round
    /// </summary>
    internal bool isClicked = false;

    bool trajectoryVisible = false;

    bool paraboleReachedEndOfScreen = true;

    GameObject trajectoryDotsParent;

    /// <summary>
    /// To make shooting easier
    /// </summary>
    GameObject ballClickArea;

    /// <summary>
    /// All children of Trajectory Dots gameObject
    /// </summary>
    List<Transform> dots = new List<Transform>();

    /// <summary>
    /// Velocity of the shot, difference of pointer position and ball position multiplied by shootingPower
    /// </summary>
    Vector2 shotForce = Vector2.zero;


    internal float startValShootingPower = 3f;

    internal float shootingPower = 3f;

    /// <summary>
    /// Minimum distance between pointer and ball
    /// </summary>
    float minBallPointerDist = 0.5f;

    float dotSeparation = 10f;

    /// <summary>
    /// Distance between first dot and ball
    /// </summary>
    float dotShift = 5f;

    Transform lastDotTransform;

    void Start()
    {
        startPos = transform.position;
        startValShootingPower = shootingPower;

        rb = GetComponent<Rigidbody2D>();
        trajectoryDotsParent = GameObject.Find("Trajectory Dots");
        ballClickArea = transform.Find("ClickArea").gameObject;

        dotCount = trajectoryDotsParent.transform.childCount;
        foreach (var dot in trajectoryDotsParent.GetComponentsInChildren<Transform>())
        {
            dots.Add(dot);
        }
        dots.Remove(trajectoryDotsParent.transform);
    }

    void Update()
    {
        foreach(var dot in dots)
        {
            if(dot.GetComponent<SpriteRenderer>().enabled)
            {
                lastDotTransform = dot;
            }
        }
        paraboleReachedEndOfScreen = lastDotTransform.position.x <= -UsefulReferences.CameraViewFrustum.x / 2 || lastDotTransform.position.x >= UsefulReferences.CameraViewFrustum.x / 2;

        ballClickArea.SetActive(!isClicked);

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Input.GetButtonDown("Fire1"))
        {
            if (hit.collider != null)
            {
                //check if the mouse is hovering ball click area
                if (hit.collider.transform.root.name == name)
                {
                    trajectoryVisible = true;
                }
            }
        }

        if ((Input.GetButtonUp("Fire1") || (paraboleReachedEndOfScreen && Input.GetButton("Fire1"))) && !isClicked && trajectoryVisible) //player released the mouse button or the parabole reached the end of the screen
        {
            trajectoryVisible = false;
            isClicked = true;
            UsefulReferences.ballScript.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.velocity = shotForce;
            foreach (var dot in dots)
                dot.transform.position = Vector3.zero;
            Invoke("GroundCheckAfterShot", 0.1f);
        }

        if (trajectoryVisible && !isClicked)
        {
            shotForce = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)) * shootingPower;
            for (int i = 0; i < dotCount; i++) //each dot has its unique calculated position
            {
                dots[i].gameObject.SetActive(true);

                dots[i].transform.position = new Vector2(transform.position.x + shotForce.x * Time.fixedDeltaTime * (dotSeparation * i + dotShift),
                    transform.position.y + shotForce.y * Time.fixedDeltaTime * (dotSeparation * i + dotShift) - (-Physics2D.gravity.y / 2f * Time.fixedDeltaTime * Time.fixedDeltaTime * (dotSeparation * i + dotShift) * (dotSeparation * i + dotShift)));
            }
        }
        else
        {
            foreach (var dot in dots)
            {
                dot.gameObject.SetActive(false);
            }
        }
        //if the ball tried to fly out of the screen
        if (transform.position.y > UsefulReferences.CameraViewFrustum.y / 2 || transform.position.x > UsefulReferences.CameraViewFrustum.x / 2 || transform.position.x < -UsefulReferences.CameraViewFrustum.x / 2)
        {
            OnCollisionEnter2D(null);
        }


        if (isClicked && UsefulReferences.ballScript.rb.constraints == RigidbodyConstraints2D.FreezeAll)
        {
            UsefulReferences.gcsScript.gameOver = true;
        }
    }
    
    /// <summary>
    /// To detect scoring
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hole")
        {
            UsefulReferences.gcsScript.Score();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    /// <summary>
    /// It applies to two cases: 1) Ball touches the ground 2) Ball flies out of the screen
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    /// <summary>
    /// This method is invoked a moment after shot because if there hadn't been any break, it would cause an immediate ball freeze
    /// </summary>
    void GroundCheckAfterShot()
    {
        //if the difference between start pos y and transform pos y is that low, freeze the ball
        if(transform.position.y - startPos.y < 0.05f)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
