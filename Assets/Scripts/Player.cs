using System;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Player : NetworkBehaviour
{
    public NetworkConnectionToClient  connection;
    public int maxHealth = 5;
    public int currentHealth;
    public Slider healthSlider;
    public GameObject model;
    
    public GameObject BulletPrefab;
    
    [Inject] IInput _input;

    [SerializeField] private GameObject directionGO;

    public void Init(NetworkConnectionToClient conn)
    {
        connection = conn;
        
    }
    // Вызывается на всех клиентах, когда объект появляется у них
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (healthSlider != null)
        {
            currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // --- ВАЖНО: Этот код выполняется только на клиенте-владельце ---
        Debug.Log($"OnStartAuthority called for {name}. Now setting up input.");
        
        // Получаем ссылку на систему ввода (Zenject или FindObjectOfType)

        // if (_input == null)
        // {
        //     // var container = ProjectContext.Instance.Container;
        //     // _input = container.Resolve<IInput>();
        //     //ProjectContext.Instance.Container.Inject(this);
        //     
        //     var container = Context.GetContainer();
        //     container.Inject(this);  // внедрит зависимости в этот компонент
        //     var input = container.Resolve<IInput>();
        // }
        
        if (_input != null)
        {
            _input.MoveChanged += Movement;
            _input.AttackPressed += StartAttack;
            _input.AttackCancelled += EndAttack;
            _input.AimChanged += TargetDirection;
            _input.AttackReleased += Attack;
        }
        else
        {
            Debug.LogError("Input system not found!", this);
        }
    }
    public override void OnStopAuthority()
    {
        base.OnStopAuthority();
        if (_input != null)
        {
            _input.MoveChanged -= Movement;
            _input.AttackPressed -= StartAttack;
            _input.AttackCancelled -= EndAttack;
            _input.AimChanged -= TargetDirection;
            _input.AttackReleased -= Attack;
        }
    }

    public void OnDestroy()
    {
        _input.MoveChanged -= Movement;
        _input.AttackPressed -= StartAttack;
        _input.AttackCancelled -= EndAttack;
        _input.AimChanged -= TargetDirection;
        _input.AttackReleased -= Attack;
    }

    void Movement(Vector2 direction)
    {
        float speed = 5f * Time.deltaTime;
        transform.Translate(new Vector3(direction.x * speed, 0, direction.y * speed));
        var rotationDirection = Vector3.RotateTowards(model.transform.forward, new Vector3(direction.x, 0, direction.y).normalized, 5 * Time.deltaTime, 0);
        model.transform.rotation = Quaternion.LookRotation(rotationDirection);
    }

    void StartAttack()
    {
        directionGO.SetActive(true);
    }
    void EndAttack()
    {
        directionGO.SetActive(false);
    }
    void Attack(Vector2 direction)
    {
        Vector3 pos = new Vector3(direction.x, 0, direction.y);

        if (isServer)
            SpawnBullet(netId, pos);
        else
            CmdSpawnBullet(netId, pos);
        directionGO.SetActive(false);
    }

    void TargetDirection(Vector2 direction)
    {
        Vector3 direction3D = new Vector3(direction.x, 0, direction.y);
    
        // Поворачиваем объект, чтобы его forward совпадал с направлением
        directionGO.transform.rotation = Quaternion.LookRotation(direction3D);
    }

    void Update()
    {
        if (isOwned) 
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                player.GetComponent<Player>().UpdateHealth();
            }
        }
    }

    public void UpdateHealth()
    {
        healthSlider.value = currentHealth;
    }
    
    [SyncVar(hook = nameof(SyncHealth))] //задаем метод, который будет выполняться при синхронизации переменной
    int _SyncHealth;

    //метод не выполнится, если старое значение равно новому
    void SyncHealth(int oldValue, int newValue) //обязательно делаем два значения - старое и новое. 
    {
        currentHealth = newValue;
    }
    [Server] //обозначаем, что этот метод будет вызываться и выполняться только на сервере
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
    }
    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(int newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue); //переходим к непосредственному изменению переменной
    }
    [Server]
    public void SpawnBullet(uint owner, Vector3 target)
    {
        GameObject bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity); //Создаем локальный объект пули на сервере
        NetworkServer.Spawn(bulletGo); //отправляем информацию о сетевом объекте всем игрокам.
        bulletGo.GetComponent<Bullet>().Init(owner, target); //инициализируем поведение пули
    }
    [Command]
    public void CmdSpawnBullet(uint owner, Vector3 target)
    {
        SpawnBullet(owner, target);
    }
}
