using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBuilder : MonoBehaviour
{
    [Header("Сцена")]
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Transform headerTransform;
    [SerializeField] private Transform backgroundTransform;

    [Header("Анимация")]
    [Tooltip("Скорость перемещения элемента (единиц/сек)")]
    [SerializeField] private float moveSpeed = 300f;

    [Tooltip("Задержка перед стартом каждого следующего узла (сек)")]
    [SerializeField] private float delayBetweenNodes = 0.3f;

    [Header("Рёбра")]
    [SerializeField] private Color edgeColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private float edgeWidth = 5f;

    [Header("Отступы внутри main")]
    [SerializeField] private float paddingX = 50f;
    [SerializeField] private float paddingY = 30f;

    private readonly List<GameObject> edgeObjects = new List<GameObject>();
    private readonly List<GameObject> cloneObjects = new List<GameObject>();

    // -----------------------------------------------------------------------
    private void OnMouseDown()
    {
        BuildTree();
    }
    public void BuildTree()
    {
        List<Transform> elems = GetElems();

        ClearEdges();
        ClearClones();
        StopAllCoroutines();

        List<Vector3> positions = CalcTreePositions(elems);
        StartCoroutine(AnimateTree(elems, positions));
    }

    // -----------------------------------------------------------------------
    // все elem_ из background 
    // -----------------------------------------------------------------------
    private List<Transform> GetElems()
    {
        var result = new List<Transform>();
        if (backgroundTransform == null) return result;

        foreach (Transform child in backgroundTransform)
            if (child.name.StartsWith("elem_") && !child.name.StartsWith("elem_in_tree_"))
                result.Add(child);

        return result;
    }

    // -----------------------------------------------------------------------
    // Позиции дерева внутри main
    // -----------------------------------------------------------------------
    private List<Vector3> CalcTreePositions(List<Transform> elems)
    {
        int count = elems.Count;
        Bounds b = GetWorldBounds(mainTransform.gameObject);

        float left = b.min.x + paddingX;
        float right = b.max.x - paddingX;
        float top = b.max.y - paddingY;
        float bottom = b.min.y + paddingY;
        float z = backgroundTransform.position.z;

        int levels = Mathf.CeilToInt(Mathf.Log(count + 1, 2));
        if (levels < 1) levels = 1;

        float totalHeight = top - bottom;
        float levelStep = totalHeight / levels;

        var positions = new List<Vector3>(count);

        for (int i = 0; i < count; i++)
        {
            int level = (int)Mathf.Log(i + 1, 2);
            int nodesOnLevel = (int)Mathf.Pow(2, level);
            int posOnLevel = i - (nodesOnLevel - 1);

            float xStep = (right - left) / nodesOnLevel;
            float x = left + xStep * posOnLevel + xStep / 2f;
            float y = top - levelStep * level - levelStep / 2f;

            positions.Add(new Vector3(x, y, z));
        }

        return positions;
    }

    // -----------------------------------------------------------------------
    private IEnumerator AnimateTree(List<Transform> elems, List<Vector3> positions)
    {
        for (int i = 0; i < elems.Count; i++)
        {
            int index = i;
            Transform original = elems[index];
            Vector3 target = positions[index];

            string origValue = ExtractValue(original.name);
            string cloneName = $"elem_in_tree_{origValue}";

            // Создаём клон на месте оригинала
            GameObject clone = Instantiate(original.gameObject,
                                           original.position,
                                           original.rotation,
                                           backgroundTransform);
            clone.name = cloneName;
            clone.transform.localScale = original.localScale;
            clone.layer = 4;

            StartCoroutine(MoveElem(clone.transform, target, () =>
            {
                if (index > 0)
                {
                    int parentIndex = (index - 1) / 2;
                    DrawEdge(positions[parentIndex], positions[index], $"edge_{parentIndex}->{index}");
                }
            }));

            yield return new WaitForSeconds(delayBetweenNodes);
        }
    }

    // -----------------------------------------------------------------------
    private static string ExtractValue(string name)
    {
        int firstUnderscore = name.IndexOf('_');
        if (firstUnderscore < 0 || firstUnderscore >= name.Length - 1)
            return name;
        return name.Substring(firstUnderscore + 1);
    }

    // -----------------------------------------------------------------------
    private IEnumerator MoveElem(Transform elem, Vector3 target, System.Action onArrived)
    {
        while (Vector3.Distance(elem.position, target) > 0.5f)
        {
            elem.position = Vector3.MoveTowards(elem.position, target,
                                                moveSpeed * Time.deltaTime);
            yield return null;
        }

        elem.position = target;
        onArrived?.Invoke();
    }

    // -----------------------------------------------------------------------
    private void DrawEdge(Vector3 from, Vector3 to, string edgeName)
    {
        GameObject edgeGO = new GameObject(edgeName);
        edgeGO.transform.SetParent(backgroundTransform, true);
        edgeObjects.Add(edgeGO);

        LineRenderer lr = edgeGO.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);

        lr.startWidth = edgeWidth;
        lr.endWidth = edgeWidth;

        Shader shader = Shader.Find("Sprites/Default")
                     ?? Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default")
                     ?? Shader.Find("Unlit/Color");
        lr.material = new Material(shader);
        lr.material.color = edgeColor;
        lr.startColor = edgeColor;
        lr.endColor = edgeColor;

        lr.sortingLayerName = "Default";
        lr.sortingOrder = 2;
        lr.useWorldSpace = true;
    }

    // -----------------------------------------------------------------------
    private void ClearEdges()
    {
        foreach (GameObject go in edgeObjects)
            if (go != null) Destroy(go);
        edgeObjects.Clear();
    }

    private void ClearClones()
    {
        foreach (GameObject go in cloneObjects)
            if (go != null) Destroy(go);
        cloneObjects.Clear();
    }

    private static Bounds GetWorldBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one * 0.1f);

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }
}