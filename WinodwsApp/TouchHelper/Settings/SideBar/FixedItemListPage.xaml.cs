using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    }
    private Dictionary<string, FixedItem> Items = [];

    private void AddButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string name = newTextBox.Text;
        if (!string.IsNullOrWhiteSpace(name))
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
            _ = DataContainer.SaveFixedItems();
        }
    }

    private void Del_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((Button)sender).Tag;
        var item = Items[Tag];
        itemsControl.Items.Remove(item);
        Items.Remove(Tag);
        _ = DataContainer.SaveFixedItems();
    }


    private void SubItem_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((SettingsCard)sender).Tag;
        var item = Items[Tag];
        SettingsWindowUI.Titles.Add(item.Name + " 的子项");
        this.Frame.Navigate(typeof(FixedItemListPage), item.SubItems, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private void Action_SettingsCard_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string Tag = (string)((SettingsCard)sender).Tag;
        var item = Items[Tag];
        SettingsWindowUI.Titles.Add(item.Name + " 的操作");
        this.Frame.Navigate(typeof(ActionListPage), item.Actions, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
    }

    private async void UI_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Delay(500);
        foreach (var item in Items)
        {
            await Task.Delay(1);
            itemsControl.Items.Insert(0, item.Value);
        }
        await Task.Delay(100);
        progressBar.IsIndeterminate = false;
        progressBar.Visibility = Visibility.Collapsed;
    }

    private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        await Task.Delay(10);
        _ = DataContainer.SaveFixedItems();
    }

    private async void BrowerImage_Button_Click(object sender, RoutedEventArgs e)
    {
        FileOpenPicker picker = new(this.XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            ViewMode = PickerViewMode.List,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeFilter = { ".jpg", ".jpeg", ".png", ".bmp",".dib", ".gif",".svg",".ico",".tif",".tiff",".heic",".hif",".avif",".webp" }
        };
        var file = await picker.PickSingleFileAsync();
        if(file !=null)
        {
            string path = file.Path;
            string Tag = (string)((Button)sender).Tag;
            Items[Tag].IconUri = path;
            _ = DataContainer.SaveFixedItems();
        }
    }
}