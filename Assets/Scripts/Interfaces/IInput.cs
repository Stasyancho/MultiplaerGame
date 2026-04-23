using System;
using UnityEngine;

public interface IInput
{
    event Action<Vector2> OnMove;
    event Action<Vector2> OnAim;
    event Action OnAttackPressed;
    event Action<Vector2> OnAttackReleased;
    event Action OnAttackCancelled;

    void Enable();
    void Disable();
}
