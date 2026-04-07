#import <Foundation/Foundation.h>
#import <mach/mach.h>
#import <mach/mach_host.h>
#import <os/proc.h>

extern "C" {

    /// メモリ情報を取得する構造体
    struct NativeMemoryData {
        uint64_t allocatedMemory;      // 確保したメモリ量（タスクメモリ）
        uint64_t memoryFootprint;      // 実際の物理メモリ使用量（phys_footprint）
        uint64_t availableMemory;      // 残り利用可能メモリ
        uint64_t absoluteLimit;        // 絶対的限界値
        uint64_t physicalMemorySize;   // デバイスの物理メモリサイズ
    };

    /// タスクメモリ情報を取得
    static uint64_t GetTaskMemoryInfo(task_vm_info_data_t *vmInfo) {
        mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
        kern_return_t kr = task_info(mach_task_self(),
                                     TASK_VM_INFO,
                                     (task_info_t)vmInfo,
                                     &count);
        if (kr == KERN_SUCCESS) {
            return vmInfo->phys_footprint;
        }
        return 0;
    }

    /// 物理メモリサイズを取得
    static uint64_t GetPhysicalMemorySize() {
        return [NSProcessInfo processInfo].physicalMemory;
    }

    /// 利用可能メモリを取得（iOS 13+）
    static uint64_t GetAvailableMemory() {
        if (@available(iOS 13.0, *)) {
            return os_proc_available_memory();
        }
        return 0;
    }

    /// メモリプレッシャー状態を取得
    static int GetMemoryPressureLevel() {
        dispatch_source_t source = dispatch_source_create(
            DISPATCH_SOURCE_TYPE_MEMORYPRESSURE,
            0,
            DISPATCH_MEMORYPRESSURE_NORMAL | DISPATCH_MEMORYPRESSURE_WARN | DISPATCH_MEMORYPRESSURE_CRITICAL,
            dispatch_get_main_queue()
        );

        if (source) {
            dispatch_source_cancel(source);
            return 0; // 現在の実装では常に0を返す（改善の余地あり）
        }
        return -1;
    }

    /// ネイティブメモリ情報を取得（Unity呼び出し用）
    void GetNativeMemoryInfo(NativeMemoryData *data) {
        if (data == NULL) return;

        // タスク情報を取得
        task_vm_info_data_t vmInfo;
        mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
        kern_return_t kr = task_info(mach_task_self(),
                                     TASK_VM_INFO,
                                     (task_info_t)&vmInfo,
                                     &count);

        if (kr == KERN_SUCCESS) {
            data->allocatedMemory = vmInfo.internal + vmInfo.compressed;
            data->memoryFootprint = vmInfo.phys_footprint;
        } else {
            data->allocatedMemory = 0;
            data->memoryFootprint = 0;
        }

        // 利用可能メモリ（iOS 13+）
        if (@available(iOS 13.0, *)) {
            data->availableMemory = os_proc_available_memory();
        } else {
            data->availableMemory = 0;
        }

        // 物理メモリサイズ
        data->physicalMemorySize = GetPhysicalMemorySize();

        // 絶対的限界値の計算
        // 現在のフットプリント + 利用可能メモリ = このアプリが使用できる最大メモリ
        data->absoluteLimit = data->memoryFootprint + data->availableMemory;
    }

    /// 個別取得関数
    uint64_t GetAllocatedMemory() {
        task_vm_info_data_t vmInfo;
        GetTaskMemoryInfo(&vmInfo);
        return vmInfo.internal + vmInfo.compressed;
    }

    uint64_t GetMemoryFootprint() {
        task_vm_info_data_t vmInfo;
        GetTaskMemoryInfo(&vmInfo);
        return vmInfo.phys_footprint;
    }

    uint64_t GetAvailableMemorySize() {
        return GetAvailableMemory();
    }

    uint64_t GetAbsoluteMemoryLimit() {
        NativeMemoryData data;
        GetNativeMemoryInfo(&data);
        return data.absoluteLimit;
    }

    uint64_t GetPhysicalMemory() {
        return GetPhysicalMemorySize();
    }
}
