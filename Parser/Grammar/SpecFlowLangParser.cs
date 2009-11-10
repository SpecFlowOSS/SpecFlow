// $ANTLR 3.1.2 SpecFlowLangParser.g 2009-11-10 15:19:43

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  TechTalk.SpecFlow.Parser.Grammar 
{

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;


using Antlr.Runtime.Tree;

public partial class SpecFlowLangParser : Parser
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"WSCHAR", 
		"NONWCHR", 
		"NEWLINECHR", 
		"NONNLCHR", 
		"T_FEATURE", 
		"T_BACKGROUND", 
		"T_SCENARIO", 
		"T_SCENARIO_OUTLINE", 
		"T_EXAMPLES", 
		"T_GIVEN", 
		"T_WHEN", 
		"T_THEN", 
		"T_AND", 
		"T_BUT", 
		"MLTEXT", 
		"CELLSEP", 
		"AT", 
		"COMMENT", 
		"WS", 
		"NEWLINE", 
		"WORDCHAR", 
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
		"CELL"
    };

    public const int NEWLINECHR = 6;
    public const int ROW = 49;
    public const int T_BACKGROUND = 9;
    public const int TABLE = 46;
    public const int CELL = 50;
    public const int MLTEXT = 18;
    public const int T_SCENARIO_OUTLINE = 11;
    public const int DESCRIPTIONLINE = 26;
    public const int AND = 38;
    public const int EOF = -1;
    public const int T_AND = 16;
    public const int INDENT = 44;
    public const int T_GIVEN = 13;
    public const int AT = 20;
    public const int WORD = 42;
    public const int BACKGROUND = 27;
    public const int THEN = 36;
    public const int MULTILINETEXT = 43;
    public const int T_EXAMPLES = 12;
    public const int NONWCHR = 5;
    public const int BODY = 48;
    public const int GIVEN = 34;
    public const int HEADER = 47;
    public const int COMMENT = 21;
    public const int SCENARIO = 29;
    public const int CELLSEP = 19;
    public const int EXAMPLESET = 32;
    public const int T_THEN = 15;
    public const int BUT = 39;
    public const int TAGS = 40;
    public const int EXAMPLES = 31;
    public const int WSCHAR = 4;
    public const int TEXT = 37;
    public const int NONNLCHR = 7;
    public const int LINE = 45;
    public const int FEATURE = 25;
    public const int TAG = 41;
    public const int SCENARIOS = 28;
    public const int T_BUT = 17;
    public const int WORDCHAR = 24;
    public const int WS = 22;
    public const int T_WHEN = 14;
    public const int NEWLINE = 23;
    public const int SCENARIOOUTLINE = 30;
    public const int WHEN = 35;
    public const int T_FEATURE = 8;
    public const int STEPS = 33;
    public const int T_SCENARIO = 10;

    // delegates
    // delegators



        public SpecFlowLangParser(ITokenStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public SpecFlowLangParser(ITokenStream input, RecognizerSharedState state)
    		: base(input, state) {
            InitializeCyclicDFAs();

             
        }
        
    protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

    public ITreeAdaptor TreeAdaptor
    {
        get { return this.adaptor; }
        set {
    	this.adaptor = value;
    	}
    }

    override public string[] TokenNames {
		get { return SpecFlowLangParser.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "SpecFlowLangParser.g"; }
    }


    public class feature_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "feature"
    // SpecFlowLangParser.g:42:1: feature : ( newlineWithSpaces )? ( tags )? ( WS )? T_FEATURE ( WS )? text newlineWithSpaces ( descriptionLine )* ( background )? ( scenarioKind )* ( WS )? EOF -> ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) ) ;
    public SpecFlowLangParser.feature_return feature() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.feature_return retval = new SpecFlowLangParser.feature_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS3 = null;
        IToken T_FEATURE4 = null;
        IToken WS5 = null;
        IToken WS11 = null;
        IToken EOF12 = null;
        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces1 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.tags_return tags2 = default(SpecFlowLangParser.tags_return);

        SpecFlowLangParser.text_return text6 = default(SpecFlowLangParser.text_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces7 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.descriptionLine_return descriptionLine8 = default(SpecFlowLangParser.descriptionLine_return);

        SpecFlowLangParser.background_return background9 = default(SpecFlowLangParser.background_return);

        SpecFlowLangParser.scenarioKind_return scenarioKind10 = default(SpecFlowLangParser.scenarioKind_return);


        object WS3_tree=null;
        object T_FEATURE4_tree=null;
        object WS5_tree=null;
        object WS11_tree=null;
        object EOF12_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_EOF = new RewriteRuleTokenStream(adaptor,"token EOF");
        RewriteRuleTokenStream stream_T_FEATURE = new RewriteRuleTokenStream(adaptor,"token T_FEATURE");
        RewriteRuleSubtreeStream stream_tags = new RewriteRuleSubtreeStream(adaptor,"rule tags");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        RewriteRuleSubtreeStream stream_scenarioKind = new RewriteRuleSubtreeStream(adaptor,"rule scenarioKind");
        RewriteRuleSubtreeStream stream_background = new RewriteRuleSubtreeStream(adaptor,"rule background");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        RewriteRuleSubtreeStream stream_descriptionLine = new RewriteRuleSubtreeStream(adaptor,"rule descriptionLine");
        try 
    	{
            // SpecFlowLangParser.g:43:5: ( ( newlineWithSpaces )? ( tags )? ( WS )? T_FEATURE ( WS )? text newlineWithSpaces ( descriptionLine )* ( background )? ( scenarioKind )* ( WS )? EOF -> ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) ) )
            // SpecFlowLangParser.g:43:9: ( newlineWithSpaces )? ( tags )? ( WS )? T_FEATURE ( WS )? text newlineWithSpaces ( descriptionLine )* ( background )? ( scenarioKind )* ( WS )? EOF
            {
            	// SpecFlowLangParser.g:43:9: ( newlineWithSpaces )?
            	int alt1 = 2;
            	int LA1_0 = input.LA(1);

            	if ( (LA1_0 == WS) )
            	{
            	    int LA1_1 = input.LA(2);

            	    if ( (LA1_1 == NEWLINE) )
            	    {
            	        alt1 = 1;
            	    }
            	}
            	else if ( (LA1_0 == NEWLINE) )
            	{
            	    alt1 = 1;
            	}
            	switch (alt1) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: newlineWithSpaces
            	        {
            	        	PushFollow(FOLLOW_newlineWithSpaces_in_feature252);
            	        	newlineWithSpaces1 = newlineWithSpaces();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces1.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:44:9: ( tags )?
            	int alt2 = 2;
            	int LA2_0 = input.LA(1);

            	if ( (LA2_0 == WS) )
            	{
            	    int LA2_1 = input.LA(2);

            	    if ( (LA2_1 == AT) )
            	    {
            	        alt2 = 1;
            	    }
            	}
            	else if ( (LA2_0 == AT) )
            	{
            	    alt2 = 1;
            	}
            	switch (alt2) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_feature263);
            	        	tags2 = tags();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_tags.Add(tags2.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:45:9: ( WS )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WS) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS3=(IToken)Match(input,WS,FOLLOW_WS_in_feature274); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS3);


            	        }
            	        break;

            	}

            	T_FEATURE4=(IToken)Match(input,T_FEATURE,FOLLOW_T_FEATURE_in_feature277); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_FEATURE.Add(T_FEATURE4);

            	// SpecFlowLangParser.g:45:23: ( WS )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == WS) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS5=(IToken)Match(input,WS,FOLLOW_WS_in_feature279); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS5);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_feature282);
            	text6 = text();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_text.Add(text6.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_feature284);
            	newlineWithSpaces7 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces7.Tree);
            	// SpecFlowLangParser.g:46:9: ( descriptionLine )*
            	do 
            	{
            	    int alt5 = 2;
            	    int LA5_0 = input.LA(1);

            	    if ( (LA5_0 == WS) )
            	    {
            	        int LA5_1 = input.LA(2);

            	        if ( (LA5_1 == WORDCHAR) )
            	        {
            	            alt5 = 1;
            	        }


            	    }
            	    else if ( (LA5_0 == WORDCHAR) )
            	    {
            	        alt5 = 1;
            	    }


            	    switch (alt5) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: descriptionLine
            			    {
            			    	PushFollow(FOLLOW_descriptionLine_in_feature294);
            			    	descriptionLine8 = descriptionLine();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_descriptionLine.Add(descriptionLine8.Tree);

            			    }
            			    break;

            			default:
            			    goto loop5;
            	    }
            	} while (true);

            	loop5:
            		;	// Stops C# compiler whining that label 'loop5' has no statements

            	// SpecFlowLangParser.g:47:9: ( background )?
            	int alt6 = 2;
            	int LA6_0 = input.LA(1);

            	if ( (LA6_0 == WS) )
            	{
            	    int LA6_1 = input.LA(2);

            	    if ( (LA6_1 == T_BACKGROUND) )
            	    {
            	        alt6 = 1;
            	    }
            	}
            	else if ( (LA6_0 == T_BACKGROUND) )
            	{
            	    alt6 = 1;
            	}
            	switch (alt6) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: background
            	        {
            	        	PushFollow(FOLLOW_background_in_feature305);
            	        	background9 = background();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_background.Add(background9.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:48:9: ( scenarioKind )*
            	do 
            	{
            	    int alt7 = 2;
            	    int LA7_0 = input.LA(1);

            	    if ( (LA7_0 == WS) )
            	    {
            	        int LA7_1 = input.LA(2);

            	        if ( ((LA7_1 >= T_SCENARIO && LA7_1 <= T_SCENARIO_OUTLINE) || LA7_1 == AT) )
            	        {
            	            alt7 = 1;
            	        }


            	    }
            	    else if ( ((LA7_0 >= T_SCENARIO && LA7_0 <= T_SCENARIO_OUTLINE) || LA7_0 == AT) )
            	    {
            	        alt7 = 1;
            	    }


            	    switch (alt7) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: scenarioKind
            			    {
            			    	PushFollow(FOLLOW_scenarioKind_in_feature316);
            			    	scenarioKind10 = scenarioKind();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_scenarioKind.Add(scenarioKind10.Tree);

            			    }
            			    break;

            			default:
            			    goto loop7;
            	    }
            	} while (true);

            	loop7:
            		;	// Stops C# compiler whining that label 'loop7' has no statements

            	// SpecFlowLangParser.g:48:23: ( WS )?
            	int alt8 = 2;
            	int LA8_0 = input.LA(1);

            	if ( (LA8_0 == WS) )
            	{
            	    alt8 = 1;
            	}
            	switch (alt8) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS11=(IToken)Match(input,WS,FOLLOW_WS_in_feature319); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS11);


            	        }
            	        break;

            	}

            	EOF12=(IToken)Match(input,EOF,FOLLOW_EOF_in_feature322); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_EOF.Add(EOF12);



            	// AST REWRITE
            	// elements:          tags, scenarioKind, descriptionLine, background, text
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 49:9: -> ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) )
            	{
            	    // SpecFlowLangParser.g:49:12: ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FEATURE, "FEATURE"), root_1);

            	    // SpecFlowLangParser.g:49:22: ( tags )?
            	    if ( stream_tags.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_tags.NextTree());

            	    }
            	    stream_tags.Reset();
            	    adaptor.AddChild(root_1, stream_text.NextTree());
            	    // SpecFlowLangParser.g:49:33: ( descriptionLine )*
            	    while ( stream_descriptionLine.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_descriptionLine.NextTree());

            	    }
            	    stream_descriptionLine.Reset();
            	    // SpecFlowLangParser.g:49:50: ( background )?
            	    if ( stream_background.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_background.NextTree());

            	    }
            	    stream_background.Reset();
            	    // SpecFlowLangParser.g:50:13: ^( SCENARIOS ( scenarioKind )* )
            	    {
            	    object root_2 = (object)adaptor.GetNilNode();
            	    root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(SCENARIOS, "SCENARIOS"), root_2);

            	    // SpecFlowLangParser.g:50:25: ( scenarioKind )*
            	    while ( stream_scenarioKind.HasNext() )
            	    {
            	        adaptor.AddChild(root_2, stream_scenarioKind.NextTree());

            	    }
            	    stream_scenarioKind.Reset();

            	    adaptor.AddChild(root_1, root_2);
            	    }

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "feature"

    public class tags_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "tags"
    // SpecFlowLangParser.g:54:1: tags : ( WS )? ( tag )+ -> ^( TAGS ( tag )+ ) ;
    public SpecFlowLangParser.tags_return tags() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.tags_return retval = new SpecFlowLangParser.tags_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS13 = null;
        SpecFlowLangParser.tag_return tag14 = default(SpecFlowLangParser.tag_return);


        object WS13_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_tag = new RewriteRuleSubtreeStream(adaptor,"rule tag");
        try 
    	{
            // SpecFlowLangParser.g:55:5: ( ( WS )? ( tag )+ -> ^( TAGS ( tag )+ ) )
            // SpecFlowLangParser.g:55:9: ( WS )? ( tag )+
            {
            	// SpecFlowLangParser.g:55:9: ( WS )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == WS) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS13=(IToken)Match(input,WS,FOLLOW_WS_in_tags399); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS13);


            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:55:13: ( tag )+
            	int cnt10 = 0;
            	do 
            	{
            	    int alt10 = 2;
            	    int LA10_0 = input.LA(1);

            	    if ( (LA10_0 == AT) )
            	    {
            	        alt10 = 1;
            	    }


            	    switch (alt10) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: tag
            			    {
            			    	PushFollow(FOLLOW_tag_in_tags402);
            			    	tag14 = tag();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_tag.Add(tag14.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt10 >= 1 ) goto loop10;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee10 =
            		                new EarlyExitException(10, input);
            		            throw eee10;
            	    }
            	    cnt10++;
            	} while (true);

            	loop10:
            		;	// Stops C# compiler whinging that label 'loop10' has no statements



            	// AST REWRITE
            	// elements:          tag
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 56:9: -> ^( TAGS ( tag )+ )
            	{
            	    // SpecFlowLangParser.g:56:12: ^( TAGS ( tag )+ )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TAGS, "TAGS"), root_1);

            	    if ( !(stream_tag.HasNext()) ) {
            	        throw new RewriteEarlyExitException();
            	    }
            	    while ( stream_tag.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_tag.NextTree());

            	    }
            	    stream_tag.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "tags"

    public class tag_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "tag"
    // SpecFlowLangParser.g:59:1: tag : AT word ( newlineWithSpaces | WS ) -> ^( TAG word ) ;
    public SpecFlowLangParser.tag_return tag() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.tag_return retval = new SpecFlowLangParser.tag_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken AT15 = null;
        IToken WS18 = null;
        SpecFlowLangParser.word_return word16 = default(SpecFlowLangParser.word_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces17 = default(SpecFlowLangParser.newlineWithSpaces_return);


        object AT15_tree=null;
        object WS18_tree=null;
        RewriteRuleTokenStream stream_AT = new RewriteRuleTokenStream(adaptor,"token AT");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_word = new RewriteRuleSubtreeStream(adaptor,"rule word");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:60:5: ( AT word ( newlineWithSpaces | WS ) -> ^( TAG word ) )
            // SpecFlowLangParser.g:60:9: AT word ( newlineWithSpaces | WS )
            {
            	AT15=(IToken)Match(input,AT,FOLLOW_AT_in_tag439); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_AT.Add(AT15);

            	PushFollow(FOLLOW_word_in_tag441);
            	word16 = word();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_word.Add(word16.Tree);
            	// SpecFlowLangParser.g:60:17: ( newlineWithSpaces | WS )
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == WS) )
            	{
            	    int LA11_1 = input.LA(2);

            	    if ( (LA11_1 == EOF || LA11_1 == T_FEATURE || (LA11_1 >= T_SCENARIO && LA11_1 <= T_SCENARIO_OUTLINE) || LA11_1 == AT || LA11_1 == WS) )
            	    {
            	        alt11 = 2;
            	    }
            	    else if ( (LA11_1 == NEWLINE) )
            	    {
            	        alt11 = 1;
            	    }
            	    else 
            	    {
            	        if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	        NoViableAltException nvae_d11s1 =
            	            new NoViableAltException("", 11, 1, input);

            	        throw nvae_d11s1;
            	    }
            	}
            	else if ( (LA11_0 == NEWLINE) )
            	{
            	    alt11 = 1;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d11s0 =
            	        new NoViableAltException("", 11, 0, input);

            	    throw nvae_d11s0;
            	}
            	switch (alt11) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:60:18: newlineWithSpaces
            	        {
            	        	PushFollow(FOLLOW_newlineWithSpaces_in_tag444);
            	        	newlineWithSpaces17 = newlineWithSpaces();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces17.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // SpecFlowLangParser.g:60:36: WS
            	        {
            	        	WS18=(IToken)Match(input,WS,FOLLOW_WS_in_tag446); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS18);


            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          word
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 61:9: -> ^( TAG word )
            	{
            	    // SpecFlowLangParser.g:61:12: ^( TAG word )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TAG, "TAG"), root_1);

            	    adaptor.AddChild(root_1, stream_word.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "tag"

    public class word_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "word"
    // SpecFlowLangParser.g:64:1: word : ( WORDCHAR )+ -> ^( WORD ( WORDCHAR )+ ) ;
    public SpecFlowLangParser.word_return word() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.word_return retval = new SpecFlowLangParser.word_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WORDCHAR19 = null;

        object WORDCHAR19_tree=null;
        RewriteRuleTokenStream stream_WORDCHAR = new RewriteRuleTokenStream(adaptor,"token WORDCHAR");

        try 
    	{
            // SpecFlowLangParser.g:65:5: ( ( WORDCHAR )+ -> ^( WORD ( WORDCHAR )+ ) )
            // SpecFlowLangParser.g:65:9: ( WORDCHAR )+
            {
            	// SpecFlowLangParser.g:65:9: ( WORDCHAR )+
            	int cnt12 = 0;
            	do 
            	{
            	    int alt12 = 2;
            	    int LA12_0 = input.LA(1);

            	    if ( (LA12_0 == WORDCHAR) )
            	    {
            	        alt12 = 1;
            	    }


            	    switch (alt12) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: WORDCHAR
            			    {
            			    	WORDCHAR19=(IToken)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_word482); if (state.failed) return retval; 
            			    	if ( (state.backtracking==0) ) stream_WORDCHAR.Add(WORDCHAR19);


            			    }
            			    break;

            			default:
            			    if ( cnt12 >= 1 ) goto loop12;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee12 =
            		                new EarlyExitException(12, input);
            		            throw eee12;
            	    }
            	    cnt12++;
            	} while (true);

            	loop12:
            		;	// Stops C# compiler whinging that label 'loop12' has no statements



            	// AST REWRITE
            	// elements:          WORDCHAR
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 66:9: -> ^( WORD ( WORDCHAR )+ )
            	{
            	    // SpecFlowLangParser.g:66:12: ^( WORD ( WORDCHAR )+ )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(WORD, "WORD"), root_1);

            	    if ( !(stream_WORDCHAR.HasNext()) ) {
            	        throw new RewriteEarlyExitException();
            	    }
            	    while ( stream_WORDCHAR.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_WORDCHAR.NextNode());

            	    }
            	    stream_WORDCHAR.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "word"

    public class descriptionLine_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "descriptionLine"
    // SpecFlowLangParser.g:69:1: descriptionLine : ( WS )? descriptionLineText newlineWithSpaces -> ^( DESCRIPTIONLINE descriptionLineText ) ;
    public SpecFlowLangParser.descriptionLine_return descriptionLine() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.descriptionLine_return retval = new SpecFlowLangParser.descriptionLine_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS20 = null;
        SpecFlowLangParser.descriptionLineText_return descriptionLineText21 = default(SpecFlowLangParser.descriptionLineText_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces22 = default(SpecFlowLangParser.newlineWithSpaces_return);


        object WS20_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_descriptionLineText = new RewriteRuleSubtreeStream(adaptor,"rule descriptionLineText");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:70:5: ( ( WS )? descriptionLineText newlineWithSpaces -> ^( DESCRIPTIONLINE descriptionLineText ) )
            // SpecFlowLangParser.g:70:9: ( WS )? descriptionLineText newlineWithSpaces
            {
            	// SpecFlowLangParser.g:70:9: ( WS )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == WS) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS20=(IToken)Match(input,WS,FOLLOW_WS_in_descriptionLine519); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS20);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_descriptionLineText_in_descriptionLine522);
            	descriptionLineText21 = descriptionLineText();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_descriptionLineText.Add(descriptionLineText21.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_descriptionLine524);
            	newlineWithSpaces22 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces22.Tree);


            	// AST REWRITE
            	// elements:          descriptionLineText
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 71:9: -> ^( DESCRIPTIONLINE descriptionLineText )
            	{
            	    // SpecFlowLangParser.g:71:12: ^( DESCRIPTIONLINE descriptionLineText )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(DESCRIPTIONLINE, "DESCRIPTIONLINE"), root_1);

            	    adaptor.AddChild(root_1, stream_descriptionLineText.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "descriptionLine"

    public class background_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "background"
    // SpecFlowLangParser.g:74:1: background : ( WS )? T_BACKGROUND ( WS title )? newlineWithSpaces givens -> ^( BACKGROUND ( title )? givens ) ;
    public SpecFlowLangParser.background_return background() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.background_return retval = new SpecFlowLangParser.background_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS23 = null;
        IToken T_BACKGROUND24 = null;
        IToken WS25 = null;
        SpecFlowLangParser.title_return title26 = default(SpecFlowLangParser.title_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces27 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.givens_return givens28 = default(SpecFlowLangParser.givens_return);


        object WS23_tree=null;
        object T_BACKGROUND24_tree=null;
        object WS25_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_T_BACKGROUND = new RewriteRuleTokenStream(adaptor,"token T_BACKGROUND");
        RewriteRuleSubtreeStream stream_title = new RewriteRuleSubtreeStream(adaptor,"rule title");
        RewriteRuleSubtreeStream stream_givens = new RewriteRuleSubtreeStream(adaptor,"rule givens");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:75:5: ( ( WS )? T_BACKGROUND ( WS title )? newlineWithSpaces givens -> ^( BACKGROUND ( title )? givens ) )
            // SpecFlowLangParser.g:75:9: ( WS )? T_BACKGROUND ( WS title )? newlineWithSpaces givens
            {
            	// SpecFlowLangParser.g:75:9: ( WS )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == WS) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS23=(IToken)Match(input,WS,FOLLOW_WS_in_background559); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS23);


            	        }
            	        break;

            	}

            	T_BACKGROUND24=(IToken)Match(input,T_BACKGROUND,FOLLOW_T_BACKGROUND_in_background562); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_BACKGROUND.Add(T_BACKGROUND24);

            	// SpecFlowLangParser.g:76:9: ( WS title )?
            	int alt15 = 2;
            	int LA15_0 = input.LA(1);

            	if ( (LA15_0 == WS) )
            	{
            	    int LA15_1 = input.LA(2);

            	    if ( (LA15_1 == AT || LA15_1 == WORDCHAR) )
            	    {
            	        alt15 = 1;
            	    }
            	}
            	switch (alt15) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:76:10: WS title
            	        {
            	        	WS25=(IToken)Match(input,WS,FOLLOW_WS_in_background574); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS25);

            	        	PushFollow(FOLLOW_title_in_background576);
            	        	title26 = title();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_title.Add(title26.Tree);

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_background589);
            	newlineWithSpaces27 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces27.Tree);
            	PushFollow(FOLLOW_givens_in_background591);
            	givens28 = givens();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_givens.Add(givens28.Tree);


            	// AST REWRITE
            	// elements:          title, givens
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 78:9: -> ^( BACKGROUND ( title )? givens )
            	{
            	    // SpecFlowLangParser.g:78:12: ^( BACKGROUND ( title )? givens )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BACKGROUND, "BACKGROUND"), root_1);

            	    // SpecFlowLangParser.g:78:25: ( title )?
            	    if ( stream_title.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_title.NextTree());

            	    }
            	    stream_title.Reset();
            	    adaptor.AddChild(root_1, stream_givens.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "background"

    public class scenarioKind_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "scenarioKind"
    // SpecFlowLangParser.g:81:1: scenarioKind : ( scenarioOutline | scenario );
    public SpecFlowLangParser.scenarioKind_return scenarioKind() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.scenarioKind_return retval = new SpecFlowLangParser.scenarioKind_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.scenarioOutline_return scenarioOutline29 = default(SpecFlowLangParser.scenarioOutline_return);

        SpecFlowLangParser.scenario_return scenario30 = default(SpecFlowLangParser.scenario_return);



        try 
    	{
            // SpecFlowLangParser.g:82:5: ( scenarioOutline | scenario )
            int alt16 = 2;
            alt16 = dfa16.Predict(input);
            switch (alt16) 
            {
                case 1 :
                    // SpecFlowLangParser.g:82:9: scenarioOutline
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_scenarioOutline_in_scenarioKind629);
                    	scenarioOutline29 = scenarioOutline();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, scenarioOutline29.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLangParser.g:83:9: scenario
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_scenario_in_scenarioKind640);
                    	scenario30 = scenario();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, scenario30.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "scenarioKind"

    public class scenario_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "scenario"
    // SpecFlowLangParser.g:86:1: scenario : ( tags )? ( WS )? T_SCENARIO ( WS )? title newlineWithSpaces steps -> ^( SCENARIO ( tags )? title steps ) ;
    public SpecFlowLangParser.scenario_return scenario() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.scenario_return retval = new SpecFlowLangParser.scenario_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS32 = null;
        IToken T_SCENARIO33 = null;
        IToken WS34 = null;
        SpecFlowLangParser.tags_return tags31 = default(SpecFlowLangParser.tags_return);

        SpecFlowLangParser.title_return title35 = default(SpecFlowLangParser.title_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces36 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.steps_return steps37 = default(SpecFlowLangParser.steps_return);


        object WS32_tree=null;
        object T_SCENARIO33_tree=null;
        object WS34_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_T_SCENARIO = new RewriteRuleTokenStream(adaptor,"token T_SCENARIO");
        RewriteRuleSubtreeStream stream_tags = new RewriteRuleSubtreeStream(adaptor,"rule tags");
        RewriteRuleSubtreeStream stream_title = new RewriteRuleSubtreeStream(adaptor,"rule title");
        RewriteRuleSubtreeStream stream_steps = new RewriteRuleSubtreeStream(adaptor,"rule steps");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:87:5: ( ( tags )? ( WS )? T_SCENARIO ( WS )? title newlineWithSpaces steps -> ^( SCENARIO ( tags )? title steps ) )
            // SpecFlowLangParser.g:87:9: ( tags )? ( WS )? T_SCENARIO ( WS )? title newlineWithSpaces steps
            {
            	// SpecFlowLangParser.g:87:9: ( tags )?
            	int alt17 = 2;
            	int LA17_0 = input.LA(1);

            	if ( (LA17_0 == WS) )
            	{
            	    int LA17_1 = input.LA(2);

            	    if ( (LA17_1 == AT) )
            	    {
            	        alt17 = 1;
            	    }
            	}
            	else if ( (LA17_0 == AT) )
            	{
            	    alt17 = 1;
            	}
            	switch (alt17) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_scenario659);
            	        	tags31 = tags();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_tags.Add(tags31.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:87:15: ( WS )?
            	int alt18 = 2;
            	int LA18_0 = input.LA(1);

            	if ( (LA18_0 == WS) )
            	{
            	    alt18 = 1;
            	}
            	switch (alt18) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS32=(IToken)Match(input,WS,FOLLOW_WS_in_scenario662); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS32);


            	        }
            	        break;

            	}

            	T_SCENARIO33=(IToken)Match(input,T_SCENARIO,FOLLOW_T_SCENARIO_in_scenario665); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_SCENARIO.Add(T_SCENARIO33);

            	// SpecFlowLangParser.g:87:30: ( WS )?
            	int alt19 = 2;
            	int LA19_0 = input.LA(1);

            	if ( (LA19_0 == WS) )
            	{
            	    alt19 = 1;
            	}
            	switch (alt19) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS34=(IToken)Match(input,WS,FOLLOW_WS_in_scenario667); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS34);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_title_in_scenario679);
            	title35 = title();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_title.Add(title35.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_scenario681);
            	newlineWithSpaces36 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces36.Tree);
            	PushFollow(FOLLOW_steps_in_scenario692);
            	steps37 = steps();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_steps.Add(steps37.Tree);


            	// AST REWRITE
            	// elements:          title, tags, steps
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 90:9: -> ^( SCENARIO ( tags )? title steps )
            	{
            	    // SpecFlowLangParser.g:90:12: ^( SCENARIO ( tags )? title steps )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(SCENARIO, "SCENARIO"), root_1);

            	    // SpecFlowLangParser.g:90:23: ( tags )?
            	    if ( stream_tags.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_tags.NextTree());

            	    }
            	    stream_tags.Reset();
            	    adaptor.AddChild(root_1, stream_title.NextTree());
            	    adaptor.AddChild(root_1, stream_steps.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "scenario"

    public class scenarioOutline_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "scenarioOutline"
    // SpecFlowLangParser.g:93:1: scenarioOutline : ( tags )? ( WS )? T_SCENARIO_OUTLINE ( WS )? title newlineWithSpaces steps examples -> ^( SCENARIOOUTLINE ( tags )? title steps examples ) ;
    public SpecFlowLangParser.scenarioOutline_return scenarioOutline() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.scenarioOutline_return retval = new SpecFlowLangParser.scenarioOutline_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS39 = null;
        IToken T_SCENARIO_OUTLINE40 = null;
        IToken WS41 = null;
        SpecFlowLangParser.tags_return tags38 = default(SpecFlowLangParser.tags_return);

        SpecFlowLangParser.title_return title42 = default(SpecFlowLangParser.title_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces43 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.steps_return steps44 = default(SpecFlowLangParser.steps_return);

        SpecFlowLangParser.examples_return examples45 = default(SpecFlowLangParser.examples_return);


        object WS39_tree=null;
        object T_SCENARIO_OUTLINE40_tree=null;
        object WS41_tree=null;
        RewriteRuleTokenStream stream_T_SCENARIO_OUTLINE = new RewriteRuleTokenStream(adaptor,"token T_SCENARIO_OUTLINE");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_tags = new RewriteRuleSubtreeStream(adaptor,"rule tags");
        RewriteRuleSubtreeStream stream_title = new RewriteRuleSubtreeStream(adaptor,"rule title");
        RewriteRuleSubtreeStream stream_steps = new RewriteRuleSubtreeStream(adaptor,"rule steps");
        RewriteRuleSubtreeStream stream_examples = new RewriteRuleSubtreeStream(adaptor,"rule examples");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:94:5: ( ( tags )? ( WS )? T_SCENARIO_OUTLINE ( WS )? title newlineWithSpaces steps examples -> ^( SCENARIOOUTLINE ( tags )? title steps examples ) )
            // SpecFlowLangParser.g:95:9: ( tags )? ( WS )? T_SCENARIO_OUTLINE ( WS )? title newlineWithSpaces steps examples
            {
            	// SpecFlowLangParser.g:95:9: ( tags )?
            	int alt20 = 2;
            	int LA20_0 = input.LA(1);

            	if ( (LA20_0 == WS) )
            	{
            	    int LA20_1 = input.LA(2);

            	    if ( (LA20_1 == AT) )
            	    {
            	        alt20 = 1;
            	    }
            	}
            	else if ( (LA20_0 == AT) )
            	{
            	    alt20 = 1;
            	}
            	switch (alt20) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_scenarioOutline738);
            	        	tags38 = tags();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_tags.Add(tags38.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:95:15: ( WS )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == WS) )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS39=(IToken)Match(input,WS,FOLLOW_WS_in_scenarioOutline741); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS39);


            	        }
            	        break;

            	}

            	T_SCENARIO_OUTLINE40=(IToken)Match(input,T_SCENARIO_OUTLINE,FOLLOW_T_SCENARIO_OUTLINE_in_scenarioOutline744); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_SCENARIO_OUTLINE.Add(T_SCENARIO_OUTLINE40);

            	// SpecFlowLangParser.g:95:38: ( WS )?
            	int alt22 = 2;
            	int LA22_0 = input.LA(1);

            	if ( (LA22_0 == WS) )
            	{
            	    alt22 = 1;
            	}
            	switch (alt22) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS41=(IToken)Match(input,WS,FOLLOW_WS_in_scenarioOutline746); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS41);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_title_in_scenarioOutline757);
            	title42 = title();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_title.Add(title42.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_scenarioOutline759);
            	newlineWithSpaces43 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces43.Tree);
            	PushFollow(FOLLOW_steps_in_scenarioOutline769);
            	steps44 = steps();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_steps.Add(steps44.Tree);
            	PushFollow(FOLLOW_examples_in_scenarioOutline779);
            	examples45 = examples();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_examples.Add(examples45.Tree);


            	// AST REWRITE
            	// elements:          steps, title, examples, tags
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 99:9: -> ^( SCENARIOOUTLINE ( tags )? title steps examples )
            	{
            	    // SpecFlowLangParser.g:99:12: ^( SCENARIOOUTLINE ( tags )? title steps examples )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(SCENARIOOUTLINE, "SCENARIOOUTLINE"), root_1);

            	    // SpecFlowLangParser.g:99:30: ( tags )?
            	    if ( stream_tags.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_tags.NextTree());

            	    }
            	    stream_tags.Reset();
            	    adaptor.AddChild(root_1, stream_title.NextTree());
            	    adaptor.AddChild(root_1, stream_steps.NextTree());
            	    adaptor.AddChild(root_1, stream_examples.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "scenarioOutline"

    public class examples_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "examples"
    // SpecFlowLangParser.g:102:1: examples : ( exampleSet )+ -> ^( EXAMPLES ( exampleSet )+ ) ;
    public SpecFlowLangParser.examples_return examples() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.examples_return retval = new SpecFlowLangParser.examples_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.exampleSet_return exampleSet46 = default(SpecFlowLangParser.exampleSet_return);


        RewriteRuleSubtreeStream stream_exampleSet = new RewriteRuleSubtreeStream(adaptor,"rule exampleSet");
        try 
    	{
            // SpecFlowLangParser.g:103:5: ( ( exampleSet )+ -> ^( EXAMPLES ( exampleSet )+ ) )
            // SpecFlowLangParser.g:103:9: ( exampleSet )+
            {
            	// SpecFlowLangParser.g:103:9: ( exampleSet )+
            	int cnt23 = 0;
            	do 
            	{
            	    int alt23 = 2;
            	    int LA23_0 = input.LA(1);

            	    if ( (LA23_0 == WS) )
            	    {
            	        int LA23_1 = input.LA(2);

            	        if ( (LA23_1 == T_EXAMPLES) )
            	        {
            	            alt23 = 1;
            	        }


            	    }
            	    else if ( (LA23_0 == T_EXAMPLES) )
            	    {
            	        alt23 = 1;
            	    }


            	    switch (alt23) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: exampleSet
            			    {
            			    	PushFollow(FOLLOW_exampleSet_in_examples821);
            			    	exampleSet46 = exampleSet();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_exampleSet.Add(exampleSet46.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt23 >= 1 ) goto loop23;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee23 =
            		                new EarlyExitException(23, input);
            		            throw eee23;
            	    }
            	    cnt23++;
            	} while (true);

            	loop23:
            		;	// Stops C# compiler whinging that label 'loop23' has no statements



            	// AST REWRITE
            	// elements:          exampleSet
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 104:9: -> ^( EXAMPLES ( exampleSet )+ )
            	{
            	    // SpecFlowLangParser.g:104:12: ^( EXAMPLES ( exampleSet )+ )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXAMPLES, "EXAMPLES"), root_1);

            	    if ( !(stream_exampleSet.HasNext()) ) {
            	        throw new RewriteEarlyExitException();
            	    }
            	    while ( stream_exampleSet.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_exampleSet.NextTree());

            	    }
            	    stream_exampleSet.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "examples"

    public class exampleSet_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "exampleSet"
    // SpecFlowLangParser.g:107:1: exampleSet : ( WS )? T_EXAMPLES ( WS )? ( text )? newlineWithSpaces table -> ^( EXAMPLESET ( text )? table ) ;
    public SpecFlowLangParser.exampleSet_return exampleSet() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.exampleSet_return retval = new SpecFlowLangParser.exampleSet_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS47 = null;
        IToken T_EXAMPLES48 = null;
        IToken WS49 = null;
        SpecFlowLangParser.text_return text50 = default(SpecFlowLangParser.text_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces51 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.table_return table52 = default(SpecFlowLangParser.table_return);


        object WS47_tree=null;
        object T_EXAMPLES48_tree=null;
        object WS49_tree=null;
        RewriteRuleTokenStream stream_T_EXAMPLES = new RewriteRuleTokenStream(adaptor,"token T_EXAMPLES");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        RewriteRuleSubtreeStream stream_table = new RewriteRuleSubtreeStream(adaptor,"rule table");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:108:5: ( ( WS )? T_EXAMPLES ( WS )? ( text )? newlineWithSpaces table -> ^( EXAMPLESET ( text )? table ) )
            // SpecFlowLangParser.g:108:9: ( WS )? T_EXAMPLES ( WS )? ( text )? newlineWithSpaces table
            {
            	// SpecFlowLangParser.g:108:9: ( WS )?
            	int alt24 = 2;
            	int LA24_0 = input.LA(1);

            	if ( (LA24_0 == WS) )
            	{
            	    alt24 = 1;
            	}
            	switch (alt24) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS47=(IToken)Match(input,WS,FOLLOW_WS_in_exampleSet858); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS47);


            	        }
            	        break;

            	}

            	T_EXAMPLES48=(IToken)Match(input,T_EXAMPLES,FOLLOW_T_EXAMPLES_in_exampleSet861); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_EXAMPLES.Add(T_EXAMPLES48);

            	// SpecFlowLangParser.g:108:24: ( WS )?
            	int alt25 = 2;
            	int LA25_0 = input.LA(1);

            	if ( (LA25_0 == WS) )
            	{
            	    int LA25_1 = input.LA(2);

            	    if ( (synpred25_SpecFlowLangParser()) )
            	    {
            	        alt25 = 1;
            	    }
            	}
            	switch (alt25) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS49=(IToken)Match(input,WS,FOLLOW_WS_in_exampleSet863); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS49);


            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:109:9: ( text )?
            	int alt26 = 2;
            	int LA26_0 = input.LA(1);

            	if ( (LA26_0 == AT || LA26_0 == WORDCHAR) )
            	{
            	    alt26 = 1;
            	}
            	switch (alt26) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: text
            	        {
            	        	PushFollow(FOLLOW_text_in_exampleSet874);
            	        	text50 = text();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_text.Add(text50.Tree);

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_exampleSet877);
            	newlineWithSpaces51 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces51.Tree);
            	PushFollow(FOLLOW_table_in_exampleSet879);
            	table52 = table();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_table.Add(table52.Tree);


            	// AST REWRITE
            	// elements:          table, text
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 110:9: -> ^( EXAMPLESET ( text )? table )
            	{
            	    // SpecFlowLangParser.g:110:12: ^( EXAMPLESET ( text )? table )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXAMPLESET, "EXAMPLESET"), root_1);

            	    // SpecFlowLangParser.g:110:25: ( text )?
            	    if ( stream_text.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_text.NextTree());

            	    }
            	    stream_text.Reset();
            	    adaptor.AddChild(root_1, stream_table.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "exampleSet"

    public class steps_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "steps"
    // SpecFlowLangParser.g:113:1: steps : firstStep ( nextStep )* -> ^( STEPS firstStep ( nextStep )* ) ;
    public SpecFlowLangParser.steps_return steps() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.steps_return retval = new SpecFlowLangParser.steps_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstStep_return firstStep53 = default(SpecFlowLangParser.firstStep_return);

        SpecFlowLangParser.nextStep_return nextStep54 = default(SpecFlowLangParser.nextStep_return);


        RewriteRuleSubtreeStream stream_firstStep = new RewriteRuleSubtreeStream(adaptor,"rule firstStep");
        RewriteRuleSubtreeStream stream_nextStep = new RewriteRuleSubtreeStream(adaptor,"rule nextStep");
        try 
    	{
            // SpecFlowLangParser.g:114:5: ( firstStep ( nextStep )* -> ^( STEPS firstStep ( nextStep )* ) )
            // SpecFlowLangParser.g:114:9: firstStep ( nextStep )*
            {
            	PushFollow(FOLLOW_firstStep_in_steps917);
            	firstStep53 = firstStep();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_firstStep.Add(firstStep53.Tree);
            	// SpecFlowLangParser.g:114:19: ( nextStep )*
            	do 
            	{
            	    int alt27 = 2;
            	    int LA27_0 = input.LA(1);

            	    if ( (LA27_0 == WS) )
            	    {
            	        int LA27_1 = input.LA(2);

            	        if ( ((LA27_1 >= T_GIVEN && LA27_1 <= T_BUT)) )
            	        {
            	            alt27 = 1;
            	        }


            	    }
            	    else if ( ((LA27_0 >= T_GIVEN && LA27_0 <= T_BUT)) )
            	    {
            	        alt27 = 1;
            	    }


            	    switch (alt27) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: nextStep
            			    {
            			    	PushFollow(FOLLOW_nextStep_in_steps919);
            			    	nextStep54 = nextStep();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_nextStep.Add(nextStep54.Tree);

            			    }
            			    break;

            			default:
            			    goto loop27;
            	    }
            	} while (true);

            	loop27:
            		;	// Stops C# compiler whining that label 'loop27' has no statements



            	// AST REWRITE
            	// elements:          nextStep, firstStep
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 115:9: -> ^( STEPS firstStep ( nextStep )* )
            	{
            	    // SpecFlowLangParser.g:115:12: ^( STEPS firstStep ( nextStep )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(STEPS, "STEPS"), root_1);

            	    adaptor.AddChild(root_1, stream_firstStep.NextTree());
            	    // SpecFlowLangParser.g:115:30: ( nextStep )*
            	    while ( stream_nextStep.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_nextStep.NextTree());

            	    }
            	    stream_nextStep.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "steps"

    public class firstStep_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "firstStep"
    // SpecFlowLangParser.g:117:1: firstStep : ( firstGiven -> firstGiven | firstWhen -> firstWhen | firstThen -> firstThen );
    public SpecFlowLangParser.firstStep_return firstStep() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstStep_return retval = new SpecFlowLangParser.firstStep_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstGiven_return firstGiven55 = default(SpecFlowLangParser.firstGiven_return);

        SpecFlowLangParser.firstWhen_return firstWhen56 = default(SpecFlowLangParser.firstWhen_return);

        SpecFlowLangParser.firstThen_return firstThen57 = default(SpecFlowLangParser.firstThen_return);


        RewriteRuleSubtreeStream stream_firstWhen = new RewriteRuleSubtreeStream(adaptor,"rule firstWhen");
        RewriteRuleSubtreeStream stream_firstThen = new RewriteRuleSubtreeStream(adaptor,"rule firstThen");
        RewriteRuleSubtreeStream stream_firstGiven = new RewriteRuleSubtreeStream(adaptor,"rule firstGiven");
        try 
    	{
            // SpecFlowLangParser.g:118:2: ( firstGiven -> firstGiven | firstWhen -> firstWhen | firstThen -> firstThen )
            int alt28 = 3;
            switch ( input.LA(1) ) 
            {
            case WS:
            	{
                switch ( input.LA(2) ) 
                {
                case T_WHEN:
                	{
                    alt28 = 2;
                    }
                    break;
                case T_THEN:
                	{
                    alt28 = 3;
                    }
                    break;
                case T_GIVEN:
                	{
                    alt28 = 1;
                    }
                    break;
                	default:
                	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                	    NoViableAltException nvae_d28s1 =
                	        new NoViableAltException("", 28, 1, input);

                	    throw nvae_d28s1;
                }

                }
                break;
            case T_GIVEN:
            	{
                alt28 = 1;
                }
                break;
            case T_WHEN:
            	{
                alt28 = 2;
                }
                break;
            case T_THEN:
            	{
                alt28 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d28s0 =
            	        new NoViableAltException("", 28, 0, input);

            	    throw nvae_d28s0;
            }

            switch (alt28) 
            {
                case 1 :
                    // SpecFlowLangParser.g:118:4: firstGiven
                    {
                    	PushFollow(FOLLOW_firstGiven_in_firstStep952);
                    	firstGiven55 = firstGiven();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstGiven.Add(firstGiven55.Tree);


                    	// AST REWRITE
                    	// elements:          firstGiven
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	if ( (state.backtracking==0) ) {
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (object)adaptor.GetNilNode();
                    	// 118:15: -> firstGiven
                    	{
                    	    adaptor.AddChild(root_0, stream_firstGiven.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 2 :
                    // SpecFlowLangParser.g:119:4: firstWhen
                    {
                    	PushFollow(FOLLOW_firstWhen_in_firstStep961);
                    	firstWhen56 = firstWhen();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstWhen.Add(firstWhen56.Tree);


                    	// AST REWRITE
                    	// elements:          firstWhen
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	if ( (state.backtracking==0) ) {
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (object)adaptor.GetNilNode();
                    	// 119:14: -> firstWhen
                    	{
                    	    adaptor.AddChild(root_0, stream_firstWhen.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 3 :
                    // SpecFlowLangParser.g:120:4: firstThen
                    {
                    	PushFollow(FOLLOW_firstThen_in_firstStep970);
                    	firstThen57 = firstThen();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstThen.Add(firstThen57.Tree);


                    	// AST REWRITE
                    	// elements:          firstThen
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	if ( (state.backtracking==0) ) {
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (object)adaptor.GetNilNode();
                    	// 120:14: -> firstThen
                    	{
                    	    adaptor.AddChild(root_0, stream_firstThen.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "firstStep"

    public class nextStep_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "nextStep"
    // SpecFlowLangParser.g:122:1: nextStep : ( firstStep -> firstStep | firstAnd -> firstAnd | firstBut -> firstBut );
    public SpecFlowLangParser.nextStep_return nextStep() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.nextStep_return retval = new SpecFlowLangParser.nextStep_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstStep_return firstStep58 = default(SpecFlowLangParser.firstStep_return);

        SpecFlowLangParser.firstAnd_return firstAnd59 = default(SpecFlowLangParser.firstAnd_return);

        SpecFlowLangParser.firstBut_return firstBut60 = default(SpecFlowLangParser.firstBut_return);


        RewriteRuleSubtreeStream stream_firstStep = new RewriteRuleSubtreeStream(adaptor,"rule firstStep");
        RewriteRuleSubtreeStream stream_firstAnd = new RewriteRuleSubtreeStream(adaptor,"rule firstAnd");
        RewriteRuleSubtreeStream stream_firstBut = new RewriteRuleSubtreeStream(adaptor,"rule firstBut");
        try 
    	{
            // SpecFlowLangParser.g:123:5: ( firstStep -> firstStep | firstAnd -> firstAnd | firstBut -> firstBut )
            int alt29 = 3;
            switch ( input.LA(1) ) 
            {
            case WS:
            	{
                switch ( input.LA(2) ) 
                {
                case T_GIVEN:
                case T_WHEN:
                case T_THEN:
                	{
                    alt29 = 1;
                    }
                    break;
                case T_AND:
                	{
                    alt29 = 2;
                    }
                    break;
                case T_BUT:
                	{
                    alt29 = 3;
                    }
                    break;
                	default:
                	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                	    NoViableAltException nvae_d29s1 =
                	        new NoViableAltException("", 29, 1, input);

                	    throw nvae_d29s1;
                }

                }
                break;
            case T_GIVEN:
            case T_WHEN:
            case T_THEN:
            	{
                alt29 = 1;
                }
                break;
            case T_AND:
            	{
                alt29 = 2;
                }
                break;
            case T_BUT:
            	{
                alt29 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d29s0 =
            	        new NoViableAltException("", 29, 0, input);

            	    throw nvae_d29s0;
            }

            switch (alt29) 
            {
                case 1 :
                    // SpecFlowLangParser.g:123:9: firstStep
                    {
                    	PushFollow(FOLLOW_firstStep_in_nextStep992);
                    	firstStep58 = firstStep();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstStep.Add(firstStep58.Tree);


                    	// AST REWRITE
                    	// elements:          firstStep
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	if ( (state.backtracking==0) ) {
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (object)adaptor.GetNilNode();
                    	// 123:19: -> firstStep
                    	{
                    	    adaptor.AddChild(root_0, stream_firstStep.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 2 :
                    // SpecFlowLangParser.g:124:4: firstAnd
                    {
                    	PushFollow(FOLLOW_firstAnd_in_nextStep1001);
                    	firstAnd59 = firstAnd();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstAnd.Add(firstAnd59.Tree);


                    	// AST REWRITE
                    	// elements:          firstAnd
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	if ( (state.backtracking==0) ) {
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (object)adaptor.GetNilNode();
                    	// 124:13: -> firstAnd
                    	{
                    	    adaptor.AddChild(root_0, stream_firstAnd.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 3 :
                    // SpecFlowLangParser.g:125:4: firstBut
                    {
                    	PushFollow(FOLLOW_firstBut_in_nextStep1010);
                    	firstBut60 = firstBut();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstBut.Add(firstBut60.Tree);


                    	// AST REWRITE
                    	// elements:          firstBut
                    	// token labels:      
                    	// rule labels:       retval
                    	// token list labels: 
                    	// rule list labels:  
                    	// wildcard labels: 
                    	if ( (state.backtracking==0) ) {
                    	retval.Tree = root_0;
                    	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

                    	root_0 = (object)adaptor.GetNilNode();
                    	// 125:13: -> firstBut
                    	{
                    	    adaptor.AddChild(root_0, stream_firstBut.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "nextStep"

    public class firstAnd_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "firstAnd"
    // SpecFlowLangParser.g:128:1: firstAnd : ( WS )? T_AND WS sentenceEnd -> ^( AND sentenceEnd ) ;
    public SpecFlowLangParser.firstAnd_return firstAnd() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstAnd_return retval = new SpecFlowLangParser.firstAnd_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS61 = null;
        IToken T_AND62 = null;
        IToken WS63 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd64 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS61_tree=null;
        object T_AND62_tree=null;
        object WS63_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_T_AND = new RewriteRuleTokenStream(adaptor,"token T_AND");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLangParser.g:129:5: ( ( WS )? T_AND WS sentenceEnd -> ^( AND sentenceEnd ) )
            // SpecFlowLangParser.g:129:9: ( WS )? T_AND WS sentenceEnd
            {
            	// SpecFlowLangParser.g:129:9: ( WS )?
            	int alt30 = 2;
            	int LA30_0 = input.LA(1);

            	if ( (LA30_0 == WS) )
            	{
            	    alt30 = 1;
            	}
            	switch (alt30) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS61=(IToken)Match(input,WS,FOLLOW_WS_in_firstAnd1033); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS61);


            	        }
            	        break;

            	}

            	T_AND62=(IToken)Match(input,T_AND,FOLLOW_T_AND_in_firstAnd1036); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_AND.Add(T_AND62);

            	WS63=(IToken)Match(input,WS,FOLLOW_WS_in_firstAnd1038); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS63);

            	PushFollow(FOLLOW_sentenceEnd_in_firstAnd1040);
            	sentenceEnd64 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd64.Tree);


            	// AST REWRITE
            	// elements:          sentenceEnd
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 130:9: -> ^( AND sentenceEnd )
            	{
            	    // SpecFlowLangParser.g:130:12: ^( AND sentenceEnd )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(AND, "AND"), root_1);

            	    adaptor.AddChild(root_1, stream_sentenceEnd.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "firstAnd"

    public class firstBut_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "firstBut"
    // SpecFlowLangParser.g:133:1: firstBut : ( WS )? T_BUT WS sentenceEnd -> ^( BUT sentenceEnd ) ;
    public SpecFlowLangParser.firstBut_return firstBut() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstBut_return retval = new SpecFlowLangParser.firstBut_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS65 = null;
        IToken T_BUT66 = null;
        IToken WS67 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd68 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS65_tree=null;
        object T_BUT66_tree=null;
        object WS67_tree=null;
        RewriteRuleTokenStream stream_T_BUT = new RewriteRuleTokenStream(adaptor,"token T_BUT");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLangParser.g:134:5: ( ( WS )? T_BUT WS sentenceEnd -> ^( BUT sentenceEnd ) )
            // SpecFlowLangParser.g:134:9: ( WS )? T_BUT WS sentenceEnd
            {
            	// SpecFlowLangParser.g:134:9: ( WS )?
            	int alt31 = 2;
            	int LA31_0 = input.LA(1);

            	if ( (LA31_0 == WS) )
            	{
            	    alt31 = 1;
            	}
            	switch (alt31) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS65=(IToken)Match(input,WS,FOLLOW_WS_in_firstBut1075); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS65);


            	        }
            	        break;

            	}

            	T_BUT66=(IToken)Match(input,T_BUT,FOLLOW_T_BUT_in_firstBut1078); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_BUT.Add(T_BUT66);

            	WS67=(IToken)Match(input,WS,FOLLOW_WS_in_firstBut1080); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS67);

            	PushFollow(FOLLOW_sentenceEnd_in_firstBut1082);
            	sentenceEnd68 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd68.Tree);


            	// AST REWRITE
            	// elements:          sentenceEnd
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 135:9: -> ^( BUT sentenceEnd )
            	{
            	    // SpecFlowLangParser.g:135:12: ^( BUT sentenceEnd )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BUT, "BUT"), root_1);

            	    adaptor.AddChild(root_1, stream_sentenceEnd.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "firstBut"

    public class givens_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "givens"
    // SpecFlowLangParser.g:138:1: givens : firstGiven ( nextStep )* -> ^( STEPS firstGiven ( nextStep )* ) ;
    public SpecFlowLangParser.givens_return givens() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.givens_return retval = new SpecFlowLangParser.givens_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstGiven_return firstGiven69 = default(SpecFlowLangParser.firstGiven_return);

        SpecFlowLangParser.nextStep_return nextStep70 = default(SpecFlowLangParser.nextStep_return);


        RewriteRuleSubtreeStream stream_nextStep = new RewriteRuleSubtreeStream(adaptor,"rule nextStep");
        RewriteRuleSubtreeStream stream_firstGiven = new RewriteRuleSubtreeStream(adaptor,"rule firstGiven");
        try 
    	{
            // SpecFlowLangParser.g:139:5: ( firstGiven ( nextStep )* -> ^( STEPS firstGiven ( nextStep )* ) )
            // SpecFlowLangParser.g:139:9: firstGiven ( nextStep )*
            {
            	PushFollow(FOLLOW_firstGiven_in_givens1117);
            	firstGiven69 = firstGiven();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_firstGiven.Add(firstGiven69.Tree);
            	// SpecFlowLangParser.g:139:20: ( nextStep )*
            	do 
            	{
            	    int alt32 = 2;
            	    int LA32_0 = input.LA(1);

            	    if ( (LA32_0 == WS) )
            	    {
            	        int LA32_1 = input.LA(2);

            	        if ( ((LA32_1 >= T_GIVEN && LA32_1 <= T_BUT)) )
            	        {
            	            alt32 = 1;
            	        }


            	    }
            	    else if ( ((LA32_0 >= T_GIVEN && LA32_0 <= T_BUT)) )
            	    {
            	        alt32 = 1;
            	    }


            	    switch (alt32) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: nextStep
            			    {
            			    	PushFollow(FOLLOW_nextStep_in_givens1119);
            			    	nextStep70 = nextStep();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_nextStep.Add(nextStep70.Tree);

            			    }
            			    break;

            			default:
            			    goto loop32;
            	    }
            	} while (true);

            	loop32:
            		;	// Stops C# compiler whining that label 'loop32' has no statements



            	// AST REWRITE
            	// elements:          nextStep, firstGiven
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 140:9: -> ^( STEPS firstGiven ( nextStep )* )
            	{
            	    // SpecFlowLangParser.g:140:12: ^( STEPS firstGiven ( nextStep )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(STEPS, "STEPS"), root_1);

            	    adaptor.AddChild(root_1, stream_firstGiven.NextTree());
            	    // SpecFlowLangParser.g:140:31: ( nextStep )*
            	    while ( stream_nextStep.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_nextStep.NextTree());

            	    }
            	    stream_nextStep.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "givens"

    public class firstGiven_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "firstGiven"
    // SpecFlowLangParser.g:142:1: firstGiven : ( WS )? T_GIVEN WS sentenceEnd -> ^( GIVEN sentenceEnd ) ;
    public SpecFlowLangParser.firstGiven_return firstGiven() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstGiven_return retval = new SpecFlowLangParser.firstGiven_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS71 = null;
        IToken T_GIVEN72 = null;
        IToken WS73 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd74 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS71_tree=null;
        object T_GIVEN72_tree=null;
        object WS73_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_T_GIVEN = new RewriteRuleTokenStream(adaptor,"token T_GIVEN");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLangParser.g:143:5: ( ( WS )? T_GIVEN WS sentenceEnd -> ^( GIVEN sentenceEnd ) )
            // SpecFlowLangParser.g:143:9: ( WS )? T_GIVEN WS sentenceEnd
            {
            	// SpecFlowLangParser.g:143:9: ( WS )?
            	int alt33 = 2;
            	int LA33_0 = input.LA(1);

            	if ( (LA33_0 == WS) )
            	{
            	    alt33 = 1;
            	}
            	switch (alt33) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS71=(IToken)Match(input,WS,FOLLOW_WS_in_firstGiven1157); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS71);


            	        }
            	        break;

            	}

            	T_GIVEN72=(IToken)Match(input,T_GIVEN,FOLLOW_T_GIVEN_in_firstGiven1160); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_GIVEN.Add(T_GIVEN72);

            	WS73=(IToken)Match(input,WS,FOLLOW_WS_in_firstGiven1162); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS73);

            	PushFollow(FOLLOW_sentenceEnd_in_firstGiven1164);
            	sentenceEnd74 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd74.Tree);


            	// AST REWRITE
            	// elements:          sentenceEnd
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 144:9: -> ^( GIVEN sentenceEnd )
            	{
            	    // SpecFlowLangParser.g:144:12: ^( GIVEN sentenceEnd )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(GIVEN, "GIVEN"), root_1);

            	    adaptor.AddChild(root_1, stream_sentenceEnd.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "firstGiven"

    public class firstWhen_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "firstWhen"
    // SpecFlowLangParser.g:146:1: firstWhen : ( WS )? T_WHEN WS sentenceEnd -> ^( WHEN sentenceEnd ) ;
    public SpecFlowLangParser.firstWhen_return firstWhen() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstWhen_return retval = new SpecFlowLangParser.firstWhen_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS75 = null;
        IToken T_WHEN76 = null;
        IToken WS77 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd78 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS75_tree=null;
        object T_WHEN76_tree=null;
        object WS77_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_T_WHEN = new RewriteRuleTokenStream(adaptor,"token T_WHEN");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLangParser.g:147:5: ( ( WS )? T_WHEN WS sentenceEnd -> ^( WHEN sentenceEnd ) )
            // SpecFlowLangParser.g:147:9: ( WS )? T_WHEN WS sentenceEnd
            {
            	// SpecFlowLangParser.g:147:9: ( WS )?
            	int alt34 = 2;
            	int LA34_0 = input.LA(1);

            	if ( (LA34_0 == WS) )
            	{
            	    alt34 = 1;
            	}
            	switch (alt34) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS75=(IToken)Match(input,WS,FOLLOW_WS_in_firstWhen1198); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS75);


            	        }
            	        break;

            	}

            	T_WHEN76=(IToken)Match(input,T_WHEN,FOLLOW_T_WHEN_in_firstWhen1201); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_WHEN.Add(T_WHEN76);

            	WS77=(IToken)Match(input,WS,FOLLOW_WS_in_firstWhen1203); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS77);

            	PushFollow(FOLLOW_sentenceEnd_in_firstWhen1205);
            	sentenceEnd78 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd78.Tree);


            	// AST REWRITE
            	// elements:          sentenceEnd
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 148:9: -> ^( WHEN sentenceEnd )
            	{
            	    // SpecFlowLangParser.g:148:12: ^( WHEN sentenceEnd )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(WHEN, "WHEN"), root_1);

            	    adaptor.AddChild(root_1, stream_sentenceEnd.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "firstWhen"

    public class firstThen_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "firstThen"
    // SpecFlowLangParser.g:150:1: firstThen : ( WS )? T_THEN WS sentenceEnd -> ^( THEN sentenceEnd ) ;
    public SpecFlowLangParser.firstThen_return firstThen() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstThen_return retval = new SpecFlowLangParser.firstThen_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS79 = null;
        IToken T_THEN80 = null;
        IToken WS81 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd82 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS79_tree=null;
        object T_THEN80_tree=null;
        object WS81_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_T_THEN = new RewriteRuleTokenStream(adaptor,"token T_THEN");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLangParser.g:151:5: ( ( WS )? T_THEN WS sentenceEnd -> ^( THEN sentenceEnd ) )
            // SpecFlowLangParser.g:151:9: ( WS )? T_THEN WS sentenceEnd
            {
            	// SpecFlowLangParser.g:151:9: ( WS )?
            	int alt35 = 2;
            	int LA35_0 = input.LA(1);

            	if ( (LA35_0 == WS) )
            	{
            	    alt35 = 1;
            	}
            	switch (alt35) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS79=(IToken)Match(input,WS,FOLLOW_WS_in_firstThen1239); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS79);


            	        }
            	        break;

            	}

            	T_THEN80=(IToken)Match(input,T_THEN,FOLLOW_T_THEN_in_firstThen1242); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_THEN.Add(T_THEN80);

            	WS81=(IToken)Match(input,WS,FOLLOW_WS_in_firstThen1244); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS81);

            	PushFollow(FOLLOW_sentenceEnd_in_firstThen1246);
            	sentenceEnd82 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd82.Tree);


            	// AST REWRITE
            	// elements:          sentenceEnd
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 152:9: -> ^( THEN sentenceEnd )
            	{
            	    // SpecFlowLangParser.g:152:12: ^( THEN sentenceEnd )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(THEN, "THEN"), root_1);

            	    adaptor.AddChild(root_1, stream_sentenceEnd.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "firstThen"

    public class sentenceEnd_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "sentenceEnd"
    // SpecFlowLangParser.g:155:1: sentenceEnd : text newlineWithSpaces ( multilineText )? ( table )? -> text ( multilineText )? ( table )? ;
    public SpecFlowLangParser.sentenceEnd_return sentenceEnd() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.sentenceEnd_return retval = new SpecFlowLangParser.sentenceEnd_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.text_return text83 = default(SpecFlowLangParser.text_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces84 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.multilineText_return multilineText85 = default(SpecFlowLangParser.multilineText_return);

        SpecFlowLangParser.table_return table86 = default(SpecFlowLangParser.table_return);


        RewriteRuleSubtreeStream stream_multilineText = new RewriteRuleSubtreeStream(adaptor,"rule multilineText");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        RewriteRuleSubtreeStream stream_table = new RewriteRuleSubtreeStream(adaptor,"rule table");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:156:5: ( text newlineWithSpaces ( multilineText )? ( table )? -> text ( multilineText )? ( table )? )
            // SpecFlowLangParser.g:156:9: text newlineWithSpaces ( multilineText )? ( table )?
            {
            	PushFollow(FOLLOW_text_in_sentenceEnd1281);
            	text83 = text();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_text.Add(text83.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_sentenceEnd1283);
            	newlineWithSpaces84 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces84.Tree);
            	// SpecFlowLangParser.g:156:32: ( multilineText )?
            	int alt36 = 2;
            	int LA36_0 = input.LA(1);

            	if ( (LA36_0 == WS) )
            	{
            	    int LA36_1 = input.LA(2);

            	    if ( (LA36_1 == MLTEXT) )
            	    {
            	        alt36 = 1;
            	    }
            	}
            	else if ( (LA36_0 == MLTEXT) )
            	{
            	    alt36 = 1;
            	}
            	switch (alt36) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: multilineText
            	        {
            	        	PushFollow(FOLLOW_multilineText_in_sentenceEnd1285);
            	        	multilineText85 = multilineText();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_multilineText.Add(multilineText85.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:156:47: ( table )?
            	int alt37 = 2;
            	int LA37_0 = input.LA(1);

            	if ( (LA37_0 == WS) )
            	{
            	    int LA37_1 = input.LA(2);

            	    if ( (LA37_1 == CELLSEP) )
            	    {
            	        alt37 = 1;
            	    }
            	}
            	else if ( (LA37_0 == CELLSEP) )
            	{
            	    alt37 = 1;
            	}
            	switch (alt37) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: table
            	        {
            	        	PushFollow(FOLLOW_table_in_sentenceEnd1288);
            	        	table86 = table();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_table.Add(table86.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          text, multilineText, table
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 157:9: -> text ( multilineText )? ( table )?
            	{
            	    adaptor.AddChild(root_0, stream_text.NextTree());
            	    // SpecFlowLangParser.g:157:17: ( multilineText )?
            	    if ( stream_multilineText.HasNext() )
            	    {
            	        adaptor.AddChild(root_0, stream_multilineText.NextTree());

            	    }
            	    stream_multilineText.Reset();
            	    // SpecFlowLangParser.g:157:32: ( table )?
            	    if ( stream_table.HasNext() )
            	    {
            	        adaptor.AddChild(root_0, stream_table.NextTree());

            	    }
            	    stream_table.Reset();

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "sentenceEnd"

    public class multilineText_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "multilineText"
    // SpecFlowLangParser.g:160:1: multilineText : indent MLTEXT ( WS )? NEWLINE ( multilineTextLine )* ( WS )? MLTEXT ( WS )? newlineWithSpaces -> ^( MULTILINETEXT ( multilineTextLine )* indent ) ;
    public SpecFlowLangParser.multilineText_return multilineText() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.multilineText_return retval = new SpecFlowLangParser.multilineText_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken MLTEXT88 = null;
        IToken WS89 = null;
        IToken NEWLINE90 = null;
        IToken WS92 = null;
        IToken MLTEXT93 = null;
        IToken WS94 = null;
        SpecFlowLangParser.indent_return indent87 = default(SpecFlowLangParser.indent_return);

        SpecFlowLangParser.multilineTextLine_return multilineTextLine91 = default(SpecFlowLangParser.multilineTextLine_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces95 = default(SpecFlowLangParser.newlineWithSpaces_return);


        object MLTEXT88_tree=null;
        object WS89_tree=null;
        object NEWLINE90_tree=null;
        object WS92_tree=null;
        object MLTEXT93_tree=null;
        object WS94_tree=null;
        RewriteRuleTokenStream stream_MLTEXT = new RewriteRuleTokenStream(adaptor,"token MLTEXT");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_NEWLINE = new RewriteRuleTokenStream(adaptor,"token NEWLINE");
        RewriteRuleSubtreeStream stream_multilineTextLine = new RewriteRuleSubtreeStream(adaptor,"rule multilineTextLine");
        RewriteRuleSubtreeStream stream_indent = new RewriteRuleSubtreeStream(adaptor,"rule indent");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:161:5: ( indent MLTEXT ( WS )? NEWLINE ( multilineTextLine )* ( WS )? MLTEXT ( WS )? newlineWithSpaces -> ^( MULTILINETEXT ( multilineTextLine )* indent ) )
            // SpecFlowLangParser.g:161:9: indent MLTEXT ( WS )? NEWLINE ( multilineTextLine )* ( WS )? MLTEXT ( WS )? newlineWithSpaces
            {
            	PushFollow(FOLLOW_indent_in_multilineText1326);
            	indent87 = indent();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_indent.Add(indent87.Tree);
            	MLTEXT88=(IToken)Match(input,MLTEXT,FOLLOW_MLTEXT_in_multilineText1328); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_MLTEXT.Add(MLTEXT88);

            	// SpecFlowLangParser.g:161:23: ( WS )?
            	int alt38 = 2;
            	int LA38_0 = input.LA(1);

            	if ( (LA38_0 == WS) )
            	{
            	    alt38 = 1;
            	}
            	switch (alt38) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS89=(IToken)Match(input,WS,FOLLOW_WS_in_multilineText1330); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS89);


            	        }
            	        break;

            	}

            	NEWLINE90=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_multilineText1333); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_NEWLINE.Add(NEWLINE90);

            	// SpecFlowLangParser.g:162:9: ( multilineTextLine )*
            	do 
            	{
            	    int alt39 = 2;
            	    int LA39_0 = input.LA(1);

            	    if ( (LA39_0 == WS) )
            	    {
            	        int LA39_1 = input.LA(2);

            	        if ( (LA39_1 == AT || (LA39_1 >= NEWLINE && LA39_1 <= WORDCHAR)) )
            	        {
            	            alt39 = 1;
            	        }


            	    }
            	    else if ( (LA39_0 == AT || (LA39_0 >= NEWLINE && LA39_0 <= WORDCHAR)) )
            	    {
            	        alt39 = 1;
            	    }


            	    switch (alt39) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: multilineTextLine
            			    {
            			    	PushFollow(FOLLOW_multilineTextLine_in_multilineText1343);
            			    	multilineTextLine91 = multilineTextLine();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_multilineTextLine.Add(multilineTextLine91.Tree);

            			    }
            			    break;

            			default:
            			    goto loop39;
            	    }
            	} while (true);

            	loop39:
            		;	// Stops C# compiler whining that label 'loop39' has no statements

            	// SpecFlowLangParser.g:163:9: ( WS )?
            	int alt40 = 2;
            	int LA40_0 = input.LA(1);

            	if ( (LA40_0 == WS) )
            	{
            	    alt40 = 1;
            	}
            	switch (alt40) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS92=(IToken)Match(input,WS,FOLLOW_WS_in_multilineText1354); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS92);


            	        }
            	        break;

            	}

            	MLTEXT93=(IToken)Match(input,MLTEXT,FOLLOW_MLTEXT_in_multilineText1357); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_MLTEXT.Add(MLTEXT93);

            	// SpecFlowLangParser.g:163:20: ( WS )?
            	int alt41 = 2;
            	int LA41_0 = input.LA(1);

            	if ( (LA41_0 == WS) )
            	{
            	    int LA41_1 = input.LA(2);

            	    if ( (synpred43_SpecFlowLangParser()) )
            	    {
            	        alt41 = 1;
            	    }
            	}
            	switch (alt41) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS94=(IToken)Match(input,WS,FOLLOW_WS_in_multilineText1359); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS94);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_multilineText1362);
            	newlineWithSpaces95 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces95.Tree);


            	// AST REWRITE
            	// elements:          indent, multilineTextLine
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 164:9: -> ^( MULTILINETEXT ( multilineTextLine )* indent )
            	{
            	    // SpecFlowLangParser.g:164:12: ^( MULTILINETEXT ( multilineTextLine )* indent )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(MULTILINETEXT, "MULTILINETEXT"), root_1);

            	    // SpecFlowLangParser.g:164:28: ( multilineTextLine )*
            	    while ( stream_multilineTextLine.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_multilineTextLine.NextTree());

            	    }
            	    stream_multilineTextLine.Reset();
            	    adaptor.AddChild(root_1, stream_indent.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "multilineText"

    public class indent_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "indent"
    // SpecFlowLangParser.g:167:1: indent : ( WS )? -> ^( INDENT ( WS )? ) ;
    public SpecFlowLangParser.indent_return indent() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.indent_return retval = new SpecFlowLangParser.indent_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS96 = null;

        object WS96_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");

        try 
    	{
            // SpecFlowLangParser.g:168:5: ( ( WS )? -> ^( INDENT ( WS )? ) )
            // SpecFlowLangParser.g:168:9: ( WS )?
            {
            	// SpecFlowLangParser.g:168:9: ( WS )?
            	int alt42 = 2;
            	int LA42_0 = input.LA(1);

            	if ( (LA42_0 == WS) )
            	{
            	    alt42 = 1;
            	}
            	switch (alt42) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS96=(IToken)Match(input,WS,FOLLOW_WS_in_indent1400); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS96);


            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          WS
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 168:13: -> ^( INDENT ( WS )? )
            	{
            	    // SpecFlowLangParser.g:168:16: ^( INDENT ( WS )? )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(INDENT, "INDENT"), root_1);

            	    // SpecFlowLangParser.g:168:25: ( WS )?
            	    if ( stream_WS.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_WS.NextNode());

            	    }
            	    stream_WS.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "indent"

    public class multilineTextLine_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "multilineTextLine"
    // SpecFlowLangParser.g:171:1: multilineTextLine : ( WS )? ( text )? NEWLINE -> ^( LINE ( WS )? ( text )? NEWLINE ) ;
    public SpecFlowLangParser.multilineTextLine_return multilineTextLine() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.multilineTextLine_return retval = new SpecFlowLangParser.multilineTextLine_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS97 = null;
        IToken NEWLINE99 = null;
        SpecFlowLangParser.text_return text98 = default(SpecFlowLangParser.text_return);


        object WS97_tree=null;
        object NEWLINE99_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_NEWLINE = new RewriteRuleTokenStream(adaptor,"token NEWLINE");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        try 
    	{
            // SpecFlowLangParser.g:172:5: ( ( WS )? ( text )? NEWLINE -> ^( LINE ( WS )? ( text )? NEWLINE ) )
            // SpecFlowLangParser.g:172:9: ( WS )? ( text )? NEWLINE
            {
            	// SpecFlowLangParser.g:172:9: ( WS )?
            	int alt43 = 2;
            	int LA43_0 = input.LA(1);

            	if ( (LA43_0 == WS) )
            	{
            	    alt43 = 1;
            	}
            	switch (alt43) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS97=(IToken)Match(input,WS,FOLLOW_WS_in_multilineTextLine1429); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS97);


            	        }
            	        break;

            	}

            	// SpecFlowLangParser.g:172:13: ( text )?
            	int alt44 = 2;
            	int LA44_0 = input.LA(1);

            	if ( (LA44_0 == AT || LA44_0 == WORDCHAR) )
            	{
            	    alt44 = 1;
            	}
            	switch (alt44) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: text
            	        {
            	        	PushFollow(FOLLOW_text_in_multilineTextLine1432);
            	        	text98 = text();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_text.Add(text98.Tree);

            	        }
            	        break;

            	}

            	NEWLINE99=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_multilineTextLine1435); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_NEWLINE.Add(NEWLINE99);



            	// AST REWRITE
            	// elements:          NEWLINE, text, WS
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 173:9: -> ^( LINE ( WS )? ( text )? NEWLINE )
            	{
            	    // SpecFlowLangParser.g:173:12: ^( LINE ( WS )? ( text )? NEWLINE )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(LINE, "LINE"), root_1);

            	    // SpecFlowLangParser.g:173:19: ( WS )?
            	    if ( stream_WS.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_WS.NextNode());

            	    }
            	    stream_WS.Reset();
            	    // SpecFlowLangParser.g:173:23: ( text )?
            	    if ( stream_text.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_text.NextTree());

            	    }
            	    stream_text.Reset();
            	    adaptor.AddChild(root_1, stream_NEWLINE.NextNode());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "multilineTextLine"

    public class table_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "table"
    // SpecFlowLangParser.g:176:1: table : tableRow ( tableRow )+ -> ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) ) ;
    public SpecFlowLangParser.table_return table() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.table_return retval = new SpecFlowLangParser.table_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.tableRow_return tableRow100 = default(SpecFlowLangParser.tableRow_return);

        SpecFlowLangParser.tableRow_return tableRow101 = default(SpecFlowLangParser.tableRow_return);


        RewriteRuleSubtreeStream stream_tableRow = new RewriteRuleSubtreeStream(adaptor,"rule tableRow");
        try 
    	{
            // SpecFlowLangParser.g:177:5: ( tableRow ( tableRow )+ -> ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) ) )
            // SpecFlowLangParser.g:177:9: tableRow ( tableRow )+
            {
            	PushFollow(FOLLOW_tableRow_in_table1476);
            	tableRow100 = tableRow();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_tableRow.Add(tableRow100.Tree);
            	// SpecFlowLangParser.g:177:18: ( tableRow )+
            	int cnt45 = 0;
            	do 
            	{
            	    int alt45 = 2;
            	    int LA45_0 = input.LA(1);

            	    if ( (LA45_0 == WS) )
            	    {
            	        int LA45_1 = input.LA(2);

            	        if ( (LA45_1 == CELLSEP) )
            	        {
            	            alt45 = 1;
            	        }


            	    }
            	    else if ( (LA45_0 == CELLSEP) )
            	    {
            	        alt45 = 1;
            	    }


            	    switch (alt45) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: tableRow
            			    {
            			    	PushFollow(FOLLOW_tableRow_in_table1478);
            			    	tableRow101 = tableRow();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_tableRow.Add(tableRow101.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt45 >= 1 ) goto loop45;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee45 =
            		                new EarlyExitException(45, input);
            		            throw eee45;
            	    }
            	    cnt45++;
            	} while (true);

            	loop45:
            		;	// Stops C# compiler whinging that label 'loop45' has no statements



            	// AST REWRITE
            	// elements:          tableRow, tableRow
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 178:9: -> ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) )
            	{
            	    // SpecFlowLangParser.g:178:12: ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TABLE, "TABLE"), root_1);

            	    // SpecFlowLangParser.g:178:20: ^( HEADER tableRow )
            	    {
            	    object root_2 = (object)adaptor.GetNilNode();
            	    root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(HEADER, "HEADER"), root_2);

            	    adaptor.AddChild(root_2, stream_tableRow.NextTree());

            	    adaptor.AddChild(root_1, root_2);
            	    }
            	    // SpecFlowLangParser.g:178:39: ^( BODY ( tableRow )+ )
            	    {
            	    object root_2 = (object)adaptor.GetNilNode();
            	    root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(BODY, "BODY"), root_2);

            	    if ( !(stream_tableRow.HasNext()) ) {
            	        throw new RewriteEarlyExitException();
            	    }
            	    while ( stream_tableRow.HasNext() )
            	    {
            	        adaptor.AddChild(root_2, stream_tableRow.NextTree());

            	    }
            	    stream_tableRow.Reset();

            	    adaptor.AddChild(root_1, root_2);
            	    }

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "table"

    public class tableRow_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "tableRow"
    // SpecFlowLangParser.g:181:1: tableRow : ( WS )? CELLSEP ( tableCell )+ ( WS )? newlineWithSpaces -> ^( ROW ( tableCell )+ ) ;
    public SpecFlowLangParser.tableRow_return tableRow() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.tableRow_return retval = new SpecFlowLangParser.tableRow_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS102 = null;
        IToken CELLSEP103 = null;
        IToken WS105 = null;
        SpecFlowLangParser.tableCell_return tableCell104 = default(SpecFlowLangParser.tableCell_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces106 = default(SpecFlowLangParser.newlineWithSpaces_return);


        object WS102_tree=null;
        object CELLSEP103_tree=null;
        object WS105_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_CELLSEP = new RewriteRuleTokenStream(adaptor,"token CELLSEP");
        RewriteRuleSubtreeStream stream_tableCell = new RewriteRuleSubtreeStream(adaptor,"rule tableCell");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLangParser.g:182:5: ( ( WS )? CELLSEP ( tableCell )+ ( WS )? newlineWithSpaces -> ^( ROW ( tableCell )+ ) )
            // SpecFlowLangParser.g:182:9: ( WS )? CELLSEP ( tableCell )+ ( WS )? newlineWithSpaces
            {
            	// SpecFlowLangParser.g:182:9: ( WS )?
            	int alt46 = 2;
            	int LA46_0 = input.LA(1);

            	if ( (LA46_0 == WS) )
            	{
            	    alt46 = 1;
            	}
            	switch (alt46) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS102=(IToken)Match(input,WS,FOLLOW_WS_in_tableRow1525); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS102);


            	        }
            	        break;

            	}

            	CELLSEP103=(IToken)Match(input,CELLSEP,FOLLOW_CELLSEP_in_tableRow1528); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_CELLSEP.Add(CELLSEP103);

            	// SpecFlowLangParser.g:182:21: ( tableCell )+
            	int cnt47 = 0;
            	do 
            	{
            	    int alt47 = 2;
            	    int LA47_0 = input.LA(1);

            	    if ( (LA47_0 == WS) )
            	    {
            	        int LA47_1 = input.LA(2);

            	        if ( (LA47_1 == AT || LA47_1 == WORDCHAR) )
            	        {
            	            alt47 = 1;
            	        }


            	    }
            	    else if ( (LA47_0 == AT || LA47_0 == WORDCHAR) )
            	    {
            	        alt47 = 1;
            	    }


            	    switch (alt47) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: tableCell
            			    {
            			    	PushFollow(FOLLOW_tableCell_in_tableRow1530);
            			    	tableCell104 = tableCell();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_tableCell.Add(tableCell104.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt47 >= 1 ) goto loop47;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee47 =
            		                new EarlyExitException(47, input);
            		            throw eee47;
            	    }
            	    cnt47++;
            	} while (true);

            	loop47:
            		;	// Stops C# compiler whinging that label 'loop47' has no statements

            	// SpecFlowLangParser.g:182:32: ( WS )?
            	int alt48 = 2;
            	int LA48_0 = input.LA(1);

            	if ( (LA48_0 == WS) )
            	{
            	    int LA48_1 = input.LA(2);

            	    if ( (synpred50_SpecFlowLangParser()) )
            	    {
            	        alt48 = 1;
            	    }
            	}
            	switch (alt48) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS105=(IToken)Match(input,WS,FOLLOW_WS_in_tableRow1533); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS105);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_tableRow1536);
            	newlineWithSpaces106 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces106.Tree);


            	// AST REWRITE
            	// elements:          tableCell
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 183:9: -> ^( ROW ( tableCell )+ )
            	{
            	    // SpecFlowLangParser.g:183:12: ^( ROW ( tableCell )+ )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(ROW, "ROW"), root_1);

            	    if ( !(stream_tableCell.HasNext()) ) {
            	        throw new RewriteEarlyExitException();
            	    }
            	    while ( stream_tableCell.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_tableCell.NextTree());

            	    }
            	    stream_tableCell.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "tableRow"

    public class tableCell_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "tableCell"
    // SpecFlowLangParser.g:186:1: tableCell : ( WS )? text ( WS )? CELLSEP -> ^( CELL text ) ;
    public SpecFlowLangParser.tableCell_return tableCell() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.tableCell_return retval = new SpecFlowLangParser.tableCell_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS107 = null;
        IToken WS109 = null;
        IToken CELLSEP110 = null;
        SpecFlowLangParser.text_return text108 = default(SpecFlowLangParser.text_return);


        object WS107_tree=null;
        object WS109_tree=null;
        object CELLSEP110_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_CELLSEP = new RewriteRuleTokenStream(adaptor,"token CELLSEP");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        try 
    	{
            // SpecFlowLangParser.g:187:5: ( ( WS )? text ( WS )? CELLSEP -> ^( CELL text ) )
            // SpecFlowLangParser.g:187:9: ( WS )? text ( WS )? CELLSEP
            {
            	// SpecFlowLangParser.g:187:9: ( WS )?
            	int alt49 = 2;
            	int LA49_0 = input.LA(1);

            	if ( (LA49_0 == WS) )
            	{
            	    alt49 = 1;
            	}
            	switch (alt49) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS107=(IToken)Match(input,WS,FOLLOW_WS_in_tableCell1572); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS107);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_tableCell1575);
            	text108 = text();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_text.Add(text108.Tree);
            	// SpecFlowLangParser.g:187:18: ( WS )?
            	int alt50 = 2;
            	int LA50_0 = input.LA(1);

            	if ( (LA50_0 == WS) )
            	{
            	    alt50 = 1;
            	}
            	switch (alt50) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS109=(IToken)Match(input,WS,FOLLOW_WS_in_tableCell1577); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS109);


            	        }
            	        break;

            	}

            	CELLSEP110=(IToken)Match(input,CELLSEP,FOLLOW_CELLSEP_in_tableCell1580); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_CELLSEP.Add(CELLSEP110);



            	// AST REWRITE
            	// elements:          text
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 188:9: -> ^( CELL text )
            	{
            	    // SpecFlowLangParser.g:188:12: ^( CELL text )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(CELL, "CELL"), root_1);

            	    adaptor.AddChild(root_1, stream_text.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "tableCell"

    public class descriptionLineText_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "descriptionLineText"
    // SpecFlowLangParser.g:191:1: descriptionLineText : WORDCHAR ( textRest )* -> ^( TEXT WORDCHAR ( textRest )* ) ;
    public SpecFlowLangParser.descriptionLineText_return descriptionLineText() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.descriptionLineText_return retval = new SpecFlowLangParser.descriptionLineText_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WORDCHAR111 = null;
        SpecFlowLangParser.textRest_return textRest112 = default(SpecFlowLangParser.textRest_return);


        object WORDCHAR111_tree=null;
        RewriteRuleTokenStream stream_WORDCHAR = new RewriteRuleTokenStream(adaptor,"token WORDCHAR");
        RewriteRuleSubtreeStream stream_textRest = new RewriteRuleSubtreeStream(adaptor,"rule textRest");
        try 
    	{
            // SpecFlowLangParser.g:192:5: ( WORDCHAR ( textRest )* -> ^( TEXT WORDCHAR ( textRest )* ) )
            // SpecFlowLangParser.g:192:9: WORDCHAR ( textRest )*
            {
            	WORDCHAR111=(IToken)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_descriptionLineText1615); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WORDCHAR.Add(WORDCHAR111);

            	// SpecFlowLangParser.g:192:18: ( textRest )*
            	do 
            	{
            	    int alt51 = 2;
            	    int LA51_0 = input.LA(1);

            	    if ( (LA51_0 == WS) )
            	    {
            	        int LA51_1 = input.LA(2);

            	        if ( (LA51_1 == AT || LA51_1 == WS || LA51_1 == WORDCHAR) )
            	        {
            	            alt51 = 1;
            	        }


            	    }
            	    else if ( (LA51_0 == AT || LA51_0 == WORDCHAR) )
            	    {
            	        alt51 = 1;
            	    }


            	    switch (alt51) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: textRest
            			    {
            			    	PushFollow(FOLLOW_textRest_in_descriptionLineText1617);
            			    	textRest112 = textRest();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_textRest.Add(textRest112.Tree);

            			    }
            			    break;

            			default:
            			    goto loop51;
            	    }
            	} while (true);

            	loop51:
            		;	// Stops C# compiler whining that label 'loop51' has no statements



            	// AST REWRITE
            	// elements:          textRest, WORDCHAR
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 193:9: -> ^( TEXT WORDCHAR ( textRest )* )
            	{
            	    // SpecFlowLangParser.g:193:12: ^( TEXT WORDCHAR ( textRest )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TEXT, "TEXT"), root_1);

            	    adaptor.AddChild(root_1, stream_WORDCHAR.NextNode());
            	    // SpecFlowLangParser.g:193:28: ( textRest )*
            	    while ( stream_textRest.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_textRest.NextTree());

            	    }
            	    stream_textRest.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "descriptionLineText"

    public class text_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "text"
    // SpecFlowLangParser.g:196:1: text : wordchar ( textRest )* -> ^( TEXT wordchar ( textRest )* ) ;
    public SpecFlowLangParser.text_return text() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.text_return retval = new SpecFlowLangParser.text_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.wordchar_return wordchar113 = default(SpecFlowLangParser.wordchar_return);

        SpecFlowLangParser.textRest_return textRest114 = default(SpecFlowLangParser.textRest_return);


        RewriteRuleSubtreeStream stream_textRest = new RewriteRuleSubtreeStream(adaptor,"rule textRest");
        RewriteRuleSubtreeStream stream_wordchar = new RewriteRuleSubtreeStream(adaptor,"rule wordchar");
        try 
    	{
            // SpecFlowLangParser.g:197:5: ( wordchar ( textRest )* -> ^( TEXT wordchar ( textRest )* ) )
            // SpecFlowLangParser.g:197:9: wordchar ( textRest )*
            {
            	PushFollow(FOLLOW_wordchar_in_text1656);
            	wordchar113 = wordchar();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_wordchar.Add(wordchar113.Tree);
            	// SpecFlowLangParser.g:197:18: ( textRest )*
            	do 
            	{
            	    int alt52 = 2;
            	    int LA52_0 = input.LA(1);

            	    if ( (LA52_0 == WS) )
            	    {
            	        int LA52_1 = input.LA(2);

            	        if ( (LA52_1 == AT || LA52_1 == WS || LA52_1 == WORDCHAR) )
            	        {
            	            alt52 = 1;
            	        }


            	    }
            	    else if ( (LA52_0 == AT || LA52_0 == WORDCHAR) )
            	    {
            	        alt52 = 1;
            	    }


            	    switch (alt52) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: textRest
            			    {
            			    	PushFollow(FOLLOW_textRest_in_text1658);
            			    	textRest114 = textRest();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_textRest.Add(textRest114.Tree);

            			    }
            			    break;

            			default:
            			    goto loop52;
            	    }
            	} while (true);

            	loop52:
            		;	// Stops C# compiler whining that label 'loop52' has no statements



            	// AST REWRITE
            	// elements:          textRest, wordchar
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 198:9: -> ^( TEXT wordchar ( textRest )* )
            	{
            	    // SpecFlowLangParser.g:198:12: ^( TEXT wordchar ( textRest )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TEXT, "TEXT"), root_1);

            	    adaptor.AddChild(root_1, stream_wordchar.NextTree());
            	    // SpecFlowLangParser.g:198:28: ( textRest )*
            	    while ( stream_textRest.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_textRest.NextTree());

            	    }
            	    stream_textRest.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "text"

    public class textRest_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "textRest"
    // SpecFlowLangParser.g:200:1: textRest : ( WS textRest | wordchar );
    public SpecFlowLangParser.textRest_return textRest() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.textRest_return retval = new SpecFlowLangParser.textRest_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS115 = null;
        SpecFlowLangParser.textRest_return textRest116 = default(SpecFlowLangParser.textRest_return);

        SpecFlowLangParser.wordchar_return wordchar117 = default(SpecFlowLangParser.wordchar_return);


        object WS115_tree=null;

        try 
    	{
            // SpecFlowLangParser.g:201:5: ( WS textRest | wordchar )
            int alt53 = 2;
            int LA53_0 = input.LA(1);

            if ( (LA53_0 == WS) )
            {
                alt53 = 1;
            }
            else if ( (LA53_0 == AT || LA53_0 == WORDCHAR) )
            {
                alt53 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                NoViableAltException nvae_d53s0 =
                    new NoViableAltException("", 53, 0, input);

                throw nvae_d53s0;
            }
            switch (alt53) 
            {
                case 1 :
                    // SpecFlowLangParser.g:201:9: WS textRest
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WS115=(IToken)Match(input,WS,FOLLOW_WS_in_textRest1696); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WS115_tree = (object)adaptor.Create(WS115);
                    		adaptor.AddChild(root_0, WS115_tree);
                    	}
                    	PushFollow(FOLLOW_textRest_in_textRest1698);
                    	textRest116 = textRest();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, textRest116.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLangParser.g:202:9: wordchar
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_wordchar_in_textRest1708);
                    	wordchar117 = wordchar();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, wordchar117.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "textRest"

    public class title_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "title"
    // SpecFlowLangParser.g:205:1: title : wordchar ( titleRest )* -> ^( TEXT wordchar ( titleRest )* ) ;
    public SpecFlowLangParser.title_return title() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.title_return retval = new SpecFlowLangParser.title_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.wordchar_return wordchar118 = default(SpecFlowLangParser.wordchar_return);

        SpecFlowLangParser.titleRest_return titleRest119 = default(SpecFlowLangParser.titleRest_return);


        RewriteRuleSubtreeStream stream_wordchar = new RewriteRuleSubtreeStream(adaptor,"rule wordchar");
        RewriteRuleSubtreeStream stream_titleRest = new RewriteRuleSubtreeStream(adaptor,"rule titleRest");
        try 
    	{
            // SpecFlowLangParser.g:206:5: ( wordchar ( titleRest )* -> ^( TEXT wordchar ( titleRest )* ) )
            // SpecFlowLangParser.g:206:9: wordchar ( titleRest )*
            {
            	PushFollow(FOLLOW_wordchar_in_title1727);
            	wordchar118 = wordchar();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_wordchar.Add(wordchar118.Tree);
            	// SpecFlowLangParser.g:206:18: ( titleRest )*
            	do 
            	{
            	    int alt54 = 2;
            	    alt54 = dfa54.Predict(input);
            	    switch (alt54) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:0:0: titleRest
            			    {
            			    	PushFollow(FOLLOW_titleRest_in_title1729);
            			    	titleRest119 = titleRest();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_titleRest.Add(titleRest119.Tree);

            			    }
            			    break;

            			default:
            			    goto loop54;
            	    }
            	} while (true);

            	loop54:
            		;	// Stops C# compiler whining that label 'loop54' has no statements



            	// AST REWRITE
            	// elements:          wordchar, titleRest
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 207:9: -> ^( TEXT wordchar ( titleRest )* )
            	{
            	    // SpecFlowLangParser.g:207:12: ^( TEXT wordchar ( titleRest )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TEXT, "TEXT"), root_1);

            	    adaptor.AddChild(root_1, stream_wordchar.NextTree());
            	    // SpecFlowLangParser.g:207:28: ( titleRest )*
            	    while ( stream_titleRest.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_titleRest.NextTree());

            	    }
            	    stream_titleRest.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;}
            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "title"

    public class titleRest_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "titleRest"
    // SpecFlowLangParser.g:210:1: titleRest : ( WS titleRest | NEWLINE titleRest | wordchar );
    public SpecFlowLangParser.titleRest_return titleRest() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.titleRest_return retval = new SpecFlowLangParser.titleRest_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS120 = null;
        IToken NEWLINE122 = null;
        SpecFlowLangParser.titleRest_return titleRest121 = default(SpecFlowLangParser.titleRest_return);

        SpecFlowLangParser.titleRest_return titleRest123 = default(SpecFlowLangParser.titleRest_return);

        SpecFlowLangParser.wordchar_return wordchar124 = default(SpecFlowLangParser.wordchar_return);


        object WS120_tree=null;
        object NEWLINE122_tree=null;

        try 
    	{
            // SpecFlowLangParser.g:211:5: ( WS titleRest | NEWLINE titleRest | wordchar )
            int alt55 = 3;
            switch ( input.LA(1) ) 
            {
            case WS:
            	{
                alt55 = 1;
                }
                break;
            case NEWLINE:
            	{
                alt55 = 2;
                }
                break;
            case AT:
            case WORDCHAR:
            	{
                alt55 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d55s0 =
            	        new NoViableAltException("", 55, 0, input);

            	    throw nvae_d55s0;
            }

            switch (alt55) 
            {
                case 1 :
                    // SpecFlowLangParser.g:211:9: WS titleRest
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WS120=(IToken)Match(input,WS,FOLLOW_WS_in_titleRest1768); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WS120_tree = (object)adaptor.Create(WS120);
                    		adaptor.AddChild(root_0, WS120_tree);
                    	}
                    	PushFollow(FOLLOW_titleRest_in_titleRest1770);
                    	titleRest121 = titleRest();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRest121.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLangParser.g:212:9: NEWLINE titleRest
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	NEWLINE122=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_titleRest1780); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{NEWLINE122_tree = (object)adaptor.Create(NEWLINE122);
                    		adaptor.AddChild(root_0, NEWLINE122_tree);
                    	}
                    	PushFollow(FOLLOW_titleRest_in_titleRest1782);
                    	titleRest123 = titleRest();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRest123.Tree);

                    }
                    break;
                case 3 :
                    // SpecFlowLangParser.g:213:9: wordchar
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_wordchar_in_titleRest1792);
                    	wordchar124 = wordchar();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, wordchar124.Tree);

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "titleRest"

    public class titleRestLine_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "titleRestLine"
    // SpecFlowLangParser.g:216:1: titleRestLine : ( NEWLINE titleRestLine | WS titleRestLine | WORDCHAR );
    public SpecFlowLangParser.titleRestLine_return titleRestLine() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.titleRestLine_return retval = new SpecFlowLangParser.titleRestLine_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken NEWLINE125 = null;
        IToken WS127 = null;
        IToken WORDCHAR129 = null;
        SpecFlowLangParser.titleRestLine_return titleRestLine126 = default(SpecFlowLangParser.titleRestLine_return);

        SpecFlowLangParser.titleRestLine_return titleRestLine128 = default(SpecFlowLangParser.titleRestLine_return);


        object NEWLINE125_tree=null;
        object WS127_tree=null;
        object WORDCHAR129_tree=null;

        try 
    	{
            // SpecFlowLangParser.g:217:5: ( NEWLINE titleRestLine | WS titleRestLine | WORDCHAR )
            int alt56 = 3;
            switch ( input.LA(1) ) 
            {
            case NEWLINE:
            	{
                alt56 = 1;
                }
                break;
            case WS:
            	{
                alt56 = 2;
                }
                break;
            case WORDCHAR:
            	{
                alt56 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d56s0 =
            	        new NoViableAltException("", 56, 0, input);

            	    throw nvae_d56s0;
            }

            switch (alt56) 
            {
                case 1 :
                    // SpecFlowLangParser.g:217:9: NEWLINE titleRestLine
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	NEWLINE125=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_titleRestLine1811); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{NEWLINE125_tree = (object)adaptor.Create(NEWLINE125);
                    		adaptor.AddChild(root_0, NEWLINE125_tree);
                    	}
                    	PushFollow(FOLLOW_titleRestLine_in_titleRestLine1813);
                    	titleRestLine126 = titleRestLine();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRestLine126.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLangParser.g:218:9: WS titleRestLine
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WS127=(IToken)Match(input,WS,FOLLOW_WS_in_titleRestLine1823); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WS127_tree = (object)adaptor.Create(WS127);
                    		adaptor.AddChild(root_0, WS127_tree);
                    	}
                    	PushFollow(FOLLOW_titleRestLine_in_titleRestLine1825);
                    	titleRestLine128 = titleRestLine();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRestLine128.Tree);

                    }
                    break;
                case 3 :
                    // SpecFlowLangParser.g:219:9: WORDCHAR
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WORDCHAR129=(IToken)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_titleRestLine1835); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WORDCHAR129_tree = (object)adaptor.Create(WORDCHAR129);
                    		adaptor.AddChild(root_0, WORDCHAR129_tree);
                    	}

                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "titleRestLine"

    public class wordchar_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "wordchar"
    // SpecFlowLangParser.g:222:1: wordchar : ( WORDCHAR | AT );
    public SpecFlowLangParser.wordchar_return wordchar() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.wordchar_return retval = new SpecFlowLangParser.wordchar_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken set130 = null;

        object set130_tree=null;

        try 
    	{
            // SpecFlowLangParser.g:223:5: ( WORDCHAR | AT )
            // SpecFlowLangParser.g:
            {
            	root_0 = (object)adaptor.GetNilNode();

            	set130 = (IToken)input.LT(1);
            	if ( input.LA(1) == AT || input.LA(1) == WORDCHAR ) 
            	{
            	    input.Consume();
            	    if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (object)adaptor.Create(set130));
            	    state.errorRecovery = false;state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}


            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "wordchar"

    public class newlineWithSpaces_return : ParserRuleReturnScope
    {
        private object tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (object) value; }
        }
    };

    // $ANTLR start "newlineWithSpaces"
    // SpecFlowLangParser.g:227:1: newlineWithSpaces : ( WS )? NEWLINE ( ( WS )? NEWLINE )* ;
    public SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.newlineWithSpaces_return retval = new SpecFlowLangParser.newlineWithSpaces_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS131 = null;
        IToken NEWLINE132 = null;
        IToken WS133 = null;
        IToken NEWLINE134 = null;

        object WS131_tree=null;
        object NEWLINE132_tree=null;
        object WS133_tree=null;
        object NEWLINE134_tree=null;

        try 
    	{
            // SpecFlowLangParser.g:228:5: ( ( WS )? NEWLINE ( ( WS )? NEWLINE )* )
            // SpecFlowLangParser.g:228:9: ( WS )? NEWLINE ( ( WS )? NEWLINE )*
            {
            	root_0 = (object)adaptor.GetNilNode();

            	// SpecFlowLangParser.g:228:9: ( WS )?
            	int alt57 = 2;
            	int LA57_0 = input.LA(1);

            	if ( (LA57_0 == WS) )
            	{
            	    alt57 = 1;
            	}
            	switch (alt57) 
            	{
            	    case 1 :
            	        // SpecFlowLangParser.g:0:0: WS
            	        {
            	        	WS131=(IToken)Match(input,WS,FOLLOW_WS_in_newlineWithSpaces1883); if (state.failed) return retval;
            	        	if ( state.backtracking == 0 )
            	        	{WS131_tree = (object)adaptor.Create(WS131);
            	        		adaptor.AddChild(root_0, WS131_tree);
            	        	}

            	        }
            	        break;

            	}

            	NEWLINE132=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_newlineWithSpaces1886); if (state.failed) return retval;
            	if ( state.backtracking == 0 )
            	{NEWLINE132_tree = (object)adaptor.Create(NEWLINE132);
            		adaptor.AddChild(root_0, NEWLINE132_tree);
            	}
            	// SpecFlowLangParser.g:228:21: ( ( WS )? NEWLINE )*
            	do 
            	{
            	    int alt59 = 2;
            	    int LA59_0 = input.LA(1);

            	    if ( (LA59_0 == WS) )
            	    {
            	        int LA59_1 = input.LA(2);

            	        if ( (LA59_1 == NEWLINE) )
            	        {
            	            alt59 = 1;
            	        }


            	    }
            	    else if ( (LA59_0 == NEWLINE) )
            	    {
            	        alt59 = 1;
            	    }


            	    switch (alt59) 
            		{
            			case 1 :
            			    // SpecFlowLangParser.g:228:22: ( WS )? NEWLINE
            			    {
            			    	// SpecFlowLangParser.g:228:22: ( WS )?
            			    	int alt58 = 2;
            			    	int LA58_0 = input.LA(1);

            			    	if ( (LA58_0 == WS) )
            			    	{
            			    	    alt58 = 1;
            			    	}
            			    	switch (alt58) 
            			    	{
            			    	    case 1 :
            			    	        // SpecFlowLangParser.g:0:0: WS
            			    	        {
            			    	        	WS133=(IToken)Match(input,WS,FOLLOW_WS_in_newlineWithSpaces1889); if (state.failed) return retval;
            			    	        	if ( state.backtracking == 0 )
            			    	        	{WS133_tree = (object)adaptor.Create(WS133);
            			    	        		adaptor.AddChild(root_0, WS133_tree);
            			    	        	}

            			    	        }
            			    	        break;

            			    	}

            			    	NEWLINE134=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_newlineWithSpaces1892); if (state.failed) return retval;
            			    	if ( state.backtracking == 0 )
            			    	{NEWLINE134_tree = (object)adaptor.Create(NEWLINE134);
            			    		adaptor.AddChild(root_0, NEWLINE134_tree);
            			    	}

            			    }
            			    break;

            			default:
            			    goto loop59;
            	    }
            	} while (true);

            	loop59:
            		;	// Stops C# compiler whining that label 'loop59' has no statements


            }

            retval.Stop = input.LT(-1);

            if ( (state.backtracking==0) )
            {	retval.Tree = (object)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);}
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (object)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "newlineWithSpaces"

    // $ANTLR start "synpred25_SpecFlowLangParser"
    public void synpred25_SpecFlowLangParser_fragment() {
        // SpecFlowLangParser.g:108:24: ( WS )
        // SpecFlowLangParser.g:108:24: WS
        {
        	Match(input,WS,FOLLOW_WS_in_synpred25_SpecFlowLangParser863); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred25_SpecFlowLangParser"

    // $ANTLR start "synpred43_SpecFlowLangParser"
    public void synpred43_SpecFlowLangParser_fragment() {
        // SpecFlowLangParser.g:163:20: ( WS )
        // SpecFlowLangParser.g:163:20: WS
        {
        	Match(input,WS,FOLLOW_WS_in_synpred43_SpecFlowLangParser1359); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred43_SpecFlowLangParser"

    // $ANTLR start "synpred50_SpecFlowLangParser"
    public void synpred50_SpecFlowLangParser_fragment() {
        // SpecFlowLangParser.g:182:32: ( WS )
        // SpecFlowLangParser.g:182:32: WS
        {
        	Match(input,WS,FOLLOW_WS_in_synpred50_SpecFlowLangParser1533); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred50_SpecFlowLangParser"

    // $ANTLR start "synpred56_SpecFlowLangParser"
    public void synpred56_SpecFlowLangParser_fragment() {
        // SpecFlowLangParser.g:206:18: ( titleRest )
        // SpecFlowLangParser.g:206:18: titleRest
        {
        	PushFollow(FOLLOW_titleRest_in_synpred56_SpecFlowLangParser1729);
        	titleRest();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred56_SpecFlowLangParser"

    // Delegated rules

   	public bool synpred56_SpecFlowLangParser() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred56_SpecFlowLangParser_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred43_SpecFlowLangParser() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred43_SpecFlowLangParser_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred50_SpecFlowLangParser() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred50_SpecFlowLangParser_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred25_SpecFlowLangParser() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred25_SpecFlowLangParser_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}


   	protected DFA16 dfa16;
   	protected DFA54 dfa54;
	private void InitializeCyclicDFAs()
	{
    	this.dfa16 = new DFA16(this);
    	this.dfa54 = new DFA54(this);

	    this.dfa54.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA54_SpecialStateTransition);
	}

    const string DFA16_eotS =
        "\x0b\uffff";
    const string DFA16_eofS =
        "\x0b\uffff";
    const string DFA16_minS =
        "\x02\x0a\x01\x18\x02\uffff\x01\x16\x05\x0a";
    const string DFA16_maxS =
        "\x01\x16\x01\x14\x01\x18\x02\uffff\x01\x18\x02\x17\x01\x0b\x02"+
        "\x17";
    const string DFA16_acceptS =
        "\x03\uffff\x01\x01\x01\x02\x06\uffff";
    const string DFA16_specialS =
        "\x0b\uffff}>";
    static readonly string[] DFA16_transitionS = {
            "\x01\x04\x01\x03\x08\uffff\x01\x02\x01\uffff\x01\x01",
            "\x01\x04\x01\x03\x08\uffff\x01\x02",
            "\x01\x05",
            "",
            "",
            "\x01\x06\x01\x07\x01\x05",
            "\x01\x04\x01\x03\x08\uffff\x01\x02\x01\uffff\x01\x08\x01\x07",
            "\x01\x04\x01\x03\x08\uffff\x01\x02\x01\uffff\x01\x09\x01\x0a",
            "\x01\x04\x01\x03",
            "\x01\x04\x01\x03\x0b\uffff\x01\x0a",
            "\x01\x04\x01\x03\x08\uffff\x01\x02\x01\uffff\x01\x09\x01\x0a"
    };

    static readonly short[] DFA16_eot = DFA.UnpackEncodedString(DFA16_eotS);
    static readonly short[] DFA16_eof = DFA.UnpackEncodedString(DFA16_eofS);
    static readonly char[] DFA16_min = DFA.UnpackEncodedStringToUnsignedChars(DFA16_minS);
    static readonly char[] DFA16_max = DFA.UnpackEncodedStringToUnsignedChars(DFA16_maxS);
    static readonly short[] DFA16_accept = DFA.UnpackEncodedString(DFA16_acceptS);
    static readonly short[] DFA16_special = DFA.UnpackEncodedString(DFA16_specialS);
    static readonly short[][] DFA16_transition = DFA.UnpackEncodedStringArray(DFA16_transitionS);

    protected class DFA16 : DFA
    {
        public DFA16(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 16;
            this.eot = DFA16_eot;
            this.eof = DFA16_eof;
            this.min = DFA16_min;
            this.max = DFA16_max;
            this.accept = DFA16_accept;
            this.special = DFA16_special;
            this.transition = DFA16_transition;

        }

        override public string Description
        {
            get { return "81:1: scenarioKind : ( scenarioOutline | scenario );"; }
        }

    }

    const string DFA54_eotS =
        "\x5f\uffff";
    const string DFA54_eofS =
        "\x01\x03\x5e\uffff";
    const string DFA54_minS =
        "\x02\x14\x01\x0d\x02\uffff\x1c\x0d\x01\x00\x07\x0d\x01\x00\x04"+
        "\x0d\x01\x00\x02\x0d\x01\x00\x01\x0d\x02\x00\x04\x0d\x01\x00\x02"+
        "\x0d\x01\x00\x01\x0d\x02\x00\x02\x0d\x01\x00\x01\x0d\x02\x00\x01"+
        "\x0d\x03\x00\x02\x0d\x01\x00\x01\x0d\x02\x00\x01\x0d\x03\x00\x01"+
        "\x0d\x0a\x00";
    const string DFA54_maxS =
        "\x03\x18\x02\uffff\x1c\x18\x01\x00\x07\x18\x01\x00\x04\x18\x01"+
        "\x00\x02\x18\x01\x00\x01\x18\x02\x00\x04\x18\x01\x00\x02\x18\x01"+
        "\x00\x01\x18\x02\x00\x02\x18\x01\x00\x01\x18\x02\x00\x01\x18\x03"+
        "\x00\x02\x18\x01\x00\x01\x18\x02\x00\x01\x18\x03\x00\x01\x18\x0a"+
        "\x00";
    const string DFA54_acceptS =
        "\x03\uffff\x01\x02\x01\x01\x5a\uffff";
    const string DFA54_specialS =
        "\x21\uffff\x01\x1c\x07\uffff\x01\x1d\x04\uffff\x01\x1e\x02\uffff"+
        "\x01\x1b\x01\uffff\x01\x1a\x01\x19\x04\uffff\x01\x18\x02\uffff\x01"+
        "\x17\x01\uffff\x01\x16\x01\x15\x02\uffff\x01\x14\x01\uffff\x01\x13"+
        "\x01\x12\x01\uffff\x01\x11\x01\x10\x01\x0f\x02\uffff\x01\x0e\x01"+
        "\uffff\x01\x0d\x01\x0c\x01\uffff\x01\x0b\x01\x0a\x01\x09\x01\uffff"+
        "\x01\x08\x01\x07\x01\x06\x01\x05\x01\x1f\x01\x04\x01\x03\x01\x02"+
        "\x01\x01\x01\x00}>";
    static readonly string[] DFA54_transitionS = {
            "\x01\x04\x01\uffff\x01\x01\x01\x02\x01\x04",
            "\x01\x04\x01\uffff\x01\x04\x01\x05\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x06\x01\x07\x01\x04",
            "",
            "",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x08\x01\x09\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x0a\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x0b\x01\x0c\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x0d\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x0e\x01\x0f\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x10\x01\x11\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x12\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x13\x01\x14\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x15\x01\x16\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x17\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x18\x01\x19\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x1a\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x1b\x01\x1c\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x1d\x01\x1e\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x1f\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x20\x01\x21\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x22\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x23\x01\x24\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x25\x01\x26\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x27\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x28\x01\x29\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x2a\x01\x2b\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x2c\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x2d\x01\x2e\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x2f\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x30\x01\x31\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x32\x01\x33\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x34\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x35\x01\x36\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x37\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x38\x01\x39\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x3a\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x3b\x01\x3c\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x3d\x01\x3e\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x3f\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x40\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x41\x01\x42\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x43\x01\x44\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x45\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x46\x01\x47\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x48\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x49\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x4a\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x4b\x01\x4c\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x4d\x01\x4e\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x4f\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x50\x01\x51\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x52\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x53\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x54\x01\x55\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x56\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x57\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x58\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x59\x01\x5a\x01\x04",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x5b\x01\x04",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x5c\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x5d\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x03\x04\uffff\x01\x04\x01\uffff\x01\x04\x01\x5e\x01\x04",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff"
    };

    static readonly short[] DFA54_eot = DFA.UnpackEncodedString(DFA54_eotS);
    static readonly short[] DFA54_eof = DFA.UnpackEncodedString(DFA54_eofS);
    static readonly char[] DFA54_min = DFA.UnpackEncodedStringToUnsignedChars(DFA54_minS);
    static readonly char[] DFA54_max = DFA.UnpackEncodedStringToUnsignedChars(DFA54_maxS);
    static readonly short[] DFA54_accept = DFA.UnpackEncodedString(DFA54_acceptS);
    static readonly short[] DFA54_special = DFA.UnpackEncodedString(DFA54_specialS);
    static readonly short[][] DFA54_transition = DFA.UnpackEncodedStringArray(DFA54_transitionS);

    protected class DFA54 : DFA
    {
        public DFA54(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 54;
            this.eot = DFA54_eot;
            this.eof = DFA54_eof;
            this.min = DFA54_min;
            this.max = DFA54_max;
            this.accept = DFA54_accept;
            this.special = DFA54_special;
            this.transition = DFA54_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 206:18: ( titleRest )*"; }
        }

    }


    protected internal int DFA54_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITokenStream input = (ITokenStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA54_94 = input.LA(1);

                   	 
                   	int index54_94 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_94);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA54_93 = input.LA(1);

                   	 
                   	int index54_93 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_93);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA54_92 = input.LA(1);

                   	 
                   	int index54_92 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_92);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA54_91 = input.LA(1);

                   	 
                   	int index54_91 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_91);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA54_90 = input.LA(1);

                   	 
                   	int index54_90 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_90);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA54_88 = input.LA(1);

                   	 
                   	int index54_88 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_88);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA54_87 = input.LA(1);

                   	 
                   	int index54_87 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_87);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 7 : 
                   	int LA54_86 = input.LA(1);

                   	 
                   	int index54_86 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_86);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 8 : 
                   	int LA54_85 = input.LA(1);

                   	 
                   	int index54_85 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_85);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 9 : 
                   	int LA54_83 = input.LA(1);

                   	 
                   	int index54_83 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_83);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 10 : 
                   	int LA54_82 = input.LA(1);

                   	 
                   	int index54_82 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_82);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 11 : 
                   	int LA54_81 = input.LA(1);

                   	 
                   	int index54_81 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_81);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 12 : 
                   	int LA54_79 = input.LA(1);

                   	 
                   	int index54_79 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_79);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 13 : 
                   	int LA54_78 = input.LA(1);

                   	 
                   	int index54_78 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_78);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 14 : 
                   	int LA54_76 = input.LA(1);

                   	 
                   	int index54_76 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_76);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 15 : 
                   	int LA54_73 = input.LA(1);

                   	 
                   	int index54_73 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_73);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 16 : 
                   	int LA54_72 = input.LA(1);

                   	 
                   	int index54_72 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_72);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 17 : 
                   	int LA54_71 = input.LA(1);

                   	 
                   	int index54_71 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_71);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 18 : 
                   	int LA54_69 = input.LA(1);

                   	 
                   	int index54_69 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_69);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 19 : 
                   	int LA54_68 = input.LA(1);

                   	 
                   	int index54_68 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_68);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 20 : 
                   	int LA54_66 = input.LA(1);

                   	 
                   	int index54_66 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_66);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 21 : 
                   	int LA54_63 = input.LA(1);

                   	 
                   	int index54_63 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_63);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 22 : 
                   	int LA54_62 = input.LA(1);

                   	 
                   	int index54_62 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_62);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 23 : 
                   	int LA54_60 = input.LA(1);

                   	 
                   	int index54_60 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_60);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 24 : 
                   	int LA54_57 = input.LA(1);

                   	 
                   	int index54_57 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_57);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 25 : 
                   	int LA54_52 = input.LA(1);

                   	 
                   	int index54_52 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_52);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 26 : 
                   	int LA54_51 = input.LA(1);

                   	 
                   	int index54_51 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_51);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 27 : 
                   	int LA54_49 = input.LA(1);

                   	 
                   	int index54_49 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_49);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 28 : 
                   	int LA54_33 = input.LA(1);

                   	 
                   	int index54_33 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_33);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 29 : 
                   	int LA54_41 = input.LA(1);

                   	 
                   	int index54_41 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_41);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 30 : 
                   	int LA54_46 = input.LA(1);

                   	 
                   	int index54_46 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_46);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 31 : 
                   	int LA54_89 = input.LA(1);

                   	 
                   	int index54_89 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred56_SpecFlowLangParser()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index54_89);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae54 =
            new NoViableAltException(dfa.Description, 54, _s, input);
        dfa.Error(nvae54);
        throw nvae54;
    }
 

    public static readonly BitSet FOLLOW_newlineWithSpaces_in_feature252 = new BitSet(new ulong[]{0x0000000000500100UL});
    public static readonly BitSet FOLLOW_tags_in_feature263 = new BitSet(new ulong[]{0x0000000000400100UL});
    public static readonly BitSet FOLLOW_WS_in_feature274 = new BitSet(new ulong[]{0x0000000000000100UL});
    public static readonly BitSet FOLLOW_T_FEATURE_in_feature277 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_WS_in_feature279 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_text_in_feature282 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_feature284 = new BitSet(new ulong[]{0x0000000001500E00UL});
    public static readonly BitSet FOLLOW_descriptionLine_in_feature294 = new BitSet(new ulong[]{0x0000000001500E00UL});
    public static readonly BitSet FOLLOW_background_in_feature305 = new BitSet(new ulong[]{0x0000000000500C00UL});
    public static readonly BitSet FOLLOW_scenarioKind_in_feature316 = new BitSet(new ulong[]{0x0000000000500C00UL});
    public static readonly BitSet FOLLOW_WS_in_feature319 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_EOF_in_feature322 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_tags399 = new BitSet(new ulong[]{0x0000000000500000UL});
    public static readonly BitSet FOLLOW_tag_in_tags402 = new BitSet(new ulong[]{0x0000000000500002UL});
    public static readonly BitSet FOLLOW_AT_in_tag439 = new BitSet(new ulong[]{0x0000000001000000UL});
    public static readonly BitSet FOLLOW_word_in_tag441 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_tag444 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_tag446 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_word482 = new BitSet(new ulong[]{0x0000000001000002UL});
    public static readonly BitSet FOLLOW_WS_in_descriptionLine519 = new BitSet(new ulong[]{0x0000000001400000UL});
    public static readonly BitSet FOLLOW_descriptionLineText_in_descriptionLine522 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_descriptionLine524 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_background559 = new BitSet(new ulong[]{0x0000000000000200UL});
    public static readonly BitSet FOLLOW_T_BACKGROUND_in_background562 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_WS_in_background574 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_title_in_background576 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_background589 = new BitSet(new ulong[]{0x0000000000402000UL});
    public static readonly BitSet FOLLOW_givens_in_background591 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_scenarioOutline_in_scenarioKind629 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_scenario_in_scenarioKind640 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_tags_in_scenario659 = new BitSet(new ulong[]{0x0000000000400400UL});
    public static readonly BitSet FOLLOW_WS_in_scenario662 = new BitSet(new ulong[]{0x0000000000000400UL});
    public static readonly BitSet FOLLOW_T_SCENARIO_in_scenario665 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_WS_in_scenario667 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_title_in_scenario679 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_scenario681 = new BitSet(new ulong[]{0x000000000040E000UL});
    public static readonly BitSet FOLLOW_steps_in_scenario692 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_tags_in_scenarioOutline738 = new BitSet(new ulong[]{0x0000000000400800UL});
    public static readonly BitSet FOLLOW_WS_in_scenarioOutline741 = new BitSet(new ulong[]{0x0000000000000800UL});
    public static readonly BitSet FOLLOW_T_SCENARIO_OUTLINE_in_scenarioOutline744 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_WS_in_scenarioOutline746 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_title_in_scenarioOutline757 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_scenarioOutline759 = new BitSet(new ulong[]{0x000000000040E000UL});
    public static readonly BitSet FOLLOW_steps_in_scenarioOutline769 = new BitSet(new ulong[]{0x0000000000401000UL});
    public static readonly BitSet FOLLOW_examples_in_scenarioOutline779 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_exampleSet_in_examples821 = new BitSet(new ulong[]{0x0000000000401002UL});
    public static readonly BitSet FOLLOW_WS_in_exampleSet858 = new BitSet(new ulong[]{0x0000000000001000UL});
    public static readonly BitSet FOLLOW_T_EXAMPLES_in_exampleSet861 = new BitSet(new ulong[]{0x0000000001D00000UL});
    public static readonly BitSet FOLLOW_WS_in_exampleSet863 = new BitSet(new ulong[]{0x0000000001D00000UL});
    public static readonly BitSet FOLLOW_text_in_exampleSet874 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_exampleSet877 = new BitSet(new ulong[]{0x0000000000480000UL});
    public static readonly BitSet FOLLOW_table_in_exampleSet879 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstStep_in_steps917 = new BitSet(new ulong[]{0x000000000043E002UL});
    public static readonly BitSet FOLLOW_nextStep_in_steps919 = new BitSet(new ulong[]{0x000000000043E002UL});
    public static readonly BitSet FOLLOW_firstGiven_in_firstStep952 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstWhen_in_firstStep961 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstThen_in_firstStep970 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstStep_in_nextStep992 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstAnd_in_nextStep1001 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstBut_in_nextStep1010 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstAnd1033 = new BitSet(new ulong[]{0x0000000000010000UL});
    public static readonly BitSet FOLLOW_T_AND_in_firstAnd1036 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_WS_in_firstAnd1038 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstAnd1040 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstBut1075 = new BitSet(new ulong[]{0x0000000000020000UL});
    public static readonly BitSet FOLLOW_T_BUT_in_firstBut1078 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_WS_in_firstBut1080 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstBut1082 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstGiven_in_givens1117 = new BitSet(new ulong[]{0x000000000043E002UL});
    public static readonly BitSet FOLLOW_nextStep_in_givens1119 = new BitSet(new ulong[]{0x000000000043E002UL});
    public static readonly BitSet FOLLOW_WS_in_firstGiven1157 = new BitSet(new ulong[]{0x0000000000002000UL});
    public static readonly BitSet FOLLOW_T_GIVEN_in_firstGiven1160 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_WS_in_firstGiven1162 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstGiven1164 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstWhen1198 = new BitSet(new ulong[]{0x0000000000004000UL});
    public static readonly BitSet FOLLOW_T_WHEN_in_firstWhen1201 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_WS_in_firstWhen1203 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstWhen1205 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstThen1239 = new BitSet(new ulong[]{0x0000000000008000UL});
    public static readonly BitSet FOLLOW_T_THEN_in_firstThen1242 = new BitSet(new ulong[]{0x0000000000400000UL});
    public static readonly BitSet FOLLOW_WS_in_firstThen1244 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstThen1246 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_text_in_sentenceEnd1281 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_sentenceEnd1283 = new BitSet(new ulong[]{0x00000000004C0002UL});
    public static readonly BitSet FOLLOW_multilineText_in_sentenceEnd1285 = new BitSet(new ulong[]{0x0000000000480002UL});
    public static readonly BitSet FOLLOW_table_in_sentenceEnd1288 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_indent_in_multilineText1326 = new BitSet(new ulong[]{0x0000000000040000UL});
    public static readonly BitSet FOLLOW_MLTEXT_in_multilineText1328 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_WS_in_multilineText1330 = new BitSet(new ulong[]{0x0000000000800000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_multilineText1333 = new BitSet(new ulong[]{0x0000000001D40000UL});
    public static readonly BitSet FOLLOW_multilineTextLine_in_multilineText1343 = new BitSet(new ulong[]{0x0000000001D40000UL});
    public static readonly BitSet FOLLOW_WS_in_multilineText1354 = new BitSet(new ulong[]{0x0000000000040000UL});
    public static readonly BitSet FOLLOW_MLTEXT_in_multilineText1357 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_WS_in_multilineText1359 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_multilineText1362 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_indent1400 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_multilineTextLine1429 = new BitSet(new ulong[]{0x0000000001D00000UL});
    public static readonly BitSet FOLLOW_text_in_multilineTextLine1432 = new BitSet(new ulong[]{0x0000000000800000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_multilineTextLine1435 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_tableRow_in_table1476 = new BitSet(new ulong[]{0x0000000000480000UL});
    public static readonly BitSet FOLLOW_tableRow_in_table1478 = new BitSet(new ulong[]{0x0000000000480002UL});
    public static readonly BitSet FOLLOW_WS_in_tableRow1525 = new BitSet(new ulong[]{0x0000000000080000UL});
    public static readonly BitSet FOLLOW_CELLSEP_in_tableRow1528 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_tableCell_in_tableRow1530 = new BitSet(new ulong[]{0x0000000001D00000UL});
    public static readonly BitSet FOLLOW_WS_in_tableRow1533 = new BitSet(new ulong[]{0x0000000000C00000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_tableRow1536 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_tableCell1572 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_text_in_tableCell1575 = new BitSet(new ulong[]{0x0000000000480000UL});
    public static readonly BitSet FOLLOW_WS_in_tableCell1577 = new BitSet(new ulong[]{0x0000000000080000UL});
    public static readonly BitSet FOLLOW_CELLSEP_in_tableCell1580 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_descriptionLineText1615 = new BitSet(new ulong[]{0x0000000001500002UL});
    public static readonly BitSet FOLLOW_textRest_in_descriptionLineText1617 = new BitSet(new ulong[]{0x0000000001500002UL});
    public static readonly BitSet FOLLOW_wordchar_in_text1656 = new BitSet(new ulong[]{0x0000000001500002UL});
    public static readonly BitSet FOLLOW_textRest_in_text1658 = new BitSet(new ulong[]{0x0000000001500002UL});
    public static readonly BitSet FOLLOW_WS_in_textRest1696 = new BitSet(new ulong[]{0x0000000001500000UL});
    public static readonly BitSet FOLLOW_textRest_in_textRest1698 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_textRest1708 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_title1727 = new BitSet(new ulong[]{0x0000000001D00002UL});
    public static readonly BitSet FOLLOW_titleRest_in_title1729 = new BitSet(new ulong[]{0x0000000001D00002UL});
    public static readonly BitSet FOLLOW_WS_in_titleRest1768 = new BitSet(new ulong[]{0x0000000001D00000UL});
    public static readonly BitSet FOLLOW_titleRest_in_titleRest1770 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_titleRest1780 = new BitSet(new ulong[]{0x0000000001D00000UL});
    public static readonly BitSet FOLLOW_titleRest_in_titleRest1782 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_titleRest1792 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_titleRestLine1811 = new BitSet(new ulong[]{0x0000000001C00000UL});
    public static readonly BitSet FOLLOW_titleRestLine_in_titleRestLine1813 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_titleRestLine1823 = new BitSet(new ulong[]{0x0000000001C00000UL});
    public static readonly BitSet FOLLOW_titleRestLine_in_titleRestLine1825 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_titleRestLine1835 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_wordchar0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_newlineWithSpaces1883 = new BitSet(new ulong[]{0x0000000000800000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_newlineWithSpaces1886 = new BitSet(new ulong[]{0x0000000000C00002UL});
    public static readonly BitSet FOLLOW_WS_in_newlineWithSpaces1889 = new BitSet(new ulong[]{0x0000000000800000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_newlineWithSpaces1892 = new BitSet(new ulong[]{0x0000000000C00002UL});
    public static readonly BitSet FOLLOW_WS_in_synpred25_SpecFlowLangParser863 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_synpred43_SpecFlowLangParser1359 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_synpred50_SpecFlowLangParser1533 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_titleRest_in_synpred56_SpecFlowLangParser1729 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}