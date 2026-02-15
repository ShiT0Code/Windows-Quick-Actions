using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace TouchHelper.Settings;
public sealed partial class SelectAppXDialog : ContentDialog
{
    public SelectAppXDialog() => InitializeComponent();

    private List<AppXItem> Apps = [];
    public List<AppXItem> SelectApps { get; set; } = [];
    private readonly string appFamilyName = Package.Current.Id.FamilyName;

    private void CheckBox_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CheckBox checkBox = (CheckBox)sender;
        string Tag = (string)checkBox.Tag;
        var app = Apps.FirstOrDefault(a => a.FamilyName == Tag);
        if (app != null)
        {
            if (checkBox.IsChecked == true)
                SelectApps.Add(app);
            else
                SelectApps.Remove(app);
        }
        this.IsPrimaryButtonEnabled = SelectApps.Count != 0;
    }

    private async void UI_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
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
                            Name = package.DisplayName,
                            FamilyName = package.Id.FamilyName,
                            Publisher = package.PublisherDisplayName
                        };
                        try
                        {
                            _ = File.Exists(package.Logo.AbsolutePath);
                            DispatcherQueue.TryEnqueue(() => app.Logo = new(package.Logo));
                        }
                        catch { }
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Apps.Add(app);
                            itemsControl.Items.Add(app);
                        });
                    }
                }
                catch (OperationCanceledException) { Debug.WriteLine("Cancel"); }
                catch { }
            }
        });
        itemsControl.IsEnabled = true;
        progressBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        progressBar.IsIndeterminate = false;
    }
}