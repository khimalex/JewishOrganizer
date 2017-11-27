using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;


namespace JewishOrganizer.Commands
{
	public interface IAsyncCommand : ICommand
	{
		Task ExecuteAsync(Object parameter);
	}

	public abstract class AsyncCommandBase : IAsyncCommand
	{
		public abstract Boolean CanExecute(Object parameter);

		public abstract Task ExecuteAsync(Object parameter);

		public async void Execute(Object parameter) => await ExecuteAsync(parameter);

		public event EventHandler CanExecuteChanged = delegate { };
		//{
		//	add { CommandManager.RequerySuggested += value; }
		//	remove { CommandManager.RequerySuggested -= value; }
		//}

		protected void RaiseCanExecuteChanged() =>
			//CommandManager.InvalidateRequerySuggested();
			CanExecuteChanged(this, EventArgs.Empty);
	}

	public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
	{
		private readonly Func<CancellationToken, Task<TResult>> _Command;
		private readonly Func<Object, CancellationToken, Task<TResult>> _ParametrizedCommand;
		private readonly CancelAsyncCommand _CancelCommand;
		private NotifyTaskCompletion<TResult> _Execution;
		private Predicate<Object> _CanExecute;


		public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
		{
			_Command = command;
			_CancelCommand = new CancelAsyncCommand();
		}
		public AsyncCommand(Func<CancellationToken, Task<TResult>> command, Predicate<Object> canExecute) : this(command)
		{
			//_command = command;
			//_cancelCommand = new CancelAsyncCommand();
			_CanExecute = canExecute;
		}

		public AsyncCommand(Func<Object, CancellationToken, Task<TResult>> parametrizedCommand)
		{
			_ParametrizedCommand = parametrizedCommand;
			_CancelCommand = new CancelAsyncCommand();
		}

		public AsyncCommand(Func<Object, CancellationToken, Task<TResult>> parametrizedCommand, Predicate<Object> canExecute) : this(parametrizedCommand)
		{
			_CanExecute = canExecute;
		}

		public override Boolean CanExecute(Object parameter)
		{
			Boolean? canExecute = _CanExecute?.Invoke(parameter);
			var predicateResult = canExecute ?? true;
			return (Execution == null || Execution.IsCompleted) && predicateResult;
		}

		public override async Task ExecuteAsync(Object parameter)
		{
			_CancelCommand.NotifyCommandStarting();
			if (_Command != null)
			{
				Execution = new NotifyTaskCompletion<TResult>(_Command(_CancelCommand.Token));
			}
			else
			{
				Execution = new NotifyTaskCompletion<TResult>(_ParametrizedCommand(parameter, _CancelCommand.Token));
			}
			RaiseCanExecuteChanged();
			await Execution.TaskCompletion;
			OnPropertyChanged(nameof(Execution)); //Говорим ,Что св-о изменилось, Чтобы подписанные на него объекты смогли обработаться правильно.
			_CancelCommand.NotifyCommandFinished();
			RaiseCanExecuteChanged();
		}

		public ICommand CancelCommand
		{
			get { return _CancelCommand; }
		}

		public NotifyTaskCompletion<TResult> Execution
		{
			get { return _Execution; }
			private set
			{
				_Execution = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private sealed class CancelAsyncCommand : ICommand
		{
			private CancellationTokenSource _Cts = new CancellationTokenSource();
			private Boolean _CommandExecuting;

			public CancellationToken Token { get { return _Cts.Token; } }

			public void NotifyCommandStarting()
			{
				_CommandExecuting = true;
				if (!_Cts.IsCancellationRequested)
					return;
				_Cts = new CancellationTokenSource();
				RaiseCanExecuteChanged();
			}

			public void NotifyCommandFinished()
			{
				_CommandExecuting = false;
				RaiseCanExecuteChanged();
			}

			Boolean ICommand.CanExecute(Object parameter)
			{
				return _CommandExecuting && !_Cts.IsCancellationRequested;
			}

			void ICommand.Execute(Object parameter)
			{
				_Cts.Cancel();
				RaiseCanExecuteChanged();
			}

			public event EventHandler CanExecuteChanged = delegate
			{
			};
			//{
			//	add { CommandManager.RequerySuggested += value; }
			//	remove { CommandManager.RequerySuggested -= value; }
			//}

			private void RaiseCanExecuteChanged()
			{
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
				//CommandManager.InvalidateRequerySuggested();
			}
		}
	}

	public static class AsyncCommandFactory
	{
		public static AsyncCommand<Object> Create(Func<Task> command) => new AsyncCommand<Object>(async _ => { await command(); return null; });
		public static AsyncCommand<Object> Create(Func<Object, Task> parametrizedCommand) => new AsyncCommand<Object>(async (param, _) => { await parametrizedCommand(param); return null; });

		public static AsyncCommand<Object> Create(Func<Task> command, Predicate<Object> canExecute) => new AsyncCommand<Object>(async _ => { await command(); return null; }, canExecute);
		public static AsyncCommand<Object> Create(Func<Object, Task> parametrizedCommand, Predicate<Object> canExecute) => new AsyncCommand<Object>(async (param, _) => { await parametrizedCommand(param); return null; }, canExecute);

		public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command) => new AsyncCommand<TResult>(_ => command());
		public static AsyncCommand<TResult> Create<TResult>(Func<Object, Task<TResult>> parametrizedCommand) => new AsyncCommand<TResult>((param, _) => parametrizedCommand(param));

		public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command, Predicate<Object> canExecute) => new AsyncCommand<TResult>(_ => command(), canExecute);
		public static AsyncCommand<TResult> Create<TResult>(Func<Object, Task<TResult>> parametrizedCommand, Predicate<Object> canExecute) => new AsyncCommand<TResult>((param, _) => parametrizedCommand(param), canExecute);

		public static AsyncCommand<Object> Create(Func<CancellationToken, Task> command) => new AsyncCommand<Object>(async token => { await command(token); return null; });
		public static AsyncCommand<Object> Create(Func<Object, CancellationToken, Task> parametrizedCommand) => new AsyncCommand<Object>(async (param, token) => { await parametrizedCommand(param, token); return null; });

		public static AsyncCommand<Object> Create(Func<CancellationToken, Task> command, Predicate<Object> canExecute) => new AsyncCommand<Object>(async token => { await command(token); return null; }, canExecute);
		public static AsyncCommand<Object> Create(Func<Object, CancellationToken, Task> parametrizedCommand, Predicate<Object> canExecute) => new AsyncCommand<Object>(async (param, token) => { await parametrizedCommand(param, token); return null; }, canExecute);

		public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command) => new AsyncCommand<TResult>(command);
		public static AsyncCommand<TResult> Create<TResult>(Func<Object, CancellationToken, Task<TResult>> parametrizedCommand) => new AsyncCommand<TResult>(parametrizedCommand);

		public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command, Predicate<Object> canExecute) => new AsyncCommand<TResult>(command, canExecute);
		public static AsyncCommand<TResult> Create<TResult>(Func<Object, CancellationToken, Task<TResult>> parametrizedCommand, Predicate<Object> canExecute) => new AsyncCommand<TResult>(parametrizedCommand, canExecute);

	}
}
