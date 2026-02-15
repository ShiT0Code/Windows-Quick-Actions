using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TouchHelper.Settings.SideBar;
public sealed partial class ExclusionAppsPage : Page
{
    public ExclusionAppsPage() => InitializeComponent();

    private List<ExclusionApp> Items = [];

    private async void AddWin32_Button_Click(object sender, RoutedEventArgs e)
    {
        Button button = (Button)sender;
        FileOpenPicker picker = new(button.XamlRoot.ContentIslandEnvironment.AppWindowId)
        {
            ViewMode = PickerViewMode.List,
            SuggestedStartLocation = PickerLocationId.ComputerFolder
        };
        picker.FileTypeFilter.Add(".exe");
        var files = await picker.PickMultipleFilesAsync();
        progressBar.IsIndeterminate = true;
        progressBar.Visibility = Visibility.Visible;
        foreach (var file in files)
        {
            await Task.Delay(10); // 防止文件太多导致界面卡死
            string path = file.Path;
            int lastSlash = path.LastIndexOf('\\');
            int lastDot = path.LastIndexOf('.');
            string name = path.Substring(lastSlash + 1, lastDot - lastSlash - 1);
            ExclusionApp app = new()
            {
                Name = name,
                Command = file.Path
            };
            Items.Add(app);
            itemsControl.Items.Add(app);
        }
        progressBar.Visibility = Visibility.Collapsed;
        progressBar.IsIndeterminate = false;
    }

    private async void AddAppX_Button_Click(object sender, RoutedEventArgs e)
    {
        SelectAppXDialog? dialog = new()
        {
            XamlRoot = this.XamlRoot
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            progressBar.IsIndeterminate = true;
            progressBar.Visibility = Visibility.Visible;
            foreach (var a in dialog.SelectApps)
            {
                await Task.Delay(10); // 防止应用太多导致界面卡死
                ExclusionApp app = new()
                {
                    Name = a.Name,
                    Command = $"shell:AppsFolder\\{a.FamilyName}",
                    IsEditable = false
                };
                Items.Add(app);
                itemsControl.Items.Add(app);
            }
        }
        dialog = null;
        //_ = dataReaderW.SaveExclusionAppsAsync(Items);
        progressBar.Visibility = Visibility.Collapsed;
        progressBar.IsIndeterminate = false;
    }

    private void DelItem_Button_Click(object sender, RoutedEventArgs e)
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

    private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        await Task.Delay(20);
        foreach (var i in Items)
            Debug.WriteLine(i.Name);
        //
    }
}
