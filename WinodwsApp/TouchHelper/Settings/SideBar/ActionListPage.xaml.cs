using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
    //private List<string> ParentID = [];
    private List<Action> Items = [];

    private void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        EditItem(new());
    }
    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        var action = Items.FirstOrDefault(a => a.ID == Tag);
        if (action != null)
        {
            EditItem(action);
        }
    }

    private async void EditItem(Action action)
    {
        /*Debug.WriteLine(action.Command);
        Debug.WriteLine(";;;");
        EditActionDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            OldAction = action,
            IsEditing = isEditing
        };
        SelectAppXDialog dialog2 = new()
        {
            XamlRoot = this.XamlRoot,
            IsSingleSelect = true
        };
        var result = await dialog.ShowAsync();
        bool needContinue = false;
        do
        {
            if (dialog.IsSelectingAppx)
            {
                needContinue = true;
                var result2 = await dialog2.ShowAsync();
                if (result2 == ContentDialogResult.Primary && dialog2.SelectApps.Count > 0)
                    dialog.Action.App = dialog2.SelectApps[0];
                result = await dialog.ShowAsync();
            }
            else
            {
                if (result == ContentDialogResult.Primary)
                {
                    needContinue = false;
                    Debug.WriteLine("o" + action.DisplayName);
                    action.App = dialog.Action.App;
                    action.DisplayName = dialog.Action.DisplayName;
                    action.Command = dialog.Action.Command;
                    Debug.WriteLine("N" + action.Command);
                    action.Argument = dialog.Action.Argument;
                    action.ID = dialog.Action.ID;
                    action.ParentID = dialog.Action.ParentID;
                    action.Type = dialog.Action.Type;

                }
                else
                    needContinue = false;
            }
        }
        while (needContinue);

        if (!isEditing)
        {
            Items.Add(dialog.Action);
            itemsControl.Items.Add(dialog.Action);
        }
        var it = Items[0];
        Debug.WriteLine(it.Command);
        Debug.WriteLine(it.DisplayName);
        Debug.WriteLine(it.Type);
        ;*/
        if (string.IsNullOrEmpty(action.DisplayName))
            action.DisplayName = "新操作";
        SettingsWindowUI.Titles.Add(action.DisplayName);
        this.Frame.Navigate(typeof(EditActionPage), action, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
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
        //ParentID = Items[0].ParentID;
        //if (Items.Count > 0)
        //    ParentID = Items[0].ParentID;
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