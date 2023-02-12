namespace MaJiang;
// 窗口管理
    // 前面提到，想管理窗口，API 要用 Microsoft.UI.Xaml.Window，或 Microsoft.UI.Windowing.AppWindow 的。
    // 有些地方只能用原生的 Window 窗口句柄，然后用 Win32 操作。
// 自定义窗口生命周期时，一定要使用：
    // 这里必须设置为 Overlapped，之后窗口 Presenter 就是 OverlappedPresenter，便于控制
    // appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
// 然后常用的窗口方法有：

/*
  AppWindow 的 Presenter ，一定是 OverlappedPresenter
*/
public class WindowService : IWindowService {

    private readonly AppWindow _appWindow;
    private readonly Window _window;

    private WindowService(AppWindow appWindow, Window window) {
        _appWindow = appWindow;
        _window = window;
    }
        
    // 检查当前窗口是否全屏
    public bool FullScreenState {
        get {
            switch (_appWindow.Presenter) {
            case OverlappedPresenter p:return p.State == OverlappedPresenterState.Maximized;
            case FullScreenPresenter p:return p.Kind == AppWindowPresenterKind.FullScreen;
            case CompactOverlayPresenter p: return p.Kind == AppWindowPresenterKind.FullScreen;
            case AppWindowPresenter p: return p.Kind == AppWindowPresenterKind.FullScreen;
            default:return false;
            }
        }
    }
    // 让窗口全屏: 让窗口全屏有两种方法，一种是全屏时，窗口把任务栏吞了，真正意义上的的全屏，另一种是保留任务栏。
    public void FullScreen() {
        switch (_appWindow.Presenter) {
        case OverlappedPresenter overlappedPresenter:
            overlappedPresenter.SetBorderAndTitleBar(true, true);
            overlappedPresenter.Maximize();
            break;
        }
        // 全屏时去掉任务栏
        // _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
    }
    // 退出全屏
    public void ExitFullScreen() {
        switch (_appWindow.Presenter) {
        case OverlappedPresenter p: p.Restore();break;
        default: _appWindow.SetPresenter(AppWindowPresenterKind.Default); break;
        }
    }
    // 最小化到任务栏: 
    public void Minmize() {
        // 最小化只能通过 Win32 API 处理，你要先获取 Microsoft.Maui.Controls.Windows，然后转换为 Window 句柄。
        // 此时 Microsoft.UI.Xaml.Window，或 Microsoft.UI.Windowing.AppWindow 就用不上了。 
#if WINDOWS
        var mauiWindow = App.Current.Windows.First();
        var nativeWindow = mauiWindow.Handler.PlatformView;
        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
        PInvoke.User32.ShowWindow(windowHandle, PInvoke.User32.WindowShowStyle.SW_MINIMIZE);
#endif
    }
    // 激活当前窗口
    public void Active() {
        _appWindow.Show(true);
    }
    // 关闭窗口
    public void Exit() {
        _window.Close();
    }
    public void SetSize(int _X, int _Y, int _Width, int _Height) {
        _appWindow.MoveAndResize(new RectInt32(_X, _Y, _Width, _Height));
    }
    public (int X, int Y) GetPosition() {
        var p = _appWindow.Position;
        return (p.X, p.Y);
    }
    public (int X, int Y) Move(int x, int y) {
        _appWindow.Move(new PointInt32(x, y));
        return GetPosition();
    }
    public (int Width, int Height, int ClientWidth, int ClientHeight) GetSize() {
        var size = _appWindow.Size;
        var clientSize = _appWindow.ClientSize;
        return (size.Width, size.Height, clientSize.Width, clientSize.Height);
    }
    public (PointInt32 Position, SizeInt32 Size, SizeInt32 ClientSize) GetAppSize() {
        return (_appWindow.Position, _appWindow.Size, _appWindow.ClientSize);
    }
}