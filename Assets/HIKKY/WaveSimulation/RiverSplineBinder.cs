using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RiverSplineBinder : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float riverWidth = 5f;

    private Mesh mesh;

    // デバッグ用に最後に処理した頂点情報を保持
    private Vector3[] debugVerts;
    private Vector3[] debugNormals;

    void Start()
    {
        BindSplineData();
    }

    void BindSplineData()
    {
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer is null!");
            return;
        }

        Spline spline = splineContainer.Spline;
        int knotCount = spline.Knots.Count();
        int div = 100;
        int vCount = knotCount * div;

        Vector3[] verts = new Vector3[vCount];
        Vector3[] normals = new Vector3[vCount];
        Vector4[] tangents = new Vector4[vCount];
        Vector2[] uv2 = new Vector2[vCount];

        debugVerts = new Vector3[vCount];
        debugNormals = new Vector3[vCount];

        Vector3 right = Vector3.right;

        for (int i = 0; i < vCount; i++)
        {
            int segmentIndex = i / div;
            int stepInSegment = i % div;
            float t = (segmentIndex + stepInSegment / (float)(div - 1)) / knotCount;

            // スプライン上の中心点と接線
            Vector3 center = spline.EvaluatePosition(t);
            float3 tangentF3 = spline.EvaluateTangent(t);
            Vector3 tangent = new Vector3(tangentF3.x, tangentF3.y, tangentF3.z).normalized;

            // スプラインに垂直な右方向
            right = Vector3.Cross(Vector3.up, tangent).normalized;

            // ローカル座標に変換
            verts[i] = center;

            // Tangent にスプライン接線を設定
            tangents[i] = new Vector4(tangent.x, tangent.y, tangent.z, 1f);

            // UV2 に t を設定（Shader の流れ計算用）
            uv2[i] = new Vector2(t, 0);

            // 法線を補正（川面がスプライン平面に沿う）
            normals[i] = Vector3.Cross(right, tangent).normalized;
            
            // デバッグ用に情報を保存
            debugVerts[i] = verts[i];
            debugNormals[i] = normals[i];

            // 少数点付きでログ出力（頂点ごとに確認）
            if (i < div) // 上位divだけ表示
                Debug.Log($"Vertex[{i}] t={t:F3}, Pos={verts[i]}, Normal={normals[i]}, Right={right}");
        }
        mesh = GetComponent<MeshFilter>().mesh;
        int vPerSide = vCount;
        int totalV = vCount * 2;

        Vector3[] verts2 = new Vector3[totalV];
        Vector3[] normals2 = new Vector3[totalV];
        Vector4[] tangents2 = new Vector4[totalV];
        Vector2[] uv2_2 = new Vector2[totalV];

        // 左右作成（左=-riverWidth/2, 右=+riverWidth/2）
        for (int i=0;i<vCount;i++)
        {
            // 左
            verts2[i]     = verts[i] - right * (riverWidth * 0.5f);
            normals2[i]   = normals[i];
            tangents2[i]  = tangents[i];
            uv2_2[i]      = uv2[i];

            // 右
            verts2[i+vPerSide] = verts[i] + right * (riverWidth * 0.5f);
            normals2[i+vPerSide] = normals[i];
            tangents2[i+vPerSide] = tangents[i];
            uv2_2[i+vPerSide] = uv2[i];
        }

        // 三角形
        int triCount = (vPerSide - 1) * 2;
        int[] tris = new int[triCount * 3];

        int tIndex = 0;
        for (int i=0; i<vPerSide-1; i++)
        {
            int a = i;
            int b = i + 1;
            int c = i + vPerSide;
            int d = i + vPerSide + 1;

            tris[tIndex++] = a;
            tris[tIndex++] = b;
            tris[tIndex++] = c;

            tris[tIndex++] = b;
            tris[tIndex++] = d;
            tris[tIndex++] = c;
        }

        mesh.Clear();
        mesh.vertices = verts2;
        mesh.normals = normals2;
        mesh.tangents = tangents2;
        mesh.uv2 = uv2_2;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
    }
}
