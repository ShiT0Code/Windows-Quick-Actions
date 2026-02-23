using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.UI.WindowManagement;
using WinRT.Interop;


namespace TouchHelper.SideBar;

public sealed partial class SideBarWindow_Left : Window
{
    public SideBarWindow_Left()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        //this.SystemBackdrop = new WinUIEx.TransparentTintBackdrop();
        //this.SystemBackdrop  = new DesktopAcrylicBackdrop();
    }

    private void SideBarWindow_Left_Closed(object sender, WindowEventArgs args) => args.Handled = true;

    private bool IsPaneOpen = false;
    private nint hwnd;


    private async void PropertySizer_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (IsPaneOpen)
            paneCloseStoryboard.Begin();
        else
            paneOpenStoryboard.Begin();
        IsPaneOpen = !IsPaneOpen;

    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        hwnd = WindowNative.GetWindowHandle(this);
        OverlappedPresenter presenter = OverlappedPresenter.Create();
        //presenter.IsAlwaysOnTop = true;
        //presenter.IsMaximizable = false;
        //presenter.IsMinimizable = true;
        //presenter.IsResizable = false;
        presenter.SetBorderAndTitleBar(true, false);
        AppWindow.SetPresenter(presenter);
        //AppWindow.IsShownInSwitchers = false;
    }

    private void Sizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (paneTransform.X < -150)
        {
            paneTransform.X = -368;
            IsPaneOpen = false;
        }
        else
        {
            paneTransform.X = 0;
            IsPaneOpen = true;
        }
    }


}