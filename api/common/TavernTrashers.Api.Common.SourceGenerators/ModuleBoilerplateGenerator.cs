using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TavernTrashers.Api.Common.SourceGenerators
{
    [Generator]
    public class ModuleBoilerplateGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for now
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Find all classes with the [GenerateModuleBoilerplate] attribute
            var syntaxTrees = context.Compilation.SyntaxTrees;
            foreach (var tree in syntaxTrees)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                var classDeclarations = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var classDecl in classDeclarations)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(classDecl);
                    if (symbol == null)
                        continue;

                    var hasAttribute = symbol.GetAttributes().Any(attr =>
                        attr.AttributeClass?.ToDisplayString() == "TavernTrashers.Api.Common.SourceGenerators.GenerateModuleBoilerplateAttribute");
                    if (!hasAttribute)
                        continue;

                    var ns = symbol.ContainingNamespace.ToDisplayString();
                    var moduleName = symbol.Name.Replace("Module", "");

                    // Generate boilerplate classes
                    GenerateBoilerplate(context, ns, moduleName);
                }
            }
        }

        private void GenerateBoilerplate(GeneratorExecutionContext context, string ns, string moduleName)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {ns}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {moduleName}IdempotentDomainEventHandler<T> : IdempotentDomainEventHandler<T> {{ }}");
            sb.AppendLine($"    public class {moduleName}IdempotentIntegrationEventHandler<T> : IdempotentIntegrationEventHandler<T> {{ }}");
            sb.AppendLine($"    public class {moduleName}ProcessOutboxJob : ProcessOutboxJob {{ }}");
            sb.AppendLine($"    public class {moduleName}ProcessInboxJob : ProcessInboxJob {{ }}");
            sb.AppendLine("}");
            context.AddSource($"{moduleName}ModuleBoilerplate.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}

