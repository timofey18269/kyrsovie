using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Хранит данные массива и историю перестановок (swap) для алгоритма heap sort.
/// Повесьте скрипт на пустой GameObject в сцене.
/// 
/// ПУБЛИЧНЫЕ ПОЛЯ:
/// - numbers     : List<int>             — все числа исходного массива
/// - swapHistory : List<(int, int)>      — история перестановок (индекс1, индекс2)
/// 
/// Заполняются через ArrayInputManager при подтверждении ввода массива.
/// Алгоритм heap sort (отдельный скрипт) будет дописывать swapHistory по ходу сортировки.
/// </summary>
public class ArraySortData : MonoBehaviour
{
    [Tooltip("Все числа исходного массива")]
    public List<int> numbers = new List<int>();

    [Tooltip("История перестановок: пары индексов элементов массива, которые менялись местами")]
    public List<(int indexA, int indexB)> swapHistory = new List<(int, int)>();

    // -----------------------------------------------------------------------
    // Заполняет numbers новым массивом, очищая предыдущие данные
    // -----------------------------------------------------------------------
    public void SetNumbers(int[] values)
    {
        numbers.Clear();
        numbers.AddRange(values);

        // Новый массив — старая история перестановок больше не актуальна
        swapHistory.Clear();
    }

    // -----------------------------------------------------------------------
    // Добавляет одну запись в историю перестановок
    // -----------------------------------------------------------------------
    public void AddSwap(int indexA, int indexB)
    {
        swapHistory.Add((indexA, indexB));
    }

    // -----------------------------------------------------------------------
    // Полный сброс данных
    // -----------------------------------------------------------------------
    public void Clear()
    {
        numbers.Clear();
        swapHistory.Clear();
    }
}