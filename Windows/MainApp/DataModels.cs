using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
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

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ExeceteActionUI))]
public class ExeceteActionUI()
{
    public int ActionType { get; set; } = 0; // 0: 运行程序, 1: 打开文件, 2: 打开文件夹, 3: 运行命令
    public string Command { get; set; } = "";
    public string Arguments { get; set; } = "";
    public string ID { get; set; } = System.Guid.NewGuid().ToString();
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