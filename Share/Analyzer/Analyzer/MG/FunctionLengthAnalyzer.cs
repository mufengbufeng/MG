using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ET.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FunctionLengthAnalyzer : DiagnosticAnalyzer
    {
        // 定义诊断ID和标题
        private const string DiagnosticId = "MG0001";
        private static readonly LocalizableString Title = "函数过长警告";
        private static readonly LocalizableString MessageFormat = "函数 '{0}' 长度超过50行，建议拆分";
        private static readonly LocalizableString Description = "函数长度不应超过50行，过长的函数应该被拆分为更小的函数.";
        private const string Category = "代码规范";
        private const int MaxLineCount = 50;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat, 
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            if (!AnalyzerGlobalSetting.EnableAnalyzer)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            
            // 获取方法体
            var body = methodDeclaration.Body;
            if (body == null)
                return;

            // 计算方法体的行数
            var lineSpan = body.GetLocation().GetLineSpan();
            int lineCount = lineSpan.EndLinePosition.Line - lineSpan.StartLinePosition.Line + 1;

            // 如果超过50行，报告诊断
            if (lineCount > MaxLineCount)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    methodDeclaration.GetLocation(),
                    methodDeclaration.Identifier.Text);
                    
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}