using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MainApp;
/*
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

    /*private BitmapImage? _icon { get; set; }
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
    public string ID { get; set; } = Guid.NewGuid().ToString();

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

    private ObservableCollection<ExeceteAction> _execeteActions { get; set; } = new ObservableCollection<ExeceteAction>();
    public ObservableCollection<ExeceteAction> ExeceteActions
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
    /*private List<ExeceteAction> _execeteActions = [];
    public List<ExeceteAction> ExeceteActions
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

    public string ID { get; set; } = Guid.NewGuid().ToString();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MainActionUI))]
public class MainActionUI()
{
    public string Title { get; set; } = "";
    public BitmapImage? Icon { get; set; }
    public string ID { get; set; } = "";
}

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SubActionUI))]
public class SubActionUI()
{
    public string Title { get; set; } = "";
    public string ID { get; set; } = "";
}

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ExeceteAction))]
public partial class ExeceteAction():INotifyPropertyChanged
{
    private int _actionType { get; set; } = 0;
    public int ActionType 
    {
        get => _actionType;
        set
        {
            if (_actionType != value)
            {
                _actionType = value;
                OnPropertyChanged();
            }
        }
    } // 0: 运行程序, 1: 打开文件, 2: 打开文件夹, 3: 运行命令
    private string _command { get; set; } = "";
    public string Command
    {
        get=> _command;
        set
        {
            if (_command != value)
            {
                _command = value;
                OnPropertyChanged();
            }
        }
    }
    private string _arguments { get; set; } = "";
    public string Arguments
    {
        get => _arguments;
        set
        {
            if (_arguments != value)
            {
                _arguments = value;
                OnPropertyChanged();
            }
        }
    }
    public string ID { get; set; } = System.Guid.NewGuid().ToString();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
*/

public partial class FixItem : ObservableObject
{
    [ObservableProperty]
    [property: JsonPropertyName("Name")]
    private string _name = "";
    [ObservableProperty]
    private ObservableCollection<SubFixItem> _subItems = [];
    public string ID { get; set; } = Guid.NewGuid().ToString(); // ID 不需要更改
}
public class FixItemJson
{
    public string Name { get; set; } = "";
    public string ID { get; set; } = "";
    public List<SubFixItemJson> SubItems { get; set; } = [];
}
[method:DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FixItemUI))]
public class FixItemUI()
{
    public string Name { get; set; } = "";
    public string ID { get; set; } = "";
}

public partial class SubFixItem : ObservableObject
{
    [ObservableProperty]
    private string _name = "";
    [ObservableProperty]
    private ObservableCollection<ExecutionItem> _actions = [];
    public string ID { get; set; } = Guid.NewGuid().ToString(); 
}
public class SubFixItemJson
{
    public string Name { get; set; } = "";
    public string ID { get; set; } = "";
    public List<ExecutionItemJson> Actions { get; set; } = [];
}
[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SubFixItemUI))]
public class SubFixItemUI()
{
    public string Name { get; set; } = "";
    public string ID { get; set; } = "";
}

public partial class ExecutionItem : ObservableObject
{
    [ObservableProperty]
    private string _command = "";
    [ObservableProperty]
    private string _arguments = "";
    [ObservableProperty]
    private int _actionType = 0; // 0: 运行程序, 1: 打开文件, 2: 打开文件夹, 3: 运行命令
    public string ID { get; set; } = Guid.NewGuid().ToString();
}
public class ExecutionItemJson
{
    public string ID { get; set; } = "";
    public string Command { get; set; } = "";
    public string Arguments { get; set; } = "";
    public int ActionType { get; set; } = 0; // 0: 运行程序, 1: 打开文件, 2: 打开文件夹, 3: 运行命令
}



public partial class ExclusionApp : ObservableObject
{
    [ObservableProperty]
    private string _command = "";
    [ObservableProperty]
    private string _exeFilePath = "";
    [ObservableProperty]
    private string _arguments = "";
    [ObservableProperty]
    private string _displayName = "";

    public string ID { get; set; } = Guid.NewGuid().ToString();
}
public class ExclusionAppJson
{
    public string Command { get; set; } = "";
    public string ExeFilePath { get; set; } = "";
    public string Arguments { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string ID { get; set; } = "";
}

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppXItem))]
public class AppXItem()
{
    public string DisplayName { get; set; } = "";
    public string FamilyName { get; set; } = "";
    public string Publisher { get; set; } = "";
    public BitmapImage? Logo { get; set; }
}