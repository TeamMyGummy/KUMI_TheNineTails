using UnityEngine;

public class SpawnHandler : ActionHandler
{
    //봐서 노드로 옮겨도 될 듯
    [SerializeField] private GameObject spawnObject;
    private Vector3 spawnPosition;
    private float delay;

    public void SetPosition(Vector2 pos, float delay)
    {
        spawnPosition = pos;
        this.delay = delay;
    }
    public override void OnEnterAction()
    {
        GameObject spawned = ResourcesManager.Instance.Instantiate(spawnObject);
        spawned.transform.position = spawnPosition;
        if(delay > 0f) ResourcesManager.Instance.Destroy(spawned, delay);
    }
    public override bool OnExecuteAction()
    {
        return true;
    }
}
