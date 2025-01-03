using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging.Messages;

using IMKernel.Visualization;

namespace IMKernelUI.Message;
public record ViewStatusChangedMessage( (int contextID, bool viewCube, bool orignTri, bool viewTri) Value );
public record CanvasCreatedMessage( (int contextID, bool viewCube, bool orignTri, bool viewTri) Value );

public record MainCanvasChangedMessage( OCCCanvas Canvas );
public class MainCanvasRequestMessage( ):RequestMessage<OCCCanvas>;