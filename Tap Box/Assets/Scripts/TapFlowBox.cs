using UnityEngine;

public class TapFlowBox : BaseBox
{
    [SerializeField] private Transform _parent;
    public override void BoxReaction()
    {
        base.BoxReaction();
        Debug.LogError("reaction");
    }
    
    private void Update()
    {
        Debug.DrawRay(_parent.position, _parent.forward  * 50, Color.red);
    }
}
