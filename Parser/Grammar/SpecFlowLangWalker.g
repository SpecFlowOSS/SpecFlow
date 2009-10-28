tree grammar SpecFlowLangWalker;

options {
	language = CSharp2;
	tokenVocab = SpecFlowLang;
	ASTLabelType = CommonTree;
}

@namespace { TechTalk.SpecFlow.Parser.Grammar }
@header 
{
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;
}

feature returns[Feature feature]
@init {
    var scenarios = new List<Scenario>();
    var descLines = new List<DescriptionLine>();
}
@after {
    $feature = new Feature(title_, tags_, descLines.ToArray(), background_, scenarios.ToArray());
}
    :   ^(FEATURE
            (tags_=tags)?
            title_=text
            (descLine_=descriptionLine { descLines.Add(descLine_); })*
            (background_=background)?
            ^(SCENARIOS 
                (scenario_=scenarioKind { scenarios.Add(scenario_); } )* 
            )
        )
    ;

tags returns[Tags tags]
@init {
    $tags = new Tags();
}
    :   ^(TAGS
            (tag_=tag { $tags.Add(tag_); })+
        )
    ;

tag returns[Tag tag]
@after {
    $tag = new Tag(word_);
}
    :   ^(TAG
            word_=word
        )
    ;

word returns[Word word]
@init {
    var wordBuffer = new StringBuilder();
}
@after {
    $word = new Word(wordBuffer.ToString());
}
    :   ^(WORD
            (char_=WORDCHAR { wordBuffer.Append(char_.Text); })+
        )
    ;

descriptionLine returns[DescriptionLine descriptionLine]
@after {
    $descriptionLine = new DescriptionLine(text_);
}
    :   ^(DESCRIPTIONLINE
            text_=text
        )
    ;

background returns[Background background]
@after {
    $background = new Background(title_, steps_);
}
    :   ^(BACKGROUND
            (title_=text)?
            steps_=steps
        )
    ;

scenarioKind returns[Scenario scenarioKind]
    :   scenario_=scenario { $scenarioKind = scenario_; }
    |   outline_=scenarioOutline { $scenarioKind = outline_; }
    ;

scenarioOutline returns[ScenarioOutline outline]
@init {
    int? lineNo_ = null;
}
@after {
    $outline = new ScenarioOutline(title_, tags_, steps_, examples_);
    $outline.SourceFileLine = lineNo_;
}
    :   ^(SCENARIOOUTLINE
            tags_=tags?
            {
				lineNo_ = ((ITree)input.LT(1)).Line;
            }
            title_=text
            steps_=steps
            examples_=examples
        )
    ;

scenario returns[Scenario scenario]
@init {
    int? lineNo_ = null;
}
@after {
    $scenario = new Scenario(title_, tags_, steps_);
    $scenario.SourceFileLine = lineNo_;
}
    :   ^(SCENARIO 
            tags_=tags?
            {
				lineNo_ = ((ITree)input.LT(1)).Line;
            }
            title_=text
            steps_=steps
        )
    ;

examples returns[Examples examples]
@init {
    var exampleSets = new List<ExampleSet>();
}
@after {
    $examples = new Examples(exampleSets.ToArray());
}
    :   ^(EXAMPLES
            (exampleSet_=exampleSet { exampleSets.Add(exampleSet_); })+
        )
    ;

exampleSet returns[ExampleSet exampleSet]
@after {
    $exampleSet = new ExampleSet(title_, table_);
}
    :   ^(EXAMPLESET
            title_=text?
            table_=table
        )
    ;

steps returns[ScenarioSteps steps]
@init {
    $steps = new ScenarioSteps();
}
    :   ^(STEPS
            (step_=step { $steps.Add(step_); })+
        )
    ;

step returns[ScenarioStep step]
@init {
    int? lineNo_ = ((ITree)input.LT(1)).Line;
}
@after {
    $step.SourceFileLine = lineNo_;
}
    :   ^(GIVEN
            text_=text
            mlt_=multilineText?
            table_=table?
        )
        {
			$step = new Given(text_, mlt_, table_);
        }
    |   ^(WHEN
            text_=text
            mlt_=multilineText?
            table_=table?
        )
        {
			$step = new When(text_, mlt_, table_);
        }
    |   ^(THEN
            text_=text
            mlt_=multilineText?
            table_=table?
        )
        {
			$step = new Then(text_, mlt_, table_);
        }
    |   ^(AND
            text_=text
            mlt_=multilineText?
            table_=table?
        )
        {
			$step = new And(text_, mlt_, table_);
        }
    |   ^(BUT
            text_=text
            mlt_=multilineText?
            table_=table?
        )
        {
			$step = new But(text_, mlt_, table_);
        }
    ;


text returns[Text text]
@init {
    var elements = new List<string>();
}
@after {
    text = new Text(elements.ToArray());
}
    :   ^(TEXT 
            f=wordchar          { elements.Add(f); }
            (   ws=WS           { elements.Add(ws.Text); }
            |   wc=wordchar     { elements.Add(wc); }
            |   nl=NEWLINE      { elements.Add(nl.Text); }
            )*
        )
    ;

wordchar returns[string text]
    :   wc=WORDCHAR { $text = wc.Text; }
    |   at=AT       { @text = at.Text; }
    ;

multilineText returns[MultilineText multilineText]
@init {
    var lines = new StringBuilder();
}
@after {
    $multilineText = new MultilineText(lines.ToString(), indent_);
}
    :   ^(MULTILINETEXT
            (line_=line { lines.Append(line_); })*
            indent_=indent
        )   
    ;

line returns[string line]
@init {
    var buffer = new StringBuilder();
}
@after {
    $line = buffer.ToString();
}
    :   ^(LINE
            (ws=WS          { buffer.Append(ws.Text); })?
            (text_=text     { buffer.Append(text_.Value); })?
            nl=NEWLINE      { buffer.Append(nl.Text); }
        )
    ;

indent returns[string indent]
@init {
    $indent = "";
}
    :   ^(INDENT
            (ws=WS { $indent = ws.Text; } )?
        )
    ;

table returns[Table table]
@init {
    var bodyRows = new List<Row>();
}
@after {
    $table = new Table(header_, bodyRows.ToArray());
}
    :   ^(TABLE
            ^(HEADER header_=tableRow)
            ^(BODY (row_=tableRow { bodyRows.Add(row_); })+)
        )
    ;

tableRow returns[Row row]
@init {
    var cells = new List<Cell>();
}
@after {
    $row = new Row(cells.ToArray());
}
    :   ^(ROW
            (cell_=tableCell { cells.Add(cell_); })+
        )
    ;

tableCell returns[Cell cell]
@after {
    $cell = new Cell(text_);
}
    :   ^(CELL
            text_=text
        )
    ;
