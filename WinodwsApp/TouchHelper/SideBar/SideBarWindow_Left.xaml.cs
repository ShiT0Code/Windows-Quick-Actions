using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using System.Threading.Tasks;


namespace TouchHelper.SideBar;
public sealed partial class SideBarWindow_Left : Window
{
    public SideBarWindow_Left()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
    }

    private bool IsPaneOpen = false;

    private async void PropertySizer_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (IsPaneOpen)
        {
            paneCloseStoryboard.Begin();
            await Task.Delay(500);
            paneGrid.Width = 0;
            paneOpenStoryboard.Begin();
        }
        else
        {
            paneGrid.Width = 368;
            paneOpenStoryboard.Begin();
        }
        IsPaneOpen = !IsPaneOpen;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Sizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (paneGrid.ActualWidth > 175)
        {
            paneGrid.Width = 368;
            IsPaneOpen = true;
        }
        else
        {
            paneGrid.Width = 0;
            IsPaneOpen = false;
        }
    }
}
