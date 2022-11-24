using UnityEngine;
using Unity.Networking.Transport;

public class NetworkBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connections;

    public int state;

    public virtual void SendInt(int message)
    {

    }
}
