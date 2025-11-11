using UnityEngine;
using OscJack;

namespace VRBoatProject.Model
{
    [System.Serializable]
    public class SensorDataModel
    {
        public float roll;       // 姿勢：X回転
        public float pitch;      // 姿勢：Y回転
        public float yaw;        // 姿勢：Z回転
        public float loadLeft;   // 左踏ん張り入力
        public float loadRight;  // 右踏ん張り入力

        public void Update(float r, float p, float y, float l, float rgt)
        {
            roll = r; pitch = p; yaw = y;
            loadLeft = l; loadRight = rgt;
        }
        /// <summary>
        /// 左右の平均的な踏ん張り度合いを返す（例）
        /// </summary>
        public float GetBalance()
        {
            return (loadLeft - loadRight);
        }
    }
    public class OscManager : MonoBehaviour
    {
        [Header("OSC Settings")]
        [SerializeField] private string espIp = "192.168.1.50";
        [SerializeField] private int sendPort = 8000;
        [SerializeField] private int receivePort = 9000;
        
        [Header("Target Object")] 
        [SerializeField] private Transform targetObject;

        private OscClient oscOut;
        private OscServer oscIn;
        
        // 他クラスから読み取るためのデータ保持
        public SensorDataModel SensorData { get; private set; } = new SensorDataModel();
        void Start()
        {
            oscOut = new OscClient(espIp, sendPort);
            oscIn = new OscServer(receivePort);
            // BNO055 + ロードセル データ受信
            oscIn.MessageDispatcher.AddCallback("/sensor", OnSensorReceived);
            
        }
        /// <summary>
        /// X回転・Z回転・Y座標をESPに送信（GameManagerから呼ばれる）
        /// </summary>
        public void SendPoseData(float xRot, float zRot, float yPos)
        {
            oscOut.Send("/pose", xRot, zRot, yPos);
        }
        /// <summary>
        /// センサーデータ受信（姿勢 + 左右入力）
        /// </summary>
        private void OnSensorReceived(string address, OscDataHandle data)
        {
            float roll = data.GetElementAsFloat(0);
            float pitch = data.GetElementAsFloat(1);
            float yaw = data.GetElementAsFloat(2);
            float loadL = data.GetElementAsFloat(3);
            float loadR = data.GetElementAsFloat(4);

            // 内部モデルを更新
            SensorData.Update(roll, pitch, yaw, loadL, loadR);
            Debug.Log($"Sensor: Roll={roll:F2}, Pitch={pitch:F2}, Yaw={yaw:F2}");
        }
        /// <summary>
        /// 必要に応じてGameManagerから呼べる：通信終了処理
        /// </summary>
        public void Close()
        {
            oscOut.Dispose();
            oscIn.Dispose();
        }
    }
    
    
}