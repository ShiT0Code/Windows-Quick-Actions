using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics;
using WinRT.Interop;
namespace TouchHelper.SideBar;

public sealed partial class SideBarWindow_Right : Window
{
    public SideBarWindow_Right()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        this.SystemBackdrop = new WinUIEx.TransparentTintBackdrop();
        hwnd = WindowNative.GetWindowHandle(this);

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsMaximizable = false;
        //presenter.IsMinimizable = true;
        presenter.IsResizable = false;
        presenter.SetBorderAndTitleBar(true, false);
        AppWindow.SetPresenter(presenter);
        //AppWindow.IsShownInSwitchers = false;

        DisplayHelper.GetMonitorInfoFromWindow(hwnd, out int width, out int height, out double scalePercent);
        displayHeight = height;
        displayWidth = width;
        this.scalePercent = scalePercent;

        int winHe = (int)(100 * scalePercent);
        int winWidth = (int)(12 * scalePercent);
        closedRect = new RectInt32(displayWidth - 20, (int)(displayHeight / 2 - winHe / 2), 120, winHe);

        winHe = (int)(540 * scalePercent);
        winWidth = (int)(396 * scalePercent);
        openedRect = new RectInt32(displayWidth - winWidth, (int)(displayHeight / 2 - winHe / 2), winWidth, winHe);
        AppWindow.MoveAndResize(closedRect);

        grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Left;
        paneGrid.Width = 0;
        paneTransform.X = 0;
        backgroundImage.Source = null;

        DisplayHelper.DisableWindowRounding(hwnd);
    }

    private bool IsPaneOpen = false;
    private IntPtr hwnd;

    private int displayWidth = 0;
    private int displayHeight = 0;
    private double scalePercent = 0;

    private RectInt32 closedRect;
    private RectInt32 openedRect;

    private async void PropertySizer_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (IsPaneOpen)
        {
            paneCloseStoryboard.Begin();

            await Task.Delay(500);
            ClosePane();
            paneOpenStoryboard.Begin();
        }
        else
        {
            OpenPane();
            paneOpenStoryboard.Begin();
        }
        IsPaneOpen = !IsPaneOpen;
    }

    private async void OpenPane()
    {
        backgroundImage.Source = null;
        using (var bitmap = new Bitmap(500, 500))
        {
            // 2. 创建 Graphics 对象并复制屏幕
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen((int)(openedRect.X - 112), (int)((openedRect.Y + 17) * scalePercent),
                    0, 0, new Size(openedRect.Width+250, openedRect.Height));
            }

            // 3. 将 Bitmap 转换为 ImageSource 供 Image 控件显示
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
                backgroundImage.Source = bitmapImage; // MyImage 是你的 Image 控件
            }
        }
        AppWindow.MoveAndResize(openedRect);
        sizer.Visibility = paneGrid.Visibility = Visibility.Collapsed;
        paneGrid.Width = paneTransform.X = 368;
        grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Right;
        sizer.Visibility = paneGrid.Visibility = backgroundImage.Visibility = Visibility.Visible;
    }

    private void ClosePane()
    {
        AppWindow.MoveAndResize(closedRect);

        paneGrid.Width = 0;
        paneTransform.X = 0;
        backgroundImage.Source = null;
        grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Left;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Sizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (paneTransform.X < 175)
        {
            paneTransform.X = 0;
            IsPaneOpen = true;
        }
        else
        {
            ClosePane();
            IsPaneOpen = false;
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {

    }
}
