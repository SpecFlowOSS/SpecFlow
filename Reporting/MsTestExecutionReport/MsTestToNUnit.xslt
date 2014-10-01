<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:sfr="urn:TechTalk:SpecFlow.Report"
                xmlns:nunit="urn:NUnit"
                xmlns:mstest="http://microsoft.com/schemas/VisualStudio/TeamTest/2006"
                exclude-result-prefixes="msxsl nunit sfr">
  <xsl:output method="xml" />

  <xsl:key name="unit-test-result" match="mstest:UnitTestResult" use="@testId"/>
  <xsl:key name="unit-test-detail" match="mstest:UnitTest" use="@id"/>

  <xsl:template name="get-last-part">
    <xsl:param name="text" />
    <xsl:param name="delimiter" />

    <xsl:choose>
      <xsl:when test="contains($text, $delimiter)">
        <xsl:call-template name="get-last-part">
          <xsl:with-param name="text" select="substring-after($text,$delimiter)" />
          <xsl:with-param name="delimiter" select="$delimiter" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="/mstest:TestRun">
    <nunit:test-results>
      <xsl:attribute name="name">
        <xsl:value-of select="@name"/>
      </xsl:attribute>
      <xsl:variable name="startTime" select="mstest:Times/@start" />
      <!-- start="2010-06-20T21:54:47.1619417+02:00" -->
      <xsl:attribute name="date">
        <xsl:value-of select="substring($startTime, 1, 10)"/>
      </xsl:attribute>
      <xsl:attribute name="time">
        <xsl:value-of select="substring($startTime, 12, 8)"/>
      </xsl:attribute>
      <xsl:apply-templates select="mstest:ResultSummary" />

      <xsl:for-each select="mstest:TestDefinitions/mstest:UnitTest">
        <xsl:variable name="className" select="mstest:TestMethod/@className" />
        <xsl:if test="not(preceding-sibling::mstest:UnitTest[mstest:TestMethod/@className=$className])">
          <xsl:variable name="unitTests" select="//mstest:UnitTest[mstest:TestMethod/@className=$className]"/>

          <nunit:test-suite type="TestFixture" result="Failure" success="False">
            <xsl:attribute name="name">
              <xsl:call-template name="get-last-part">
                <xsl:with-param name="text" select="substring-before(mstest:TestMethod/@className, ',')" />
                <xsl:with-param name="delimiter" select="'.'" />
              </xsl:call-template>
              <!--<xsl:value-of select="substring-before(mstest:TestMethod/@className, ',')"/>-->
            </xsl:attribute>
            <!-- SpecFlow specific conversion -->
            <xsl:if test="mstest:Properties/mstest:Property[string(mstest:Key)='FeatureTitle']">
              <xsl:attribute name="description">
                <xsl:value-of select="mstest:Properties/mstest:Property[string(mstest:Key)='FeatureTitle']/mstest:Value"/>
              </xsl:attribute>
            </xsl:if>
            <xsl:attribute name="executed">True</xsl:attribute>
            <xsl:attribute name="success">
              <xsl:choose>
                <xsl:when test="$unitTests[key('unit-test-result', @id)/@outcome!='Passed']">
                  <xsl:text>False</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>True</xsl:text>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <xsl:attribute name="result">
              <xsl:choose>
                <xsl:when test="$unitTests[key('unit-test-result', @id)/@outcome='Failed']">
                  <xsl:text>Failure</xsl:text>
                </xsl:when>
                <xsl:when test="$unitTests[key('unit-test-result', @id)/@outcome='Inconclusive']">
                  <xsl:text>Inconclusive</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>Success</xsl:text>
                </xsl:otherwise>
              </xsl:choose>

            </xsl:attribute>

            <xsl:attribute name="time">0.000</xsl:attribute>
            <xsl:attribute name="asserts">0</xsl:attribute>

            <nunit:results>
              <xsl:apply-templates select="$unitTests" />
            </nunit:results>
          </nunit:test-suite>
        </xsl:if>
      </xsl:for-each>
    </nunit:test-results>
  </xsl:template>

  <xsl:template match="mstest:ResultSummary">
    <!--
    <Counters total="4" executed="4" passed="2" error="0" failed="1" timeout="0" 
              aborted="0" inconclusive="1" passedButRunAborted="0" notRunnable="0" 
              notExecuted="0" disconnected="0" warning="0" completed="0" inProgress="0" 
              pending="0" />
              
    total="4" errors="0" failures="1" not-run="1" inconclusive="1" ignored="1" skipped="0" invalid="0"              
    -->
    <xsl:attribute name="total">
      <xsl:value-of select="mstest:Counters/@total"/>
    </xsl:attribute>
    <xsl:attribute name="errors">
      <xsl:value-of select="mstest:Counters/@error"/>
    </xsl:attribute>
    <xsl:attribute name="failures">
      <xsl:value-of select="mstest:Counters/@failed"/>
    </xsl:attribute>
    <xsl:attribute name="inconclusive">
      <xsl:value-of select="mstest:Counters/@inconclusive"/>
    </xsl:attribute>
    <xsl:attribute name="ignored">0</xsl:attribute>
    <!--<xsl:attribute name="not-run">
    </xsl:attribute>-->
    <!--<xsl:attribute name="skipped">
      <xsl:value-of select="mstest:Counters/@notExecuted"/>
    </xsl:attribute>-->
    <!--<xsl:attribute name="invalid">
      <xsl:value-of select="mstest:Counters/@notRunnable"/>
    </xsl:attribute>-->
  </xsl:template>

  <xsl:template match="mstest:UnitTest">
    <!--
    input (MsTest):
    <UnitTest name="ScenarioWithPendingSteps" 
              storage="c:\users\jba\dev\code\specflowexamples\mstestconversion\reportingtest.mstestsampleproject\bin\debug\reportingtest.mstestsampleproject.dll" 
              id="e0c798ae-34fe-8aec-156d-98ed561da84b">
      <Css projectStructure="" iteration="" />
      <Execution timeOut="1800000" id="6c9f7dc0-04e5-4820-b965-35e67ab5b8dc" />
      <Description>Scenario with pending steps</Description>
      <Owners>
        <Owner name="" />
      </Owners>
      <Properties>
        <Property>
          <Key>FeatureTitle</Key>
          <Value>Feature with failing scenarios</Value>
        </Property>
      </Properties>
      <TestMethod codeBase="C:/Users/jba/Dev/Code/SpecFlowExamples/MsTestConversion/ReportingTest.MSTestSampleProject/bin/Debug/ReportingTest.MSTestSampleProject.DLL" adapterTypeName="Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter" className="ReportingTest.MSTestSampleProject.FeatureWithFailingScenariosFeature, ReportingTest.MSTestSampleProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" name="ScenarioWithPendingSteps" />
    </UnitTest>
    
    <UnitTestResult executionId="6c9f7dc0-04e5-4820-b965-35e67ab5b8dc" 
                    testId="e0c798ae-34fe-8aec-156d-98ed561da84b" 
                    testName="ScenarioWithPendingSteps" 
                    computerName="TTV-JBA01" 
                    duration="00:00:03.4117534" 
                    startTime="2010-06-29T21:54:47.5419634+02:00" 
                    endTime="2010-06-29T21:54:50.9541586+02:00" 
                    testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" 
                    outcome="Inconclusive" 
                    testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d">
      <Output>
        <StdOut>{stdout}</StdOut>
        <ErrorInfo>
          <Message>{errorMessgage}</Message>
          <StackTrace>{stackTrace}</StackTrace>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
    
    
    output (NUnit):
                  <test-case name="ReportingTest.SampleProject.FeatureWithFailingScenariosFeature.ScenarioWithPendingSteps" 
                             description="Scenario with pending steps" 
                             executed="True" 
                             result="Inconclusive" 
                             success="False" 
                             time="0.106" 
                             asserts="0">
                    <reason>
                      <message>{errorMessgage}</message>
                    </reason>
                  </test-case>
    -->

    <nunit:test-case>
      <xsl:variable name="id" select="@id" />
      <!--<xsl:variable name="testResult" select="/mstest:TestRun/mstest:Results/mstest:UnitTestResult[@testId=$id]" />-->
      <xsl:variable name="testResult" select="key('unit-test-result', $id)" />
      <xsl:variable name="testDetail" select="key('unit-test-detail', $id)" />

      <xsl:attribute name="name">
        <xsl:value-of select="substring-before(mstest:TestMethod/@className, ',')"/>.<xsl:value-of select="@name"></xsl:value-of>
      </xsl:attribute>

      <xsl:attribute name="description">
        <xsl:value-of select="mstest:Description" />
        <xsl:if test="mstest:Properties/mstest:Property[string(mstest:Key)='VariantName']">
          <xsl:text>(</xsl:text>
          <xsl:for-each select="mstest:Properties/mstest:Property[string(mstest:Key)='VariantName']">
            <xsl:value-of select="mstest:Value"/>
          </xsl:for-each>
          <xsl:text>)</xsl:text>
        </xsl:if>
      </xsl:attribute>

      <xsl:attribute name="executed">True</xsl:attribute>

      <xsl:attribute name="result">
        <xsl:choose>
          <xsl:when test="$testResult/@outcome='Failed'">
            <xsl:text>Failure</xsl:text>
          </xsl:when>
          <xsl:when test="$testResult/@outcome='Inconclusive'">
            <xsl:text>Inconclusive</xsl:text>
          </xsl:when>
          <xsl:when test="$testResult/@outcome='Passed'">
            <xsl:text>Success</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>???</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>

      <xsl:attribute name="success">
        <xsl:choose>
          <xsl:when test="$testResult/@outcome='Passed'">
            <xsl:text>True</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>False</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>

      <xsl:attribute name="time">
        <xsl:value-of select="(substring($testResult/@duration, 1, 2) * 60 * 60) + 
                              (substring($testResult/@duration, 4, 2) * 60) + 
                              (substring($testResult/@duration, 7, 2)) + 
                              (substring($testResult/@duration, 9, 4))"/>
      </xsl:attribute>

      <xsl:attribute name="asserts">1</xsl:attribute>

      <xsl:if test="$testResult/@outcome='Inconclusive'">
        <nunit:reason>
          <nunit:message>
            <xsl:value-of select="$testResult/mstest:Output/mstest:ErrorInfo/mstest:Message"/>
          </nunit:message>
        </nunit:reason>
      </xsl:if>

      <xsl:if test="$testDetail/mstest:TestCategory">
        <nunit:categories>
          <xsl:for-each select="$testDetail/mstest:TestCategory/mstest:TestCategoryItem">
            <nunit:category>
              <xsl:attribute name="name">
                <xsl:value-of select="@TestCategory"/>
              </xsl:attribute>
            </nunit:category>

          </xsl:for-each>

        </nunit:categories>
      </xsl:if>

      <xsl:if test="$testResult/@outcome='Failed'">
        <nunit:failure>
          <nunit:message>
            <xsl:value-of select="$testResult/mstest:Output/mstest:ErrorInfo/mstest:Message"/>
          </nunit:message>
          <nunit:stack-trace>
            <xsl:value-of select="$testResult/mstest:Output/mstest:ErrorInfo/mstest:StackTrace"/>
          </nunit:stack-trace>
        </nunit:failure>
      </xsl:if>

    </nunit:test-case>
  </xsl:template>

</xsl:stylesheet>
