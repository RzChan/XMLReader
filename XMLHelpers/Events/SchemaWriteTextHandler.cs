using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReader.XMLHelpers.Events
{
	public delegate void SchemaWriteTextHandler(object sender, SchemaWriteTextEventArgs e);
	public class SchemaWriteTextEventArgs : EventArgs
	{
		public string Text { get; set; }
		public int Depth { get; set; }
		public SchemaWriteTextEventArgs(string text, int depth)
		{
			Text = text;
			Depth = depth;
		}

	}
}
