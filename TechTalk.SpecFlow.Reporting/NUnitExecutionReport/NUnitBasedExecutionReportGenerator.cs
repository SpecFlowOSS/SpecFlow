using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport
{
    public abstract class NUnitBasedExecutionReportGenerator
    {
        protected abstract ReportParameters ReportParameters { get; }

        protected virtual Type ReportType
        {
            get { return GetType(); }
        }

        private void TransformReport(ReportElements.NUnitExecutionReport report, SpecFlowProject specFlowProject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ReportElements.NUnitExecutionReport), 
                                                         ReportElements.NUnitExecutionReport.XmlNamespace);

            if (XsltHelper.IsXmlOutput(ReportParameters.OutputFile))
            {
                XsltHelper.TransformXml(serializer, report, ReportParameters.OutputFile);
            }
            else
            {
                XsltHelper.TransformHtml(serializer, report, ReportType, 
                                         ReportParameters.OutputFile, specFlowProject.Configuration.SpecFlowConfiguration, 
                                         ReportParameters.XsltFile);
            }
        }

        public void GenerateAndTransformReport()
        {
            var specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(ReportParameters.ProjectFile);            

            var report = GenerateReport(specFlowProject);
            TransformReport(report, specFlowProject);
        }

        protected virtual ReportElements.NUnitExecutionReport GenerateReport(SpecFlowProject specFlowProject)
        {
            var report = new ReportElements.NUnitExecutionReport();
            report.ProjectName = specFlowProject.ProjectSettings.ProjectName;
            report.GeneratedAt = DateTime.Now.ToString("g", CultureInfo.InvariantCulture);

            XmlDocument xmlTestResult = LoadXmlTestResult();
            report.NUnitXmlTestResult = xmlTestResult.DocumentElement;

            ExtendReport(report);

            return report;
        }

        protected virtual void ExtendReport(ReportElements.NUnitExecutionReport report)
        {
            
        }

        protected abstract XmlDocument LoadXmlTestResult();
    }
}