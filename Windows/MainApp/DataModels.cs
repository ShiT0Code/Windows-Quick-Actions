using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MainApp;

public partial class MainAction: INotifyPropertyChanged
{
    private string _title { get; set; } = "";
    public string Title 
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();  // 通知UI更新
            }
        }
    }

    private BitmapImage? _icon { get; set; }
    public BitmapImage? Icon
    {
        get => _icon;
        set
        {
            if (_icon != value)
            {
                _icon = value;
                OnPropertyChanged();  // 通知UI更新
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
public class SubAction
{
}

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MainActionListViewItem))]
public class MainActionListViewItem()
{
    public string Title { get; set; } = "";
    public BitmapImage? Icon { get; set; }
}