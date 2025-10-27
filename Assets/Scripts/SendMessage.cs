using System.IO.Ports;
using UnityEngine;

public class SerialSender : MonoBehaviour {
    SerialPort port;

    void Start() {
        port = new SerialPort("/dev/cu.usbserial-110", 115200); // Macの場合
        port.Open();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            port.WriteLine("LED_ON");
        }
    }
}