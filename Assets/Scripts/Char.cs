using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Character : MonoBehaviour
{
    int resources = 1000;
    int cableCost = 10;

    private Rigidbody _rigidBody;
    private Vector3 _velocity;

    private float _speed = 7f;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Move(Vector3 movement)
    {
        if (movement.magnitude > 1f) movement.Normalize();
        movement = transform.InverseTransformDirection(movement);

        _velocity = (movement * _speed);

        _rigidBody.velocity = _velocity;
        transform.LookAt(transform.position + movement);
    }

    public bool HaveResources()
    {
        if (resources >= cableCost)
        {
            resources -= cableCost;
            return true;
        }
        return false;
    }
}
