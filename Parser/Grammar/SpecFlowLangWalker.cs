// $ANTLR 3.1.2 SpecFlowLangWalker.g 2009-10-16 10:52:13

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  TechTalk.SpecFlow.Parser.Grammar 
{

using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;


using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;


public partial class SpecFlowLangWalker : TreeParser
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"FEATURE", 
		"DESCRIPTIONLINE", 
		"BACKGROUND", 
		"SCENARIOS", 
		"SCENARIO", 
		"SCENARIOOUTLINE", 
		"EXAMPLES", 
		"EXAMPLESET", 
		"STEPS", 
		"GIVEN", 
		"WHEN", 
		"THEN", 
		"TEXT", 
		"AND", 
		"BUT", 
		"TAGS", 
		"TAG", 
		"WORD", 
		"MULTILINETEXT", 
		"INDENT", 
		"LINE", 
		"TABLE", 
		"HEADER", 
		"BODY", 
		"ROW", 
		"CELL", 
		"WS", 
		"AT", 
		"WORDCHAR", 
		"T_BACKGROUND", 
		"NEWLINE", 
		"WSCHAR", 
		"NONWCHR", 
		"NEWLINECHR", 
		"NONNLCHR", 
		"COMMENT", 
		"'Feature:'", 
		"'Scenario:'", 
		"'Scenario Outline:'", 
		"'Examples:'", 
		"'Scenarios:'", 
		"'And'", 
		"'But'", 
		"'Given'", 
		"'When'", 
		"'Then'", 
		"'\"\"\"'", 
		"'|'"
    };

    public const int NEWLINECHR = 37;
    public const int ROW = 28;
    public const int T_BACKGROUND = 33;
    public const int TABLE = 25;
    public const int CELL = 29;
    public const int DESCRIPTIONLINE = 5;
    public const int AND = 17;
    public const int EOF = -1;
    public const int INDENT = 23;
    public const int AT = 31;
    public const int WORD = 21;
    public const int T__51 = 51;
    public const int BACKGROUND = 6;
    public const int MULTILINETEXT = 22;
    public const int THEN = 15;
    public const int NONWCHR = 36;
    public const int BODY = 27;
    public const int GIVEN = 13;
    public const int HEADER = 26;
    public const int COMMENT = 39;
    public const int SCENARIO = 8;
    public const int T__50 = 50;
    public const int T__42 = 42;
    public const int T__43 = 43;
    public const int T__40 = 40;
    public const int T__41 = 41;
    public const int T__46 = 46;
    public const int T__47 = 47;
    public const int T__44 = 44;
    public const int T__45 = 45;
    public const int EXAMPLESET = 11;
    public const int T__48 = 48;
    public const int T__49 = 49;
    public const int BUT = 18;
    public const int TAGS = 19;
    public const int EXAMPLES = 10;
    public const int WSCHAR = 35;
    public const int TEXT = 16;
    public const int NONNLCHR = 38;
    public const int LINE = 24;
    public const int FEATURE = 4;
    public const int TAG = 20;
    public const int SCENARIOS = 7;
    public const int WORDCHAR = 32;
    public const int WS = 30;
    public const int NEWLINE = 34;
    public const int SCENARIOOUTLINE = 9;
    public const int WHEN = 14;
    public const int STEPS = 12;

    // delegates
    // delegators



        public SpecFlowLangWalker(ITreeNodeStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public SpecFlowLangWalker(ITreeNodeStream input, RecognizerSharedState state)
    		: base(input, state) {
            InitializeCyclicDFAs();

             
        }
        

    override public string[] TokenNames {
		get { return SpecFlowLangWalker.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "SpecFlowLangWalker.g"; }
    }



    // $ANTLR start "feature"
    // SpecFlowLangWalker.g:17:1: feature returns [Feature feature] : ^( FEATURE (tags_= tags )? title_= text (descLine_= descriptionLine )* (background_= background )? ^( SCENARIOS (scenario_= scenarioKind )* ) ) ;
    public Feature feature() // throws RecognitionException [1]
    {   
        Feature feature = default(Feature);

        Tags tags_ = default(Tags);

        Text title_ = default(Text);

        DescriptionLine descLine_ = default(DescriptionLine);

        Background background_ = default(Background);

        Scenario scenario_ = default(Scenario);



            var scenarios = new List<Scenario>();
            var descLines = new List<DescriptionLine>();

        try 
    	{
            // SpecFlowLangWalker.g:25:5: ( ^( FEATURE (tags_= tags )? title_= text (descLine_= descriptionLine )* (background_= background )? ^( SCENARIOS (scenario_= scenarioKind )* ) ) )
            // SpecFlowLangWalker.g:25:9: ^( FEATURE (tags_= tags )? title_= text (descLine_= descriptionLine )* (background_= background )? ^( SCENARIOS (scenario_= scenarioKind )* ) )
            {
            	Match(input,FEATURE,FOLLOW_FEATURE_in_feature73); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:26:13: (tags_= tags )?
            	int alt1 = 2;
            	int LA1_0 = input.LA(1);

            	if ( (LA1_0 == TAGS) )
            	{
            	    alt1 = 1;
            	}
            	switch (alt1) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:26:14: tags_= tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_feature90);
            	        	tags_ = tags();
            	        	state.followingStackPointer--;


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_feature108);
            	title_ = text();
            	state.followingStackPointer--;

            	// SpecFlowLangWalker.g:28:13: (descLine_= descriptionLine )*
            	do 
            	{
            	    int alt2 = 2;
            	    int LA2_0 = input.LA(1);

            	    if ( (LA2_0 == DESCRIPTIONLINE) )
            	    {
            	        alt2 = 1;
            	    }


            	    switch (alt2) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:28:14: descLine_= descriptionLine
            			    {
            			    	PushFollow(FOLLOW_descriptionLine_in_feature125);
            			    	descLine_ = descriptionLine();
            			    	state.followingStackPointer--;

            			    	 descLines.Add(descLine_); 

            			    }
            			    break;

            			default:
            			    goto loop2;
            	    }
            	} while (true);

            	loop2:
            		;	// Stops C# compiler whining that label 'loop2' has no statements

            	// SpecFlowLangWalker.g:29:13: (background_= background )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == BACKGROUND) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:29:14: background_= background
            	        {
            	        	PushFollow(FOLLOW_background_in_feature146);
            	        	background_ = background();
            	        	state.followingStackPointer--;


            	        }
            	        break;

            	}

            	Match(input,SCENARIOS,FOLLOW_SCENARIOS_in_feature163); 

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); 
            	    // SpecFlowLangWalker.g:31:17: (scenario_= scenarioKind )*
            	    do 
            	    {
            	        int alt4 = 2;
            	        int LA4_0 = input.LA(1);

            	        if ( ((LA4_0 >= SCENARIO && LA4_0 <= SCENARIOOUTLINE)) )
            	        {
            	            alt4 = 1;
            	        }


            	        switch (alt4) 
            	    	{
            	    		case 1 :
            	    		    // SpecFlowLangWalker.g:31:18: scenario_= scenarioKind
            	    		    {
            	    		    	PushFollow(FOLLOW_scenarioKind_in_feature185);
            	    		    	scenario_ = scenarioKind();
            	    		    	state.followingStackPointer--;

            	    		    	 scenarios.Add(scenario_); 

            	    		    }
            	    		    break;

            	    		default:
            	    		    goto loop4;
            	        }
            	    } while (true);

            	    loop4:
            	    	;	// Stops C# compiler whining that label 'loop4' has no statements


            	    Match(input, Token.UP, null); 
            	}

            	Match(input, Token.UP, null); 

            }


                feature =  new Feature(title_, tags_, descLines.ToArray(), background_, scenarios.ToArray());

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return feature;
    }
    // $ANTLR end "feature"


    // $ANTLR start "tags"
    // SpecFlowLangWalker.g:36:1: tags returns [Tags tags] : ^( TAGS (tag_= tag )+ ) ;
    public Tags tags() // throws RecognitionException [1]
    {   
        Tags tags = default(Tags);

        Tag tag_ = default(Tag);



            tags =  new Tags();

        try 
    	{
            // SpecFlowLangWalker.g:40:5: ( ^( TAGS (tag_= tag )+ ) )
            // SpecFlowLangWalker.g:40:9: ^( TAGS (tag_= tag )+ )
            {
            	Match(input,TAGS,FOLLOW_TAGS_in_tags243); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:41:13: (tag_= tag )+
            	int cnt5 = 0;
            	do 
            	{
            	    int alt5 = 2;
            	    int LA5_0 = input.LA(1);

            	    if ( (LA5_0 == TAG) )
            	    {
            	        alt5 = 1;
            	    }


            	    switch (alt5) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:41:14: tag_= tag
            			    {
            			    	PushFollow(FOLLOW_tag_in_tags260);
            			    	tag_ = tag();
            			    	state.followingStackPointer--;

            			    	 tags.Add(tag_); 

            			    }
            			    break;

            			default:
            			    if ( cnt5 >= 1 ) goto loop5;
            		            EarlyExitException eee5 =
            		                new EarlyExitException(5, input);
            		            throw eee5;
            	    }
            	    cnt5++;
            	} while (true);

            	loop5:
            		;	// Stops C# compiler whinging that label 'loop5' has no statements


            	Match(input, Token.UP, null); 

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return tags;
    }
    // $ANTLR end "tags"


    // $ANTLR start "tag"
    // SpecFlowLangWalker.g:45:1: tag returns [Tag tag] : ^( TAG word_= word ) ;
    public Tag tag() // throws RecognitionException [1]
    {   
        Tag tag = default(Tag);

        Word word_ = default(Word);


        try 
    	{
            // SpecFlowLangWalker.g:49:5: ( ^( TAG word_= word ) )
            // SpecFlowLangWalker.g:49:9: ^( TAG word_= word )
            {
            	Match(input,TAG,FOLLOW_TAG_in_tag302); 

            	Match(input, Token.DOWN, null); 
            	PushFollow(FOLLOW_word_in_tag318);
            	word_ = word();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                tag =  new Tag(word_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return tag;
    }
    // $ANTLR end "tag"


    // $ANTLR start "word"
    // SpecFlowLangWalker.g:54:1: word returns [Word word] : ^( WORD (char_= WORDCHAR )+ ) ;
    public Word word() // throws RecognitionException [1]
    {   
        Word word = default(Word);

        CommonTree char_ = null;


            var wordBuffer = new StringBuilder();

        try 
    	{
            // SpecFlowLangWalker.g:61:5: ( ^( WORD (char_= WORDCHAR )+ ) )
            // SpecFlowLangWalker.g:61:9: ^( WORD (char_= WORDCHAR )+ )
            {
            	Match(input,WORD,FOLLOW_WORD_in_word361); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:62:13: (char_= WORDCHAR )+
            	int cnt6 = 0;
            	do 
            	{
            	    int alt6 = 2;
            	    int LA6_0 = input.LA(1);

            	    if ( (LA6_0 == WORDCHAR) )
            	    {
            	        alt6 = 1;
            	    }


            	    switch (alt6) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:62:14: char_= WORDCHAR
            			    {
            			    	char_=(CommonTree)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_word378); 
            			    	 wordBuffer.Append(char_.Text); 

            			    }
            			    break;

            			default:
            			    if ( cnt6 >= 1 ) goto loop6;
            		            EarlyExitException eee6 =
            		                new EarlyExitException(6, input);
            		            throw eee6;
            	    }
            	    cnt6++;
            	} while (true);

            	loop6:
            		;	// Stops C# compiler whinging that label 'loop6' has no statements


            	Match(input, Token.UP, null); 

            }


                word =  new Word(wordBuffer.ToString());

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return word;
    }
    // $ANTLR end "word"


    // $ANTLR start "descriptionLine"
    // SpecFlowLangWalker.g:66:1: descriptionLine returns [DescriptionLine descriptionLine] : ^( DESCRIPTIONLINE text_= text ) ;
    public DescriptionLine descriptionLine() // throws RecognitionException [1]
    {   
        DescriptionLine descriptionLine = default(DescriptionLine);

        Text text_ = default(Text);


        try 
    	{
            // SpecFlowLangWalker.g:70:5: ( ^( DESCRIPTIONLINE text_= text ) )
            // SpecFlowLangWalker.g:70:9: ^( DESCRIPTIONLINE text_= text )
            {
            	Match(input,DESCRIPTIONLINE,FOLLOW_DESCRIPTIONLINE_in_descriptionLine420); 

            	Match(input, Token.DOWN, null); 
            	PushFollow(FOLLOW_text_in_descriptionLine436);
            	text_ = text();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                descriptionLine =  new DescriptionLine(text_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return descriptionLine;
    }
    // $ANTLR end "descriptionLine"


    // $ANTLR start "background"
    // SpecFlowLangWalker.g:75:1: background returns [Background background] : ^( BACKGROUND (title_= text )? steps_= steps ) ;
    public Background background() // throws RecognitionException [1]
    {   
        Background background = default(Background);

        Text title_ = default(Text);

        ScenarioSteps steps_ = default(ScenarioSteps);


        try 
    	{
            // SpecFlowLangWalker.g:79:5: ( ^( BACKGROUND (title_= text )? steps_= steps ) )
            // SpecFlowLangWalker.g:79:9: ^( BACKGROUND (title_= text )? steps_= steps )
            {
            	Match(input,BACKGROUND,FOLLOW_BACKGROUND_in_background474); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:80:13: (title_= text )?
            	int alt7 = 2;
            	int LA7_0 = input.LA(1);

            	if ( (LA7_0 == TEXT) )
            	{
            	    alt7 = 1;
            	}
            	switch (alt7) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:80:14: title_= text
            	        {
            	        	PushFollow(FOLLOW_text_in_background491);
            	        	title_ = text();
            	        	state.followingStackPointer--;


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_steps_in_background509);
            	steps_ = steps();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                background =  new Background(title_, steps_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return background;
    }
    // $ANTLR end "background"


    // $ANTLR start "scenarioKind"
    // SpecFlowLangWalker.g:85:1: scenarioKind returns [Scenario scenarioKind] : (scenario_= scenario | outline_= scenarioOutline );
    public Scenario scenarioKind() // throws RecognitionException [1]
    {   
        Scenario scenarioKind = default(Scenario);

        Scenario scenario_ = default(Scenario);

        ScenarioOutline outline_ = default(ScenarioOutline);


        try 
    	{
            // SpecFlowLangWalker.g:86:5: (scenario_= scenario | outline_= scenarioOutline )
            int alt8 = 2;
            int LA8_0 = input.LA(1);

            if ( (LA8_0 == SCENARIO) )
            {
                alt8 = 1;
            }
            else if ( (LA8_0 == SCENARIOOUTLINE) )
            {
                alt8 = 2;
            }
            else 
            {
                NoViableAltException nvae_d8s0 =
                    new NoViableAltException("", 8, 0, input);

                throw nvae_d8s0;
            }
            switch (alt8) 
            {
                case 1 :
                    // SpecFlowLangWalker.g:86:9: scenario_= scenario
                    {
                    	PushFollow(FOLLOW_scenario_in_scenarioKind543);
                    	scenario_ = scenario();
                    	state.followingStackPointer--;

                    	 scenarioKind =  scenario_; 

                    }
                    break;
                case 2 :
                    // SpecFlowLangWalker.g:87:9: outline_= scenarioOutline
                    {
                    	PushFollow(FOLLOW_scenarioOutline_in_scenarioKind557);
                    	outline_ = scenarioOutline();
                    	state.followingStackPointer--;

                    	 scenarioKind =  outline_; 

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return scenarioKind;
    }
    // $ANTLR end "scenarioKind"


    // $ANTLR start "scenarioOutline"
    // SpecFlowLangWalker.g:90:1: scenarioOutline returns [ScenarioOutline outline] : ^( SCENARIOOUTLINE (tags_= tags )? title_= text steps_= steps examples_= examples ) ;
    public ScenarioOutline scenarioOutline() // throws RecognitionException [1]
    {   
        ScenarioOutline outline = default(ScenarioOutline);

        Tags tags_ = default(Tags);

        Text title_ = default(Text);

        ScenarioSteps steps_ = default(ScenarioSteps);

        Examples examples_ = default(Examples);


        try 
    	{
            // SpecFlowLangWalker.g:94:5: ( ^( SCENARIOOUTLINE (tags_= tags )? title_= text steps_= steps examples_= examples ) )
            // SpecFlowLangWalker.g:94:9: ^( SCENARIOOUTLINE (tags_= tags )? title_= text steps_= steps examples_= examples )
            {
            	Match(input,SCENARIOOUTLINE,FOLLOW_SCENARIOOUTLINE_in_scenarioOutline587); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:95:18: (tags_= tags )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == TAGS) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:95:18: tags_= tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_scenarioOutline603);
            	        	tags_ = tags();
            	        	state.followingStackPointer--;


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_scenarioOutline620);
            	title_ = text();
            	state.followingStackPointer--;

            	PushFollow(FOLLOW_steps_in_scenarioOutline636);
            	steps_ = steps();
            	state.followingStackPointer--;

            	PushFollow(FOLLOW_examples_in_scenarioOutline652);
            	examples_ = examples();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                outline =  new ScenarioOutline(title_, tags_, steps_, examples_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return outline;
    }
    // $ANTLR end "scenarioOutline"


    // $ANTLR start "scenario"
    // SpecFlowLangWalker.g:102:1: scenario returns [Scenario scenario] : ^( SCENARIO (tags_= tags )? title_= text steps_= steps ) ;
    public Scenario scenario() // throws RecognitionException [1]
    {   
        Scenario scenario = default(Scenario);

        Tags tags_ = default(Tags);

        Text title_ = default(Text);

        ScenarioSteps steps_ = default(ScenarioSteps);


        try 
    	{
            // SpecFlowLangWalker.g:106:5: ( ^( SCENARIO (tags_= tags )? title_= text steps_= steps ) )
            // SpecFlowLangWalker.g:106:9: ^( SCENARIO (tags_= tags )? title_= text steps_= steps )
            {
            	Match(input,SCENARIO,FOLLOW_SCENARIO_in_scenario690); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:107:18: (tags_= tags )?
            	int alt10 = 2;
            	int LA10_0 = input.LA(1);

            	if ( (LA10_0 == TAGS) )
            	{
            	    alt10 = 1;
            	}
            	switch (alt10) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:107:18: tags_= tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_scenario707);
            	        	tags_ = tags();
            	        	state.followingStackPointer--;


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_scenario724);
            	title_ = text();
            	state.followingStackPointer--;

            	PushFollow(FOLLOW_steps_in_scenario740);
            	steps_ = steps();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                scenario =  new Scenario(title_, tags_, steps_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return scenario;
    }
    // $ANTLR end "scenario"


    // $ANTLR start "examples"
    // SpecFlowLangWalker.g:113:1: examples returns [Examples examples] : ^( EXAMPLES (exampleSet_= exampleSet )+ ) ;
    public Examples examples() // throws RecognitionException [1]
    {   
        Examples examples = default(Examples);

        ExampleSet exampleSet_ = default(ExampleSet);



            var exampleSets = new List<ExampleSet>();

        try 
    	{
            // SpecFlowLangWalker.g:120:5: ( ^( EXAMPLES (exampleSet_= exampleSet )+ ) )
            // SpecFlowLangWalker.g:120:9: ^( EXAMPLES (exampleSet_= exampleSet )+ )
            {
            	Match(input,EXAMPLES,FOLLOW_EXAMPLES_in_examples783); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:121:13: (exampleSet_= exampleSet )+
            	int cnt11 = 0;
            	do 
            	{
            	    int alt11 = 2;
            	    int LA11_0 = input.LA(1);

            	    if ( (LA11_0 == EXAMPLESET) )
            	    {
            	        alt11 = 1;
            	    }


            	    switch (alt11) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:121:14: exampleSet_= exampleSet
            			    {
            			    	PushFollow(FOLLOW_exampleSet_in_examples800);
            			    	exampleSet_ = exampleSet();
            			    	state.followingStackPointer--;

            			    	 exampleSets.Add(exampleSet_); 

            			    }
            			    break;

            			default:
            			    if ( cnt11 >= 1 ) goto loop11;
            		            EarlyExitException eee11 =
            		                new EarlyExitException(11, input);
            		            throw eee11;
            	    }
            	    cnt11++;
            	} while (true);

            	loop11:
            		;	// Stops C# compiler whinging that label 'loop11' has no statements


            	Match(input, Token.UP, null); 

            }


                examples =  new Examples(exampleSets.ToArray());

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return examples;
    }
    // $ANTLR end "examples"


    // $ANTLR start "exampleSet"
    // SpecFlowLangWalker.g:125:1: exampleSet returns [ExampleSet exampleSet] : ^( EXAMPLESET (title_= text )? table_= table ) ;
    public ExampleSet exampleSet() // throws RecognitionException [1]
    {   
        ExampleSet exampleSet = default(ExampleSet);

        Text title_ = default(Text);

        Table table_ = default(Table);


        try 
    	{
            // SpecFlowLangWalker.g:129:5: ( ^( EXAMPLESET (title_= text )? table_= table ) )
            // SpecFlowLangWalker.g:129:9: ^( EXAMPLESET (title_= text )? table_= table )
            {
            	Match(input,EXAMPLESET,FOLLOW_EXAMPLESET_in_exampleSet842); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:130:19: (title_= text )?
            	int alt12 = 2;
            	int LA12_0 = input.LA(1);

            	if ( (LA12_0 == TEXT) )
            	{
            	    alt12 = 1;
            	}
            	switch (alt12) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:130:19: title_= text
            	        {
            	        	PushFollow(FOLLOW_text_in_exampleSet858);
            	        	title_ = text();
            	        	state.followingStackPointer--;


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_table_in_exampleSet875);
            	table_ = table();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                exampleSet =  new ExampleSet(title_, table_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return exampleSet;
    }
    // $ANTLR end "exampleSet"


    // $ANTLR start "steps"
    // SpecFlowLangWalker.g:135:1: steps returns [ScenarioSteps steps] : ^( STEPS (step_= step )+ ) ;
    public ScenarioSteps steps() // throws RecognitionException [1]
    {   
        ScenarioSteps steps = default(ScenarioSteps);

        ScenarioStep step_ = default(ScenarioStep);



            steps =  new ScenarioSteps();

        try 
    	{
            // SpecFlowLangWalker.g:139:5: ( ^( STEPS (step_= step )+ ) )
            // SpecFlowLangWalker.g:139:9: ^( STEPS (step_= step )+ )
            {
            	Match(input,STEPS,FOLLOW_STEPS_in_steps913); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:140:13: (step_= step )+
            	int cnt13 = 0;
            	do 
            	{
            	    int alt13 = 2;
            	    int LA13_0 = input.LA(1);

            	    if ( ((LA13_0 >= GIVEN && LA13_0 <= THEN) || (LA13_0 >= AND && LA13_0 <= BUT)) )
            	    {
            	        alt13 = 1;
            	    }


            	    switch (alt13) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:140:14: step_= step
            			    {
            			    	PushFollow(FOLLOW_step_in_steps930);
            			    	step_ = step();
            			    	state.followingStackPointer--;

            			    	 steps.Add(step_); 

            			    }
            			    break;

            			default:
            			    if ( cnt13 >= 1 ) goto loop13;
            		            EarlyExitException eee13 =
            		                new EarlyExitException(13, input);
            		            throw eee13;
            	    }
            	    cnt13++;
            	} while (true);

            	loop13:
            		;	// Stops C# compiler whinging that label 'loop13' has no statements


            	Match(input, Token.UP, null); 

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return steps;
    }
    // $ANTLR end "steps"


    // $ANTLR start "step"
    // SpecFlowLangWalker.g:144:1: step returns [ScenarioStep step] : ( ^( GIVEN text_= text (mlt_= multilineText )? (table_= table )? ) | ^( WHEN text_= text (mlt_= multilineText )? (table_= table )? ) | ^( THEN text_= text (mlt_= multilineText )? (table_= table )? ) | ^( AND text_= text (mlt_= multilineText )? (table_= table )? ) | ^( BUT text_= text (mlt_= multilineText )? (table_= table )? ) );
    public ScenarioStep step() // throws RecognitionException [1]
    {   
        ScenarioStep step = default(ScenarioStep);

        Text text_ = default(Text);

        MultilineText mlt_ = default(MultilineText);

        Table table_ = default(Table);


        try 
    	{
            // SpecFlowLangWalker.g:145:5: ( ^( GIVEN text_= text (mlt_= multilineText )? (table_= table )? ) | ^( WHEN text_= text (mlt_= multilineText )? (table_= table )? ) | ^( THEN text_= text (mlt_= multilineText )? (table_= table )? ) | ^( AND text_= text (mlt_= multilineText )? (table_= table )? ) | ^( BUT text_= text (mlt_= multilineText )? (table_= table )? ) )
            int alt24 = 5;
            switch ( input.LA(1) ) 
            {
            case GIVEN:
            	{
                alt24 = 1;
                }
                break;
            case WHEN:
            	{
                alt24 = 2;
                }
                break;
            case THEN:
            	{
                alt24 = 3;
                }
                break;
            case AND:
            	{
                alt24 = 4;
                }
                break;
            case BUT:
            	{
                alt24 = 5;
                }
                break;
            	default:
            	    NoViableAltException nvae_d24s0 =
            	        new NoViableAltException("", 24, 0, input);

            	    throw nvae_d24s0;
            }

            switch (alt24) 
            {
                case 1 :
                    // SpecFlowLangWalker.g:145:9: ^( GIVEN text_= text (mlt_= multilineText )? (table_= table )? )
                    {
                    	Match(input,GIVEN,FOLLOW_GIVEN_in_step967); 

                    	Match(input, Token.DOWN, null); 
                    	PushFollow(FOLLOW_text_in_step983);
                    	text_ = text();
                    	state.followingStackPointer--;

                    	// SpecFlowLangWalker.g:147:17: (mlt_= multilineText )?
                    	int alt14 = 2;
                    	int LA14_0 = input.LA(1);

                    	if ( (LA14_0 == MULTILINETEXT) )
                    	{
                    	    alt14 = 1;
                    	}
                    	switch (alt14) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:147:17: mlt_= multilineText
                    	        {
                    	        	PushFollow(FOLLOW_multilineText_in_step999);
                    	        	mlt_ = multilineText();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}

                    	// SpecFlowLangWalker.g:148:19: (table_= table )?
                    	int alt15 = 2;
                    	int LA15_0 = input.LA(1);

                    	if ( (LA15_0 == TABLE) )
                    	{
                    	    alt15 = 1;
                    	}
                    	switch (alt15) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:148:19: table_= table
                    	        {
                    	        	PushFollow(FOLLOW_table_in_step1016);
                    	        	table_ = table();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); 

                    				step =  new Given(text_, mlt_, table_);
                    	        

                    }
                    break;
                case 2 :
                    // SpecFlowLangWalker.g:153:9: ^( WHEN text_= text (mlt_= multilineText )? (table_= table )? )
                    {
                    	Match(input,WHEN,FOLLOW_WHEN_in_step1048); 

                    	Match(input, Token.DOWN, null); 
                    	PushFollow(FOLLOW_text_in_step1064);
                    	text_ = text();
                    	state.followingStackPointer--;

                    	// SpecFlowLangWalker.g:155:17: (mlt_= multilineText )?
                    	int alt16 = 2;
                    	int LA16_0 = input.LA(1);

                    	if ( (LA16_0 == MULTILINETEXT) )
                    	{
                    	    alt16 = 1;
                    	}
                    	switch (alt16) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:155:17: mlt_= multilineText
                    	        {
                    	        	PushFollow(FOLLOW_multilineText_in_step1080);
                    	        	mlt_ = multilineText();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}

                    	// SpecFlowLangWalker.g:156:19: (table_= table )?
                    	int alt17 = 2;
                    	int LA17_0 = input.LA(1);

                    	if ( (LA17_0 == TABLE) )
                    	{
                    	    alt17 = 1;
                    	}
                    	switch (alt17) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:156:19: table_= table
                    	        {
                    	        	PushFollow(FOLLOW_table_in_step1097);
                    	        	table_ = table();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); 

                    				step =  new When(text_, mlt_, table_);
                    	        

                    }
                    break;
                case 3 :
                    // SpecFlowLangWalker.g:161:9: ^( THEN text_= text (mlt_= multilineText )? (table_= table )? )
                    {
                    	Match(input,THEN,FOLLOW_THEN_in_step1129); 

                    	Match(input, Token.DOWN, null); 
                    	PushFollow(FOLLOW_text_in_step1145);
                    	text_ = text();
                    	state.followingStackPointer--;

                    	// SpecFlowLangWalker.g:163:17: (mlt_= multilineText )?
                    	int alt18 = 2;
                    	int LA18_0 = input.LA(1);

                    	if ( (LA18_0 == MULTILINETEXT) )
                    	{
                    	    alt18 = 1;
                    	}
                    	switch (alt18) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:163:17: mlt_= multilineText
                    	        {
                    	        	PushFollow(FOLLOW_multilineText_in_step1161);
                    	        	mlt_ = multilineText();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}

                    	// SpecFlowLangWalker.g:164:19: (table_= table )?
                    	int alt19 = 2;
                    	int LA19_0 = input.LA(1);

                    	if ( (LA19_0 == TABLE) )
                    	{
                    	    alt19 = 1;
                    	}
                    	switch (alt19) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:164:19: table_= table
                    	        {
                    	        	PushFollow(FOLLOW_table_in_step1178);
                    	        	table_ = table();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); 

                    				step =  new Then(text_, mlt_, table_);
                    	        

                    }
                    break;
                case 4 :
                    // SpecFlowLangWalker.g:169:9: ^( AND text_= text (mlt_= multilineText )? (table_= table )? )
                    {
                    	Match(input,AND,FOLLOW_AND_in_step1210); 

                    	Match(input, Token.DOWN, null); 
                    	PushFollow(FOLLOW_text_in_step1226);
                    	text_ = text();
                    	state.followingStackPointer--;

                    	// SpecFlowLangWalker.g:171:17: (mlt_= multilineText )?
                    	int alt20 = 2;
                    	int LA20_0 = input.LA(1);

                    	if ( (LA20_0 == MULTILINETEXT) )
                    	{
                    	    alt20 = 1;
                    	}
                    	switch (alt20) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:171:17: mlt_= multilineText
                    	        {
                    	        	PushFollow(FOLLOW_multilineText_in_step1242);
                    	        	mlt_ = multilineText();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}

                    	// SpecFlowLangWalker.g:172:19: (table_= table )?
                    	int alt21 = 2;
                    	int LA21_0 = input.LA(1);

                    	if ( (LA21_0 == TABLE) )
                    	{
                    	    alt21 = 1;
                    	}
                    	switch (alt21) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:172:19: table_= table
                    	        {
                    	        	PushFollow(FOLLOW_table_in_step1259);
                    	        	table_ = table();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); 

                    				step =  new And(text_, mlt_, table_);
                    	        

                    }
                    break;
                case 5 :
                    // SpecFlowLangWalker.g:177:9: ^( BUT text_= text (mlt_= multilineText )? (table_= table )? )
                    {
                    	Match(input,BUT,FOLLOW_BUT_in_step1291); 

                    	Match(input, Token.DOWN, null); 
                    	PushFollow(FOLLOW_text_in_step1307);
                    	text_ = text();
                    	state.followingStackPointer--;

                    	// SpecFlowLangWalker.g:179:17: (mlt_= multilineText )?
                    	int alt22 = 2;
                    	int LA22_0 = input.LA(1);

                    	if ( (LA22_0 == MULTILINETEXT) )
                    	{
                    	    alt22 = 1;
                    	}
                    	switch (alt22) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:179:17: mlt_= multilineText
                    	        {
                    	        	PushFollow(FOLLOW_multilineText_in_step1323);
                    	        	mlt_ = multilineText();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}

                    	// SpecFlowLangWalker.g:180:19: (table_= table )?
                    	int alt23 = 2;
                    	int LA23_0 = input.LA(1);

                    	if ( (LA23_0 == TABLE) )
                    	{
                    	    alt23 = 1;
                    	}
                    	switch (alt23) 
                    	{
                    	    case 1 :
                    	        // SpecFlowLangWalker.g:180:19: table_= table
                    	        {
                    	        	PushFollow(FOLLOW_table_in_step1340);
                    	        	table_ = table();
                    	        	state.followingStackPointer--;


                    	        }
                    	        break;

                    	}


                    	Match(input, Token.UP, null); 

                    				step =  new But(text_, mlt_, table_);
                    	        

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return step;
    }
    // $ANTLR end "step"


    // $ANTLR start "text"
    // SpecFlowLangWalker.g:188:1: text returns [Text text] : ^( TEXT f= wordchar (ws= WS | wc= wordchar | nl= NEWLINE )* ) ;
    public Text text() // throws RecognitionException [1]
    {   
        Text text = default(Text);

        CommonTree ws = null;
        CommonTree nl = null;
        string f = default(string);

        string wc = default(string);



            var elements = new List<string>();

        try 
    	{
            // SpecFlowLangWalker.g:195:5: ( ^( TEXT f= wordchar (ws= WS | wc= wordchar | nl= NEWLINE )* ) )
            // SpecFlowLangWalker.g:195:9: ^( TEXT f= wordchar (ws= WS | wc= wordchar | nl= NEWLINE )* )
            {
            	Match(input,TEXT,FOLLOW_TEXT_in_text1395); 

            	Match(input, Token.DOWN, null); 
            	PushFollow(FOLLOW_wordchar_in_text1412);
            	f = wordchar();
            	state.followingStackPointer--;

            	 elements.Add(f); 
            	// SpecFlowLangWalker.g:197:13: (ws= WS | wc= wordchar | nl= NEWLINE )*
            	do 
            	{
            	    int alt25 = 4;
            	    switch ( input.LA(1) ) 
            	    {
            	    case WS:
            	    	{
            	        alt25 = 1;
            	        }
            	        break;
            	    case AT:
            	    case WORDCHAR:
            	    	{
            	        alt25 = 2;
            	        }
            	        break;
            	    case NEWLINE:
            	    	{
            	        alt25 = 3;
            	        }
            	        break;

            	    }

            	    switch (alt25) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:197:17: ws= WS
            			    {
            			    	ws=(CommonTree)Match(input,WS,FOLLOW_WS_in_text1443); 
            			    	 elements.Add(ws.Text); 

            			    }
            			    break;
            			case 2 :
            			    // SpecFlowLangWalker.g:198:17: wc= wordchar
            			    {
            			    	PushFollow(FOLLOW_wordchar_in_text1475);
            			    	wc = wordchar();
            			    	state.followingStackPointer--;

            			    	 elements.Add(wc); 

            			    }
            			    break;
            			case 3 :
            			    // SpecFlowLangWalker.g:199:17: nl= NEWLINE
            			    {
            			    	nl=(CommonTree)Match(input,NEWLINE,FOLLOW_NEWLINE_in_text1501); 
            			    	 elements.Add(nl.Text); 

            			    }
            			    break;

            			default:
            			    goto loop25;
            	    }
            	} while (true);

            	loop25:
            		;	// Stops C# compiler whining that label 'loop25' has no statements


            	Match(input, Token.UP, null); 

            }


                text = new Text(elements.ToArray());

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return text;
    }
    // $ANTLR end "text"


    // $ANTLR start "wordchar"
    // SpecFlowLangWalker.g:204:1: wordchar returns [string text] : (wc= WORDCHAR | at= AT );
    public string wordchar() // throws RecognitionException [1]
    {   
        string text = default(string);

        CommonTree wc = null;
        CommonTree at = null;

        try 
    	{
            // SpecFlowLangWalker.g:205:5: (wc= WORDCHAR | at= AT )
            int alt26 = 2;
            int LA26_0 = input.LA(1);

            if ( (LA26_0 == WORDCHAR) )
            {
                alt26 = 1;
            }
            else if ( (LA26_0 == AT) )
            {
                alt26 = 2;
            }
            else 
            {
                NoViableAltException nvae_d26s0 =
                    new NoViableAltException("", 26, 0, input);

                throw nvae_d26s0;
            }
            switch (alt26) 
            {
                case 1 :
                    // SpecFlowLangWalker.g:205:9: wc= WORDCHAR
                    {
                    	wc=(CommonTree)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_wordchar1557); 
                    	 text =  wc.Text; 

                    }
                    break;
                case 2 :
                    // SpecFlowLangWalker.g:206:9: at= AT
                    {
                    	at=(CommonTree)Match(input,AT,FOLLOW_AT_in_wordchar1571); 
                    	 @text = at.Text; 

                    }
                    break;

            }
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return text;
    }
    // $ANTLR end "wordchar"


    // $ANTLR start "multilineText"
    // SpecFlowLangWalker.g:209:1: multilineText returns [MultilineText multilineText] : ^( MULTILINETEXT (line_= line )* indent_= indent ) ;
    public MultilineText multilineText() // throws RecognitionException [1]
    {   
        MultilineText multilineText = default(MultilineText);

        string line_ = default(string);

        string indent_ = default(string);



            var lines = new StringBuilder();

        try 
    	{
            // SpecFlowLangWalker.g:216:5: ( ^( MULTILINETEXT (line_= line )* indent_= indent ) )
            // SpecFlowLangWalker.g:216:9: ^( MULTILINETEXT (line_= line )* indent_= indent )
            {
            	Match(input,MULTILINETEXT,FOLLOW_MULTILINETEXT_in_multilineText1612); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:217:13: (line_= line )*
            	do 
            	{
            	    int alt27 = 2;
            	    int LA27_0 = input.LA(1);

            	    if ( (LA27_0 == LINE) )
            	    {
            	        alt27 = 1;
            	    }


            	    switch (alt27) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:217:14: line_= line
            			    {
            			    	PushFollow(FOLLOW_line_in_multilineText1629);
            			    	line_ = line();
            			    	state.followingStackPointer--;

            			    	 lines.Append(line_); 

            			    }
            			    break;

            			default:
            			    goto loop27;
            	    }
            	} while (true);

            	loop27:
            		;	// Stops C# compiler whining that label 'loop27' has no statements

            	PushFollow(FOLLOW_indent_in_multilineText1649);
            	indent_ = indent();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                multilineText =  new MultilineText(lines.ToString(), indent_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return multilineText;
    }
    // $ANTLR end "multilineText"


    // $ANTLR start "line"
    // SpecFlowLangWalker.g:222:1: line returns [string line] : ^( LINE (ws= WS )? (text_= text )? nl= NEWLINE ) ;
    public string line() // throws RecognitionException [1]
    {   
        string line = default(string);

        CommonTree ws = null;
        CommonTree nl = null;
        Text text_ = default(Text);



            var buffer = new StringBuilder();

        try 
    	{
            // SpecFlowLangWalker.g:229:5: ( ^( LINE (ws= WS )? (text_= text )? nl= NEWLINE ) )
            // SpecFlowLangWalker.g:229:9: ^( LINE (ws= WS )? (text_= text )? nl= NEWLINE )
            {
            	Match(input,LINE,FOLLOW_LINE_in_line1695); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:230:13: (ws= WS )?
            	int alt28 = 2;
            	int LA28_0 = input.LA(1);

            	if ( (LA28_0 == WS) )
            	{
            	    alt28 = 1;
            	}
            	switch (alt28) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:230:14: ws= WS
            	        {
            	        	ws=(CommonTree)Match(input,WS,FOLLOW_WS_in_line1712); 
            	        	 buffer.Append(ws.Text); 

            	        }
            	        break;

            	}

            	// SpecFlowLangWalker.g:231:13: (text_= text )?
            	int alt29 = 2;
            	int LA29_0 = input.LA(1);

            	if ( (LA29_0 == TEXT) )
            	{
            	    alt29 = 1;
            	}
            	switch (alt29) 
            	{
            	    case 1 :
            	        // SpecFlowLangWalker.g:231:14: text_= text
            	        {
            	        	PushFollow(FOLLOW_text_in_line1742);
            	        	text_ = text();
            	        	state.followingStackPointer--;

            	        	 buffer.Append(text_.Value); 

            	        }
            	        break;

            	}

            	nl=(CommonTree)Match(input,NEWLINE,FOLLOW_NEWLINE_in_line1766); 
            	 buffer.Append(nl.Text); 

            	Match(input, Token.UP, null); 

            }


                line =  buffer.ToString();

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return line;
    }
    // $ANTLR end "line"


    // $ANTLR start "indent"
    // SpecFlowLangWalker.g:236:1: indent returns [string indent] : ^( INDENT (ws= WS )? ) ;
    public string indent() // throws RecognitionException [1]
    {   
        string indent = default(string);

        CommonTree ws = null;


            indent =  "";

        try 
    	{
            // SpecFlowLangWalker.g:240:5: ( ^( INDENT (ws= WS )? ) )
            // SpecFlowLangWalker.g:240:9: ^( INDENT (ws= WS )? )
            {
            	Match(input,INDENT,FOLLOW_INDENT_in_indent1811); 

            	if ( input.LA(1) == Token.DOWN )
            	{
            	    Match(input, Token.DOWN, null); 
            	    // SpecFlowLangWalker.g:241:13: (ws= WS )?
            	    int alt30 = 2;
            	    int LA30_0 = input.LA(1);

            	    if ( (LA30_0 == WS) )
            	    {
            	        alt30 = 1;
            	    }
            	    switch (alt30) 
            	    {
            	        case 1 :
            	            // SpecFlowLangWalker.g:241:14: ws= WS
            	            {
            	            	ws=(CommonTree)Match(input,WS,FOLLOW_WS_in_indent1828); 
            	            	 indent =  ws.Text; 

            	            }
            	            break;

            	    }


            	    Match(input, Token.UP, null); 
            	}

            }

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return indent;
    }
    // $ANTLR end "indent"


    // $ANTLR start "table"
    // SpecFlowLangWalker.g:245:1: table returns [Table table] : ^( TABLE ^( HEADER header_= tableRow ) ^( BODY (row_= tableRow )+ ) ) ;
    public Table table() // throws RecognitionException [1]
    {   
        Table table = default(Table);

        Row header_ = default(Row);

        Row row_ = default(Row);



            var bodyRows = new List<Row>();

        try 
    	{
            // SpecFlowLangWalker.g:252:5: ( ^( TABLE ^( HEADER header_= tableRow ) ^( BODY (row_= tableRow )+ ) ) )
            // SpecFlowLangWalker.g:252:9: ^( TABLE ^( HEADER header_= tableRow ) ^( BODY (row_= tableRow )+ ) )
            {
            	Match(input,TABLE,FOLLOW_TABLE_in_table1876); 

            	Match(input, Token.DOWN, null); 
            	Match(input,HEADER,FOLLOW_HEADER_in_table1891); 

            	Match(input, Token.DOWN, null); 
            	PushFollow(FOLLOW_tableRow_in_table1895);
            	header_ = tableRow();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 
            	Match(input,BODY,FOLLOW_BODY_in_table1911); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:254:20: (row_= tableRow )+
            	int cnt31 = 0;
            	do 
            	{
            	    int alt31 = 2;
            	    int LA31_0 = input.LA(1);

            	    if ( (LA31_0 == ROW) )
            	    {
            	        alt31 = 1;
            	    }


            	    switch (alt31) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:254:21: row_= tableRow
            			    {
            			    	PushFollow(FOLLOW_tableRow_in_table1916);
            			    	row_ = tableRow();
            			    	state.followingStackPointer--;

            			    	 bodyRows.Add(row_); 

            			    }
            			    break;

            			default:
            			    if ( cnt31 >= 1 ) goto loop31;
            		            EarlyExitException eee31 =
            		                new EarlyExitException(31, input);
            		            throw eee31;
            	    }
            	    cnt31++;
            	} while (true);

            	loop31:
            		;	// Stops C# compiler whinging that label 'loop31' has no statements


            	Match(input, Token.UP, null); 

            	Match(input, Token.UP, null); 

            }


                table =  new Table(header_, bodyRows.ToArray());

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return table;
    }
    // $ANTLR end "table"


    // $ANTLR start "tableRow"
    // SpecFlowLangWalker.g:258:1: tableRow returns [Row row] : ^( ROW (cell_= tableCell )+ ) ;
    public Row tableRow() // throws RecognitionException [1]
    {   
        Row row = default(Row);

        Cell cell_ = default(Cell);



            var cells = new List<Cell>();

        try 
    	{
            // SpecFlowLangWalker.g:265:5: ( ^( ROW (cell_= tableCell )+ ) )
            // SpecFlowLangWalker.g:265:9: ^( ROW (cell_= tableCell )+ )
            {
            	Match(input,ROW,FOLLOW_ROW_in_tableRow1964); 

            	Match(input, Token.DOWN, null); 
            	// SpecFlowLangWalker.g:266:13: (cell_= tableCell )+
            	int cnt32 = 0;
            	do 
            	{
            	    int alt32 = 2;
            	    int LA32_0 = input.LA(1);

            	    if ( (LA32_0 == CELL) )
            	    {
            	        alt32 = 1;
            	    }


            	    switch (alt32) 
            		{
            			case 1 :
            			    // SpecFlowLangWalker.g:266:14: cell_= tableCell
            			    {
            			    	PushFollow(FOLLOW_tableCell_in_tableRow1981);
            			    	cell_ = tableCell();
            			    	state.followingStackPointer--;

            			    	 cells.Add(cell_); 

            			    }
            			    break;

            			default:
            			    if ( cnt32 >= 1 ) goto loop32;
            		            EarlyExitException eee32 =
            		                new EarlyExitException(32, input);
            		            throw eee32;
            	    }
            	    cnt32++;
            	} while (true);

            	loop32:
            		;	// Stops C# compiler whinging that label 'loop32' has no statements


            	Match(input, Token.UP, null); 

            }


                row =  new Row(cells.ToArray());

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return row;
    }
    // $ANTLR end "tableRow"


    // $ANTLR start "tableCell"
    // SpecFlowLangWalker.g:270:1: tableCell returns [Cell cell] : ^( CELL text_= text ) ;
    public Cell tableCell() // throws RecognitionException [1]
    {   
        Cell cell = default(Cell);

        Text text_ = default(Text);


        try 
    	{
            // SpecFlowLangWalker.g:274:5: ( ^( CELL text_= text ) )
            // SpecFlowLangWalker.g:274:9: ^( CELL text_= text )
            {
            	Match(input,CELL,FOLLOW_CELL_in_tableCell2023); 

            	Match(input, Token.DOWN, null); 
            	PushFollow(FOLLOW_text_in_tableCell2039);
            	text_ = text();
            	state.followingStackPointer--;


            	Match(input, Token.UP, null); 

            }


                cell =  new Cell(text_);

        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
        }
        finally 
    	{
        }
        return cell;
    }
    // $ANTLR end "tableCell"

    // Delegated rules


	private void InitializeCyclicDFAs()
	{
	}

 

    public static readonly BitSet FOLLOW_FEATURE_in_feature73 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tags_in_feature90 = new BitSet(new ulong[]{0x0000000000010000UL});
    public static readonly BitSet FOLLOW_text_in_feature108 = new BitSet(new ulong[]{0x00000000000000E0UL});
    public static readonly BitSet FOLLOW_descriptionLine_in_feature125 = new BitSet(new ulong[]{0x00000000000000E0UL});
    public static readonly BitSet FOLLOW_background_in_feature146 = new BitSet(new ulong[]{0x0000000000000080UL});
    public static readonly BitSet FOLLOW_SCENARIOS_in_feature163 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_scenarioKind_in_feature185 = new BitSet(new ulong[]{0x0000000000000308UL});
    public static readonly BitSet FOLLOW_TAGS_in_tags243 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tag_in_tags260 = new BitSet(new ulong[]{0x0000000000100008UL});
    public static readonly BitSet FOLLOW_TAG_in_tag302 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_word_in_tag318 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_WORD_in_word361 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_word378 = new BitSet(new ulong[]{0x0000000100000008UL});
    public static readonly BitSet FOLLOW_DESCRIPTIONLINE_in_descriptionLine420 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_descriptionLine436 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BACKGROUND_in_background474 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_background491 = new BitSet(new ulong[]{0x0000000000001000UL});
    public static readonly BitSet FOLLOW_steps_in_background509 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_scenario_in_scenarioKind543 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_scenarioOutline_in_scenarioKind557 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SCENARIOOUTLINE_in_scenarioOutline587 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tags_in_scenarioOutline603 = new BitSet(new ulong[]{0x0000000000010000UL});
    public static readonly BitSet FOLLOW_text_in_scenarioOutline620 = new BitSet(new ulong[]{0x0000000000001000UL});
    public static readonly BitSet FOLLOW_steps_in_scenarioOutline636 = new BitSet(new ulong[]{0x0000000000000400UL});
    public static readonly BitSet FOLLOW_examples_in_scenarioOutline652 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_SCENARIO_in_scenario690 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tags_in_scenario707 = new BitSet(new ulong[]{0x0000000000010000UL});
    public static readonly BitSet FOLLOW_text_in_scenario724 = new BitSet(new ulong[]{0x0000000000001000UL});
    public static readonly BitSet FOLLOW_steps_in_scenario740 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_EXAMPLES_in_examples783 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_exampleSet_in_examples800 = new BitSet(new ulong[]{0x0000000000000808UL});
    public static readonly BitSet FOLLOW_EXAMPLESET_in_exampleSet842 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_exampleSet858 = new BitSet(new ulong[]{0x0000000002000000UL});
    public static readonly BitSet FOLLOW_table_in_exampleSet875 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_STEPS_in_steps913 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_step_in_steps930 = new BitSet(new ulong[]{0x000000000006E008UL});
    public static readonly BitSet FOLLOW_GIVEN_in_step967 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_step983 = new BitSet(new ulong[]{0x0000000002400008UL});
    public static readonly BitSet FOLLOW_multilineText_in_step999 = new BitSet(new ulong[]{0x0000000002000008UL});
    public static readonly BitSet FOLLOW_table_in_step1016 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_WHEN_in_step1048 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_step1064 = new BitSet(new ulong[]{0x0000000002400008UL});
    public static readonly BitSet FOLLOW_multilineText_in_step1080 = new BitSet(new ulong[]{0x0000000002000008UL});
    public static readonly BitSet FOLLOW_table_in_step1097 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_THEN_in_step1129 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_step1145 = new BitSet(new ulong[]{0x0000000002400008UL});
    public static readonly BitSet FOLLOW_multilineText_in_step1161 = new BitSet(new ulong[]{0x0000000002000008UL});
    public static readonly BitSet FOLLOW_table_in_step1178 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_AND_in_step1210 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_step1226 = new BitSet(new ulong[]{0x0000000002400008UL});
    public static readonly BitSet FOLLOW_multilineText_in_step1242 = new BitSet(new ulong[]{0x0000000002000008UL});
    public static readonly BitSet FOLLOW_table_in_step1259 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BUT_in_step1291 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_step1307 = new BitSet(new ulong[]{0x0000000002400008UL});
    public static readonly BitSet FOLLOW_multilineText_in_step1323 = new BitSet(new ulong[]{0x0000000002000008UL});
    public static readonly BitSet FOLLOW_table_in_step1340 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_TEXT_in_text1395 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_wordchar_in_text1412 = new BitSet(new ulong[]{0x00000005C0000008UL});
    public static readonly BitSet FOLLOW_WS_in_text1443 = new BitSet(new ulong[]{0x00000005C0000008UL});
    public static readonly BitSet FOLLOW_wordchar_in_text1475 = new BitSet(new ulong[]{0x00000005C0000008UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_text1501 = new BitSet(new ulong[]{0x00000005C0000008UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_wordchar1557 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_AT_in_wordchar1571 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_MULTILINETEXT_in_multilineText1612 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_line_in_multilineText1629 = new BitSet(new ulong[]{0x0000000001800000UL});
    public static readonly BitSet FOLLOW_indent_in_multilineText1649 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_LINE_in_line1695 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WS_in_line1712 = new BitSet(new ulong[]{0x0000000400010000UL});
    public static readonly BitSet FOLLOW_text_in_line1742 = new BitSet(new ulong[]{0x0000000400000000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_line1766 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_INDENT_in_indent1811 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_WS_in_indent1828 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_TABLE_in_table1876 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_HEADER_in_table1891 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableRow_in_table1895 = new BitSet(new ulong[]{0x0000000000000008UL});
    public static readonly BitSet FOLLOW_BODY_in_table1911 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableRow_in_table1916 = new BitSet(new ulong[]{0x0000000010000008UL});
    public static readonly BitSet FOLLOW_ROW_in_tableRow1964 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_tableCell_in_tableRow1981 = new BitSet(new ulong[]{0x0000000020000008UL});
    public static readonly BitSet FOLLOW_CELL_in_tableCell2023 = new BitSet(new ulong[]{0x0000000000000004UL});
    public static readonly BitSet FOLLOW_text_in_tableCell2039 = new BitSet(new ulong[]{0x0000000000000008UL});

}
}