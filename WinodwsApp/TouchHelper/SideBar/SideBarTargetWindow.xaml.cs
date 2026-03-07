using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using Windows.Graphics;
using static TouchHelper.SideBar.SideBarDataContainer;
namespace TouchHelper.SideBar;
public partial class SideBarTargetWindow : Window
{
    public enum PaneWindowType { Left, Right };
    private PaneWindowType _paneWindowType;
    private readonly PointInt32 _closePoint;
    public SideBarTargetWindow(PaneWindowType paneWindowType, RectInt32 closeRect)
    {
        _paneWindowType = paneWindowType;
        AppWindow.MoveAndResize(closeRect);
        _closePoint.X = closeRect.X;
        _closePoint.Y = closeRect.Y;

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;
        //presenter.SetBorderAndTitleBar(true, false);
        AppWindow.SetPresenter(presenter);
        //AppWindow.IsShownInSwitchers = false;

        InitializeComponent();
        rectangle.HorizontalAlignment = (_paneWindowType == PaneWindowType.Left) ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    }

    private void Close_AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void Window_Closed(object sender, WindowEventArgs args)
    {
        args.Handled = true;
        AppWindow.Move(_closePoint);
        if (_paneWindowType == PaneWindowType.Left)
        {
            LeftBar.PaneCurrentPosition_L = _closePoint;
            LeftBar.LeftIsPaneOpen = false;
        }
        else
        {
            RightBar.PaneCurrentPosition_R = _closePoint;
            RightBar.RightIsPaneOpen= false;
        }
    }

    private async void Grid_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private void ManipulationDelta(object sender, Microsoft.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
    {
        double deltaX = e.Delta.Translation.X;
        if (_paneWindowType == PaneWindowType.Left && LeftBar.PaneCurrentPosition_L.X + deltaX < 48 * ScalePercent)
        {
            LeftBar.PaneCurrentPosition_L.X += (int)e.Delta.Translation.X;
            AppWindow.Move(LeftBar.PaneCurrentPosition_L);
        }
        else if (_paneWindowType == PaneWindowType.Right && RightBar.PaneCurrentPosition_R.X + deltaX > ScreenWidth - 324 * ScalePercent)
        {
            RightBar.PaneCurrentPosition_R.X += (int)e.Delta.Translation.X;
            AppWindow.Move(RightBar.PaneCurrentPosition_R);
        }
    }

    private void ManipulationCompleted(object sender, Microsoft.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
    {
        if (_paneWindowType == PaneWindowType.Left)
        {
            if(LeftBar.PaneCurrentPosition_L.X > - 180 * ScalePercent)
            {
                LeftBar.PaneCurrentPosition_L.X = 7;
                LeftBar.LeftIsPaneOpen = true;
            }
            else
            {
                LeftBar.PaneCurrentPosition_L.X = (int)(-276 * ScalePercent);
                LeftBar.LeftIsPaneOpen = false;
            }
            AppWindow.Move(LeftBar.PaneCurrentPosition_L);
        }
        else
        {
            if (RightBar.PaneCurrentPosition_R.X <= ScreenWidth - 180 * ScalePercent)
            {
                RightBar.PaneCurrentPosition_R.X = (int)(ScreenWidth - 286 * ScalePercent);
                RightBar.RightIsPaneOpen = true;
            }
            else
            {
                RightBar.PaneCurrentPosition_R.X = ScreenWidth;
                RightBar.RightIsPaneOpen = false;
            }
            AppWindow.Move(RightBar.PaneCurrentPosition_R);
        }
    }

    private void ExitApp_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}