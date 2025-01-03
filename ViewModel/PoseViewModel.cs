using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

namespace IMKernelUI.ViewModel;

public partial class PoseViewModel:ObservableObject, IOCCFinilize {

	public PoseViewModel( ) {
		//value
		Name = "";
		ReferPose = OriginPose.ToPose( );
		TrsfVM = new( );
		//view
		References = new( );
		TrsfVM.IsSettingChanged += value => IsSetting = value;

		#region Message

		//view
		WeakReferenceMessenger.Default.Register<ReferencePosesChangedMessage>(this, ( r, m ) => {
			if( m.Refer.Count != 0 ) {
				References.Clear( );
				m.Refer.ForEach(x => References.Add(x));
			}
		});

		//occ
		context = WeakReferenceMessenger.Default.Send<Main3DContextRequestMessage>( );
		WeakReferenceMessenger.Default.Register<Main3DContextChangedMessage>(this, ( r, m ) => {
			context = m.Context;
		});

		#endregion
	}

	#region 三维显示

	public void OCCFinilize( ) {
		TrsfVM.OCCFinilize( );
	}

	private ThreeDimensionContext? context;

	#endregion

	#region Value

	public Pose ThePose {
		get {
			return new Pose(Name, TrsfVM.TheTrsf, ReferPose);
		}
		set {
			Name = value.Name;
			ReferPose = value.Reference;
			TrsfVM.TheTrsf = value.Transfrom;
		}
	}

	[ObservableProperty]
	public TrsfViewModel trsfVM;

	[ObservableProperty]
	private string name;

	[ObservableProperty]
	private Pose referPose;

	#endregion

	#region View

	[ObservableProperty]
	private bool isSetting;

	partial void OnIsSettingChanged( bool value ) {
		if( value ) {
			IsSettingVisbility = Visibility.Visible;
		} else {
			IsSettingVisbility = Visibility.Collapsed;
		}
	}

	[ObservableProperty]
	private Visibility myVisibility;

	public Visibility IsSettingVisbility { get; protected set; }

	/// <summary>
	/// 可作为参考的坐标系
	/// </summary>
	private ObservableCollection<Pose> References { get; }

	#endregion

}
