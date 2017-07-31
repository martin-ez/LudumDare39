using UnityEngine;
using UnityEngine.UI;

public class Source : MonoBehaviour
{
    public GameObject pulsePrefab;
    public GameObject sourceDestroyedPrefab;

    public int startingPower;
    public int uses;

    int powerLeft;
    int powerRate;

    Vector2 coords;

    [Header("UI")]
    public Image healthBar;
    public Text healthText;

    void Start()
    {
        powerLeft = startingPower;
        powerRate = startingPower / uses;
        coords = GetComponentInParent<HexUnit>().coords;

        healthBar.fillAmount = 1;
        healthText.text = powerLeft + " / " + startingPower;
    }

    public void StartExtract()
    {
        FindObjectOfType<HexGrid>().Pulse += OnPulse;
    }

    void OnPulse()
    {
        GameObject newPulse = Instantiate(pulsePrefab);

        newPulse.GetComponent<Pulse>().Generate(coords, powerRate);

        powerLeft -= powerRate;
        healthBar.fillAmount = (float)powerLeft / (float)startingPower;
        healthText.text = powerLeft + " / " + startingPower;
        if (powerLeft <= 0)
        {
            DestroySource();
        }
    }

    void DestroySource()
    {
        FindObjectOfType<HexGrid>().Pulse -= OnPulse;
        GameObject sourceDestroyed = Instantiate(sourceDestroyedPrefab);
        sourceDestroyed.transform.SetParent(transform.parent);
        Destroy(gameObject);
    }
}
