using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerConnection : NetworkBehaviour
{ 
    /// <summary>
    /// The Sessions ID for the current server.
    /// </summary>
    [SyncVar]
    public string sessionId = "";

    /// <summary>
    /// Player name.
    /// </summary>
    public string username;

    public string ip;

    /// <summary>
    /// Platform the user is on.
    /// </summary>
    public string platform;

    public string netid;
    
    [SerializeField] private GameObject gamePlayerPrefab;
    
    private void Awake()
    {
        username = SystemInfo.deviceName;
        platform = Application.platform.ToString();
        ip = NetworkManager.singleton.networkAddress;
    }

    private void Start()
    {
        netid = netId.ToString();
    }

    [Command]
    public void CmdSpawnPlayer()
    {
        NetworkConnectionToClient conn = connectionToClient;
        SpawnPLayer(conn);
    }
    [Server]
    public void SpawnPLayer(NetworkConnectionToClient conn)
    {
        GameObject playerGO = GameObject.Instantiate(gamePlayerPrefab); //Создаем локальный объект пули на сервере
            
        NetworkServer.Spawn(playerGO, conn);
        playerGO.GetComponent<Player>().Init(conn);
    }
}
