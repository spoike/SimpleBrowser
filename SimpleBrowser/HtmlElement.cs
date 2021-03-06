﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SimpleBrowser.Elements;
using System.Collections.Specialized;

namespace SimpleBrowser
{
	internal class HtmlElement
	{
		private readonly XElement _element;

		public HtmlElement(XElement element)
		{
			_element = element;
		}

		protected XAttribute GetAttribute(string name)
		{
			return GetAttribute(Element, name);
		}

		public XElement XElement
		{
			get { return _element; }
		}

		protected XAttribute GetAttribute(XElement x, string name)
		{
			return x.Attributes().Where(h => h.Name.LocalName.ToLower() == name.ToLower()).FirstOrDefault();
		}

		internal string GetAttributeValue(string name)
		{
			return GetAttributeValue(Element, name);
		}

		protected string GetAttributeValue(XElement x, string name)
		{
			var attr = GetAttribute(x, name);
			return attr == null ? null : attr.Value;
		}

		public string TagName
		{
			get { return Element.Name.LocalName; }
		}
		public virtual string InputType
		{
			get { throw new InvalidOperationException("Not an input element"); }
		}

		public bool Disabled
		{
			get { return GetAttribute("disabled") != null; }
		}

		/// <summary>
		/// Selected returns a boolean value reflecting to state of certain input element. In the Checkbox and Radiobutton
		/// it corresponds to the 'checked' attribute, in an option under a selectbox, it more or less reflects the 
		/// 'selected' attribute
		/// </summary>
		public virtual bool Selected
		{
			get { throw new InvalidOperationException("This element cannot be checked"); }
			set { throw new InvalidOperationException("This element cannot be checked"); }
		}


		public class NavigationArgs
		{
			/// <summary>
			/// This can be a full Url, but also a relative url that can be combined with the current url of the Browser object
			/// </summary>
			public string Uri;
			public string Method = "GET";
			public NameValueCollection UserVariables = new NameValueCollection();
			public string PostData = "";
			public string ContentType = "";
			public string EncodingType = "";
			public int TimeoutMilliseconds;
		}
		public class UserVariableEntry
		{
			public string Name;
			public string Value;
		}


		public event Func<NavigationArgs, bool> NavigationRequested;

		public virtual ClickResult Click()
		{
			return ClickResult.SucceededNoOp;
		}

		protected virtual bool RequestNavigation(NavigationArgs args)
		{
			if (NavigationRequested != null)
				return NavigationRequested(args);
			else
				return false;
		}

		public virtual bool SubmitForm(string url = null, HtmlElement clickedElement = null)
		{
			XElement formElem = this.Element.GetAncestorCI("form");
			if (formElem != null)
			{
				FormElement form = this.OwningBrowser.CreateHtmlElement<FormElement>(formElem);
				form.SubmitForm(url, clickedElement);
			}
			return false;
		}

		public void DoAspNetLinkPostBack()
		{
			if (this is AnchorElement)
			{
				this.Click();
			}
			throw new InvalidOperationException("This method must only be called on <a> elements having a __doPostBack javascript call in the href attribute");
		}

		public virtual string Value
		{
			get
			{
				return Element.Value;
			}
			set
			{
				throw new InvalidOperationException("Can only set the Value attribute for select, textarea and input elements");
			}
		}

		internal XElement Element
		{
			get { return _element; }
		}
		internal Browser OwningBrowser { get; set; }

	}
}


