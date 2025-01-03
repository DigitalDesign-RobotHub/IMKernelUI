using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevExpress.Mvvm.POCO;

using IMKernel.Interfaces;
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
/// 部件View Model
/// </summary>
/// <remarks>只包含静态部分的定义</remarks>
public partial class ComponentPropertiesViewModel:ObservableObject, IOCCFinilize, IComponent {
	public ComponentPropertiesViewModel( ) {
		//value
		Name = "";
		PartVM = new( );
		Parent = new OriginComponent( );
		Components = new( );
		Components.Add(new OriginComponent( ));
	}

	#region OCC

	public void OCCFinilize( ) {
		PartVM?.OCCFinilize( );
	}


	#endregion

	#region View

	public bool IsComponentValid {
		get {
			return true;
		}
	}

	public ObservableCollection<IComponent> Components { get; }

	#endregion

	#region Value

	public Component TheComponent {
		get {
			return new(Name, PartVM.ThePart, Parent);
		}
		set {
			//todo 
		}
	}


	/// <summary>
	/// 零件VM
	/// </summary>
	[ObservableProperty]
	private PartViewModel partVM;

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
