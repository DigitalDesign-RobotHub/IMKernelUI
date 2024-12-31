using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Kinematic;
using IMKernel.Model;
using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;
using IMKernelUI.ViewModel;

using OCCTK.Extension;
using OCCTK.OCC.gp;

using Component = IMKernel.Model.Component;

namespace IMKernelUI.ViewModel;

/// <summary>
/// 部件(静态)View Model
/// </summary>
public partial class ComponentPropertiesViewModel:ObservableObject, IOCCFinilize {
	public ComponentPropertiesViewModel( ) {
		//canvas = WeakReferenceMessenger.Default.Send<MainCanvasRequestMessage>( );

		Name = "";
		Parent = new OriginComponent( );
	}

	public bool IsComponentValid {
		get {
			return true;
		}
	}
	public void OCCFinilize( ) {
		ThePartViewModel?.OCCFinilize( );
	}
	#region Component属性
	public Component TheComponent {
		get {
			//Trsf tWithParent;
			//if( Parent == null ) {
			//	tWithParent = PoseViewModel.ThePose.Datum;
			//} else {
			//	tWithParent =
			//		-( Parent.Datum * Parent.Connection[ParentConnection] ) * PoseViewModel.ThePose.Datum;
			//}
			return new Component( );
		}
	}
	//public ThreeDimensionContext? Context { protected get; set; }
	//private OCCCanvas? canvas;

	/// <summary>
	/// 零件VM
	/// </summary>
	[ObservableProperty]
	private PartViewModel? thePartViewModel;

	/// <summary>
	/// 部件名称
	/// </summary>
	[ObservableProperty]
	private string name;

	partial void OnNameChanged( string value ) {
		WeakReferenceMessenger.Default.Send(new ComponentPropertyChangedMessage( ));
	}

	/// <summary>
	/// 父部件
	/// </summary>
	[ObservableProperty]
	private IComponent parent;

	/// <summary>
	/// 父部件连接点
	/// </summary>
	[ObservableProperty]
	private int parentConnection;

	#endregion

	#region command
	#endregion

}
