//https://stackoverflow.com/questions/6087835/can-i-overlay-a-wpf-window-on-top-of-another/6452940#6452940
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

using Point = System.Windows.Point;

namespace IMKernelUI.View;

/// <summary>
/// 自定义的 Popup 控件，专门用于解决 WPF 中复杂的层叠和放置问题
/// 特别适用于处理 WebBrowser、WindowsFormsHost 等非原生 WPF 控件的悬浮显示
/// </summary>
/// <remarks>
/// 主要解决传统 Popup 在特定场景下的显示和交互限制
/// 1. 支持跟随目标控件
/// 2. 允许部分超出屏幕显示
/// 3. 动态调整层级和位置
/// 4. 处理父窗口激活/失活事件
/// </remarks>
public class AirspacePopup:Popup {
	/// <summary>
	/// 是否置顶属性，控制 Popup 是否始终位于其他窗口之上
	/// </summary>
	public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register(
		"IsTopmost",
		typeof(bool),
		typeof(AirspacePopup),
		new FrameworkPropertyMetadata(false, OnIsTopmostChanged)
	);

	/// <summary>
	/// 是否跟随放置目标的属性，控制 Popup 是否自动跟随目标控件
	/// </summary>
	public static readonly DependencyProperty FollowPlacementTargetProperty =
		DependencyProperty.RegisterAttached(
			"FollowPlacementTarget",
			typeof(bool),
			typeof(AirspacePopup),
			new UIPropertyMetadata(false)
		);

	/// <summary>
	/// 是否允许部分超出屏幕的属性，控制 Popup 是否可以在屏幕边缘部分显示
	/// </summary>
	public static readonly DependencyProperty AllowOutsideScreenPlacementProperty =
		DependencyProperty.RegisterAttached(
			"AllowOutsideScreenPlacement",
			typeof(bool),
			typeof(AirspacePopup),
			new UIPropertyMetadata(false)
		);

	/// <summary>
	/// 父窗口属性，用于关联和监听父窗口的位置和大小变化
	/// </summary>
	public static readonly DependencyProperty ParentWindowProperty =
		DependencyProperty.RegisterAttached(
			"ParentWindow",
			typeof(Window),
			typeof(AirspacePopup),
			new UIPropertyMetadata(null, ParentWindowPropertyChanged)
		);

	private static void OnIsTopmostChanged(
		DependencyObject source,
		DependencyPropertyChangedEventArgs e
	) {
		AirspacePopup? airspacePopup = source as AirspacePopup;
		airspacePopup?.SetTopmostState(airspacePopup.IsTopmost);
	}

	private static void ParentWindowPropertyChanged(
		DependencyObject source,
		DependencyPropertyChangedEventArgs e
	) {
		AirspacePopup? airspacePopup = source as AirspacePopup;
		airspacePopup?.ParentWindowChanged( );
	}

	private bool? m_appliedTopMost;
	private bool m_alreadyLoaded;
	private Window? m_parentWindow;

	/// <summary>
	/// 构造函数，设置 Popup 的基本行为和事件监听
	/// </summary>
	public AirspacePopup( ) {
		Loaded += OnPopupLoaded;
		Unloaded += OnPopupUnloaded;

		DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(
			PlacementTargetProperty,
			typeof(AirspacePopup)
		);
		descriptor.AddValueChanged(this, PlacementTargetChanged);
	}

	public bool IsTopmost {
		get { return (bool)GetValue(IsTopmostProperty); }
		set { SetValue(IsTopmostProperty, value); }
	}
	public bool FollowPlacementTarget {
		get { return (bool)GetValue(FollowPlacementTargetProperty); }
		set { SetValue(FollowPlacementTargetProperty, value); }
	}
	public bool AllowOutsideScreenPlacement {
		get { return (bool)GetValue(AllowOutsideScreenPlacementProperty); }
		set { SetValue(AllowOutsideScreenPlacementProperty, value); }
	}

	public Window ParentWindow {
		get { return (Window)GetValue(ParentWindowProperty); }
		set { SetValue(ParentWindowProperty, value); }
	}

	private void ParentWindowChanged( ) {
		if( ParentWindow != null ) {
			ParentWindow.LocationChanged += ( sender, e2 ) => {
				UpdatePopupPosition( );
			};
			ParentWindow.SizeChanged += ( sender, e2 ) => {
				UpdatePopupPosition( );
			};
		}
	}

	/// <summary>
	/// 当绑定的控件发生变化，注册SizeChanged事件
	/// </summary>
	/// <param nameString="sender"></param>
	/// <param nameString="e"></param>
	private void PlacementTargetChanged( object? sender, EventArgs e ) {
		FrameworkElement? placementTarget = this.PlacementTarget as FrameworkElement;
		if( placementTarget != null ) {
			placementTarget.SizeChanged += ( sender2, e2 ) => {
				UpdatePopupPosition( );
			};
		}
	}

	/// <summary>
	/// 更新 Popup 的位置和尺寸
	/// 核心方法，处理跨屏幕和动态放置的逻辑
	/// </summary>
	public void UpdatePopupPosition( ) {
		// 获取放置目标和子元素
		FrameworkElement? placementTarget = this.PlacementTarget as FrameworkElement;
		FrameworkElement? child = this.Child as FrameworkElement;

		// 计算屏幕边缘裁剪
		if(
			placementTarget != null &&
			PresentationSource.FromVisual(placementTarget) != null
			&& AllowOutsideScreenPlacement == true
		) {
			// 计算左、上、右、下的偏移量
			double leftOffset = CutLeft(placementTarget);
			double topOffset = CutTop(placementTarget);
			double rightOffset = CutRight(placementTarget);
			double bottomOffset = CutBottom(placementTarget);

			// 动态调整 Popup 尺寸
			this.Width = Math.Max(
				0,
				Math.Min(leftOffset, rightOffset) + placementTarget.ActualWidth
			);
			this.Height = Math.Max(
				0,
				Math.Min(topOffset, bottomOffset) + placementTarget.ActualHeight
			);

			// 设置子元素边距，实现部分超出屏幕效果
			if( child != null ) {
				child.Margin = new Thickness(leftOffset, topOffset, rightOffset, bottomOffset);
			}
		}

		// 微调位置，触发重新布局
		if( FollowPlacementTarget == true ) {
			this.HorizontalOffset += 0.01;
			this.HorizontalOffset -= 0.01;
		}
	}

	private double CutLeft( FrameworkElement placementTarget ) {
		Point point = placementTarget.PointToScreen(new Point(0, placementTarget.ActualWidth));
		return Math.Min(0, point.X);
	}

	private double CutTop( FrameworkElement placementTarget ) {
		Point point = placementTarget.PointToScreen(new Point(placementTarget.ActualHeight, 0));
		return Math.Min(0, point.Y);
	}

	private double CutRight( FrameworkElement placementTarget ) {
		Point point = placementTarget.PointToScreen(new Point(0, placementTarget.ActualWidth));
		point.X += placementTarget.ActualWidth;
		return Math.Min(
			0,
			SystemParameters.VirtualScreenWidth
				- ( Math.Max(SystemParameters.VirtualScreenWidth, point.X) )
		);
	}

	private double CutBottom( FrameworkElement placementTarget ) {
		Point point = placementTarget.PointToScreen(new Point(placementTarget.ActualHeight, 0));
		point.Y += placementTarget.ActualHeight;
		return Math.Min(
			0,
			SystemParameters.VirtualScreenHeight
				- ( Math.Max(SystemParameters.VirtualScreenHeight, point.Y) )
		);
	}

	private void OnPopupLoaded( object sender, RoutedEventArgs e ) {
		if( m_alreadyLoaded )
			return;

		m_alreadyLoaded = true;

		if( Child != null ) {
			Child.AddHandler(
				PreviewMouseLeftButtonDownEvent,
				new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown),
				true
			);
		}

		m_parentWindow = Window.GetWindow(this);

		if( m_parentWindow == null )
			return;

		m_parentWindow.Activated += OnParentWindowActivated;
		m_parentWindow.Deactivated += OnParentWindowDeactivated;
		//m_parentWindow.SizeChanged += OnParentSizeChanged;
	}

	private void OnPopupUnloaded( object? sender, RoutedEventArgs e ) {
		if( m_parentWindow == null )
			return;
		m_parentWindow.Activated -= OnParentWindowActivated;
		m_parentWindow.Deactivated -= OnParentWindowDeactivated;
	}

	private void OnParentWindowActivated( object? sender, EventArgs e ) {
		SetTopmostState(true);
	}

	private void OnParentWindowDeactivated( object? sender, EventArgs e ) {
		if( IsTopmost == false ) {
			SetTopmostState(IsTopmost);
		}
	}

	private void OnChildPreviewMouseLeftButtonDown( object? sender, MouseButtonEventArgs e ) {
		SetTopmostState(true);
		if( !m_parentWindow?.IsActive ?? false && IsTopmost == false ) {
			m_parentWindow?.Activate( );
		}
	}

	protected override void OnOpened( EventArgs e ) {
		SetTopmostState(IsTopmost);
		base.OnOpened(e);
	}

	/// <summary>
	/// 设置窗口的 TopMost 状态（置顶或取消置顶）。
	/// </summary>
	/// <param nameString="isTop">是否置顶窗口，true 为置顶，false 为取消置顶。</param>
	/// <remarks>
	/// 此方法通过 Win32 API 调用更改窗口的 Z-Order，从而实现窗口置顶或取消置顶功能。
	/// 它首先检测当前窗口的 TopMost 状态，避免重复设置。
	/// </remarks>
	private void SetTopmostState( bool isTop ) {
		// 如果当前状态与目标状态相同，则无需操作
		if( m_appliedTopMost.HasValue && m_appliedTopMost == isTop ) {
			return;
		}

		// 如果没有子控件，直接返回
		if( Child == null )
			return;

		// 获取窗口句柄（HwndSource）
		var hwndSource = (PresentationSource.FromVisual(Child)) as HwndSource;

		if( hwndSource == null )
			return;

		// 获取窗口的句柄
		var hwnd = hwndSource.Handle;

		// 定义窗口矩形区域
		RECT rect;

		// 获取窗口的当前位置和尺寸
		if( !GetWindowRect(hwnd, out rect) )
			return;

		//Debug.WriteLine("setting z-order " + isTop);

		if( isTop ) {
			// 设置窗口为 TopMost（置顶）
			SetWindowPos(
				hwnd,
				HWND_TOPMOST, // 将窗口置于所有非 TopMost 窗口之上
				rect.Left,
				rect.Top,
				(int)Width,
				(int)Height,
				TOPMOST_FLAGS // 指定不调整窗口大小
			);
		} else {
			// 为了正确刷新 Z-Order，按照顺序设置为底部、顶部，然后取消置顶
			// 1. 设置窗口为 HWND_BOTTOM（移至最底层）
			SetWindowPos(
				hwnd,
				HWND_BOTTOM,
				rect.Left,
				rect.Top,
				(int)Width,
				(int)Height,
				TOPMOST_FLAGS
			);

			// 2. 设置窗口为 HWND_TOP（移至非置顶层的顶部）
			SetWindowPos(
				hwnd,
				HWND_TOP,
				rect.Left,
				rect.Top,
				(int)Width,
				(int)Height,
				TOPMOST_FLAGS
			);

			// 3. 最后设置为 HWND_NOTOPMOST（取消置顶状态）
			SetWindowPos(
				hwnd,
				HWND_NOTOPMOST,
				rect.Left,
				rect.Top,
				(int)Width,
				(int)Height,
				TOPMOST_FLAGS
			);
		}

		// 更新已应用的 TopMost 状态
		m_appliedTopMost = isTop;
	}

	#region 平台调用服务 P/Invoke imports & definitions

	// 禁用特定的警告以保持代码清晰
#pragma warning disable 1591 // 禁用 XML 文档警告
#pragma warning disable 169  // 禁用未使用的警告
	// ReSharper disable InconsistentNaming // 禁用命名不一致警告，遵循 P/Invoke 命名规则

	/// <summary>
	/// 表示一个矩形的结构体，包含左、上、右、下的坐标。
	/// 用于与 Windows API 交互以描述窗口的位置和尺寸。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT {
		public int Left; // 矩形的左坐标
		public int Top; // 矩形的上坐标
		public int Right; // 矩形的右坐标
		public int Bottom; // 矩形的下坐标
	}

	/// <summary>
	/// 调用 Windows API 获取指定窗口的矩形信息。
	/// </summary>
	/// <param nameString="hWnd">窗口的句柄。</param>
	/// <param nameString="lpRect">输出的 RECT 结构体，用于接收窗口的边界信息。</param>
	/// <returns>如果成功，返回 true；否则返回 false。</returns>
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

	/// <summary>
	/// 调用 Windows API 设置窗口的位置和 Z 顺序。
	/// </summary>
	/// <param nameString="hWnd">目标窗口的句柄。</param>
	/// <param nameString="hWndInsertAfter">Z 顺序的参照窗口句柄。</param>
	/// <param nameString="X">新位置的 X 坐标。</param>
	/// <param nameString="Y">新位置的 Y 坐标。</param>
	/// <param nameString="cx">窗口的新宽度。</param>
	/// <param nameString="cy">窗口的新高度。</param>
	/// <param nameString="uFlags">用于定义窗口位置的标志。</param>
	/// <returns>如果成功，返回 true；否则返回 false。</returns>
	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(
		IntPtr hWnd,
		IntPtr hWndInsertAfter,
		int X,
		int Y,
		int cx,
		int cy,
		uint uFlags
	);

	// Z 顺序的特殊句柄，用于设置窗口的位置和属性

	/// <summary>
	/// 将窗口放置在所有其他窗口之上。
	/// </summary>
	static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

	/// <summary>
	/// 将窗口移出顶层窗口的列表。
	/// </summary>
	static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

	/// <summary>
	/// 将窗口置于 Z 顺序的顶部。
	/// </summary>
	static readonly IntPtr HWND_TOP = new IntPtr(0);

	/// <summary>
	/// 将窗口置于 Z 顺序的底部。
	/// </summary>
	static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

	// SetWindowPos 的标志定义，用于控制窗口行为

	/// <summary>
	/// 保持窗口的当前大小。
	/// </summary>
	private const UInt32 SWP_NOSIZE = 0x0001;

	/// <summary>
	/// 保持窗口的当前位置。
	/// </summary>
	private const UInt32 SWP_NOMOVE = 0x0002;

	/// <summary>
	/// 保持当前的 Z 顺序。
	/// </summary>
	private const UInt32 SWP_NOZORDER = 0x0004;

	/// <summary>
	/// 不重绘窗口。
	/// </summary>
	private const UInt32 SWP_NOREDRAW = 0x0008;

	/// <summary>
	/// 不激活窗口。
	/// </summary>
	private const UInt32 SWP_NOACTIVATE = 0x0010;

	/// <summary>
	/// 框架已更改：发送 WM_NCCALCSIZE 消息。
	/// </summary>
	private const UInt32 SWP_FRAMECHANGED = 0x0020;

	/// <summary>
	/// 显示窗口。
	/// </summary>
	private const UInt32 SWP_SHOWWINDOW = 0x0040;

	/// <summary>
	/// 隐藏窗口。
	/// </summary>
	private const UInt32 SWP_HIDEWINDOW = 0x0080;

	/// <summary>
	/// 不复制窗口内容。
	/// </summary>
	private const UInt32 SWP_NOCOPYBITS = 0x0100;

	/// <summary>
	/// 忽略所有者窗口的 Z 顺序。
	/// </summary>
	private const UInt32 SWP_NOOWNERZORDER = 0x0200;

	/// <summary>
	/// 不发送 WM_WINDOWPOSCHANGING 消息。
	/// </summary>
	private const UInt32 SWP_NOSENDCHANGING = 0x0400;

	/// <summary>
	/// 顶层窗口的默认标志。
	/// </summary>
	private const UInt32 TOPMOST_FLAGS =
		SWP_NOACTIVATE
		| SWP_NOOWNERZORDER
		| SWP_NOSIZE
		| SWP_NOMOVE
		| SWP_NOREDRAW
		| SWP_NOSENDCHANGING;

	// 恢复警告设置
#pragma warning restore 1591 // 恢复 XML 文档警告
#pragma warning restore 169  // 恢复未使用的警告
	// ReSharper restore InconsistentNaming

	#endregion
}
