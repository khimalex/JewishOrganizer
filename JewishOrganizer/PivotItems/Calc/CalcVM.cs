using JewishOrganizer.Commands;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JewishOrganizer.PivotItems
{
	public sealed class CalcVM : ViewModelBase
	{
		private const String _NotAllowed = "Неправильно заданы параметры";
		public CalcVM()
		{
			CalcCommand = AsyncCommandFactory.Create(CalcAsync);
			ClearHistoryCommand = AsyncCommandFactory.Create(ClearHistoryAsync);
			History = new ObservableCollection<HistoryItem>();
		}
		private Decimal _TotalValue;
		public Decimal TotalValue
		{
			get { return _TotalValue; }
			set { SetField(ref _TotalValue, value); }
		}

		private Decimal _PricePerTotalValue;
		public Decimal PricePerTotalValue
		{
			get { return _PricePerTotalValue; }
			set { SetField(ref _PricePerTotalValue, value); }
		}

		private Decimal _CalcPriceForValue;
		public Decimal CalcPriceForValue
		{
			get { return _CalcPriceForValue; }
			set { SetField(ref _CalcPriceForValue, value); }
		}

		private String _Result;

		public String Result
		{
			get { return _Result; }
			set { SetField(ref _Result, value); }
		}

		public ObservableCollection<HistoryItem> History { get; private set; }


		public Task CalcAsync()
		{
			if (TotalValue <= 0.0m)
			{
				Result = _NotAllowed;
			}
			else
			{
				Result = ((CalcPriceForValue * PricePerTotalValue) / TotalValue).ToString();
				History.Add(new HistoryItem()
				{
					CalcPriceForValue = CalcPriceForValue,
					PricePerTotalValue = PricePerTotalValue,
					Result = Result,
					TotalValue = TotalValue
				});
			}

			return Task.FromResult(Result);
		}
		public Task ClearHistoryAsync()
		{
			History.Clear();
			return Task.FromResult(-1);

		}

		public ICommand CalcCommand { get; private set; }
		public ICommand ClearHistoryCommand { get; set; }


	}

	public sealed class HistoryItem
	{
		public Decimal TotalValue { get; set; }
		public Decimal PricePerTotalValue { get; set; }
		public Decimal CalcPriceForValue { get; set; }
		public String Result { get; set; }
	}

}
