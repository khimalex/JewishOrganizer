using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace JewishOrganizer.Commands
{
	public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
	{
		//NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		public NotifyTaskCompletion(Task<TResult> task)
		{
			Task = task;
			TaskCompletion = WatchTaskAsync(task);
		}

		public NotifyTaskCompletion(Func<Task<TResult>> command)
		{
			Task = command();
			TaskCompletion = WatchTaskAsync(Task);
		}

		private async Task WatchTaskAsync(Task task)
		{
			try
			{
				await task;
			}
			catch (Exception e)
			{
				//logger.Error("Error, while performing Task in " + this.GetType().Name + " Error: " + e.ToString());
			}
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged == null)
				return;
			propertyChanged(this, new PropertyChangedEventArgs(nameof(Status)));
			propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
			propertyChanged(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
			//propertyChanged(this, new PropertyChangedEventArgs(nameof(TaskCompletion)));
			if (task.IsCanceled)
			{
				propertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
			}
			else if (task.IsFaulted)
			{
				propertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
				propertyChanged(this, new PropertyChangedEventArgs(nameof(Exception)));
				propertyChanged(this, new PropertyChangedEventArgs(nameof(InnerException)));
				propertyChanged(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
			}
			else
			{
				propertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
				propertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
			}
		}

		public Task<TResult> Task { get; private set; }
		public Task TaskCompletion { get; private set; }
		public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
		public TaskStatus Status => Task.Status; 
		public Boolean IsCompleted => Task.IsCompleted; 
		public Boolean IsNotCompleted => !Task.IsCompleted; 
		public Boolean IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
		public Boolean IsCanceled => Task.IsCanceled;
		public Boolean IsFaulted => Task.IsFaulted;
		public AggregateException Exception => Task.Exception;
		public Exception InnerException => (Exception == null) ? null : Exception.InnerException;
		public String ErrorMessage => InnerException?.Message;
		public event PropertyChangedEventHandler PropertyChanged = delegate { };
	}

}
