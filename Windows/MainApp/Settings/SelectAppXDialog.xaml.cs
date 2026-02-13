using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace MainApp.Settings;
public sealed partial class SelectAppXDialog : ContentDialog
{
    public SelectAppXDialog() => InitializeComponent();

    public List<AppXItem> SelectApps { get; set; } = [];
    private List<AppXItem> Apps = [];

    private void RefreshButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => _ = Refresh();

    private async void UI_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(300);
        _ = Refresh();
    }

    private readonly string appFamilyName = Package.Current.Id.FamilyName;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    private async Task Refresh()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            searchBox.Text = "";
            refreshButton.IsEnabled = searchBox.IsEnabled = itemsControl.IsEnabled = false;
            itemsControl.Items.Clear();
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            progressBar.IsIndeterminate = true;
        });

        await Task.Run(() =>
        {
            var packages = new PackageManager().FindPackagesForUser("");
            foreach (var package in packages)
            {
                try
                {
                    var appEntries = package.GetAppListEntries();
                    if (appEntries.Any() && package.Id.FamilyName != appFamilyName)
                    {
                        var app = new AppXItem()
                        {
                            DisplayName = package.DisplayName,
                            FamilyName = package.Id.FamilyName,
                            Publisher = package.PublisherDisplayName
                        };
                        DispatcherQueue.TryEnqueue(() => 
                        { 
                            app.Logo = new(package.Logo);
                            itemsControl.Items.Add(app);
                            Apps.Add(app);
                        });
                    }
                }
                catch { }
            }
        });

        DispatcherQueue.TryEnqueue(() =>
        {
            refreshButton.IsEnabled = searchBox.IsEnabled = itemsControl.IsEnabled = true;
            progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            progressBar.IsIndeterminate = false;
        });
    }

    private void Select_CheckBox_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender;
        string Tag = (string)checkBox.Tag;
        var app = Apps.FirstOrDefault(a => a.FamilyName == Tag);
        if(app != null)
        {
            if (checkBox.IsChecked == true)
                SelectApps.Add(app);
            else
                SelectApps.Remove(app);
        }
        this.IsPrimaryButtonEnabled = SelectApps.Count != 0;
    }


    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        
    }

    private async void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            string text = searchBox.Text;
            List<string> matches = [];
            foreach (var app in Apps)
            {
                if ((app.DisplayName.Contains(text) || app.Publisher.Contains(text) || app.FamilyName.Contains(text)) &&
                    !matches.Contains(app.DisplayName))
                {
                    matches.Add(app.DisplayName);
                }
            }
            searchBox.ItemsSource = matches;
        }
    }

    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

    }

    private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {

    }
}