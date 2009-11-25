parser grammar SpecFlowLangParser;

options {
	tokenVocab = SpecFlowLangLexer_en;
	language = CSharp2;
	output = AST;
    backtrack = true;
}

tokens {
    FEATURE;
    DESCRIPTIONLINE;
    BACKGROUND;
    SCENARIOS;
    SCENARIO;
    SCENARIOOUTLINE;
    EXAMPLES;
    EXAMPLESET;
    STEPS;
    GIVEN;
    WHEN;
    THEN;
    TEXT;
    AND;
    BUT;
    TAGS;
    TAG;
    WORD;
    MULTILINETEXT;
    INDENT;
    LINE;
    TABLE;
    HEADER;
    BODY;
    ROW;
    CELL;
    FILEPOSITION;
}

//@lexer::namespace { TechTalk.SpecFlow.Parser.Grammar }
@namespace { TechTalk.SpecFlow.Parser.Grammar }

feature
    :   newlineWithSpaces?
        tags?
        WS? T_FEATURE WS? text newlineWithSpaces
        descriptionLine*
        background?
        scenarioKind* WS? EOF
        -> ^(FEATURE tags? text descriptionLine* background? 
            ^(SCENARIOS scenarioKind*)
            )
    ;

tags
    :   WS? tag+
        -> ^(TAGS tag+)
    ;

tag
    :   AT word (newlineWithSpaces|WS)
        -> ^(TAG word)
    ;

word
    :   WORDCHAR+
        -> ^(WORD WORDCHAR+)
    ;

descriptionLine
    :   WS? descriptionLineText newlineWithSpaces
        -> ^(DESCRIPTIONLINE descriptionLineText)
    ;

background
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		T_BACKGROUND 
        (WS title)? 
        newlineWithSpaces givens
        -> ^(BACKGROUND title? givens FILEPOSITION[fp_])
    ;

scenarioKind
    :   scenarioOutline 
    |   scenario
    ;

scenario
@init {
	string fp_ = null;
}
    :   tags? WS? 
		{
			fp_ = GetFilePosition();
		}
		T_SCENARIO WS? 
        title newlineWithSpaces 
        steps
        -> ^(SCENARIO tags? title steps FILEPOSITION[fp_])
    ;

scenarioOutline
@init {
	string fp_ = null;
}
    :
        tags? WS? 
		{
			fp_ = GetFilePosition();
		}
        T_SCENARIO_OUTLINE WS?
        title newlineWithSpaces
        steps
        examples
        -> ^(SCENARIOOUTLINE tags? title steps examples FILEPOSITION[fp_])
    ;

examples
    :   exampleSet+
        -> ^(EXAMPLES exampleSet+)
    ;

exampleSet
    :   WS? T_EXAMPLES WS?
        text? newlineWithSpaces table
        -> ^(EXAMPLESET text? table)
    ;

steps
    :   firstStep nextStep*
        -> ^(STEPS firstStep nextStep*)
    ;
firstStep
	:	firstGiven -> firstGiven
	|	firstWhen -> firstWhen
	|	firstThen -> firstThen
    ;
nextStep
    :   firstStep -> firstStep
	|	firstAnd -> firstAnd
	|	firstBut -> firstBut
    ;

firstAnd
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		T_AND WS sentenceEnd
        -> ^(AND sentenceEnd FILEPOSITION[fp_])
    ;

firstBut
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		T_BUT WS sentenceEnd
        -> ^(BUT sentenceEnd FILEPOSITION[fp_])
    ;

givens
    :   firstGiven nextStep*
        -> ^(STEPS firstGiven nextStep*)
    ;
firstGiven
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		T_GIVEN WS sentenceEnd
        -> ^(GIVEN sentenceEnd FILEPOSITION[fp_])
    ;
firstWhen
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		T_WHEN WS sentenceEnd
        -> ^(WHEN sentenceEnd FILEPOSITION[fp_])
    ;
firstThen
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		T_THEN WS sentenceEnd
        -> ^(THEN sentenceEnd FILEPOSITION[fp_])
    ;

sentenceEnd
    :   text newlineWithSpaces multilineText? table?
        -> text multilineText? table?
    ;

multilineText
    :   indent MLTEXT WS? NEWLINE
        multilineTextLine*
        WS? MLTEXT WS? newlineWithSpaces
        -> ^(MULTILINETEXT multilineTextLine* indent)
    ;

indent
    :   WS? -> ^(INDENT WS?)
    ;

multilineTextLine
    :   WS? text? NEWLINE
        -> ^(LINE WS? text? NEWLINE)
    ;

table
    :   tableRow tableRow+
        -> ^(TABLE ^(HEADER tableRow) ^(BODY tableRow+))
    ;

tableRow
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		CELLSEP tableCell+ WS? newlineWithSpaces
        -> ^(ROW tableCell+ FILEPOSITION[fp_])
    ;

tableCell
@init {
	string fp_ = null;
}
    :   WS? 
		{
			fp_ = GetFilePosition();
		}
		text WS? CELLSEP
        -> ^(CELL text FILEPOSITION[fp_])
    ;

descriptionLineText
    :   WORDCHAR textRest*
        -> ^(TEXT WORDCHAR textRest*)
    ;

text
    :   wordchar textRest*
        -> ^(TEXT wordchar textRest*)
    ;
textRest
    :   WS textRest
    |   wordchar
    ;

title
    :   wordchar titleRest*
        -> ^(TEXT wordchar titleRest*)
    ;

titleRest
    :   WS titleRest
    |   NEWLINE titleRest
    |   wordchar
    ;

titleRestLine
    :   NEWLINE titleRestLine
    |   WS titleRestLine
    |   WORDCHAR
    ;

wordchar
    :   WORDCHAR
    |   AT
    ;

newlineWithSpaces
    :   WS? NEWLINE (WS? NEWLINE)*
    ;

/*fragment WSCHAR     : (' '|'\t');
fragment NONWCHR    : (' '|'\t'|'\r'|'\n'|'#'|'@');
fragment NEWLINECHR : ('\r'|'\n');
fragment NONNLCHR   : ('\u0000'..'\t')|('\u000B'..'\f')|('\u000E'..'\uFFFF');


T_FEATURE : 'AlmaFeature:';
T_BACKGROUND : 'KorteBackground:';
T_SCENARIO : 'BarackScenario:';
T_SCENARIO_OUTLINE : 'BananScenario Outline:';
T_EXAMPLES : 'SzilvaExamples:'|'EperScenarios:';
T_GIVEN : 'NarancsGiven';
T_WHEN : 'SzoloWhen';
T_THEN : 'EgresThen';
T_AND : 'DinnyeAnd';
T_BUT : 'GyumolcsBut';

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

*/