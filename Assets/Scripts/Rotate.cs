using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateForce;
    private bool isReverse;

    private void Update()
    {
        transform.Rotate(0, rotateForce * Time.deltaTime * (isReverse ? -1 : 1), 0) ;
        
    }

    public void Reverse(bool reverse)
    {
        isReverse = reverse;
    }

}