using BehaviorTree.Leaf;

namespace BehaviorTree
{
    public interface IBTGraph
    {
        public BTContext Context { get;}
        public void CheckPrevNode(LeafNode currNode);
        public void StopGraph();
    }
}