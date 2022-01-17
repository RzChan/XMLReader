using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReader.XMLHelpers.Datas
{
	/// <summary>
	/// 表示一個 ComplexType，類似於一般物件導向的 class 物件
	/// </summary>
	class XmlComplexData : IXmlSchemaData
	{
		/// <summary> Tag 名稱 </summary>
		public string Name { get; set; }
		/// <summary> 節點路徑 </summary>
		public string Path { get; set; }
		/// <summary> Tag 子元素 </summary>
		public List<IXmlSchemaData> Children { get; set; } = new List<IXmlSchemaData>();
		/// <summary> 最少元素出現次數 </summary>
		public decimal MinOccurs { get; set; } = 1;
		/// <summary> 最多元素出現次數 </summary>
		public decimal MaxOccurs { get; set; } = 1;
		/// <summary> 最多元素出現次數，是否為 unbounded (無限) </summary>
		public bool IsMaxOccursUnbounded
		{
			get { return MaxOccurs == decimal.MaxValue; }
			set
			{
				if (value)
				{
					MaxOccurs = decimal.MaxValue;
				}
				else
				{
					MaxOccurs = 1;
				}
			}
		}
		/// <summary> 最多或最少元素出現次數有被指定 </summary>
		public bool IsOccursHasValue
		{
			get
			{
				return MinOccurs != 1 || MaxOccurs != 1;
			}
		}
		/// <summary> 取得由最多和最少元素出現次數組成的範圍。例如 1~10 或是 0~unbounded。</summary>
		public string OccursRangeText
		{
			get
			{
				if (IsMaxOccursUnbounded)
					return $"{MinOccurs}~unbounded";
				else
					return $"{MinOccurs}~{MaxOccurs}";
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
