using UnityEngine;

[RequireComponent(typeof(GerstnerWaveCalculator))]
public class BoatController : MonoBehaviour
{
    public Transform boat;
    public float offsetY = 0f;
    public float rotationFactor = 10f; // ピッチ・ロールの感度

    private GerstnerWaveCalculator wave;

    void Start()
    {
        wave = GetComponent<GerstnerWaveCalculator>();
    }

    void Update()
    {
        Vector2 posXZ = new Vector2(boat.position.x, boat.position.z);
        float height = wave.GetHeight(posXZ);
        Vector3 normal = wave.GetNormal(posXZ);

        // 高さを反映
        boat.position = new Vector3(boat.position.x, height + offsetY, boat.position.z);

        // 傾きを反映
        Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, normal);
        boat.rotation = Quaternion.Slerp(boat.rotation, targetRot, Time.deltaTime * rotationFactor);
    }
}