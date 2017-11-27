using JewishOrganizer.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JewishOrganizer.PivotItems
{
	public sealed class CalcVM : ViewModelBase
	{
		private const String NotAllowed = "Неправильно заданы параметры";
		public CalcVM()
		{
			Calc = AsyncCommandFactory.Create(CalcCommand);
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

		public Task CalcCommand()
		{
			if (TotalValue <= 0.0m)
			{
				Result = NotAllowed;
			}
			else
				Result = ((CalcPriceForValue * PricePerTotalValue) / TotalValue).ToString();
			return Task.FromResult(Result);
		}
		public ICommand Calc { get; private set; }

	}

}
