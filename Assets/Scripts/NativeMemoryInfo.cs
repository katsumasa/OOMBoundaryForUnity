using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// OSネイティブのメモリ情報を取得するクラス
/// </summary>
public static class NativeMemoryInfo
{
    /// <summary>
    /// ネイティブメモリデータ構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryData
    {
        public ulong allocatedMemory;      // 確保したメモリ量
        public ulong memoryFootprint;      // 実際の物理メモリ使用量
        public ulong availableMemory;      // 残り利用可能メモリ
        public ulong absoluteLimit;        // 絶対的限界値
        public ulong physicalMemorySize;   // デバイスの物理メモリサイズ
    }

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void GetNativeMemoryInfo(out MemoryData data);

    [DllImport("__Internal")]
    private static extern ulong GetAllocatedMemory();

    [DllImport("__Internal")]
    private static extern ulong GetMemoryFootprint();

    [DllImport("__Internal")]
    private static extern ulong GetAvailableMemorySize();

    [DllImport("__Internal")]
    private static extern ulong GetAbsoluteMemoryLimit();

    [DllImport("__Internal")]
    private static extern ulong GetPhysicalMemory();
#endif

    /// <summary>
    /// すべてのネイティブメモリ情報を取得
    /// </summary>
    public static MemoryData GetMemoryInfo()
    {
#if UNITY_IOS && !UNITY_EDITOR
        MemoryData data;
        GetNativeMemoryInfo(out data);
        return data;
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetMemoryInfoAndroid();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return GetMemoryInfoWindows();
#else
        // フォールバック: Unityの情報を返す
        return GetMemoryInfoFallback();
#endif
    }

    /// <summary>
    /// 確保したメモリ量を取得
    /// </summary>
    public static ulong GetAllocatedMemorySize()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return GetAllocatedMemory();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetAllocatedMemoryAndroid();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return GetAllocatedMemoryWindows();
#else
        return 0;
#endif
    }

    /// <summary>
    /// 実際の物理メモリ使用量を取得（Memory Footprint）
    /// </summary>
    public static ulong GetMemoryFootprintSize()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return GetMemoryFootprint();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetMemoryFootprintAndroid();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return GetMemoryFootprintWindows();
#else
        return 0;
#endif
    }

    /// <summary>
    /// 残り利用可能メモリを取得
    /// </summary>
    public static ulong GetAvailableMemory()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return GetAvailableMemorySize();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetAvailableMemoryAndroid();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return GetAvailableMemoryWindows();
#else
        return 0;
#endif
    }

    /// <summary>
    /// 絶対的メモリ限界値を取得
    /// </summary>
    public static ulong GetAbsoluteLimit()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return GetAbsoluteMemoryLimit();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetAbsoluteLimitAndroid();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return GetAbsoluteLimitWindows();
#else
        return 0;
#endif
    }

    /// <summary>
    /// 物理メモリサイズを取得
    /// </summary>
    public static ulong GetPhysicalMemorySize()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return GetPhysicalMemory();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return GetPhysicalMemoryAndroid();
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return GetPhysicalMemoryWindows();
#else
        return (ulong)SystemInfo.systemMemorySize * 1024 * 1024;
#endif
    }

    // ==================== Android実装 ====================
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaClass nativeMemoryClass;

    private static AndroidJavaClass GetNativeMemoryClass()
    {
        if (nativeMemoryClass == null)
        {
            nativeMemoryClass = new AndroidJavaClass("com.unity.nativememory.NativeMemoryInfo");
        }
        return nativeMemoryClass;
    }

    private static MemoryData GetMemoryInfoAndroid()
    {
        AndroidJavaObject memoryDataObj = GetNativeMemoryClass().CallStatic<AndroidJavaObject>("GetNativeMemoryInfo");

        MemoryData data = new MemoryData
        {
            allocatedMemory = (ulong)memoryDataObj.Get<long>("allocatedMemory"),
            memoryFootprint = (ulong)memoryDataObj.Get<long>("memoryFootprint"),
            availableMemory = (ulong)memoryDataObj.Get<long>("availableMemory"),
            absoluteLimit = (ulong)memoryDataObj.Get<long>("absoluteLimit"),
            physicalMemorySize = (ulong)memoryDataObj.Get<long>("physicalMemorySize")
        };

        return data;
    }

    private static ulong GetAllocatedMemoryAndroid()
    {
        return (ulong)GetNativeMemoryClass().CallStatic<long>("GetAllocatedMemory");
    }

    private static ulong GetMemoryFootprintAndroid()
    {
        return (ulong)GetNativeMemoryClass().CallStatic<long>("GetMemoryFootprint");
    }

    private static ulong GetAvailableMemoryAndroid()
    {
        return (ulong)GetNativeMemoryClass().CallStatic<long>("GetAvailableMemory");
    }

    private static ulong GetAbsoluteLimitAndroid()
    {
        return (ulong)GetNativeMemoryClass().CallStatic<long>("GetAbsoluteMemoryLimit");
    }

    private static ulong GetPhysicalMemoryAndroid()
    {
        return (ulong)GetNativeMemoryClass().CallStatic<long>("GetPhysicalMemory");
    }

    public static bool IsLowMemory()
    {
        return GetNativeMemoryClass().CallStatic<bool>("IsLowMemory");
    }
#endif

    // ==================== Windows実装 ====================
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [StructLayout(LayoutKind.Sequential)]
    private struct MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("psapi.dll", SetLastError = true)]
    private static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);

    [StructLayout(LayoutKind.Sequential, Size = 72)]
    private struct PROCESS_MEMORY_COUNTERS
    {
        public uint cb;
        public uint PageFaultCount;
        public ulong PeakWorkingSetSize;
        public ulong WorkingSetSize;
        public ulong QuotaPeakPagedPoolUsage;
        public ulong QuotaPagedPoolUsage;
        public ulong QuotaPeakNonPagedPoolUsage;
        public ulong QuotaNonPagedPoolUsage;
        public ulong PagefileUsage;
        public ulong PeakPagefileUsage;
    }

    private static MemoryData GetMemoryInfoWindows()
    {
        MemoryData data = new MemoryData();

        MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
        memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

        if (GlobalMemoryStatusEx(ref memStatus))
        {
            data.physicalMemorySize = memStatus.ullTotalPhys;
            data.availableMemory = memStatus.ullAvailPhys;
        }

        PROCESS_MEMORY_COUNTERS pmc = new PROCESS_MEMORY_COUNTERS();
        pmc.cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));

        if (GetProcessMemoryInfo(GetCurrentProcess(), out pmc, pmc.cb))
        {
            data.allocatedMemory = pmc.PagefileUsage;
            data.memoryFootprint = pmc.WorkingSetSize;
        }

        // 絶対的限界値の計算
        // 現在のフットプリント + 利用可能メモリ = このアプリが使用できる最大メモリ
        data.absoluteLimit = data.memoryFootprint + data.availableMemory;

        return data;
    }

    private static ulong GetAllocatedMemoryWindows()
    {
        PROCESS_MEMORY_COUNTERS pmc = new PROCESS_MEMORY_COUNTERS();
        pmc.cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));
        if (GetProcessMemoryInfo(GetCurrentProcess(), out pmc, pmc.cb))
        {
            return pmc.PagefileUsage;
        }
        return 0;
    }

    private static ulong GetMemoryFootprintWindows()
    {
        PROCESS_MEMORY_COUNTERS pmc = new PROCESS_MEMORY_COUNTERS();
        pmc.cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS));
        if (GetProcessMemoryInfo(GetCurrentProcess(), out pmc, pmc.cb))
        {
            return pmc.WorkingSetSize;
        }
        return 0;
    }

    private static ulong GetAvailableMemoryWindows()
    {
        MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
        memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        if (GlobalMemoryStatusEx(ref memStatus))
        {
            return memStatus.ullAvailPhys;
        }
        return 0;
    }

    private static ulong GetAbsoluteLimitWindows()
    {
        // 現在のフットプリント + 利用可能メモリ
        return GetMemoryFootprintWindows() + GetAvailableMemoryWindows();
    }

    private static ulong GetPhysicalMemoryWindows()
    {
        MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
        memStatus.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        if (GlobalMemoryStatusEx(ref memStatus))
        {
            return memStatus.ullTotalPhys;
        }
        return 0;
    }
#endif

    // ==================== フォールバック実装 ====================
    private static MemoryData GetMemoryInfoFallback()
    {
        MemoryData data = new MemoryData
        {
            allocatedMemory = (ulong)UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong(),
            memoryFootprint = (ulong)UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong(),
            availableMemory = 0,
            absoluteLimit = (ulong)SystemInfo.systemMemorySize * 1024 * 1024,
            physicalMemorySize = (ulong)SystemInfo.systemMemorySize * 1024 * 1024
        };

        return data;
    }
}
