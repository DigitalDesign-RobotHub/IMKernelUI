using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.OCCExtension;
using IMKernel.Utils;
using IMKernel.Visualization;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

using OCCTK.OCC.AIS;
using OCCTK.OCC.gp;

namespace IMKernelUI.ViewModel;


public partial class TrsfViewModel:ObservableObject, IOCCFinilize {
	public TrsfViewModel( ) {
		//value
		currentRotationFormula = RotationFormula.Euler_WPR;
		backupTrsf = new( );
		theTrsf = new( );

		//view
		RotationFormulas = new( ) { RotationFormula.Euler_WPR };
		Visibility = Visibility.Visible;
		IsSetting = true;

		//occ
		ATri = new ATrihedron(10);

		#region Message

		//occ
		context = WeakReferenceMessenger.Default.Send<Main3DContextRequestMessage>( );
		WeakReferenceMessenger.Default.Register<Main3DContextChangedMessage>(this, ( r, m ) => {
			context = m.Context;
		});

		//command
		var cmMessage = WeakReferenceMessenger.Default.Send<CommandManagerRequestMessage>( );
		commandManager = cmMessage.Response;

		#endregion

	}

	#region View

	/// <summary>
	/// 是否开启设置
	/// </summary>
	[ObservableProperty]
	private bool isSetting;

	/// <summary>
	/// 控件的可见性
	/// </summary>
	[ObservableProperty]
	private Visibility visibility;

	/// <summary>
	/// 交互按钮的可见性
	/// </summary>
	public Visibility IsSettingVisbility { get; set; }

	#endregion

	#region Value

	private Trsf theTrsf;

	public Trsf TheTrsf {
		get {
			//! 取值必须创建新对象，否则传出的是托管地址
			return (Trsf)theTrsf.Clone( );
		}
		set {
			backupTrsf = value;
			(X, Y, Z) = value.Translation;
			(W, P, R) = value.Rotation.ToEuler(EulerSequence.WPR);
		}
	}

	#region 平移

	[ObservableProperty]
	private double x;

	partial void OnXChanged( double value ) {
		try {
			SetTranslation(0, value);
			UpdateATri( );
			ApplySettingCommand.NotifyCanExecuteChanged( );
			//通知绑定整个对象发生了变化
			OnPropertyChanged(string.Empty);
		} catch( Exception ) {
			Context?.Erase(ATri, true);
		}
	}

	[ObservableProperty]
	private double y;

	partial void OnYChanged( double value ) {
		try {
			SetTranslation(1, value);
			UpdateATri( );
			ApplySettingCommand.NotifyCanExecuteChanged( );
			//通知绑定整个对象发生了变化
			OnPropertyChanged(string.Empty);
		} catch( Exception ) {
			Context?.Erase(ATri, true);
		}
	}

	[ObservableProperty]
	private double z;

	partial void OnZChanged( double value ) {
		try {
			SetTranslation(2, value);
			UpdateATri( );
			ApplySettingCommand.NotifyCanExecuteChanged( );
			//通知绑定整个对象发生了变化
			OnPropertyChanged(string.Empty);
		} catch( Exception ) {
			Context?.Erase(ATri, true);
		}
	}

	private double GetTranslation( int index ) {
		var (x, y, z) = TheTrsf.Translation;
		return index switch {
			0 => x,
			1 => y,
			2 => z,
			_ => throw new ArgumentOutOfRangeException(nameof(index))
		};
	}

	private void SetTranslation( int index, double value ) {
		var (x, y, z) = theTrsf.Translation;
		(x, y, z) = index switch {
			0 => (value, y, z),
			1 => (x, value, z),
			2 => (x, y, value),
			_ => throw new ArgumentOutOfRangeException(nameof(index))
		};

		var newTranslation = new Vec(x,y,z);
		var newRotation = theTrsf.Rotation;

		theTrsf = new Trsf(newTranslation, newRotation);
	}

	#endregion

	#region 旋转

	[ObservableProperty]
	public ObservableCollection<RotationFormula> rotationFormulas;

	[ObservableProperty]
	private RotationFormula currentRotationFormula;

	#region WPR

	[ObservableProperty]
	private double w;

	partial void OnWChanged( double value ) {
		try {
			SetWPR(0, value.ToRadians( ));
			UpdateATri( );
			ApplySettingCommand.NotifyCanExecuteChanged( );
			//通知绑定整个对象发生了变化
			OnPropertyChanged(string.Empty);
		} catch( Exception ) {
			Context?.Erase(ATri, true);
		}
	}

	[ObservableProperty]
	private double p;

	partial void OnPChanged( double value ) {
		try {
			SetWPR(1, value.ToRadians( ));
			UpdateATri( );
			ApplySettingCommand.NotifyCanExecuteChanged( );
			//通知绑定整个对象发生了变化
			OnPropertyChanged(string.Empty);

		} catch( Exception ) {
			Context?.Erase(ATri, true);
		}
	}

	[ObservableProperty]
	private double r;

	partial void OnRChanged( double value ) {
		try {
			SetWPR(2, value.ToRadians( ));
			UpdateATri( );
			ApplySettingCommand.NotifyCanExecuteChanged( );
			//通知绑定整个对象发生了变化
			OnPropertyChanged(string.Empty);
		} catch( Exception ) {
			Context?.Erase(ATri, true);
		}
	}

	private void SetWPR( int index, double value ) {
		var q = theTrsf.Rotation;
		var (w, p, r) = q.ToEuler(EulerSequence.Intrinsic_XYZ);
		(w, p, r) = index switch {
			0 => (value, p, r),
			1 => (w, value, r),
			2 => (w, p, value),
			_ => throw new ArgumentOutOfRangeException(nameof(index))
		};

		var newTranslation = theTrsf.Translation;
		var newRotation = new Quat(w, p, r, EulerSequence.Intrinsic_XYZ);
		theTrsf = new Trsf(newTranslation, newRotation);
	}

	#endregion

	#region Quat

	//public double QX
	//{
	//    get => GetQuat(0);
	//    set => SetQuat(0, value);
	//}
	//public double QY
	//{
	//    get => GetQuat(1);
	//    set => SetQuat(1, value);
	//}
	//public double QZ
	//{
	//    get => GetQuat(2);
	//    set => SetQuat(2, value);
	//}
	//public double QW
	//{
	//    get => GetQuat(3);
	//    set => SetQuat(3, value);
	//}

	//private double GetQuat(int Index)
	//{
	//    var quat = TheTrsf.Rotation;
	//    double x = quat.X;
	//    double y = quat.Y;
	//    double z = quat.Z;
	//    double w = quat.W;
	//    return Index switch
	//    {
	//        0 => x,
	//        1 => y,
	//        2 => z,
	//        3 => w,
	//        _ => throw new ArgumentOutOfRangeException(nameof(Index))
	//    };
	//}

	//private void SetQuat(int Index, double value)
	//{
	//    var quat = TheTrsf.Rotation;
	//    double x = quat.X;
	//    double y = quat.Y;
	//    double z = quat.Z;
	//    double w = quat.W;
	//    (x, y, z, w) = Index switch
	//    {
	//        0 => (value, y, z, w),
	//        1 => (x, value, z, w),
	//        2 => (x, y, value, w),
	//        3 => (x, y, z, value),
	//        _ => throw new ArgumentOutOfRangeException(nameof(Index))
	//    };
	//    TheTrsf.SetRotationPart(new Quat(x, y, z, w));
	//}
	#endregion

	#endregion

	#endregion

	#region 三维显示

	public void OCCFinilize( ) {
		try {
			ATri.RemoveSelf(true);
			Update( );
		} catch( Exception ) {
			throw;
		}
	}

	private InteractiveObject ATri { get; }

	private ThreeDimensionContext? context;
	public ThreeDimensionContext? Context {
		protected get { return context; }
		set {
			context = value;
			if( context != null && ATri != null ) {
				context.Display(ATri, true);
				context.AISContext.SetSelectionMode(ATri, OCCTK.OCC.AIS.SelectionMode.None);
			}
		}
	}

	~TrsfViewModel( ) {
		OCCFinilize( );
	}

	private OCCCanvas? OCCCanvas { get; }

	#region 简化代码
	private Action Update {
		get {
			return Context != null
				? Context.Update
				: throw new NotImplementedException("需要传入3D上下文管理器");
		}
	}
	private Action<InteractiveObject, bool> Display {
		get {
			return Context != null
				? Context.Display
				: throw new NotImplementedException("需要传入3D上下文管理器");
		}
	}
	private Action<InteractiveObject, bool> Redisplay {
		get {
			return Context != null
				? Context.Redisplay
				: throw new NotImplementedException("需要传入3D上下文管理器");
		}
	}
	private Func<InteractiveObject, bool> IsDisplayed {
		get {
			if( Context != null ) {
				return Context.AISContext.IsDisplayed;
			} else {
				throw new NotImplementedException("需要传入3D上下文管理器");
			}
		}
	}
	#endregion

	private void UpdateATri( ) {
		if( Context == null ) {
			return;
		}
		ATri.SetLocalTransformation(TheTrsf);
		if( IsDisplayed(ATri) ) {
			Display(ATri, true);
		} else {
			Redisplay(ATri, true);
		}
		Update( );
	}

	#endregion

	#region command

	private readonly ICommandManager commandManager;

	/// <summary>
	/// 用于撤销的变换的值
	/// </summary>
	private Trsf backupTrsf;

	/// <summary>
	/// 应用设置
	/// </summary>
	[RelayCommand(CanExecute = nameof(CanApplySetting))]
	private void ApplySetting( ) {
		OCCCanvas?.Detach( );
		WeakReferenceMessenger.Default.Send(new TrsfAppliedMessage( ));
	}

	private bool CanApplySetting( ) {
		return true;
	}

	/// <summary>
	/// 取消设置
	/// </summary>
	[RelayCommand]
	private void CancelSetting( ) {
		OCCCanvas?.Detach( );
		TheTrsf = backupTrsf;
		WeakReferenceMessenger.Default.Send(new TrsfSetCanceledMessage( ));
	}

	#endregion

}

/// <summary>
/// 转换器
/// </summary>
[ValueConversion(typeof(RotationFormula), typeof(String))]
public class EnumToVisibilityConverter:IValueConverter {
	public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
		if( value is RotationFormula formula && parameter is string targetFormula ) {
			return formula.ToString( ) == targetFormula ? Visibility.Visible : Visibility.Collapsed;
		}
		return Visibility.Collapsed;
	}

	public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
		throw new NotImplementedException( );
	}
}
