using UnityEngine;
using UnityEngine.UI;
using VRBoatProject.Model;

public class OscDebugPanel : MonoBehaviour
{
    [Header("OSC Manager 参照")]
    [SerializeField] private OscManager oscManager;

    [Header("表示用UI Text")]
    [SerializeField] private Text rollText;
    [SerializeField] private Text pitchText;
    [SerializeField] private Text yawText;
    [SerializeField] private Text loadLeftText;
    [SerializeField] private Text loadRightText;
    [SerializeField] private Text balanceText;

    void Update()
    {
        if (oscManager == null) return;
        var data = oscManager.SensorData;

        // 値をテキストに反映
        rollText.text = $"Roll : {data.roll:F2}";
        pitchText.text = $"Pitch: {data.pitch:F2}";
        yawText.text = $"Yaw  : {data.yaw:F2}";
        loadLeftText.text = $"Load L: {data.loadLeft:F2}";
        loadRightText.text = $"Load R: {data.loadRight:F2}";
        balanceText.text = $"Balance: {data.loadLeft - data.loadRight:F2}";
    }
}
