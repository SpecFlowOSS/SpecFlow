lexer grammar SpecFlowLangLexer;

options {
	language = CSharp2;
    filter=true;
}

@lexer::namespace { TechTalk.SpecFlow.Parser.Grammar }

fragment WSCHAR     : (' '|'\t');
fragment NONWCHR    : (' '|'\t'|'\r'|'\n'|'#'|'@');
fragment NEWLINECHR : ('\r'|'\n');
fragment NONNLCHR   : ('\u0000'..'\t')|('\u000B'..'\f')|('\u000E'..'\uFFFF');


T_FEATURE : 'Feature:';
T_BACKGROUND : 'Background:';
T_SCENARIO : 'Scenario:';
T_SCENARIO_OUTLINE : 'Scenario Outline:';
T_EXAMPLES : 'Examples:'|'Scenarios:';
T_GIVEN : 'Given';
T_WHEN : 'When';
T_THEN : 'Then';
T_AND : 'And';
T_BUT : 'But';

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
    