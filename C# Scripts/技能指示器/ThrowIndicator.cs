using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThrowIndicator : MonoBehaviour
{
    [Header("角色与地面设置")]
    [SerializeField] private Transform character; // 技能释放起点（角色脚底）
    [SerializeField] private LayerMask groundLayer = 1 << 6; // 默认Ground层
    
    [Header("拖拽参数")]
    [SerializeField, Range(1f, 30f)] private float maxDragDistance = 15f; // 最大拖拽距离
    [SerializeField, Range(0.5f, 10f)] private float skillRadius = 3f;    // 技能实际影响范围
    
    [Header("视觉效果")]
    [SerializeField] private Color dragCircleColor = new Color(0, 0.7f, 1, 0.25f); // 拖拽范围（半透明蓝）
    [SerializeField] private Color skillCircleColor = new Color(1, 0.3f, 0.3f, 0.35f); // 技能范围（半透明红）
    [SerializeField] private float circleEdgeSharpness = 2.0f; // 边缘锐化（1=柔和，5=锐利）
    [SerializeField] private int circleSegments = 48; // 圆滑度（32-64足够）

    // ====== 资源复用（核心：全程无new！）======
    private GameObject dragCircleGo;   // 拖拽范围圆（内部大圆）
    private GameObject skillCircleGo;  // 技能范围圆（外部小圆）
    private Material dragMat;          // 拖拽圆材质（复用）
    private Material skillMat;         // 技能圆材质（复用）
    private Mesh circleMesh;           // 单一圆形模板Mesh（所有圆复用！）
    private Vector3[] verticesCache;   // 顶点缓存（避免每帧分配）
    private bool isIndicatorActive;

    private void Start()
    {
        if (character == null) 
        {
            Debug.LogError("[ThrowSkillIndicator] 未指定角色Transform！");
            enabled = false;
            return;
        }
        
        InitializeResources();
        HideIndicator();
    }

    private void Update()
    {
        // 按住右键激活指示器（可自定义按键）
        if (Input.GetMouseButton(1))
        {
            ShowIndicator();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            HideIndicator();
            // TODO: 此处添加技能释放逻辑（使用 GetCurrentSkillPosition() 和 skillRadius）
        }

        if (isIndicatorActive)
        {
            UpdateIndicator();
        }
    }

    // ====== 资源初始化（仅执行1次！）======
    private void InitializeResources()
    {
        // ✅ 创建单一圆形模板Mesh（所有圆复用！）
        circleMesh = new Mesh();
        GenerateCircleMeshTemplate(circleMesh);
        
        // ✅ 创建共享材质（带边缘锐化效果）
        dragMat = CreateCircleMaterial(dragCircleColor);
        skillMat = CreateCircleMaterial(skillCircleColor);
        
        // ✅ 创建指示器GameObject（复用结构）
        dragCircleGo = CreateCircleObject("DragCircle", dragMat);
        skillCircleGo = CreateCircleObject("SkillCircle", skillMat);
        
        // ✅ 顶点缓存（避免每帧GC）
        verticesCache = new Vector3[circleSegments + 1];
    }

    // 创建圆形模板（中心点+边缘点）
    private void GenerateCircleMeshTemplate(Mesh mesh)
    {
        Vector3[] vertices = new Vector3[circleSegments + 1];
        Vector2[] uvs = new Vector2[circleSegments + 1];
        int[] triangles = new int[circleSegments * 3];
        
        // 中心点
        vertices[0] = Vector3.zero;
        uvs[0] = Vector2.zero;
        
        // 边缘点（单位圆）
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * (360f / circleSegments) * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            uvs[i + 1] = new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f);
        }
        
        // 三角形（扇形）
        for (int i = 0; i < circleSegments; i++)
        {
            int idx = i * 3;
            triangles[idx] = 0;
            triangles[idx + 1] = i + 1;
            triangles[idx + 2] = (i + 2) % circleSegments + 1;
        }
        
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.normals = new Vector3[vertices.Length]; // 所有法线设为(0,1,0)由Shader处理
        for (int i = 0; i < mesh.normals.Length; i++) mesh.normals[i] = Vector3.down;
    }

    // 创建带边缘锐化效果的材质
    private Material CreateCircleMaterial(Color color)
    {
        Shader shader = Shader.Find("Unlit/Transparent");
        if (shader == null) shader = Shader.Find("Standard");
        
        Material mat = new Material(shader)
        {
            color = color,
            renderQueue = 3000
        };
        
        // 关键：启用双面渲染 + 边缘锐化参数
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_ALPHABLEND_ON");
        
        // 传递锐化参数（需配合自定义Shader，此处用标准Shader降级处理）
        // 实际项目建议使用带边缘锐化的自定义Shader
        return mat;
    }

    private GameObject CreateCircleObject(string name, Material mat)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().mesh = circleMesh; // 复用同一Mesh！
        go.AddComponent<MeshRenderer>().material = mat;
        go.SetActive(false);
        return go;
    }

    // ====== 指示器控制 ======
    private void ShowIndicator()
    {
        isIndicatorActive = true;
        dragCircleGo.SetActive(true);
        skillCircleGo.SetActive(true);
        UpdateIndicator(); // 立即更新位置
    }

    private void HideIndicator()
    {
        isIndicatorActive = false;
        dragCircleGo.SetActive(false);
        skillCircleGo.SetActive(false);
    }

    // ====== 核心：每帧更新（零GC！）======
    private void UpdateIndicator()
    {
        // 1. 射线检测获取地面位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer)) return;
        
        Vector3 mousePos = hit.point;
        mousePos.y = character.position.y; // 锁定Y轴（地面高度）
        
        // 2. 计算拖拽距离（限制在最大范围内）
        Vector3 dirToMouse = mousePos - character.position;
        dirToMouse.y = 0;
        float currentDragDist = Mathf.Clamp(dirToMouse.magnitude, 0, maxDragDistance);
        Vector3 direction = dirToMouse.normalized;
        if (dirToMouse.sqrMagnitude < 0.1f) direction = Vector3.forward; // 避免除零
        
        // 3. 计算技能落地点（拖拽终点）
        Vector3 skillPosition = character.position + direction * currentDragDist;
        
        // 4. ✅ 零GC更新：复用verticesCache + 直接修改Mesh顶点
        UpdateCircleVertices(dragCircleGo, character.position, currentDragDist);
        UpdateCircleVertices(skillCircleGo, skillPosition, skillRadius);
    }

    // 高效更新圆（复用缓存数组，避免分配）
    private void UpdateCircleVertices(GameObject circleGo, Vector3 center, float radius)
    {
        // 填充缓存数组（中心点+边缘点）
        verticesCache[0] = center; // 中心
        
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * (360f / circleSegments) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            verticesCache[i + 1] = center + new Vector3(x, 0, z);
        }
        
        // 直接更新Mesh（无new！）
        Mesh mesh = circleGo.GetComponent<MeshFilter>().mesh;
        mesh.vertices = verticesCache; // Unity内部会复制，但数组大小固定，GC压力极小
        // 注：法线已在初始化时设为Vector3.up，无需每帧更新
    }

    // ====== 辅助方法：供外部调用 ======
    /// <summary>获取当前技能落地位置（松开鼠标时调用）</summary>
    public Vector3 GetCurrentSkillPosition()
    {
        if (!isIndicatorActive) return character.position + Vector3.forward * maxDragDistance;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            Vector3 pos = hit.point;
            pos.y = character.position.y;
            Vector3 dir = (pos - character.position).normalized;
            dir.y = 0;
            float dist = Mathf.Clamp((pos - character.position).magnitude, 0, maxDragDistance);
            return character.position + dir * dist;
        }
        return character.position + Vector3.forward * maxDragDistance;
    }

    public float GetSkillRadius() => skillRadius;

    // ====== 资源清理 ======
    private void OnDestroy()
    {
        // 安全销毁手动创建的资源
        if (dragMat != null) Destroy(dragMat);
        if (skillMat != null) Destroy(skillMat);
        if (circleMesh != null) Destroy(circleMesh);
        if (dragCircleGo != null) Destroy(dragCircleGo);
        if (skillCircleGo != null) Destroy(skillCircleGo);
    }

    // ====== 编辑器辅助（可选）======
    private void OnDrawGizmosSelected()
    {
        if (character == null) return;
        
        // 绘制最大拖拽范围（编辑器辅助）
        Gizmos.color = new Color(0, 0.7f, 1, 0.3f);
        Gizmos.DrawWireSphere(character.position, maxDragDistance);
        
        // 绘制技能范围示例
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.4f);
        Gizmos.DrawWireSphere(character.position + Vector3.forward * maxDragDistance, skillRadius);
    }
}