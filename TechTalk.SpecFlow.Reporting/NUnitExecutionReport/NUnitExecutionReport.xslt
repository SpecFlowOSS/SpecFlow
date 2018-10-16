<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sfr="urn:TechTalk:SpecFlow.Report"
                xmlns:nunit="urn:NUnit"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"/>

  <xsl:include href="../Common/Common.xslt"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:key name="step-usage-variation" match="sfr:Instance" use="normalize-space(string(sfr:ScenarioStep))"/>

  <xsl:template match="/">
    <xsl:variable name="title">
      <xsl:value-of select="sfr:NUnitExecutionReport/@projectName"/> Test Execution Report
    </xsl:variable>
    <html>
      <head>
        <xsl:call-template name="html-common-header">
          <xsl:with-param name="title" select="$title" />
        </xsl:call-template>
        <style>
          <![CDATA[
          .bar
          {
            height: 1.5em;
          }
          .success
          {
            background-color: #22dd22;
          }
          .warning
          {
            background-color: #ffff00;
          }
          .failure
          {
            background-color: #dd2222;
          }
          .ignored
          {
            background-color: #2266ff;
          }
          .pending
          {
            background-color: #dd66ff;
          }
          .bar div
          {
            height: 1.5em;
            float: left;
          }
          div.reasonPanel
          {
            background-color: #DDDDDD;
          }
          span.traceMessage
          {
            font-style:italic;
            margin-left: 2em;
            color: #888888;
          }
          ]]>
        </style>
      </head>
      <body>
        <xsl:call-template name="html-body-header">
          <xsl:with-param name="title" select="$title" />
        </xsl:call-template>
        <h2>Summary</h2>
        <xsl:variable name="summary">
          <xsl:call-template name="get-summary" />
        </xsl:variable>

        <table class="reportTable" cellpadding="0" cellspacing="0">
          <tr>
            <th class="top left">Features</th>
            <th class="top" colspan="2">Success rate (incl. warnings)</th>
            <th class="top">Scenarios</th>
            <th class="top">Success</th>
            <th class="top">Warning</th>
            <th class="top">Failed</th>
            <th class="top">Pending</th>
            <th class="top">Ignored</th>
          </tr>
          <tr>
            <td class="left">
              <xsl:value-of select="count(//nunit:test-suite[@type='TestFixture'])"/> features
            </td>
            <xsl:call-template name="summary-row">
              <xsl:with-param name="summary" select="$summary" />
            </xsl:call-template>
          </tr>
        </table>
        <hr />
        <h2>Feature Summary</h2>
        <table class="reportTable" cellpadding="0" cellspacing="0">
          <tr>
            <th class="top left">Feature</th>
            <th class="top" colspan="2">Success rate (incl. warnings)</th>
            <th class="top">Scenarios</th>
            <th class="top">Success</th>
            <th class="top">Warning</th>
            <th class="top">Failed</th>
            <th class="top">Pending</th>
            <th class="top">Ignored</th>
          </tr>
          <xsl:apply-templates select="//nunit:test-suite[@type='TestFixture']" mode="summary">
            <xsl:sort select="@description" />
            <xsl:sort select="@name"/>
          </xsl:apply-templates>
        </table>
        <h2>Feature Execution Details</h2>
        <xsl:apply-templates select="//nunit:test-suite[@type='TestFixture']">
          <xsl:sort select="@description" />
          <xsl:sort select="@name"/>
        </xsl:apply-templates>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="nunit:test-suite" mode="summary">
    <xsl:variable name="featureSummary">
      <xsl:call-template name="get-summary" />
    </xsl:variable>
    <xsl:variable name="feature-id" select="generate-id()" />
    <tr>
      <td class="left">
        <a href="#{$feature-id}">
          <xsl:call-template name="get-name"/>
        </a>
      </td>
      <xsl:call-template name="summary-row">
        <xsl:with-param name="summary" select="$featureSummary" />
      </xsl:call-template>
    </tr>
  </xsl:template>

  <xsl:template name="summary-row">
    <xsl:param name="summary" />
    <td>
      <xsl:call-template name="get-success-rate">
        <xsl:with-param name="summary" select="$summary" />
      </xsl:call-template>
    </td>
    <td style="width:21em">
      <xsl:call-template name="draw-bar">
        <xsl:with-param name="summary" select="$summary" />
      </xsl:call-template>
    </td>
    <td>
      <xsl:value-of select="msxsl:node-set($summary)/*/all"/>
    </td>
    <td>
      <xsl:value-of select="msxsl:node-set($summary)/*/success"/>
    </td>
    <td>
      <xsl:value-of select="msxsl:node-set($summary)/*/warning"/>
    </td>
    <td>
      <xsl:value-of select="msxsl:node-set($summary)/*/failure"/>
    </td>
    <td>
      <xsl:value-of select="msxsl:node-set($summary)/*/pending"/>
    </td>
    <td>
      <xsl:value-of select="msxsl:node-set($summary)/*/ignored"/>
    </td>
  </xsl:template>

  <xsl:template match="nunit:test-suite">
    <xsl:variable name="feature-id" select="generate-id()" />
    <a name="{$feature-id}" />
    <h3>
      <xsl:call-template name="get-keyword">
        <xsl:with-param name="keyword" select="'Feature'" />
      </xsl:call-template>: <xsl:call-template name="get-name"/>
    </h3>
    <table class="reportTable" cellpadding="0" cellspacing="0">
      <tr>
        <th class="top left" style="word-wrap: break-word; max-width: 80%;">
          <xsl:call-template name="get-keyword">
            <xsl:with-param name="keyword" select="'Scenario'" />
          </xsl:call-template>
        </th>
        <th class="top" style="width: 5em">Status</th>
        <th class="top" style="width: 5em">Time(s)</th>
      </tr>
      <xsl:apply-templates select=".//nunit:test-case">
        <xsl:sort select="@name" />
      </xsl:apply-templates>
    </table>
  </xsl:template>

  <xsl:template match="nunit:test-case">
    <xsl:variable name="scenarioSummary">
      <xsl:call-template name="get-summary">
        <xsl:with-param name="nodes" select="." />
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="status" select="local-name(msxsl:node-set($scenarioSummary)/*/*[local-name()!='all' and text() = '1'])" />
    <xsl:variable name="scenario-id" select="generate-id()" />
    <xsl:variable name="testName" select="@name" />
    <tr>
      <td class="left" style="word-wrap: break-word; max-width: 80%;">
        <xsl:call-template name="get-name"/>
        <xsl:if test="/sfr:NUnitExecutionReport/sfr:ScenarioOutput[@name = $testName]">
          <xsl:text> </xsl:text>
          <a href="#" onclick="toggle('out{$scenario-id}', event); return false;" class="button">[<xsl:call-template name="get-common-tool-text">
              <xsl:with-param name="text-key" select="'Show'" />
            </xsl:call-template>]</a>
        </xsl:if>
      </td>
      <td class="{$status}">
        <xsl:value-of select="$status"/>
        <xsl:if test="nunit:failure or nunit:reason">
          <xsl:text> </xsl:text>
          <a href="#" onclick="toggle('err{$scenario-id}', event); return false;" class="button">[<xsl:call-template name="get-common-tool-text">
              <xsl:with-param name="text-key" select="'Show'" />
            </xsl:call-template>]</a>
        </xsl:if>
      </td>
      <td style="text-align:right;">
        <xsl:choose>
          <xsl:when test="@time">
            <xsl:value-of select='format-number(@time, "#0.000")' />
          </xsl:when>
          <xsl:otherwise>N/A</xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
    <xsl:if test="/sfr:NUnitExecutionReport/sfr:ScenarioOutput[@name = $testName]">
      <tr id="out{$scenario-id}" class="hidden">
        <td class="left subRow" style="max-width: 80%;">
          <div class="reasonPanel" style="overflow: visible; max-width: 640px">
            <pre>
              <xsl:call-template name="scenario-output">
                <xsl:with-param name="word" select="/sfr:NUnitExecutionReport/sfr:ScenarioOutput[@name = $testName]/sfr:Text"/>
              </xsl:call-template>
            </pre>
          </div>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="nunit:failure or nunit:reason">
      <tr id="err{$scenario-id}" class="hidden">
        <td class="left subRow" style="max-width: 80%;">
          <xsl:apply-templates select="*[self::nunit:failure or self::nunit:reason]" />
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template match="*[self::nunit:failure or self::nunit:reason]">
    <div class="reasonPanel" style="overflow: visible; max-width: 640px">
      <xsl:choose>
        <xsl:when test="not(nunit:message)">N/A</xsl:when>
        <xsl:otherwise>
          <b>
            <xsl:call-template name="br-replace">
              <xsl:with-param name="word" select="nunit:message"/>
            </xsl:call-template>
          </b>
        </xsl:otherwise>
      </xsl:choose>
      <!-- display the stacktrace -->
      <code>
        <br/>
        <xsl:call-template name="br-replace">
          <xsl:with-param name="word" select="nunit:stack-trace"/>
        </xsl:call-template>
      </code>
    </div>
  </xsl:template>

  <xsl:template name="br-replace">
    <xsl:param name="word"/>
    <xsl:choose>
      <xsl:when test="contains($word,'&#xA;')">
        <xsl:value-of select="substring-before($word,'&#xA;')"/>
        <br/>
        <xsl:call-template name="br-replace">
          <xsl:with-param name="word" select="substring-after($word,'&#xA;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$word"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="scenario-output">
    <xsl:param name="word"/>
    <xsl:param name="traceMode" select="Normal" />
    <xsl:choose>
      <xsl:when test="contains($word,'&#xA;')">
        <xsl:variable name="line" select="substring-before($word,'&#xA;')" />
        <xsl:variable name="newTraceMode">
          <xsl:call-template name="get-scenario-trace-mode">
            <xsl:with-param name="text" select="$line" />
            <xsl:with-param name="traceMode" select="$traceMode" />
          </xsl:call-template>
        </xsl:variable>
        <xsl:call-template name="scenario-line-output">
          <xsl:with-param name="text" select="$line" />
          <xsl:with-param name="traceMode" select="$newTraceMode" />
        </xsl:call-template>
        <xsl:call-template name="scenario-output">
          <xsl:with-param name="word" select="substring-after($word,'&#xA;')"/>
          <xsl:with-param name="traceMode" select="$newTraceMode" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:variable name="newTraceMode">
          <xsl:call-template name="get-scenario-trace-mode">
            <xsl:with-param name="text" select="$word" />
            <xsl:with-param name="traceMode" select="$traceMode" />
          </xsl:call-template>
        </xsl:variable>
        <xsl:call-template name="scenario-line-output">
          <xsl:with-param name="text" select="$word" />
          <xsl:with-param name="traceMode" select="$newTraceMode" />
        </xsl:call-template>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="get-scenario-trace-mode">
    <xsl:param name="text" />
    <xsl:param name="traceMode" />

    <xsl:choose>
      <xsl:when test="starts-with($text, '->')">Trace</xsl:when>
      <xsl:when test="$traceMode = 'Trace' and not(starts-with($text, ' '))">Normal</xsl:when>
      <xsl:when test="$traceMode = 'Trace'">Trace</xsl:when>
      <xsl:otherwise>Normal</xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="scenario-line-output">
    <xsl:param name="text" />
    <xsl:param name="traceMode" />
    <xsl:choose>
      <xsl:when test="$traceMode = 'Trace'">
        <span class="traceMessage">
          <xsl:value-of select="$text"/>
        </span>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="get-summary">
    <xsl:param name="nodes" select=".//nunit:test-case" />
    <testCaseSummary>
      <all>
        <xsl:value-of select="count($nodes)"/>
      </all>
      <success>
        <xsl:value-of select="count($nodes[@result = 'Success' and @success = 'True'])"/>
      </success>
      <warning>
        <xsl:value-of select="count($nodes[@result = 'Success' and @success = 'False'])"/>
      </warning>
      <failure>
        <xsl:value-of select="count($nodes[@result = 'Failure' or @result = 'Error'])"/>
      </failure>
      <pending>
        <xsl:value-of select="count($nodes[@result = 'Inconclusive'])"/>
      </pending>
      <ignored>
        <xsl:value-of select="count($nodes[@executed = 'False'])"/>
      </ignored>
    </testCaseSummary>
  </xsl:template>

  <xsl:template name="get-success-rate">
    <xsl:param name="summary" />

    <xsl:value-of select="round(msxsl:node-set($summary)/*/success * 100 div msxsl:node-set($summary)/*/all)"/>
    <xsl:text>% (</xsl:text>
    <xsl:value-of select="round((msxsl:node-set($summary)/*/success + msxsl:node-set($summary)/*/warning) * 100 div msxsl:node-set($summary)/*/all)"/>
    <xsl:text>%)</xsl:text>
  </xsl:template>

  <xsl:template name="draw-bar">
    <xsl:param name="summary" />

    <xsl:variable name="testCount" select="msxsl:node-set($summary)/*/all" />
    <xsl:variable name="successWidth" select="(msxsl:node-set($summary)/*/success * 20 div $testCount)" />
    <xsl:variable name="warningWidth" select="(msxsl:node-set($summary)/*/warning * 20 div $testCount)" />
    <xsl:variable name="failureWidth" select="(msxsl:node-set($summary)/*/failure * 20 div $testCount)" />
    <xsl:variable name="pendingWidth" select="(msxsl:node-set($summary)/*/pending * 20 div $testCount)" />
    <xsl:variable name="ignoredWidth" select="(msxsl:node-set($summary)/*/ignored * 20 div $testCount)" />

    <div class="bar">
      <xsl:if test="$successWidth != 0">
        <div class="success" style="width:{$successWidth}em" title="{msxsl:node-set($summary)/*/success} succeeded">
          <xsl:text disable-output-escaping="yes">
            <![CDATA[&nbsp;]]>
          </xsl:text>
        </div>
      </xsl:if>
      <xsl:if test="$warningWidth != 0">
        <div class="warning" style="width:{$warningWidth}em" title="{msxsl:node-set($summary)/*/warning} warned">
          <xsl:text disable-output-escaping="yes">
            <![CDATA[&nbsp;]]>
          </xsl:text>
        </div>
      </xsl:if>
      <xsl:if test="$failureWidth != 0">
        <div class="failure" style="width:{$failureWidth}em" title="{msxsl:node-set($summary)/*/failure} failed">
          <xsl:text disable-output-escaping="yes">
            <![CDATA[&nbsp;]]>
          </xsl:text>
        </div>
      </xsl:if>
      <xsl:if test="$pendingWidth != 0">
        <div class="pending" style="width:{$pendingWidth}em" title="{msxsl:node-set($summary)/*/pending} pending/not bound">
          <xsl:text disable-output-escaping="yes">
            <![CDATA[&nbsp;]]>
          </xsl:text>
        </div>
      </xsl:if>
      <xsl:if test="$ignoredWidth != 0">
        <div class="ignored" style="width:{$ignoredWidth}em" title="{msxsl:node-set($summary)/*/ignored} ignored">
          <xsl:text disable-output-escaping="yes">
            <![CDATA[&nbsp;]]>
          </xsl:text>
        </div>
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template name="get-name">
    <xsl:choose>
      <xsl:when test="./@description">
        <span title="{@name}">
          <xsl:value-of select="./@description"/>
        </span>
      </xsl:when>
      <xsl:when test="../../@type='ParameterizedTest' and ../../@description">
        <span title="{@name}">
          <xsl:value-of select="../../@description"/>
          <xsl:text>(</xsl:text>
          <xsl:value-of select="substring-after(./@name,'(')"/>
        </span>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="./@name"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
