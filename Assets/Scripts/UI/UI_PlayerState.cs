using System.Collections;
using System.Collections.Generic;
using Managers;
using R3;
using TMPro;
using UI;
using UnityEngine;

public class UI_PlayerState : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hp;
    private CompositeDisposable _disposables = new();
    private VM_PlayerState _playerVM;

    void Awake()
    {
        _playerVM = GetComponent<VM_PlayerState>();
    }
    
    void Start()
    {
        //View 코드
        UpdateUp(_playerVM.Hp.CurrentValue);
        _disposables.Add(_playerVM.Hp.Subscribe(UpdateUp));
    }

    //추후 해당 코드 변경
    public void UpdateUp(float hp)
    {
        _hp.text = hp.ToString();
    }
    
    void OnDestroy()
    {
        _disposables.Dispose();
    }
}
