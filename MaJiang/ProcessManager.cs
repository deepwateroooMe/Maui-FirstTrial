namespace MaJiang;

// 使用场景：上下文
// 如何限制一次只能打开一个程序
// 场景，如果程序D 已被运行 进程 A，那么再次启动程序D 运行进程 B，B 会识别到已有相同的进程，此时 B 会将 A 窗口激活弹出来，然后 B 再退出。
// 这样不仅可以限制只能运行一个进程，而且可以让用户体验更加好。
// 锁可以使用 Mutex 来实现，在整个操作系统中，大家可以识别到同一个锁。
// 然后激活另一个窗口，可以使用 Win32。

// 进程管理器
internal static class ProcessManager {

    private static Mutex ProcessLock;
    private static bool HasLock;

    // 获取进程锁
    public static void GetProcessLock() {
        // 全局锁
        ProcessLock = new Mutex(false, "Global\\" + "自定义锁名称", out HasLock);
        if (!HasLock) {
            ActiveWindow();
            Environment.Exit(0);
        }
    }

    // 激活当前进程并将其窗口放到屏幕最前面
    public static void ActiveWindow() {
        string pName = Constants.Name;
        Process[] temp = Process.GetProcessesByName(pName);
        if (temp.Length > 0) {
            IntPtr handle = temp[0].MainWindowHandle;
            SwitchToThisWindow(handle, true);
        }
    }

    // 释放当前进程的锁
    // <remarks>小心使用</remarks>
    public static void ReleaseLock() {
        if (ProcessLock != null && HasLock) {
            ProcessLock.Dispose();
            HasLock = false;
        }
    }
        
    // 将另一个窗口激活放到前台。
    // Win32 API
    [DllImport("user32.dll")]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
}
