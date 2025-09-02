using UnityEngine;

public interface IPlayerMoveDirectionProvider
{
    public Vector3 CurrentMoveDirectionWorld { get; }
}