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

    public static void Start()
    {
        FixedItem item1 = new()
        {
            Name = "Item 1",
            ID = "1"
        };
        FixedItem item2 = new()
        {
            Name = "Item 2",
            ID = "2"
        };
        FixedItem item3 = new()
        {
            Name = "Item 3",
            ID = "3"
        };
        FixedItem item4 = new()
        {
            Name = "Item 4",
            ID = "4"
        };

        Action action1 = new()
        {
            DisplayName = "A1",
            ID = "a1",
            ParentID = ["1", "a1"]
        };
        Action action2 = new()
        {
            DisplayName = "A2",
            ID = "a2",
            ParentID = ["1", "a2"]
        };
        Action action3 = new()
        {
            DisplayName = "A3",
            ID = "a3",
            ParentID = ["1", "3", "a3"]
        };
        Action action4 = new()
        {
            DisplayName = "A4",
            ID = "a4"
        };
        //return;
        item1.Actions.Add("a1", action1);
        item1.Actions.Add("a2", action2);
        item3.Actions.Add("a3", action3);
        item4.Actions.Add("a4", action4);

        item3.SubItems.Add("4",item4);
        item1.SubItems.Add("3",item3);
        RootFixedItem.Add("1",item1);
        RootFixedItem.Add("2",item2);
        Debug.WriteLine("已添加！！！");
    }

    private static Windows.Storage.StorageFolder LocalFolder =
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

    public static async Task LoadFixedJsons()
    {
        Debug.WriteLine("正在加载。。。");
        try
        {
            StorageFile file = await LocalFolder.GetFileAsync("fixedItems.json");
            string json = await FileIO.ReadTextAsync(file);
            RootFixedItem = JsonSerializer.Deserialize(json, AppJsonContext.Default.DictionaryStringFixedItem) ?? [];
        }
        catch(FileNotFoundException)
        {
            RootFixedItem = [];
        }
        catch(Exception ex)
        {
            Debug.Write("加载固定Item时发生错误：" + ex.Message);
            RootFixedItem = [];
        }
    }
}

[JsonSerializable(typeof(Dictionary<string, FixedItem>))]
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class AppJsonContext : JsonSerializerContext;