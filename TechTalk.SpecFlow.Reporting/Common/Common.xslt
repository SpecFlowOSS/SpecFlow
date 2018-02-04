﻿<?xml version="1.0" encoding="utf-8"?>
<!-- to be finished and used from   -->
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                xmlns:sfr="urn:TechTalk:SpecFlow.Report"
                exclude-result-prefixes="msxsl">

  <xsl:param name="tool-language" select="'en'" />
  <xsl:param name="base-tool-language" select="substring-before($tool-language, '-')" />

  <xsl:include href="GherkinElements.xslt"/>

  <xsl:param name="common-tool-text">
    <Language code="en">
      <GeneratedByPre>Generated by SpecFlow at </GeneratedByPre>
      <GeneratedByPost></GeneratedByPost>
      <See>see </See>
      <Show>show</Show>
      <Hide>hide</Hide>
      <Copy>copy</Copy>
    </Language>
    <Language code="de">
      <GeneratedByPre>Erstellt durch SpecFlow am </GeneratedByPre>
      <GeneratedByPost></GeneratedByPost>
      <See></See>
      <Show>anzeigen</Show>
      <Hide>ausblenden</Hide>
      <Copy>kopieren</Copy>
    </Language>
  </xsl:param>

  <xsl:template name="get-common-tool-text">
    <xsl:param name="text-key" />

    <xsl:call-template name="get-tool-text-base">
      <xsl:with-param name="text-key" select="$text-key" />
      <xsl:with-param name="local-tool-texts" select="msxsl:node-set($common-tool-text)" />
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="get-tool-text-base">
    <xsl:param name="text-key" />
    <xsl:param name="local-tool-texts" />

    <xsl:choose>
      <xsl:when test="$local-tool-texts//Language[@code = $tool-language]">
        <xsl:value-of select="$local-tool-texts//Language[@code = $tool-language]/node()[local-name() = $text-key]"/>
      </xsl:when>
      <xsl:when test="$local-tool-texts//Language[@code = $base-tool-language]">
        <xsl:value-of select="$local-tool-texts//Language[@code = $base-tool-language]/node()[local-name() = $text-key]"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$local-tool-texts//Language[@code = 'en']/node()[local-name() = $text-key]"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="sfr:ScenarioStep" priority="-0.1" mode="clipboardCopy">
    <xsl:variable name="step-id" select="generate-id()" />
    
    <xsl:text> </xsl:text>
    <a href="#" onclick="copyStepToClipboard('{$step-id}'); return false;" class="button">[<xsl:call-template name="get-common-tool-text">
      <xsl:with-param name="text-key" select="'Copy'" />
    </xsl:call-template>]</a>
  </xsl:template>

  <xsl:template name="html-common-header">
    <xsl:param name="title" />

    <xsl:comment>Generated by SpecFlow (see http://www.specflow.org/)</xsl:comment>
    <title>
      <xsl:value-of select="$title"/>
    </title>
    <style>
        <![CDATA[
      body
      {
        font: small verdana, arial, helvetica; color:#000000;
      }
      h1
      {
        font-size: 16px;
      }
      h2
      {
        font-size: 14px;
      }
      h3
      {
        font-size: 12px;
      }
      div.marker
      {
        height: 1.1em;
        width: 1.1em;
        float:left;
        margin-right: 0.3em;
      }
      table.reportTable
      {
        width: 100%;
        font-size: 12px;
      }
      table.subReportTable
      {
        font-size: 12px;
        margin-left: 2em;
      }
      td, th
      {
        text-align: left;
        border-bottom: solid 1px #dcdcdc;
        border-right: solid 1px #dcdcdc;
        padding-left: 0.5em;
        padding-right: 0.5em;
        padding-top: 0.25em;
        padding-bottom: 0.25em;
      }
      th
      {
        background-color: #FFF2E5;
        padding-top: 0.4em;
        padding-bottom: 0.4em;
      }
      .top
      {
        border-top: solid 1px #dcdcdc;
      }
      .left
      {
        border-left: solid 1px #dcdcdc;
      }
      td.accessPath, th.accessPath
      {
        padding-left: 1.0em;
      }
      td.accessPath
      {
        font-style:italic;
      }
      td.empty
      {
        border: none;
        height: 2.0em;
      }
      td.numeric
      {
        text-align: right;
      }
      td.marker
      {
        white-space: nowrap;
      }
      td.subRow
      {
        padding-left: 2em;
      }
      
      div.legend
      {
        margin-top: 2em;
        padding-left: 2em;
        font-style:italic;
        font-size: 10px;
      }
      a.button
      {
      }
      .hidden
      {
        display: none;
      }

      // gherkin elements style
      table.tableArg
      {
        font-size: 12px;
        margin-left: 2em;
      }
      table.tableArg th
      {
        background-color: #BBFFBB;
      }
      .stepKeyword
      {
        font-style:italic;
      }
      .stepParam
      {
        font-style:italic;
        color: Green;
      }
      ]]></style>
    <script>
      var showButtonText = "[<xsl:call-template name="get-common-tool-text">
      <xsl:with-param name="text-key" select="'Show'" />
    </xsl:call-template>]";
      var hideButtonText = "[<xsl:call-template name="get-common-tool-text">
      <xsl:with-param name="text-key" select="'Hide'" />
    </xsl:call-template>]";
      <![CDATA[
          function toggle(sdid, event){
            var link;
            if(window.event) {
              link = window.event.srcElement;
            } else {
              link = event.target;
            }

            toToggle=document.getElementById(sdid);
            if (link.innerHTML==showButtonText)
            {
              link.innerHTML=hideButtonText;
              toToggle.style.display="block";
            }
            else
            {
              link.innerHTML=showButtonText;
              toToggle.style.display="none";
            }
          }

          function copyToClipboard(s)
          {
            if (window.clipboardData)
            {
              window.clipboardData.setData('Text',s);
            }
            else
            {
              try
              {
                netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
              }
              catch(e)
              {
                alert("The clipboard copy didn't work.\nYour browser doesn't allow Javascript clipboard copy.\nIf you want to change its behaviour: \n1.Open a new browser window\n2.Enter the URL: about:config\n3.Change the signed.applets.codebase_principal_support property to true");
                return;
              }
              var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
              var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
              trans.addDataFlavor('text/unicode');
              var len = new Object();
              var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
              str.data=s;
              trans.setTransferData("text/unicode",str,s.length*2);
              var clipid=Components.interfaces.nsIClipboard;
              clip.setData(trans,null,clipid.kGlobalClipboard);
            }
          } 
          ]]>
    </script>
  </xsl:template>
  
  <xsl:template name="html-copy-step-to-clipboard-script">
    <xsl:text disable-output-escaping="yes">
      <![CDATA[
        <script>
          function getInnerText(elm)
          {
            if (elm.textContent)
              return elm.textContent;
            
            return elm.innerText;
          }
      
          function copyStepToClipboard(stepId)
          {
            text=document.getElementById("txt" + stepId);
            tableArg=document.getElementById("tbl" + stepId);
            
            result = getInnerText(text);
            if (tableArg != null)
            {
              result += "\r\n";
              result += getTableSource(tableArg);
            }
            
            copyToClipboard(result);
          }
          
          function getTableSource(table)
          {
            indent = "  ";
            
            var rows = table.getElementsByTagName("tr");   
            header = rows[0].cells;
            
            columnWidths = new Array();
            for (colIndex = 0; colIndex < header.length; colIndex++)
                columnWidths[colIndex] = header[colIndex].innerHTML.length;

            for (rowIndex = 1; rowIndex < rows.length; rowIndex++)
            {
                row = rows[rowIndex].cells;
                for (colIndex = 0; colIndex < header.length; colIndex++)
                    if (row[colIndex].innerHTML.length > columnWidths[colIndex])
                        columnWidths[colIndex] = row[colIndex].innerHTML.length;
            }

            tableSource = "";
            for (rowIndex = 0; rowIndex < rows.length; rowIndex++)
            {
                row = rows[rowIndex].cells;
                tableSource = tableSource + indent + "|";
                for (colIndex = 0; colIndex < header.length; colIndex++)
                {
                  tableSource = tableSource + " ";
                  tableSource = tableSource + row[colIndex].innerHTML;
                  for (i = row[colIndex].innerHTML.length; i < columnWidths[colIndex]; i++)
                    tableSource = tableSource + " ";
                  tableSource = tableSource + " |";
                }
                tableSource = tableSource + "\r\n";
            }
            return tableSource;
          }

        </script>
      ]]>
    </xsl:text>
  </xsl:template>

  <xsl:template name="html-body-header">
    <xsl:param name="title" />
    <xsl:param name="generatedAt" select="/*/@generatedAt" />
    <h1>
      <xsl:value-of select="$title"/>
    </h1>

    <xsl:call-template name="get-common-tool-text">
      <xsl:with-param name="text-key" select="'GeneratedByPre'" />
    </xsl:call-template>
    <xsl:value-of select="$generatedAt"/>
    <xsl:call-template name="get-common-tool-text">
      <xsl:with-param name="text-key" select="'GeneratedByPost'" />
    </xsl:call-template>
    <xsl:text> (</xsl:text>
    <xsl:call-template name="get-common-tool-text">
      <xsl:with-param name="text-key" select="'See'" />
    </xsl:call-template>
    <a href="http://www.specflow.org/">http://www.specflow.org/</a>).
  </xsl:template>
</xsl:stylesheet>
