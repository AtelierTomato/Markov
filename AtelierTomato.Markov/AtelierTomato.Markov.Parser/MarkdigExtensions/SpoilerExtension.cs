using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace AtelierTomato.Markov.Parser.MarkdigExtensions
{
	public class SpoilerExtension : IMarkdownExtension
	{
		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			var parser = pipeline.InlineParsers.FindExact<EmphasisInlineParser>();
			if (parser != null)
			{
				var hasPipe = false;
				foreach (var emphasis in parser.EmphasisDescriptors)
				{
					if (emphasis.Character == '|')
					{
						hasPipe = true;
					}
				}

				if (!hasPipe)
				{
					parser.EmphasisDescriptors.Add(new EmphasisDescriptor('|', 2, 2, true));
				}
			}
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			if (renderer is HtmlRenderer htmlRenderer)
			{
				// Extend the rendering here.
				var emphasisRenderer = htmlRenderer.ObjectRenderers.FindExact<EmphasisInlineRenderer>();
				if (emphasisRenderer != null)
				{
					var previousTag = emphasisRenderer.GetTag;
					emphasisRenderer.GetTag = inline => GetTag(inline) ?? previousTag(inline);
				}
			}
		}

		private string? GetTag(EmphasisInline emphasisInline)
		{
			var c = emphasisInline.DelimiterChar;
			switch (c)
			{
				case '|':
					// HACK: this uses <tt> as a sort of placeholder.
					return "tt";
			}

			return null;
		}
	}
}
