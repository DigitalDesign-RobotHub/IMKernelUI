using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
		backupPose = OriginPose.ToPose( );
		TrsfVM = new( );
		//view
		References = new( );
		TrsfVM.IsSettingVisbility = Visibility.Collapsed;

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

	private ThreeDimensionContext? context;

	public void OCCFinilize( ) {
		TrsfVM.OCCFinilize( );
	}

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

	/// <summary>
	/// Trsf
	/// </summary>
	[ObservableProperty]
	public TrsfViewModel trsfVM;

	partial void OnTrsfVMChanged( TrsfViewModel value ) {
		ApplySettingCommand.NotifyCanExecuteChanged( );
		//通知绑定整个对象发生了变化
		OnPropertyChanged(string.Empty);
	}

	/// <summary>
	///	名称
	/// </summary>
	[ObservableProperty]
	private string name;

	partial void OnNameChanged( string value ) {
		ApplySettingCommand.NotifyCanExecuteChanged( );
		//通知绑定整个对象发生了变化
		OnPropertyChanged(string.Empty);
	}

	/// <summary>
	/// 参考位姿
	/// </summary>
	[ObservableProperty]
	private Pose referPose;

	partial void OnReferPoseChanged( Pose value ) {
		//todo occ
	}

	#endregion

	#region View

	/// <summary>
	/// 是否开启设置
	/// </summary>
	[ObservableProperty]
	private bool isSetting;

	partial void OnIsSettingChanging( bool value ) {
		TrsfVM.IsSetting = value;
	}

	/// <summary>
	/// 控件的可见性
	/// </summary>
	[ObservableProperty]
	private Visibility visibility;

	/// <summary>
	/// 交互按钮是否可见
	/// </summary>
	public Visibility IsSettingVisbility { get; set; }
	partial void OnIsSettingChanged( bool value ) {
		if( value ) {
			IsSettingVisbility = Visibility.Visible;
		} else {
			IsSettingVisbility = Visibility.Collapsed;
		}
	}

	/// <summary>
	/// 可作为参考的坐标系
	/// </summary>
	public ObservableCollection<Pose> References { get; }

	#endregion

	#region Command


	/// <summary>
	/// 用于撤销设置的值
	/// </summary>
	private Pose backupPose;

	[RelayCommand(CanExecute = nameof(CanApplySetting))]
	private void ApplySetting( ) {
		//OCCCanvas?.Detach( );
		WeakReferenceMessenger.Default.Send(new PoseAppliedMessage( ));
	}


	private bool CanApplySetting( ) {
		if( Name == "" ) {
			return false;
		}
		return true;
	}

	[RelayCommand]
	private void CancelSetting( ) {
		//OCCCanvas?.Detach( );
		ThePose = backupPose;
		WeakReferenceMessenger.Default.Send(new PoseSetCanceledMessage( ));
	}

	#endregion

}
