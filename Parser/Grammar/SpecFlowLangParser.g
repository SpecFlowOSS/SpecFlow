parser grammar SpecFlowLangParser;

options {
	tokenVocab = SpecFlowLangLexer;
	language = CSharp2;
	output = AST;
    backtrack = true;
}

//import SpecFlowLangLexer;

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
    :   WS? T_BACKGROUND 
        (WS title)? 
        newlineWithSpaces givens
        -> ^(BACKGROUND title? givens)
    ;

scenarioKind
    :   scenarioOutline 
    |   scenario
    ;

scenario
    :   tags? WS? T_SCENARIO WS? 
        title newlineWithSpaces 
        steps
        -> ^(SCENARIO tags? title steps)
    ;

scenarioOutline
    :
        tags? WS? T_SCENARIO_OUTLINE WS?
        title newlineWithSpaces
        steps
        examples
        -> ^(SCENARIOOUTLINE tags? title steps examples)
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
    :   WS? T_AND WS sentenceEnd
        -> ^(AND sentenceEnd)
    ;

firstBut
    :   WS? T_BUT WS sentenceEnd
        -> ^(BUT sentenceEnd)
    ;

givens
    :   firstGiven nextStep*
        -> ^(STEPS firstGiven nextStep*)
    ;
firstGiven
    :   WS? T_GIVEN WS sentenceEnd
        -> ^(GIVEN sentenceEnd)
    ;
firstWhen
    :   WS? T_WHEN WS sentenceEnd
        -> ^(WHEN sentenceEnd)
    ;
firstThen
    :   WS? T_THEN WS sentenceEnd
        -> ^(THEN sentenceEnd)
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
    :   WS? CELLSEP tableCell+ WS? newlineWithSpaces
        -> ^(ROW tableCell+)
    ;

tableCell
    :   WS? text WS? CELLSEP
        -> ^(CELL text)
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