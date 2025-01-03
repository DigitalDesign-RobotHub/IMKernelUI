using System.Collections.Generic;

using IMKernel.OCCExtension;

namespace IMKernelUI.Message;

public record ReferencePosesChangedMessage( List<Pose> Refer );