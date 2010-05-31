using System;
using System.IO;
using System.Text;
using System.Threading;

using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;

namespace MonoDevelop.TechTalk.SpecFlow
{
	public class SingleFeatureFileGenerator : ISingleFileCustomTool
	{
		public IAsyncOperation Generate(IProgressMonitor monitor, ProjectFile file, SingleFileCustomToolResult result)
		{
			return new ThreadAsyncOperation(() => {
				FilePath outputFile = file.FilePath.ChangeExtension(".feature.cs");
				string content = @"// {0} was generated successfully";
				File.WriteAllText(outputFile, String.Format(content, outputFile.FileName), Encoding.UTF8);
				result.GeneratedFilePath = outputFile;
			}, result);
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
