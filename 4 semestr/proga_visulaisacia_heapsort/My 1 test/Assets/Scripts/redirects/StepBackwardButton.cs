using UnityEngine;

public class StepBackwardButton : MonoBehaviour
{
    [SerializeField] private HeapSortVisualizer sorter;
    private void OnMouseDown() => sorter.StepBackward();
}