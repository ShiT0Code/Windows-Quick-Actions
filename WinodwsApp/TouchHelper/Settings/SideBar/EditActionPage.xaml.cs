using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;

namespace TouchHelper.Settings.SideBar;

public sealed partial class EditActionPage : Page
{
    public EditActionPage() => InitializeComponent();

    private Action Action { get; set; } = new();
    List<string> parentIDs = [];
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Action action)
        {
            foreach (var s in action.ParentID)
                parentIDs.Add(s);
            Action = action;
        }
    }

    int option = 0;
    private void Option0_RadioButton_Click(object sender, RoutedEventArgs e)
    {
        Action.Type = option = 0;
        OptionNot2();
    }

    private void Option1_RadioButton_Click(object sender, RoutedEventArgs e)
    {
        Action.Type = option = 1;
        OptionNot2();
    }

    private void OptionNot2()
    {
        Action.RunAppx = false;
        appView.Visibility = Visibility.Collapsed;
        commandBox.Visibility = browserBtn.Visibility = Visibility.Visible;
        Action.App.Name = Action.App.Publisher = "";
        Action.App.Logo = null;
        appIcon.Source = null;
        Action.App.Name = appName.Text = Action.App.Publisher = appPublisher.Text = "";
    }

    private void Option2_RadioButton_Click(object sender, RoutedEventArgs e)
    {
        Option2();
    }
    private void Option2()
    {
        Action.Type = option = 2;
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
        if (result == ContentDialogResult.Primary && dialog.SelectApps.Count > 0)
        {
            var app = dialog.SelectApps[0];
            Action.App.Name = appName.Text = app.Name;
            Action.App.Publisher = appPublisher.Text = app.Publisher;
            Action.App.Logo = app.Logo;
            appIcon.Source = app.Logo;
            Action.RunAppx = true;
            Action.Type = 2;
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
        if (option == 0)
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

    /*private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine(FixedCurrentItemData.RootFixedItem[parentIDs[0]].Actions[parentIDs[1]].DisplayName);
        Debug.WriteLine(Action.DisplayName);
        Debug.WriteLine(Action.Type);

        //*FixedCurrentItemData.RootFixedItem[parentIDs[0]].Actions[parentIDs[1]].Type = option;
        FixedCurrentItemData.RootFixedItem[parentIDs[0]].Actions[parentIDs[1]].DisplayName = Action.DisplayName;
        FixedCurrentItemData.RootFixedItem[parentIDs[0]].Actions[parentIDs[1]].Command = Action.Command;
        FixedCurrentItemData.RootFixedItem[parentIDs[0]].Actions[parentIDs[1]].Argument = Action.Argument;
        FixedCurrentItemData.RootFixedItem[parentIDs[0]].Actions[parentIDs[1]].App = Action.App*

        SettingsWindowUI.Titles.RemoveAt(SettingsWindowUI.Titles.Count - 1);
        this.Frame.GoBack();
    }*/

    private async void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        commandBox.Text = Action.Command;
        displayBox.Text = Action.DisplayName;
        appName.Text = Action.App.Name;
        appPublisher.Text = Action.App.Publisher;
        appIcon.Source = Action.App.Logo;
        selectBtns.SelectedIndex = option = Action.Type;
        if (option == 2)
            Option2();
        else
            OptionNot2();
    }

    private void Back_Button_Click(object sender, RoutedEventArgs e)
    {
        this.Frame.GoBack();
    }
}