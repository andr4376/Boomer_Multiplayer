using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Handles syncing and transmitting player movement (client side)
/// </summary>
public class PlayerTransform : NetworkBehaviour
{
    public float lerpSpeed = 10f;

    // Define a struct to hold position and rotation data
    public struct TransformData : INetworkSerializable
    {
        public Vector3 Position;
        public short RotationY;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref RotationY);
        }
    }

    // Create a NetworkVariable to hold the transform data
    private NetworkVariable<TransformData> networkTransform;

    private void Awake()
    {
        var permission = NetworkVariableWritePermission.Owner;
        networkTransform = new NetworkVariable<TransformData>(writePerm: permission);
    }

    private void Update()
    {
        if (IsOwner)
        {
            TransmitNetworkTransform();
            return;
        }
        else
        {
            SyncPositionFromServerNetworkTransform();
            return;
        }
    }

    private void SyncPositionFromServerNetworkTransform()
    {
        // If this is not the owner, interpolate the transform with the NetworkVariable
        transform.position = Vector3.Lerp(transform.position, networkTransform.Value.Position, Time.deltaTime * lerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, networkTransform.Value.RotationY, 0), Time.deltaTime * lerpSpeed);
    }

    private void TransmitNetworkTransform()
    {
        var newNetworkState = new TransformData
        {
            Position = transform.position,
            RotationY = (short)transform.rotation.eulerAngles.y
        };

        networkTransform.Value = newNetworkState;
    }

}
