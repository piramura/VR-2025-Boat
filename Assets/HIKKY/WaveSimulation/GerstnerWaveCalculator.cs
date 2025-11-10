using UnityEngine;

[System.Serializable]
public class GerstnerWave
{
    public bool active = true;
    public float amplitude = 0.1f;
    public float wavelength = 3f;
    public float speed = 0.2f;
    [Range(0, 1)] public float qRatio = 0.25f;
    public Vector2 direction = new Vector2(1, 0);

    private float k;   // 波数 (2π/L)
    private float w;   // 角速度 (k * speed)
    private float phi; // φ = speed * k

    public void Initialize()
    {
        k = 2f * Mathf.PI / wavelength;
        w = k * speed;
        phi = speed * k;
        direction.Normalize();
    }

    /// <summary>
    /// Shaderと同一のGerstner波式
    /// </summary>
    public Vector3 GetDisplacement(Vector2 xy, float time)
    {
        if (!active) return Vector3.zero;

        float theta = k * Vector2.Dot(direction, xy) + phi * time;
        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float Q = (1f / (w * amplitude)) * qRatio;

        // Shaderの P() と同じ順番
        return new Vector3(
            Q * amplitude * direction.x * cosTheta,
            amplitude * sinTheta,
            Q * amplitude * direction.y * cosTheta
        );
    }
}

public class GerstnerWaveCalculator : MonoBehaviour
{
    public GerstnerWave[] waves;

    void Start()
    {
        foreach (var w in waves) w.Initialize();
    }

    /// <summary> 指定座標(x,z)と時間tでの高さを返す </summary>
    public float GetHeight(Vector2 position)
    {
        float time = Time.time;
        Vector3 total = Vector3.zero;
        foreach (var w in waves)
            total += w.GetDisplacement(position, time);
        return total.y;
    }

    /// <summary> 法線ベクトル（傾き）を求める </summary>
    public Vector3 GetNormal(Vector2 position)
    {
        // 近傍2点で高さをサンプリングして法線を近似
        float delta = 0.1f;
        float hL = GetHeight(position + Vector2.left * delta);
        float hR = GetHeight(position + Vector2.right * delta);
        float hF = GetHeight(position + Vector2.up * delta);
        float hB = GetHeight(position + Vector2.down * delta);

        Vector3 dx = new Vector3(2 * delta, hR - hL, 0);
        Vector3 dz = new Vector3(0, hF - hB, 2 * delta);
        return Vector3.Cross(dz, dx).normalized;
    }
}
