using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxConnections : MonoBehaviour
{
    public bool Up;
    public bool Down;
    public bool Left;
    public bool Right;
    public bool Forward;
    public bool Back;

    public bool HasConnection(BoxConnections otherBox)
    {
        if (otherBox == this)
            return false;

        var connections = otherBox.GetConnectPoint();
        var thisConnections = GetConnectPoint();

        foreach (var connection in thisConnections)
        {
            var result = connections.FirstOrDefault(x => 
                Vector3.Distance(x, transform.position) < 0.3f 
                && Vector3.Distance(otherBox.transform.position, connection) < 0.3f);
            
            if (result != Vector3.zero)
            {
                return true;
            }
        }

        return false;
    }

    private List<Vector3> GetConnectPoint()
    {
        List<Vector3> result = new List<Vector3>();
        var transform1 = transform;

        if (Up)
            result.Add(transform1.position + transform1.up);

        if (Down)
            result.Add(transform1.position - transform1.up);

        if (Right)
            result.Add(transform1.position + transform1.right);

        if (Left)
            result.Add(transform1.position - transform1.right);

        if (Forward)
            result.Add(transform1.position + transform1.forward);

        if (Back)
            result.Add(transform1.position - transform1.forward);

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        var thisTransform = transform;
        var position = thisTransform.position;

        if (Up)
            Debug.DrawRay(position, transform.up * 50, Color.green);

        if (Down)
            Debug.DrawRay(position, -transform.up * 50, Color.green);

        if (Left)
            Debug.DrawRay(position, -transform.right * 50, Color.green);

        if (Right)
            Debug.DrawRay(position, transform.right * 50, Color.green);

        if (Forward)
            Debug.DrawRay(position, transform.forward * 50, Color.green);

        if (Back)
            Debug.DrawRay(position, -transform.forward * 50, Color.green);
    }
}