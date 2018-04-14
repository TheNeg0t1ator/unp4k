﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using unp4k.gui.Plugins;
using unp4k.gui.Extensions;
using System.Windows.Media;
using System.Windows.Controls;
using System.Diagnostics;

namespace unp4k.gui.TreeModel
{
	public class DataForgeTreeItem : StreamTreeItem, IStreamTreeItem, IBranchItem, ITreeItem
	{
		public override String RelativePath => this.Parent.RelativePath;
		public virtual Boolean Expanded { get; set; }

		public DataForgeTreeItem(String title, ITreeItem parent, unforge.DataForge dataForge)
			: base(title, parent, () => dataForge.GetStream())

		{
			var maxIndex = dataForge.Length - 1;
			var lastIndex = 0L;

			var oldProgress = ArchiveExplorer.RegisterProgress(async (ProgressBar barProgress) =>
			{
				barProgress.Maximum = maxIndex;
				barProgress.Value = lastIndex;

				await ArchiveExplorer.UpdateStatus($"Deserializing file {lastIndex:#,##0}/{maxIndex:#,##0} from dataforge");

				await Task.CompletedTask;
			});

			foreach ((String FileName, XmlDocument XmlDocument) entry in dataForge)
			{
				this.Children.AddStream(
					() => entry.XmlDocument.GetStream(),
					entry.FileName,
					this);

				lastIndex++;
			}

			ArchiveExplorer.RegisterProgress(oldProgress);
		}
	}

	public class CryXmlTreeItem : StreamTreeItem, IStreamTreeItem, ITreeItem
	{
		public CryXmlTreeItem(IStreamTreeItem node, XmlDocument xml)
			: base(node.Title, node.Parent, () => xml.GetStream())
		{ }
	}
}
