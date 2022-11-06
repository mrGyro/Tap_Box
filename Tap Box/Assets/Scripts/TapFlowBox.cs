using Cysharp.Threading.Tasks;
using UnityEngine;

public class TapFlowBox : BaseBox
{
    [SerializeField] private Transform _parent;
    [SerializeField] private float _speed;

    private bool _isMove;
    private void Update()
    {
        Debug.DrawRay(_parent.position, _parent.forward * 50, Color.red);
    }

    public override void BoxReaction()
    {
        _isMove = true;
        var box = GetNearestForwardBox();
        if (box == null)
        {
            GameField.Instance.RemoveBox(this);
            MoveOut();
        }
        else
        {
            MoveTo(box);
        }
    }

    private async void MoveOut()
    {
        var x = _parent.forward * 50;
        while (Vector3.Distance(_parent.position, x) > 1.03f)
        {
            _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
            await UniTask.Delay(30);
        }
        
        _isMove = false;
    }

    private async void MoveTo(BaseBox box)
    {
        while (Vector3.Distance(_parent.position, box.transform.position) > 1.03f)
        {
            _parent.Translate(_parent.forward * Time.deltaTime * _speed, Space.World);
            await UniTask.Delay(30);
        }
        
        var nearestPosition = GetNearestPosition(box);

        Data.ArrayPosition = GameField.Instance.GetIndexByWorldPosition(nearestPosition);
        _parent.position = nearestPosition;
        _isMove = false;
    }

    private Vector3 GetNearestPosition(BaseBox box)
    {
        var direction = Data.ArrayPosition - box.Data.ArrayPosition;
        return GameField.Instance.GetWorldPosition(box.Data.ArrayPosition + direction.normalized);
    }

    private BaseBox GetNearestForwardBox()
    {
        var layerMask = LayerMask.GetMask($"GameFieldElement");

        if (Camera.main == null)
        {
            return null;
        }

        var ray = new Ray(_parent.position, _parent.forward * 1000);
        if (!Physics.Raycast(ray, out var hit, 1000, layerMask))
        {
            return null;
        }

        var box = hit.transform.GetComponent<BaseBox>();

        return box;
    }
}