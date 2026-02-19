using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouchHelper.DataCore;

namespace TouchHelper.Settings.SideBar;

public sealed partial class ActionListPage : Page
{
    public ActionListPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Dictionary<string, Action> fixedItems)
            Items = fixedItems;
    }
    private Dictionary<string, Action> Items = [];

    private void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Action action = new()
        {
            DisplayName = "新操作",
            ID = Guid.NewGuid().ToString()
        };
        itemsControl.Items.Insert(0, action);
        Items.Add(action.ID, action);
        _ = DataContainer.SaveFixedItems();
    }

    private void EditCard_Click(object sender, RoutedEventArgs e)
    {
        string Tag = (string)((CommunityToolkit.WinUI.Controls.SettingsCard)sender).Tag;
        var action = Items[Tag];
        SettingsWindowUI.Titles.Add(action.DisplayName + " 的详情");
        this.Frame.Navigate(typeof(EditActionPage), action, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void Del_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        var item = Items[Tag];
        itemsControl.Items.Remove(item);
        Items.Remove(Tag);
        _ = DataContainer.SaveFixedItems();
    }

    private async void UI_Loaded(object sender, RoutedEventArgs e)
    {

        foreach (var item in Items)
        {
            itemsControl.Items.Insert(0, item.Value);
            await Task.Delay(20);
        }
        progressBar.IsIndeterminate = false;
        progressBar.Visibility = Visibility.Collapsed;
    }

    private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        await Task.Delay(10);
        _ = DataContainer.SaveFixedItems();
    }
}