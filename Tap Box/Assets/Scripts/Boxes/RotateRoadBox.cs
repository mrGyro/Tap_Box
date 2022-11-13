using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RotateRoadBox : BaseBox
{
    [SerializeField] private Transform _parent;
    [SerializeField] private float _speed;

    private bool _isMove;

    private void Update()
    {
        Debug.DrawRay(_parent.position, _parent.forward * 50, Color.red);
        Debug.DrawRay(_parent.position, _parent.right * 50, Color.green);
        Debug.DrawRay(_parent.position, _parent.up * 50, Color.blue);
    }

    public override void BoxReaction()
    {
        if (_isMove)
            return;

        _isMove = true;
        RotateBox();
    }

    private async void RotateBox()
    {
        var target = Quaternion.LookRotation(_parent.right, _parent.up);
        while (_parent.localRotation != target)
        {
            var rot = Quaternion.RotateTowards(transform.localRotation, target, _speed * Time.deltaTime);
            _parent.localRotation = rot;
            await UniTask.Delay(30);
        }

        _isMove = false;
    }
}