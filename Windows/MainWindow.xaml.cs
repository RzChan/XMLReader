using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using XMLReader.Datas;
using XMLReader.XMLHelpers;
using XMLReader.XMLHelpers.Datas;

namespace XMLReader.Windows
{
	/// <summary>
	/// MainWindow.xaml 的互動邏輯
	/// </summary>
	public partial class MainWindow : Window
	{
		MainWindowData dataModel = null;

		public MainWindow()
		{
			InitializeComponent();
			dataModel = new MainWindowData();
			this.DataContext = dataModel;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = ".xsd",
				Filter = "Xml Schema Files (*.xsd)|*.xsd|XML Files (*.xml)|*.xml"
			};
			var result = dialog.ShowDialog();
			if (result == true)
			{
				dataModel.XMLPath = dialog.FileName;
				switch (Path.GetExtension(dataModel.XMLPath).ToLower())
				{
					case ".xsd":
						OnXSDFileSelected(dataModel.XMLPath);
						break;
					case ".xml":
						MessageBox.Show("尚未支援 xml 格式", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
						break;
					default:
						MessageBox.Show("不支援的檔案格式", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
				}
			}
		}

		private void OnXSDFileSelected(string filePath)
		{
			try
			{
				var humanReadText = new StringBuilder();
				var typeTable = new List<TypeTableData>();
				void XmlObjectTextIterator(object obj, int depth)
				{
					var type = obj.GetType();
					if (type == typeof(XmlComplexData))
					{
						var xmlObjectData = obj as XmlComplexData;
						humanReadText.Append(new string('\t', depth) + $"{xmlObjectData.Name}");
						if (xmlObjectData.IsOccursHasValue)
						{
							humanReadText.Append($"出現次數 [{xmlObjectData.OccursRangeText}]");
						}
						humanReadText.AppendLine();
						foreach (var child in xmlObjectData.Children)
						{
							XmlObjectTextIterator(child, depth + 1);
						}
					}
					else if (type == typeof(XmlSimpleData))
					{
						var xmlBaseData = obj as XmlSimpleData;
						var subInfos = new List<string>();
						humanReadText.AppendLine(new string('\t', depth) + $"{xmlBaseData.Name} [{xmlBaseData.Type}]");
						if (xmlBaseData.MinLength.HasValue || xmlBaseData.MaxLength.HasValue)
						{
							subInfos.Add($"字串長度:{(object)xmlBaseData.MinLength ?? "0"}~{(object)xmlBaseData.MaxLength ?? ""}  ");
						}
						if (xmlBaseData.TotalDigits.HasValue)
						{
							subInfos.Add($"數值字數:{(object)xmlBaseData.TotalDigits ?? ""}  ");
						}
						if (xmlBaseData.Pattern != null)
						{
							subInfos.Add($"規則運算式:{(object)xmlBaseData.Pattern ?? ""}  ");
						}
						if (subInfos.Count > 0)
							humanReadText.AppendLine(new string('\t', depth + 1) + string.Join("  ", subInfos));
					}
				}

				// 將 XmlObject 轉換成 type table
				void XmlObjectTableIterator(IXmlSchemaData obj, int depth)
				{
					var type = obj.GetType();
					if (type == typeof(XmlComplexData))
					{
						var xmlObjectData = obj as XmlComplexData;
						var typeData = new TypeTableData()
						{
							Name = xmlObjectData.Name,
							FullPath = xmlObjectData.Path,
							TypeRestriction = xmlObjectData.IsOccursHasValue ? $"出現次數 [{xmlObjectData.OccursRangeText}]" : "",
							Description = xmlObjectData.DocumentText
						};
						typeTable.Add(typeData);

						foreach (var child in xmlObjectData.Children)
						{
							XmlObjectTableIterator(child, depth + 1);
						}
					}
					else if (type == typeof(XmlSimpleData))
					{
						var xmlBaseData = obj as XmlSimpleData;
						var typeData = new TypeTableData()
						{
							Name = xmlBaseData.Name,
							FullPath = xmlBaseData.Path,
							Type = xmlBaseData.Type,
							NetType = xmlBaseData.NetType,
							TypeRestriction = "",
							Description = xmlBaseData.DocumentText
						};
						typeTable.Add(typeData);
						var typeRestriction = new List<string>();
						if (xmlBaseData.HasTextLength)
						{
							typeRestriction.Add($"字數限制:{xmlBaseData.TextLengthRange}");
						}
						if (xmlBaseData.TotalDigits.HasValue)
						{
							typeRestriction.Add($"數字字數:{xmlBaseData.TotalDigits.Value}");
						}
						if (xmlBaseData.Pattern != null)
						{
							typeRestriction.Add($"規則運算式:{xmlBaseData.Pattern}");
						}
						typeData.TypeRestriction = string.Join("  ", typeRestriction);
						if (xmlBaseData.Enumeration.Count > 0)
							typeData.Enumeration = string.Join(", ", xmlBaseData.Enumeration);
					}
				}
				//dataModel.Log = XMLHelper.XSDToFormatString(filePath);
				var data = XMLHelper.XSDToTypeObject(filePath);
				foreach (var schema in data)
				{
					XmlObjectTextIterator(schema, 0);
				}
				foreach (var schema in data)
				{
					var firstChild = schema.Children.FirstOrDefault();
					if (firstChild != null)
						XmlObjectTableIterator(firstChild, 0);
				}
				dataModel.Log = humanReadText.ToString();
				dataModel.XmlObjectTree = data;
				dataModel.XmlObjectTable = typeTable;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			dataModel.SelectedXmlTreeObject = e.NewValue;
		}
	}
}
