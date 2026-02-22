using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace TouchHelper.DataCore;
public class DataContainer
{
    public static Dictionary<string, FixedItem> RootFixedItem { get; set; } = [];
    public static bool IsFixedItemsLoaded { get; set; } = false;

    public static List<ExclusionApp> ExclusionApps { get; set; } = [];
    public static bool IsExclusionAppsLoaded { get; set; } = false;

    private static StorageFolder LocalFolder =
        Windows.Storage.ApplicationData.Current.LocalFolder;

    public static async Task SaveFixedItems()
    {
        Debug.WriteLine("正在保存。。。");
        try
        {
            string json = JsonSerializer.Serialize(RootFixedItem, AppJsonContext.Default.DictionaryStringFixedItem);
            StorageFile file = await LocalFolder.CreateFileAsync("fixedItems.json", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, json);
        }
        catch (Exception ex)
        {
            Debug.Write("保存固定Item时发生错误：" + ex.Message);
        }
    }
    public static async Task LoadFixedItems()
    {
        Debug.WriteLine("正在加载。。。");
        try
        {
            StorageFile file = await LocalFolder.GetFileAsync("fixedItems.json");
            string json = await FileIO.ReadTextAsync(file);
            RootFixedItem = JsonSerializer.Deserialize(json, AppJsonContext.Default.DictionaryStringFixedItem) ?? [];
            IsFixedItemsLoaded = true;
        }
        catch (FileNotFoundException)
        {
            RootFixedItem = [];
            IsFixedItemsLoaded = true;
        }
        catch (Exception ex)
        {
            Debug.Write("加载固定Item时发生错误：" + ex.Message);
            RootFixedItem = [];
        }
    }

    public static async Task SaveExclusionApps()
    {
        try
        {
            string json = JsonSerializer.Serialize(ExclusionApps, AppJsonContext.Default.ListExclusionApp);
            StorageFile file = await LocalFolder.CreateFileAsync("exclusionApps.json", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, json);
        }
        catch (Exception ex)
        {
            Debug.Write("保存固定Item时发生错误：" + ex.Message);
        }
    }
    public static async Task LoadExclusionApps()
    {
        try
        {
            StorageFile file = await LocalFolder.GetFileAsync("exclusionApps.json");
            string json = await FileIO.ReadTextAsync(file);
            ExclusionApps = JsonSerializer.Deserialize(json, AppJsonContext.Default.ListExclusionApp) ?? [];
            IsExclusionAppsLoaded = true;
            Debug.WriteLine(ExclusionApps.Count);
        }
        catch (FileNotFoundException)
        {
            ExclusionApps = [];
            IsExclusionAppsLoaded = true;
        }
        catch (Exception ex)
        {
            Debug.Write("加载固定Item时发生错误：" + ex.Message);
            ExclusionApps = [];
        }
    }

}

[JsonSerializable(typeof(Dictionary<string, FixedItem>))]
[JsonSerializable(typeof(List<ExclusionApp>))]
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class AppJsonContext : JsonSerializerContext;