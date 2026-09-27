using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Consumer.CodeAnalysis;

/// <summary>
/// Reports its assembly identity so package loading and side-by-side execution can be tested.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FixtureDiagnosticAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new(
        "CBTEST001",
        "CommonBuild fixture analyzer",
        "Analyzer '{0}' executed",
        "Testing",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { WellKnownDiagnosticTags.CompilationEnd });

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(compilationContext => compilationContext.ReportDiagnostic(
            Diagnostic.Create(_rule, Location.None, typeof(FixtureDiagnosticAnalyzer).Assembly.GetName().Name)));
    }
}