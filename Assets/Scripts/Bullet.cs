using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public uint owner;
    bool inited;
    Vector3 target;
    private float lifetime = 3f;

    [Server]
    public void Init(uint owner, Vector3 target)
    {
        this.owner = owner; 
        this.target = target; 
        inited = true;
    }

    void Update()
    {
        if (inited && isServer)
        {
            lifetime -= Time.deltaTime;
            transform.Translate(target * Time.deltaTime * 5);

            foreach (var item in Physics.OverlapSphere(transform.position, 1f))
            {
                var player = item.GetComponent<Player>();
                if (player)
                {
                    if (player.netId != owner)
                    {
                        player.TakeDamage(1); 
                        NetworkServer.Destroy(gameObject); 
                    }
                }
            }

            if (lifetime < 0) 
            {
                inited = false;
                //NetworkServer.Destroy(gameObject); 
            }
        }
    }
}
