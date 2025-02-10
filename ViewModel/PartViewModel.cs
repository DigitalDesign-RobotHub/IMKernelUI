using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Mvvm.Native;

using IMKernel.Interfaces;
using IMKernel.Kinematic;
using IMKernel.Model;
using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

using log4net;

using OCCTK.Extension;
using OCCTK.OCC.gp;

namespace IMKernelUI.ViewModel;

public class FontSizeToWidthConverter:IValueConverter {
	public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
		if( value is double fontSize ) {
			return fontSize * 2.7; // 让 Width = FontSize * 2.5
		}
		return 50; // 默认宽度
	}

	public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) => DependencyProperty.UnsetValue;
}

/// <summary>
/// 零件View Model
/// </summary>
public partial class PartViewModel:ObservableObject, IOCCFinilize, IPart {
	private static readonly ILog log = LogManager.GetLogger(typeof(PartViewModel));
	public PartViewModel( ) {
		//value
		Name = "";
		AvailableMovements = MovementFormulaMap.All.ToList( );
		Connections = new( );
		Movements = new( );
		Joints = new( );

		//view
		IsSettingVisibility = Visibility.Collapsed;
		TrsfVM = new( );
		TrsfVM.Visibility = Visibility.Collapsed;
		MFVM = new( );
		MFVM.Visibility = Visibility.Collapsed;

		#region Message

		//occ
		context = WeakReferenceMessenger.Default.Send<Main3DContextRequestMessage>( );
		WeakReferenceMessenger.Default.Register<Main3DContextChangedMessage>(this, ( r, m ) => {
			context = m.Context;
		});

		//value
		WeakReferenceMessenger.Default.Register<TrsfAppliedMessage>(this, ( r, m ) => {
			TrsfVM.Visibility = Visibility.Collapsed;
			if( MFVM.Visibility == Visibility.Collapsed ) {
				IsSettingVisibility = Visibility.Collapsed;
			}
			if( currentJointTrsf == null ) {
				return;
			}
			currentJointTrsf.Connection = TrsfVM.TheTrsf;
			//var i =Joints.IndexOf( currentJointTrsf );
			//Joints[i].Connection = TrsfVM.TheTrsf;
		});
		WeakReferenceMessenger.Default.Register<TrsfSetCanceledMessage>(this, ( r, m ) => {
			TrsfVM.Visibility = Visibility.Collapsed;
			if( MFVM.Visibility == Visibility.Collapsed ) {
				IsSettingVisibility = Visibility.Collapsed;
			}
		});
		WeakReferenceMessenger.Default.Register<MovementFormulaAppliedMessage>(this, ( r, m ) => {
			MFVM.Visibility = Visibility.Collapsed;
			if( TrsfVM.Visibility == Visibility.Collapsed ) {
				IsSettingVisibility = Visibility.Collapsed;
			}
			if( currentJointMF == null ) {
				return;
			}
			currentJointMF.MovementFormula = MFVM.TheMF;
		});
		WeakReferenceMessenger.Default.Register<MovementFormulaCanceledMessage>(this, ( r, m ) => {
			MFVM.Visibility = Visibility.Collapsed;
			if( TrsfVM.Visibility == Visibility.Collapsed ) {
				IsSettingVisibility = Visibility.Collapsed;
			}
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
				Connections = Connections,
				Movements = Movements,
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

	#endregion

	#region View

	[ObservableProperty]
	private Visibility isSettingVisibility;

	public List<MovementFormula> AvailableMovements { get; }

	public ObservableCollection<PartJointVM> Joints { get; }

	public List<Trsf> Connections { get; }

	public List<MovementFormula> Movements { get; }

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

	private PartJointVM? currentJointTrsf;
	private PartJointVM? currentJointMF;

	#endregion

	#region Command

	[RelayCommand]
	private void AddNewJoint( PartJointVM? joint ) {
		//WeakReferenceMessenger.Default.Send(new AddNewJointMessage( ));
		if( joint == null ) {
			if( Joints.Count == 0 ) {
				Joints.Add(new PartJointVM(1, new( ), MovementFormula.Static));
			}
			return;
		}
		var num=Joints.IndexOf(joint);
		Joints.Insert(num + 1, new PartJointVM(num + 1, new( ), MovementFormula.Static));
		for( int i = 0; i < Joints.Count; i++ ) {
			Joints[i].ID = i + 1;
		}
	}

	[RelayCommand]
	private void RemoveJoint( PartJointVM joint ) {
		Joints.Remove(joint);
		for( int i = 0; i < Joints.Count; i++ ) {
			Joints[i].ID = i + 1;
		}
	}

	[RelayCommand]
	private void SetTrsf( PartJointVM joint ) {
		TrsfVM.Visibility = Visibility.Visible;
		IsSettingVisibility = Visibility.Visible;
		currentJointTrsf = joint;
		TrsfVM.TheTrsf = joint.Connection;
	}

	[RelayCommand]
	private void SetMF( PartJointVM joint ) {
		MFVM.Visibility = Visibility.Visible;
		IsSettingVisibility = Visibility.Visible;
		currentJointMF = joint;
		MFVM.TheMF = joint.MovementFormula;
	}

	#endregion

}

public partial class PartJointVM:ObservableObject {
	public PartJointVM( int id, Trsf connection, MovementFormula mf ) {
		ID = id;
		Connection = connection;
		MovementFormula = mf;
	}
	[ObservableProperty]
	private int iD;
	[ObservableProperty]
	private Trsf connection;
	[ObservableProperty]
	private MovementFormula movementFormula;
}

public class DesignTimeJointsViewModel {
	public ObservableCollection<PartJointVM> Joints { get; set; }

	public DesignTimeJointsViewModel( ) {
		Joints = new( )
		{
			new ( 1,new Trsf(new(),new Pnt(1,2,3)), MovementFormula.Static),
			new ( 2,new Trsf(new(),new Pnt(2,3,4)), MovementFormula.dX_Minus),
			new ( 3,new Trsf(new(),new Pnt(3,4,5)), MovementFormula.dY_Plus),
		};
	}
}
