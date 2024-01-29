using Unity.Netcode;
using UnityEngine;

public class PlayerInit : NetworkBehaviour
{
    public GameObject playerGraphics;

    public MonoBehaviour[] scriptsToEnableifOwner;

    public SkinnedMeshRenderer[] meshRenderersToDisable;

    public override void OnNetworkSpawn()
    {
        if (IsOwner == false) //not my player
        {
            return;
        }
        foreach (var item in scriptsToEnableifOwner)
        {
            item.enabled = true;
        }
        //playerGraphics.SetActive(false);

        foreach(var item in meshRenderersToDisable) { item.enabled = false; }
        //foreach (var item in playerGraphics.GetComponentsInChildren<MeshRenderer>())
        //{
        //    item.enabled=false;
        //}
        //foreach (var item in playerGraphics.GetComponentsInChildren<SkinnedMeshRenderer>())
        //{
        //    item.enabled = false;
        //}

        GetComponent<CharacterController>().enabled = true;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
    }
}
