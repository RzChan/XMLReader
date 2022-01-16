using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace XMLReader.XMLHelpers
{
	class XMLHelper
	{
		public static void ParseXSD(string filePath)
		{
			XmlTextReader reader = new XmlTextReader(filePath);
			XmlSchema myschema = XmlSchema.Read(reader, ValidationCallback);
			var schemaSet = new XmlSchemaSet();
			schemaSet.Add(myschema);
			schemaSet.Compile();

			foreach (XmlSchema schema in schemaSet.Schemas())
			{
				foreach (XmlSchemaElement element in schema.Elements.Values)
				{

					Console.WriteLine("Element: {0}", element.Name);

					// Get the complex type of the Customer element.
					XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;

					// If the complex type has any attributes, get an enumerator
					// and write each attribute name to the console.
					if (complexType.AttributeUses.Count > 0)
					{
						IDictionaryEnumerator enumerator =
							complexType.AttributeUses.GetEnumerator();

						while (enumerator.MoveNext())
						{
							XmlSchemaAttribute attribute =
								(XmlSchemaAttribute)enumerator.Value;

							Console.WriteLine("Attribute: {0}", attribute.Name);
						}
					}

					// Get the sequence particle of the complex type.
					XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

					// Iterate over each XmlSchemaElement in the Items collection.
					foreach (var _childElement in sequence.Items)
					{
						if(_childElement.GetType() == typeof(XmlSchemaElement))
						{
							var childElement = _childElement as XmlSchemaElement;
							Console.WriteLine("Element: {0}", childElement.Name);
						}
						else if (_childElement.GetType() == typeof(XmlSchemaAny))
						{
							var childElement = _childElement as XmlSchemaElement;
							Console.WriteLine("Element: {0}", childElement.Name);
						}
						else if (_childElement.GetType() == typeof(XmlSchemaSequence))
						{
							var childElement = _childElement as XmlSchemaElement;
							Console.WriteLine("Element: {0}", childElement.Name);
						}
					}
				}
			}
		}

		private static void ValidationCallback(object sender, ValidationEventArgs args)
		{
			Debug.WriteLine($"[ValidationCallback] ${args.Message}");
		}
	}
}
