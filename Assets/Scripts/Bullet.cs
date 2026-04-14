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
        this.owner = owner; //кто сделал выстрел
        this.target = target; //куда должна лететь пуля
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
                Player player = item.GetComponent<Player>();
                if (player)
                {
                    if (player.netId != owner)
                    {
                        player.ChangeHealthValue(player.currentHealth - 1); //отнимаем одну жизнь по аналогии с примером SyncVar
                        NetworkServer.Destroy(gameObject); //уничтожаем пулю
                    }
                }
            }

            if (lifetime < 0) 
            {
                inited = false;
                //NetworkServer.Destroy(gameObject); //значит ее можно уничтожить
            }
        }
    }
}
