using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
public class Base : MonoBehaviour
{
    float deadlineMinutes = 5;
    float timePassed = 0;
    bool active = false;

    int maxPower = 1000;
    int power = 0;

    public System.Action OnWin;
    public System.Action OnLose;

    [Header("UI")]
    public Image healthBar;
    public Text healthText;

    void Start()
    {
        healthBar.fillAmount = 0;
        healthText.text = power + " / " + maxPower;
    }

    void Update()
    {
        if (active) timePassed += Time.deltaTime;

        if (timePassed >= deadlineMinutes * 60)
        {
            if (OnLose != null) OnLose();
        }
    }

    public void Activate()
    {
        active = !active;
    }

    public void GetPower(int amount)
    {
        power += amount;

        healthBar.fillAmount = (float) power / (float) maxPower;
        healthText.text = power + " / " + maxPower;

        if (power > maxPower)
        {
            if (OnWin != null) OnWin();
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
}
