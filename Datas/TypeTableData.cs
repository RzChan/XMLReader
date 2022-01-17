using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReader.Datas
{
	/// <summary>
	/// 用於呈現在資料表上的格式
	/// </summary>
	class TypeTableData
	{
		public string Name { get; set; }
		public string FullPath { get; set; }
		public string Type { get; set; }
		public string NetType { get; set; }
		public string TypeRestriction { get; set; }
		public string Enumeration { get; set; }
		public string Description { get; set; }
	}
}
