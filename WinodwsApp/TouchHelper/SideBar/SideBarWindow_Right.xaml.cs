using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace TouchHelper.SideBar;

public sealed partial class SideBarWindow_Right : Window
{
    public SideBarWindow_Right()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
    }

    private bool IsPaneOpen = false;

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
        if (IsPaneOpen)
        {
            paneCloseStoryboard.Begin();
            await Task.Delay(500);
            paneTransform.X = 0;
        }
        else
        {
            paneOpenStoryboard.Begin();
        }
        IsPaneOpen = !IsPaneOpen;
    }

    private void Sizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (paneTransform.X < 175)
        {
            paneTransform.X = 0;
            IsPaneOpen = true;
        }
        else
        {
            paneTransform.X = 368;
            IsPaneOpen = false;
        }
    }
}
