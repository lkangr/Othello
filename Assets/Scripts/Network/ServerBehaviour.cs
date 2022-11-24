using UnityEngine;
using Unity.Networking.Transport;

public class ServerBehaviour : NetworkBehaviour
{
    void Start()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 9000;
        state = (int)OnlineState.WAITCONNECT;

        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port 9000");
        }
        else m_Driver.Listen();
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

        if (timeValidate >= 0 && Time.realtimeSinceStartup - timeValidate > 2)
        {
            state = (int)OnlineState.DISCONNECT;
        }

        if (!m_Connections.IsCreated)
        {
            // Accept new connections
            NetworkConnection c;
            if ((c = m_Driver.Accept()) != default(NetworkConnection))
            {
                m_Connections = c;
                Debug.Log("Accepted a connection");
                timeValidate = Time.realtimeSinceStartup;
                state = (int)OnlineState.BLACK;
            }
        }

        if (m_Connections.IsCreated)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    int number = stream.ReadInt();
                    Debug.Log("Got " + number + " from the Client");
                    if (number == (int)OnlineState.VALIDATE)
                    {
                        timeValidate = Time.realtimeSinceStartup;
                        SendInt((int)OnlineState.VALIDATE);
                    }
                    else state = number;
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    state = (int)OnlineState.DISCONNECT;
                    Debug.Log("Client disconnected from server");
                    m_Connections = default(NetworkConnection);
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
