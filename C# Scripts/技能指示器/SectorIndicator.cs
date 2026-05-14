using System.Collections.Generic;
using UnityEngine;

public class SectorIndicator : MonoBehaviour
{
    public Material m_Material; // 扇形填充材质
    [Range(1, 360)] public float angle = 90f;
    [Range(0, 20)] public float radius = 5f;
    [Range(1, 60)] public int quality = 12;
    
    [Header("边界线设置")]
    [Range(0.01f, 0.3f)] public float borderWidth = 0.08f;
    public Color borderColor = Color.white;

    // ====== 资源复用（关键！避免内存泄漏）======
    private GameObject sectorGo;
    private MeshFilter sectorMeshFilter;
    private MeshRenderer sectorMeshRenderer;
    
    private Material borderMaterial; // 全局唯一材质
    private GameObject borderStart;  // 复用对象
    private GameObject borderEnd;    // 复用对象
    private Mesh borderMeshStart;    // 复用Mesh
    private Mesh borderMeshEnd;      // 复用Mesh

    private void Start()
    {
        // ===== 1. 创建扇形容器 =====
        sectorGo = new GameObject("Sector");
        sectorGo.transform.SetParent(transform, false);
        sectorGo.SetActive(false);
        
        sectorMeshFilter = sectorGo.AddComponent<MeshFilter>();
        sectorMeshRenderer = sectorGo.AddComponent<MeshRenderer>();

        // ===== 2. 初始化边界资源（仅创建1次！）=====
        // 创建共享材质（关闭剔除 + 高渲染队列）
        borderMaterial = new Material(Shader.Find("Unlit/Color"))
        {
            color = borderColor,
            renderQueue = 3000
        };
        borderMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        // 创建起始边界对象
        borderStart = new GameObject("BorderStart");
        borderStart.transform.SetParent(sectorGo.transform, false);
        borderStart.AddComponent<MeshFilter>();
        borderStart.AddComponent<MeshRenderer>().material = borderMaterial;
        borderMeshStart = new Mesh(); // 创建1次
        borderStart.GetComponent<MeshFilter>().mesh = borderMeshStart;
        borderStart.SetActive(false);

        // 创建结束边界对象
        borderEnd = new GameObject("BorderEnd");
        borderEnd.transform.SetParent(sectorGo.transform, false);
        borderEnd.AddComponent<MeshFilter>();
        borderEnd.AddComponent<MeshRenderer>().material = borderMaterial;
        borderMeshEnd = new Mesh(); // 创建1次
        borderEnd.GetComponent<MeshFilter>().mesh = borderMeshEnd;
        borderEnd.SetActive(false);
    }

    private void Update()
    {
        // if (Input.GetMouseButtonDown(1))
        // {
        //     GenerateSector();
        //     sectorGo.SetActive(true);
        // }
        //
        // if (Input.GetMouseButtonDown(0))
        // {           
        //     sectorGo.SetActive(true);
        // }
        
        transform.localPosition = new Vector3(0, -PlayerControlModule.Instance.transform.localPosition.y, 0);
    }

    public void SectorDisplay(float angle, float radius)
    {
        this.angle = angle;
        this.radius = radius;
        
        GenerateSector();
        sectorGo.SetActive(true);
    }

    public void SectorHide()
    {
        sectorGo.SetActive(false);
    }

    private void GenerateSector()
    {
        // ===== 1. 生成扇形填充Mesh =====
        List<Vector3> vertices = new List<Vector3> { Vector3.zero };
        float eachAngle = angle / quality;
        
        for (int i = 0; i <= quality; i++)
        {
            float currentAngle = -angle / 2f + eachAngle * i;
            Vector3 vertex = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * radius;
            vertices.Add(vertex);
        }

        int triangleCount = quality;
        int[] triangles = new int[triangleCount * 3];
        for (int i = 0; i < triangleCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // 修复UV
        Vector2[] uvs = new Vector2[vertices.Count];
        uvs[0] = new Vector2(0.5f, 0.5f);
        for (int i = 1; i < uvs.Length; i++)
        {
            float u = (vertices[i].x + radius) / (2f * radius);
            float v = (vertices[i].z + radius) / (2f * radius);
            uvs[i] = new Vector2(u, v);
        }

        Mesh fillMesh = new Mesh();
        fillMesh.vertices = vertices.ToArray();
        fillMesh.triangles = triangles;
        fillMesh.uv = uvs;
        fillMesh.RecalculateNormals();
        fillMesh.RecalculateBounds();
        
        sectorMeshFilter.mesh = fillMesh;
        sectorMeshRenderer.material = m_Material;

        // ===== 2. 更新边界（复用资源！零GC）=====
        UpdateBorder(borderMeshStart, -angle / 2f);
        UpdateBorder(borderMeshEnd, angle / 2f);
        
        borderStart.SetActive(true);
        borderEnd.SetActive(true);
    }

    // 核心：仅更新Mesh数据，不创建新对象
    private void UpdateBorder(Mesh mesh, float angleDeg)
    {
        if (angle == 360)
        {
            mesh.Clear();
            return;
        }
        
        // 计算方向
        Vector3 direction = (Quaternion.Euler(0, angleDeg, 0) * Vector3.forward.normalized);
        Vector3 rightDir = Vector3.Cross(Vector3.up, direction).normalized;
        
        // 4个顶点（逆时针顺序，确保法线朝上）
        Vector3[] verts = new Vector3[]
        {
            Vector3.zero + rightDir * (borderWidth * 0.5f), // 0: 中心右侧
            direction * radius * 0.92f + rightDir * (borderWidth * 0.5f), // 1: 边缘右侧
            direction * radius * 0.92f - rightDir * (borderWidth * 0.5f), // 2: 边缘左侧
            Vector3.zero - rightDir * (borderWidth * 0.5f)  // 3: 中心左侧
        };
        
        // 2个三角形（逆时针）
        int[] tris = { 0, 2, 1, 0, 3, 2 };
        
        // ✅ 关键：复用Mesh（Clear后填入新数据）
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    // 清理资源（场景切换时调用）
    private void OnDestroy()
    {
        if (borderMaterial != null) Destroy(borderMaterial);
        if (borderMeshStart != null) Destroy(borderMeshStart);
        if (borderMeshEnd != null) Destroy(borderMeshEnd);
        if (sectorGo != null) Destroy(sectorGo);
    }
}