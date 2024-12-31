using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using IMKernel.Kinematic;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;

using OCCTK.OCC.AIS;

namespace IMKernelUI.ViewModel;
public partial class MovementFormulaViewModel:ObservableObject, IOCCFinilize {
	public MovementFormulaViewModel( ) {
		AvailableMovements = new(MovementFormulaMap.All);
		SelectedMovement = MovementFormula.Static;
		InitMovement = 0.0;
		MinMovement = double.NegativeInfinity;
		MaxMovement = double.PositiveInfinity;
	}

	#region View

	public List<MovementFormula> AvailableMovements { get; }

	#endregion

	#region Value

	[ObservableProperty]
	private MovementFormula selectedMovement;

	/// <summary>
	/// 初始运动值
	/// </summary>
	[ObservableProperty]
	private double initMovement;

	/// <summary>
	/// 最小运动值
	/// </summary>
	[ObservableProperty]
	private double minMovement;

	/// <summary>
	/// 最大运动值
	/// </summary>
	[ObservableProperty]
	private double maxMovement;

	#endregion

	//todo 实现范围显示
	#region OCC
	private AShape? MinBox { get; }
	private AShape? MaxBox { get; }
	private AShape? MinSector { get; }
	private AShape? MaxSector { get; }

	private ThreeDimensionContext? context;
	public ThreeDimensionContext? Context {
		protected get { return context; }
		set {
			context = value;
			if( context != null && MinBox != null && MaxBox != null ) {
				context.Display(MinBox, true);
				context.AISContext.SetSelectionMode(MinBox, SelectionMode.None);
				context.Display(MaxBox, true);
				context.AISContext.SetSelectionMode(MaxBox, SelectionMode.None);
			}
		}
	}

	public void OCCFinilize( ) {
		//todo 
		throw new NotImplementedException( );
	}

	~MovementFormulaViewModel( ) {
		OCCFinilize( );
	}
	private OCCCanvas? OCCCanvas { get; }

	#region 简化代码
	private Action Update {
		get {
			return Context != null
				? Context.Update
				: throw new NotImplementedException("需要传入3D上下文管理器");
		}
	}
	private Action<InteractiveObject, bool> Display {
		get {
			return Context != null
				? Context.Display
				: throw new NotImplementedException("需要传入3D上下文管理器");
		}
	}
	private Action<InteractiveObject, bool> Redisplay {
		get {
			return Context != null
				? Context.Redisplay
				: throw new NotImplementedException("需要传入3D上下文管理器");
		}
	}
	private Func<InteractiveObject, bool> IsDisplayed {
		get {
			if( Context != null ) {
				return Context.AISContext.IsDisplayed;
			} else {
				throw new NotImplementedException("需要传入3D上下文管理器");
			}
		}
	}

	#endregion

	#endregion

}
