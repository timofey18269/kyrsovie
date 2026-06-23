using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeapSortVisualizer : MonoBehaviour
{
    [Header("Данные")]
    [SerializeField] private ArraySortData sortData;

    [Header("Сцена")]
    [SerializeField] private Transform mainTransform;
    [SerializeField] private Transform backgroundTransform;
    [SerializeField] private Transform footerTransform;

    [Header("Текстовый компонент на кнопке Остановить/Продолжить")]
    [SerializeField] private Component pauseButtonLabel;

    [Header("Текстовое поле описания этапа (TextMeshPro в сцене)")]
    [Tooltip("Текстовый объект, в который выводится описание текущего этапа сортировки")]
    [SerializeField] private TextMeshPro descriptionLabel;

    [Header("Анимация")]

    [SerializeField] private float moveSpeed = 400f;
    [SerializeField] private float stepDelay = 0.4f;

    [Tooltip("Задержка после подсветки перед движением, и после движения перед сбросом цвета (сек)")]
    [SerializeField] private float highlightDelay = 0.2f;

    [Header("Рёбра")]
    [SerializeField] private Color edgeColor = new Color(0.2f, 0.6f, 1f, 1f);
    [SerializeField] private float edgeWidth = 5f;

    [Header("Отступы внутри main")]
    [SerializeField] private float mainPaddingX = 50f;
    [SerializeField] private float mainPaddingY = 30f;

    [Header("Отступы внутри footer")]
    [SerializeField] private float footerPaddingX = 0.05f;

    [Header("Цвета подсветки")]
    [SerializeField] private Color highlightSwapColor = new Color(0.2f, 0.5f, 1f, 1f);   // синий — меняются местами
    [SerializeField] private Color highlightPivotColor = new Color(1f, 0.2f, 0.2f, 1f);  // красный — не двигается
    [SerializeField] private Color highlightDefaultColor = new Color(0.2f, 0.8f, 0.2f, 1f); // зелёный — исходный

    // -----------------------------------------------------------------------

    private enum StepType { Swap, ExtractRoot }

    private enum SortPhase
    {
        Heapify,     // построение двоичной кучи
        ExtractMax,  // извлечение максимума
        SiftDown     // восстановление свойств двоичной кучи
    }

    private class SortStep
    {
        public StepType type;
        public SortPhase phase;
        public int indexA;        // первый участник (синий)
        public int indexB;        // второй участник (синий)
        public int indexC = -1;   // наблюдатель (красный), -1 если отсутствует
        public int heapSizeAfter;
    }


    private List<SortStep> steps = new List<SortStep>();


    private int currentStepIndex = 0;


    private List<Transform> heapNodes = new List<Transform>();


    private List<Vector3> nodePositions = new List<Vector3>();


    private readonly List<GameObject> edgeObjects = new List<GameObject>();
    private int footerCount = 0;
    public bool isSorting = false;
    private bool isPaused = false;
    private bool isAnimating = false;
    private Coroutine playCoroutine;

    // -----------------------------------------------------------------------


    private void OnMouseDown() => StartSort();

    public void StartSort()
    {


        if (isSorting) { Debug.LogWarning("[HeapSortVisualizer] Сортировка уже идёт."); return; }



        if (sortData == null || sortData.numbers == null || sortData.numbers.Count == 0)

        { Debug.LogWarning("[HeapSortVisualizer] Нет данных."); return; }
        if (!CollectTreeNodes()) return;

        ClearAllEdgesInBackground();


        nodePositions = new List<Vector3>(heapNodes.Count);
        foreach (Transform t in heapNodes) nodePositions.Add(t.position);

        RedrawEdges(heapNodes.Count);

        footerCount = 0;
        currentStepIndex = 0;
        isPaused = false;
        SetPauseButtonText("Остановить");
        SetDescription("");


        steps = PlanHeapSort();

        isSorting = true;
        playCoroutine = StartCoroutine(PlayFromCurrentStep());
    }


    public void TogglePause()
    {


        if (!isSorting) { Debug.LogWarning("[HeapSortVisualizer] Сортировка не запущена."); return; }

        isPaused = !isPaused;

        if (isPaused)
        {
            SetPauseButtonText("Продолжить");
        }
        else
        {
            SetPauseButtonText("Остановить");
            if (playCoroutine == null)
                playCoroutine = StartCoroutine(PlayFromCurrentStep());
        }
    }


    public void StepForward()
    {
        if (!isSorting || !isPaused || isAnimating)

        { Debug.LogWarning("[HeapSortVisualizer] 'Вперёд' доступно только во время паузы."); return; }

        if (currentStepIndex >= steps.Count)

        { Debug.LogWarning("[HeapSortVisualizer] Сортировка уже завершена."); return; }



        StartCoroutine(ExecuteSingleStepForward());
    }


    public void StepBackward()
    {
        if (!isSorting || !isPaused || isAnimating)

        { Debug.LogWarning("[HeapSortVisualizer] 'Назад' доступно только во время паузы."); return; }

        if (currentStepIndex <= 0)

        { Debug.LogWarning("[HeapSortVisualizer] Это первый шаг."); return; }



        StartCoroutine(ExecuteSingleStepBackward());
    }

    // -----------------------------------------------------------------------


    private IEnumerator PlayFromCurrentStep()
    {
        while (currentStepIndex < steps.Count)
        {


            if (isPaused) { playCoroutine = null; yield break; }



            yield return ApplyStepForward(steps[currentStepIndex]);
            currentStepIndex++;

            yield return new WaitForSeconds(stepDelay);
        }


        isSorting = false;
        playCoroutine = null;
        SetPauseButtonText("Остановить");
        SetDescription("Сортировка завершена");
        ResetAllHighlights();
        Debug.Log("[HeapSortVisualizer] Сортировка завершена.");
    }




    private IEnumerator ExecuteSingleStepForward()
    {
        isAnimating = true;

        yield return ApplyStepForward(steps[currentStepIndex]);
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            isSorting = false;
            SetDescription("Сортировка завершена");
            ResetAllHighlights();
            Debug.Log("[HeapSortVisualizer] Сортировка завершена.");
        }

        isAnimating = false;
    }




    private IEnumerator ExecuteSingleStepBackward()
    {
        isAnimating = true;

        currentStepIndex--;
        yield return ApplyStepBackward(steps[currentStepIndex]);

        isAnimating = false;
    }

    // -----------------------------------------------------------------------


    private IEnumerator ApplyStepForward(SortStep step)
    {
        // Описание и подсветка ДО анимации
        SetDescription(PhaseToString(step.phase));
        ApplyHighlights(step);

        // Задержка после подсветки — перед началом движения
        yield return new WaitForSeconds(highlightDelay);

        if (step.type == StepType.Swap)
        {
            yield return AnimateSwap(step.indexA, step.indexB);
            sortData.AddSwap(step.indexA, step.indexB);
        }
        else
        {
            yield return AnimateExtractRoot(step.indexA, step.indexB, forward: true);
            sortData.AddSwap(step.indexA, step.indexB);
        }

        // Задержка после движения — перед сбросом цвета
        yield return new WaitForSeconds(highlightDelay);

        ResetHighlights(step);
        RedrawEdges(step.heapSizeAfter);
    }




    private IEnumerator ApplyStepBackward(SortStep step)
    {
        SetDescription(PhaseToString(step.phase));
        ApplyHighlights(step);

        yield return new WaitForSeconds(highlightDelay);

        if (step.type == StepType.Swap)


            yield return AnimateSwap(step.indexA, step.indexB);

        else

            yield return AnimateExtractRoot(step.indexA, step.indexB, forward: false);


        if (sortData.swapHistory.Count > 0)
            sortData.swapHistory.RemoveAt(sortData.swapHistory.Count - 1);

        yield return new WaitForSeconds(highlightDelay);

        ResetHighlights(step);
        int heapSizeBefore = step.type == StepType.ExtractRoot
            ? step.heapSizeAfter + 1 : step.heapSizeAfter;
        RedrawEdges(heapSizeBefore);
    }

    // -----------------------------------------------------------------------
    // Подсветка: A и B — синие (меняются местами), C — красный (наблюдатель)
    // -----------------------------------------------------------------------
    private void ApplyHighlights(SortStep step)
    {
        SetObvodkaColor(step.indexA, highlightSwapColor);
        if (step.indexB != step.indexA)
            SetObvodkaColor(step.indexB, highlightSwapColor);
        if (step.indexC >= 0)
            SetObvodkaColor(step.indexC, highlightPivotColor);
    }

    private void ResetHighlights(SortStep step)
    {
        SetObvodkaColor(step.indexA, highlightDefaultColor);
        if (step.indexB != step.indexA)
            SetObvodkaColor(step.indexB, highlightDefaultColor);
        if (step.indexC >= 0)
            SetObvodkaColor(step.indexC, highlightDefaultColor);
    }

    private void ResetAllHighlights()
    {
        for (int i = 0; i < heapNodes.Count; i++)
            SetObvodkaColor(i, highlightDefaultColor);
    }

    // Ищет дочерний объект "obvodka" у узла по индексу в heapNodes и меняет его цвет
    private void SetObvodkaColor(int nodeIndex, Color color)
    {
        if (nodeIndex < 0 || nodeIndex >= heapNodes.Count) return;
        Transform node = heapNodes[nodeIndex];
        if (node == null) return;

        Transform obvodka = node.Find("obvodka");
        if (obvodka == null) return;

        SpriteRenderer sr = obvodka.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;
    }

    // -----------------------------------------------------------------------
    // Текстовое поле описания
    // -----------------------------------------------------------------------
    private void SetDescription(string text)
    {
        if (descriptionLabel != null)
            descriptionLabel.text = text;
    }

    private static string PhaseToString(SortPhase phase)
    {
        switch (phase)
        {
            case SortPhase.Heapify: return "Построение двоичной кучи";
            case SortPhase.ExtractMax: return "Извлечение максимума";
            case SortPhase.SiftDown: return "Восстановление свойств двоичной кучи";
            default: return "";
        }
    }

    // -----------------------------------------------------------------------
    // Анимации
    // -----------------------------------------------------------------------
    private IEnumerator AnimateSwap(int indexA, int indexB)
    {
        if (indexA == indexB) yield break;

        Transform nodeA = heapNodes[indexA];
        Transform nodeB = heapNodes[indexB];

        Vector3 posA = nodePositions[indexA];
        Vector3 posB = nodePositions[indexB];

        bool doneA = false, doneB = false;
        StartCoroutine(MoveNode(nodeA, posB, () => doneA = true));
        StartCoroutine(MoveNode(nodeB, posA, () => doneB = true));
        while (!doneA || !doneB) yield return null;




        heapNodes[indexA] = nodeB;
        heapNodes[indexB] = nodeA;
    }


    private IEnumerator AnimateExtractRoot(int rootIndex, int lastIndex, bool forward)
    {
        if (forward)
        {
            Transform rootNode = heapNodes[rootIndex];




            if (rootIndex == lastIndex) { yield return MoveToFooter(rootNode); yield break; }



            Transform lastNode = heapNodes[lastIndex];

            Coroutine rootToFooter = StartCoroutine(MoveToFooter(rootNode));
            Coroutine lastToRoot = StartCoroutine(MoveNode(lastNode, nodePositions[rootIndex], null));

            heapNodes[rootIndex] = lastNode;
            heapNodes[lastIndex] = rootNode;

            yield return rootToFooter;
            yield return lastToRoot;
        }
        else
        {

            Transform nodeNowAtRoot = heapNodes[rootIndex];
            Transform nodeInFooter = heapNodes[lastIndex];

            footerCount--;

            bool doneA = false, doneB = false;
            StartCoroutine(MoveNode(nodeNowAtRoot, nodePositions[lastIndex], () => doneA = true));
            StartCoroutine(MoveNode(nodeInFooter, nodePositions[rootIndex], () => doneB = true));
            while (!doneA || !doneB) yield return null;




            int value = GetNodeValue(nodeInFooter);
            nodeInFooter.name = $"elem_in_tree_{rootIndex}_{value}";

            heapNodes[rootIndex] = nodeInFooter;
            heapNodes[lastIndex] = nodeNowAtRoot;
        }
    }

    // -----------------------------------------------------------------------
    // Планирование шагов алгоритма ("сухой прогон")
    // -----------------------------------------------------------------------
    private List<SortStep> PlanHeapSort()
    {
        var plannedSteps = new List<SortStep>();


        var values = new List<int>(heapNodes.Count);
        foreach (Transform t in heapNodes) values.Add(GetNodeValue(t));


        int heapSize = values.Count;

        // Этап 1: heapify
        for (int i = heapSize / 2 - 1; i >= 0; i--)
            SiftDownPlan(values, i, heapSize, plannedSteps, SortPhase.Heapify);

        // Этап 2: извлечение
        while (heapSize > 1)
        {
            int lastIndex = heapSize - 1;



            int tmp = values[0]; values[0] = values[lastIndex]; values[lastIndex] = tmp;


            heapSize--;

            plannedSteps.Add(new SortStep
            {
                type = StepType.ExtractRoot,
                phase = SortPhase.ExtractMax,
                indexA = 0,
                indexB = lastIndex,
                indexC = -1,
                heapSizeAfter = heapSize
            });

            SiftDownPlan(values, 0, heapSize, plannedSteps, SortPhase.SiftDown);
        }

        if (heapSize == 1)
        {
            plannedSteps.Add(new SortStep
            {
                type = StepType.ExtractRoot,
                phase = SortPhase.ExtractMax,
                indexA = 0,
                indexB = 0,
                indexC = -1,
                heapSizeAfter = 0
            });
        }

        return plannedSteps;
    }

    // При sift-down: A и B — те что меняются (синие),
    // C — тот потомок из двух, который НЕ выиграл сравнение (красный).

    private void SiftDownPlan(List<int> values, int i, int heapSize,
                               List<SortStep> plannedSteps, SortPhase phase)
    {
        while (true)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < heapSize && values[left] > values[largest]) largest = left;
            if (right < heapSize && values[right] > values[largest]) largest = right;

            if (largest == i) break;

            // Определяем "проигравшего" потомка — он будет красным (indexC)
            int loser = -1;
            if (largest == left && right < heapSize) loser = right;
            else if (largest == right && left < heapSize) loser = left;

            int tmp = values[i]; values[i] = values[largest]; values[largest] = tmp;



            plannedSteps.Add(new SortStep
            {
                type = StepType.Swap,
                phase = phase,
                indexA = i,
                indexB = largest,
                indexC = loser,
                heapSizeAfter = heapSize
            });

            i = largest;
        }
    }

    // -----------------------------------------------------------------------

    private IEnumerator MoveNode(Transform node, Vector3 target, System.Action onArrived)
    {
        while (Vector3.Distance(node.position, target) > 0.5f)
        {
            node.position = Vector3.MoveTowards(node.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        node.position = target;
        onArrived?.Invoke();
    }




    private IEnumerator MoveToFooter(Transform node)
    {
        Vector3 target = CalcFooterSlotPosition(footerCount);
        footerCount++;

        yield return MoveNode(node, target, null);

        node.name = $"elem_sorted_{footerCount - 1}_{GetNodeValue(node)}";
    }


    private Vector3 CalcFooterSlotPosition(int slotIndex)
    {
        Bounds footerBounds = GetWorldBounds(footerTransform.gameObject);

        float left = footerBounds.min.x + footerPaddingX;
        float right = footerBounds.max.x - footerPaddingX;
        float y = footerBounds.center.y;
        float z = backgroundTransform.position.z;

        int totalSlots = sortData.numbers.Count;
        float elemSize = GetElemWorldSize();

        float availableGap = totalSlots > 1
            ? (right - left - totalSlots * elemSize) / (totalSlots - 1) : 0f;


        float gap = Mathf.Clamp(availableGap, 0f, elemSize * 0.45f);
        float step = elemSize + gap;

        float startX = left + elemSize * 0.75f;


        return new Vector3(startX + slotIndex * step, y, z);
    }


    private float GetElemWorldSize()
    {
        foreach (Transform node in heapNodes)
        {
            if (node == null) continue;
            Bounds b = GetWorldBounds(node.gameObject);
            if (b.size.x > 0f) return b.size.x;
        }
        return 1f;
    }


    private void RedrawEdges(int heapSize)
    {
        ClearEdges();

        List<Vector3> positions = CalcTreePositions(heapNodes.Count);

        for (int i = 1; i < heapSize; i++)
        {
            int parent = (i - 1) / 2;
            DrawEdge(positions[parent], positions[i], $"edge_{parent}->{i}");
        }
    }


    private List<Vector3> CalcTreePositions(int count)
    {
        Bounds b = GetWorldBounds(mainTransform.gameObject);

        float left = b.min.x + mainPaddingX;
        float right = b.max.x - mainPaddingX;
        float top = b.max.y - mainPaddingY;
        float bottom = b.min.y + mainPaddingY;
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

    private void ClearEdges()
    {
        foreach (GameObject go in edgeObjects) if (go != null) Destroy(go);
        edgeObjects.Clear();
    }

    private void ClearAllEdgesInBackground()
    {
        var toDelete = new List<GameObject>();
        foreach (Transform child in backgroundTransform)
            if (child.name.StartsWith("edge_"))
                toDelete.Add(child.gameObject);
        foreach (GameObject go in toDelete) Destroy(go);
        edgeObjects.Clear();
    }

    private bool CollectTreeNodes()
    {
        var found = new SortedDictionary<int, Transform>();
        foreach (Transform child in backgroundTransform)
        {
            if (!child.name.StartsWith("elem_in_tree_")) continue;
            int treeIndex = ExtractTreeIndex(child.name);
            if (treeIndex < 0) continue;
            found[treeIndex] = child;
        }
        if (found.Count == 0)
        {
            Debug.LogWarning("[HeapSortVisualizer] Нет elem_in_tree_* объектов.");
            return false;
        }
        heapNodes = new List<Transform>(found.Count);
        for (int i = 0; i < found.Count; i++)
        {
            if (!found.ContainsKey(i))
            { Debug.LogError($"[HeapSortVisualizer] Пропущен узел {i}."); return false; }
            heapNodes.Add(found[i]);
        }
        return true;
    }

    private static int ExtractTreeIndex(string name)
    {
        const string prefix = "elem_in_tree_";
        if (!name.StartsWith(prefix)) return -1;
        string remainder = name.Substring(prefix.Length);
        int underscoreIdx = remainder.IndexOf('_');
        string treeIndexStr = underscoreIdx >= 0 ? remainder.Substring(0, underscoreIdx) : remainder;
        return int.TryParse(treeIndexStr, out int idx) ? idx : -1;
    }

    private int GetNodeValue(Transform node)
    {
        Transform znachnie = node.Find("Znachnie");
        if (znachnie == null) return 0;
        var tmp = znachnie.GetComponent<TextMeshPro>();
        if (tmp != null && int.TryParse(tmp.text, out int v)) return v;
        return 0;
    }

    private void SetPauseButtonText(string text)
    {
        pauseButtonLabel.transform.Find("Znachnie").GetComponent<TextMeshPro>().text = text;
    }

    private static Bounds GetWorldBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one * 0.1f);

        Bounds b = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
        return b;
    }
}