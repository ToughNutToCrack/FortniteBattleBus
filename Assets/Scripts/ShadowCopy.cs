using UnityEngine;

public class ShadowCopy : MonoBehaviour
{
    public Transform target;
    public Transform target2;
    public Vector3 rotationOffset;

    void Update()
    {
        transform.position = target.position;
        transform.rotation = target2.rotation * Quaternion.Euler(rotationOffset);
    }
}
