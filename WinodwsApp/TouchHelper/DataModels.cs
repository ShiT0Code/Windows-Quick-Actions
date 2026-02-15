using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TouchHelper;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppXItem))]
// 只要这一个，所有的数据模型都能在 Release 模式中正常工作（有点神奇
public class AppXItem()
{
    public string Name { get; set; } = "";
    public string FamilyName { get; set; } = "";
    public string Publisher { get; set; } = "";
    public BitmapImage? Logo { get; set; }
}

public class ExclusionApp : INotifyPropertyChanged
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            // 仅在值真正改变时更新并通知
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    private string _command = "";
    public string Command
    {
        get => _command;
        set
        {
            if (_command != value)
            {
                _command = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsEditable { get; set; } = true;
    public string ID { get; set; } = Guid.NewGuid().ToString();

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class FixedItem() : INotifyPropertyChanged
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            if(_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string ParentID { get; set; } = ""; 

    public List<FixedItem> SubItems { get; set; } = [];
    public List<Action> Actions { get; set; } = [];

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}

/*public class FixedSubItem : INotifyPropertyChanged
{
    private string _name = "";
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public string ID { get; set; } = Guid.NewGuid().ToString();


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}*/

public class Action : INotifyPropertyChanged
{
    private string _command = "";
    public string Command
    {
        get => _command;
        set
        {
            if (_command != value)
            {
                _command = value;
                OnPropertyChanged();
            }
        }
    }


    /*private string _argument = "";
    public string Argument
    {
        get => _argument;
        set
        {
            if (_command != value)
            {
                _command = value;
                OnPropertyChanged();
            }
        }
    }*/


    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string ParentID { get; set; } = "";

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler? PropertyChanged;
}