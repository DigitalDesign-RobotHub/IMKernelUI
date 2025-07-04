﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Messaging;

using IMKernelUI.Interfaces;
using IMKernelUI.Message;

using log4net;

namespace IMKernelUI.Singleton;

public class CommandManager:ICommandManager {
	private static readonly ILog log = LogManager.GetLogger(typeof(CommandManager));
	private readonly Stack<IUndoCommand> UndoStack;

	public CommandManager( ) {
		this.UndoStack = new Stack<IUndoCommand>( );

		#region Message
		WeakReferenceMessenger.Default.Register<CommandManagerRequestMessage>(this, ( r, m ) => {
			m.Reply(this);
		});
		#endregion
	}

	public void Execute( IUndoCommand command, object? parameter = null ) {
		try {
			command.Execute(parameter);
		} catch( Exception e ) {
			log.Error(e);
			MessageBox.Show($"！！执行失败！！\n{e.Message}");
		}
		this.UndoStack.Push(command);
	}

	public void Execute( ICommand command, object? parameter = null ) {
		try {
			command.Execute(parameter);
		} catch( Exception e ) {
			log.Error(e);
			MessageBox.Show($"！！执行失败！！\n{e.Message}");
		}
	}

	public void Undo( ) {
		if( this.UndoStack.Count == 0 )
			return;

		var lastCommand = this.UndoStack.Pop();
		log.Debug($"Undo: {lastCommand.Description}");
		lastCommand.Undo( );
	}
}
