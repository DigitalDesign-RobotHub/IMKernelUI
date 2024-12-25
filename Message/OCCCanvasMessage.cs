using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMKernelUI.Message;
public record ViewStatusChangedMessage( (int contextID, bool viewCube, bool orignTri, bool viewTri) Value );
public record CanvasCreatedMessage( (int contextID, bool viewCube, bool orignTri, bool viewTri) Value );

