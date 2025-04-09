using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    [SerializeField] NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;

    private void Awake()
    {
        networkRunner = FindAnyObjectByType<NetworkRunner>();
    }

    void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";
        }

        // To Change to client Host Start here
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", NetAddress.Any(), SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex), null);

        Utils.DebugLog("InitializeNetworkRunner Called");
    
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        INetworkSceneManager sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        
        if (sceneManager == null)
        {
            //Handle netowrked objects that already exists in the scene
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner
        (NetworkRunner networkRunner, GameMode gameMode, string sessionName, 
        NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        INetworkSceneManager sceneManager = GetSceneManager(networkRunner);

        networkRunner.ProvideInput = true;

        return networkRunner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            SceneManager = sceneManager,
        });
    }

}
