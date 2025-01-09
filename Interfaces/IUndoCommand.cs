using System;

using CommunityToolkit.Mvvm.Input;

namespace IMKernelUI.Interfaces;

public interface IUndoCommand:IRelayCommand {
	string Description { get; }
	void Undo( );
}
