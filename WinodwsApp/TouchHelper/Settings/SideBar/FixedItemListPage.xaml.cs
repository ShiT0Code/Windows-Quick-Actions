using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouchHelper.DataCore;

namespace TouchHelper.Settings.SideBar;
public sealed partial class FixedItemListPage : Page
{
    public FixedItemListPage() => InitializeComponent();

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Dictionary<string, FixedItem> fixedItems)
            Items = fixedItems;
        else
            Items = DataContainer.RootFixedItem;
    }
    private Dictionary<string, FixedItem> Items = [];

    private void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string name = newTextBox.Text;
        if (!string.IsNullOrEmpty(name))
        {
            string id = Guid.NewGuid().ToString();
            FixedItem item = new()
            {
                Name = newTextBox.Text,
                ID = id
            };
            Items.Add(id, item);
            itemsControl.Items.Insert(0, item);
            flyout.Hide();
            newTextBox.Text = "";
        }
    }

    private void Del_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        //foreach (var item in Items)
        //{
        //    //if (item.ID == Tag)
        //    //{
        //    //    Items.Remove(item);
        //    //    itemsControl.Items.Remove(item);
        //    //    break;
        //    //}
        //}
        var item = Items[Tag];
        itemsControl.Items.Remove(item);
        Items.Remove(Tag);
    }


    private void SubItem_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((SettingsCard)sender).Tag;
        //var item = Items.FirstOrDefault(i => i.ID == Tag);
        //if( item != null)
        //{
        //    var items = item.SubItems.Values.ToList();
        //}
        var item = Items[Tag];
        SettingsWindowUI.Titles.Add(item.Name + " 的子项");
        this.Frame.Navigate(typeof(FixedItemListPage), item.SubItems, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void Action_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((SettingsCard)sender).Tag;
        //var item = Items.FirstOrDefault(i => i.ID == Tag);
        //if (item != null)
        //{
        //    var items = item.Actions;
        //}
        var item = Items[Tag];
        SettingsWindowUI.Titles.Add(item.Name + " 的操作");
        this.Frame.Navigate(typeof(ActionListPage), item.Actions, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
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
}