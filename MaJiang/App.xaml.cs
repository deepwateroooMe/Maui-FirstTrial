namespace MaJiang;
// 这个类，真的是：App.cs 吗？
// public partial class App : Application {
public partial class App : MauiWinUIWindow {

    // Initialize the singleton Application object. This is the first line of authoried code executed
    // and as such is the logical equivalent of main() or WinMain()
    public App() {
        ProcessManager.GetProcessLock();
        this.InitializeComponent();
        // InitializeComponent();
        // MainPage = new AppShell();
    }
    protected void MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

// 好像，下面这个方法，我放错地方了    
    // 但是，窗口运行中，要设置窗口大小或限制大小，则是要通过 Microsoft.Maui.Controls.Windows。
    // 例如，控制主窗口大小不能太小，不能被无限缩小，要在 APP.cs 中这样写：
    protected override Window CreateWindow(IActivationState？ activationState) {
        var window = base.CreateWindow(activationState);
        window.title = "LoveMyDearCousin.Maui";

        var minSize = GetMinSize();
        window.MinimumWidth = minSize.MinWidth;
        window.MinimumHeight = minSize.MinHeight;

        // Give the Window time to resize
        window.SizeChanged += (sender, e) => {
            var minSize = GetMinSize();
            window.MinimumWidth = minSize.MinWidth;
            window.MinimumHeight = minSize.MinHeight;
        };
        //window.Created += (s, e) =>
        //{
        //};
        //window.Stopped += (s, e) =>
        //{
        //};
        return window;
        (int MinWidth, int MinHeight) GetMinSize() { // 这就是内部类了呀
            // 获取当前屏幕的长宽，用 X、Y 表示。
            var x = PInvoke.User32.GetSystemMetrics(PInvoke.User32.SystemMetric.SM_CXFULLSCREEN);
            var y = PInvoke.User32.GetSystemMetrics(PInvoke.User32.SystemMetric.SM_CYFULLSCREEN);
            // 设置窗口最小值，可以按照比例计算，也可以直接设置固定大小
            return (x / 3 * 2, y / 5 * 4);
        }
    }
}
