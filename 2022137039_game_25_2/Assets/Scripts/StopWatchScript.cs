using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class StopWatchScript : MonoBehaviour
{
    public void OnClick()
    {
        int[] GenerateRandomArray(int size)
        {
            int[] arr = new int[size];
            System.Random rand = new System.Random();
            for (int i = 0; i < size; i++)
            {
                arr[i] = rand.Next(0, 10000);
            }
            return arr;
        }

        int[] data1 = GenerateRandomArray(10000);
        int[] data2 = (int[])data1.Clone();
        int[] data3 = (int[])data1.Clone();

        Stopwatch sw = new Stopwatch();

        //선택 정렬
        //sw.Reset();
        sw.Start();
        SelectionSortTest.StartSelectionSort(data1);
        sw.Stop();
        long selectionTime = sw.ElapsedMilliseconds;

        //버블 정렬
        sw.Reset();
        sw.Start();
        BubbleSortTest.StartBubbleSort(data2);
        sw.Stop();
        long bubbleTime = sw.ElapsedMilliseconds;

        //퀵
        sw.Reset();
        sw.Start();
        QuickSortTest.StartQuickSort(data3, 0, data3.Length - 1);
        sw.Stop();
        long quickTime = sw.ElapsedMilliseconds;

        UnityEngine.Debug.Log(
            $"Selection Sort: {selectionTime} ms\n" +
            $"Bubble Sort: {bubbleTime} ms\n" +
            $"Quick Sort: {quickTime} ms"
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
