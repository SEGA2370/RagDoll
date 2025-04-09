using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] Collider thisCollider;
    [SerializeField] Collider[] colliderToIgnorel;


    void Start()
    {
        foreach (Collider otherCollider in colliderToIgnorel)
        {
            Physics.IgnoreCollision(thisCollider, otherCollider, true);
        }
    }
}
