using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Gates : MonoBehaviour
{
    private int _score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            _score++;
            Debug.Log($"Гол! Текущий счёт: {_score}");
            Destroy(other.gameObject);
        }
    }
}