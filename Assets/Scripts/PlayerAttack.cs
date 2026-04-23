using Mirror;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ZenAutoInjecter))] 
public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject directionVisual; 
    
    [Inject] IInput _input;

    public override void OnStartAuthority()
    {
        if (_input != null)
        {
            _input.OnAim += SetAimDirection;
            _input.OnAttackPressed += ShowDirectionVisual;
            _input.OnAttackCancelled += HideDirectionVisual;
            _input.OnAttackReleased += PerformAttack;
        }
    }

    private void OnDisable()
    {
        if (_input != null)
        {
            _input.OnAim -= SetAimDirection;
            _input.OnAttackPressed -= ShowDirectionVisual;
            _input.OnAttackCancelled -= HideDirectionVisual;
            _input.OnAttackReleased -= PerformAttack;
        }
    }

    private void SetAimDirection(Vector2 dir)
    {
        if (directionVisual != null && dir != Vector2.zero)
        {
            Vector3 dir3D = new Vector3(dir.x, 0, dir.y);
            directionVisual.transform.rotation = Quaternion.LookRotation(dir3D);
        }
    }

    private void ShowDirectionVisual()
    {
        if (directionVisual != null)
            directionVisual.SetActive(true);
    }

    private void HideDirectionVisual()
    {
        if (directionVisual != null)
            directionVisual.SetActive(false);
    }

    private void PerformAttack(Vector2 dir)
    {
        HideDirectionVisual();
        if (isServer)
            SpawnBullet(dir);
        else
            CmdSpawnBullet(dir);
    }

    [Command]
    private void CmdSpawnBullet(Vector2 direction)
    {
        SpawnBullet(direction);
    }

    [Server]
    private void SpawnBullet(Vector2 direction)
    {
        Vector3 targetDir = new Vector3(direction.x, 0, direction.y).normalized;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(bullet);
        bullet.GetComponent<Bullet>().Init(netId, targetDir);
    }
}
