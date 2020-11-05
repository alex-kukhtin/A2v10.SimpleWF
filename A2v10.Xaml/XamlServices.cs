using System;
using System.IO;
using System.Xml;
using System.Xml.Resolvers;

namespace A2v10.Xaml
{
	public class XamlElement
	{
		public String Name { get; }

		public XamlElement(String name)
		{
			Name = name;
		}

		public XamlElement AddElement(XmlReader rdr)
		{
			var elem = new XamlElement(rdr.LocalName);
			elem.FillAttributes(rdr);
			Console.WriteLine(elem.Name);
			return elem;
		}

		void FillAttributes(XmlReader rdr)
		{
			if (!rdr.HasAttributes)
				return;
			Boolean ok = rdr.MoveToFirstAttribute();
			while (ok)
			{
				var attName = rdr.LocalName;
				var val = rdr.Value;
				Console.WriteLine($"{attName} = '{val}'");
				ok = rdr.MoveToNextAttribute();
			}
		}
	}

	public static class XamlServices
	{
		public static Object Read(String code)
		{
			using var st = new StringReader(code);
			using var rdr = XmlReader.Create(st);

			XamlElement root = new XamlElement(null);
			while (rdr.Read())
			{
				switch (rdr.NodeType)
				{
					case XmlNodeType.Whitespace:
					case XmlNodeType.Comment:
						continue;
					case XmlNodeType.Element:
						var elem = root.AddElement(rdr);
						continue;

				}
				if (rdr.NodeType == XmlNodeType.Whitespace)
					continue;

				Console.WriteLine($"{rdr.NodeType}: {rdr.Name}={rdr.Value}");
			}
			return null;
		}
	}
}
