using Unity.Netcode;

namespace Network
{
    public class CameraFollow : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) gameObject.SetActive(false);
        }
    }
}
