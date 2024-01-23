using Unity.Netcode;
using UnityEngine;

public class PlayerInit : NetworkBehaviour
{
    public GameObject playerGraphics;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        var playerCam = Camera.main.gameObject.AddComponent<PlayerCamera>();

        playerCam.player = this.transform;

        playerCam.offset = new Vector3(0, 2, 0);

        playerGraphics.SetActive(false);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
       
        Destroy(Camera.main.gameObject.GetComponent<PlayerCamera>());
    }
}
