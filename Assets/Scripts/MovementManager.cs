using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private Transform[] _players;
    private Dictionary<int, Transform> _playerIds;

    private int _ownerId = -1;
    [HideInInspector] public UnityEvent<int, Transform> OnPositionChange;

    private void Awake()
    {
        _playerIds = new Dictionary<int, Transform>();
        for (int i = 0; i < _players.Length; i++)
        {
            _playerIds.Add(i, _players[i]);
        }
    }
    public void SetOwner(int id)
    {
        _ownerId = id;
    }

    private void Update()
    {
        if (_ownerId == -1) return;

        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 velocity = new Vector3(horizontal, 0, vertical);

        _players[_ownerId].position += velocity;
        if (velocity.magnitude > 0)
        {
            OnPositionChange?.Invoke(_ownerId, _players[_ownerId]);
        }
    }
    public void MovePlayer(int id, Vector3 pos)
    {
        _playerIds[id].position = pos;
    }

}
