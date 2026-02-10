using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRotation : MonoBehaviour
{
    public Vector3 vector = new Vector3(0, 360);
    public float speedRotation = 1f;
    private void Update()
    {
        Rotate();
    }

    public void Rotate()
    {
        gameObject.transform.Rotate(vector * Time.deltaTime * speedRotation);
    }
}
