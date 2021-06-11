﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class Score
{
    public string pseudo;
    public float time;
    public int score;
    public int idNiveau;
}


public class GameManager : MonoBehaviour
{
    private int id_level;
    private bool isPause = false;
    private int score = 0;
    private string pseudo;
    private int bestScore;
    private string time;
    private float secElapsed = 0f;
    private float minElapsed = 0f;
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI scoreUI;
    public GameObject pauseMenu;
    public GameObject winScreen;
    public GameObject daethScreen;


    private void Start()
    {
        string[] subs = SceneManager.GetActiveScene().name.Split('_');
        this.id_level = int.Parse(subs[1]);
        this.pseudo = "WiZaR";
        StartCoroutine(GetBestScore("http://hell-of-cthulhu/api.php?action=get_best_score&pseudo=" + this.pseudo + "&id_niveau=" + this.id_level));
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.F)) IncreaseScore();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPause) ResumeGame();
            else PauseGame();
        }

        secElapsed += Time.deltaTime;

        if (secElapsed >= 59)
        {
            minElapsed += 1f;
            secElapsed = 0f;
        }

        time = Mathf.Round(minElapsed).ToString("00") + ":" + Mathf.Round(secElapsed).ToString("00");
        timeUI.text = time;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPause = true;
        GameObject.Find("WeaponHolder").GetComponent<WeaponSwitching>().enabled = false;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        isPause = false;
        GameObject.Find("WeaponHolder").GetComponent<WeaponSwitching>().enabled = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void Leave()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToOption()
    {
        // set non-visible l'UI PauseMenu
        // set visible le GameObject qui contnient mon UI OPTION
    }

    public void IncreaseScore()
    {
        score += 100;
        scoreUI.text = $"{score}";
    }

    public void GameWin()
    {
        winScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        if (this.bestScore == -1) StartCoroutine(PostRun());
        else if (this.score > this.bestScore) StartCoroutine(UpdateBestRun());
        score = 0;
    }

    public void GameOver()
    {
        daethScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        score = 0;
    }

    IEnumerator UpdateBestRun()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "update_rank");
        form.AddField("id_niveau", this.id_level);
        form.AddField("pseudo", this.pseudo);
        form.AddField("score", this.score);
        form.AddField("temps", this.time);
        using (UnityWebRequest www = UnityWebRequest.Post("http://hell-of-cthulhu/api.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) Debug.Log(www.error);
        }
    }

    IEnumerator PostRun()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "insert_rank");
        form.AddField("id_niveau", this.id_level);
        form.AddField("pseudo", this.pseudo);
        form.AddField("score", this.score);
        form.AddField("temps", this.time);
        using (UnityWebRequest www = UnityWebRequest.Post("http://hell-of-cthulhu/api.php", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) Debug.Log(www.error);
        }
    }

    IEnumerator GetBestScore(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();
        if (uwr.isNetworkError) Debug.Log("Error While Sending: " + uwr.error);
        else
        {
            Debug.Log(uwr.downloadHandler.text);
            if (uwr.downloadHandler.text == "false")
            {
                Debug.Log("il est false le batard");
                this.bestScore = -1;
            }
            else
            {
                var myObject = JsonUtility.FromJson<Score>(uwr.downloadHandler.text);
                this.bestScore = myObject.score;
                Debug.Log(this.bestScore);
            }
        }
    }
}
