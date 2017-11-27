using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace JewishOrganizer
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

		private void Forward_Click(Object sender, RoutedEventArgs e)
		{
			if (rootPivot.SelectedIndex < rootPivot.Items.Count - 1)
			{
				// If not at the last item, go to the next one.
				rootPivot.SelectedIndex += 1;
			}
			else
			{
				// The last PivotItem is selected, so loop around to the first item.
				rootPivot.SelectedIndex = 0;
			}
		}

		private void Back_Click(Object sender, RoutedEventArgs e)
		{
			if (rootPivot.SelectedIndex > 0)
			{
				// If not at the first item, go back to the previous one.
				rootPivot.SelectedIndex -= 1;
			}
			else
			{
				// The first PivotItem is selected, so loop around to the last item.
				rootPivot.SelectedIndex = rootPivot.Items.Count - 1;
			}
		}
	}
}
