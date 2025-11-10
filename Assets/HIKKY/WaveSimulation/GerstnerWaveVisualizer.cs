using UnityEngine;

[ExecuteAlways]
public class GerstnerWaveVisualizer : MonoBehaviour
{
    public GerstnerWaveCalculator wave;
    public int resolution = 100;
    public float range = 10f;
    public Color color = Color.cyan;

    void OnDrawGizmos()
    {
        if (wave == null) return;
        Gizmos.color = color;

        Vector3 prev = Vector3.zero;
        for (int i = 0; i < resolution; i++)
        {
            float x = (i / (float)(resolution - 1)) * range - range / 2f;
            float z = 0;
            float y = wave.GetHeight(new Vector2(x, z));
            Vector3 pos = new Vector3(x, y, z);
            if (i > 0)
                Gizmos.DrawLine(prev + transform.position, pos + transform.position);
            prev = pos;
        }
    }
}