using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    // 4 copper + 1 prismalite = 1 wire
    // 4 copper + 1 zetthyst = 4 wires

    int copper = 0;
    int prismalite = 0;
    int zetthyst = 0;
    int wires = 0;

    private Rigidbody _rigidBody;
    private Vector3 _velocity;
    public Transform model;

    private float _speed = 30f;

    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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
        if (wires > 0)
        {
            wires--;
            Debug.Log("Copper: " + copper + " Prismalite: " + prismalite + " Zetthyst: " + zetthyst + " Wire: " + wires);
            return true;
        }
        return false;
    }

    public void Mine()
    {
        int newCopper = Random.Range(1, 4);
        copper += newCopper;
        float chance = Random.value;
        int newPrismalite = 0;
        if (chance < 0.5f) newPrismalite = 1;
        prismalite += newPrismalite;
        chance = Random.value;
        int newZetthyst = 0;
        if (chance < 0.05f) newZetthyst = 1;
        zetthyst += newZetthyst;
        Debug.Log("Copper: " + copper + " Prismalite: " + prismalite + " Zetthyst: " + zetthyst + " Wire: " + wires);
    }

    public void ConvertToWire()
    {
        int newWire = 0;
        bool can = true;
        while (can)
        {
            if (copper >= 4)
            {
                if (zetthyst >= 1)
                {
                    zetthyst--;
                    copper -= 4;
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
            if (copper >= 4)
            {
                if (prismalite >= 1)
                {
                    prismalite--;
                    copper -= 4;
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

        wires += newWire;

        Debug.Log("Copper: " + copper + " Prismalite: " + prismalite + " Zetthyst: " + zetthyst + " Wire: " + wires);
    }
}
