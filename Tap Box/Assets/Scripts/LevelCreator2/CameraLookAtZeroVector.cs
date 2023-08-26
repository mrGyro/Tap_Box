using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtZeroVector : MonoBehaviour
{
    [SerializeField] private Transform _thisTransform;

    // Start is called before the first frame update
    void Start()
    {
        if (_thisTransform == null)
        {
            _thisTransform = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _thisTransform.LookAt(Vector3.zero);
    }
}