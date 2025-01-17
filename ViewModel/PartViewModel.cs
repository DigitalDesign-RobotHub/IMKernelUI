using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevExpress.ClipboardSource.SpreadsheetML;

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
		AvailableMovements = MovementFormulaMap.All.ToList( );
		JointMovements = new( );

		//view
		currentJointTrsfIndex = -1;
		currentJointMFTrsfIndex = -1;
		IsSettingVisibility = Visibility.Collapsed;
		TrsfVM = new( );
		TrsfVM.Visibility = Visibility.Collapsed;
		MFVM = new( );
		MFVM.Visibility = Visibility.Collapsed;
		MFVM.AvailableMovements = new(MovementFormulaMap.All.ToList( ));
		MFVM.AvailableMovements.Remove(MovementFormula.Static);

		#region Message

		//occ
		context = WeakReferenceMessenger.Default.Send<Main3DContextRequestMessage>( );
		WeakReferenceMessenger.Default.Register<Main3DContextChangedMessage>(this, ( r, m ) => {
			context = m.Context;
		});

		//value
		WeakReferenceMessenger.Default.Register<TrsfAppliedMessage>(this, ( r, m ) => {
			TrsfVM.Visibility = Visibility.Collapsed;
		});
		WeakReferenceMessenger.Default.Register<TrsfSetCanceledMessage>(this, ( r, m ) => {
			TrsfVM.Visibility = Visibility.Collapsed;
		});
		WeakReferenceMessenger.Default.Register<MovementFormulaAppliedMessage>(this, ( r, m ) => {
			MFVM.Visibility = Visibility.Collapsed;
		});
		WeakReferenceMessenger.Default.Register<MovementFormulaCanceledMessage>(this, ( r, m ) => {
			MFVM.Visibility = Visibility.Collapsed;
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
		get {
			return new( ) {
				Name = Name,
				Shape = Shape,
				JointMovements = JointMovements,
			};
		}
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

	public List<(Trsf transfrom, MovementFormula movementFormula)> JointMovements { get; }

	#endregion

	#region View

	[ObservableProperty]
	private Visibility isSettingVisibility;

	public List<MovementFormula> AvailableMovements { get; }

	/// <summary>
	/// 连接点位姿VM
	/// </summary>
	[ObservableProperty]
	private TrsfViewModel trsfVM;

	/// <summary>
	/// 运动方向VM
	/// </summary>
	[ObservableProperty]
	private MovementFormulaViewModel mFVM;

	private int currentJointTrsfIndex;
	private int currentJointMFTrsfIndex;

	#endregion

	#region Command

	[RelayCommand(CanExecute = nameof(CanAddNewJoint))]
	private void AddNewJoint( int rowIndex ) {
		//JointMovements.Add((new( ), new( )));
		WeakReferenceMessenger.Default.Send(new AddNewJointMessage( ));
	}

	private bool CanAddNewJoint( int rowIndex ) {
		// 自定义逻辑，确保返回 true 或 false
		return rowIndex >= 0;
	}

	[RelayCommand]
	private void SetTrsf( int rowIndex ) {
		//todo 记录选中的是Grid的第几个
		currentJointTrsfIndex = ( rowIndex - 3 ) / 3;
		TrsfVM.Visibility = Visibility.Visible;
		IsSettingVisibility = Visibility.Visible;
		TrsfVM.TheTrsf = JointMovements[currentJointTrsfIndex].transfrom;
	}

	[RelayCommand]
	private void SetMF( int rowIndex ) {
		//todo 记录选中的是Grid的第几个
		currentJointMFTrsfIndex = ( rowIndex - 3 - 2 ) / 3;
		MFVM.Visibility = Visibility.Visible;
		IsSettingVisibility = Visibility.Visible;
		MFVM.TheMF = JointMovements[currentJointMFTrsfIndex].movementFormula;
	}

	#endregion

}
