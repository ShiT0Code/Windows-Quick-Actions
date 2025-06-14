using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Quick_Launch_Bar.UI.Pages.Settings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Quick_Launch_Bar.UI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllSettingsWindow : Window
    {
        public AllSettingsWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        }

        public static int Index { get; set; } = 0;

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            Nav.SelectedItem = Nav.MenuItems[0];
        }

        private void Nav_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            string selectedItemTag = ((string)selectedItem.Tag);

            HeaderText.Text = ((string)selectedItem.Content);

            switch (selectedItemTag)
            {
                case "About":
                    ContentFrame.Navigate(typeof(About), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    break;
                case "SideBar":
                    ContentFrame.Navigate(typeof(SideBarSetting), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    break;
            }

            Index = -1;
        }

        private void ContentFrame_Navigating(object sender, Microsoft.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
            if (Index != -1)
                Nav.SelectedItem = Nav.MenuItems[Index];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Closed += ExitApp;
            Close();
        }

        private void ExitApp(object sender, WindowEventArgs args)
        {
            App.Current.Exit();
        }
    }
}
