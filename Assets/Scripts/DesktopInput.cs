using System;
using Zenject;
using UnityEngine;

public class DesktopInput : IInput, ITickable
{
    public event Action<Vector2> MoveChanged;
    public event Action<Vector2> AimChanged;
    
    public event Action AttackPressed;
    public event Action<Vector2> AttackReleased;
    public event Action AttackCancelled;

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
            MoveChanged?.Invoke(new Vector2(h, v));
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (isAttackMode)
            {
                AttackCancelled?.Invoke();
                isAttackMode = false;
            }
            else
            {
                AttackPressed?.Invoke();
                isAttackMode = true;
            }
        }

        if (isAttackMode)
        {
            Vector2 pos2 = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 direction = (pos2 - screenCenter).normalized;

            AimChanged?.Invoke(direction);
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && isAttackMode)
        {
            Vector2 pos2 = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 direction = (pos2 - screenCenter).normalized;
            
            AttackReleased?.Invoke(direction);
            
            isAttackMode = false;
        }
    }
}
