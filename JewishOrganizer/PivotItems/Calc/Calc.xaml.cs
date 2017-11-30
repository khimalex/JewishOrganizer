using System.Collections.Specialized;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JewishOrganizer.PivotItems
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Calc : Page
	{
		public Calc()
		{
			this.DataContext = new CalcVM();
			this.InitializeComponent();
		}

		private void HistoryList_Loaded(System.Object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (this.HistoryList.ItemsSource is INotifyCollectionChanged items)
				items.CollectionChanged += ScrollDown;
		}

		private void ScrollDown(object s, NotifyCollectionChangedEventArgs e)
		{
			var o = HistoryList.Items[HistoryList.Items.Count - 1];
			HistoryList.ScrollIntoView(o);
		}

		private void HistoryList_Unloaded(System.Object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (this.HistoryList.ItemsSource is INotifyCollectionChanged items)
				items.CollectionChanged -= ScrollDown;
		}
	}

}
