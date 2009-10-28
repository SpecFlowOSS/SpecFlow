<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sfr="urn:TechTalk:SpecFlow.Report"
                exclude-result-prefixes="msxsl">
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"/>

  <xsl:include href="..\Common\Common.xslt"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:key name="step-usage-variation" match="sfr:Instance" use="normalize-space(string(sfr:ScenarioStep))"/>

  <xsl:template match="/">
    <xsl:variable name="title">
      <xsl:value-of select="sfr:StepDefinitionReport/@projectName"/> Step Definition Report
    </xsl:variable>
    <html>
      <head>
        <xsl:call-template name="html-common-header">
          <xsl:with-param name="title" select="$title" />
        </xsl:call-template>
        <style>
          <![CDATA[
          a.instanceRef
          {
            font-size: 10px;
            margin-left: 2em;
          }
          .noBinding td
          {
            background-color: #FFFFBB;
          }
          .noInstances td
          {
            background-color: #FFBBBB;
          }
          ]]>
        </style>
        <xsl:call-template name="html-copy-step-to-clipboard-script" />
      </head>
      <body>
        <xsl:call-template name="html-body-header">
          <xsl:with-param name="title" select="$title" />
        </xsl:call-template>
        <xsl:if test="(/sfr:StepDefinitionReport/@onlySelectedFeatures='true')">
          <div class="legend">
            Only the following features from the project are included in this report:
            <xsl:variable name="featureRefs">
              <FeatureRefs>
                <xsl:for-each select="//sfr:FeatureRef">
                  <FeatureName>
                    <xsl:value-of select="@name"/>
                  </FeatureName>
                </xsl:for-each>
              </FeatureRefs>
            </xsl:variable>
            <ul>
            <xsl:for-each select="msxsl:node-set($featureRefs)/*/*">
              <xsl:variable name="feature-name" select="string(text())" />
              <xsl:if test="not(preceding-sibling::FeatureName[string(text())=$feature-name])">
                <li>
                  <xsl:value-of select="$feature-name"/>
                </li>
              </xsl:if>
            </xsl:for-each>
            </ul>
          </div>
        </xsl:if>        
        <xsl:if test="(/sfr:StepDefinitionReport/@showBindingsWithoutInsance!='true')">
          <div class="legend">
            Bindings without instances are not included in this report.
          </div>
        </xsl:if>        
        <h2>Givens</h2>
        <xsl:call-template name="process-block">
          <xsl:with-param name="block-name" select="'Given'" />
        </xsl:call-template>
        <h2>Whens</h2>
        <xsl:call-template name="process-block">
          <xsl:with-param name="block-name" select="'When'" />
        </xsl:call-template>
        <h2>Thens</h2>
        <xsl:call-template name="process-block">
          <xsl:with-param name="block-name" select="'Then'" />
        </xsl:call-template>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="process-block">
    <xsl:param name="block-name" />

    <table class="reportTable" cellpadding="0" cellspacing="0">
      <tr>
        <th class="top left">Step Definition</th>
        <th class="top">Instances</th>
      </tr>
      <xsl:for-each select="sfr:StepDefinitionReport/sfr:StepDefinition[@type=$block-name]">
        <xsl:sort order="ascending" select="sfr:ScenarioStep/sfr:Text" data-type="text"/>
        <xsl:if test="(/sfr:StepDefinitionReport/@showBindingsWithoutInsance='true') or sfr:Instances">
          <xsl:variable name="stepdef-id" select="generate-id()" />
          <tr>
            <xsl:attribute name="class">
              <xsl:choose>
                <xsl:when test="not(sfr:Binding)">noBinding</xsl:when>
                <xsl:when test="not(sfr:Instances)">noInstances</xsl:when>
                <xsl:otherwise></xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <td class="left">
              <xsl:apply-templates select="sfr:ScenarioStep" />
            </td>
            <td>
              <xsl:value-of select="count(./sfr:Instances/sfr:Instance)"/>
              <xsl:if test="sfr:Instances">
                <xsl:text> </xsl:text>
                <a href="#" onclick="toggle('{$stepdef-id}'); return false;" class="button">[show]</a>
              </xsl:if>
            </td>
          </tr>
          <xsl:if test="sfr:Instances">
            <tr id="{$stepdef-id}" class="hidden">
              <td class="left" colspan="2">
                <table class="subReportTable" cellpadding="0" cellspacing="0">
                  <tr>
                    <th class="top left">Instances:</th>
                  </tr>
                  <xsl:for-each select="sfr:Instances/sfr:Instance">
                    <xsl:variable name="current-key" select="normalize-space(string(sfr:ScenarioStep))" />
                    <xsl:if test="generate-id() = generate-id(key('step-usage-variation', $current-key))">
                      <tr>
                        <td class="left">
                          <!--<xsl:value-of select="count(key('step-usage-variation', $current-key))"/>-->
                          <xsl:apply-templates select="sfr:ScenarioStep" />
                          <br/>
                          <xsl:for-each select="../sfr:Instance[normalize-space(string(sfr:ScenarioStep)) = $current-key]">
                            <xsl:apply-templates select="." mode="instance-ref" />
                            <br/>
                          </xsl:for-each>
                        </td>
                      </tr>
                    </xsl:if>
                  </xsl:for-each>
                </table>
              </td>
            </tr>
          </xsl:if>
        </xsl:if>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template match="sfr:Instance" mode="instance-ref">
    <b>
      <a class="instanceRef" href="{sfr:FeatureRef/@file}">
        <xsl:value-of select="sfr:FeatureRef/@name"/>
        <xsl:text> / </xsl:text>
        <xsl:value-of select="sfr:ScenarioRef/@name"/>
        <xsl:if test="@fromScenarioOutline = 'true'">
          <xsl:text> (scenario outline example)</xsl:text>
        </xsl:if>
      </a>
    </b>
  </xsl:template>

  <xsl:template name="step-param">
    <xsl:param name="param-name" select="@name" />
    <xsl:param name="instances-root" select="../../sfr:Instances" />
    <xsl:variable name="param-sample">
      <xsl:choose>
        <xsl:when test="$instances-root/sfr:Instance/sfr:Parameters/sfr:Parameter[@name=$param-name]">
          <xsl:value-of select="$instances-root/sfr:Instance/sfr:Parameters/sfr:Parameter[@name=$param-name]"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>{</xsl:text>
          <xsl:value-of select="$param-name"/>
          <xsl:text>}</xsl:text>
        </xsl:otherwise>
      </xsl:choose>

    </xsl:variable>
    <xsl:variable name="param-values">
      <xsl:text>Used values for parameter '</xsl:text><xsl:value-of select="$param-name"/>'<xsl:text>:
</xsl:text>
      <xsl:for-each select="$instances-root/sfr:Instance/sfr:Parameters/sfr:Parameter[@name=$param-name]">
        <xsl:variable name="param-value" select="string(text())" />
        <xsl:if test="not(../../preceding-sibling::sfr:Instance[string(sfr:Parameters/sfr:Parameter[@name=$param-name]/text())=$param-value])">
          <xsl:value-of select="text()"/>
          <xsl:text>
</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:variable>
    <span class="stepParam" title="{$param-values}">
      <xsl:value-of select="$param-sample"/>
    </span>
  </xsl:template>

  <xsl:template match="sfr:Text">
    <xsl:call-template name="display-text">
      <xsl:with-param name="text" select="string(node())" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="display-text">
    <xsl:param name="text" />

    <xsl:choose>
      <xsl:when test="contains($text, '{')">
        <xsl:value-of select="substring-before($text, '{')" />
        <xsl:variable name="rest" select="substring-after($text, '{')" />
        <xsl:call-template name="step-param">
          <xsl:with-param name="param-name" select="substring-before($rest, '}')" />
        </xsl:call-template>
        <xsl:call-template name="display-text">
          <xsl:with-param name="text" select="substring-after($rest, '}')" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
