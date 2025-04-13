using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkRunnerHandler : MonoBehaviour
{
    public static NetworkRunnerHandler Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] GameObject roomSelectionUI;
    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] GameObject roomListPanel;
    [SerializeField] GameObject roomListItemPrefab;

    [Header("Network References")]
    [SerializeField] NetworkRunner networkRunnerPrefab;

    private NetworkRunner networkRunner;
    private List<SessionInfo> sessionList = new List<SessionInfo>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";
        }
        roomSelectionUI.SetActive(true);
    }

    public async void OnCreateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room_" + UnityEngine.Random.Range(1000, 9999);
        }

        var result = await InitializeNetworkRunner(
            GameMode.Host,
            roomName
        );

        if (result.Ok)
        {
            roomSelectionUI.SetActive(false);
        }
    }

    public async void OnJoinRoom(SessionInfo sessionInfo)
    {
        var result = await InitializeNetworkRunner(
            GameMode.Client,
            sessionInfo.Name
        );

        if (result.Ok)
        {
            roomSelectionUI.SetActive(false);
        }
    }

    public async void OnRefreshRoomList()
    {
        ClearRoomList();
        if (networkRunner == null) return;

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, "OurLobbyID");
        if (!result.Ok)
        {
            Debug.LogError($"Failed to join lobby: {result.ErrorMessage}");
        }
    }

    private void ClearRoomList()
    {
        foreach (Transform child in roomListPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private async Task<StartGameResult> InitializeNetworkRunner(
        GameMode gameMode,
        string sessionName)
    {
        // Get or create the Spawner component
        var spawner = networkRunner.GetComponent<Spawner>();
        if (spawner == null)
        {
            spawner = networkRunner.gameObject.AddComponent<Spawner>();
        }

        networkRunner.ProvideInput = true;
        networkRunner.AddCallbacks(spawner);

        return await networkRunner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 8
        });
    }

    public void HandleSessionListUpdated(List<SessionInfo> sessions)
    {
        sessionList = sessions.Where(s => s.IsVisible && s.PlayerCount < 8).ToList();
        ClearRoomList();

        foreach (var session in sessionList)
        {
            var listItem = Instantiate(roomListItemPrefab, roomListPanel.transform);
            if (listItem.TryGetComponent<RoomListItem>(out var item))
            {
                item.Setup(session, OnJoinRoom);
            }
        }
    }
}