/*
   Copyright 2026 Alexander Stärk

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

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
        customTags: [WellKnownDiagnosticTags.CompilationEnd]);

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_rule];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(static compilationContext => compilationContext.ReportDiagnostic(
            Diagnostic.Create(_rule, Location.None, typeof(FixtureDiagnosticAnalyzer).Assembly.GetName().Name)));
    }
}