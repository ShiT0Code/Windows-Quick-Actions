using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace TouchHelper.Settings.SideBar;
public sealed partial class EditActionDialog : ContentDialog
{
    public EditActionDialog() => InitializeComponent();

    public bool IsSelectingAppx { get; set; } = false;
    public AppXItem? AppX { get; set; } = null;

    public Action Action { get; set; } = new();

    private int Index { get; set; } = 0;

    private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = comBox.SelectedIndex;
        this.Index = index;
        argumentTextBox.Visibility = (index == 2) ? Visibility.Visible : Visibility.Collapsed;
        if (index != 3)
        {
            locationView.Visibility = Visibility.Visible;
            appXView.Visibility = Visibility.Collapsed;

            locationTextBox.Header = index switch
            {
                0 => "文件路径",
                1 => "文件夹路径",
                2 => "可执行文件（.exe）路径",
                4 => "命令",
                5 => "链接",
                _ => "位置"
            };
            locationTextBox.PlaceholderText = index switch
            {
                0 => "C:\\exmaple\\path\\txt.txt",
                1 => "C:\\Windows",
                2 => "C:\\Windows\\explorer.exe",
                4 => "powershell -Command \"winver;explorer\"",
                5 => "https://bilibili.com",
                _ => ""
            };
            browserButton.Content = (index == 4) ? "浏览命令的执行程序" : "浏览";
            browserButton.Visibility = (index == 5) ? Visibility.Collapsed : Visibility.Visible;
        }
        else
        {
            locationView.Visibility = Visibility.Collapsed;
            appXView.Visibility = Visibility.Visible;
        }
    }

    private void SelectAppx_Button_Click(object sender, RoutedEventArgs e)
    {
        IsSelectingAppx = true;
        this.Hide();
    }

    private void UI_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsSelectingAppx)
        {
            comBox.SelectedIndex = 3;
            if (AppX != null)
            {
                AppXName.Text = AppX.Name;
                AppXPublisher.Text = AppX.Publisher;
                appIcon.Source = AppX.Logo;
            }
        }
        else
            comBox.SelectedIndex = 0;
        IsSelectingAppx = false;
    }

    private async void BrowserButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.Index == 0 || this.Index == 2 || this.Index == 4)
        {
            FileOpenPicker picker = new(browserButton.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                CommitButtonText = "选择"
            };
            if (this.Index != 0)
            {
                picker.FileTypeFilter.Add(".exe");
                picker.FileTypeFilter.Add(".pif");
                picker.FileTypeFilter.Add(".com");
                picker.FileTypeFilter.Add(".bat");
                picker.FileTypeFilter.Add(".cmd");
            }
            var file = await picker.PickSingleFileAsync();
            if (file != null)
                locationTextBox.Text = file.Path;
        }
        else if(this.Index == 1)
        {
            FolderPicker picker = new(browserButton.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.List
            };
            var folder = await picker.PickSingleFolderAsync();
            if(folder!=null)
                locationTextBox.Text = folder.Path;
        }
    }
}