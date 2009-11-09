<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:user="http://mycompany.com/mynamespace"
    exclude-result-prefixes="msxsl"
  >
  <xsl:output method="text" indent="no" encoding="utf-8"/>
  <xsl:param name="language" select="'de'" />

  <msxsl:script language="JScript" implements-prefix="user">
    <![CDATA[
    function lpad(text, padString, length) {
	    var str = text;
        while (str.length < length)
            str = padString + str;
        return str;
    }
    
    function replaceUnicode(text) {
    
      var result = "";
      for (var i = 0; i < text.length; i++) {
        var charCode = text.charCodeAt(i);
        if (charCode < 128)
          result += text.charAt(i);
        else 
          result += "\\u" + lpad(text.charCodeAt(i).toString(16), "0", 4);
      }
      return "" + result; // "" is needed to workatound msxsl compiler issue
    }
    ]]>
  </msxsl:script>

  <xsl:template match="/">
    <xsl:apply-templates select="//Language[@code = $language]" />
  </xsl:template>

  <xsl:template match="Language">
    lexer grammar SpecFlowLangLexer_<xsl:value-of select="@code"/>;

    options {
    language = CSharp2;
    filter=true;
    superClass = SpecFlowLangLexer;
    }

    @lexer::namespace { TechTalk.SpecFlow.Parser.Grammar }

    fragment WSCHAR     : (' '|'\t');
    fragment NONWCHR    : (' '|'\t'|'\r'|'\n'|'#'|'@');
    fragment NEWLINECHR : ('\r'|'\n');
    fragment NONNLCHR   : ('\u0000'..'\t')|('\u000B'..'\f')|('\u000E'..'\uFFFF');


    T_FEATURE : <xsl:call-template name="token-list">
      <xsl:with-param name="nodes" select="Feature" />
    </xsl:call-template>;
    T_BACKGROUND : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="Background" />
  </xsl:call-template>;
    T_SCENARIO : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="Scenario" />
  </xsl:call-template>;
    T_SCENARIO_OUTLINE : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="ScenarioOutline" />
  </xsl:call-template>;
    T_EXAMPLES : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="Examples" />
  </xsl:call-template>;
    T_GIVEN : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="Given" />
    <xsl:with-param name="postfix" select="''" />
  </xsl:call-template>;
    T_WHEN : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="When" />
      <xsl:with-param name="postfix" select="''" />
    </xsl:call-template>;
    T_THEN : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="Then" />
      <xsl:with-param name="postfix" select="''" />
    </xsl:call-template>;
    T_AND : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="And" />
    <xsl:with-param name="postfix" select="''" />
  </xsl:call-template>;
    T_BUT : <xsl:call-template name="token-list">
    <xsl:with-param name="nodes" select="But" />
    <xsl:with-param name="postfix" select="''" />
  </xsl:call-template>;

    MLTEXT		: '"""';
    CELLSEP		: '|';
    AT          : '@';
    COMMENT     : WSCHAR* '#' NONNLCHR* { $channel = Token.HIDDEN_CHANNEL; };
    WS          : WSCHAR+;
    NEWLINE     : '\r\n' | '\n';
    WORDCHAR    : (('\u0000'..'\b')
    | ('\u000B'..'\f')
    | ('\u000E'..'\u001F')
    | ('!'..'"')
    | ('$'..'?')
    | ('A'..'\uFFFF'))+ ;
  </xsl:template>

  <xsl:template name="token-list">
    <xsl:param name="nodes" />
    <xsl:param name="postfix" select="':'" />

    <xsl:for-each select="$nodes">
      <xsl:if test="position() > 1">
        <xsl:text>|</xsl:text>
      </xsl:if>
      <xsl:text>'</xsl:text>
      <xsl:value-of select="user:replaceUnicode(string(.))"/>
      <xsl:value-of select="$postfix"/>
      <xsl:text>'</xsl:text>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
