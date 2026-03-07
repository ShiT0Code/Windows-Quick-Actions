using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using Windows.Graphics;
using static TouchHelper.SideBar.SideBarDataContainer;
namespace TouchHelper.SideBar;
public partial class SideBarTargetWindow : Window
{
    public enum PaneWindowType { Left, Right };
    private PaneWindowType _paneWindowType;
    private readonly PointInt32 _closePoint;
    private SideBarCurrentData CuDa { get; set; }

    public SideBarTargetWindow(PaneWindowType paneWindowType, RectInt32 closeRect,SideBarCurrentData data)
    {
        CuDa = data;

        _paneWindowType = paneWindowType;
        AppWindow.MoveAndResize(closeRect);
        _closePoint.X = closeRect.X;
        _closePoint.Y = closeRect.Y;

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsMaximizable = presenter.IsMinimizable = presenter.IsResizable = false;
        presenter.SetBorderAndTitleBar(true, false);
        AppWindow.SetPresenter(presenter);
        AppWindow.IsShownInSwitchers = false;

        InitializeComponent();
        if (_paneWindowType == PaneWindowType.Left)
        {
            Grid.SetColumn(rectangle, 2);
            titleBar.ManipulationDelta += Left_PaneManipulationDelta;
            rectangle.ManipulationDelta += Left_PaneManipulationDelta;
        }
        else
        {
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            titleBar.ManipulationDelta += Right_PaneManipulationDelta;
            rectangle.ManipulationDelta += Right_PaneManipulationDelta;
        }
        rectangle.DoubleTapped += Rectangle_DoubleTapped;
    }

    private void Close_AppBarButton_Click(object sender, RoutedEventArgs e) => this.Close();
    private void Window_Closed(object sender, WindowEventArgs args)
    {
        args.Handled = true;
        AppWindow.Move(_closePoint);
        if (_paneWindowType == PaneWindowType.Left)
        {
            CuDa.LeftPaneCurrentPoint = _closePoint;
            CuDa.IsLeftPaneOpen = false;
            CuDa.LeftOpacity = 1;
        }
        else
        {
            CuDa.RightPaneCurrentPoint = _closePoint;
            CuDa.IsRightPaneOpen = false;
            CuDa.RightOpacity = 1;
        }
    }

    private void Left_PaneManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        double deltaX = e.Delta.Translation.X;
        if (CuDa.LeftPaneCurrentPoint.X + deltaX < 48 * ScalePercent)
        {
            CuDa.LeftPaneCurrentPoint = new((int)(CuDa.LeftPaneCurrentPoint.X + e.Delta.Translation.X), CuDa.LeftPaneCurrentPoint.Y);
            AppWindow.Move(CuDa.LeftPaneCurrentPoint);
        }
    }
    private void Right_PaneManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        double deltaX = e.Delta.Translation.X;
        if (CuDa.RightPaneCurrentPoint.X + deltaX > ScreenWidth - 324 * ScalePercent)
        {
            CuDa.RightPaneCurrentPoint = new((int)(CuDa.RightPaneCurrentPoint.X + e.Delta.Translation.X), CuDa.RightPaneCurrentPoint.Y);
            AppWindow.Move(CuDa.RightPaneCurrentPoint);
        }
    }


    private void ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (_paneWindowType == PaneWindowType.Left)
        {
            if(CuDa.LeftPaneCurrentPoint.X > - 180 * ScalePercent)
            {
                CuDa.LeftPaneCurrentPoint = new(7, CuDa.LeftPaneCurrentPoint.Y);
                CuDa.IsLeftPaneOpen = true;
                CuDa.LeftOpacity = 0;
            }
            else
            {
                CuDa.LeftPaneCurrentPoint = _closePoint;
                CuDa.IsLeftPaneOpen = false;
                CuDa.LeftOpacity = 1;
            }
            AppWindow.Move(CuDa.LeftPaneCurrentPoint);
        }
        else
        {
            if (CuDa.RightPaneCurrentPoint.X <= ScreenWidth - 180 * ScalePercent)
            {
                CuDa.RightPaneCurrentPoint = new((int)(ScreenWidth - 286 * ScalePercent), CuDa.RightPaneCurrentPoint.Y);
                CuDa.IsRightPaneOpen = true;
                CuDa.RightOpacity = 0;
            }
            else
            {
                CuDa.RightPaneCurrentPoint = _closePoint;
                CuDa.IsRightPaneOpen = false;
                CuDa.RightOpacity = 1;
            }
            AppWindow.Move(CuDa.RightPaneCurrentPoint);
        }
    }

    private void ExitApp_MenuFlyoutItem_Click(object sender, RoutedEventArgs e) => Environment.Exit(0);

    private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => this.Close();

    private async void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Delay(750);
        var content = new SideBarWindowContent();
        Grid.SetRow(content, 1);
        Grid.SetColumn(content, 1);
        rootGrid.Children.Add(content);
    }
}