using UnityEngine;

public interface IMoveDirectionProvider
{
    public Vector3 CurrentMoveDirectionWorld { get; }
}