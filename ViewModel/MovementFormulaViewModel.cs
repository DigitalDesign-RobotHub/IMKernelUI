using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using IMKernel.Kinematic;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;

using OCCTK.OCC.gp;

namespace IMKernelUI.ViewModel;
public partial class MovementFormulaViewModel:ObservableObject, IOCCFinilize {
	public MovementFormulaViewModel( ) {
		name = "未定义";
		MovementType = MovementType.Static;
		MovementAxis = new(new( ), new Dir(1, 0, 0));
		InitMovement = 0.0;
		MinMovement = double.NegativeInfinity;
		MaxMovement = double.PositiveInfinity;

		//view
		AvailableMovements = new(MovementFormulaMap.All.ToList( ));
		//todo 自定义运动轴
		//AvailableMovements.Add(new MovementFormula("自定义", MovementType.Translation, new( )));
		SelectedMF = MovementFormula.Static;
		MyVisibility = Visibility.Visible;
		backupMF = TheMF;
	}

	#region OCC

	public void OCCFinilize( ) {
		//todo 
	}

	private ThreeDimensionContext? context;
	public ThreeDimensionContext? Context {
		protected get { return context; }
		set {
			context = value;
		}
	}
	#endregion

	#region View

	public List<MovementFormula> AvailableMovements { get; }

	[ObservableProperty]
	private MovementFormula selectedMF;
	partial void OnSelectedMFChanged( MovementFormula value ) {
		if( value.Name == "自定义" ) {
			//todo 自定义运动轴
		} else {
			name = value.Name;
			MovementType = value.Type;
			MovementAxis = value.Axis;
		}
	}

	[ObservableProperty]
	private Visibility myVisibility;

	#endregion

	#region Value

	public MovementFormula TheMF {
		get {
			return
				new(name, MovementType, MovementAxis) {
					InitMovement = InitMovement,
					MinMovement = MinMovement,
					MaxMovement = MaxMovement
				};
		}
		set {
			name = value.Name;
			MovementType = value.Type;
			MovementAxis = value.Axis;
			InitMovement = value.InitMovement;
			MinMovement = value.MinMovement;
			MaxMovement = value.MaxMovement;
			backupMF = value;
		}
	}

	private string name;

	[ObservableProperty]
	private MovementType movementType;

	[ObservableProperty]
	private Ax1 movementAxis;

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

	#region Command

	/// <summary>
	/// 用于取消设置的运动方向值
	/// </summary>
	private MovementFormula backupMF;

	[RelayCommand]
	private void Apply( ) {
		if( MinMovement <= InitMovement &&
			InitMovement <= MaxMovement ) {
			MyVisibility = Visibility.Collapsed;
		} else {
			MessageBox.Show("运动值超出范围", "错误！");
			return;
		}
	}

	[RelayCommand]
	private void Cancel( ) {
		TheMF = backupMF;
	}

	#endregion

}
