using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Interfaces;

using IMKernelUI.Message;

using OCCTK.OCC.AIS;
using OCCTK.OCC.V3d;

using Color = OCCTK.Extension.Color;

namespace IMKernel.Visualization;

public class ThreeDimensionContext {
	public ThreeDimensionContext( InteractiveContext context, int contextID ) {
		ID = contextID;
		AISContext = context;
		Viewer = context.Viewer;

		//设置默认灯光
		Viewer.SetICOLight( );
		#region 构造指示器对象并注册
		viewCube = new(3.0);
		context.Display(viewCube, false);
		context.Erase(viewCube, false);

		viewTrihedron = new(80);
		viewTrihedron.SetAspect(20, 100);
		viewTrihedron.ArrowWidth = 10;
		context.Display(viewTrihedron, false);
		context.Erase(viewTrihedron, false);

		originTrihedron = new(100);
		foreach( var part in new List<DatumParts>( ) { DatumParts.XAxis, DatumParts.YAxis, DatumParts.ZAxis } ) {
			originTrihedron.SetDatumColor(part, Color.Purple);
			originTrihedron.SetArrowColor(part, Color.Purple);
		}
		context.Display(originTrihedron, false);
		context.Erase(originTrihedron, false);
		#endregion

		#region message
		WeakReferenceMessenger.Default.Register<ViewStatusChangedMessage>(this, ( r, m ) => {
			if( m.Value.contextID != ID ) { return; }
			DisplayViewCube(m.Value.viewCube);
			DisplayOriginTrihedron(m.Value.orignTri);
			DisplayViewTrihedron(m.Value.viewTri);
		});
		#endregion
	}

	private readonly int ID;
	public readonly InteractiveContext AISContext;
	public readonly Viewer Viewer;

	/// <summary>
	/// 视图列表
	/// </summary>
	public List<OCCCanvas> ViewList { get; set; } = [ ];
	public OCCCanvas CreateView( ICommandManager commandManager ) {
		var occCanva= new OCCCanvas(this, commandManager);
		ViewList.Add(occCanva);
		return occCanva;
	}

	#region 视图立方

	protected ViewCube viewCube;

	public void DisplayViewCube( bool showViewCube ) {
		if( showViewCube ) {
			if( !AISContext.IsDisplayed(viewCube) ) {
				Display(viewCube, true);
			}
		} else {
			if( AISContext.IsDisplayed(viewCube) ) {
				Erase(viewCube, true);
			}
		}
	}

	#endregion

	#region 视图坐标系
	protected ATrihedron viewTrihedron;

	public void DisplayViewTrihedron( bool showViewTrihedron ) {
		if( showViewTrihedron ) {
			if( !AISContext.IsDisplayed(viewTrihedron) ) {
				Display(viewTrihedron, true);
			}
		} else {
			if( AISContext.IsDisplayed(viewTrihedron) ) {
				Erase(viewTrihedron, true);
			}
		}
	}

	#endregion

	#region 原点坐标系

	protected ATrihedron originTrihedron;

	public void DisplayOriginTrihedron( bool showOriginTrihedron ) {
		if( showOriginTrihedron ) {
			if( !AISContext.IsDisplayed(originTrihedron) ) {
				Display(originTrihedron, true);
				AISContext.SetSelectionMode(originTrihedron, OCCTK.OCC.AIS.SelectionMode.None);
			}
		} else {
			if( AISContext.IsDisplayed(originTrihedron) ) {
				Erase(originTrihedron, true);
			}
		}
	}

	#endregion

	#region 显示

	public void Display( InteractiveObject theAIS, bool Toupdate = true ) {
		AISContext.Display(theAIS, false);
		//默认颜色为灰色
		AISContext.SetColor(theAIS, new Color(125, 125, 125), Toupdate);
	}

	public void Redisplay( InteractiveObject theAIS, bool Toupdate = true ) {
		AISContext.Redisplay(theAIS, false);
	}

	public void EraseSelected( ) {
		AISContext.EraseSelected( );
	}

	public void Erase( InteractiveObject theAIS, bool Toupdate ) {
		AISContext.Erase(theAIS, Toupdate);
	}

	public void EraseAll( bool update ) {
		AISContext.EraseAll(false);
		DisplayViewCube(false);
		DisplayOriginTrihedron(false);
		DisplayViewTrihedron(false);
		if( update ) {
			AISContext.UpdateCurrentViewer( );
		}
	}

	public void Remove( InteractiveObject theAIS, bool Toupdate ) {
		AISContext.Remove(theAIS, Toupdate);
	}

	public void Update( ) {
		AISContext.UpdateCurrentViewer( );
	}

	#endregion

	#region 对象交互

	/// <summary>
	/// 设置选择模式
	/// </summary>
	/// <param name="theMode"></param>
	public void SetDefaultSelectionMode( OCCTK.OCC.AIS.SelectionMode theMode ) {
		AISContext.SetDefaultSelectionMode(theMode);
	}

	/// <summary>
	/// 获取选中的AIS对象
	/// </summary>
	/// <returns></returns>
	public AShape GetSelectedShape( ) {
		return AISContext.SelectedAIS( );
	}

	#endregion
}
