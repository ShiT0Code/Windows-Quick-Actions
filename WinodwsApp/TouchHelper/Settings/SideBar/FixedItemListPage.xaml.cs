using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TouchHelper.Settings.SideBar;
public sealed partial class FixedItemListPage : Page
{
    public FixedItemListPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is List<FixedItem> fixedItems)
            Items = fixedItems;
    }
    private string ParentID = ""; // 如果为空，则为根
    private List<FixedItem> Items = [];

    private void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            string name = newTextBox.Text;
            if (!string.IsNullOrEmpty(name))
            {
                FixedItem item = new()
                {
                    Name = newTextBox.Text
                };
                Items.Add(item);
                itemsControl.Items.Add(item);
                flyout.Hide();
            }
        }
        catch(Exception ex)
        {
            new Window()
            {
                Title = ex.Message
            }.Activate();
        }
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


    private void SubItem_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((SettingsCard)sender).Tag;
        var item = Items.FirstOrDefault(i => i.ID == Tag);
        if( item != null)
        {
            var items = item.SubItems;
            if (items.Count == 0)
                items.Add(new()
                {
                    ParentID = this.ParentID,
                    ID = ""
                });
            SettingsWindowUI.Titles.Add(item.Name);
            this.Frame.Navigate(typeof(FixedItemListPage), items, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }

    private void Action_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((SettingsCard)sender).Tag;
        var item = Items.FirstOrDefault(i => i.ID == Tag);
        if (item != null)
        {
            SettingsWindowUI.Titles.Add(item.Name);
            var items = item.Actions;
            if (items.Count == 0)
                items.Add(new()
                {
                    ParentID = this.ParentID,
                    ID = ""
                });
            this.Frame.Navigate(typeof(ActionListPage), items, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }

    private async void UI_Loaded(object sender, RoutedEventArgs e)
    {
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