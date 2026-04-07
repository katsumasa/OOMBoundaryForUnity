using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
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
    [SerializeField] TextMeshProUGUI mTextTotalAllocatorMemory;
    [SerializeField] TextMeshProUGUI mTextTotalUnReservedMemory;
    [SerializeField] TextMeshProUGUI mTextGraphicDriverAllocatorMemory;
    [SerializeField] TextMeshProUGUI mTextTmpAllocatorSize;

    [SerializeField] Button mButtonGraphicsDriverAllocater;

    long mSystemMemorySizeMB;
    long mGraphicsMemorySizeMB;
    long mMonoHeapSizeLong;
    long mMonoUsedHeapSizeLong;
    long mTotalReservedMemoryLong;
    long mTotalAllocatorMemoryLong;
    long mTotalUnReservedMemoryLong;
    long mGraphicsDriverAllocatorMemoryLong;
    uint mTempAllocatorSize;


    AllocateMode mTotalAllocaterMode = AllocateMode.None;
    AllocateMode mGraphicsDriverAllocaterMode = AllocateMode.None;
    AllocateMode mMonoHeapAllocaterMode = AllocateMode.None;

    // Memory allocation storage
    List<NativeArray<byte>> mNativeArrays = new List<NativeArray<byte>>();
    List<byte[]> mManagedArrays = new List<byte[]>();
    List<Texture2D> mTextures = new List<Texture2D>();

    const int ALLOCATION_SIZE_MB = 10; // 10MB per allocation

    // Public methods for UI buttons

    /// <summary>
    /// Total Memoryのアロケートを開始/停止
    /// </summary>
    public void ToggleTotalAllocate()
    {
        if (mTotalAllocaterMode == AllocateMode.Increase)
        {
            mTotalAllocaterMode = AllocateMode.None;
        }
        else
        {
            mTotalAllocaterMode = AllocateMode.Increase;
        }
    }

    /// <summary>
    /// Total Memoryの解放を開始/停止
    /// </summary>
    public void ToggleTotalFree()
    {
        if (mTotalAllocaterMode == AllocateMode.Decrease)
        {
            mTotalAllocaterMode = AllocateMode.None;
        }
        else
        {
            mTotalAllocaterMode = AllocateMode.Decrease;
        }
    }

    /// <summary>
    /// Graphics Driverのアロケートを開始/停止
    /// </summary>
    public void ToggleGraphicsDriverAllocate()
    {
        if (mGraphicsDriverAllocaterMode == AllocateMode.Increase)
        {
            mGraphicsDriverAllocaterMode = AllocateMode.None;
        }
        else
        {
            mGraphicsDriverAllocaterMode = AllocateMode.Increase;
        }
    }

    /// <summary>
    /// Graphics Driverの解放を開始/停止
    /// </summary>
    public void ToggleGraphicsDriverFree()
    {
        if (mGraphicsDriverAllocaterMode == AllocateMode.Decrease)
        {
            mGraphicsDriverAllocaterMode = AllocateMode.None;
        }
        else
        {
            mGraphicsDriverAllocaterMode = AllocateMode.Decrease;
        }
    }

    /// <summary>
    /// MonoHeapのアロケートを開始/停止
    /// </summary>
    public void ToggleMonoHeapAllocate()
    {
        if (mMonoHeapAllocaterMode == AllocateMode.Increase)
        {
            mMonoHeapAllocaterMode = AllocateMode.None;
        }
        else
        {
            mMonoHeapAllocaterMode = AllocateMode.Increase;
        }
    }

    /// <summary>
    /// MonoHeapの解放を開始/停止
    /// </summary>
    public void ToggleMonoHeapFree()
    {
        if (mMonoHeapAllocaterMode == AllocateMode.Decrease)
        {
            mMonoHeapAllocaterMode = AllocateMode.None;
        }
        else
        {
            mMonoHeapAllocaterMode = AllocateMode.Decrease;
        }
    }

    /// <summary>
    /// ガベージコレクションを即座に実行
    /// </summary>
    public void ForceGarbageCollection()
    {
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
    }

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
        mGraphicsDriverAllocatorMemoryLong = -1;
        mTempAllocatorSize = 0;

        mTotalAllocaterMode = AllocateMode.None;
        mGraphicsDriverAllocaterMode = AllocateMode.None;
        mMonoHeapAllocaterMode = AllocateMode.None;
    }

    void OnDestroy()
    {
        // Clean up native arrays
        foreach (var nativeArray in mNativeArrays)
        {
            if (nativeArray.IsCreated)
            {
                nativeArray.Dispose();
            }
        }
        mNativeArrays.Clear();

        // Clean up textures
        foreach (var texture in mTextures)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }
        mTextures.Clear();

        // Managed arrays will be cleaned up by GC
        mManagedArrays.Clear();
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
                GraphicsDriverAllocaterIncrease();
                break;
            case AllocateMode.Decrease:
                GraphicsDriverAllocaterDecrease();
                break;
        }

        switch (mMonoHeapAllocaterMode)
        {
            case AllocateMode.None:
                break;
            case AllocateMode.Increase:
                MonoHeapIncrease();
                break;
            case AllocateMode.Decrease:
                MonoHeapDecrease();
                break;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        UpdateMemoryDisplayInt(ref mSystemMemorySizeMB, SystemInfo.systemMemorySize, mTextSystemMemorySize, 1024 * 1024);
        UpdateMemoryDisplayInt(ref mGraphicsMemorySizeMB, SystemInfo.graphicsMemorySize, mTextGraphicsMemorySize, 1024 * 1024);
        UpdateMemoryDisplayLong(ref mMonoHeapSizeLong, Profiler.GetMonoHeapSizeLong(), mTextMonoHeapSize);
        UpdateMemoryDisplayLong(ref mMonoUsedHeapSizeLong, Profiler.GetMonoUsedSizeLong(), mTextMonoUsedSize);
        UpdateMemoryDisplayLong(ref mTotalReservedMemoryLong, Profiler.GetTotalReservedMemoryLong(), mTextTotalReservedMemory);
        UpdateMemoryDisplayLong(ref mTotalAllocatorMemoryLong, Profiler.GetTotalAllocatedMemoryLong(), mTextTotalAllocatorMemory);
        UpdateMemoryDisplayLong(ref mTotalUnReservedMemoryLong, Profiler.GetTotalUnusedReservedMemoryLong(), mTextTotalUnReservedMemory);
        UpdateMemoryDisplayLong(ref mGraphicsDriverAllocatorMemoryLong, Profiler.GetAllocatedMemoryForGraphicsDriver(), mTextGraphicDriverAllocatorMemory);
        UpdateMemoryDisplayUInt(ref mTempAllocatorSize, Profiler.GetTempAllocatorSize(), mTextTmpAllocatorSize);
    }

    /// <summary>
    /// メモリ表示を更新するヘルパーメソッド (int型用)
    /// </summary>
    void UpdateMemoryDisplayInt(ref long cachedValue, int newValue, TextMeshProUGUI textField, long multiplier = 1)
    {
        if (cachedValue != newValue)
        {
            cachedValue = newValue;
            if (textField != null)
            {
                textField.text = FormatBytes(cachedValue * multiplier);
            }
        }
    }

    /// <summary>
    /// メモリ表示を更新するヘルパーメソッド (long型用)
    /// </summary>
    void UpdateMemoryDisplayLong(ref long cachedValue, long newValue, TextMeshProUGUI textField, long multiplier = 1)
    {
        if (cachedValue != newValue)
        {
            cachedValue = newValue;
            if (textField != null)
            {
                textField.text = FormatBytes(cachedValue * multiplier);
            }
        }
    }

    /// <summary>
    /// メモリ表示を更新するヘルパーメソッド (uint型用)
    /// </summary>
    void UpdateMemoryDisplayUInt(ref uint cachedValue, uint newValue, TextMeshProUGUI textField, long multiplier = 1)
    {
        if (cachedValue != newValue)
        {
            cachedValue = newValue;
            if (textField != null)
            {
                textField.text = FormatBytes(cachedValue * multiplier);
            }
        }
    }

    void TotalAllocaterIncrease()
    {
        // Allocate native memory (NativeArray)
        int sizeInBytes = ALLOCATION_SIZE_MB * 1024 * 1024;
        NativeArray<byte> nativeArray = new NativeArray<byte>(sizeInBytes, Allocator.Persistent);
        mNativeArrays.Add(nativeArray);
    }

    void TotalAllocaterDecrease()
    {
        // Free native memory
        if (mNativeArrays.Count > 0)
        {
            int lastIndex = mNativeArrays.Count - 1;
            if (mNativeArrays[lastIndex].IsCreated)
            {
                mNativeArrays[lastIndex].Dispose();
            }
            mNativeArrays.RemoveAt(lastIndex);
        }
    }

    /// <summary>
    /// MonoHeapを増加させるためにマネージドオブジェクトを生成
    /// </summary>
    void MonoHeapIncrease()
    {
        // Allocate managed memory (byte array)
        int sizeInBytes = ALLOCATION_SIZE_MB * 1024 * 1024;
        byte[] managedArray = new byte[sizeInBytes];
        mManagedArrays.Add(managedArray);
    }

    /// <summary>
    /// MonoHeapに確保されたオブジェクトを解放
    /// </summary>
    void MonoHeapDecrease()
    {
        // Free managed memory (GC is not called automatically - use ForceGarbageCollection() if needed)
        if (mManagedArrays.Count > 0)
        {
            mManagedArrays.RemoveAt(mManagedArrays.Count - 1);
        }
    }
    

    /// <summary>
    /// Graphics Driver用のメモリを増加させる
    /// </summary>
    void GraphicsDriverAllocaterIncrease()
    {
        // Allocate graphics memory (Texture2D)
        // Approximate 10MB texture: 1024x1024 RGBA32 = 4MB, so we use 1536x1536 for ~10MB
        int textureSize = 1536;
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, true);

        // Fill texture with random data to ensure allocation
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1.0f);
        }
        texture.SetPixels(pixels);
        texture.Apply();

        mTextures.Add(texture);
    }

    /// <summary>
    /// Graphics Driver用のメモリを解放する
    /// </summary>
    void GraphicsDriverAllocaterDecrease()
    {
        // Free graphics memory
        if (mTextures.Count > 0)
        {
            int lastIndex = mTextures.Count - 1;
            if (mTextures[lastIndex] != null)
            {
                Destroy(mTextures[lastIndex]);
            }
            mTextures.RemoveAt(lastIndex);
        }
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

        // 符号を付けて返す
        return (Math.Sign(byteCount) * num).ToString() + " " + Units[place];
    }
}
