using Unity.Netcode;

public class NetworkDisableIfNotOwner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            gameObject.SetActive(false);
    }

}
