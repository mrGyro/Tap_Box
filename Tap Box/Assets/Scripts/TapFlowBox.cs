using UnityEngine;

public class TapFlowBox : BaseBox
{
    public override void BoxReaction()
    {
        base.BoxReaction();
        Debug.LogError("reaction");
    }
}
