<?xml version="1.0" encoding="utf-8"?>
<!-- to be finished and used from   -->
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sfr="urn:TechTalk:SpecFlow.Report"
                exclude-result-prefixes="msxsl">

  <xsl:param name="feature-language" select="'en'" />
  <xsl:param name="languages" select="document('Languages.xml')" />

  <xsl:template name="get-keyword">
    <xsl:param name="keyword" />
    <xsl:param name="language">
      <xsl:choose>
        <xsl:when test="ancestor::sfr:Feature[1]/sfr:Language">
          <xsl:value-of select="ancestor::sfr:Feature[1]/sfr:Language"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$feature-language" />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:param>

    <xsl:variable name="local-keyword">
      <xsl:choose>
        <xsl:when test="$languages//Language[@code = $language]">
          <xsl:value-of select="$languages//Language[@code = $language]/node()[local-name() = $keyword]"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:variable name="base-language" select="substring-before($language, '-')" />
          <xsl:value-of select="$languages//Language[@code = $base-language]/node()[local-name() = $keyword]"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:value-of select="$local-keyword"/>
  </xsl:template>

  <xsl:template match="sfr:ScenarioStep">
    <xsl:variable name="step-id" select="generate-id()" />
    <xsl:variable name="step-keyword" select="@xsi:type" />
    <span id="txt{$step-id}">
      <span class="stepKeyword">
        <xsl:call-template name="get-keyword">
          <xsl:with-param name="keyword" select="$step-keyword" />
        </xsl:call-template>
      </span>
      <xsl:text> </xsl:text>
      <xsl:apply-templates select="sfr:Text" />
    </span>
    <xsl:apply-templates select="." mode="clipboardCopy"/>
    <xsl:apply-templates select="sfr:TableArg" />
    <xsl:apply-templates select="sfr:MultiLineTextArgument" />
  </xsl:template>

  <xsl:template match="sfr:ScenarioStep" priority="-0.5" mode="clipboardCopy">
    <!-- nop -->
  </xsl:template>

  <xsl:template match="sfr:Text" priority="-0.1">
    <xsl:value-of select="string(node())"/>
  </xsl:template>

  <xsl:template match="sfr:TableArg">
    <xsl:variable name="table-id" select="concat('tbl', generate-id(..))" />
    <table class="tableArg" id="{$table-id}" cellpadding="0" cellspacing="0">
      <tr>
        <xsl:for-each select="sfr:Header/sfr:Cells/sfr:Cell">
          <th>
            <xsl:attribute name="class">
              <xsl:choose>
                <xsl:when test="position()=1">left top</xsl:when>
                <xsl:otherwise>top</xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <xsl:value-of select="sfr:Value"/>
          </th>
        </xsl:for-each>
      </tr>
      <xsl:for-each select="sfr:Body/sfr:Row">
        <tr>
          <xsl:for-each select="sfr:Cells/sfr:Cell">
            <td>
              <xsl:attribute name="class">
                <xsl:choose>
                  <xsl:when test="position()=1">left</xsl:when>
                  <xsl:otherwise></xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
              <xsl:value-of select="sfr:Value"/>
            </td>
          </xsl:for-each>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template match="sfr:MultiLineTextArgument">
    <pre>
      <xsl:value-of select="."/>
    </pre>
  </xsl:template>
</xsl:stylesheet>
