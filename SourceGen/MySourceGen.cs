using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

[Generator]
public class XmlCommentIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var members = context.CompilationProvider
            .Select((compilation, _) => {
                var names = compilation.GlobalNamespace.GetNamespaceMembers()
                    .Where(x => x.Name.Contains("SourceGenTest") || x.Name.Contains("SomeLib"))
                    .Select(x => $"{x.Name}  DeclaringSyntaxReferences:{x.DeclaringSyntaxReferences.FirstOrDefault()}");

                return names.ToImmutableArray();
            });

        context.RegisterSourceOutput(members, (ctx, array) => {
            string output = "//" + string.Join("\n//", array);
            ctx.AddSource("Output.g.cs", SourceText.From(output, Encoding.UTF8));
        }
        );
    }
}
