using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.SimpleWeb;
using TMPro;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class UIManger : MonoBehaviour
{
    [SerializeField] MyNetworkManager m_Manager;
    
    [SerializeField] GameObject StartWindow;
    [SerializeField] GameObject LobbyWindow;
    [SerializeField] GameObject GameWindow;
    
    [SerializeField] Button m_StartButton;
    [SerializeField] Button m_GetHostsButton;
    [SerializeField] Button m_HostButton;
    [SerializeField] Button m_ClinetButton;
    [SerializeField] Button m_DisconnectButton;
    [SerializeField] Button m_CreatePlayerButton;
    
    [SerializeField] TMP_Text m_CodeText;
    [SerializeField] TMP_InputField m_CodeInput;
    /// <summary>
    /// Whether to show the default control HUD at runtime.
    /// </summary>
    public bool showGUI = true;

    private void Start()
    {
	    m_StartButton.onClick.AddListener(m_Manager.UnityLogin);
	    m_StartButton.onClick.AddListener(() => {LobbyWindow.SetActive(true);});
	    m_StartButton.onClick.AddListener(() => {StartWindow.SetActive(false);});
	    
	    m_HostButton.onClick.AddListener(() => {
		    int maxPlayers = 8;
		    m_Manager.StartRelayHost(maxPlayers);
	    });
	    m_HostButton.onClick.AddListener(() => {GameWindow.SetActive(true);});
	    m_HostButton.onClick.AddListener(() => {LobbyWindow.SetActive(false);});
	    
	    m_ClinetButton.onClick.AddListener(() =>
	    {
		    m_Manager.relayJoinCode = m_CodeInput.text;
	    });
	    m_ClinetButton.onClick.AddListener(m_Manager.JoinRelayServer);
	    m_ClinetButton.onClick.AddListener(() => {GameWindow.SetActive(true);});
	    m_ClinetButton.onClick.AddListener(() => {LobbyWindow.SetActive(false);});
	    
	    m_DisconnectButton.onClick.AddListener(m_Manager.StopHost);
	    m_DisconnectButton.onClick.AddListener(() => {LobbyWindow.SetActive(true);});
	    m_DisconnectButton.onClick.AddListener(() => {GameWindow.SetActive(false);});
	    
	    m_CreatePlayerButton.onClick.AddListener(() => {m_Manager.CreatePlayer();});
    }
    
    void Update()
    {
	    m_CodeText.text = "code: " + m_Manager.relayJoinCode;
    }
    
    void OnGUI()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        foreach (string arg in args)
        {
            if (arg == "-server")
            {
	            m_Manager.StartServer();
                showGUI = false;
            }
        }

        if (!showGUI)
            return;

        GUILayout.BeginArea(new Rect(10, 40, 215, 9999));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();

            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }

        }

        StopButtons();

        GUILayout.EndArea();
    }

		void StartButtons()
		{
			if (!NetworkClient.active)
			{
				// Server Only
				if (Application.platform == RuntimePlatform.WebGLPlayer)
				{
					// cant be a server in webgl build
					GUILayout.Box("(  WebGL cannot be server  )");
				}
				else
				{
					//if (GUILayout.Button("Server Only")) m_Manager.StartStandardServer();
				}

				if (m_Manager.isLoggedIn)
				{
					// Server + Client
					if (Application.platform != RuntimePlatform.WebGLPlayer)
					{
						// if (GUILayout.Button("Standard Host (Server + Client)"))
						// {
						// 	m_Manager.StartStandardHost();
						// }

						if (GUILayout.Button("Relay Host (Server + Client)"))
						{
							int maxPlayers = 8;
							m_Manager.StartRelayHost(maxPlayers);
						}
					}

					// // Client + IP
					// GUILayout.BeginHorizontal();
					// if (GUILayout.Button("Client (without Relay)"))
					// {
					// 	m_Manager.JoinStandardServer();
					// }
					// m_Manager.networkAddress = GUILayout.TextField(m_Manager.networkAddress);
					// GUILayout.EndHorizontal();

					// Client + Relay Join Code
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("Client (with Relay)"))
					{
						m_Manager.JoinRelayServer();
					}
					//m_Manager.relayJoinCode = GUILayout.TextField(m_Manager.relayJoinCode);
					GUILayout.EndHorizontal();

					// if (GUILayout.Button("Get Relay Regions"))
					// {
					// 	// Note: We are not doing anything with these regions in this example, we are just illustrating how you would go about fetching these regions
					// 	m_Manager.GetRelayRegions((List<Region> regions) =>
					// 	{
					// 		if (regions.Count > 0)
					// 		{
					// 			for (int i = 0; i < regions.Count; i++)
					// 			{
					// 				Region region = regions[i];
					// 				Debug.Log("Found region. ID: " + region.Id + ", Name: " + region.Description);
					// 			}
					// 		}
					// 		else
					// 		{
					// 			Debug.LogWarning("No regions received");
					// 		}
					// 	},
					//
					// 	() =>
					// 	{
					// 		Debug.LogError("Failed to retrieve the list of Relay regions.");
					// 	});
					// }
				}
				else
				{
					if (GUILayout.Button("Auth Login"))
					{
						m_Manager.UnityLogin();
					}
				}
			}
			else
			{
				// Connecting
				GUILayout.Label("Connecting to " + m_Manager.networkAddress + "..");
				if (GUILayout.Button("Cancel Connection Attempt"))
				{
					m_Manager.StopClient();
				}
			}
		}

		void StatusLabels()
		{
			// server / client status message
			if (NetworkServer.active)
			{
				GUILayout.Label("Server: active. Transport: " + Transport.active);
				if (m_Manager.IsRelayEnabled())
				{
					GUILayout.Label("Relay enabled. Join code: " + m_Manager.relayJoinCode);
				}
			}
			if (NetworkClient.isConnected)
			{
				GUILayout.Label("Client: address=" + m_Manager.networkAddress);
			}
		}

		void StopButtons()
		{
			// stop host if host mode
			if (NetworkServer.active && NetworkClient.isConnected)
			{
				if (GUILayout.Button("Stop Host"))
				{
					m_Manager.StopHost();
				}
			}
			// stop client if client-only
			else if (NetworkClient.isConnected)
			{
				if (GUILayout.Button("Stop Client"))
				{
					m_Manager.StopClient();
				}
			}
			// stop server if server-only
			else if (NetworkServer.active)
			{
				if (GUILayout.Button("Stop Server"))
				{
					m_Manager.StopServer();
				}
			}
		}
}
