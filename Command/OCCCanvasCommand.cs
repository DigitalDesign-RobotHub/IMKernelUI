using System;

using IMKernelUI.Interfaces;

using OCCTK.Extension;

namespace IMKernelUI.Command;

#region 视角控制

public class RotationCommand:IUndoCommand {
	private CameraOrientation? lastCameraOrientation;
	private OCCTK.OCC.V3d.View? undoView;

	public RotationCommand( ) {
		Description = "旋转";
	}

	public string Description { get; protected set; }

	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter ) {
		if( parameter is ValueTuple<CameraOrientation, OCCTK.OCC.V3d.View> tuple ) {
			lastCameraOrientation = tuple.Item1;
			undoView = tuple.Item2;
		} else {
			throw new NotImplementedException("Rotation Command需要传入记录的Camera和View");
		}
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}

	public void Undo( ) {
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}
}

public class FrontViewCommand:IUndoCommand {
	public string Description => "设置视角正视";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Front, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Front View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

public class BackViewCommand:IUndoCommand {
	public string Description => "设置视角后视";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Back, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Back View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

public class TopViewCommand:IUndoCommand {
	public string Description => "设置视角俯视";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Top, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Top View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

public class BottomViewCommand:IUndoCommand {
	public string Description => "设置视角仰视";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Bottom, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Bottom View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

public class LeftViewCommand:IUndoCommand {
	public string Description => "设置视角左视";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Left, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Left View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

public class RightViewCommand:IUndoCommand {
	public string Description => "设置视角右视";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Right, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Right View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

public class FitAllCommand:IUndoCommand {
	public string Description => "设置视角等轴测";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.FitAll(0.1, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Axo View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

#endregion

#region 大小控制

public class AxoViewCommand:IUndoCommand {
	public string Description => "设置视角等轴测";
	private OCCTK.OCC.V3d.View? undoView;
	private CameraOrientation? lastCameraOrientation;
	public event EventHandler? CanExecuteChanged;

	public bool CanExecute( object? parameter ) {
		return true;
	}

	public void Execute( object? parameter = null ) {
		if( parameter is OCCTK.OCC.V3d.View view ) {
			lastCameraOrientation = view.CurrentViewOrientation( );
			view.SetViewOrientation(OCCTK.OCC.V3d.ViewOrientation.Axo, true);
			undoView = view;
		} else {
			throw new NotImplementedException("Axo View Command需要传入View");
		}
	}

	public void Undo( ) {
		if( lastCameraOrientation == null )
			return;
		undoView?.SetViewOrientation(lastCameraOrientation, true);
	}

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}
}

#endregion
