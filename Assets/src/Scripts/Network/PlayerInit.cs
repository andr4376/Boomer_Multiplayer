using Unity.Netcode;
using UnityEngine;

public class PlayerInit : NetworkBehaviour
{
    public GameObject playerGraphics;

    public MonoBehaviour[] scriptsToDisableifNotOwner;
    public override void OnNetworkSpawn()
    {

        if (IsOwner == false) //not my player
        {
            foreach (var item in scriptsToDisableifNotOwner)
            {
                item.enabled = false;
            }
        }
        else //my player
        {
            playerGraphics.SetActive(false);
        }

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
    }
}
