using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private Text player_doubloons;
    [SerializeField] private Text player_time;
    [SerializeField] private Text timer;
    [SerializeField] private float timer_duration = 10;

    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject gameover_ui;

    public bool gameover { get; set; }

    private float t = 0;

    void OnEnable()
    {
        t = timer_duration;
        player_doubloons.text = "Doubloons: $" + GameManager.scene.player_score.accumulated_cash.ToString();
        player_time.text = "Time: " + GameManager.scene.player_score.GetTimePlayedString();
    }

    // Update is called once per frame
    void Update ()
    {
        t -= Time.deltaTime;
        float time = Mathf.Clamp(t, 0, timer_duration);
        timer.text = time.ToString("N0");

        if (!gameover)
            return;   

        if (Input.GetButton("Jump"))
        {
            ContinueGame();
        }

        if (Input.GetButton("B") || t <= 0)
        {
            ReturnToMenu();
        }
	}


    void ContinueGame()
    {
        gameover = false;
        t = timer_duration;
        gameover_ui.SetActive(false);
        HUD.SetActive(true);
        GameManager.scene.player_score.InsertDoubloon();
    }


    void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
