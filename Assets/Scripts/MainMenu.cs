using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Transform character;
    public string levelToLoad;

    public float hoverAmount;
    public float hoverSpeed;

    Vector3 from;
    Vector3 to;

    Vector3 rotFrom;
    Vector3 rotTo;

    float timePassed = 0;

    void Start()
    {
        from = character.position;
        to = from + Vector3.down * hoverAmount;

        rotFrom = new Vector3(0, 5, 0);
        rotTo = new Vector3(0, 2, 0);
    }

    void Update()
    {
        float percent = timePassed / hoverSpeed;

        character.position = Vector3.Lerp(from, to, percent);
        character.eulerAngles = Vector3.Lerp(rotFrom, rotTo, percent);

        if (percent >= 1)
        {
            timePassed = 0;
            Vector3 temp = from;
            from = to;
            to = temp;
            temp = rotFrom;
            rotFrom = rotTo;
            rotTo = temp;
        }
        else
        {
            timePassed += Time.deltaTime;
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
