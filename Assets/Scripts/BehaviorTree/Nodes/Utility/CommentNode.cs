using BehaviorTree;
using UnityEngine;
using XNode;

[CreateNodeMenu("Utility/Comment")]
[NodeTint(NodeColorPalette.COMMENT_NODE)]
public class CommentNode : Node
{
    [TextArea(3, 10)]
    public string comment;

    public override object GetValue(NodePort port) => null;
}