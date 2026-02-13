using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MainApp;

[JsonSerializable(typeof(List<FixItemJson>))]
[JsonSerializable(typeof(List<ExclusionAppJson>))]
[JsonSourceGenerationOptions(WriteIndented = true)]
public partial class AppJsonContext : JsonSerializerContext;

public class FixItemDataReaderW
{
    Windows.Storage.StorageFolder localFolder =
        Windows.Storage.ApplicationData.Current.LocalFolder;

    public async Task SaveActionsAsync(List<FixItem> fixItems)
    {
        try
        {
            List<FixItemJson> itemJsons = [];
            foreach (var item in fixItems)
            {
                List<SubFixItemJson> subItemJsons = [];
                foreach(var subItem in item.SubItems)
                {
                    List<ExecutionItemJson> executionJsons = [];
                    foreach(var execution in subItem.Actions)
                    {
                        executionJsons.Add(new()
                        {
                            ID = execution.ID,
                            Command = execution.Command,
                            Arguments = execution.Arguments,
                            ActionType = execution.ActionType
                        });
                    }
                    subItemJsons.Add(new()
                    {
                        ID = subItem.ID,
                        Name = subItem.Name,
                        Actions = executionJsons
                    });
                }
                itemJsons.Add(new()
                {
                    Name = item.Name,
                    ID = item.ID,
                    SubItems = subItemJsons
                });
            }
            var file = await localFolder.CreateFileAsync("FixedItem.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            string json = JsonSerializer.Serialize(itemJsons, AppJsonContext.Default.ListFixItemJson);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);
        }
        catch (FileNotFoundException)
        {
            _ = await localFolder.CreateFileAsync("FixedItem.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
            await SaveActionsAsync(fixItems);
        }
        catch(IOException)
        {
            await Task.Delay(3000);
            await SaveActionsAsync(fixItems);
        }
        catch (Exception ex)
        {
            // 处理其他可能的异常
            System.Diagnostics.Debug.WriteLine($"保存失败: {ex.Message}");
        }
    }

    public async Task<List<FixItem>> LoadActionsAsync()
    {
        try
        {
            var file = await localFolder.GetFileAsync("FixedItem.json");
            string json = await Windows.Storage.FileIO.ReadTextAsync(file);
            var items = JsonSerializer.Deserialize(json, AppJsonContext.Default.ListFixItemJson) ?? [];
            List<FixItem> fixItems = [];
            foreach(var item in items)
            {
                ObservableCollection<SubFixItem> subFixItems = [];
                foreach(var subItem in item.SubItems)
                {
                    ObservableCollection<ExecutionItem> executionItems = [];
                    foreach(var execution in subItem.Actions)
                    {
                        executionItems.Add(new()
                        {
                            ID = execution.ID,
                            Command = execution.Command,
                            Arguments = execution.Arguments,
                            ActionType = execution.ActionType
                        });
                    }
                    subFixItems.Add(new()
                    {
                        Name = subItem.Name,
                        ID = subItem.ID,
                        Actions = executionItems
                    });
                }
                fixItems.Add(new()
                {
                    Name = item.Name,
                    ID = item.ID,
                    SubItems = subFixItems
                });
            }
            return fixItems;
        }
        catch (FileNotFoundException)
        {
            // 文件不存在，返回一个空列表
            return [];
        }
        catch (Exception ex)
        {
            // 处理其他可能的异常
            System.Diagnostics.Debug.WriteLine($"加载失败: {ex.Message}");
            return [];
        }
    }
}
