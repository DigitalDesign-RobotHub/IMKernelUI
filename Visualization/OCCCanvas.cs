using System;
using System.Collections.Generic;
using System.Windows.Forms;

using log4net;

using OCCTK.OCC.AIS;
using OCCTK.OCC.V3d;

using OCCView = OCCTK.OCC.V3d.View;

namespace IMKernel.Visualization;

public partial class OCCCanvas:Form {
	private static readonly ILog log = LogManager.GetLogger(typeof(OCCCanvas));

	#region 属性和字段

	protected readonly ThreeDimensionContext ThreeDimensionContext;
	protected Viewer Viewer => ThreeDimensionContext.Viewer;
	protected InteractiveContext AISContext => ThreeDimensionContext.AISContext;

	/// <summary>
	/// 框选零宽度的最小值
	/// </summary>
	protected const int ZEROWIDTHMIN = 1;

	/// <summary>
	/// 设备的像素宽高比
	/// </summary>
	protected float devicePixelRatioX;
	protected float devicePixelRatioY;

	protected List<AShape> _CurrentAIS = [];

	public List<AShape> SelectedAISList = [];

	//#region 刻度坐标系

	//private bool _ShowGraduatedTrihedron;
	//public bool ShowGraduatedTrihedron {
	//    get { return _ShowGraduatedTrihedron; }
	//    set {
	//    _ShowGraduatedTrihedron = value;
	//    DisplayGraduatedTrihedron();
	//    }
	//}

	//private void DisplayGraduatedTrihedron() {
	//if (showOriginTrihedron) {
	//View?.DisplayDefault_GraduatedTrihedron();
	//}
	//else {
	//View?.Hide_GraduatedTrihedron();
	//}
	//}

	//#endregion



	#region 选中AIS的原点坐标系
	public ATrihedron selectedAISOriginTrihedron;

	/// <summary>
	/// 创建默认的选中Shape的原点坐标系，只更改其pose，避免反复创建的开销
	/// </summary>
	public ATrihedron SelectedAISOriginTrihedron {
		get => selectedAISOriginTrihedron;
	}
	#endregion


	/// <summary>
	/// 主视图（第一个创建的视图）
	/// </summary>
	public OCCView View { get; protected set; }

	#region 状态flag

	private bool _ShowGrid;

	/// <summary>
	/// 是否显示网格
	/// </summary>
	public bool ShowGrid {
		get { return _ShowGrid; }
		set { _ShowGrid = value; }
	}

	/// <summary>
	/// 当前显示模式
	/// </summary>
	public DisplayMode CurrentDisplayMode { get; private set; }

	public OCCTK.OCC.AIS.SelectionMode _currentSelectionMode;

	/// <summary>
	/// 当前选择模式
	/// </summary>
	public OCCTK.OCC.AIS.SelectionMode CurrentSelectionMode {
		get { return _currentSelectionMode; }
		set {
			if( AISContext != null ) {
				AISContext.SetDefaultSelectionMode(value);
				_currentSelectionMode = value;
			}
		}
	}

	/// <summary>
	/// 当前视图方向
	/// </summary>
	public OCCTK.OCC.V3d.ViewOrientation CurrentViewOrientation { get; private set; }

	#endregion

	#region 显示模式

	/// <summary>
	/// 隐藏线显示模式
	/// </summary>
	public bool IsDegenerateMode { get; private set; }

	/// <summary>
	/// 框线模式
	/// </summary>
	public bool IsWireframeEnabled { get; private set; }

	/// <summary>
	/// 阴影模式
	/// </summary>
	public bool IsShadingEnabled { get; private set; }

	#endregion

	#endregion

	private void InitializeComponent( ) {
		// 标题栏中不显示控制框的值
		ControlBox = false;
		// 设置不为顶层窗口
		TopLevel = false;
		this.ImeMode = System.Windows.Forms.ImeMode.NoControl;

		SizeChanged += new System.EventHandler(OnSizeChanged);
		Paint += new System.Windows.Forms.PaintEventHandler(OnPaint);
	}

	private void SetDefault( ) {
		if( Viewer == null ) {
			throw new Exception("OCC OCCCanvas is null");
		}
		if( AISContext == null ) {
			throw new Exception("AISContext is null");
		}
		if( View == null ) {
			throw new Exception("View is null");
		}

		//设置交互默认值
		AISContext.SetDefault( );
		//设置相机默认值
		View.SetICORendering( );
		//设置默认背景颜色
		View.SetDefaultBGColor( );
		//设置选择模式
		CurrentSelectionMode = OCCTK.OCC.AIS.SelectionMode.Shape;

		float dpiX = CreateGraphics().DpiX;
		float dpiY = CreateGraphics().DpiY;

		// 计算设备像素比
		devicePixelRatioX = dpiX / 96.0f;
		devicePixelRatioY = dpiY / 96.0f;

		#region 设置画布

		//隐藏线模式
		IsDegenerateMode = true;
		//显示网格
		ShowGrid = false;
		////显示视图立方
		//ShowViewCube = true;
		////显示原点坐标轴
		//ShowOriginTrihedron = false;
		////显示视图坐标轴
		//ShowViewTrihedron = false;
		////显示刻度坐标系
		//ShowGraduatedTrihedron = true;

		#endregion

		_manipulatorMode = null;
		_manipulatorAxis = null;
	}

	/// <summary>
	/// 循环选择下一个枚举值
	/// </summary>
	/// <param name="currentEnum"></param>
	/// <returns></returns>
	private static T CycleEnum<T>( T currentEnum )
		where T : Enum {
		// 获取枚举类型的所有值
		T[] enumValues = (T[])Enum.GetValues(typeof(T));

		// 获取当前枚举值的索引
		int currentIndex = Array.IndexOf(enumValues, currentEnum);

		// 获取下一个枚举值的索引，如果当前值是最后一个，则循环到第一个
		int nextIndex = (currentIndex + 1) % enumValues.Length;

		// 返回下一个枚举值
		return enumValues[nextIndex];
	}

	#region 重写虚方法

	#region 渲染

	protected override void OnPaintBackground( PaintEventArgs e ) {
		// 不调用基类方法，避免系统背景绘制
	}

	private void OnPaint( object? sender, PaintEventArgs e ) {
		if( AISContext == null || View == null ) {
			return;
		}

		View.Redraw( );
		AISContext.UpdateCurrentViewer( );
	}

	private void OnSizeChanged( object? sender, EventArgs e ) {
		if( View == null ) {
			return;
		}
		View.MustBeResized( );
	}

	#endregion

	#endregion

	#region 视图控制

	public new void Update( ) {
		View.Redraw( );
		AISContext.UpdateCurrentViewer( );
	}

	/// <summary>
	/// 相机缩放至最佳尺寸
	/// </summary>
	public void FitAll( ) {
		View.FitAll(0.01, true);
	}

	/// <summary>
	/// 循环设置相机视图方向
	/// </summary>
	public void SetViewOrientation( ) {
		View.SetViewOrientation(CycleEnum(CurrentViewOrientation), true);
	}

	/// <summary>
	/// 设置相机视图方向为theMode
	/// </summary>
	/// <param name="theMode"></param>
	public void SetViewOrientation( ViewOrientation theMode ) {
		CurrentViewOrientation = theMode;
		View.SetViewOrientation(theMode, true);
	}

	/// <summary>
	/// 重置视图的居中和方向
	/// </summary>
	public void Reset( ) {
		View.Reset( );
	}

	/// <summary>
	/// 设置隐藏线模式
	/// </summary>
	/// <param name="theMode"></param>
	public void SetHidden( bool toOpen ) {
		View.SetDegenerateMode(toOpen);
		IsDegenerateMode = toOpen;
	}

	#endregion
}
