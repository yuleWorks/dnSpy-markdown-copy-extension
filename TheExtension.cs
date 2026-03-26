using System.Collections.Generic;
using dnSpy.Contracts.Extension;

namespace DnSpyMarkdownExtension {
	[ExportExtension]
	sealed class TheExtension : IExtension {
		public IEnumerable<string> MergedResourceDictionaries {
			get {
				// No extra dictionaries are needed
				yield break;
			}
		}

		public ExtensionInfo ExtensionInfo => new ExtensionInfo {
			ShortDescription = "Copy Markdown Extension",
		};

		public void OnEvent(ExtensionEvent @event, object? obj) {
			// No events are needed
		}
	}
}
