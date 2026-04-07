package com.unity.nativememory;

import android.app.ActivityManager;
import android.content.Context;
import android.os.Debug;
import com.unity3d.player.UnityPlayer;

public class NativeMemoryInfo {

    /// メモリ情報構造体（データ受け渡し用）
    public static class MemoryData {
        public long allocatedMemory;      // 確保したメモリ量
        public long memoryFootprint;      // 実際の物理メモリ使用量
        public long availableMemory;      // 残り利用可能メモリ
        public long absoluteLimit;        // 絶対的限界値
        public long physicalMemorySize;   // デバイスの物理メモリサイズ
    }

    /// コンテキストを取得
    private static Context getContext() {
        return UnityPlayer.currentActivity.getApplicationContext();
    }

    /// ActivityManagerを取得
    private static ActivityManager getActivityManager() {
        return (ActivityManager) getContext().getSystemService(Context.ACTIVITY_SERVICE);
    }

    /// ネイティブメモリ情報を取得
    public static MemoryData GetNativeMemoryInfo() {
        MemoryData data = new MemoryData();

        Context context = getContext();
        ActivityManager activityManager = getActivityManager();

        // Debug.MemoryInfo を取得
        Debug.MemoryInfo memoryInfo = new Debug.MemoryInfo();
        Debug.getMemoryInfo(memoryInfo);

        // 確保したメモリ量（Native Heap + Dalvik Heap）
        data.allocatedMemory = (memoryInfo.nativePss + memoryInfo.dalvikPss) * 1024L;

        // メモリフットプリント（合計PSS）
        data.memoryFootprint = memoryInfo.getTotalPss() * 1024L;

        // ActivityManager.MemoryInfo を取得
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        activityManager.getMemoryInfo(mi);

        // 利用可能メモリ
        data.availableMemory = mi.availMem;

        // 物理メモリサイズ
        data.physicalMemorySize = mi.totalMem;

        // 絶対的限界値の計算
        // 現在のフットプリント + 利用可能メモリ = このアプリが使用できる最大メモリ
        data.absoluteLimit = data.memoryFootprint + data.availableMemory;

        return data;
    }

    /// 個別取得関数
    public static long GetAllocatedMemory() {
        Debug.MemoryInfo memoryInfo = new Debug.MemoryInfo();
        Debug.getMemoryInfo(memoryInfo);
        return (memoryInfo.nativePss + memoryInfo.dalvikPss) * 1024L;
    }

    public static long GetMemoryFootprint() {
        Debug.MemoryInfo memoryInfo = new Debug.MemoryInfo();
        Debug.getMemoryInfo(memoryInfo);
        return memoryInfo.getTotalPss() * 1024L;
    }

    public static long GetAvailableMemory() {
        ActivityManager activityManager = getActivityManager();
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        activityManager.getMemoryInfo(mi);
        return mi.availMem;
    }

    public static long GetAbsoluteMemoryLimit() {
        // 現在のフットプリント + 利用可能メモリ
        return GetMemoryFootprint() + GetAvailableMemory();
    }

    public static long GetPhysicalMemory() {
        ActivityManager activityManager = getActivityManager();
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        activityManager.getMemoryInfo(mi);
        return mi.totalMem;
    }

    /// メモリプレッシャーレベルを取得（Androidでは低メモリ状態かどうか）
    public static boolean IsLowMemory() {
        ActivityManager activityManager = getActivityManager();
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        activityManager.getMemoryInfo(mi);
        return mi.lowMemory;
    }
}
