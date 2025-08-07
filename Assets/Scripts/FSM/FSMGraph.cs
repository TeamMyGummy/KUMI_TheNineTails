using GameAbilitySystem;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "FSM Graph", menuName = "AI/FSM Graph")]
public class FSMGraph : NodeGraph
{
    [System.NonSerialized]
    public BaseNode currentNode;

    [System.NonSerialized] 
    private AbilitySystem _asc;

    public void InitGraph(AbilitySystem asc)
    {
        _asc = asc;
    }
    
    public void Start()
    {
        foreach (var node in nodes)
        {
            if (node is AbilityNode)
            {
                AbilityNode abilityNode = node as AbilityNode;
                abilityNode.SetContext(_asc);
            }
        }

        foreach (var node in nodes)
        {
            if (node is StartNode startNode)
            {
                currentNode = startNode.GetNextNode();
                currentNode?.OnEnter();
                break;
            }
        }
    }
    
    public void Update()
    {
        if (currentNode == null) return;
        
        var nextNode = currentNode.Execute();
        
        if (nextNode != currentNode)
        {
            currentNode.OnExit();
            nextNode?.OnEnter();
            currentNode = nextNode;
        }
    }
}