using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace TouchHelper.Settings.SideBar;

public sealed partial class ActionListPage : Page
{
    public ActionListPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is List<Action> fixedItems)
            Items = fixedItems;
    }
    private string? ParentID = null;
    private List<Action> Items = [];

    private async void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        EditActionDialog dialog = new()
        {
            XamlRoot = this.XamlRoot
        };
        SelectAppXDialog dialog2 = new()
        {
            XamlRoot = this.XamlRoot
        };
        var result = await dialog.ShowAsync();
        bool needContinue = false;
        do
        {
            if (dialog.IsSelectingAppx)
            {
                needContinue = true;
                var result2 = await dialog2.ShowAsync();
                if (result2 == ContentDialogResult.Primary && dialog2.SelectApps.Count>0)
                    dialog.AppX = dialog2.SelectApps[0];
                result = await dialog.ShowAsync();
            }
            else
            {
                if (result == ContentDialogResult.Primary)
                {
                    needContinue = false;
                }
                else
                {
                    needContinue = false;
                }
            }
        }
        while (needContinue);

    }

    private void Del_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        foreach (var item in Items)
        {
            if (item.ID == Tag)
            {
                Items.Remove(item);
                itemsControl.Items.Remove(item);
                break;
            }
        }
    }

    private async void UI_Loaded(object sender, RoutedEventArgs e)
    {
        ParentID = Items[0].ParentID;
        if (Items.Count > 0)
            ParentID = Items[0].ParentID;
        if (Items.Count == 0 || Items[0].ID == "")
            Items.Clear();

        foreach (var item in Items)
        {
            itemsControl.Items.Add(item);
            await Task.Delay(20);
        }
        progressBar.IsIndeterminate = false;
        progressBar.Visibility = Visibility.Collapsed;

    }
}