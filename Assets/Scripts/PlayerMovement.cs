using Mirror;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ZenAutoInjecter))] 
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform modelTransform;
    
    [Inject] IInput _input;
    
    public override void OnStartAuthority()
    {
        if (_input != null)
            _input.OnMove += Move;
    }

    private void OnDisable()
    {
        if (_input != null)
            _input.OnMove -= Move;
    }

    void Move(Vector2 direction)
    {
        float speedDeltaTime = 5f * Time.deltaTime;
        transform.Translate(new Vector3(direction.x * speedDeltaTime, 0, direction.y * speedDeltaTime));
        var rotationDirection = Vector3.RotateTowards(modelTransform.transform.forward, new Vector3(direction.x, 0, direction.y).normalized, speedDeltaTime, 0);
        modelTransform.transform.rotation = Quaternion.LookRotation(rotationDirection);
    }
}
