using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Kinematic;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

using OCCTK.OCC.gp;

namespace IMKernelUI.ViewModel;
public partial class MovementFormulaViewModel:ObservableObject, IOCCFinilize {
	public MovementFormulaViewModel( ) {
		nameString = "未定义";
		MovementType = MovementType.Static;
		X = 1.0;
		Y = 0.0;
		Z = 0.0;
		InitMovement = 0.0;
		MinMovement = -1000;
		MaxMovement = 1000;
		//MinMovement = double.NegativeInfinity;
		//MaxMovement = double.PositiveInfinity;
		StepValues = new( );
		//view
		AvailableMovements = new(MovementFormulaMap.All);
		SelectedMF = MovementFormula.Static;
		Visibility = Visibility.Visible;
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
		if( value.NameString == "自定义" ) {
			nameString = value.NameString;
		} else {
			nameString = value.NameString;
			MovementType = value.Type;
			X = value.Dir.X;
			Y = value.Dir.Y;
			Z = value.Dir.Z;
		}
	}

	[ObservableProperty]
	private Visibility visibility;

	#endregion

	#region Value

	public MovementFormula TheMF {
		get {
			return
				new(nameString, MovementType, new(X, Y, Z)) {
					InitMovement = InitMovement,
					MinMovement = MinMovement,
					MaxMovement = MaxMovement,
					StepValues = StepValues.ToList( ),
				};
		}
		set {
			nameString = value.NameString;
			MovementType = value.Type;
			X = value.Dir.X;
			Y = value.Dir.Y;
			Z = value.Dir.Z;
			InitMovement = value.InitMovement;
			MinMovement = value.MinMovement;
			MaxMovement = value.MaxMovement;
			MovementFormula? finded=AvailableMovements.Find(v => v.GetHashCode() == value.GetHashCode());
			if( finded != null ) {
				SelectedMF = value;
			} else {
				SelectedMF = MovementFormula.Custom;
			}
			StepValues = new(value.StepValues);
			backupMF = value;
		}
	}

	private string nameString;

	/// <summary>
	/// 运动的基本类型
	/// </summary>
	[ObservableProperty]
	private MovementType movementType;

	/// <summary>
	/// 运动方向/旋转轴方向
	/// </summary>
	/// <remarks>
	/// 有正负的区别
	/// </remarks>
	[ObservableProperty]
	private double x;
	[ObservableProperty]
	private double y;
	[ObservableProperty]
	private double z;

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

	/// <summary>
	/// 最大运动值
	/// </summary>
	public ObservableCollection<double> StepValues { get; set; }

	#endregion

	#region Command

	/// <summary>
	/// 备份输入的值
	/// </summary>
	/// <remarks>用于取消设置的运动方向值</remarks>
	private MovementFormula backupMF;

	/// <summary>
	/// 应用设置，并关闭窗口
	/// </summary>
	[RelayCommand]
	private void Apply( ) {
		if( MinMovement <= InitMovement &&
			InitMovement <= MaxMovement ) {
			WeakReferenceMessenger.Default.Send(new MovementFormulaAppliedMessage( ));
		} else {
			MessageBox.Show("运动值超出范围", "错误！");
		}
	}

	/// <summary>
	/// 取消设置，并关闭窗口
	/// </summary>
	[RelayCommand]
	private void Cancel( ) {
		TheMF = backupMF;
		WeakReferenceMessenger.Default.Send(new MovementFormulaCanceledMessage( ));
	}

	#endregion

}
