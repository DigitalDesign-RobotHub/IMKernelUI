using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Interfaces;
using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

using log4net;

namespace IMKernelUI.ViewModel;

public partial class PoseViewModel:ObservableObject, IOCCFinilize {
	private static readonly ILog log = LogManager.GetLogger(typeof(PoseViewModel));
	public PoseViewModel( ) {
		//value
		this.ReferPose = ORIGIN.Instance;
		this.backupPose = ORIGIN.Instance;
		this.TrsfVM = new( );
		//view
		this.References = [ ];
		this.TrsfVM.IsSettingVisbility = Visibility.Collapsed;

		#region Message

		//view
		WeakReferenceMessenger.Default.Register<ReferencePosesChangedMessage>(this, ( r, m ) => {
			if( m.Refer.Count != 0 ) {
				this.References.Clear( );
				m.Refer.ForEach(x => this.References.Add(x));
			}
		});

		//occ
		this.context = WeakReferenceMessenger.Default.Send<Main3DContextRequestMessage>( );
		WeakReferenceMessenger.Default.Register<Main3DContextChangedMessage>(this, ( r, m ) => {
			this.context = m.Context;
		});

		#endregion
	}

	#region 三维显示

	private ThreeDimensionContext? context;

	public void OCCFinilize( ) {
		this.TrsfVM.OCCFinilize( );
	}

	#endregion

	#region Value

	public Pose ThePose {
		get {
			return new Pose(this.TrsfVM.TheTrsf, this.ReferPose);
		}
		set {
			this.ReferPose = value.Reference;
			this.TrsfVM.TheTrsf = value.Datum.Transform;
		}
	}

	/// <summary>
	/// Trsf
	/// </summary>
	[ObservableProperty]
	public TrsfViewModel trsfVM;

	partial void OnTrsfVMChanged( TrsfViewModel value ) {
		//todo 更新
		//ApplySettingCommand.NotifyCanExecuteChanged( );
		//通知绑定整个对象发生了变化
		OnPropertyChanged(string.Empty);
	}

	/// <summary>
	/// 参考位姿
	/// </summary>
	[ObservableProperty]
	private IPose referPose;

	partial void OnReferPoseChanged( IPose value ) {
		//todo occ更新
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
	private readonly IPose backupPose;

	//[RelayCommand(CanExecute = nameof(CanApplySetting))]
	private void ApplySetting( ) {
		//OCCCanvas?.Detach( );
		WeakReferenceMessenger.Default.Send(new PoseAppliedMessage( ));
	}


	//private bool CanApplySetting( ) {
	//	return true;
	//}

	[RelayCommand]
	private void CancelSetting( ) {
		//OCCCanvas?.Detach( );
		if( this.backupPose is Pose p ) {
			this.ThePose = p;
		}
		WeakReferenceMessenger.Default.Send(new PoseSetCanceledMessage( ));
	}

	#endregion

}
