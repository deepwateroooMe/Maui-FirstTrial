using Microsoft.Maui.LifecycleEvents;

namespace MaJiang;

// 这个是它说的：App.cs 
public static class MauiProgram {

    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })

// 这里再次改写一下：成为这篇文章时原参考
            // MauiProgram.cs 中，有个 Microsoft.UI.Xaml.Window ,
            // 然后在 Windows 下 Microsoft.UI.Xaml.Window 是  Microsoft.Maui.MauiWinUIWindow，
            // Microsoft.UI.Xaml.Window 多种平台统一的抽象。
            // builder => {
            .ConfigureLifecycleEvents(events => {
                    events.AddWindows(windowLifeCycleBuilder => {
                        windowLifeCycleBuilder.OnWindowCreated(window => {
                            var nativeWindow = (window as Microsoft.UI.Xaml.Window) window;
                            // 如果是主窗口
                            
                        });
                    });
            });
        // 虽然你获得了 Microsoft.Maui.Controls.Window ，但是不能直接管理这个 Window，而是应该通过 Microsoft.UI.Xaml.Window，或 Microsoft.UI.Windowing.AppWindow 管理。
        // 也就是在依赖注入里面的窗口生命周期管理里面写。
        //     或者除非你可以拿到 AppWindow 实例。
        //     遗憾的是，Microsoft.Maui.Controls.Window 转不了 Microsoft.UI.Xaml.Window 或 Microsoft.UI.Windowing.AppWindow。
        //     你应该这样写：
        .ConfigureLifecycleEvents(events => {
            events.AddWindows(wndLifeCycleBuilder => {
                wndLifeCycleBuilder.OnWindowCreated(window => {
                    var nativeWindow = (window as Microsoft.Maui.MauiWinUIWindow)!;
                    // ... ...
                })
                    .OnActivated((window, args) => {
                    })
                    .OnClosed((window, args) => {
                        });
            });
        });            
        // 然后 Microsoft.UI.Xaml.Window 可以获取一个 AppWindow。
        AppWindow appWindow = nativeWindow.GetAppWindow()!;
        
// 这里作了分不同平台的生命周期函数的注册回调: 配置了两个平台，windows 和macOS,暂时去掉一个
// #if WINDOWS
//                 builder.RegisterWinUILifeCycleEvent();
// #endif
// #if MACCATALYST
//                 builder.RegisterMacLifeCycleEvent();
// #endif
                // });;
        return builder.Build();
    }

    // 它需要一个创建窗口的方法：我在两个类中都写这了这个方法，只要一个地方：App.cs 
    protected override Window CreateWindow() {
        // 如果要修改窗口标题，只能在窗口创建时修改，也就是 Microsoft.Maui.Controls.Windows，
        // 用 Microsoft.UI.Xaml.Window，或 Microsoft.UI.Windowing.AppWindow 都改不了
        Window window = base.CreateWindow(activationState);
        window.Title = Constants.Name; // 给它起某个名字
    }
// 窗口创建之后的回调：相关事件的回调注册定义等等
    // 要使用 Microsoft.UI.Xaml.Window，或 Microsoft.UI.Windowing.AppWindow ，例如在 MauiProgram.cs 里面记录了窗口的事件，创建窗口时控制大小。但是.......
    private static void MainWindowCreated(MauiWinUIWindow nativeWindow) {
            const int width = 1440;
            const int height = 900;
            AppWindow appWindow = nativeWindow.GetAppWindow()!;

            // 扩展标题栏，要自定义标题栏颜色，必须 true
            nativeWindow.ExtendsContentIntoTitleBar = true;
            // 这里必须设置为 Overlapped，之后窗口 Presenter 就是 OverlappedPresenter，便于控制
            appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            //if (appWindow.Presenter is OverlappedPresenter p)
            //{
            //   // p.SetBorderAndTitleBar(hasBorder: false, hasTitleBar: true);
            //}

            // 重新设置默认打开大小
            appWindow.MoveAndResize(new RectInt32(1920 / 2 - width / 2, 1080 / 2 - height / 2, width, height));
// 窗口调整的各类事件：在 AppWindow 里面的事件做大小限制，是做不到的，这里主要是观察，想做窗口大小等限制是不行的。
            appWindow.Changed += (w, e) => { // 窗口调整的各类事件
// 位置发生变化
                if (e.DidPositionChange) {
                }
                if (e.DidPresenterChange) { }
// 大小发生变化
                if (e.DidSizeChange) { }
                if (e.DidVisibilityChange) { }
                if (e.DidZOrderChange) { }
            };
            appWindow.Closing += async (w, e) => {
                try {
                    Environment.Exit(0);
                }
                catch (Exception ex) {
                    var log = AppHelpers.LoggerFactory.CreateLogger<AppWindow>();
                    log.LogError(ex, "Can't close WebHost");
                    ProcessManager.ReleaseLock();
                }
                finally {
                    ProcessManager.ExitProcess(0);
                }
            };
            appWindow.MoveInZOrderAtTop();
        }

    // 如果自己写了一个页面，要弹出这个窗口页面，那么应该使用 Microsoft.Maui.Controls.Window ，
    // 但是自己写的页面是 ContentPage，并不是 Window。
    // 因此并不能直接使用 Window，而是将 ContentPage 放到 Window 中，生成 Window 后再操作。
    private Microsoft.Maui.Controls.Window BuildUpdateWindow(ContentPage updatePage) {
        Window window = new Window(updatePage);
        window.title = "更新通知";

// 不知道下面的这些是具体在什么情况下使用，需要自己再多调试一下
        // 然后弹出这个窗口。
        Application.Current!.OpenWindow(updateWindow!);
        // 如果要异步打开窗口，请使用  Application.Current!.Dispatcher.DispatchAsync。
        await Application.Current!.Dispatcher.DispatchAsync(async () => {
            try {
                Application.Current!.OpenWindow(updateWindow!);
            }
            catch (Exception ex) {
                Logger.LogError("无法启动更新窗口", ex);
            }
        });
        // 如果想关闭所有窗口：
        await Application.Current!.Dispatcher.DispatchAsync(async () => {
            var windows = Application.Current!.Windows.ToArray();
            foreach (var window in windows) {
                try {
                    Application.Current.CloseWindow(window);
                }
                catch (Exception ex) {
                    Debug.Assert(ex != null);
                }
            }
        });        

        return window;
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