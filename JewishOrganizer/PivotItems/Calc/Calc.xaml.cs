﻿using Windows.UI.Xaml.Controls;

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
	}
	
}