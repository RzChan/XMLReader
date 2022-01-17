using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using XMLReader.XMLHelpers.Datas;
using XMLReader.XMLHelpers.Events;

namespace XMLReader.XMLHelpers
{
	class XMLHelper
	{
		public static string XSDToFormatString(string filePath)
		{
			XmlTextReader reader = new XmlTextReader(filePath);
			XmlSchema myschema = XmlSchema.Read(reader, ValidationCallback);
			var schemaSet = new XmlSchemaSet();
			schemaSet.Add(myschema);
			schemaSet.Compile();
			var strBuilder = new StringBuilder();
			var schemaParser = new XMLHelper();
			foreach (XmlSchema schema in schemaSet.Schemas())
			{
				strBuilder.AppendLine($"[{schema.SourceUri}]");
				strBuilder.AppendLine(schemaParser.ToFormatString(schema));
			}
			return strBuilder.ToString();
		}


		public static List<XmlComplexData> XSDToTypeObject(string filePath)
		{
			XmlTextReader reader = new XmlTextReader(filePath);
			XmlSchema myschema = XmlSchema.Read(reader, ValidationCallback);
			var schemaSet = new XmlSchemaSet();
			schemaSet.Add(myschema);
			schemaSet.Compile();
			var schemaParser = new XMLHelper();
			var result = new List<XmlComplexData>();
			foreach (XmlSchema schema in schemaSet.Schemas())
			{
				result.Add(new XmlComplexData()
				{
					Name = schema.SourceUri,
					Children = { schemaParser.ToTypeObject(schema) }
				});
			}
			return result;
		}

		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			Debug.WriteLine($"[ValidationCallback] ${args.Message}");
		}

		public event SchemaWriteTextHandler OnSchemaWriteText;
		private StringBuilder stringBuilder;
		private XMLHelper()
		{
			OnSchemaWriteText = LogConsole;
		}

		private XmlComplexData ToTypeObject(XmlSchemaObject schemaObj)
		{
			var xmlObjectData = new XmlComplexData() { Name = "XmlRoot" };
			SchemaIterator(schemaObj, 0, xmlObjectData, null, null);
			return xmlObjectData;
		}

		private string ToFormatString(XmlSchemaObject schemaObj)
		{
			stringBuilder = new StringBuilder();
			OnSchemaWriteText = LogStringBuilder;
			SchemaIterator(schemaObj, 0, null, null, null);
			return stringBuilder.ToString();
		}

		private void SchemaIterator(XmlSchemaObject schemaObj, int depth, IXmlSchemaData schemaData, string parentName, string parentPath)
		{
			if (schemaObj == null)
				return;

			void WriteSchemaText(string text)
			{
				OnSchemaWriteText?.Invoke(this, new SchemaWriteTextEventArgs(text, depth));
			}

			// 根據目前節點名稱，組合成完整節點路徑
			string XmlPathCombineWithParent(string currentNodeName)
			{
				return parentPath == null ? currentNodeName : parentPath + "." + currentNodeName;
			}

			var type = schemaObj.GetType();
			if (type == typeof(XmlSchema))
			{
				var childElement = schemaObj as XmlSchema;
				WriteSchemaText(string.Format("XmlSchema: (Version: {0})", childElement.Version));
				foreach (var item in childElement.Items)
				{
					SchemaIterator(item, depth + 1, schemaData, null, parentPath);
				}
			}
			else if (type == typeof(XmlSchemaElement))
			{
				var element = schemaObj as XmlSchemaElement;
				var elementName = element.Name;
				var info = string.Format("Element: {0}", element.Name);
				var arrayInfo = "";
				var minVal = "1";
				var maxVal = "1";
				if (!string.IsNullOrEmpty(element.MinOccursString))
				{
					minVal = element.MinOccursString;
				}
				if (!string.IsNullOrEmpty(element.MaxOccursString))
				{
					maxVal = element.MaxOccursString;
				}
				if (!string.IsNullOrEmpty(element.MinOccursString) || !string.IsNullOrEmpty(element.MaxOccursString))
					arrayInfo = $"  Occur Times:({minVal}~{maxVal})";

				WriteSchemaText(info + arrayInfo);

				// 根據子 schema type 建立 XmlSchemaData
				var childSchemaType = element.ElementSchemaType.GetType();
				IXmlSchemaData newSchemaData = null;
				if (childSchemaType == typeof(XmlSchemaComplexType))
				{
					newSchemaData = new XmlComplexData
					{
						Name = elementName,
						Path = XmlPathCombineWithParent(elementName),
						MaxOccurs = element.MaxOccurs,
						MinOccurs = element.MinOccurs
					};
				}
				else if (childSchemaType == typeof(XmlSchemaSimpleType))
				{
					newSchemaData = new XmlSimpleData
					{
						Name = elementName,
						Path = XmlPathCombineWithParent(elementName)
					};
				}
				else
				{
					WriteSchemaText($"不支援的子節點類型: ${childSchemaType.Name}");
				}
				((XmlComplexData)schemaData)?.Children.Add(newSchemaData);

				SchemaIterator(element.Annotation, depth + 1, newSchemaData, elementName, newSchemaData.Path);
				SchemaIterator(element.ElementSchemaType, depth + 1, newSchemaData, elementName, newSchemaData.Path);
			}
			else if (type == typeof(XmlSchemaComplexType))
			{
				var complexType = schemaObj as XmlSchemaComplexType;
				WriteSchemaText(string.Format("ComplexType: {0}", complexType.Name));

				if (complexType.AttributeUses.Count > 0)
				{
					IDictionaryEnumerator enumerator =
						complexType.AttributeUses.GetEnumerator();

					while (enumerator.MoveNext())
					{
						XmlSchemaAttribute attribute =
							(XmlSchemaAttribute)enumerator.Value;

						WriteSchemaText(string.Format("Attribute: {0}", attribute.Name));
					}
				}

				var path = XmlPathCombineWithParent(parentName);
				SchemaIterator(complexType.ContentTypeParticle, depth + 1, schemaData, null, path);
			}
			else if (type == typeof(XmlSchemaSimpleType))
			{
				var simpleType = schemaObj as XmlSchemaSimpleType;
				WriteSchemaText(string.Format("SimpleType: {0}", simpleType.Name));

				var path = XmlPathCombineWithParent(parentName);
				if (schemaData.GetType() != typeof(XmlSimpleData))
					throw new Exception("在 XmlSchemaSimpleType 接收到的 schemaData 不是 XmlSimpleData 類型.");

				var _schemaData = schemaData as XmlSimpleData;
				_schemaData.Type = simpleType.TypeCode.ToString();
				_schemaData.NetType = simpleType.Datatype.ValueType.Name;
				_schemaData.TypeTagName = simpleType.Name;
				SchemaIterator(simpleType.Content, depth + 1, _schemaData, simpleType.Name, path);
			}
			else if (type == typeof(XmlSchemaSimpleTypeRestriction))
			{
				var simpleTypeRestr = schemaObj as XmlSchemaSimpleTypeRestriction;
				WriteSchemaText(string.Format("SimpleTypeRestriction: {0}", simpleTypeRestr.BaseTypeName));
				if (schemaData != null)
				{
					((XmlSimpleData)schemaData).TypeNameSpace = simpleTypeRestr.BaseTypeName.Namespace;
				}
				foreach (var schemaObject in simpleTypeRestr.Facets)
				{
					SchemaIterator(schemaObject, depth + 1, schemaData, null, parentPath);
				}
			}
			else if (type == typeof(XmlSchemaEnumerationFacet))
			{
				var enumFacet = schemaObj as XmlSchemaEnumerationFacet;
				((XmlSimpleData)schemaData)?.Enumeration.Add(enumFacet.Value);
				WriteSchemaText(string.Format("EnumerationFacet: {0}", enumFacet.Value));
			}
			else if (type == typeof(XmlSchemaMinLengthFacet))
			{
				var minLen = schemaObj as XmlSchemaMinLengthFacet;
				if (schemaData != null)
				{
					((XmlSimpleData)schemaData).MinLength = Convert.ToInt32(minLen.Value);
				}
				WriteSchemaText(string.Format("MinLengthFacet: {0}", minLen.Value));
			}
			else if (type == typeof(XmlSchemaMaxLengthFacet))
			{
				var maxLen = schemaObj as XmlSchemaMaxLengthFacet;
				if (schemaData != null)
				{
					((XmlSimpleData)schemaData).MaxLength = Convert.ToInt32(maxLen.Value);
				}
				WriteSchemaText(string.Format("MaxLengthFacet: {0}", maxLen.Value));
			}
			else if (type == typeof(XmlSchemaPatternFacet))
			{
				var pattern = schemaObj as XmlSchemaPatternFacet;
				if (schemaData != null)
				{
					((XmlSimpleData)schemaData).Pattern = pattern.Value;
				}
				WriteSchemaText(string.Format("PatternFacet: {0}", pattern.Value));
			}
			else if (type == typeof(XmlSchemaLengthFacet))
			{
				var len = schemaObj as XmlSchemaLengthFacet;
				if (schemaData != null)
				{
					((XmlSimpleData)schemaData).MinLength = Convert.ToInt32(len.Value);
					((XmlSimpleData)schemaData).MaxLength = ((XmlSimpleData)schemaData).MinLength;
				}
				WriteSchemaText(string.Format("LengthFacet: {0}", len.Value));
			}
			else if (type == typeof(XmlSchemaTotalDigitsFacet))
			{
				var totalDigits = schemaObj as XmlSchemaTotalDigitsFacet;
				if (schemaData != null)
				{
					((XmlSimpleData)schemaData).TotalDigits = Convert.ToInt32(totalDigits.Value);
				}
				WriteSchemaText(string.Format("TotalDigitsFacet: {0}", totalDigits.Value));
			}
			else if (type == typeof(XmlSchemaAny))
			{
				var childElement = schemaObj as XmlSchemaAny;
				WriteSchemaText(string.Format("Any: {0}", childElement.Id));
			}
			else if (type == typeof(XmlSchemaSequence))
			{
				var childElement = schemaObj as XmlSchemaSequence;
				WriteSchemaText(string.Format("Sequence: {0}", childElement.Id));
				foreach (var _childElement in childElement.Items)
				{
					SchemaIterator(_childElement, depth + 1, schemaData, null, parentPath);
				}
			}
			else if (type == typeof(XmlSchemaAnnotation))
			{
				var annotation = schemaObj as XmlSchemaAnnotation;
				foreach (var item in annotation.Items)
				{
					SchemaIterator(item, depth + 1, schemaData, parentName, parentPath);
				}
			}
			else if (type == typeof(XmlSchemaDocumentation))
			{
				var doc = schemaObj as XmlSchemaDocumentation;
				foreach (var xmlNode in doc.Markup)
				{
					switch (xmlNode.NodeType)
					{
						case XmlNodeType.Text:
							schemaData.Document.Add(Regex.Replace(xmlNode.Value, "^\t+", "", RegexOptions.Multiline));
							break;
						default:
							WriteSchemaText("不支援的 XmlNodeType 於 XmlSchemaDocumentation.");
							break;
					}
				}
			}
			else
			{
				WriteSchemaText(string.Format("[錯誤] 未處裡的 XMLSchema 類型: {0}", schemaObj.GetType().Name));
			}
		}

		private void LogStringBuilder(object sender, SchemaWriteTextEventArgs args)
		{
			stringBuilder.AppendLine(new string('\t', args.Depth) + args.Text);
		}

		private static void LogConsole(object sender, SchemaWriteTextEventArgs args)
		{
			Console.WriteLine(new string('\t', args.Depth) + args.Text);
		}
	}
}
