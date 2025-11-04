using UnityEngine;
using OscJack;

public class OscJackServerSample : MonoBehaviour
{
    [SerializeField] int port = 8000;
    OscServer server;

    void OnEnable()
    {
        server = new OscServer(port);
        server.MessageDispatcher.AddCallback(
            "/OscJack/sample",
            (string address, OscDataHandle data) => {
                var stringValue = data.GetElementAsString(0);
                var floatValue = data.GetElementAsFloat(1);
                var intValue = data.GetElementAsInt(2);
                Debug.Log($"OscJack receive: {address} {stringValue} {floatValue} {intValue}");
            }
        );
    }

    void OnDisable()
    {
        server.Dispose();
        server = null;
    }
}