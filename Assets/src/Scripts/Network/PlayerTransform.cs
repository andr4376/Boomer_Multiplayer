using UnityEngine;
using Unity.Netcode;

public class PlayerTransform : NetworkBehaviour
{
    public float lerpSpeed = 10f;

    // Define a struct to hold position and rotation data
    public struct TransformData : INetworkSerializable
    {
        public Vector3 Position;
        public float RotationY;

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
            // If this is the owner, update the NetworkVariable with the current transform
            networkTransform.Value = new TransformData
            {
                Position = transform.position,
                RotationY = transform.rotation.eulerAngles.y
            };
        }
        else
        {
            // If this is not the owner, interpolate the transform with the NetworkVariable
            transform.position = Vector3.Lerp(transform.position, networkTransform.Value.Position, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, networkTransform.Value.RotationY, 0), Time.deltaTime * lerpSpeed);
        }
    }
}
