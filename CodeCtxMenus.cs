using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using dnlib.DotNet;
using dnSpy.Contracts.Controls;
using dnSpy.Contracts.Documents.Tabs.DocViewer;
using dnSpy.Contracts.Extension;
using dnSpy.Contracts.Menus;
using Microsoft.VisualStudio.Text;
//using dnSpy.Contracts.Themes;
using dnSpy.Themes;
using System.Collections.Generic;
using dnSpy.Contracts.Text;
using dnSpy.Contracts.Documents.TreeView;
//using dnSpy.Themes;

namespace DnSpyMarkdownExtension {
	static class Constants {
		public const string GROUP_TEXTEDITOR = "20000,3567EC95-E68E-44CE-932C-98A686FDCACF";
	}

	[ExportMenuItem(Group = Constants.GROUP_TEXTEDITOR, Order = 30)]
	sealed class CopyMarkdownCommand : MenuItemBase {
		public static Dictionary<string, string> UsedHexColors = new Dictionary<string, string>();
		public static List<Tuple<string, string>> ThemeColors = new List<Tuple<string, string>>();

		public override void Execute(IMenuItemContext context) {
			var documentViewer = GetDocumentViewer(context);
			if (documentViewer is null) return;
			int startPos = 0;
			int endPos = 0;

			if (documentViewer.Selection.ActivePoint.Position.Position > documentViewer.Selection.AnchorPoint.Position.Position) 
			{
				startPos = documentViewer.Selection.AnchorPoint.Position.Position;
				endPos = documentViewer.Selection.ActivePoint.Position.Position;
			}
			else {
				startPos = documentViewer.Selection.ActivePoint.Position.Position;
				endPos = documentViewer.Selection.AnchorPoint.Position.Position;
			}

			if (startPos == endPos) {
				return;
			}

			var themeColors = Theme.hexColors;

			if(ThemeColors.Count == 0) {
				foreach (Tuple<string, string> pair in themeColors) {
					string TextClassification = "";
					if (pair.Item1 == "DefaultText") { //Theme.hexColors uses "DefaultText" to describe normal text, while colorSpan uses "Text".
						TextClassification = "Text";
					}
					else {
						TextClassification = pair.Item1;
					}
					ThemeColors.Add(Tuple.Create(TextClassification, pair.Item2));
				}
			}

			int colorListIndexStart = 0;
			int colorListIndexEnd = 0;
			int counter	= 0;

			foreach (SpanData<object> colorSpan in documentViewer.Content.ColorCollection.colorsList) 
			{
				string spanColor = Enum.GetName(typeof(TextColor), (TextColor)colorSpan.Data);
				if(!UsedHexColors.ContainsKey(spanColor)) 
				{
					foreach (var (themeColor, themeHex) in ThemeColors) {
						if (spanColor == themeColor) {
							UsedHexColors.Add(spanColor, themeHex);
							break;
						}
					}
				}

				if (startPos >= colorSpan.Span.Start && startPos < colorSpan.Span.End) colorListIndexStart = counter;
				if (endPos > colorSpan.Span.Start && endPos <= colorSpan.Span.End) colorListIndexEnd = counter;

				counter++;
			} // This should be tightened eventually to stop full iteration each time, but it works as is.

			string markdownOutput = "";

			//&emsp; is used for markdown blank space

			for (int i = colorListIndexStart; i <= colorListIndexEnd; i++) 
			{
				string part = documentViewer.Content.Text.Substring(documentViewer.Content.ColorCollection.colorsList[i].Span.Start, documentViewer.Content.ColorCollection.colorsList[i].Span.Length);
				string hex = UsedHexColors[Enum.GetName(typeof(TextColor), documentViewer.Content.ColorCollection.colorsList[i].Data)];
				if (part.Contains("\t"))
				{
					part = part.Replace("\t", "&emsp;&emsp;&emsp;");	
				}
				if (part.Contains("*")) {
					part = part.Replace("*", @"\*");
				}

				if(part == " "
				|| part == "." || part == ";" || part == "(" || part == ")" || part == "<" || part == ">" || part == "{" || part == "}" // This stops punctuation from being colored
				|| part.Contains("\t") || part.Contains("&emsp;") || part.Contains("\n") || part.Contains("\r"))
				{
					markdownOutput += part;
				}
				else 
				{
					markdownOutput += "<font style=\"color: " + hex + ";\">" + part + "</font>";
				}				
			}

			try {
				Clipboard.SetText($"{markdownOutput}");
			}
			catch (ExternalException) { }
		}

		public override string? GetHeader(IMenuItemContext context) {
			var documentViewer = GetDocumentViewer(context);
			if (documentViewer is null)
				return "Copy with markdown";
			int startPos = 0;
			int endPos = 0;

			if (documentViewer.Selection.ActivePoint.Position.Position > documentViewer.Selection.AnchorPoint.Position.Position) {
				startPos = documentViewer.Selection.AnchorPoint.Position.Position;
				endPos = documentViewer.Selection.ActivePoint.Position.Position;
			}
			else {
				startPos = documentViewer.Selection.ActivePoint.Position.Position;
				endPos = documentViewer.Selection.AnchorPoint.Position.Position;
			}
			
			return $"Copy with markdown ({startPos} to {endPos})";
		}

		LineColumn GetLineColumn(VirtualSnapshotPoint point) {
			var line = point.Position.GetContainingLine();
			int column = point.Position - line.Start + point.VirtualSpaces;
			return new LineColumn(line.LineNumber, column);
		}

		struct LineColumn {
			public int Line { get; }
			public int Column { get; }
			public LineColumn(int line, int column) {
				Line = line;
				Column = column;
			}
		}

		IDocumentViewer? GetDocumentViewer(IMenuItemContext context) {
			if (context.CreatorObject.Guid != new Guid(MenuConstants.GUIDOBJ_DOCUMENTVIEWERCONTROL_GUID))
				return null;

			return context.Find<IDocumentViewer>();
		}

		public override bool IsVisible(IMenuItemContext context) => context.CreatorObject.Guid == new Guid(MenuConstants.GUIDOBJ_DOCUMENTVIEWERCONTROL_GUID);
		public override bool IsEnabled(IMenuItemContext context) => GetDocumentViewer(context) is not null;
	}
}
