grammar SpecFlowLang;

options {
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
    //GIVENS;
    GIVEN;
    //WHENS;
    WHEN;
    //THENS;
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

@lexer::namespace { TechTalk.SpecFlow.Parser.Grammar }
@namespace { TechTalk.SpecFlow.Parser.Grammar }

feature
    :   newlineWithSpaces?
        tags?
        WS? 'Feature:' WS? text newlineWithSpaces
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
    :   tags? WS? 'Scenario:' WS? 
        title newlineWithSpaces 
        steps
        -> ^(SCENARIO tags? title steps)
    ;

scenarioOutline
    :
        tags? WS? 'Scenario Outline:' WS?
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
    :   WS? ('Examples:'|'Scenarios:') WS?
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
    :   WS? 'And' WS sentenceEnd
        -> ^(AND sentenceEnd)
    ;

firstBut
    :   WS? 'But' WS sentenceEnd
        -> ^(BUT sentenceEnd)
    ;

givens
    :   firstGiven nextStep*
        -> ^(STEPS firstGiven nextStep*)
    ;
firstGiven
    :   WS? 'Given' WS sentenceEnd
        -> ^(GIVEN sentenceEnd)
    ;
/*nextGiven
    :   andSentence -> ^(GIVEN andSentence)
    |   firstGiven -> firstGiven
    ;

whens
    :   firstWhen nextWhen*
        -> ^(WHENS firstWhen nextWhen*)
    ;*/
firstWhen
    :   WS? 'When' WS sentenceEnd
        -> ^(WHEN sentenceEnd)
    ;
/*nextWhen
    :   andSentence -> ^(WHEN andSentence)
    |   firstWhen -> firstWhen
    ;

thens
    :   firstThen nextThen*
        -> ^(THENS firstThen nextThen*)
    ;*/
firstThen
    :   WS? 'Then' WS sentenceEnd
        -> ^(THEN sentenceEnd)
    ;
/*nextThen
    :   andSentence -> ^(THEN andSentence)
    |   butSentence -> ^(THEN butSentence)
    |   firstThen -> firstThen
    ;

andSentence
    :   WS? 'And' WS sentenceEnd 
        -> sentenceEnd
    ;

butSentence
    :   WS? 'But' WS sentenceEnd 
        -> sentenceEnd
    ;*/

sentenceEnd
    :   text newlineWithSpaces multilineText? table?
        -> text multilineText? table?
    ;

multilineText
    :   indent '"""' WS? NEWLINE
        multilineTextLine*
        WS? '"""' WS? newlineWithSpaces
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
    :   WS? '|' tableCell+ WS? newlineWithSpaces
        -> ^(ROW tableCell+)
    ;

tableCell
    :   WS? text WS? '|'
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

fragment WSCHAR     : (' '|'\t');
fragment NONWCHR    : (' '|'\t'|'\r'|'\n'|'#'|'@');
fragment NEWLINECHR : ('\r'|'\n');
fragment NONNLCHR   : ('\u0000'..'\t')|('\u000B'..'\f')|('\u000E'..'\uFFFF');


T_BACKGROUND : 'Background:';
AT          : '@';
/*COMMENT     : WSCHAR* '#' (~NEWLINECHR)* { $channel = Token.HIDDEN_CHANNEL; };*/
COMMENT     : WSCHAR* '#' NONNLCHR* { $channel = Token.HIDDEN_CHANNEL; };
WS          : WSCHAR+;
NEWLINE     : '\r\n' | '\n';
/*WORDCHAR    : ~NONWCHR; */
WORDCHAR    : (('\u0000'..'\b') 
    | ('\u000B'..'\f') 
    | ('\u000E'..'\u001F')
    | ('!'..'"') 
    | ('$'..'?')
    | ('A'..'\uFFFF'))+ ;
