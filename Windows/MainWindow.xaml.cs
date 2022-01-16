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
using XMLReader.XMLHelpers;

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
				if (Path.GetExtension(dataModel.XMLPath).ToLower() == ".xsd")
				{
					OnXSDFileSelected(dataModel.XMLPath);
				}
			}
		}

		private void OnXSDFileSelected(string filePath)
		{
			XMLHelper.ParseXSD(filePath);
		}
	}
}
