using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.UI.WindowManagement;
using WinRT.Interop;

namespace TouchHelper.SideBar;

public sealed partial class SideBarWindow_Left : Window
{
    public SideBarWindow_Left()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        this.SystemBackdrop = new WinUIEx.TransparentTintBackdrop();
        hwnd = WindowNative.GetWindowHandle(this);

        OverlappedPresenter presenter = OverlappedPresenter.Create();
        presenter.IsAlwaysOnTop = true;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = true;
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
        closedRect = new RectInt32(-120 + 3, (int)(displayHeight / 2 - winHe / 2), 120, winHe);

        winHe = (int)(540 * scalePercent);
        winWidth = (int)(396 * scalePercent);
        openedRect = new RectInt32(0, (int)(displayHeight / 2 - winHe / 2), winWidth, winHe);

        grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Right;
        paneGrid.Width = 0;
        paneTransform.X = 0;
        backgroundImage.Source = null;
        AppWindow.MoveAndResize(closedRect);
        DisplayHelper.DisableWindowRounding(hwnd);
    }

    private void SideBarWindow_Left_Closed(object sender, WindowEventArgs args) { }

    private bool IsPaneOpen = false;
    private IntPtr hwnd;

    private int displayWidth = 0;
    private int displayHeight = 0;
    private double scalePercent = 0;

    private RectInt32 closedRect;
    private RectInt32 openedRect;

    bool IsOpening = false;
    private async void PropertySizer_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (IsPaneOpen)
        {
            paneCloseStoryboard.Begin();
            await Task.Delay(450);
            paneOpenStoryboard.Begin();
            AppWindow.MoveAndResize(closedRect);
            backgroundImage.Visibility = Visibility.Collapsed;
            grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Right;
            paneGrid.Width = 0;
            paneTransform.X = 0;
            backgroundImage.Source = null;
        }
        else
        {
            backgroundImage.Source = null;
            using (var bitmap = new Bitmap(500, 500))
            {
                // 2. 创建 Graphics 对象并复制屏幕
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen((int)(openedRect.X + 8 * scalePercent), (int)((openedRect.Y + 17) * scalePercent), 
                        0, 0, new Size(openedRect.Width, openedRect.Height));
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
            backgroundImage.Visibility = Visibility.Visible;
            grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Left;
            paneGrid.Width = 368;
            paneTransform.X = -368;
            paneOpenStoryboard.Begin();
        }
        IsPaneOpen = !IsPaneOpen;
    }

    private async void Sizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (paneTransform.X < -150)
        {
            paneTransform.X = -368;
            IsPaneOpen = false;
            AppWindow.MoveAndResize(closedRect);
            backgroundImage.Visibility = Visibility.Collapsed;
            grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Right;
            paneGrid.Width = 0;
            paneTransform.X = 0;
        }
        else
        {
            paneTransform.X = 0;
            IsPaneOpen = true;
            AppWindow.MoveAndResize(openedRect);
        }
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
    }

    private void PropertySizer_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
    {
        AppWindow.MoveAndResize(openedRect);
        backgroundImage.Visibility = Visibility.Visible;
        grid1.HorizontalAlignment = rootGrid.HorizontalAlignment = HorizontalAlignment.Left;
        paneGrid.Width = 368;
        paneTransform.X = -368;
    }

    private async void Button_Click_1(object sender, RoutedEventArgs e)
    {
    }

    private void PropertySizer_PointerEntered(object sender, PointerRoutedEventArgs e)
    {

    }
}