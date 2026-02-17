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
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace TouchHelper.Settings.SideBar;

public sealed partial class EditActionPage : Page
{
    public EditActionPage() => InitializeComponent();

    private Action Action { get; set; } = new();
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Action action)
        {
            Action.App = action.App;
            Action.Argument = action.Argument;
            Action.Command = action.Command;
            Action.DisplayName = action.DisplayName;
            Action.ID = action.ID;
            //Action.ParentID = action.ParentID;
            Action.Type = action.Type;
            Action.RunAppx = action.RunAppx;
        }
    }

    int option = 1;
    private void Option1_RadioButton_Click(object sender, RoutedEventArgs e)
    {
        option = 1;
        OptionNot3();
    }

    private void Option2_RadioButton_Click(object sender, RoutedEventArgs e)
    {
        option = 2;
        OptionNot3();
    }

    private void OptionNot3()
    {
        Action.RunAppx = false;
        appView.Visibility = Visibility.Collapsed;
        commandBox.Visibility = browserBtn.Visibility = Visibility.Visible;
        Action.App.Name = Action.App.Publisher = "";
        Action.App.Logo = null;
        appIcon.Source = null;
        Action.App.Name = appName.Text = Action.App.Publisher = appPublisher.Text = "";
    }

    private void Option3_RadioButton_Click(object sender, RoutedEventArgs e)
    {
        option = 3;
        Action.RunAppx = true;
        commandBox.Visibility = browserBtn.Visibility = Visibility.Collapsed;
        appView.Visibility = Visibility.Visible;
    }

    private async void SelectApp_Button_Click(object sender, RoutedEventArgs e)
    {
        SelectAppXDialog dialog = new()
        {
            XamlRoot = this.XamlRoot,
            IsSingleSelect = true
        };
        var result = await dialog.ShowAsync();
        if(result == ContentDialogResult.Primary && dialog.SelectApps.Count>0)
        {
            var app = dialog.SelectApps[0];
            Action.App.Name = appName.Text = app.Name;
            Action.App.Publisher = appPublisher.Text = app.Publisher;
            Action.App.Logo = app.Logo;
            appIcon.Source = app.Logo;
            Action.RunAppx = true;
        }
    }

    private void CommandBox_LostFocus(object sender, RoutedEventArgs e)
    {
        Action.Command = commandBox.Text;
    }

    private void Display_TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        Action.DisplayName = displayBox.Text;
    }

    private async void Browser_Click(object sender, RoutedEventArgs e)
    {
        if(option==1)
        {
            FileOpenPicker picker = new(this.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                CommitButtonText = "选择"
            };
            var file = await picker.PickSingleFileAsync();
            if (file != null)
                commandBox.Text = Action.Command = file.Path;
        }
        else
        {
            FolderPicker picker = new(this.XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                ViewMode = PickerViewMode.List
            };
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
                commandBox.Text = Action.Command = folder.Path;
        }
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsWindowUI.Titles.RemoveAt(SettingsWindowUI.Titles.Count - 1);
        this.Frame.GoBack();
    }
}