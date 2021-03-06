using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Collections.Generic;
using NJsonSchema.CodeGeneration.CSharp;
using System.Text;

namespace NJsonSchema.CodeGeneration.Roslyn
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context) {
            try{
                var cancellationToken = context.CancellationToken;
                
                foreach(var file in context.AdditionalFiles){
                    if(context.AnalyzerConfigOptions.GetOptions(file)
                            .TryGetValue("build_metadata.AdditionalFiles.GenerateInNamespace", out string? space))
                    {
                        var schema = JsonSchema.FromJsonAsync(file.GetText(cancellationToken)?.ToString(), cancellationToken).GetAwaiter().GetResult();

                        CSharpGeneratorSettings settings = new(){
                            Namespace = space
                        };
                        CSharpGenerator generator = new(schema, settings);

                        string code = generator.GenerateFile();

                        context.AddSource($"{Path.GetFileName(file.Path)}.cs", SourceText.From(code, Encoding.UTF8));
                    }
                }
            }
            catch(Exception ex){
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}
