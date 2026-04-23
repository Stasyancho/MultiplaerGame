using System;
using Zenject;
using UnityEngine;

public class DesktopInput : IInput, ITickable
{
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnAim;
    public event Action OnAttackPressed;
    public event Action<Vector2> OnAttackReleased;
    public event Action OnAttackCancelled;

    private bool isInputActive = false;
    private bool isAttackMode = false;
    
    public void Enable()
    {
        isInputActive = true;
    }

    public void Disable()
    {
        isInputActive = false;
    }

    public void Tick()
    {
        // if (!isInputActive)
        //     return;
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            OnMove?.Invoke(new Vector2(h, v));
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (isAttackMode)
            {
                OnAttackCancelled?.Invoke();
                isAttackMode = false;
            }
            else
            {
                OnAttackPressed?.Invoke();
                isAttackMode = true;
            }
        }

        if (isAttackMode)
        {
            Vector2 direction = GetMouseDirection();

            OnAim?.Invoke(direction);
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && isAttackMode)
        {
            Vector2 direction = GetMouseDirection();
            
            OnAttackReleased?.Invoke(direction);
            
            isAttackMode = false;
        }
    }

    private Vector2 GetMouseDirection()
    {
        Vector2 pos2 = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        return (pos2 - screenCenter).normalized;
    }
}
