using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControlScript : MonoBehaviour
{
    int score = 0;
    public bool gameOver = false;

    Text curScoreText;
    GameObject gameOverPanel;
    Text finalScoreText;
    Text bestScoreText;

    void Start()
    {
        UsefulReferences.Initialize();
        curScoreText = GameObject.Find("CurScoreText").GetComponent<Text>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        finalScoreText = gameOverPanel.transform.Find("FinalScoreText").gameObject.GetComponent<Text>();
        bestScoreText = gameOverPanel.transform.Find("BestScoreText").gameObject.GetComponent<Text>();
        GameReset();
    }

    void Update()
    {
        //update current score text value
        curScoreText.text = "SCORE: " + score;

        //when the game is over, some objects have to change their activity state
        curScoreText.enabled = !gameOver;
        gameOverPanel.gameObject.SetActive(gameOver);
        UsefulReferences.ballScript.enabled = !gameOver;

        if (gameOver)
        {
            //update score values on the game over screen and make sure
            finalScoreText.text = curScoreText.text;
            bestScoreText.text = "BEST: " + PlayerPrefs.GetInt("BestScore").ToString();
        }
    }

    public void Score()
    {
        score++;
        UsefulReferences.ballScript.shootingPower *= 1.1f;
        GameReset();
        //if the score is higher than current high score, set the BestScore to current score
        if (score > PlayerPrefs.GetInt("BestScore"))
            PlayerPrefs.SetInt("BestScore", score);
    }

    public void Restart()
    {
        gameOver = false;
        score = 0;
        UsefulReferences.ballScript.shootingPower = UsefulReferences.ballScript.startValShootingPower;
        GameReset();
    }

    /// <summary>
    /// Called on the game start, scoring and restart
    /// </summary>
    public void GameReset()
    {
        UsefulReferences.ballGO.transform.position = UsefulReferences.ballScript.startPos;
        UsefulReferences.flagGO.transform.position = new Vector3(Random.Range(0, UsefulReferences.CameraViewFrustum.x / 2), UsefulReferences.flagGO.transform.position.y, 0);
        UsefulReferences.ballScript.isClicked = false;
        UsefulReferences.ballScript.rb.velocity = Vector2.zero; //reset ball's velocity to be sure it isn't moving
    }
}
