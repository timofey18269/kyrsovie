using System.Collections;
using TMPro;
using UnityEngine;

public class CleanupBackground : MonoBehaviour
{
    [Header("Target Object")]
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private ArraySortData sortData;
    [SerializeField] private HeapSortVisualizer hsv;
    [SerializeField] private TextMeshPro descriptionLabel;

    private void OnMouseDown()
    {
        StopAllCoroutinesOnBackground();
        CleanupChildren();
        sortData.Clear();
        hsv.isSorting = false;
        if (descriptionLabel != null)
            descriptionLabel.text = "";
    }
   
    public void CleanupChildren()
    {
        if (backgroundObject == null)
        {
            Debug.LogWarning("Background object is null!");
            return;
        }

        int removedCount = 0;
        for (int i = backgroundObject.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = backgroundObject.transform.GetChild(i);
            string childName = child.name;

            if (childName.StartsWith("elem") || childName.StartsWith("edge"))
            {
                Debug.Log($"Removing child: {childName}");
                Destroy(child.gameObject);
                removedCount++;
            }
        }

        Debug.Log($"Removed {removedCount} child elements.");
    }

    public void StopAllCoroutinesOnBackground()
    {
        if (backgroundObject == null)
        {
            Debug.LogWarning("Background object is null!");
            return;
        }

        MonoBehaviour[] monoBehaviours = backgroundObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour mb in monoBehaviours)
        {
            if (mb != null && mb.isActiveAndEnabled)
            {
                try
                {
                    mb.StopAllCoroutines();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to stop coroutines on {mb.name}: {e.Message}");
                }
            }
        }

        Debug.Log("All coroutines stopped on background and its children.");
    }
}