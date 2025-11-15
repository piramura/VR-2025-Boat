using UnityEngine;
using UnityEngine.InputSystem;   // ← 新Input System

namespace VRBoatProject.Model
{
    public class GameManagerTest : MonoBehaviour
    {
        private OscManager osc;

        void Start()
        {
            osc = FindAnyObjectByType<OscManager>();
        }

        void Update()
        {
            // 新Input Systemでのキーチェック
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                var s = osc.SensorData;

                osc.SendPoseData(s.roll, s.yaw, s.pitch);
                Debug.Log("Sent pose data");
            }
        }
    }
}