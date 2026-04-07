using System;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MemoryManager : MonoBehaviour
{
    enum AllocateMode
    {
        None,
        Increase,
        Decrease,
    }
    
    private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

    [SerializeField] TextMeshProUGUI mTextSystemMemorySize;
    [SerializeField] TextMeshProUGUI mTextGraphicsMemorySize;
    [SerializeField] TextMeshProUGUI mTextMonoHeapSize;
    [SerializeField] TextMeshProUGUI mTextMonoUsedSize;
    [SerializeField] TextMeshProUGUI mTextTotalReservedMemory;
    [SerializeField] TextMeshProUGUI mTextTotalAlloatorMemoey;
    [SerializeField] TextMeshProUGUI mTextTotalUnReservedMemory;
    [SerializeField] TextMeshProUGUI mTextGraphicDriverAllocatorMemory;
    [SerializeField] TextMeshProUGUI mTextTmpAllocatorSize;



    long mSystemMemorySizeMB;
    long mGraphicsMemorySizeMB;
    long mMonoHeapSizeLong;
    long mMonoUsedHeapSizeLong;
    long mTotalReservedMemoryLong;
    long mTotalAllocatorMemoryLong;
    long mTotalUnReservedMemoryLong;
    long mGraphicsDriverAllocatorMemoeyLong;
    uint mTempAllocatorSize;

    
    AllocateMode mTotalAllocaterMode = AllocateMode.None;
    AllocateMode mGraphicsDriverAllocaterMode = AllocateMode.None;
    AllocateMode mMonoHeapAllocaterMode = AllocateMode.None;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mSystemMemorySizeMB = -1;
        mGraphicsMemorySizeMB = -1;
        mMonoHeapSizeLong = -1;
        mMonoUsedHeapSizeLong = -1;
        mTotalReservedMemoryLong = -1;
        mTotalAllocatorMemoryLong = -1;
        mTotalUnReservedMemoryLong = -1;
        mGraphicsDriverAllocatorMemoeyLong = -1;
        mTempAllocatorSize = 0;

        mTotalAllocaterMode = AllocateMode.None;
        mGraphicsDriverAllocaterMode = AllocateMode.None;
        mMonoHeapAllocaterMode = AllocateMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        switch (mTotalAllocaterMode)
        {
            case AllocateMode.None:
                break;
            case AllocateMode.Increase:
                TotalAllocaterIncrease();
                break;
            case AllocateMode.Decrease:
                TotalAllocaterDecrease();
                break;
        }
        
        switch (mGraphicsDriverAllocaterMode)
        {
            case AllocateMode.None:
                break;
            case AllocateMode.Increase:
                TotalAllocaterIncrease();
                break; 
            case AllocateMode.Decrease:
                TotalAllocaterDecrease();
                break;
        }

        switch (mMonoHeapAllocaterMode)
        {
            case AllocateMode.None:
                break;
            case AllocateMode.Increase:
                TotalAllocaterIncrease();
                break;
            case AllocateMode.Decrease:
                TotalAllocaterDecrease();
                break;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {

        if(mSystemMemorySizeMB != SystemInfo.systemMemorySize)
        {
            mSystemMemorySizeMB = SystemInfo.systemMemorySize;
            mTextSystemMemorySize.text = FormatBytes(mSystemMemorySizeMB * 1024 * 1024);
        }

        if(mGraphicsMemorySizeMB != SystemInfo.graphicsMemorySize)
        {
            mGraphicsMemorySizeMB = SystemInfo.graphicsMemorySize;
            mTextGraphicsMemorySize.text = FormatBytes(mGraphicsMemorySizeMB * 1024 * 1024);
        }

        if (mMonoHeapSizeLong != Profiler.GetMonoHeapSizeLong())
        {
            mMonoHeapSizeLong = Profiler.GetMonoHeapSizeLong();
            mTextMonoHeapSize.text = FormatBytes(mMonoHeapSizeLong);
        }

        if (mMonoUsedHeapSizeLong != Profiler.GetMonoUsedSizeLong())
        {
            mMonoUsedHeapSizeLong = Profiler.GetMonoUsedSizeLong();
            mTextMonoUsedSize.text = FormatBytes(mMonoUsedHeapSizeLong);
        }

        if(mTotalReservedMemoryLong != Profiler.GetTotalReservedMemoryLong())
        {
            mTotalReservedMemoryLong= Profiler.GetTotalReservedMemoryLong();
            mTextTotalReservedMemory.text = FormatBytes(mTotalReservedMemoryLong);
        }

        if (mTotalAllocatorMemoryLong != Profiler.GetTotalAllocatedMemoryLong())
        {
            mTotalAllocatorMemoryLong = Profiler.GetTotalAllocatedMemoryLong();
            mTextTotalAlloatorMemoey.text = FormatBytes(mTotalAllocatorMemoryLong);
        }
        if(mTotalUnReservedMemoryLong != Profiler.GetTotalUnusedReservedMemoryLong())
        {
            mTotalUnReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
            mTextTotalUnReservedMemory.text = FormatBytes(mTotalUnReservedMemoryLong);
        }

        if(mGraphicsDriverAllocatorMemoeyLong != Profiler.GetAllocatedMemoryForGraphicsDriver())
        {
            mGraphicsDriverAllocatorMemoeyLong = Profiler.GetAllocatedMemoryForGraphicsDriver();
            mTextGraphicDriverAllocatorMemory.text = FormatBytes(mGraphicsDriverAllocatorMemoeyLong);
        }

        if(mTempAllocatorSize!= Profiler.GetTempAllocatorSize())
        {
            mTempAllocatorSize = Profiler.GetTempAllocatorSize();
            mTextTmpAllocatorSize.text = FormatBytes(mTempAllocatorSize);         
        }
    }

    void TotalAllocaterIncrease()
    {

    }

    void TotalAllocaterDecrease()
    {
    }

    /// <summary>
    /// MonoHeapを増加させDirtyとする
    /// </summary>
    void MonoHeapIncrease()
    {
        
    }

    /// <summary>
    /// MonoHeapに確保したオブジェクトを減らす
    /// </summary>
    void MonoHeapDecrease()
    {

    }
    

    /// <summary>
    /// Graphics Draiver用のメモリを増加させる
    /// </summary>
    void GraphicsDriverAllocaterIncrease()
    {

    }

    /// <summary>
    /// Graphics Driver用のメモリを減少させる
    /// </summary>
    void GraphicsDriverFreeAllocaterDecrease()
    {

    }



    public static string FormatBytes(long byteCount)
    {
        if (byteCount == 0) return "0 B";

        // 絶対値で計算（負の数の場合も考慮）
        long bytes = Math.Abs(byteCount);

        // 1024の何乗かを計算
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));

        // 単位の配列範囲を超えないように調整
        place = Math.Min(place, Units.Length - 1);

        // 数値を計算
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);

        // 元の符号を付けて返す
        return (Math.Sign(byteCount) * num).ToString() + " " + Units[place];
    }
}
