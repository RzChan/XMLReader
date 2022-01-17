using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReader.XMLHelpers.Datas
{
	/// <summary>
	/// 表示基礎型別，類似於物件導向的 string, int, long, float。
	/// </summary>
	class XmlSimpleData : IXmlSchemaData
	{
		/// <summary> 名稱 </summary>
		public string Name { get; set; }
		/// <summary> 節點路徑 </summary>
		public string Path { get; set; }
		/// <summary> 型別名稱 </summary>
		public string TypeTagName { get; set; }
		/// <summary> 基礎型別 </summary>
		public string Type { get; set; }
		/// <summary> .Net 型別 </summary>
		public string NetType { get; set; }
		/// <summary> 型別名稱 </summary>
		public string TypeNameSpace { get; set; }
		/// <summary> 字串最小長度 </summary>
		public int? MinLength { get; set; }
		/// <summary> 字串最大長度 </summary>
		public int? MaxLength { get; set; }
		// string? object?
		public List<string> Enumeration { get; set; } = new List<string>();
		/// <summary> Regex Pattern </summary>
		public string Pattern { get; set; }
		/// <summary> 限制數值的字數。例如 4 可以表示成 12.34 或是 123.4。 </summary>
		public int? TotalDigits { get; set; }

		public bool HasTextLength { get { return MinLength.HasValue || MaxLength.HasValue; } }
		public string TextLengthRange
		{
			get
			{
				if (MinLength == MaxLength)
					return MinLength?.ToString() ?? "";
				else
					return $"{(object)MinLength ?? "1"}~{(object)MaxLength ?? ""}";
			}
		}

		/// <summary> 註解文件 </summary>
		public List<string> Document { get; set; } = new List<string>();

		public string DocumentText
		{
			get
			{
				return string.Join(" ", Document);
			}
		}
	}
}
