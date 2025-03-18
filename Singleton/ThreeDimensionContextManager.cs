using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Visualization;

using IMKernelUI.Message;

using OCCTK.OCC.AIS;
using OCCTK.OCC.OpenGL;
using OCCTK.OCC.V3d;

namespace IMKernelUI.Singleton;

/// <summary>
/// Viewer管理器作为全局单例，负责绘制引擎和上下文的创建和管理
/// </summary>
public class ThreeDimensionContextManager {
	public ThreeDimensionContextManager( ) {
		this.Contexts = [ ];
		this.contextID = 0;
		this.IsShowViewCube = true;
		this.IsShowOriginTrihedron = false;
		this.IsShowViewTrihedron = false;
		this.IsShowGraduatedTrihedron = false;
		WeakReferenceMessenger.Default.Register<Main3DContextRequestMessage>(this, ( r, m ) => {
			m.Reply(this.MainContext);
		});
	}

	public bool IsShowViewCube { get; set; }
	public bool IsShowOriginTrihedron { get; set; }
	public bool IsShowViewTrihedron { get; set; }
	public bool IsShowGraduatedTrihedron { get; set; }

	public List<ThreeDimensionContext> Contexts { get; set; }

	/// <summary>
	/// 默认的画布，列表第一项
	/// </summary>
	public ThreeDimensionContext? MainContext => this.Contexts.FirstOrDefault( );
	private int contextID;
	public ThreeDimensionContext CreateContext( ) {
		ThreeDimensionContext context;
		try {
			//创建图形引擎
			GraphicDriver graphicDriver = new();
			//创建视图对象
			Viewer viewer = new(graphicDriver);
			//创建AIS上下文管理器
			InteractiveContext anAISContext = new(viewer);
			//创建三维画布上下文对象
			context = new(anAISContext, this.contextID);
			this.Contexts.Add(context);
			if( this.Contexts.Count == 1 )
				WeakReferenceMessenger.Default.Send(new Main3DContextChangedMessage(context));
		} catch( Exception e ) {
			throw new Exception($"画布创建失败：{e}");
		}

		//初始化画布状态
		context.DisplayViewCube(this.IsShowViewCube);
		context.DisplayViewTrihedron(this.IsShowViewTrihedron);
		context.DisplayOriginTrihedron(this.IsShowOriginTrihedron);
		WeakReferenceMessenger.Default.Send(new CanvasCreatedMessage((this.contextID, this.IsShowViewCube, this.IsShowOriginTrihedron, this.IsShowViewTrihedron)));

		//完成后计数器+1
		this.contextID++;
		return context;
	}
}
