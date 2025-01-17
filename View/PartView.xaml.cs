using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using IMKernelUI.Message;

namespace IMKernelUI.View;
/// <summary>
/// PartView.xaml 的交互逻辑
/// </summary>
public partial class PartView:UserControl {
	public PartView( ) {
		InitializeComponent( );
		WeakReferenceMessenger.Default.Register<AddNewJointMessage>(this, ( r, m ) => {
			AddNewJointToConnectionsGrid( );
		});
	}

	private void AddNewJointToConnectionsGrid( ) {
		// 假设 Connections_Grid 是你的 Grid 的名称
		int newRow = Connections_Grid.RowDefinitions.Count;
		//删掉第四行
        if (newRow==4)
        {
            Connections_Grid.Children.RemoveAt(3);
		}

		// 添加新的 RowDefinition
		Connections_Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		Connections_Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		Connections_Grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		// 创建 编号标签
		Label label = new Label
	{
			Padding = new Thickness(2),
			HorizontalContentAlignment = HorizontalAlignment.Center,
			VerticalContentAlignment = VerticalAlignment.Center,
			Background = new SolidColorBrush(Color.FromRgb(255, 252, 204)), // "#ffc"
			Content = "0"
		};
		Grid.SetRow(label, newRow + 1);
		Grid.SetRowSpan(label, 2);
		Connections_Grid.Children.Add(label);

		// 创建 Trsf 按钮
		Button trsfButton = new Button
	{
			Padding = new Thickness(2),
			Command = DataContext.GetType().GetProperty("SetTrsfCommand")?.GetValue(DataContext) as ICommand,
			CommandParameter = newRow,
			Content = "xyz: (4,5,6), wpr: (40,0,90)"
		};
		Grid.SetRow(trsfButton, newRow);
		Grid.SetColumn(trsfButton, 2);
		Connections_Grid.Children.Add(trsfButton);

		// 创建 MovementFormula 按钮
		Button movementFormulaButton = new Button
	{
			Padding = new Thickness(2),
			Command = DataContext.GetType().GetProperty("SetMFCommand")?.GetValue(DataContext) as ICommand,
			CommandParameter = newRow + 2,
			Content = "运动类型"
		};
		Grid.SetRow(movementFormulaButton, newRow + 2);
		Grid.SetColumn(movementFormulaButton, 2);
		Connections_Grid.Children.Add(movementFormulaButton);

		// 创建➕按钮
		Button addButton = new Button
	{
			Padding = new Thickness(0),
			HorizontalContentAlignment = HorizontalAlignment.Center,
			VerticalContentAlignment = VerticalAlignment.Center,
			Background = Brushes.LightGray,
			Content = "➕",
			Foreground = FindResource("OK_Brush") as Brush
		};
		Grid.SetRow(addButton, newRow);
		Grid.SetRowSpan(addButton, 2);
		Grid.SetColumn(addButton, 3);
		Connections_Grid.Children.Add(addButton);

		// 创建➖按钮
		Button removeButton = new Button
	{
			Padding = new Thickness(2),
			HorizontalContentAlignment = HorizontalAlignment.Center,
			VerticalContentAlignment = VerticalAlignment.Center,
			Background = Brushes.LightGray,
			Content = "➖",
			Foreground = FindResource("Cancel_Brush") as Brush
		};
		Grid.SetRow(removeButton, newRow);
		Grid.SetRowSpan(removeButton, 2);
		Grid.SetColumn(removeButton, 4);
		Connections_Grid.Children.Add(removeButton);
	}

}

