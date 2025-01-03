using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Interfaces;
using IMKernel.Kinematic;
using IMKernel.Model;
using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

using OCCTK.Extension;
using OCCTK.OCC.gp;

namespace IMKernelUI.ViewModel;

/// <summary>
/// 零件View Model
/// </summary>
public partial class PartViewModel:ObservableObject, IOCCFinilize, IPart {
	public PartViewModel( ) {
		//value
		Name = "";
		Shape = null;
		Connections = new( );
		Movements = new( );
		AvailableMovements = MovementFormulaMap.All.ToList( );

		//view
		PoseVM = new( );
		MFVM = new( );

		#region Message

		//occ
		context = WeakReferenceMessenger.Default.Send<Main3DContextRequestMessage>( );
		WeakReferenceMessenger.Default.Register<Main3DContextChangedMessage>(this, ( r, m ) => {
			context = m.Context;
		});

		#endregion
	}

	#region OCC

	private OCCCanvas? occCanvas;

	public void OCCFinilize( ) {
		//todo 
	}

	private ThreeDimensionContext? context;

	#endregion

	#region Value

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

	public List<(Trsf transfrom, MovementFormula movementFormula)> JointMovements => throw new NotImplementedException( );

	/// <summary>
	/// 连接点
	/// </summary>
	public ObservableCollection<Trsf> Connections { get; }

	/// <summary>
	/// 连接点
	/// </summary>
	public ObservableCollection<MovementFormula> Movements { get; }

	#endregion

	#region View

	public List<MovementFormula> AvailableMovements { get; }

	/// <summary>
	/// 连接点位姿VM
	/// </summary>
	[ObservableProperty]
	private PoseViewModel poseVM;

	/// <summary>
	/// 运动方向VM
	/// </summary>
	[ObservableProperty]
	private MovementFormulaViewModel mFVM;

	#endregion

	#region Command

	[RelayCommand]
	private void SetTrsf( ) {
		if( PoseVM != null ) {
			PoseVM.MyVisibility = Visibility.Visible;
		} else {
		}
	}

	[RelayCommand]
	private void SetMF( ) {
		if( MFVM != null ) {
			MFVM.MyVisibility = Visibility.Visible;
		}
	}

	#endregion

}
