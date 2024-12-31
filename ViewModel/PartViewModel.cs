using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using IMKernel.Kinematic;
using IMKernel.Model;
using IMKernel.OCCExtension;

using IMKernelUI.Interfaces;

using OCCTK.Extension;

namespace IMKernelUI.ViewModel;
public partial class PartViewModel:ObservableObject, IOCCFinilize {
	public PartViewModel( ) {
		Name = "";
		Shape = null;
		Connections = new( );
		AvailableMovements = MovementFormulaMap.All.ToList( );
		TrsfVMVisibility = Visibility.Collapsed;
		MovementFormulaVMVisibility = Visibility.Collapsed;
	}

	public void OCCFinilize( ) {
		throw new NotImplementedException( );
	}

	#region Part属性
	public Part ThePart {
		get { return new( ); }
		set {
			//todo 
		}
	}
	/// <summary>
	/// 名称
	/// </summary>
	[ObservableProperty]
	private string name;

	/// <summary>
	///	形状
	/// </summary>
	[ObservableProperty]
	private XShape? shape;

	/// <summary>
	/// 连接点
	/// </summary>
	public ObservableCollection<Pose> Connections { get; }

	#endregion

	#region View属性

	public List<MovementFormula> AvailableMovements { get; }

	[ObservableProperty]
	private Visibility trsfVMVisibility;


	/// <summary>
	/// 连接点位姿VM
	/// </summary>
	[ObservableProperty]
	private TrsfViewModel? trsfVM;

	[ObservableProperty]
	private Visibility movementFormulaVMVisibility;

	/// <summary>
	/// 运动方向VM
	/// </summary>
	[ObservableProperty]
	private MovementFormulaViewModel? movementFormulaVM;


	#endregion
}
