using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class Monster : MonoBehaviour, IInteractable
{
    [SerializeField] private float _speed = 1.5f;
    public float Speed { get => _speed; private set => _speed = value;  }

    public bool isCanInteract => _isCanInteract;
    private bool _isCanInteract;

    public StateMachine StateMachine;

    [HideInInspector] public RestState RestState;
    [SerializeField] private Transform _restPlace;

    [HideInInspector] public PatrolState RatrolState;
    [SerializeField] private PatrolData PatrolData;

    [HideInInspector] public WanderingState WanderingState;

    [HideInInspector] public FollowingState FollowingState;
    [SerializeField] private MonstersMoveByClickController monstersMoveByClickController;

    private void Start()
    {
        InitializeStates();

        StateMachine = new StateMachine();
        StateMachine.Init(WanderingState);
    }

    private void InitializeStates()
    {
        RestState = new RestState(this, Speed, _restPlace);
        RatrolState = new PatrolState(this, Speed, PatrolData);
        WanderingState = new WanderingState(this, Speed, 5f);
    }

    private void Update()
    {
        StateMachine.CurrentState.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (StateMachine.CurrentState.GetType() == RestState.GetType()) StateMachine.ChangeState(RatrolState);
            else StateMachine.ChangeState(RestState);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collider = collision.GetComponent<CharacterController2D>();
        if (collider != null)
        {
            _isCanInteract = true;
            StateMachine.ChangeState(new FollowingState(this, Speed, collider.gameObject));
        }
    }

    public void Interact()
    {
        _isCanInteract = false;
        monstersMoveByClickController.enabled = true;
        Debug.Log($"Im interact with {this.name}");
    }
}
