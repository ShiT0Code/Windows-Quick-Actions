using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using Windows.Graphics;
using static TouchHelper.SideBar.SideBarDataContainer;
namespace TouchHelper.SideBar;
public partial class SideBarWindow : Window
{
    public enum PaneWindowType { Left, Right };
    private PaneWindowType _paneWindowType;
    private readonly PointInt32 _closePoint;
    private SideBarCurrentData Data { get; set; }

    public SideBarWindow(PaneWindowType paneWindowType, RectInt32 closeRect,SideBarCurrentData data)
    {
        Data = data;

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
            Data.LeftPaneCurrentPoint = _closePoint;
            Data.IsLeftPaneOpen = false;
            Data.LeftOpacity = 1;
        }
        else
        {
            Data.RightPaneCurrentPoint = _closePoint;
            Data.IsRightPaneOpen = false;
            Data.RightOpacity = 1;
        }
    }

    private void Left_PaneManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        double deltaX = e.Delta.Translation.X;
        if (Data.LeftPaneCurrentPoint.X + deltaX < 50 * ScalePercent)
        {
            Data.LeftPaneCurrentPoint = new((int)(Data.LeftPaneCurrentPoint.X + e.Delta.Translation.X), Data.LeftPaneCurrentPoint.Y);
            AppWindow.Move(Data.LeftPaneCurrentPoint);
        }
    }
    private void Right_PaneManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        double deltaX = e.Delta.Translation.X;
        if (Data.RightPaneCurrentPoint.X + deltaX > ScreenWidth - (Data.Width+50) * ScalePercent)
        {
            Data.RightPaneCurrentPoint = new((int)(Data.RightPaneCurrentPoint.X + e.Delta.Translation.X), Data.RightPaneCurrentPoint.Y);
            AppWindow.Move(Data.RightPaneCurrentPoint);
        }
    }


    private void ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (_paneWindowType == PaneWindowType.Left)
        {
            if(Data.LeftPaneCurrentPoint.X > - Data.Width/2 * ScalePercent)
            {
                Data.LeftPaneCurrentPoint = new(7, Data.LeftPaneCurrentPoint.Y);
                Data.IsLeftPaneOpen = true;
                Data.LeftOpacity = 0;
            }
            else
            {
                Data.LeftPaneCurrentPoint = _closePoint;
                Data.IsLeftPaneOpen = false;
                Data.LeftOpacity = 1;
            }
            AppWindow.Move(Data.LeftPaneCurrentPoint);
        }
        else
        {
            if (Data.RightPaneCurrentPoint.X <= ScreenWidth - Data.Width/2 * ScalePercent)
            {
                Data.RightPaneCurrentPoint = new((int)(ScreenWidth - (Data.Width+10) * ScalePercent), Data.RightPaneCurrentPoint.Y);
                Data.IsRightPaneOpen = true;
                Data.RightOpacity = 0;
            }
            else
            {
                Data.RightPaneCurrentPoint = _closePoint;
                Data.IsRightPaneOpen = false;
                Data.RightOpacity = 1;
            }
            AppWindow.Move(Data.RightPaneCurrentPoint);
        }
    }

    private void ExitApp_MenuFlyoutItem_Click(object sender, RoutedEventArgs e) => Environment.Exit(0);

    private void Rectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) => this.Close();

    private async void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Delay(750);
        frame.Navigate(typeof(SideBarMainPage));
    }
}