using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAddon.Util;

namespace XMLReader.Windows
{
	class MainWindowData : NotifyObject
	{
		public string XMLPath
		{
			get { return _xmlPath; }
			set
			{
				_xmlPath = value;
				OnPropertyChanged(nameof(XMLPath));
			}
		}
		private string _xmlPath = "";
	}
}
