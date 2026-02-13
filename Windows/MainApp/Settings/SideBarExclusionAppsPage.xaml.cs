using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace MainApp.Settings;
public sealed partial class SideBarExclusionAppsPage : Page
{
    public SideBarExclusionAppsPage() => InitializeComponent();

    private List<ExclusionApp> Items = [];
    private void AddWin32App_Button_Click(object sender, RoutedEventArgs e)
    {
        ExclusionApp item = new()
        {
            DisplayName = "应用 1",
            ExeFilePath = "cmd.exe"
        };
        Items.Add(item);
        itemsControl.Items.Add(item);
    }

    private async void AddAppXApp_Button_Click(object sender, RoutedEventArgs e)
    {
        SelectAppXDialog? dialog = new()
        {
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
        dialog = null;
    }

    private void DelItem_Button_Click(object sender, RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        foreach(var item in Items)
        {
            if(item.ID == Tag)
            {
                Items.Remove(item);
                itemsControl.Items.Remove(item);
                break;
            }
        }
    }
}