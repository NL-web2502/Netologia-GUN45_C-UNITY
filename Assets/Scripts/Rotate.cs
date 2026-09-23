using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotate;

    private IEnumerator Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        while (true)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(_rotate * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
    }
}
