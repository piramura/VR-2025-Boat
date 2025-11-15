// csharp
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
public class GerstnerWaveSync : MonoBehaviour
{
    public GerstnerWaveCalculator calculator;
    public bool continuousSync = false;

    private Renderer _renderer;
    private Material _material;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_renderer == null) return;

        if (Application.isPlaying)
        {
            _material = _renderer.material;
        }
        else
        {
            _material = _renderer.sharedMaterial;
        }
        SyncFromMaterial();
    }

    void Update()
    {
        if (continuousSync)
            SyncFromMaterial();
    }

    void OnValidate()
    {
        // エディタでプロパティを変えたときに即同期したい場合
        if (!Application.isPlaying && _renderer == null) _renderer = GetComponent<Renderer>();
        if (!Application.isPlaying && _renderer != null) _material = _renderer.sharedMaterial;
        if (!Application.isPlaying && _material != null && calculator != null) SyncFromMaterial();
    }

    void SyncFromMaterial()
    {
        if (calculator == null)
        {
            Debug.LogWarning("GerstnerWaveSync: calculator is null");
            return;
        }

        if (_material == null)
        {
            Debug.LogWarning("GerstnerWaveSync: material not found on renderer");
            return;
        }

        if (calculator.waves == null || calculator.waves.Length < 3)
        {
            Debug.LogWarning("GerstnerWaveSync: calculator.waves must contain at least 3 elements");
            return;
        }

        // Wave1
        calculator.waves[0].active = _material.GetFloat("_Active1") > 0.5f;
        calculator.waves[0].direction = new Vector2(_material.GetFloat("_Direction1X"), _material.GetFloat("_Direction1Z"));
        calculator.waves[0].amplitude = _material.GetFloat("_Amplitude1");
        calculator.waves[0].wavelength = _material.GetFloat("_WaveLength1");
        calculator.waves[0].speed = _material.GetFloat("_Speed1");
        calculator.waves[0].qRatio = _material.GetFloat("_QRatio1");

        // Wave2
        calculator.waves[1].active = _material.GetFloat("_Active2") > 0.5f;
        calculator.waves[1].direction = new Vector2(_material.GetFloat("_Direction2X"), _material.GetFloat("_Direction2Z"));
        calculator.waves[1].amplitude = _material.GetFloat("_Amplitude2");
        calculator.waves[1].wavelength = _material.GetFloat("_WaveLength2");
        calculator.waves[1].speed = _material.GetFloat("_Speed2");
        calculator.waves[1].qRatio = _material.GetFloat("_QRatio2");

        // Wave3
        calculator.waves[2].active = _material.GetFloat("_Active3") > 0.5f;
        calculator.waves[2].direction = new Vector2(_material.GetFloat("_Direction3X"), _material.GetFloat("_Direction3Z"));
        calculator.waves[2].amplitude = _material.GetFloat("_Amplitude3");
        calculator.waves[2].wavelength = _material.GetFloat("_WaveLength3");
        calculator.waves[2].speed = _material.GetFloat("_Speed3");
        calculator.waves[2].qRatio = _material.GetFloat("_QRatio3");

        // 初期化（内部 k,w, 正規化など）
        foreach (var w in calculator.waves) w.Initialize();
    }
}