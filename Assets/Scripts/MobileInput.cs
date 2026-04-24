using System;
using UnityEngine;
using Zenject;

public class MobileInput : IInput
{
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnAim;
    public event Action OnAttackPressed;
    public event Action<Vector2> OnAttackReleased;
    public event Action OnAttackCancelled;

    private readonly UICanvasJoystick moveJoystick;  
    private readonly UICanvasJoystick attackJoystick;  
    
    private bool isInputActive = false;
    
    public void Enable()
    {
        isInputActive = true;
    }

    public void Disable()
    {
        isInputActive = false;
    }
    public MobileInput(
        [Inject(Id = "MoveJoystick")] UICanvasJoystick moveJoystick,
        [Inject(Id = "AttackJoystick")] UICanvasJoystick attackJoystick)
    {
        this.moveJoystick = moveJoystick;
        this.attackJoystick = attackJoystick;
        
        moveJoystick.OnJoystickPositionChanged += HandleMove;
        attackJoystick.OnJoystickPositionChanged += HandleAim;
        attackJoystick.OnJoystickPressed += HandleAttackPressed;
        attackJoystick.OnJoystickReleased += HandleAttackReleased;
        attackJoystick.OnJoystickCancelled += HandleAttackCancelled;
    }

    private void HandleMove(Vector2 direction)
    {
        OnMove?.Invoke(direction);
    }
    private void HandleAim(Vector2 direction)
    {
        OnAim?.Invoke(direction);
    }
    private void HandleAttackPressed(Vector2 direction)
    {
        OnAttackPressed?.Invoke();
    }
    private void HandleAttackReleased(Vector2 direction)
    {
        OnAttackReleased?.Invoke(direction);
    }
    private void HandleAttackCancelled()
    {
        OnAttackCancelled?.Invoke();
    }
}
