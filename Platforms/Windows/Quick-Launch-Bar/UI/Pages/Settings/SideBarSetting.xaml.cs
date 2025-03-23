using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System;
using System.IO;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Quick_Launch_Bar.UI.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SideBarSetting : Page
    {
        public SideBarSetting()
        {
            this.InitializeComponent();

            ViewModel = new SideBarSettingsViewModel();
        }

        public SideBarSettingsViewModel ViewModel { get; set; }

        public static bool IsLoadedLeft { get; set; } = false;


        private void LoadInfo(bool IsEe, string name, string des, string ic_path, int style)
        {
            ViewGrid.Children.Clear();

            ToggleSwitch toggleSwitch = new ToggleSwitch()
            {
                IsOn = IsEe,
                OnContent = "启用",
                OffContent = "禁用"
            };
            toggleSwitch.Toggled += ItemIsEnable_ToggleSwitch_Toggled;

            TextBox textBox1 = new TextBox()
            {
                Text = name,
                Header = "名称："
            };
            textBox1.LostFocus += TextBox1_LostFocus;

            TextBox textBox2 = new TextBox()
            {
                Text = des,
                Header = "描述："
            };
            textBox2.LostFocus += TextBox2_LostFocus;

            ComboBox comboBox = new()
            {
                Header = "选择显示样式",
                SelectedIndex = style
            };
            comboBox.Items.Add(new ComboBoxItem() { Content = "普通按钮" });
            comboBox.Items.Add(new ComboBoxItem() { Content = "链接按钮" });
            comboBox.Items.Add(new ComboBoxItem() { Content = "下拉按钮" });
            comboBox.Items.Add(new ComboBoxItem() { Content = "拆分按钮" });

            comboBox.SelectionChanged += Style_ComboBox_SelectionChanged;

            Microsoft.UI.Xaml.Controls.Image image = new()
            {
                Width = 48,
                Height = 48
            };
            if(File.Exists(ic_path))
                image.Source = new BitmapImage(new Uri(ic_path));

            TextBox textBox3 = new TextBox()
            {
                Text = ic_path,
                Header = "图标路径",
                MinWidth = 250
            };
            textBox3.LostFocus += TextBox3_LostFocus;

            var IconEditSP = new StackPanel()
            {
                Spacing = 12,
                Orientation = Orientation.Horizontal
            };
            IconEditSP.Children.Add(image);
            IconEditSP.Children.Add(textBox3);

            var BasicStackPanel = new StackPanel()
            {
                Spacing = 8,
                MaxWidth = 850,
                Orientation=Orientation.Vertical
            };
            BasicStackPanel.Children.Add(toggleSwitch);
            BasicStackPanel.Children.Add(textBox1);
            BasicStackPanel.Children.Add(textBox2);
            BasicStackPanel.Children.Add(comboBox);
            BasicStackPanel.Children.Add(IconEditSP);

            ViewGrid.Children.Add(BasicStackPanel);
        }

        private void Style_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            item.style = comboBox.SelectedIndex;
        }

        private void TextBox3_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            item.iconPath = textBox.Text;
        }

        private void TextBox2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            item.description = textBox.Text;
        }

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (item.name != textBox.Text)
            {
                item.name = textBox.Text;

                SideBarList.ItemsSource = null;
                SideBarList.ItemsSource = ViewModel.Items;
                SideBarList.SelectedItem = item;
            }
        }

        private void ItemIsEnable_ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            item.isEnable = toggleSwitch.IsOn;
        }


        SideBarItem item = new SideBarItem();
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SideBarList.SelectedItem is SideBarItem selectedItem)
            {
                bool IsEe = selectedItem.isEnable;
                string name = selectedItem.name;
                string des = selectedItem.description;
                string ic_path = selectedItem.iconPath;
                int style = selectedItem.style;

                add_A.IsEnabled = ActionList.IsEnabled = del_I.IsEnabled = true;

                ActionList.ItemsSource = selectedItem.actions;

                LoadInfo(IsEe, name, des, ic_path, style);

                item = selectedItem;
            }
        }


        private async void DelItem_Button_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Title = "确认删除该项？",
                Content = "你确定要删除该项吗？",
                CloseButtonText = "取消",
                PrimaryButtonText = "确认删除",
                DefaultButton = ContentDialogButton.Primary
            };
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Items.Remove(item);

                ActionList.IsEnabled = add_A.IsEnabled = false;

                ViewGrid.Children.Clear();

                if(ViewModel.Items.Count==0)
                    new SettingsManager().SaveBoolSetting("IsNoneItem", true);
            }
        }

        private void AddItem_HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            SideBarItem sideBarItem = new SideBarItem()
            {
                name = "新项"
            };
            ViewModel.Items.Add(sideBarItem);
        }


        SideBarItemAction action = new SideBarItemAction();
        private void ActionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ActionList.SelectedItem is SideBarItemAction selectedItem)
            {
                action = selectedItem;

                del_I_A.IsEnabled = true;
            }
        }
        private async void DelAction_HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = this.XamlRoot,
                Title = "确认删除该操作？",
                Content = "你确定要删除该操作吗？",
                CloseButtonText = "取消",
                PrimaryButtonText = "确认删除",
                DefaultButton = ContentDialogButton.Primary
            };
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                item.actions.Remove(action);

                del_I_A.IsEnabled = false;
            }
        }

        private void AddAction_HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            SideBarItemAction action = new SideBarItemAction()
            {
                title1 = "操作"
            };
            item.actions.Add(action);
        }

        private void SideBarList_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void LoadData()
        {
            SideBarSettingsViewModel sidebarSettingsViewModel = await new SettingsManager().LoadFromJsonFileAsync<SideBarSettingsViewModel>("SideBarItem.json");

            if (sidebarSettingsViewModel != null)
            {
                ViewModel.Items.Clear();
                foreach (var item in sidebarSettingsViewModel.Items)
                {
                    ViewModel.Items.Add(item);
                }
            }
            else if(new SettingsManager().CheckBoolSetting("IsNoneItem"))
            {
                ;
            }
            else
            {
                await new ContentDialog()
                {
                    XamlRoot = this.XamlRoot,
                    Title = "警告",
                    Content = "设置无法正确加载！",
                    CloseButtonText = "确定"
                }.ShowAsync();
            }
        }

        private async void SaveAll_Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Task.Delay(250);
            await new SettingsManager().SaveViewModelToJsonFileAsync(ViewModel, "SideBarItem.json");
        }

        private void GiveUp_Button_Click_1(object sender, RoutedEventArgs e)
        {
            ViewGrid.Children.Clear();

            ActionList.IsEnabled = add_A.IsEnabled = del_I.IsEnabled = del_I_A.IsEnabled = false;

            SideBarList.ItemsSource = null;

            LoadData();
            SideBarList.ItemsSource = ViewModel.Items;
        }

        private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {

        }
    }


    public class SideBarItem
    {
        public string name { get; set; } = "";

        public string description { get; set; } = "";

        public int style { get; set; } = 0;

        public bool isEnable { get; set; } = true;
        public string iconPath { get; set; } = "";
        public ObservableCollection<SideBarItemAction> actions { get; set; } = new ObservableCollection<SideBarItemAction>();
    }

    public class SideBarItemAction
    {
        public string title1 { get; set; } = "";

        public string description1 { get; set; } = "";

        public string action1 { get; set; } = "";

        public bool isEnable1 { get; set; } = true;

        public bool isShowNot { get; set; } = true;

        public int actionTimes { get; set; } = 1;
    }
    public class SideBarSettingsViewModel
    {
        public ObservableCollection<SideBarItem> Items { get; set; }=new ObservableCollection<SideBarItem>();
    }
}