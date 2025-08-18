using UnityEngine;
using Game.Inventory;

[RequireComponent(typeof(Collider2D))]
public class Honbul : MonoBehaviour
{
    public static event System.Action<int> OnCollected; // 혼불 획득 이벤트
    [SerializeField] private float pickupRadius = 0.2f; //이 거리 안으로 들어오면 획득됨
    [SerializeField] private float magnetRadius = 3f;  //이 거리 안으로 들어오면 빨려옴
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float accel = 6f; //가속
    [SerializeField] private float maxSpeed = 12f;

    private Transform _player;
    private Rigidbody2D _rb;
    private bool _magnet;
    private float _t;
    
    private static int honbulCount = 0; //디버깅용
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.gravityScale = 0f;
        }
        _player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        if (!_magnet && dist <= magnetRadius)
        {
            _magnet = true;
            _t = 0f;
        }
        
        if (_magnet)
        {
            _t += Time.deltaTime;
            Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
            float speed = Mathf.Min(maxSpeed, baseSpeed + accel * _t);

            if (_rb != null)
                _rb.velocity = Vector2.Lerp(_rb.velocity, dir * speed, 0.5f);
            else
                transform.position = Vector2.MoveTowards(transform.position, _player.position, speed * Time.deltaTime);

            if (dist <= pickupRadius)
                Absorb();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Absorb();
    }

    private void Absorb()
    {
        var pc = _player?.GetComponent<PlayerController>();

        //인벤토리 추가
        DomainFactory.Instance.GetDomain(DomainKey.Inventory, out InventoryDomain inv);
        inv.AddItem(ItemType.Honbul, 1);

        Destroy(gameObject);
        honbulCount++;

        OnCollected?.Invoke(honbulCount);
        
        Debug.Log($"[Honbul] 현재 혼불 개수: {honbulCount}");
    }
}
