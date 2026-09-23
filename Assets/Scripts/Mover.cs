using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Vector3 _start;
    [SerializeField] private Vector3 _end;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _delay = 2f;

    private IEnumerator Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.position = _start;

        while (true)
        { 
            yield return MoveTo(rb, _start, _end);
 
            yield return new WaitForSeconds(_delay);
 
            yield return MoveTo(rb, _end, _start);
 
            yield return new WaitForSeconds(_delay);
        }
    }

    private IEnumerator MoveTo(Rigidbody rb, Vector3 from, Vector3 to)
    {
        var distance = Vector3.Distance(from, to);
        if (distance < Mathf.Epsilon || _speed <= 0f) yield break;

        var moveTime = distance / _speed;
        var time = 0f;

        while (time < moveTime)
        {
            rb.MovePosition(Vector3.Lerp(from, to, time / moveTime));
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(to);
    }
}
