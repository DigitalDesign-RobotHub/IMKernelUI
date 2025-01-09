using IMKernelUI.Interfaces;

namespace IMKernelUI.Message;

public class CommandManagerRequestMessage {

	//! 接口类型不能进行隐式转换，无法直接用工具包
	public CommandManagerRequestMessage( ) {

	}
	private ICommandManager? _response;

	public ICommandManager Response {
		get => _response ?? throw new System.Exception("消息没有注册Reply");
		set => _response = value;
	}
	public void Reply( ICommandManager commandManager ) {
		Response = commandManager;
	}
}
