using System.Windows.Controls;

using CommunityToolkit.Mvvm.Messaging;

using DevExpress.Xpf.LayoutControl;

using IMKernelUI.ViewModel;
using IMKernelUI.Message;

using UserControl = System.Windows.Controls.UserControl;

namespace IMKernelUI.View;

/// <summary>
/// Interaction logic for ComponentProperties.xaml
/// </summary>
public partial class ComponentPropertiesView:UserControl {
	public ComponentPropertiesView( ) {
		InitializeComponent( );
	}
	public void UpdateConnections( ) {
		Connections_LayoutControl.Children.Clear( );
	}

	private void Connection_00_TextEdit_DataContextChanged( object sender, System.Windows.DependencyPropertyChangedEventArgs e ) {
		//WeakReferenceMessenger.Default.Send(new ComponentConnectionNameChangedMessage( ));
	}
}
