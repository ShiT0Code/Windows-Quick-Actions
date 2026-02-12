using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    }*/

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
    }*/

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


[JsonSerializable(typeof(List<MainAction>))]
[JsonSourceGenerationOptions(WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AppJsonContext : JsonSerializerContext;

public class ActionDataManager
{
    Windows.Storage.StorageFolder localFolder =
        Windows.Storage.ApplicationData.Current.LocalFolder;

    public async Task SaveActionsAsync(List<MainAction> actions)
    {
        try
        {
            var file = await localFolder.CreateFileAsync("actions.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            string json = JsonSerializer.Serialize(actions, AppJsonContext.Default.ListMainAction);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);
        }
        catch(FileNotFoundException)
        {
            _ = await localFolder.CreateFileAsync("actions.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
            await SaveActionsAsync(actions);
        }
        catch(Exception ex)
        {
            // 处理其他可能的异常
            System.Diagnostics.Debug.WriteLine($"保存失败: {ex.Message}");
        }
    }

    public async Task<List<MainAction>> LoadActionsAsync()
    {
        try
        {
            var file = await localFolder.GetFileAsync("actions.json");
            string json = await Windows.Storage.FileIO.ReadTextAsync(file);
            return JsonSerializer.Deserialize(json, AppJsonContext.Default.ListMainAction) ?? [];
        }
        catch(FileNotFoundException)
        {
            // 文件不存在，返回一个空列表
            return [];
        }
        catch(Exception ex)
        {
            // 处理其他可能的异常
            System.Diagnostics.Debug.WriteLine($"加载失败: {ex.Message}");
            return [];
        }
    }
}


//public partial class FixItem : ObservableObject
//{
//    [ObservableProperty]
//    private string _name = "";
//    //[ObservableObject]

//}