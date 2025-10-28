using UnityEngine;
using OscJack;

public class EspReceiver : MonoBehaviour
{
    OscServer server;

    void Start()
    {
        server = new OscServer(9000); // ESP側と同じポート番号
        server.MessageDispatcher.AddCallback("/esp/value", OnValueReceived);
    }

    void OnValueReceived(string address, OscDataHandle data)
    {
        int value = data.GetElementAsInt(0);
        Debug.Log($"ESP Value: {value}");

        // 例: Unity上で何かのオブジェクトを点灯
        GameObject cube = GameObject.Find("Cube");
        if (cube)
        {
            cube.GetComponent<Renderer>().material.color =
                (value == 1 ? Color.green : Color.red);
        }
    }

    void OnDestroy()
    {
        server?.Dispose();
    }
}