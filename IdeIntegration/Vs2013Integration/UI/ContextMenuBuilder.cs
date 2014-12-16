using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TechTalk.SpecFlow.Vs2010Integration.UI
{
    internal class ContextMenuBuilder
	{
		private string title = null;
		private readonly List<ContextItem> items = new List<ContextItem>();

		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		public delegate void OnContextItemSelected();

		public void Add(ContextItem contextItem)
		{
			items.Add(contextItem);
		}

		public void AddRange(IEnumerable<ContextItem> contextItems)
		{
			items.AddRange(contextItems);
		}

		internal abstract class ContextItem
		{
			internal abstract ToolStripItem ToMenuItem();
			internal abstract bool Selectable { get; }
		}

		internal class ContextSeparatorItem : ContextItem
		{
			internal override ToolStripItem ToMenuItem()
			{
				return new ToolStripSeparator();
			}

			internal override bool Selectable
			{
				get { return false; }
			}
		}

		internal class ContextCommandItem : ContextItem
		{
			private readonly string title;
			private readonly OnContextItemSelected selectedCallback;

			public string Title
			{
				get { return title; }
			}

			public ContextCommandItem(string title, OnContextItemSelected selectedCallback)
			{
				this.title = title;
				this.selectedCallback = selectedCallback;
			}

			public OnContextItemSelected OnSelected
			{
				get { return selectedCallback; }
			}

			internal override ToolStripItem ToMenuItem()
			{
				ToolStripItem menuItem = new ToolStripMenuItem(
					title,
					null,
					OnMenuItemClicked,
					title);
				menuItem.Tag = this;
				return menuItem;
			}

			internal override bool Selectable
			{
				get { return true; }
			}

			private static void OnMenuItemClicked(object sender, EventArgs e)
			{
				((ContextCommandItem)((ToolStripItem)sender).Tag).OnSelected();
			}
		}

		public ContextMenuStrip ToContextMenu()
		{
			ContextMenuStrip retval = new ContextMenuStrip();
			if (!String.IsNullOrEmpty(title))
			{
				ToolStripItem titleItem = new ToolStripLabel(title);
				titleItem.Font = new Font(titleItem.Font, FontStyle.Bold);
				retval.Items.Add(titleItem);
			}

			ToolStripItem selected = null;
			foreach (ContextItem item in items)
			{
				ToolStripItem menuItem = item.ToMenuItem();
				retval.Items.Add(menuItem);
				if (item.Selectable && selected == null)
					selected = menuItem;
			}
			if (selected != null)
				selected.Select();
			return retval;
		}

	}}
