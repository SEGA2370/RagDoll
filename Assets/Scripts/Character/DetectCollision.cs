using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    NetworkPlayer networkPlayer;
    Rigidbody hitRigidbody;

    ContactPoint[] contactPoints = new ContactPoint[5];

    void Awake()
    {
        networkPlayer = GetComponentInParent<NetworkPlayer>();
        hitRigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!networkPlayer.HasStateAuthority)
        {
            return;
        }

        if (!networkPlayer.IsActiveRagdoll)
        {
            return;
        }

        if (!collision.collider.CompareTag("CauseDamage"))
        {
            return;
        }

        //Avoid having the player hurt themselves
        if (collision.collider.transform.root == networkPlayer.transform)
        {
            return;
        }

        int numberOfContacts = collision.GetContacts(contactPoints);

        for (int i = 0; i < numberOfContacts; i++)
        {
            ContactPoint contactPoint = contactPoints[i];

            // Get the contact impulse
            Vector3 contactIpmulse = contactPoint.impulse / Time.fixedDeltaTime;

            //Check that the force was great enough to cause a knockout
            if (contactIpmulse.magnitude < 15)
                continue;

            networkPlayer.OnPlayerBodyPartHit();

            Vector3 forceDirection = (contactIpmulse + Vector3.up) * 0.5f;

            //Limit the force so it doesnt get to big
            forceDirection = Vector3.ClampMagnitude(forceDirection, 30);

            Debug.DrawRay(hitRigidbody.position, forceDirection * 40, Color.red, 4);

            //Increase the effect of the hit
            hitRigidbody.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
