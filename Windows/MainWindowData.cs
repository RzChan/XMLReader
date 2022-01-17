using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAddon.Util;
using XMLReader.Datas;
using XMLReader.XMLHelpers.Datas;

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

		public string Log
		{
			get { return _log; }
			set
			{
				_log = value;
				OnPropertyChanged(nameof(Log));
			}
		}
		private string _log = "";

		public List<XmlComplexData> XmlObjectTree
		{
			get { return _xmlObjectTree; }
			set
			{
				_xmlObjectTree = value;
				OnPropertyChanged(nameof(XmlObjectTree));
			}
		}
		private List<XmlComplexData> _xmlObjectTree = null;

		public List<TypeTableData> XmlObjectTable
		{
			get { return _xmlObjectTable; }
			set
			{
				_xmlObjectTable = value;
				OnPropertyChanged(nameof(XmlObjectTable));
			}
		}
		private List<TypeTableData> _xmlObjectTable = null;

		public object SelectedXmlTreeObject
		{
			get { return _selectedXmlTreeObject; }
			set
			{
				_selectedXmlTreeObject = value;
				OnPropertyChanged(nameof(SelectedXmlTreeObject));
			}
		}
		private object _selectedXmlTreeObject = null;
	}
}
