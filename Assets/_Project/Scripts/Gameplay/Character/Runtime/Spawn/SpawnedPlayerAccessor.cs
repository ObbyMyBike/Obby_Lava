using UnityEngine;

public class SpawnedPlayerAccessor
{
    private Player _player;
    private CharacterController _characterController;
    private Transform _transform;
    private Transform _playerModelRoot;

    public Player Player => _player;
    public CharacterController CharacterController => _characterController;
    public Transform Transform => _transform;
    public Transform PlayerModelRoot => _playerModelRoot;

    public void Set(Player player)
    {
        if (player == null)
            return;

        _player = player;
        _transform = player.transform;
        _playerModelRoot = player.PlayerModelRoot;
        
        if (!_player.TryGetComponent(out _characterController))
            _characterController = null;
    }
}