using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReader.XMLHelpers.Datas
{
	interface IXmlSchemaData
	{
		/// <summary> 名稱 </summary>
		string Name { get; set; }
		/// <summary> 節點路徑 </summary>
		string Path { get; set; }
		/// <summary> 註解文件 </summary>
		List<string> Document { get; set; }
		/// <summary> 註解文件全部文字 </summary>
		string DocumentText { get; }
	}
}
