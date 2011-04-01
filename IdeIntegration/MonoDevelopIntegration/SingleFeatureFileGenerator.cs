using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Threading;

using Microsoft.CSharp;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Parser;

namespace MonoDevelop.TechTalk.SpecFlow
{
	public class SingleFeatureFileGenerator : ISingleFileCustomTool
	{
		public IAsyncOperation Generate(IProgressMonitor monitor, ProjectFile file, SingleFileCustomToolResult result)
		{
			return new ThreadAsyncOperation(() => {
				
				FilePath codeFilePath = file.FilePath.ChangeExtension(".feature.cs");
				
				try
				{
					codeFilePath = GenerateFeatureCodeFileFor(file);
				}
				catch (Exception ex)
				{
					HandleException(ex, file, result);
				}
				
				result.GeneratedFilePath = codeFilePath;
				
			}, result);
		}
		
		private FilePath GenerateFeatureCodeFileFor(ProjectFile featureFile)
		{
			// TODO: We only support C# for now, later we'll add support to grab the provider based on the project
			CodeDomProvider codeProvider = new CSharpCodeProvider();
			FilePath outputFile = featureFile.FilePath.ChangeExtension(".feature." + codeProvider.FileExtension);
			SpecFlowProject specFlowProject = MonoDevelopProjectReader.CreateSpecFlowProjectFrom(featureFile.Project);
			var specFlowGenerator = new SpecFlowGenerator(specFlowProject);
			
			using (var writer = new StringWriter(new StringBuilder()))
			using (var reader = new StringReader(File.ReadAllText(featureFile.FilePath)))
			{
                FeatureFileInput specFlowFeatureFile = specFlowProject.GetOrCreateFeatureFile(featureFile.FilePath);
				specFlowGenerator.GenerateTestFile(specFlowFeatureFile, codeProvider, reader, writer);
				File.WriteAllText(outputFile, writer.ToString());
			}
			
			return outputFile;
		}
		
		private void HandleException(Exception ex, ProjectFile file, SingleFileCustomToolResult result)
		{
			if (ex is SpecFlowParserException)
			{
				SpecFlowParserException sfpex = (SpecFlowParserException) ex;
			                
				if (sfpex.ErrorDetails == null || sfpex.ErrorDetails.Count == 0)
				{
					result.UnhandledException = ex;
				}
				else
				{
					var compilerErrors = new CompilerErrorCollection();
					
					foreach (var errorDetail in sfpex.ErrorDetails)
					{
						var compilerError = new CompilerError(file.Name, errorDetail.ForcedLine, errorDetail.ForcedColumn, "0", errorDetail.Message);
						compilerErrors.Add(compilerError);
					}
							
					result.Errors.AddRange(compilerErrors);
				}
			}
			else
			{
				result.UnhandledException = ex;
			}
		}
	}
	
	internal class ThreadAsyncOperation : IAsyncOperation
	{
		private Thread Thread { get; set; }
		private bool Cancelled { get; set; }
		private SingleFileCustomToolResult Result { get; set; }
		private Action Task { get; set; }
		
		public ThreadAsyncOperation(Action task, SingleFileCustomToolResult result)
		{
			if (result == null)
				throw new ArgumentNullException("result");
			
			Task = task;
			Result = result;
			Thread = new Thread(Run);
			Thread.Start();
		}
		
		private void Run()
		{
			try
			{
				Task();
			}
			catch (ThreadAbortException ex)
			{
				Result.UnhandledException = ex;
				Thread.ResetAbort();
			}
			catch (Exception ex)
			{
				Result.UnhandledException = ex;
			}
			
			if (Completed != null)
				Completed(this);
		}
		
		public event OperationHandler Completed;
		
		public void Cancel()
		{
			Thread.Abort();
		}
		
		public void WaitForCompleted()
		{
			Thread.Join();
		}
		
		public bool IsCompleted
		{
			get { return !Thread.IsAlive; }
		}
		
		public bool Success
		{
			get { return !Cancelled && Result.Success; }
		}
		
		public bool SuccessWithWarnings
		{
			get { return !Cancelled && Result.SuccessWithWarnings; }
		}
	}
}
