using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrayInputManager : MonoBehaviour
{
    [Header("Сцена")]
    [SerializeField] private Transform headerTransform;
    [SerializeField] private Transform backgroundTransform;
    [SerializeField] private GameObject elemPrefab;
    [SerializeField] private ArraySortData sortData;

    [Header("Отступы внутри header")]
    [SerializeField] private float paddingX = 0.05f;
    [SerializeField] private float gapMin = 0.01f;

    [Tooltip("Множитель размера элемента (0.5 = половина от максимального)")]
    [SerializeField] private float elemSizeMultiplier = 0.75f;

    [Header("UI панели ввода (назначить в Inspector)")]
    [Tooltip("Корневой GameObject панели ввода — будет показываться/скрываться")]
    [SerializeField] private GameObject overlayCanvasGO;

    [Tooltip("TMP_InputField внутри панели")]
    [SerializeField] private TMP_InputField inputField;

    [Tooltip("Кнопка подтверждения")]
    [SerializeField] private Button confirmButton;

    [Tooltip("Кнопка отмены")]
    [SerializeField] private Button cancelButton;

    private readonly List<GameObject> spawnedElems = new List<GameObject>();

    // -----------------------------------------------------------------------
    private void Start()
    {
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
        if (inputField != null) inputField.onSubmit.AddListener(_ => OnConfirm());

        if (overlayCanvasGO != null) overlayCanvasGO.SetActive(false);
    }

    private void OnMouseDown()
    {
        OpenInputPanel();
    }

    // -----------------------------------------------------------------------
    public void OpenInputPanel()
    {
        if (inputField != null) inputField.text = "";
        if (overlayCanvasGO != null) overlayCanvasGO.SetActive(true);
        StartCoroutine(FocusNextFrame());
    }

    private System.Collections.IEnumerator FocusNextFrame()
    {
        yield return null;
        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    private void OnConfirm()
    {
        string rawText = inputField != null ? inputField.text.Trim() : "";
        if (string.IsNullOrEmpty(rawText))
        {
            Debug.LogWarning("[ArrayInputManager] Поле ввода пустое.");
            return;
        }

        int[] numbers = ParseInput(rawText);
        if (numbers == null || numbers.Length == 0)
        {
            Debug.LogWarning("[ArrayInputManager] Не удалось распарсить числа: " + rawText);
            return;
        }

        if (overlayCanvasGO != null) overlayCanvasGO.SetActive(false);
        SpawnElems(numbers);

        if (sortData != null)
            sortData.SetNumbers(numbers);
        else
            Debug.LogWarning("[ArrayInputManager] sortData не назначен — числа не сохранены.");
    }

    private void OnCancel()
    {
        if (overlayCanvasGO != null) overlayCanvasGO.SetActive(false);
    }

    // -----------------------------------------------------------------------
    private int[] ParseInput(string input)
    {
        string[] parts = input.Split(new char[] { ' ', '\t' },
            System.StringSplitOptions.RemoveEmptyEntries);
        var result = new List<int>();
        foreach (string part in parts)
        {
            if (int.TryParse(part, out int num))
                result.Add(num);
            else
                Debug.LogWarning($"[ArrayInputManager] Пропущен токен: '{part}'");
        }
        return result.ToArray();
    }

    // -----------------------------------------------------------------------
    private void SpawnElems(int[] numbers)
    {
        if (elemPrefab == null) { Debug.LogError("[ArrayInputManager] elemPrefab не назначен!"); return; }
        if (headerTransform == null) { Debug.LogError("[ArrayInputManager] headerTransform не назначен!"); return; }
        if (backgroundTransform == null) { Debug.LogError("[ArrayInputManager] backgroundTransform не назначен!"); return; }

        foreach (GameObject go in spawnedElems)
            if (go != null) Destroy(go);
        spawnedElems.Clear();

        Bounds headerBounds = GetWorldBounds(headerTransform.gameObject);
        float headerLeft = headerBounds.min.x + paddingX;
        float headerRight = headerBounds.max.x - paddingX;
        float headerWidth = headerRight - headerLeft;
        float headerY = headerBounds.center.y;
        float headerZ = headerTransform.position.z;

        int count = numbers.Length;

        float maxElemSize = (headerWidth - (count - 1) * gapMin) / count;
        float maxByHeight = headerBounds.size.y * 0.85f;
        float elemWorldSize = Mathf.Min(maxElemSize, maxByHeight);
        if (elemWorldSize <= 0f) elemWorldSize = maxElemSize;
        elemWorldSize *= Mathf.Clamp(elemSizeMultiplier, 0.1f, 1f);

        float elemLocalScaleX = elemWorldSize / (100f * backgroundTransform.lossyScale.x);
        float elemLocalScaleY = elemWorldSize / (100f * backgroundTransform.lossyScale.y);

        float slotSize = Mathf.Min(maxElemSize, elemWorldSize * 1.5f);
        float startX = headerLeft + slotSize / 2f;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * slotSize;
            Vector3 pos = new Vector3(x, headerY, headerZ);

            GameObject elem = Instantiate(elemPrefab, pos, Quaternion.identity, backgroundTransform);
            elem.name = $"elem_{i}_{numbers[i]}";
            elem.transform.localScale = new Vector3(elemLocalScaleX, elemLocalScaleY, 1f);

            SetElemValue(elem, numbers[i]);
            spawnedElems.Add(elem);
        }

        Debug.Log($"[ArrayInputManager] Создано {count} элементов, scale={elemLocalScaleX:F6}");
    }

    // -----------------------------------------------------------------------
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

    private void SetElemValue(GameObject elem, int value)
    {
        Transform znachnie = elem.transform.Find("Znachnie");
        if (znachnie == null)
        {
            Debug.LogWarning("[ArrayInputManager] 'Znachnie' не найден в префабе.");
            return;
        }

        var tmp = znachnie.GetComponent<TextMeshPro>();
        if (tmp != null) { tmp.text = value.ToString(); return; }

        var tmpUI = znachnie.GetComponent<TextMeshProUGUI>();
        if (tmpUI != null) { tmpUI.text = value.ToString(); return; }

        var legacy = znachnie.GetComponent<Text>();
        if (legacy != null) { legacy.text = value.ToString(); return; }

        Debug.LogWarning("[ArrayInputManager] Текстовый компонент не найден на 'Znachnie'.");
    }
}