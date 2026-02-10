using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
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

    public List<SubAction> SubActions { get; set; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
public partial class SubAction : INotifyPropertyChanged
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
                OnPropertyChanged();
            }
        }
    }

    private List<ExeceteActionUI> _execeteActions = [];
    public List<ExeceteActionUI> ExeceteActions
    {
        get => _execeteActions;
        set
        {
            if (_execeteActions != value)
            {
                _execeteActions = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}


[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MainActionListViewItem))]
public class MainActionListViewItem()
{
    public string Title { get; set; } = "";
    public BitmapImage? Icon { get; set; }
}

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ExeceteActionUI))]
public class ExeceteActionUI()
{
    public int ActionType { get; set; } = 0; // 0: 运行程序, 1: 打开文件, 2: 打开文件夹, 3: 运行命令
    public string Command { get; set; } = "";
    public string Arguments { get; set; } = "";
}