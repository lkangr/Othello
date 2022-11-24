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

        if (timeValidate >= 0 && Time.realtimeSinceStartup - timeValidate > 2)
        {
            state = (int)OnlineState.DISCONNECT;
        }

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

                    timeValidate = Time.realtimeSinceStartup;
                    InvokeRepeating("ValidateConnection", 1, 1);

                    state = (int)OnlineState.WHITE;
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    int value = stream.ReadInt();
                    Debug.Log("Got the value = " + value + " from the server");
                    if (value == (int)OnlineState.VALIDATE)
                    {
                        timeValidate = Time.realtimeSinceStartup;
                    }
                    else state = value;
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

    private void ValidateConnection()
    {
        SendInt((int)OnlineState.VALIDATE);
    }
}
