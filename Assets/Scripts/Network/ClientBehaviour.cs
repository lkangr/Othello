using UnityEngine;
using Unity.Networking.Transport;

public class ClientBehaviour : NetworkBehaviour
{

    private NetworkEndPoint endpoint;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connections = default(NetworkConnection);

        state = (int)OnlineState.WAITCONNECT;

        endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        m_Connections = m_Driver.Connect(endpoint);
    }

    private void OnDestroy()
    {
        if (m_Driver.IsCreated)
        {
            m_Driver.Dispose();
        }
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connections.IsCreated)
        {
            m_Connections = m_Driver.Connect(endpoint);
        }

        if (m_Connections.IsCreated) {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = m_Connections.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("We are now connected to the server");

                    state = (int)OnlineState.WHITE;
                    //m_Driver.BeginSend(m_Connections, out var writter);
                    //writter.WriteUInt(value);
                    //m_Driver.EndSend(writter);
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    int value = stream.ReadInt();
                    Debug.Log("Got the value = " + value + " from the server");
                    state = value;
                    //m_Connections.Disconnect(m_Driver);
                    //m_Connections = default(NetworkConnection);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    m_Connections = default(NetworkConnection);
                    state = (int)OnlineState.DISCONNECT;
                }
            }
        }
    }

    public override void SendInt(int message)
    {
        if (m_Driver.IsCreated && m_Connections.IsCreated)
        {
            m_Driver.BeginSend(m_Connections, out var writer);
            writer.WriteInt(message);
            m_Driver.EndSend(writer);
        }
        else
            Debug.LogError("No Connection");
    }
}
