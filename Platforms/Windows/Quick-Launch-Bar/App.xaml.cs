using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Quick_Launch_Bar.UI;
using Windows.ApplicationModel.Activation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Quick_Launch_Bar
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
        public App()
#pragma warning restore CS8618 // 不会的！
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 获取当前 AppInstance
            var appInstance = AppInstance.GetCurrent();

            // 获取激活参数
            var activatedArgs = appInstance.GetActivatedEventArgs();
            // 检查是否通过协议启动
            if (activatedArgs != null && activatedArgs.Kind == ExtendedActivationKind.Protocol)
            {
                // 处理协议启动
                var protocolArgs = activatedArgs.Data as ProtocolActivatedEventArgs;
#pragma warning disable CS8602 // 解引用可能出现空引用。
                string uri = protocolArgs.Uri.AbsoluteUri;
#pragma warning restore CS8602 // 不会的！

                // 处理协议参数
                HandleProtocolLaunch(uri);
            }
            else
            {
                NormalBoot();
#pragma warning disable CS8602 // 解引用可能出现空引用。
                m_window.Activate();
#pragma warning restore CS8602 
            }
        }

        public  string[] parts { get; set; }
        private void HandleProtocolLaunch(string uri)
        {
            // 解析协议参数并执行相应操作
            if (uri.StartsWith("shi-qlb://"))
            {
                string parameter = uri.Substring("shi-qlb://".Length);

                // 根据参数执行操作
                parts = parameter.Split('/');
                string firstPart = parts[0];

                    if (firstPart == "settings")
                        new AllSettingsWindow().Activate();
                    else if (firstPart == "sidebar" && new SettingsManager().CheckBoolSetting("IsSideBarOn"))
                        new SideBarWindow().Activate();
                    else if (firstPart == "none")
                        NormalBoot();
                    else
                        new AllSettingsWindow().Activate();
            }
        }

        private void NormalBoot()
        {
            bool Actioned = false;
            if (new SettingsManager().CheckBoolSetting("IsSideBarOn"))
            {
                m_window = new SideBarWindow();
                Actioned = true;
            }
            if (!Actioned)
                m_window = new AllSettingsWindow();
            else
            {
                var Notif = new AppNotificationBuilder()
                    .AddText("快速启动栏已启动！");
                AppNotificationManager.Default.Show(Notif.BuildNotification());
            }
        }

        private Window? m_window;
    }
}
