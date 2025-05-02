using UnityEngine;
using Unity.Netcode;

public abstract class ManagerBase : NetworkBehaviour
{
    [SerializeField] protected ReadySystem readySystem;
    [SerializeField] protected GameObject gameUI;
    [SerializeField] protected GameObject endGameUI;
    
    protected bool gameActive = false;
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Subscribe to ready system
            readySystem.OnAllPlayersReady += StartGame;
        }
        
        // Initialize UI
        if (gameUI != null)
            gameUI.SetActive(false);
        if (endGameUI != null)
            endGameUI.SetActive(false);
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            readySystem.OnAllPlayersReady -= StartGame;
        }
    }
    
    protected virtual void StartGame()
    {
        gameActive = true;
        
        if (gameUI != null)
            gameUI.SetActive(true);
            
        StartGameRpc();
        
        // Minigame-specific initialization
        OnGameStart();
    }
    
    [Rpc(SendTo.NotServer)]
    private void StartGameRpc()
    {
        if (gameUI != null)
            gameUI.SetActive(true);
    }
    
    protected virtual void EndGame()
    {
        gameActive = false;
        
        if (gameUI != null)
            gameUI.SetActive(false);
        if (endGameUI != null)
            endGameUI.SetActive(true);
            
        // Minigame-specific cleanup
        OnGameEnd();
    }
    
    // Override these in your specific minigames
    protected abstract void OnGameStart();
    protected abstract void OnGameEnd();
    
    protected virtual void Update()
    {
        if (!IsServer) return;
        
        if (gameActive)
        {
            // Only run game logic when active
            GameUpdate();
        }
    }
    
    // Override this for minigame-specific update logic
    protected virtual void GameUpdate() { }
}
