using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    // 4 copper + 1 prismalite = 1 wire
    // 4 copper + 1 zetthyst = 4 wires

    int copper = 0;
    int prismalite = 0;
    int zetthyst = 0;
    int wire = 0;

    private Rigidbody _rigidBody;
    private Vector3 _velocity;
    public Transform model;

    private float _speed = 45f;

    bool canCraft;

    [Header("UI")]
    public Text countCopper;
    public Text countPrisma;
    public Text countZet;
    public Text countWire;

    public Image addCopper;
    public Text addCountCopper;
    public Image addPrisma;
    public Text addCountPrisma;
    public Image addZet;
    public Text addCountZet;
    public Image addWire;
    public Text addCountWire;
    public Text[] texts;

    public Image crafRecipe;

    public float fadeUI = 5f;
    public Color add;
    public Color remove;

    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        UpdateCount();
        addCopper.canvasRenderer.SetAlpha(0f);
        addPrisma.canvasRenderer.SetAlpha(0f);
        addZet.canvasRenderer.SetAlpha(0f);
        addWire.canvasRenderer.SetAlpha(0f);
        addCountCopper.canvasRenderer.SetAlpha(0f);
        addCountPrisma.canvasRenderer.SetAlpha(0f);
        addCountZet.canvasRenderer.SetAlpha(0f);
        addCountWire.canvasRenderer.SetAlpha(0f);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].canvasRenderer.SetAlpha(0f);
        }

        canCraft = false;
        crafRecipe.gameObject.SetActive(false);
    }

    public void Move(Vector3 movement)
    {
        if (movement.magnitude > 1f) movement.Normalize();
        movement = transform.InverseTransformDirection(movement);

        _velocity = (movement * _speed);

        _rigidBody.velocity = _velocity;
        if (movement != Vector3.zero)
        {
            model.LookAt(transform.position + movement);
            model.eulerAngles = new Vector3(0, model.eulerAngles.y, 0);
        }
    }

    public bool HaveResources()
    {
        if (wire > 0)
        {
            wire--;
            UpdateCount();
            return true;
        }
        return false;
    }

    public void Mine()
    {
        int newCopper = Random.Range(2, 7);
        copper += newCopper;

        float chance = Random.value;
        int newPrismalite = 0;
        if (chance < 0.75f)
        {
            newPrismalite = Random.Range(1, 3);
        }
        prismalite += newPrismalite;

        chance = Random.value;
        int newZetthyst = 0;
        if (chance < 0.1f)
        {
            newZetthyst = 1;
        }
        zetthyst += newZetthyst;

        UpdateCount();
        ShowAdds(newCopper, newPrismalite, newZetthyst, 0);
        FindObjectOfType<AudioManager>().PlaySound(AudioManager.Sound.Mine);
        if (!canCraft)
        {
            CheckCanCraff();
        }
    }

    public void ConvertToWire()
    {
        int newWire = 0;
        int pCopper = copper;
        int pPrisma = prismalite;
        int pZet = zetthyst;

        bool can = true;
        while (can)
        {
            if (pCopper >= 4)
            {
                if (pZet >= 1)
                {
                    pZet--;
                    pCopper -= 4;
                    newWire += 4;
                }
                else
                {
                    can = false;
                }
            }
            else
            {
                can = false;
            }
        }

        can = true;
        while (can)
        {
            if (pCopper >= 4)
            {
                if (pPrisma >= 1)
                {
                    pPrisma--;
                    pCopper -= 4;
                    newWire++;
                }
                else
                {
                    can = false;
                }
            }
            else
            {
                can = false;
            }
        }

        wire += newWire;
        int newCopper = pCopper - copper;
        int newPrismalite = pPrisma - prismalite;
        int newZetthyst = pZet - zetthyst;

        copper = pCopper;
        prismalite = pPrisma;
        zetthyst = pZet;

        UpdateCount();
        ShowAdds(newCopper, newPrismalite, newZetthyst, newWire);
        canCraft = false;
        crafRecipe.gameObject.SetActive(false);
        FindObjectOfType<AudioManager>().PlaySound(AudioManager.Sound.CraftWire);
    }

    void UpdateCount()
    {
        countCopper.text = "" + copper;
        countPrisma.text = "" + prismalite;
        countZet.text = "" + zetthyst;
        countWire.text = "" + wire;
    }

    void ShowAdds(int nCopper, int nPrisma, int nZet, int nWire)
    {
        addCopper.canvasRenderer.SetAlpha(0.8f);
        addPrisma.canvasRenderer.SetAlpha(0.8f);
        addZet.canvasRenderer.SetAlpha(0.8f);
        addWire.canvasRenderer.SetAlpha(0.8f);
        addCountCopper.canvasRenderer.SetAlpha(0.8f);
        addCountPrisma.canvasRenderer.SetAlpha(0.8f);
        addCountZet.canvasRenderer.SetAlpha(0.8f);
        addCountWire.canvasRenderer.SetAlpha(0.8f);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].canvasRenderer.SetAlpha(0.8f);
        }

        if (nCopper == 0)
        {
            addCopper.gameObject.SetActive(false);
        }
        else
        {
            addCopper.gameObject.SetActive(true);
            addCountCopper.text = "" + nCopper;
            addCopper.color = (nCopper > 0) ? add : remove;
        }

        if (nPrisma == 0)
        {
            addPrisma.gameObject.SetActive(false);
        }
        else
        {
            addPrisma.gameObject.SetActive(true);
            addCountPrisma.text = "" + nPrisma;
            addPrisma.color = (nPrisma > 0) ? add : remove;
        }

        if (nZet == 0)
        {
            addZet.gameObject.SetActive(false);
        }
        else
        {
            addZet.gameObject.SetActive(true);
            addCountZet.text = "" + nZet;
            addZet.color = (nZet > 0) ? add : remove;
        }

        if (nWire == 0)
        {
            addWire.gameObject.SetActive(false);
        }
        else
        {
            addWire.gameObject.SetActive(true);
            addCountWire.text = "" + nWire;
            addWire.color = (nWire > 0) ? add : remove;
        }

        addCopper.CrossFadeAlpha(0f, fadeUI, false);
        addPrisma.CrossFadeAlpha(0f, fadeUI, false);
        addZet.CrossFadeAlpha(0f, fadeUI, false);
        addWire.CrossFadeAlpha(0f, fadeUI, false);
        addCountCopper.CrossFadeAlpha(0f, fadeUI, false);
        addCountPrisma.CrossFadeAlpha(0f, fadeUI, false);
        addCountZet.CrossFadeAlpha(0f, fadeUI, false);
        addCountWire.CrossFadeAlpha(0f, fadeUI, false);
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].CrossFadeAlpha(0f, fadeUI, false);
        }
    }

    void CheckCanCraff()
    {
        if (copper >= 4 && (prismalite > 0 || zetthyst > 0))
        {
            crafRecipe.gameObject.SetActive(true);
        }
    }
}
