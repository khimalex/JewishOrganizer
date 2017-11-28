using System;
using Windows.ApplicationModel;

namespace JewishOrganizer.PivotItems
{
	public class AboutVM : ViewModelBase
	{
		public String Version
		{
			get
			{
				var version = Package.Current.Id.Version;
				return $"Версия: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
			}
		}

	}
}
