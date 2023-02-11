namespace MaJiang;

public partial class App : Application {

    public App() {
        InitializeComponent();
        MainPage = new AppShell();
    }
    protected override Window CreateWindow(IActivationState activationState) {
        var window = base.CreateWindow(activationState);
        // window.title = "LoveMyDearCousin.Maui";
        return window;
    }
}
