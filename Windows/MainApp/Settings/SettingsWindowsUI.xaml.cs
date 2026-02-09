using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace MainApp.Settings;
public sealed partial class SettingsWindowsUI : Grid
{
    public SettingsWindowsUI() => InitializeComponent();

    public static List<string> Titles { get; set; } = [];

    private async void Frame_Loaded(object s, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        frame.Navigate(typeof(SettingsHomePage));
        Titles.Add("设置");
        breadcrumbBar.ItemsSource = Titles.ToArray();
        frame.Navigated += Frame_Navigated;
    }

    private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e) => breadcrumbBar.ItemsSource = Titles.ToArray();

    private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        string item = (string)args.Item;
        int index = Titles.IndexOf(item);
        int count = Titles.Count;
        if (index == -1 || count == 1) return;
        while (Titles.Count > index + 1) Titles.RemoveAt(Titles.Count - 1);
        for (int i = 0; i < count - index - 1; i++)
            if (frame.CanGoBack)
                frame.GoBack();
    }

    private void Back_KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        if (Titles.Count <= 1) return;
        Titles.RemoveAt(Titles.Count - 1);
        if (frame.CanGoBack) frame.GoBack();
    }

    private void Home_KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
    {
        int count = Titles.Count;
        while (Titles.Count > 1)
            Titles.RemoveAt(Titles.Count - 1);
        for (int i = 0; i < count - 1; i++)
            if (frame.CanGoBack)
                frame.GoBack();
    }
}