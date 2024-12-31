using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;

using OCCTK.OCC.gp;

using Windows.Services.Maps;

namespace IMKernelUI.ViewModel;

public partial class PoseViewModel:ObservableObject, IOCCFinilize {

	public PoseViewModel( Pose pose, ObservableCollection<Pose> references, OCCCanvas? canvas = null ) {
		inputPose = pose;
		Name = pose.Name;
		ReferPose = pose.Reference;
		Transfrom = pose.Transfrom;
		References = references;
		TrsfViewModel = new(Transfrom, canvas);
	}

	public Pose ThePose {
		get {
			if( inputPose != null ) {
				return inputPose;
			} else {
				try {
					return new(Name, Transfrom, ReferPose);
				} catch( Exception e ) {
					throw new Exception($"Pose输入信息有误，创建失败 {e.Message}");
				}
			}
		}
	}

	private Pose? inputPose;

	#region 三维显示
	private ThreeDimensionContext? context;
	public ThreeDimensionContext? Context {
		protected get { return context; }
		set {
			context = value;
			TrsfViewModel.Context = value;
		}
	}
	#endregion

	[ObservableProperty]
	public TrsfViewModel trsfViewModel;

	[ObservableProperty]
	private string name;

	[ObservableProperty]
	private Pose? referPose;

	[ObservableProperty]
	private Trsf transfrom;

	/// <summary>
	/// 可作为参考的坐标系
	/// </summary>
	[ObservableProperty]
	private ObservableCollection<Pose> references;
	public void OCCFinilize( ) {
		TrsfViewModel.OCCFinilize( );
	}
}
