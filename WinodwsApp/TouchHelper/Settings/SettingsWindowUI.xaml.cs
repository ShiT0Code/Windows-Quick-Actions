using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace TouchHelper.Settings;
public sealed partial class SettingsWindowUI : Grid
{
    public static ObservableCollection<string> Titles { get; set; } = [];

    public SettingsWindowUI()
    {
        InitializeComponent();
        Titles.CollectionChanged += Titles_CollectionChanged;
    }

    private void Titles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        breadcrumbBar.ItemsSource = Titles.ToArray();
    }

    private async void Frame_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Task.Delay(50);
        Titles.Add("设置");
        frame.Navigate(typeof(SettingsHomePage));
    }

    private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        string item = (string)args.Item;
        int index = Titles.IndexOf(item);
        int count = Titles.Count;
        if (index == -1 || count == 1) return;
        while (Titles.Count > index + 1) Titles.RemoveAt(Titles.Count - 1);
        for (int i = 0; i < count - index - 1; i++)
            if (frame.CanGoBack)
                frame.GoBack();
    }
}
