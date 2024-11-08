using Mirror;
using UnityEngine;

public class MultiplayerMenuManager : MonoBehaviour
{
    public void OpenSession()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            NetworkManager.singleton.StartHost(); // Memulai sebagai Host (Server + Client)
            Debug.Log("Opening session and starting host...");
        }
        else
        {
            Debug.LogWarning("Already connected to a session!");
        }
    }

    public void JoinSession()
    {
        if (!NetworkClient.isConnected)
        {
            NetworkManager.singleton.StartClient(); // Bergabung sebagai Client
            Debug.Log("Joining session...");
        }
        else
        {
            Debug.LogWarning("Already connected to a session!");
        }
    }
}
