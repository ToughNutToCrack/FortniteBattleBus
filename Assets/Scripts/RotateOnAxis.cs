using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    public Vector3 axis;
    public float angularSpeed = 90;

    void Update()
    {
        transform.Rotate(axis, angularSpeed * Time.deltaTime);
    }
}
