using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using Unity.Services.Authentication;
using Unity.Services.Core;

using Utp;
using Zenject;

public class MyNetworkManager : RelayNetworkManager
    {
        /// <summary>
        /// The local player object that spawns in.
        /// </summary>
        public Player localPlayer;
        private string m_SessionId = "";
        private string m_Username;
        private string m_UserId;
        private NetworkConnectionToClient connTest;

        [SerializeField] private GameObject gamePlayerPrefab;

        /// <summary>
        /// Flag to determine if the user is logged into the backend.
        /// </summary>
        public bool isLoggedIn = false;

        /// <summary>
        /// List of players currently connected to the server.
        /// </summary>
        private List<PlayerConnection> m_Players;
        [Inject] private DiContainer _container;
        public override void Awake()
        {
            base.Awake();
            m_Players = new List<PlayerConnection>();

            m_Username = SystemInfo.deviceName;
        }

        public async void UnityLogin()
		{
			try
			{
				await UnityServices.InitializeAsync();
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
                isLoggedIn = true;
            }
			catch (Exception e)
			{
                isLoggedIn = false;
                Debug.Log(e);
			}
		}

        public override void Update()
        {
            if (NetworkManager.singleton.isNetworkActive)
            {
                if (localPlayer == null)
                {
                    FindLocalPlayer();
                }
            }
            else
            {
                localPlayer = null;
                m_Players.Clear();
            }
        }


        public override void OnStartServer()
        {
            Debug.Log("MyNetworkManager: Server Started!");

            m_SessionId = System.Guid.NewGuid().ToString();
        }
        
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            connTest = conn;
            Transform startPos = GetStartPosition();
            GameObject player = App.Unity.Instantiate(playerPrefab);
            // GameObject player = startPos != null
            //     ? _container.InstantiatePrefab(playerPrefab, startPos.position, startPos.rotation)
            //     : _container.InstantiatePrefab(playerPrefab);
            
            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
        
            foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkServer.spawned)
            {
                var comp = kvp.Value.GetComponent<PlayerConnection>();
        
                // Add to player list if new
                if (comp != null && !m_Players.Contains(comp))
                {
                    comp.sessionId = m_SessionId;
                    m_Players.Add(comp);
                }
            }
        }

        public void CreatePlayer()
        {
            if (NetworkClient.connection != null && NetworkClient.connection.identity != null)
            {
                PlayerConnection pc = NetworkClient.connection.identity.GetComponent<PlayerConnection>();
                if (pc != null)
                    pc.CmdSpawnPlayer();
            }
            else
            {
                // Fallback: поиск по тегу (работает, но медленнее)
                var player = GameObject.FindGameObjectsWithTag("PlayerConnection")
                    .First(p => p.GetComponent<PlayerConnection>().isOwned)
                    .GetComponent<PlayerConnection>();
                player.CmdSpawnPlayer();
            }
        }

        public override void OnStopServer()
        {
            Debug.Log("MyNetworkManager: Server Stopped!");
            m_SessionId = "";
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            Dictionary<uint, NetworkIdentity> spawnedPlayers = NetworkServer.spawned;
            
            // Update players list on client disconnect
            foreach (PlayerConnection player in m_Players)
            {
                bool playerFound = false;

                foreach (KeyValuePair<uint, NetworkIdentity> kvp in spawnedPlayers)
                {
                    var comp = kvp.Value.GetComponent<PlayerConnection>();

                    // Verify the player is still in the match
                    if (comp != null && player == comp)
                    {
                        playerFound = true;
                        break;
                    }
                }

                if (!playerFound)
                {
                    m_Players.Remove(player);
                    break;
                }
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            Debug.Log("MyNetworkManager: Left the Server!");

            localPlayer = null;

            m_SessionId = "";
        }

        public override void OnClientConnect()
        {
            Debug.Log($"MyNetworkManager: {m_Username} Connected to Server!");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("MyNetworkManager: Disconnected from Server!");
        }

        /// <summary>
        /// Finds the local player if they are spawned in the scene.
        /// </summary>
        void FindLocalPlayer()
        {
            //Check to see if the player is loaded in yet
            if (NetworkClient.localPlayer == null)
                return;

            localPlayer = NetworkClient.localPlayer.GetComponent<Player>();
        }
    }

