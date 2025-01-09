using System.Windows.Input;

namespace IMKernelUI.Interfaces;

public interface ICommandManager {
	public void Execute( IUndoCommand command, object? parameter = null );
	public void Execute( ICommand command, object? parameter = null );
	public void Undo( );
}
