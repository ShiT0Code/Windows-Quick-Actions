using Microsoft.UI.Xaml;

namespace TouchQuickAction;
public sealed partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        SetTitleBar(titleBar);
    }
}
