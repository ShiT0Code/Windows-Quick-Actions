using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Graphics;

namespace TouchHelper.SideBar;

public class SideBarCurrentData : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public double Width { get; set; }
    public double HalfofHeight { get; set; }

    private bool _isLeftPaneOpen = false;
    public bool IsLeftPaneOpen
    {
        get => _isLeftPaneOpen;
        set
        {
            if(_isLeftPaneOpen != value)
            {
                _isLeftPaneOpen = value;
                LeftOpacity = value ? 1 : 0;
                OnPropertyChanged();
            }
        }
    }

    private double _leftOpacity = 1;
    public double LeftOpacity
    {
        get => _leftOpacity;
        set
        {
            if( _leftOpacity != value)
            {
                _leftOpacity = value;
                OnPropertyChanged();
            }
        }
    }

    public PointInt32 LeftPaneCurrentPoint { get; set; } = new();

    // 右侧边栏
    private bool _isRightPaneOpen = false;
    public bool IsRightPaneOpen
    {
        get => _isRightPaneOpen;
        set
        {
            if (_isRightPaneOpen != value)
            {
                _isRightPaneOpen = value;
                RightOpacity = value ? 1 : 0;
                OnPropertyChanged();
            }
        }
    }

    private double _rightOpacity = 1;
    public double RightOpacity
    {
        get => _rightOpacity;
        set
        {
            if (_rightOpacity != value)
            {
                _rightOpacity = value;
                OnPropertyChanged();
            }
        }
    }
    public PointInt32 RightPaneCurrentPoint { get; set; } = new();
}
