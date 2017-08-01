using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CapsuleCollider))]
public class Base : MonoBehaviour
{
    bool active = true;
    float deadlineMinutes = 5f;
    float timePassed = 0;

    int maxPower = 500;
    int power = 0;

    [Header("UI")]
    public Image healthBar;
    public Text healthText;

    public Image timeBar;
    public Text timeText;

    public Image finishPanel;
    public Text win;
    public Text lose;

    [Header("Music")]
    AudioManager audio;

    void Start()
    {
        healthBar.fillAmount = 0;
        healthText.text = power + " / " + maxPower;

        timeBar.fillAmount = 1;
        timeText.text = "5:00";

        audio = FindObjectOfType<AudioManager>();

        audio.SetVolume(1f, AudioManager.AudioChannel.Acoustic);
        audio.SetVolume(0f, AudioManager.AudioChannel.Electric);
    }

    void Update()
    {
        if (active)
        {
            timePassed += Time.deltaTime;

            string minText = "";
            string secText = "";
            int seconds = (int)((deadlineMinutes * 60f) - timePassed);
            minText = "" + Mathf.RoundToInt(seconds / 60);
            int sec = Mathf.RoundToInt(seconds % 60);
            if (sec < 10)
            {
                secText = "0" + sec;
            }
            else
            {
                secText = "" + sec;
            }
            timeText.text = minText + ":" + secText;

            timeBar.fillAmount = ((float)seconds / (float)(deadlineMinutes * 60f));

            if (timePassed >= deadlineMinutes * 60)
            {
                OnLose();
            }
        }
    }

    public void GetPower(int amount)
    {
        if (active)
        {
            if (amount > 0)
            {
                power += amount;
            }

            healthBar.fillAmount = (float)power / (float)maxPower;
            healthText.text = power + " / " + maxPower;

            if (power > maxPower)
            {
                OnWin();
            }

            if (power > maxPower / 2f)
            {
                float max = (float)maxPower / 2f;
                float current = (float)power - max;
                float percent = current / max;
                audio.SetVolume(1 - percent, AudioManager.AudioChannel.Acoustic);
                audio.SetVolume(percent, AudioManager.AudioChannel.Electric);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Char"))
        {
            FindObjectOfType<InputController>().Interact += OnInteract;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Char"))
        {
            FindObjectOfType<InputController>().Interact -= OnInteract;
        }
    }

    void OnInteract()
    {
        FindObjectOfType<Character>().ConvertToWire();
    }

    void OnWin()
    {
        active = false;
        win.gameObject.SetActive(true);
        lose.gameObject.SetActive(false);
        StartCoroutine(BringPanel());
    }

    void OnLose()
    {
        active = false;
        win.gameObject.SetActive(false);
        lose.gameObject.SetActive(true);
        StartCoroutine(BringPanel());
    }

    IEnumerator BringPanel()
    {
        float length = 0.75f;
        float timePassed = 0f;

        float percent = 0f;

        while (percent < 1)
        {
            finishPanel.rectTransform.localScale = new Vector3(1, Mathf.Lerp(0, 1, percent), 1);
            timePassed += Time.deltaTime;
            percent = timePassed / length;
            yield return null;
        }

    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
