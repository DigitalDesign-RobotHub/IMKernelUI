using CommunityToolkit.Mvvm.Messaging.Messages;

using IMKernel.Visualization;

namespace IMKernelUI.Message;


public record Main3DContextChangedMessage( ThreeDimensionContext Context );

public class Main3DContextRequestMessage( ):RequestMessage<ThreeDimensionContext?>;
