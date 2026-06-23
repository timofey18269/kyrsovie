using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private HeapSortVisualizer sorter;
    private void OnMouseDown() => sorter.TogglePause();
}