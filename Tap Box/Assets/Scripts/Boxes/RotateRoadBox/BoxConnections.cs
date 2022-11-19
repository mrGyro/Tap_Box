using UnityEngine;

public class BoxConnections : MonoBehaviour
{
    public bool Up;
    public bool Down;
    public bool Left;
    public bool Right;
    public bool Forward;
    public bool Back;

    private void OnDrawGizmosSelected()
    {
        var thisTransform = transform;
        var position = thisTransform.position;

        if (Up)
            Debug.DrawRay(position, transform.up * 50, Color.green);

        if (Down)
            Debug.DrawRay(position, -transform.up * 50, Color.green);

        if (Left)
            Debug.DrawRay(position, transform.right * 50, Color.green);

        if (Right)
            Debug.DrawRay(position, -transform.right * 50, Color.green);

        if (Forward)
            Debug.DrawRay(position, transform.forward * 50, Color.green);

        if (Back)
            Debug.DrawRay(position, -transform.forward * 50, Color.green);
    }
}