using Microsoft.Maui.LifecycleEvents;

namespace MaJiang;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureLifecycleEvents(builder => {
// 这里作了分不同平台的生命周期函数的注册回调: 配置了两个平台，windows 和macOS,暂时去掉一个
// #if WINDOWS
//                 builder.RegisterWinUILifeCycleEvent();
// #endif
// #if MACCATALYST
//                 builder.RegisterMacLifeCycleEvent();
// #endif
                
            });;
        return builder.Build();
    }
    // // 不知道下面的要放在哪里才比较好呢？
    // public static bool RegisterWinUILifeCycleEvent(this ILifecycleBuilder builder) {
    //     builder.AddWindows(windows => {
    //         windows.OnWindowCreated(w => {
    //             MauiWinUIWindow mauiWindow = w as MauiWinUIWindow;
    //             if (mauiWindow is null)
    //                 return;
    //             var assist = new WindowUIAssist(mauiWindow);
    //             assist.SetTitleBar();
    //             assist.SetWindowSize(1300, 800);
    //         }).OnVisibilityChanged((w, e) => {
                
    //             });
    //     });
    // }

    // public bool SetTitleBar() {
    //     if (!AppWindowTitleBar.IsCustomizationSupported())
    //         return false;
    //     if (_AppWindow is null) {
    //         var windowId = Win32Interop.GetWindowidFromWindow(_MainWindow.WindowHandle);
    //         _AppWindow = AppWindow.GetFromWindowId(windowId);
    //     }
    //     if (_AppWindow is null) return false;
    //     _MainWindow.ExtendsContentIntoTitleBar = false;
    //     var titleBar = _AppWindow.TitleBar;
    //     // Set active window colors
    //     titleBar.ForegroundColor = Microsoft.UI.Colors.Black;
    //     titleBar.BackgroundColor = Microsoft.UI.Colors.Transparent;
    //     titleBar.ButtonForegroundColor = Microsoft.UI.Colors.Gray;
    //     titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
    //     titleBar.ButtonHoverForegroundColor = Microsoft.UI.Colors.Gainsboro;
    //     titleBar.ButtonHoverBackgroundColor = Microsoft.UI.Colors.DarkSeaGreen;
    //     titleBar.ButtonPressedForegroundColor = WindowsUI.Colors.FromArgb(80, 255, 255, 255);
    //     titleBar.ButtonPressedBackgroundColor = Microsoft.UI.Colors.DarkGreen;
    //     // Set inactive window Colors
    //     titleBar.InactiveForegroundColor = Microsoft.UI.Gainsboro;
    //     titleBar.InactiveBackgroundColor = Microsoft.UI.Transparent;
    //     titleBar.ButtonInactiveForegroundColor = Microsoft.UI.Colors.DarkSeaGreen;
    //     titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
    //     return true;
    // }
}