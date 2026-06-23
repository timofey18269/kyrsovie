using UnityEngine;

public class StepForwardButton : MonoBehaviour
{
    [SerializeField] private HeapSortVisualizer sorter;
    private void OnMouseDown() => sorter.StepForward();
}