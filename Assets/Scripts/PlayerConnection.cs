using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PlayerConnection : NetworkBehaviour
{ 
    [SyncVar]
    public string sessionId = "";
    public string username;
    public string ip;
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
        SpawnPLayer(connectionToClient);
    }
    [Server]
    public void SpawnPLayer(NetworkConnectionToClient conn)
    {
        var playerGO = Instantiate(gamePlayerPrefab); 
            
        NetworkServer.Spawn(playerGO, conn);
        //playerGO.GetComponent<Player>().Init(conn);
    }
}
