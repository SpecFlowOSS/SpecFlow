// $ANTLR 3.1.2 SpecFlowLang.g 2009-10-28 16:45:50

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
    public const int WORD = 21;
    public const int AT = 31;
    public const int BACKGROUND = 6;
    public const int T__51 = 51;
    public const int THEN = 15;
    public const int MULTILINETEXT = 22;
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
		get { return "SpecFlowLang.g"; }
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
    // SpecFlowLang.g:44:1: feature : ( newlineWithSpaces )? ( tags )? ( WS )? 'Feature:' ( WS )? text newlineWithSpaces ( descriptionLine )* ( background )? ( scenarioKind )* ( WS )? EOF -> ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) ) ;
    public SpecFlowLangParser.feature_return feature() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.feature_return retval = new SpecFlowLangParser.feature_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS3 = null;
        IToken string_literal4 = null;
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
        object string_literal4_tree=null;
        object WS5_tree=null;
        object WS11_tree=null;
        object EOF12_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_40 = new RewriteRuleTokenStream(adaptor,"token 40");
        RewriteRuleTokenStream stream_EOF = new RewriteRuleTokenStream(adaptor,"token EOF");
        RewriteRuleSubtreeStream stream_tags = new RewriteRuleSubtreeStream(adaptor,"rule tags");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        RewriteRuleSubtreeStream stream_scenarioKind = new RewriteRuleSubtreeStream(adaptor,"rule scenarioKind");
        RewriteRuleSubtreeStream stream_background = new RewriteRuleSubtreeStream(adaptor,"rule background");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        RewriteRuleSubtreeStream stream_descriptionLine = new RewriteRuleSubtreeStream(adaptor,"rule descriptionLine");
        try 
    	{
            // SpecFlowLang.g:45:5: ( ( newlineWithSpaces )? ( tags )? ( WS )? 'Feature:' ( WS )? text newlineWithSpaces ( descriptionLine )* ( background )? ( scenarioKind )* ( WS )? EOF -> ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) ) )
            // SpecFlowLang.g:45:9: ( newlineWithSpaces )? ( tags )? ( WS )? 'Feature:' ( WS )? text newlineWithSpaces ( descriptionLine )* ( background )? ( scenarioKind )* ( WS )? EOF
            {
            	// SpecFlowLang.g:45:9: ( newlineWithSpaces )?
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
            	        // SpecFlowLang.g:0:0: newlineWithSpaces
            	        {
            	        	PushFollow(FOLLOW_newlineWithSpaces_in_feature264);
            	        	newlineWithSpaces1 = newlineWithSpaces();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces1.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLang.g:46:9: ( tags )?
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
            	        // SpecFlowLang.g:0:0: tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_feature275);
            	        	tags2 = tags();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_tags.Add(tags2.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLang.g:47:9: ( WS )?
            	int alt3 = 2;
            	int LA3_0 = input.LA(1);

            	if ( (LA3_0 == WS) )
            	{
            	    alt3 = 1;
            	}
            	switch (alt3) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS3=(IToken)Match(input,WS,FOLLOW_WS_in_feature286); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS3);


            	        }
            	        break;

            	}

            	string_literal4=(IToken)Match(input,40,FOLLOW_40_in_feature289); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_40.Add(string_literal4);

            	// SpecFlowLang.g:47:24: ( WS )?
            	int alt4 = 2;
            	int LA4_0 = input.LA(1);

            	if ( (LA4_0 == WS) )
            	{
            	    alt4 = 1;
            	}
            	switch (alt4) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS5=(IToken)Match(input,WS,FOLLOW_WS_in_feature291); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS5);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_feature294);
            	text6 = text();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_text.Add(text6.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_feature296);
            	newlineWithSpaces7 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces7.Tree);
            	// SpecFlowLang.g:48:9: ( descriptionLine )*
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
            			    // SpecFlowLang.g:0:0: descriptionLine
            			    {
            			    	PushFollow(FOLLOW_descriptionLine_in_feature306);
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

            	// SpecFlowLang.g:49:9: ( background )?
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
            	        // SpecFlowLang.g:0:0: background
            	        {
            	        	PushFollow(FOLLOW_background_in_feature317);
            	        	background9 = background();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_background.Add(background9.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLang.g:50:9: ( scenarioKind )*
            	do 
            	{
            	    int alt7 = 2;
            	    int LA7_0 = input.LA(1);

            	    if ( (LA7_0 == WS) )
            	    {
            	        int LA7_1 = input.LA(2);

            	        if ( (LA7_1 == AT || (LA7_1 >= 41 && LA7_1 <= 42)) )
            	        {
            	            alt7 = 1;
            	        }


            	    }
            	    else if ( (LA7_0 == AT || (LA7_0 >= 41 && LA7_0 <= 42)) )
            	    {
            	        alt7 = 1;
            	    }


            	    switch (alt7) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: scenarioKind
            			    {
            			    	PushFollow(FOLLOW_scenarioKind_in_feature328);
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

            	// SpecFlowLang.g:50:23: ( WS )?
            	int alt8 = 2;
            	int LA8_0 = input.LA(1);

            	if ( (LA8_0 == WS) )
            	{
            	    alt8 = 1;
            	}
            	switch (alt8) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS11=(IToken)Match(input,WS,FOLLOW_WS_in_feature331); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS11);


            	        }
            	        break;

            	}

            	EOF12=(IToken)Match(input,EOF,FOLLOW_EOF_in_feature334); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_EOF.Add(EOF12);



            	// AST REWRITE
            	// elements:          text, descriptionLine, tags, scenarioKind, background
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 51:9: -> ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) )
            	{
            	    // SpecFlowLang.g:51:12: ^( FEATURE ( tags )? text ( descriptionLine )* ( background )? ^( SCENARIOS ( scenarioKind )* ) )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(FEATURE, "FEATURE"), root_1);

            	    // SpecFlowLang.g:51:22: ( tags )?
            	    if ( stream_tags.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_tags.NextTree());

            	    }
            	    stream_tags.Reset();
            	    adaptor.AddChild(root_1, stream_text.NextTree());
            	    // SpecFlowLang.g:51:33: ( descriptionLine )*
            	    while ( stream_descriptionLine.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_descriptionLine.NextTree());

            	    }
            	    stream_descriptionLine.Reset();
            	    // SpecFlowLang.g:51:50: ( background )?
            	    if ( stream_background.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_background.NextTree());

            	    }
            	    stream_background.Reset();
            	    // SpecFlowLang.g:52:13: ^( SCENARIOS ( scenarioKind )* )
            	    {
            	    object root_2 = (object)adaptor.GetNilNode();
            	    root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(SCENARIOS, "SCENARIOS"), root_2);

            	    // SpecFlowLang.g:52:25: ( scenarioKind )*
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
    // SpecFlowLang.g:56:1: tags : ( WS )? ( tag )+ -> ^( TAGS ( tag )+ ) ;
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
            // SpecFlowLang.g:57:5: ( ( WS )? ( tag )+ -> ^( TAGS ( tag )+ ) )
            // SpecFlowLang.g:57:9: ( WS )? ( tag )+
            {
            	// SpecFlowLang.g:57:9: ( WS )?
            	int alt9 = 2;
            	int LA9_0 = input.LA(1);

            	if ( (LA9_0 == WS) )
            	{
            	    alt9 = 1;
            	}
            	switch (alt9) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS13=(IToken)Match(input,WS,FOLLOW_WS_in_tags411); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS13);


            	        }
            	        break;

            	}

            	// SpecFlowLang.g:57:13: ( tag )+
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
            			    // SpecFlowLang.g:0:0: tag
            			    {
            			    	PushFollow(FOLLOW_tag_in_tags414);
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
            	// 58:9: -> ^( TAGS ( tag )+ )
            	{
            	    // SpecFlowLang.g:58:12: ^( TAGS ( tag )+ )
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
    // SpecFlowLang.g:61:1: tag : AT word ( newlineWithSpaces | WS ) -> ^( TAG word ) ;
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
            // SpecFlowLang.g:62:5: ( AT word ( newlineWithSpaces | WS ) -> ^( TAG word ) )
            // SpecFlowLang.g:62:9: AT word ( newlineWithSpaces | WS )
            {
            	AT15=(IToken)Match(input,AT,FOLLOW_AT_in_tag451); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_AT.Add(AT15);

            	PushFollow(FOLLOW_word_in_tag453);
            	word16 = word();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_word.Add(word16.Tree);
            	// SpecFlowLang.g:62:17: ( newlineWithSpaces | WS )
            	int alt11 = 2;
            	int LA11_0 = input.LA(1);

            	if ( (LA11_0 == WS) )
            	{
            	    int LA11_1 = input.LA(2);

            	    if ( (LA11_1 == EOF || (LA11_1 >= WS && LA11_1 <= AT) || (LA11_1 >= 40 && LA11_1 <= 42)) )
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
            	        // SpecFlowLang.g:62:18: newlineWithSpaces
            	        {
            	        	PushFollow(FOLLOW_newlineWithSpaces_in_tag456);
            	        	newlineWithSpaces17 = newlineWithSpaces();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces17.Tree);

            	        }
            	        break;
            	    case 2 :
            	        // SpecFlowLang.g:62:36: WS
            	        {
            	        	WS18=(IToken)Match(input,WS,FOLLOW_WS_in_tag458); if (state.failed) return retval; 
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
            	// 63:9: -> ^( TAG word )
            	{
            	    // SpecFlowLang.g:63:12: ^( TAG word )
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
    // SpecFlowLang.g:66:1: word : ( WORDCHAR )+ -> ^( WORD ( WORDCHAR )+ ) ;
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
            // SpecFlowLang.g:67:5: ( ( WORDCHAR )+ -> ^( WORD ( WORDCHAR )+ ) )
            // SpecFlowLang.g:67:9: ( WORDCHAR )+
            {
            	// SpecFlowLang.g:67:9: ( WORDCHAR )+
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
            			    // SpecFlowLang.g:0:0: WORDCHAR
            			    {
            			    	WORDCHAR19=(IToken)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_word494); if (state.failed) return retval; 
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
            	// 68:9: -> ^( WORD ( WORDCHAR )+ )
            	{
            	    // SpecFlowLang.g:68:12: ^( WORD ( WORDCHAR )+ )
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
    // SpecFlowLang.g:71:1: descriptionLine : ( WS )? descriptionLineText newlineWithSpaces -> ^( DESCRIPTIONLINE descriptionLineText ) ;
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
            // SpecFlowLang.g:72:5: ( ( WS )? descriptionLineText newlineWithSpaces -> ^( DESCRIPTIONLINE descriptionLineText ) )
            // SpecFlowLang.g:72:9: ( WS )? descriptionLineText newlineWithSpaces
            {
            	// SpecFlowLang.g:72:9: ( WS )?
            	int alt13 = 2;
            	int LA13_0 = input.LA(1);

            	if ( (LA13_0 == WS) )
            	{
            	    alt13 = 1;
            	}
            	switch (alt13) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS20=(IToken)Match(input,WS,FOLLOW_WS_in_descriptionLine531); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS20);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_descriptionLineText_in_descriptionLine534);
            	descriptionLineText21 = descriptionLineText();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_descriptionLineText.Add(descriptionLineText21.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_descriptionLine536);
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
            	// 73:9: -> ^( DESCRIPTIONLINE descriptionLineText )
            	{
            	    // SpecFlowLang.g:73:12: ^( DESCRIPTIONLINE descriptionLineText )
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
    // SpecFlowLang.g:76:1: background : ( WS )? T_BACKGROUND ( WS title )? newlineWithSpaces givens -> ^( BACKGROUND ( title )? givens ) ;
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
            // SpecFlowLang.g:77:5: ( ( WS )? T_BACKGROUND ( WS title )? newlineWithSpaces givens -> ^( BACKGROUND ( title )? givens ) )
            // SpecFlowLang.g:77:9: ( WS )? T_BACKGROUND ( WS title )? newlineWithSpaces givens
            {
            	// SpecFlowLang.g:77:9: ( WS )?
            	int alt14 = 2;
            	int LA14_0 = input.LA(1);

            	if ( (LA14_0 == WS) )
            	{
            	    alt14 = 1;
            	}
            	switch (alt14) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS23=(IToken)Match(input,WS,FOLLOW_WS_in_background571); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS23);


            	        }
            	        break;

            	}

            	T_BACKGROUND24=(IToken)Match(input,T_BACKGROUND,FOLLOW_T_BACKGROUND_in_background574); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_T_BACKGROUND.Add(T_BACKGROUND24);

            	// SpecFlowLang.g:78:9: ( WS title )?
            	int alt15 = 2;
            	int LA15_0 = input.LA(1);

            	if ( (LA15_0 == WS) )
            	{
            	    int LA15_1 = input.LA(2);

            	    if ( ((LA15_1 >= AT && LA15_1 <= WORDCHAR)) )
            	    {
            	        alt15 = 1;
            	    }
            	}
            	switch (alt15) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:78:10: WS title
            	        {
            	        	WS25=(IToken)Match(input,WS,FOLLOW_WS_in_background586); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS25);

            	        	PushFollow(FOLLOW_title_in_background588);
            	        	title26 = title();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_title.Add(title26.Tree);

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_background601);
            	newlineWithSpaces27 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces27.Tree);
            	PushFollow(FOLLOW_givens_in_background603);
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
            	// 80:9: -> ^( BACKGROUND ( title )? givens )
            	{
            	    // SpecFlowLang.g:80:12: ^( BACKGROUND ( title )? givens )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(BACKGROUND, "BACKGROUND"), root_1);

            	    // SpecFlowLang.g:80:25: ( title )?
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
    // SpecFlowLang.g:83:1: scenarioKind : ( scenarioOutline | scenario );
    public SpecFlowLangParser.scenarioKind_return scenarioKind() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.scenarioKind_return retval = new SpecFlowLangParser.scenarioKind_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.scenarioOutline_return scenarioOutline29 = default(SpecFlowLangParser.scenarioOutline_return);

        SpecFlowLangParser.scenario_return scenario30 = default(SpecFlowLangParser.scenario_return);



        try 
    	{
            // SpecFlowLang.g:84:5: ( scenarioOutline | scenario )
            int alt16 = 2;
            alt16 = dfa16.Predict(input);
            switch (alt16) 
            {
                case 1 :
                    // SpecFlowLang.g:84:9: scenarioOutline
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_scenarioOutline_in_scenarioKind641);
                    	scenarioOutline29 = scenarioOutline();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, scenarioOutline29.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:85:9: scenario
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_scenario_in_scenarioKind652);
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
    // SpecFlowLang.g:88:1: scenario : ( tags )? ( WS )? 'Scenario:' ( WS )? title newlineWithSpaces steps -> ^( SCENARIO ( tags )? title steps ) ;
    public SpecFlowLangParser.scenario_return scenario() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.scenario_return retval = new SpecFlowLangParser.scenario_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS32 = null;
        IToken string_literal33 = null;
        IToken WS34 = null;
        SpecFlowLangParser.tags_return tags31 = default(SpecFlowLangParser.tags_return);

        SpecFlowLangParser.title_return title35 = default(SpecFlowLangParser.title_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces36 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.steps_return steps37 = default(SpecFlowLangParser.steps_return);


        object WS32_tree=null;
        object string_literal33_tree=null;
        object WS34_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_41 = new RewriteRuleTokenStream(adaptor,"token 41");
        RewriteRuleSubtreeStream stream_tags = new RewriteRuleSubtreeStream(adaptor,"rule tags");
        RewriteRuleSubtreeStream stream_title = new RewriteRuleSubtreeStream(adaptor,"rule title");
        RewriteRuleSubtreeStream stream_steps = new RewriteRuleSubtreeStream(adaptor,"rule steps");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLang.g:89:5: ( ( tags )? ( WS )? 'Scenario:' ( WS )? title newlineWithSpaces steps -> ^( SCENARIO ( tags )? title steps ) )
            // SpecFlowLang.g:89:9: ( tags )? ( WS )? 'Scenario:' ( WS )? title newlineWithSpaces steps
            {
            	// SpecFlowLang.g:89:9: ( tags )?
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
            	        // SpecFlowLang.g:0:0: tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_scenario671);
            	        	tags31 = tags();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_tags.Add(tags31.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLang.g:89:15: ( WS )?
            	int alt18 = 2;
            	int LA18_0 = input.LA(1);

            	if ( (LA18_0 == WS) )
            	{
            	    alt18 = 1;
            	}
            	switch (alt18) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS32=(IToken)Match(input,WS,FOLLOW_WS_in_scenario674); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS32);


            	        }
            	        break;

            	}

            	string_literal33=(IToken)Match(input,41,FOLLOW_41_in_scenario677); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_41.Add(string_literal33);

            	// SpecFlowLang.g:89:31: ( WS )?
            	int alt19 = 2;
            	int LA19_0 = input.LA(1);

            	if ( (LA19_0 == WS) )
            	{
            	    alt19 = 1;
            	}
            	switch (alt19) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS34=(IToken)Match(input,WS,FOLLOW_WS_in_scenario679); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS34);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_title_in_scenario691);
            	title35 = title();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_title.Add(title35.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_scenario693);
            	newlineWithSpaces36 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces36.Tree);
            	PushFollow(FOLLOW_steps_in_scenario704);
            	steps37 = steps();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_steps.Add(steps37.Tree);


            	// AST REWRITE
            	// elements:          steps, tags, title
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 92:9: -> ^( SCENARIO ( tags )? title steps )
            	{
            	    // SpecFlowLang.g:92:12: ^( SCENARIO ( tags )? title steps )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(SCENARIO, "SCENARIO"), root_1);

            	    // SpecFlowLang.g:92:23: ( tags )?
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
    // SpecFlowLang.g:95:1: scenarioOutline : ( tags )? ( WS )? 'Scenario Outline:' ( WS )? title newlineWithSpaces steps examples -> ^( SCENARIOOUTLINE ( tags )? title steps examples ) ;
    public SpecFlowLangParser.scenarioOutline_return scenarioOutline() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.scenarioOutline_return retval = new SpecFlowLangParser.scenarioOutline_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS39 = null;
        IToken string_literal40 = null;
        IToken WS41 = null;
        SpecFlowLangParser.tags_return tags38 = default(SpecFlowLangParser.tags_return);

        SpecFlowLangParser.title_return title42 = default(SpecFlowLangParser.title_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces43 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.steps_return steps44 = default(SpecFlowLangParser.steps_return);

        SpecFlowLangParser.examples_return examples45 = default(SpecFlowLangParser.examples_return);


        object WS39_tree=null;
        object string_literal40_tree=null;
        object WS41_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_42 = new RewriteRuleTokenStream(adaptor,"token 42");
        RewriteRuleSubtreeStream stream_tags = new RewriteRuleSubtreeStream(adaptor,"rule tags");
        RewriteRuleSubtreeStream stream_title = new RewriteRuleSubtreeStream(adaptor,"rule title");
        RewriteRuleSubtreeStream stream_steps = new RewriteRuleSubtreeStream(adaptor,"rule steps");
        RewriteRuleSubtreeStream stream_examples = new RewriteRuleSubtreeStream(adaptor,"rule examples");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLang.g:96:5: ( ( tags )? ( WS )? 'Scenario Outline:' ( WS )? title newlineWithSpaces steps examples -> ^( SCENARIOOUTLINE ( tags )? title steps examples ) )
            // SpecFlowLang.g:97:9: ( tags )? ( WS )? 'Scenario Outline:' ( WS )? title newlineWithSpaces steps examples
            {
            	// SpecFlowLang.g:97:9: ( tags )?
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
            	        // SpecFlowLang.g:0:0: tags
            	        {
            	        	PushFollow(FOLLOW_tags_in_scenarioOutline750);
            	        	tags38 = tags();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_tags.Add(tags38.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLang.g:97:15: ( WS )?
            	int alt21 = 2;
            	int LA21_0 = input.LA(1);

            	if ( (LA21_0 == WS) )
            	{
            	    alt21 = 1;
            	}
            	switch (alt21) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS39=(IToken)Match(input,WS,FOLLOW_WS_in_scenarioOutline753); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS39);


            	        }
            	        break;

            	}

            	string_literal40=(IToken)Match(input,42,FOLLOW_42_in_scenarioOutline756); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_42.Add(string_literal40);

            	// SpecFlowLang.g:97:39: ( WS )?
            	int alt22 = 2;
            	int LA22_0 = input.LA(1);

            	if ( (LA22_0 == WS) )
            	{
            	    alt22 = 1;
            	}
            	switch (alt22) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS41=(IToken)Match(input,WS,FOLLOW_WS_in_scenarioOutline758); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS41);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_title_in_scenarioOutline769);
            	title42 = title();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_title.Add(title42.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_scenarioOutline771);
            	newlineWithSpaces43 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces43.Tree);
            	PushFollow(FOLLOW_steps_in_scenarioOutline781);
            	steps44 = steps();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_steps.Add(steps44.Tree);
            	PushFollow(FOLLOW_examples_in_scenarioOutline791);
            	examples45 = examples();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_examples.Add(examples45.Tree);


            	// AST REWRITE
            	// elements:          tags, steps, title, examples
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 101:9: -> ^( SCENARIOOUTLINE ( tags )? title steps examples )
            	{
            	    // SpecFlowLang.g:101:12: ^( SCENARIOOUTLINE ( tags )? title steps examples )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(SCENARIOOUTLINE, "SCENARIOOUTLINE"), root_1);

            	    // SpecFlowLang.g:101:30: ( tags )?
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
    // SpecFlowLang.g:104:1: examples : ( exampleSet )+ -> ^( EXAMPLES ( exampleSet )+ ) ;
    public SpecFlowLangParser.examples_return examples() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.examples_return retval = new SpecFlowLangParser.examples_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.exampleSet_return exampleSet46 = default(SpecFlowLangParser.exampleSet_return);


        RewriteRuleSubtreeStream stream_exampleSet = new RewriteRuleSubtreeStream(adaptor,"rule exampleSet");
        try 
    	{
            // SpecFlowLang.g:105:5: ( ( exampleSet )+ -> ^( EXAMPLES ( exampleSet )+ ) )
            // SpecFlowLang.g:105:9: ( exampleSet )+
            {
            	// SpecFlowLang.g:105:9: ( exampleSet )+
            	int cnt23 = 0;
            	do 
            	{
            	    int alt23 = 2;
            	    int LA23_0 = input.LA(1);

            	    if ( (LA23_0 == WS) )
            	    {
            	        int LA23_1 = input.LA(2);

            	        if ( ((LA23_1 >= 43 && LA23_1 <= 44)) )
            	        {
            	            alt23 = 1;
            	        }


            	    }
            	    else if ( ((LA23_0 >= 43 && LA23_0 <= 44)) )
            	    {
            	        alt23 = 1;
            	    }


            	    switch (alt23) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: exampleSet
            			    {
            			    	PushFollow(FOLLOW_exampleSet_in_examples833);
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
            	// 106:9: -> ^( EXAMPLES ( exampleSet )+ )
            	{
            	    // SpecFlowLang.g:106:12: ^( EXAMPLES ( exampleSet )+ )
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
    // SpecFlowLang.g:109:1: exampleSet : ( WS )? ( 'Examples:' | 'Scenarios:' ) ( WS )? ( text )? newlineWithSpaces table -> ^( EXAMPLESET ( text )? table ) ;
    public SpecFlowLangParser.exampleSet_return exampleSet() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.exampleSet_return retval = new SpecFlowLangParser.exampleSet_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS47 = null;
        IToken string_literal48 = null;
        IToken string_literal49 = null;
        IToken WS50 = null;
        SpecFlowLangParser.text_return text51 = default(SpecFlowLangParser.text_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces52 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.table_return table53 = default(SpecFlowLangParser.table_return);


        object WS47_tree=null;
        object string_literal48_tree=null;
        object string_literal49_tree=null;
        object WS50_tree=null;
        RewriteRuleTokenStream stream_43 = new RewriteRuleTokenStream(adaptor,"token 43");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_44 = new RewriteRuleTokenStream(adaptor,"token 44");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        RewriteRuleSubtreeStream stream_table = new RewriteRuleSubtreeStream(adaptor,"rule table");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLang.g:110:5: ( ( WS )? ( 'Examples:' | 'Scenarios:' ) ( WS )? ( text )? newlineWithSpaces table -> ^( EXAMPLESET ( text )? table ) )
            // SpecFlowLang.g:110:9: ( WS )? ( 'Examples:' | 'Scenarios:' ) ( WS )? ( text )? newlineWithSpaces table
            {
            	// SpecFlowLang.g:110:9: ( WS )?
            	int alt24 = 2;
            	int LA24_0 = input.LA(1);

            	if ( (LA24_0 == WS) )
            	{
            	    alt24 = 1;
            	}
            	switch (alt24) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS47=(IToken)Match(input,WS,FOLLOW_WS_in_exampleSet870); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS47);


            	        }
            	        break;

            	}

            	// SpecFlowLang.g:110:13: ( 'Examples:' | 'Scenarios:' )
            	int alt25 = 2;
            	int LA25_0 = input.LA(1);

            	if ( (LA25_0 == 43) )
            	{
            	    alt25 = 1;
            	}
            	else if ( (LA25_0 == 44) )
            	{
            	    alt25 = 2;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d25s0 =
            	        new NoViableAltException("", 25, 0, input);

            	    throw nvae_d25s0;
            	}
            	switch (alt25) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:110:14: 'Examples:'
            	        {
            	        	string_literal48=(IToken)Match(input,43,FOLLOW_43_in_exampleSet874); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_43.Add(string_literal48);


            	        }
            	        break;
            	    case 2 :
            	        // SpecFlowLang.g:110:26: 'Scenarios:'
            	        {
            	        	string_literal49=(IToken)Match(input,44,FOLLOW_44_in_exampleSet876); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_44.Add(string_literal49);


            	        }
            	        break;

            	}

            	// SpecFlowLang.g:110:40: ( WS )?
            	int alt26 = 2;
            	int LA26_0 = input.LA(1);

            	if ( (LA26_0 == WS) )
            	{
            	    int LA26_1 = input.LA(2);

            	    if ( (synpred26_SpecFlowLang()) )
            	    {
            	        alt26 = 1;
            	    }
            	}
            	switch (alt26) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS50=(IToken)Match(input,WS,FOLLOW_WS_in_exampleSet879); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS50);


            	        }
            	        break;

            	}

            	// SpecFlowLang.g:111:9: ( text )?
            	int alt27 = 2;
            	int LA27_0 = input.LA(1);

            	if ( ((LA27_0 >= AT && LA27_0 <= WORDCHAR)) )
            	{
            	    alt27 = 1;
            	}
            	switch (alt27) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: text
            	        {
            	        	PushFollow(FOLLOW_text_in_exampleSet890);
            	        	text51 = text();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_text.Add(text51.Tree);

            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_exampleSet893);
            	newlineWithSpaces52 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces52.Tree);
            	PushFollow(FOLLOW_table_in_exampleSet895);
            	table53 = table();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_table.Add(table53.Tree);


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
            	// 112:9: -> ^( EXAMPLESET ( text )? table )
            	{
            	    // SpecFlowLang.g:112:12: ^( EXAMPLESET ( text )? table )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(EXAMPLESET, "EXAMPLESET"), root_1);

            	    // SpecFlowLang.g:112:25: ( text )?
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
    // SpecFlowLang.g:115:1: steps : firstStep ( nextStep )* -> ^( STEPS firstStep ( nextStep )* ) ;
    public SpecFlowLangParser.steps_return steps() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.steps_return retval = new SpecFlowLangParser.steps_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstStep_return firstStep54 = default(SpecFlowLangParser.firstStep_return);

        SpecFlowLangParser.nextStep_return nextStep55 = default(SpecFlowLangParser.nextStep_return);


        RewriteRuleSubtreeStream stream_firstStep = new RewriteRuleSubtreeStream(adaptor,"rule firstStep");
        RewriteRuleSubtreeStream stream_nextStep = new RewriteRuleSubtreeStream(adaptor,"rule nextStep");
        try 
    	{
            // SpecFlowLang.g:116:5: ( firstStep ( nextStep )* -> ^( STEPS firstStep ( nextStep )* ) )
            // SpecFlowLang.g:116:9: firstStep ( nextStep )*
            {
            	PushFollow(FOLLOW_firstStep_in_steps933);
            	firstStep54 = firstStep();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_firstStep.Add(firstStep54.Tree);
            	// SpecFlowLang.g:116:19: ( nextStep )*
            	do 
            	{
            	    int alt28 = 2;
            	    int LA28_0 = input.LA(1);

            	    if ( (LA28_0 == WS) )
            	    {
            	        int LA28_1 = input.LA(2);

            	        if ( ((LA28_1 >= 45 && LA28_1 <= 49)) )
            	        {
            	            alt28 = 1;
            	        }


            	    }
            	    else if ( ((LA28_0 >= 45 && LA28_0 <= 49)) )
            	    {
            	        alt28 = 1;
            	    }


            	    switch (alt28) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: nextStep
            			    {
            			    	PushFollow(FOLLOW_nextStep_in_steps935);
            			    	nextStep55 = nextStep();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_nextStep.Add(nextStep55.Tree);

            			    }
            			    break;

            			default:
            			    goto loop28;
            	    }
            	} while (true);

            	loop28:
            		;	// Stops C# compiler whining that label 'loop28' has no statements



            	// AST REWRITE
            	// elements:          firstStep, nextStep
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 117:9: -> ^( STEPS firstStep ( nextStep )* )
            	{
            	    // SpecFlowLang.g:117:12: ^( STEPS firstStep ( nextStep )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(STEPS, "STEPS"), root_1);

            	    adaptor.AddChild(root_1, stream_firstStep.NextTree());
            	    // SpecFlowLang.g:117:30: ( nextStep )*
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
    // SpecFlowLang.g:119:1: firstStep : ( firstGiven -> firstGiven | firstWhen -> firstWhen | firstThen -> firstThen );
    public SpecFlowLangParser.firstStep_return firstStep() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstStep_return retval = new SpecFlowLangParser.firstStep_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstGiven_return firstGiven56 = default(SpecFlowLangParser.firstGiven_return);

        SpecFlowLangParser.firstWhen_return firstWhen57 = default(SpecFlowLangParser.firstWhen_return);

        SpecFlowLangParser.firstThen_return firstThen58 = default(SpecFlowLangParser.firstThen_return);


        RewriteRuleSubtreeStream stream_firstWhen = new RewriteRuleSubtreeStream(adaptor,"rule firstWhen");
        RewriteRuleSubtreeStream stream_firstThen = new RewriteRuleSubtreeStream(adaptor,"rule firstThen");
        RewriteRuleSubtreeStream stream_firstGiven = new RewriteRuleSubtreeStream(adaptor,"rule firstGiven");
        try 
    	{
            // SpecFlowLang.g:120:2: ( firstGiven -> firstGiven | firstWhen -> firstWhen | firstThen -> firstThen )
            int alt29 = 3;
            switch ( input.LA(1) ) 
            {
            case WS:
            	{
                switch ( input.LA(2) ) 
                {
                case 48:
                	{
                    alt29 = 2;
                    }
                    break;
                case 49:
                	{
                    alt29 = 3;
                    }
                    break;
                case 47:
                	{
                    alt29 = 1;
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
            case 47:
            	{
                alt29 = 1;
                }
                break;
            case 48:
            	{
                alt29 = 2;
                }
                break;
            case 49:
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
                    // SpecFlowLang.g:120:4: firstGiven
                    {
                    	PushFollow(FOLLOW_firstGiven_in_firstStep968);
                    	firstGiven56 = firstGiven();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstGiven.Add(firstGiven56.Tree);


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
                    	// 120:15: -> firstGiven
                    	{
                    	    adaptor.AddChild(root_0, stream_firstGiven.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:121:4: firstWhen
                    {
                    	PushFollow(FOLLOW_firstWhen_in_firstStep977);
                    	firstWhen57 = firstWhen();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstWhen.Add(firstWhen57.Tree);


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
                    	// 121:14: -> firstWhen
                    	{
                    	    adaptor.AddChild(root_0, stream_firstWhen.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 3 :
                    // SpecFlowLang.g:122:4: firstThen
                    {
                    	PushFollow(FOLLOW_firstThen_in_firstStep986);
                    	firstThen58 = firstThen();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstThen.Add(firstThen58.Tree);


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
                    	// 122:14: -> firstThen
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
    // SpecFlowLang.g:124:1: nextStep : ( firstStep -> firstStep | firstAnd -> firstAnd | firstBut -> firstBut );
    public SpecFlowLangParser.nextStep_return nextStep() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.nextStep_return retval = new SpecFlowLangParser.nextStep_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstStep_return firstStep59 = default(SpecFlowLangParser.firstStep_return);

        SpecFlowLangParser.firstAnd_return firstAnd60 = default(SpecFlowLangParser.firstAnd_return);

        SpecFlowLangParser.firstBut_return firstBut61 = default(SpecFlowLangParser.firstBut_return);


        RewriteRuleSubtreeStream stream_firstStep = new RewriteRuleSubtreeStream(adaptor,"rule firstStep");
        RewriteRuleSubtreeStream stream_firstAnd = new RewriteRuleSubtreeStream(adaptor,"rule firstAnd");
        RewriteRuleSubtreeStream stream_firstBut = new RewriteRuleSubtreeStream(adaptor,"rule firstBut");
        try 
    	{
            // SpecFlowLang.g:125:5: ( firstStep -> firstStep | firstAnd -> firstAnd | firstBut -> firstBut )
            int alt30 = 3;
            switch ( input.LA(1) ) 
            {
            case WS:
            	{
                switch ( input.LA(2) ) 
                {
                case 47:
                case 48:
                case 49:
                	{
                    alt30 = 1;
                    }
                    break;
                case 45:
                	{
                    alt30 = 2;
                    }
                    break;
                case 46:
                	{
                    alt30 = 3;
                    }
                    break;
                	default:
                	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                	    NoViableAltException nvae_d30s1 =
                	        new NoViableAltException("", 30, 1, input);

                	    throw nvae_d30s1;
                }

                }
                break;
            case 47:
            case 48:
            case 49:
            	{
                alt30 = 1;
                }
                break;
            case 45:
            	{
                alt30 = 2;
                }
                break;
            case 46:
            	{
                alt30 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d30s0 =
            	        new NoViableAltException("", 30, 0, input);

            	    throw nvae_d30s0;
            }

            switch (alt30) 
            {
                case 1 :
                    // SpecFlowLang.g:125:9: firstStep
                    {
                    	PushFollow(FOLLOW_firstStep_in_nextStep1008);
                    	firstStep59 = firstStep();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstStep.Add(firstStep59.Tree);


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
                    	// 125:19: -> firstStep
                    	{
                    	    adaptor.AddChild(root_0, stream_firstStep.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:126:4: firstAnd
                    {
                    	PushFollow(FOLLOW_firstAnd_in_nextStep1017);
                    	firstAnd60 = firstAnd();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstAnd.Add(firstAnd60.Tree);


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
                    	// 126:13: -> firstAnd
                    	{
                    	    adaptor.AddChild(root_0, stream_firstAnd.NextTree());

                    	}

                    	retval.Tree = root_0;retval.Tree = root_0;}
                    }
                    break;
                case 3 :
                    // SpecFlowLang.g:127:4: firstBut
                    {
                    	PushFollow(FOLLOW_firstBut_in_nextStep1026);
                    	firstBut61 = firstBut();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( (state.backtracking==0) ) stream_firstBut.Add(firstBut61.Tree);


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
                    	// 127:13: -> firstBut
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
    // SpecFlowLang.g:130:1: firstAnd : ( WS )? 'And' WS sentenceEnd -> ^( AND sentenceEnd ) ;
    public SpecFlowLangParser.firstAnd_return firstAnd() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstAnd_return retval = new SpecFlowLangParser.firstAnd_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS62 = null;
        IToken string_literal63 = null;
        IToken WS64 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd65 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS62_tree=null;
        object string_literal63_tree=null;
        object WS64_tree=null;
        RewriteRuleTokenStream stream_45 = new RewriteRuleTokenStream(adaptor,"token 45");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLang.g:131:5: ( ( WS )? 'And' WS sentenceEnd -> ^( AND sentenceEnd ) )
            // SpecFlowLang.g:131:9: ( WS )? 'And' WS sentenceEnd
            {
            	// SpecFlowLang.g:131:9: ( WS )?
            	int alt31 = 2;
            	int LA31_0 = input.LA(1);

            	if ( (LA31_0 == WS) )
            	{
            	    alt31 = 1;
            	}
            	switch (alt31) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS62=(IToken)Match(input,WS,FOLLOW_WS_in_firstAnd1049); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS62);


            	        }
            	        break;

            	}

            	string_literal63=(IToken)Match(input,45,FOLLOW_45_in_firstAnd1052); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_45.Add(string_literal63);

            	WS64=(IToken)Match(input,WS,FOLLOW_WS_in_firstAnd1054); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS64);

            	PushFollow(FOLLOW_sentenceEnd_in_firstAnd1056);
            	sentenceEnd65 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd65.Tree);


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
            	// 132:9: -> ^( AND sentenceEnd )
            	{
            	    // SpecFlowLang.g:132:12: ^( AND sentenceEnd )
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
    // SpecFlowLang.g:135:1: firstBut : ( WS )? 'But' WS sentenceEnd -> ^( BUT sentenceEnd ) ;
    public SpecFlowLangParser.firstBut_return firstBut() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstBut_return retval = new SpecFlowLangParser.firstBut_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS66 = null;
        IToken string_literal67 = null;
        IToken WS68 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd69 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS66_tree=null;
        object string_literal67_tree=null;
        object WS68_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_46 = new RewriteRuleTokenStream(adaptor,"token 46");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLang.g:136:5: ( ( WS )? 'But' WS sentenceEnd -> ^( BUT sentenceEnd ) )
            // SpecFlowLang.g:136:9: ( WS )? 'But' WS sentenceEnd
            {
            	// SpecFlowLang.g:136:9: ( WS )?
            	int alt32 = 2;
            	int LA32_0 = input.LA(1);

            	if ( (LA32_0 == WS) )
            	{
            	    alt32 = 1;
            	}
            	switch (alt32) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS66=(IToken)Match(input,WS,FOLLOW_WS_in_firstBut1091); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS66);


            	        }
            	        break;

            	}

            	string_literal67=(IToken)Match(input,46,FOLLOW_46_in_firstBut1094); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_46.Add(string_literal67);

            	WS68=(IToken)Match(input,WS,FOLLOW_WS_in_firstBut1096); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS68);

            	PushFollow(FOLLOW_sentenceEnd_in_firstBut1098);
            	sentenceEnd69 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd69.Tree);


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
            	// 137:9: -> ^( BUT sentenceEnd )
            	{
            	    // SpecFlowLang.g:137:12: ^( BUT sentenceEnd )
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
    // SpecFlowLang.g:140:1: givens : firstGiven ( nextStep )* -> ^( STEPS firstGiven ( nextStep )* ) ;
    public SpecFlowLangParser.givens_return givens() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.givens_return retval = new SpecFlowLangParser.givens_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.firstGiven_return firstGiven70 = default(SpecFlowLangParser.firstGiven_return);

        SpecFlowLangParser.nextStep_return nextStep71 = default(SpecFlowLangParser.nextStep_return);


        RewriteRuleSubtreeStream stream_nextStep = new RewriteRuleSubtreeStream(adaptor,"rule nextStep");
        RewriteRuleSubtreeStream stream_firstGiven = new RewriteRuleSubtreeStream(adaptor,"rule firstGiven");
        try 
    	{
            // SpecFlowLang.g:141:5: ( firstGiven ( nextStep )* -> ^( STEPS firstGiven ( nextStep )* ) )
            // SpecFlowLang.g:141:9: firstGiven ( nextStep )*
            {
            	PushFollow(FOLLOW_firstGiven_in_givens1133);
            	firstGiven70 = firstGiven();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_firstGiven.Add(firstGiven70.Tree);
            	// SpecFlowLang.g:141:20: ( nextStep )*
            	do 
            	{
            	    int alt33 = 2;
            	    int LA33_0 = input.LA(1);

            	    if ( (LA33_0 == WS) )
            	    {
            	        int LA33_1 = input.LA(2);

            	        if ( ((LA33_1 >= 45 && LA33_1 <= 49)) )
            	        {
            	            alt33 = 1;
            	        }


            	    }
            	    else if ( ((LA33_0 >= 45 && LA33_0 <= 49)) )
            	    {
            	        alt33 = 1;
            	    }


            	    switch (alt33) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: nextStep
            			    {
            			    	PushFollow(FOLLOW_nextStep_in_givens1135);
            			    	nextStep71 = nextStep();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_nextStep.Add(nextStep71.Tree);

            			    }
            			    break;

            			default:
            			    goto loop33;
            	    }
            	} while (true);

            	loop33:
            		;	// Stops C# compiler whining that label 'loop33' has no statements



            	// AST REWRITE
            	// elements:          firstGiven, nextStep
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 142:9: -> ^( STEPS firstGiven ( nextStep )* )
            	{
            	    // SpecFlowLang.g:142:12: ^( STEPS firstGiven ( nextStep )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(STEPS, "STEPS"), root_1);

            	    adaptor.AddChild(root_1, stream_firstGiven.NextTree());
            	    // SpecFlowLang.g:142:31: ( nextStep )*
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
    // SpecFlowLang.g:144:1: firstGiven : ( WS )? 'Given' WS sentenceEnd -> ^( GIVEN sentenceEnd ) ;
    public SpecFlowLangParser.firstGiven_return firstGiven() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstGiven_return retval = new SpecFlowLangParser.firstGiven_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS72 = null;
        IToken string_literal73 = null;
        IToken WS74 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd75 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS72_tree=null;
        object string_literal73_tree=null;
        object WS74_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_47 = new RewriteRuleTokenStream(adaptor,"token 47");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLang.g:145:5: ( ( WS )? 'Given' WS sentenceEnd -> ^( GIVEN sentenceEnd ) )
            // SpecFlowLang.g:145:9: ( WS )? 'Given' WS sentenceEnd
            {
            	// SpecFlowLang.g:145:9: ( WS )?
            	int alt34 = 2;
            	int LA34_0 = input.LA(1);

            	if ( (LA34_0 == WS) )
            	{
            	    alt34 = 1;
            	}
            	switch (alt34) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS72=(IToken)Match(input,WS,FOLLOW_WS_in_firstGiven1173); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS72);


            	        }
            	        break;

            	}

            	string_literal73=(IToken)Match(input,47,FOLLOW_47_in_firstGiven1176); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_47.Add(string_literal73);

            	WS74=(IToken)Match(input,WS,FOLLOW_WS_in_firstGiven1178); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS74);

            	PushFollow(FOLLOW_sentenceEnd_in_firstGiven1180);
            	sentenceEnd75 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd75.Tree);


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
            	// 146:9: -> ^( GIVEN sentenceEnd )
            	{
            	    // SpecFlowLang.g:146:12: ^( GIVEN sentenceEnd )
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
    // SpecFlowLang.g:157:1: firstWhen : ( WS )? 'When' WS sentenceEnd -> ^( WHEN sentenceEnd ) ;
    public SpecFlowLangParser.firstWhen_return firstWhen() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstWhen_return retval = new SpecFlowLangParser.firstWhen_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS76 = null;
        IToken string_literal77 = null;
        IToken WS78 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd79 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS76_tree=null;
        object string_literal77_tree=null;
        object WS78_tree=null;
        RewriteRuleTokenStream stream_48 = new RewriteRuleTokenStream(adaptor,"token 48");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLang.g:158:5: ( ( WS )? 'When' WS sentenceEnd -> ^( WHEN sentenceEnd ) )
            // SpecFlowLang.g:158:9: ( WS )? 'When' WS sentenceEnd
            {
            	// SpecFlowLang.g:158:9: ( WS )?
            	int alt35 = 2;
            	int LA35_0 = input.LA(1);

            	if ( (LA35_0 == WS) )
            	{
            	    alt35 = 1;
            	}
            	switch (alt35) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS76=(IToken)Match(input,WS,FOLLOW_WS_in_firstWhen1216); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS76);


            	        }
            	        break;

            	}

            	string_literal77=(IToken)Match(input,48,FOLLOW_48_in_firstWhen1219); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_48.Add(string_literal77);

            	WS78=(IToken)Match(input,WS,FOLLOW_WS_in_firstWhen1221); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS78);

            	PushFollow(FOLLOW_sentenceEnd_in_firstWhen1223);
            	sentenceEnd79 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd79.Tree);


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
            	// 159:9: -> ^( WHEN sentenceEnd )
            	{
            	    // SpecFlowLang.g:159:12: ^( WHEN sentenceEnd )
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
    // SpecFlowLang.g:170:1: firstThen : ( WS )? 'Then' WS sentenceEnd -> ^( THEN sentenceEnd ) ;
    public SpecFlowLangParser.firstThen_return firstThen() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.firstThen_return retval = new SpecFlowLangParser.firstThen_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS80 = null;
        IToken string_literal81 = null;
        IToken WS82 = null;
        SpecFlowLangParser.sentenceEnd_return sentenceEnd83 = default(SpecFlowLangParser.sentenceEnd_return);


        object WS80_tree=null;
        object string_literal81_tree=null;
        object WS82_tree=null;
        RewriteRuleTokenStream stream_49 = new RewriteRuleTokenStream(adaptor,"token 49");
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleSubtreeStream stream_sentenceEnd = new RewriteRuleSubtreeStream(adaptor,"rule sentenceEnd");
        try 
    	{
            // SpecFlowLang.g:171:5: ( ( WS )? 'Then' WS sentenceEnd -> ^( THEN sentenceEnd ) )
            // SpecFlowLang.g:171:9: ( WS )? 'Then' WS sentenceEnd
            {
            	// SpecFlowLang.g:171:9: ( WS )?
            	int alt36 = 2;
            	int LA36_0 = input.LA(1);

            	if ( (LA36_0 == WS) )
            	{
            	    alt36 = 1;
            	}
            	switch (alt36) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS80=(IToken)Match(input,WS,FOLLOW_WS_in_firstThen1259); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS80);


            	        }
            	        break;

            	}

            	string_literal81=(IToken)Match(input,49,FOLLOW_49_in_firstThen1262); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_49.Add(string_literal81);

            	WS82=(IToken)Match(input,WS,FOLLOW_WS_in_firstThen1264); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WS.Add(WS82);

            	PushFollow(FOLLOW_sentenceEnd_in_firstThen1266);
            	sentenceEnd83 = sentenceEnd();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_sentenceEnd.Add(sentenceEnd83.Tree);


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
            	// 172:9: -> ^( THEN sentenceEnd )
            	{
            	    // SpecFlowLang.g:172:12: ^( THEN sentenceEnd )
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
    // SpecFlowLang.g:190:1: sentenceEnd : text newlineWithSpaces ( multilineText )? ( table )? -> text ( multilineText )? ( table )? ;
    public SpecFlowLangParser.sentenceEnd_return sentenceEnd() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.sentenceEnd_return retval = new SpecFlowLangParser.sentenceEnd_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.text_return text84 = default(SpecFlowLangParser.text_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces85 = default(SpecFlowLangParser.newlineWithSpaces_return);

        SpecFlowLangParser.multilineText_return multilineText86 = default(SpecFlowLangParser.multilineText_return);

        SpecFlowLangParser.table_return table87 = default(SpecFlowLangParser.table_return);


        RewriteRuleSubtreeStream stream_multilineText = new RewriteRuleSubtreeStream(adaptor,"rule multilineText");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        RewriteRuleSubtreeStream stream_table = new RewriteRuleSubtreeStream(adaptor,"rule table");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLang.g:191:5: ( text newlineWithSpaces ( multilineText )? ( table )? -> text ( multilineText )? ( table )? )
            // SpecFlowLang.g:191:9: text newlineWithSpaces ( multilineText )? ( table )?
            {
            	PushFollow(FOLLOW_text_in_sentenceEnd1303);
            	text84 = text();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_text.Add(text84.Tree);
            	PushFollow(FOLLOW_newlineWithSpaces_in_sentenceEnd1305);
            	newlineWithSpaces85 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces85.Tree);
            	// SpecFlowLang.g:191:32: ( multilineText )?
            	int alt37 = 2;
            	int LA37_0 = input.LA(1);

            	if ( (LA37_0 == WS) )
            	{
            	    int LA37_1 = input.LA(2);

            	    if ( (LA37_1 == 50) )
            	    {
            	        alt37 = 1;
            	    }
            	}
            	else if ( (LA37_0 == 50) )
            	{
            	    alt37 = 1;
            	}
            	switch (alt37) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: multilineText
            	        {
            	        	PushFollow(FOLLOW_multilineText_in_sentenceEnd1307);
            	        	multilineText86 = multilineText();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_multilineText.Add(multilineText86.Tree);

            	        }
            	        break;

            	}

            	// SpecFlowLang.g:191:47: ( table )?
            	int alt38 = 2;
            	int LA38_0 = input.LA(1);

            	if ( (LA38_0 == WS) )
            	{
            	    int LA38_1 = input.LA(2);

            	    if ( (LA38_1 == 51) )
            	    {
            	        alt38 = 1;
            	    }
            	}
            	else if ( (LA38_0 == 51) )
            	{
            	    alt38 = 1;
            	}
            	switch (alt38) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: table
            	        {
            	        	PushFollow(FOLLOW_table_in_sentenceEnd1310);
            	        	table87 = table();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_table.Add(table87.Tree);

            	        }
            	        break;

            	}



            	// AST REWRITE
            	// elements:          text, table, multilineText
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 192:9: -> text ( multilineText )? ( table )?
            	{
            	    adaptor.AddChild(root_0, stream_text.NextTree());
            	    // SpecFlowLang.g:192:17: ( multilineText )?
            	    if ( stream_multilineText.HasNext() )
            	    {
            	        adaptor.AddChild(root_0, stream_multilineText.NextTree());

            	    }
            	    stream_multilineText.Reset();
            	    // SpecFlowLang.g:192:32: ( table )?
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
    // SpecFlowLang.g:195:1: multilineText : indent '\"\"\"' ( WS )? NEWLINE ( multilineTextLine )* ( WS )? '\"\"\"' ( WS )? newlineWithSpaces -> ^( MULTILINETEXT ( multilineTextLine )* indent ) ;
    public SpecFlowLangParser.multilineText_return multilineText() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.multilineText_return retval = new SpecFlowLangParser.multilineText_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken string_literal89 = null;
        IToken WS90 = null;
        IToken NEWLINE91 = null;
        IToken WS93 = null;
        IToken string_literal94 = null;
        IToken WS95 = null;
        SpecFlowLangParser.indent_return indent88 = default(SpecFlowLangParser.indent_return);

        SpecFlowLangParser.multilineTextLine_return multilineTextLine92 = default(SpecFlowLangParser.multilineTextLine_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces96 = default(SpecFlowLangParser.newlineWithSpaces_return);


        object string_literal89_tree=null;
        object WS90_tree=null;
        object NEWLINE91_tree=null;
        object WS93_tree=null;
        object string_literal94_tree=null;
        object WS95_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_NEWLINE = new RewriteRuleTokenStream(adaptor,"token NEWLINE");
        RewriteRuleTokenStream stream_50 = new RewriteRuleTokenStream(adaptor,"token 50");
        RewriteRuleSubtreeStream stream_multilineTextLine = new RewriteRuleSubtreeStream(adaptor,"rule multilineTextLine");
        RewriteRuleSubtreeStream stream_indent = new RewriteRuleSubtreeStream(adaptor,"rule indent");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLang.g:196:5: ( indent '\"\"\"' ( WS )? NEWLINE ( multilineTextLine )* ( WS )? '\"\"\"' ( WS )? newlineWithSpaces -> ^( MULTILINETEXT ( multilineTextLine )* indent ) )
            // SpecFlowLang.g:196:9: indent '\"\"\"' ( WS )? NEWLINE ( multilineTextLine )* ( WS )? '\"\"\"' ( WS )? newlineWithSpaces
            {
            	PushFollow(FOLLOW_indent_in_multilineText1348);
            	indent88 = indent();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_indent.Add(indent88.Tree);
            	string_literal89=(IToken)Match(input,50,FOLLOW_50_in_multilineText1350); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_50.Add(string_literal89);

            	// SpecFlowLang.g:196:22: ( WS )?
            	int alt39 = 2;
            	int LA39_0 = input.LA(1);

            	if ( (LA39_0 == WS) )
            	{
            	    alt39 = 1;
            	}
            	switch (alt39) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS90=(IToken)Match(input,WS,FOLLOW_WS_in_multilineText1352); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS90);


            	        }
            	        break;

            	}

            	NEWLINE91=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_multilineText1355); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_NEWLINE.Add(NEWLINE91);

            	// SpecFlowLang.g:197:9: ( multilineTextLine )*
            	do 
            	{
            	    int alt40 = 2;
            	    int LA40_0 = input.LA(1);

            	    if ( (LA40_0 == WS) )
            	    {
            	        int LA40_1 = input.LA(2);

            	        if ( ((LA40_1 >= AT && LA40_1 <= WORDCHAR) || LA40_1 == NEWLINE) )
            	        {
            	            alt40 = 1;
            	        }


            	    }
            	    else if ( ((LA40_0 >= AT && LA40_0 <= WORDCHAR) || LA40_0 == NEWLINE) )
            	    {
            	        alt40 = 1;
            	    }


            	    switch (alt40) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: multilineTextLine
            			    {
            			    	PushFollow(FOLLOW_multilineTextLine_in_multilineText1365);
            			    	multilineTextLine92 = multilineTextLine();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_multilineTextLine.Add(multilineTextLine92.Tree);

            			    }
            			    break;

            			default:
            			    goto loop40;
            	    }
            	} while (true);

            	loop40:
            		;	// Stops C# compiler whining that label 'loop40' has no statements

            	// SpecFlowLang.g:198:9: ( WS )?
            	int alt41 = 2;
            	int LA41_0 = input.LA(1);

            	if ( (LA41_0 == WS) )
            	{
            	    alt41 = 1;
            	}
            	switch (alt41) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS93=(IToken)Match(input,WS,FOLLOW_WS_in_multilineText1376); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS93);


            	        }
            	        break;

            	}

            	string_literal94=(IToken)Match(input,50,FOLLOW_50_in_multilineText1379); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_50.Add(string_literal94);

            	// SpecFlowLang.g:198:19: ( WS )?
            	int alt42 = 2;
            	int LA42_0 = input.LA(1);

            	if ( (LA42_0 == WS) )
            	{
            	    int LA42_1 = input.LA(2);

            	    if ( (synpred44_SpecFlowLang()) )
            	    {
            	        alt42 = 1;
            	    }
            	}
            	switch (alt42) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS95=(IToken)Match(input,WS,FOLLOW_WS_in_multilineText1381); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS95);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_multilineText1384);
            	newlineWithSpaces96 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces96.Tree);


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
            	// 199:9: -> ^( MULTILINETEXT ( multilineTextLine )* indent )
            	{
            	    // SpecFlowLang.g:199:12: ^( MULTILINETEXT ( multilineTextLine )* indent )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(MULTILINETEXT, "MULTILINETEXT"), root_1);

            	    // SpecFlowLang.g:199:28: ( multilineTextLine )*
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
    // SpecFlowLang.g:202:1: indent : ( WS )? -> ^( INDENT ( WS )? ) ;
    public SpecFlowLangParser.indent_return indent() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.indent_return retval = new SpecFlowLangParser.indent_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS97 = null;

        object WS97_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");

        try 
    	{
            // SpecFlowLang.g:203:5: ( ( WS )? -> ^( INDENT ( WS )? ) )
            // SpecFlowLang.g:203:9: ( WS )?
            {
            	// SpecFlowLang.g:203:9: ( WS )?
            	int alt43 = 2;
            	int LA43_0 = input.LA(1);

            	if ( (LA43_0 == WS) )
            	{
            	    alt43 = 1;
            	}
            	switch (alt43) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS97=(IToken)Match(input,WS,FOLLOW_WS_in_indent1422); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS97);


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
            	// 203:13: -> ^( INDENT ( WS )? )
            	{
            	    // SpecFlowLang.g:203:16: ^( INDENT ( WS )? )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(INDENT, "INDENT"), root_1);

            	    // SpecFlowLang.g:203:25: ( WS )?
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
    // SpecFlowLang.g:206:1: multilineTextLine : ( WS )? ( text )? NEWLINE -> ^( LINE ( WS )? ( text )? NEWLINE ) ;
    public SpecFlowLangParser.multilineTextLine_return multilineTextLine() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.multilineTextLine_return retval = new SpecFlowLangParser.multilineTextLine_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS98 = null;
        IToken NEWLINE100 = null;
        SpecFlowLangParser.text_return text99 = default(SpecFlowLangParser.text_return);


        object WS98_tree=null;
        object NEWLINE100_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_NEWLINE = new RewriteRuleTokenStream(adaptor,"token NEWLINE");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        try 
    	{
            // SpecFlowLang.g:207:5: ( ( WS )? ( text )? NEWLINE -> ^( LINE ( WS )? ( text )? NEWLINE ) )
            // SpecFlowLang.g:207:9: ( WS )? ( text )? NEWLINE
            {
            	// SpecFlowLang.g:207:9: ( WS )?
            	int alt44 = 2;
            	int LA44_0 = input.LA(1);

            	if ( (LA44_0 == WS) )
            	{
            	    alt44 = 1;
            	}
            	switch (alt44) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS98=(IToken)Match(input,WS,FOLLOW_WS_in_multilineTextLine1451); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS98);


            	        }
            	        break;

            	}

            	// SpecFlowLang.g:207:13: ( text )?
            	int alt45 = 2;
            	int LA45_0 = input.LA(1);

            	if ( ((LA45_0 >= AT && LA45_0 <= WORDCHAR)) )
            	{
            	    alt45 = 1;
            	}
            	switch (alt45) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: text
            	        {
            	        	PushFollow(FOLLOW_text_in_multilineTextLine1454);
            	        	text99 = text();
            	        	state.followingStackPointer--;
            	        	if (state.failed) return retval;
            	        	if ( (state.backtracking==0) ) stream_text.Add(text99.Tree);

            	        }
            	        break;

            	}

            	NEWLINE100=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_multilineTextLine1457); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_NEWLINE.Add(NEWLINE100);



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
            	// 208:9: -> ^( LINE ( WS )? ( text )? NEWLINE )
            	{
            	    // SpecFlowLang.g:208:12: ^( LINE ( WS )? ( text )? NEWLINE )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(LINE, "LINE"), root_1);

            	    // SpecFlowLang.g:208:19: ( WS )?
            	    if ( stream_WS.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_WS.NextNode());

            	    }
            	    stream_WS.Reset();
            	    // SpecFlowLang.g:208:23: ( text )?
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
    // SpecFlowLang.g:211:1: table : tableRow ( tableRow )+ -> ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) ) ;
    public SpecFlowLangParser.table_return table() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.table_return retval = new SpecFlowLangParser.table_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.tableRow_return tableRow101 = default(SpecFlowLangParser.tableRow_return);

        SpecFlowLangParser.tableRow_return tableRow102 = default(SpecFlowLangParser.tableRow_return);


        RewriteRuleSubtreeStream stream_tableRow = new RewriteRuleSubtreeStream(adaptor,"rule tableRow");
        try 
    	{
            // SpecFlowLang.g:212:5: ( tableRow ( tableRow )+ -> ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) ) )
            // SpecFlowLang.g:212:9: tableRow ( tableRow )+
            {
            	PushFollow(FOLLOW_tableRow_in_table1498);
            	tableRow101 = tableRow();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_tableRow.Add(tableRow101.Tree);
            	// SpecFlowLang.g:212:18: ( tableRow )+
            	int cnt46 = 0;
            	do 
            	{
            	    int alt46 = 2;
            	    int LA46_0 = input.LA(1);

            	    if ( (LA46_0 == WS) )
            	    {
            	        int LA46_1 = input.LA(2);

            	        if ( (LA46_1 == 51) )
            	        {
            	            alt46 = 1;
            	        }


            	    }
            	    else if ( (LA46_0 == 51) )
            	    {
            	        alt46 = 1;
            	    }


            	    switch (alt46) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: tableRow
            			    {
            			    	PushFollow(FOLLOW_tableRow_in_table1500);
            			    	tableRow102 = tableRow();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_tableRow.Add(tableRow102.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt46 >= 1 ) goto loop46;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee46 =
            		                new EarlyExitException(46, input);
            		            throw eee46;
            	    }
            	    cnt46++;
            	} while (true);

            	loop46:
            		;	// Stops C# compiler whinging that label 'loop46' has no statements



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
            	// 213:9: -> ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) )
            	{
            	    // SpecFlowLang.g:213:12: ^( TABLE ^( HEADER tableRow ) ^( BODY ( tableRow )+ ) )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TABLE, "TABLE"), root_1);

            	    // SpecFlowLang.g:213:20: ^( HEADER tableRow )
            	    {
            	    object root_2 = (object)adaptor.GetNilNode();
            	    root_2 = (object)adaptor.BecomeRoot((object)adaptor.Create(HEADER, "HEADER"), root_2);

            	    adaptor.AddChild(root_2, stream_tableRow.NextTree());

            	    adaptor.AddChild(root_1, root_2);
            	    }
            	    // SpecFlowLang.g:213:39: ^( BODY ( tableRow )+ )
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
    // SpecFlowLang.g:216:1: tableRow : ( WS )? '|' ( tableCell )+ ( WS )? newlineWithSpaces -> ^( ROW ( tableCell )+ ) ;
    public SpecFlowLangParser.tableRow_return tableRow() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.tableRow_return retval = new SpecFlowLangParser.tableRow_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS103 = null;
        IToken char_literal104 = null;
        IToken WS106 = null;
        SpecFlowLangParser.tableCell_return tableCell105 = default(SpecFlowLangParser.tableCell_return);

        SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces107 = default(SpecFlowLangParser.newlineWithSpaces_return);


        object WS103_tree=null;
        object char_literal104_tree=null;
        object WS106_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_51 = new RewriteRuleTokenStream(adaptor,"token 51");
        RewriteRuleSubtreeStream stream_tableCell = new RewriteRuleSubtreeStream(adaptor,"rule tableCell");
        RewriteRuleSubtreeStream stream_newlineWithSpaces = new RewriteRuleSubtreeStream(adaptor,"rule newlineWithSpaces");
        try 
    	{
            // SpecFlowLang.g:217:5: ( ( WS )? '|' ( tableCell )+ ( WS )? newlineWithSpaces -> ^( ROW ( tableCell )+ ) )
            // SpecFlowLang.g:217:9: ( WS )? '|' ( tableCell )+ ( WS )? newlineWithSpaces
            {
            	// SpecFlowLang.g:217:9: ( WS )?
            	int alt47 = 2;
            	int LA47_0 = input.LA(1);

            	if ( (LA47_0 == WS) )
            	{
            	    alt47 = 1;
            	}
            	switch (alt47) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS103=(IToken)Match(input,WS,FOLLOW_WS_in_tableRow1547); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS103);


            	        }
            	        break;

            	}

            	char_literal104=(IToken)Match(input,51,FOLLOW_51_in_tableRow1550); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_51.Add(char_literal104);

            	// SpecFlowLang.g:217:17: ( tableCell )+
            	int cnt48 = 0;
            	do 
            	{
            	    int alt48 = 2;
            	    int LA48_0 = input.LA(1);

            	    if ( (LA48_0 == WS) )
            	    {
            	        int LA48_1 = input.LA(2);

            	        if ( ((LA48_1 >= AT && LA48_1 <= WORDCHAR)) )
            	        {
            	            alt48 = 1;
            	        }


            	    }
            	    else if ( ((LA48_0 >= AT && LA48_0 <= WORDCHAR)) )
            	    {
            	        alt48 = 1;
            	    }


            	    switch (alt48) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: tableCell
            			    {
            			    	PushFollow(FOLLOW_tableCell_in_tableRow1552);
            			    	tableCell105 = tableCell();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_tableCell.Add(tableCell105.Tree);

            			    }
            			    break;

            			default:
            			    if ( cnt48 >= 1 ) goto loop48;
            			    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            		            EarlyExitException eee48 =
            		                new EarlyExitException(48, input);
            		            throw eee48;
            	    }
            	    cnt48++;
            	} while (true);

            	loop48:
            		;	// Stops C# compiler whinging that label 'loop48' has no statements

            	// SpecFlowLang.g:217:28: ( WS )?
            	int alt49 = 2;
            	int LA49_0 = input.LA(1);

            	if ( (LA49_0 == WS) )
            	{
            	    int LA49_1 = input.LA(2);

            	    if ( (synpred51_SpecFlowLang()) )
            	    {
            	        alt49 = 1;
            	    }
            	}
            	switch (alt49) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS106=(IToken)Match(input,WS,FOLLOW_WS_in_tableRow1555); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS106);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_newlineWithSpaces_in_tableRow1558);
            	newlineWithSpaces107 = newlineWithSpaces();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_newlineWithSpaces.Add(newlineWithSpaces107.Tree);


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
            	// 218:9: -> ^( ROW ( tableCell )+ )
            	{
            	    // SpecFlowLang.g:218:12: ^( ROW ( tableCell )+ )
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
    // SpecFlowLang.g:221:1: tableCell : ( WS )? text ( WS )? '|' -> ^( CELL text ) ;
    public SpecFlowLangParser.tableCell_return tableCell() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.tableCell_return retval = new SpecFlowLangParser.tableCell_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS108 = null;
        IToken WS110 = null;
        IToken char_literal111 = null;
        SpecFlowLangParser.text_return text109 = default(SpecFlowLangParser.text_return);


        object WS108_tree=null;
        object WS110_tree=null;
        object char_literal111_tree=null;
        RewriteRuleTokenStream stream_WS = new RewriteRuleTokenStream(adaptor,"token WS");
        RewriteRuleTokenStream stream_51 = new RewriteRuleTokenStream(adaptor,"token 51");
        RewriteRuleSubtreeStream stream_text = new RewriteRuleSubtreeStream(adaptor,"rule text");
        try 
    	{
            // SpecFlowLang.g:222:5: ( ( WS )? text ( WS )? '|' -> ^( CELL text ) )
            // SpecFlowLang.g:222:9: ( WS )? text ( WS )? '|'
            {
            	// SpecFlowLang.g:222:9: ( WS )?
            	int alt50 = 2;
            	int LA50_0 = input.LA(1);

            	if ( (LA50_0 == WS) )
            	{
            	    alt50 = 1;
            	}
            	switch (alt50) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS108=(IToken)Match(input,WS,FOLLOW_WS_in_tableCell1594); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS108);


            	        }
            	        break;

            	}

            	PushFollow(FOLLOW_text_in_tableCell1597);
            	text109 = text();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_text.Add(text109.Tree);
            	// SpecFlowLang.g:222:18: ( WS )?
            	int alt51 = 2;
            	int LA51_0 = input.LA(1);

            	if ( (LA51_0 == WS) )
            	{
            	    alt51 = 1;
            	}
            	switch (alt51) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS110=(IToken)Match(input,WS,FOLLOW_WS_in_tableCell1599); if (state.failed) return retval; 
            	        	if ( (state.backtracking==0) ) stream_WS.Add(WS110);


            	        }
            	        break;

            	}

            	char_literal111=(IToken)Match(input,51,FOLLOW_51_in_tableCell1602); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_51.Add(char_literal111);



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
            	// 223:9: -> ^( CELL text )
            	{
            	    // SpecFlowLang.g:223:12: ^( CELL text )
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
    // SpecFlowLang.g:226:1: descriptionLineText : WORDCHAR ( textRest )* -> ^( TEXT WORDCHAR ( textRest )* ) ;
    public SpecFlowLangParser.descriptionLineText_return descriptionLineText() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.descriptionLineText_return retval = new SpecFlowLangParser.descriptionLineText_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WORDCHAR112 = null;
        SpecFlowLangParser.textRest_return textRest113 = default(SpecFlowLangParser.textRest_return);


        object WORDCHAR112_tree=null;
        RewriteRuleTokenStream stream_WORDCHAR = new RewriteRuleTokenStream(adaptor,"token WORDCHAR");
        RewriteRuleSubtreeStream stream_textRest = new RewriteRuleSubtreeStream(adaptor,"rule textRest");
        try 
    	{
            // SpecFlowLang.g:227:5: ( WORDCHAR ( textRest )* -> ^( TEXT WORDCHAR ( textRest )* ) )
            // SpecFlowLang.g:227:9: WORDCHAR ( textRest )*
            {
            	WORDCHAR112=(IToken)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_descriptionLineText1637); if (state.failed) return retval; 
            	if ( (state.backtracking==0) ) stream_WORDCHAR.Add(WORDCHAR112);

            	// SpecFlowLang.g:227:18: ( textRest )*
            	do 
            	{
            	    int alt52 = 2;
            	    int LA52_0 = input.LA(1);

            	    if ( (LA52_0 == WS) )
            	    {
            	        int LA52_1 = input.LA(2);

            	        if ( ((LA52_1 >= WS && LA52_1 <= WORDCHAR)) )
            	        {
            	            alt52 = 1;
            	        }


            	    }
            	    else if ( ((LA52_0 >= AT && LA52_0 <= WORDCHAR)) )
            	    {
            	        alt52 = 1;
            	    }


            	    switch (alt52) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: textRest
            			    {
            			    	PushFollow(FOLLOW_textRest_in_descriptionLineText1639);
            			    	textRest113 = textRest();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_textRest.Add(textRest113.Tree);

            			    }
            			    break;

            			default:
            			    goto loop52;
            	    }
            	} while (true);

            	loop52:
            		;	// Stops C# compiler whining that label 'loop52' has no statements



            	// AST REWRITE
            	// elements:          WORDCHAR, textRest
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	if ( (state.backtracking==0) ) {
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (object)adaptor.GetNilNode();
            	// 228:9: -> ^( TEXT WORDCHAR ( textRest )* )
            	{
            	    // SpecFlowLang.g:228:12: ^( TEXT WORDCHAR ( textRest )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TEXT, "TEXT"), root_1);

            	    adaptor.AddChild(root_1, stream_WORDCHAR.NextNode());
            	    // SpecFlowLang.g:228:28: ( textRest )*
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
    // SpecFlowLang.g:231:1: text : wordchar ( textRest )* -> ^( TEXT wordchar ( textRest )* ) ;
    public SpecFlowLangParser.text_return text() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.text_return retval = new SpecFlowLangParser.text_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.wordchar_return wordchar114 = default(SpecFlowLangParser.wordchar_return);

        SpecFlowLangParser.textRest_return textRest115 = default(SpecFlowLangParser.textRest_return);


        RewriteRuleSubtreeStream stream_textRest = new RewriteRuleSubtreeStream(adaptor,"rule textRest");
        RewriteRuleSubtreeStream stream_wordchar = new RewriteRuleSubtreeStream(adaptor,"rule wordchar");
        try 
    	{
            // SpecFlowLang.g:232:5: ( wordchar ( textRest )* -> ^( TEXT wordchar ( textRest )* ) )
            // SpecFlowLang.g:232:9: wordchar ( textRest )*
            {
            	PushFollow(FOLLOW_wordchar_in_text1678);
            	wordchar114 = wordchar();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_wordchar.Add(wordchar114.Tree);
            	// SpecFlowLang.g:232:18: ( textRest )*
            	do 
            	{
            	    int alt53 = 2;
            	    int LA53_0 = input.LA(1);

            	    if ( (LA53_0 == WS) )
            	    {
            	        int LA53_1 = input.LA(2);

            	        if ( ((LA53_1 >= WS && LA53_1 <= WORDCHAR)) )
            	        {
            	            alt53 = 1;
            	        }


            	    }
            	    else if ( ((LA53_0 >= AT && LA53_0 <= WORDCHAR)) )
            	    {
            	        alt53 = 1;
            	    }


            	    switch (alt53) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: textRest
            			    {
            			    	PushFollow(FOLLOW_textRest_in_text1680);
            			    	textRest115 = textRest();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_textRest.Add(textRest115.Tree);

            			    }
            			    break;

            			default:
            			    goto loop53;
            	    }
            	} while (true);

            	loop53:
            		;	// Stops C# compiler whining that label 'loop53' has no statements



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
            	// 233:9: -> ^( TEXT wordchar ( textRest )* )
            	{
            	    // SpecFlowLang.g:233:12: ^( TEXT wordchar ( textRest )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TEXT, "TEXT"), root_1);

            	    adaptor.AddChild(root_1, stream_wordchar.NextTree());
            	    // SpecFlowLang.g:233:28: ( textRest )*
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
    // SpecFlowLang.g:235:1: textRest : ( WS textRest | wordchar );
    public SpecFlowLangParser.textRest_return textRest() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.textRest_return retval = new SpecFlowLangParser.textRest_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS116 = null;
        SpecFlowLangParser.textRest_return textRest117 = default(SpecFlowLangParser.textRest_return);

        SpecFlowLangParser.wordchar_return wordchar118 = default(SpecFlowLangParser.wordchar_return);


        object WS116_tree=null;

        try 
    	{
            // SpecFlowLang.g:236:5: ( WS textRest | wordchar )
            int alt54 = 2;
            int LA54_0 = input.LA(1);

            if ( (LA54_0 == WS) )
            {
                alt54 = 1;
            }
            else if ( ((LA54_0 >= AT && LA54_0 <= WORDCHAR)) )
            {
                alt54 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return retval;}
                NoViableAltException nvae_d54s0 =
                    new NoViableAltException("", 54, 0, input);

                throw nvae_d54s0;
            }
            switch (alt54) 
            {
                case 1 :
                    // SpecFlowLang.g:236:9: WS textRest
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WS116=(IToken)Match(input,WS,FOLLOW_WS_in_textRest1718); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WS116_tree = (object)adaptor.Create(WS116);
                    		adaptor.AddChild(root_0, WS116_tree);
                    	}
                    	PushFollow(FOLLOW_textRest_in_textRest1720);
                    	textRest117 = textRest();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, textRest117.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:237:9: wordchar
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_wordchar_in_textRest1730);
                    	wordchar118 = wordchar();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, wordchar118.Tree);

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
    // SpecFlowLang.g:240:1: title : wordchar ( titleRest )* -> ^( TEXT wordchar ( titleRest )* ) ;
    public SpecFlowLangParser.title_return title() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.title_return retval = new SpecFlowLangParser.title_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        SpecFlowLangParser.wordchar_return wordchar119 = default(SpecFlowLangParser.wordchar_return);

        SpecFlowLangParser.titleRest_return titleRest120 = default(SpecFlowLangParser.titleRest_return);


        RewriteRuleSubtreeStream stream_wordchar = new RewriteRuleSubtreeStream(adaptor,"rule wordchar");
        RewriteRuleSubtreeStream stream_titleRest = new RewriteRuleSubtreeStream(adaptor,"rule titleRest");
        try 
    	{
            // SpecFlowLang.g:241:5: ( wordchar ( titleRest )* -> ^( TEXT wordchar ( titleRest )* ) )
            // SpecFlowLang.g:241:9: wordchar ( titleRest )*
            {
            	PushFollow(FOLLOW_wordchar_in_title1749);
            	wordchar119 = wordchar();
            	state.followingStackPointer--;
            	if (state.failed) return retval;
            	if ( (state.backtracking==0) ) stream_wordchar.Add(wordchar119.Tree);
            	// SpecFlowLang.g:241:18: ( titleRest )*
            	do 
            	{
            	    int alt55 = 2;
            	    alt55 = dfa55.Predict(input);
            	    switch (alt55) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:0:0: titleRest
            			    {
            			    	PushFollow(FOLLOW_titleRest_in_title1751);
            			    	titleRest120 = titleRest();
            			    	state.followingStackPointer--;
            			    	if (state.failed) return retval;
            			    	if ( (state.backtracking==0) ) stream_titleRest.Add(titleRest120.Tree);

            			    }
            			    break;

            			default:
            			    goto loop55;
            	    }
            	} while (true);

            	loop55:
            		;	// Stops C# compiler whining that label 'loop55' has no statements



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
            	// 242:9: -> ^( TEXT wordchar ( titleRest )* )
            	{
            	    // SpecFlowLang.g:242:12: ^( TEXT wordchar ( titleRest )* )
            	    {
            	    object root_1 = (object)adaptor.GetNilNode();
            	    root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(TEXT, "TEXT"), root_1);

            	    adaptor.AddChild(root_1, stream_wordchar.NextTree());
            	    // SpecFlowLang.g:242:28: ( titleRest )*
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
    // SpecFlowLang.g:245:1: titleRest : ( WS titleRest | NEWLINE titleRest | wordchar );
    public SpecFlowLangParser.titleRest_return titleRest() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.titleRest_return retval = new SpecFlowLangParser.titleRest_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS121 = null;
        IToken NEWLINE123 = null;
        SpecFlowLangParser.titleRest_return titleRest122 = default(SpecFlowLangParser.titleRest_return);

        SpecFlowLangParser.titleRest_return titleRest124 = default(SpecFlowLangParser.titleRest_return);

        SpecFlowLangParser.wordchar_return wordchar125 = default(SpecFlowLangParser.wordchar_return);


        object WS121_tree=null;
        object NEWLINE123_tree=null;

        try 
    	{
            // SpecFlowLang.g:246:5: ( WS titleRest | NEWLINE titleRest | wordchar )
            int alt56 = 3;
            switch ( input.LA(1) ) 
            {
            case WS:
            	{
                alt56 = 1;
                }
                break;
            case NEWLINE:
            	{
                alt56 = 2;
                }
                break;
            case AT:
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
                    // SpecFlowLang.g:246:9: WS titleRest
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WS121=(IToken)Match(input,WS,FOLLOW_WS_in_titleRest1790); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WS121_tree = (object)adaptor.Create(WS121);
                    		adaptor.AddChild(root_0, WS121_tree);
                    	}
                    	PushFollow(FOLLOW_titleRest_in_titleRest1792);
                    	titleRest122 = titleRest();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRest122.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:247:9: NEWLINE titleRest
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	NEWLINE123=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_titleRest1802); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{NEWLINE123_tree = (object)adaptor.Create(NEWLINE123);
                    		adaptor.AddChild(root_0, NEWLINE123_tree);
                    	}
                    	PushFollow(FOLLOW_titleRest_in_titleRest1804);
                    	titleRest124 = titleRest();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRest124.Tree);

                    }
                    break;
                case 3 :
                    // SpecFlowLang.g:248:9: wordchar
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_wordchar_in_titleRest1814);
                    	wordchar125 = wordchar();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, wordchar125.Tree);

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
    // SpecFlowLang.g:251:1: titleRestLine : ( NEWLINE titleRestLine | WS titleRestLine | WORDCHAR );
    public SpecFlowLangParser.titleRestLine_return titleRestLine() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.titleRestLine_return retval = new SpecFlowLangParser.titleRestLine_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken NEWLINE126 = null;
        IToken WS128 = null;
        IToken WORDCHAR130 = null;
        SpecFlowLangParser.titleRestLine_return titleRestLine127 = default(SpecFlowLangParser.titleRestLine_return);

        SpecFlowLangParser.titleRestLine_return titleRestLine129 = default(SpecFlowLangParser.titleRestLine_return);


        object NEWLINE126_tree=null;
        object WS128_tree=null;
        object WORDCHAR130_tree=null;

        try 
    	{
            // SpecFlowLang.g:252:5: ( NEWLINE titleRestLine | WS titleRestLine | WORDCHAR )
            int alt57 = 3;
            switch ( input.LA(1) ) 
            {
            case NEWLINE:
            	{
                alt57 = 1;
                }
                break;
            case WS:
            	{
                alt57 = 2;
                }
                break;
            case WORDCHAR:
            	{
                alt57 = 3;
                }
                break;
            	default:
            	    if ( state.backtracking > 0 ) {state.failed = true; return retval;}
            	    NoViableAltException nvae_d57s0 =
            	        new NoViableAltException("", 57, 0, input);

            	    throw nvae_d57s0;
            }

            switch (alt57) 
            {
                case 1 :
                    // SpecFlowLang.g:252:9: NEWLINE titleRestLine
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	NEWLINE126=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_titleRestLine1833); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{NEWLINE126_tree = (object)adaptor.Create(NEWLINE126);
                    		adaptor.AddChild(root_0, NEWLINE126_tree);
                    	}
                    	PushFollow(FOLLOW_titleRestLine_in_titleRestLine1835);
                    	titleRestLine127 = titleRestLine();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRestLine127.Tree);

                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:253:9: WS titleRestLine
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WS128=(IToken)Match(input,WS,FOLLOW_WS_in_titleRestLine1845); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WS128_tree = (object)adaptor.Create(WS128);
                    		adaptor.AddChild(root_0, WS128_tree);
                    	}
                    	PushFollow(FOLLOW_titleRestLine_in_titleRestLine1847);
                    	titleRestLine129 = titleRestLine();
                    	state.followingStackPointer--;
                    	if (state.failed) return retval;
                    	if ( state.backtracking == 0 ) adaptor.AddChild(root_0, titleRestLine129.Tree);

                    }
                    break;
                case 3 :
                    // SpecFlowLang.g:254:9: WORDCHAR
                    {
                    	root_0 = (object)adaptor.GetNilNode();

                    	WORDCHAR130=(IToken)Match(input,WORDCHAR,FOLLOW_WORDCHAR_in_titleRestLine1857); if (state.failed) return retval;
                    	if ( state.backtracking == 0 )
                    	{WORDCHAR130_tree = (object)adaptor.Create(WORDCHAR130);
                    		adaptor.AddChild(root_0, WORDCHAR130_tree);
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
    // SpecFlowLang.g:257:1: wordchar : ( WORDCHAR | AT );
    public SpecFlowLangParser.wordchar_return wordchar() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.wordchar_return retval = new SpecFlowLangParser.wordchar_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken set131 = null;

        object set131_tree=null;

        try 
    	{
            // SpecFlowLang.g:258:5: ( WORDCHAR | AT )
            // SpecFlowLang.g:
            {
            	root_0 = (object)adaptor.GetNilNode();

            	set131 = (IToken)input.LT(1);
            	if ( (input.LA(1) >= AT && input.LA(1) <= WORDCHAR) ) 
            	{
            	    input.Consume();
            	    if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (object)adaptor.Create(set131));
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
    // SpecFlowLang.g:262:1: newlineWithSpaces : ( WS )? NEWLINE ( ( WS )? NEWLINE )* ;
    public SpecFlowLangParser.newlineWithSpaces_return newlineWithSpaces() // throws RecognitionException [1]
    {   
        SpecFlowLangParser.newlineWithSpaces_return retval = new SpecFlowLangParser.newlineWithSpaces_return();
        retval.Start = input.LT(1);

        object root_0 = null;

        IToken WS132 = null;
        IToken NEWLINE133 = null;
        IToken WS134 = null;
        IToken NEWLINE135 = null;

        object WS132_tree=null;
        object NEWLINE133_tree=null;
        object WS134_tree=null;
        object NEWLINE135_tree=null;

        try 
    	{
            // SpecFlowLang.g:263:5: ( ( WS )? NEWLINE ( ( WS )? NEWLINE )* )
            // SpecFlowLang.g:263:9: ( WS )? NEWLINE ( ( WS )? NEWLINE )*
            {
            	root_0 = (object)adaptor.GetNilNode();

            	// SpecFlowLang.g:263:9: ( WS )?
            	int alt58 = 2;
            	int LA58_0 = input.LA(1);

            	if ( (LA58_0 == WS) )
            	{
            	    alt58 = 1;
            	}
            	switch (alt58) 
            	{
            	    case 1 :
            	        // SpecFlowLang.g:0:0: WS
            	        {
            	        	WS132=(IToken)Match(input,WS,FOLLOW_WS_in_newlineWithSpaces1905); if (state.failed) return retval;
            	        	if ( state.backtracking == 0 )
            	        	{WS132_tree = (object)adaptor.Create(WS132);
            	        		adaptor.AddChild(root_0, WS132_tree);
            	        	}

            	        }
            	        break;

            	}

            	NEWLINE133=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_newlineWithSpaces1908); if (state.failed) return retval;
            	if ( state.backtracking == 0 )
            	{NEWLINE133_tree = (object)adaptor.Create(NEWLINE133);
            		adaptor.AddChild(root_0, NEWLINE133_tree);
            	}
            	// SpecFlowLang.g:263:21: ( ( WS )? NEWLINE )*
            	do 
            	{
            	    int alt60 = 2;
            	    int LA60_0 = input.LA(1);

            	    if ( (LA60_0 == WS) )
            	    {
            	        int LA60_1 = input.LA(2);

            	        if ( (LA60_1 == NEWLINE) )
            	        {
            	            alt60 = 1;
            	        }


            	    }
            	    else if ( (LA60_0 == NEWLINE) )
            	    {
            	        alt60 = 1;
            	    }


            	    switch (alt60) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:263:22: ( WS )? NEWLINE
            			    {
            			    	// SpecFlowLang.g:263:22: ( WS )?
            			    	int alt59 = 2;
            			    	int LA59_0 = input.LA(1);

            			    	if ( (LA59_0 == WS) )
            			    	{
            			    	    alt59 = 1;
            			    	}
            			    	switch (alt59) 
            			    	{
            			    	    case 1 :
            			    	        // SpecFlowLang.g:0:0: WS
            			    	        {
            			    	        	WS134=(IToken)Match(input,WS,FOLLOW_WS_in_newlineWithSpaces1911); if (state.failed) return retval;
            			    	        	if ( state.backtracking == 0 )
            			    	        	{WS134_tree = (object)adaptor.Create(WS134);
            			    	        		adaptor.AddChild(root_0, WS134_tree);
            			    	        	}

            			    	        }
            			    	        break;

            			    	}

            			    	NEWLINE135=(IToken)Match(input,NEWLINE,FOLLOW_NEWLINE_in_newlineWithSpaces1914); if (state.failed) return retval;
            			    	if ( state.backtracking == 0 )
            			    	{NEWLINE135_tree = (object)adaptor.Create(NEWLINE135);
            			    		adaptor.AddChild(root_0, NEWLINE135_tree);
            			    	}

            			    }
            			    break;

            			default:
            			    goto loop60;
            	    }
            	} while (true);

            	loop60:
            		;	// Stops C# compiler whining that label 'loop60' has no statements


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

    // $ANTLR start "synpred26_SpecFlowLang"
    public void synpred26_SpecFlowLang_fragment() {
        // SpecFlowLang.g:110:40: ( WS )
        // SpecFlowLang.g:110:40: WS
        {
        	Match(input,WS,FOLLOW_WS_in_synpred26_SpecFlowLang879); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred26_SpecFlowLang"

    // $ANTLR start "synpred44_SpecFlowLang"
    public void synpred44_SpecFlowLang_fragment() {
        // SpecFlowLang.g:198:19: ( WS )
        // SpecFlowLang.g:198:19: WS
        {
        	Match(input,WS,FOLLOW_WS_in_synpred44_SpecFlowLang1381); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred44_SpecFlowLang"

    // $ANTLR start "synpred51_SpecFlowLang"
    public void synpred51_SpecFlowLang_fragment() {
        // SpecFlowLang.g:217:28: ( WS )
        // SpecFlowLang.g:217:28: WS
        {
        	Match(input,WS,FOLLOW_WS_in_synpred51_SpecFlowLang1555); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred51_SpecFlowLang"

    // $ANTLR start "synpred57_SpecFlowLang"
    public void synpred57_SpecFlowLang_fragment() {
        // SpecFlowLang.g:241:18: ( titleRest )
        // SpecFlowLang.g:241:18: titleRest
        {
        	PushFollow(FOLLOW_titleRest_in_synpred57_SpecFlowLang1751);
        	titleRest();
        	state.followingStackPointer--;
        	if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred57_SpecFlowLang"

    // Delegated rules

   	public bool synpred51_SpecFlowLang() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred51_SpecFlowLang_fragment(); // can never throw exception
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
   	public bool synpred44_SpecFlowLang() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred44_SpecFlowLang_fragment(); // can never throw exception
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
   	public bool synpred26_SpecFlowLang() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred26_SpecFlowLang_fragment(); // can never throw exception
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
   	public bool synpred57_SpecFlowLang() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred57_SpecFlowLang_fragment(); // can never throw exception
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
   	protected DFA55 dfa55;
	private void InitializeCyclicDFAs()
	{
    	this.dfa16 = new DFA16(this);
    	this.dfa55 = new DFA55(this);

	    this.dfa55.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA55_SpecialStateTransition);
	}

    const string DFA16_eotS =
        "\x0b\uffff";
    const string DFA16_eofS =
        "\x0b\uffff";
    const string DFA16_minS =
        "\x01\x1e\x01\x1f\x01\x20\x02\uffff\x03\x1e\x01\x29\x01\x22\x01"+
        "\x1e";
    const string DFA16_maxS =
        "\x02\x2a\x01\x20\x02\uffff\x01\x22\x05\x2a";
    const string DFA16_acceptS =
        "\x03\uffff\x01\x01\x01\x02\x06\uffff";
    const string DFA16_specialS =
        "\x0b\uffff}>";
    static readonly string[] DFA16_transitionS = {
            "\x01\x01\x01\x02\x09\uffff\x01\x04\x01\x03",
            "\x01\x02\x09\uffff\x01\x04\x01\x03",
            "\x01\x05",
            "",
            "",
            "\x01\x06\x01\uffff\x01\x05\x01\uffff\x01\x07",
            "\x01\x08\x01\x02\x02\uffff\x01\x07\x06\uffff\x01\x04\x01\x03",
            "\x01\x09\x01\x02\x02\uffff\x01\x0a\x06\uffff\x01\x04\x01\x03",
            "\x01\x04\x01\x03",
            "\x01\x0a\x06\uffff\x01\x04\x01\x03",
            "\x01\x09\x01\x02\x02\uffff\x01\x0a\x06\uffff\x01\x04\x01\x03"
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
            get { return "83:1: scenarioKind : ( scenarioOutline | scenario );"; }
        }

    }

    const string DFA55_eotS =
        "\x5f\uffff";
    const string DFA55_eofS =
        "\x01\x03\x5e\uffff";
    const string DFA55_minS =
        "\x03\x1e\x02\uffff\x1c\x1e\x01\x00\x07\x1e\x01\x00\x04\x1e\x01"+
        "\x00\x02\x1e\x01\x00\x01\x1e\x02\x00\x04\x1e\x01\x00\x02\x1e\x01"+
        "\x00\x01\x1e\x02\x00\x02\x1e\x01\x00\x01\x1e\x02\x00\x01\x1e\x03"+
        "\x00\x02\x1e\x01\x00\x01\x1e\x02\x00\x01\x1e\x03\x00\x01\x1e\x0a"+
        "\x00";
    const string DFA55_maxS =
        "\x02\x22\x01\x31\x02\uffff\x1c\x31\x01\x00\x07\x31\x01\x00\x04"+
        "\x31\x01\x00\x02\x31\x01\x00\x01\x31\x02\x00\x04\x31\x01\x00\x02"+
        "\x31\x01\x00\x01\x31\x02\x00\x02\x31\x01\x00\x01\x31\x02\x00\x01"+
        "\x31\x03\x00\x02\x31\x01\x00\x01\x31\x02\x00\x01\x31\x03\x00\x01"+
        "\x31\x0a\x00";
    const string DFA55_acceptS =
        "\x03\uffff\x01\x02\x01\x01\x5a\uffff";
    const string DFA55_specialS =
        "\x21\uffff\x01\x1d\x07\uffff\x01\x1e\x04\uffff\x01\x1f\x02\uffff"+
        "\x01\x1c\x01\uffff\x01\x1b\x01\x1a\x04\uffff\x01\x19\x02\uffff\x01"+
        "\x18\x01\uffff\x01\x17\x01\x16\x02\uffff\x01\x15\x01\uffff\x01\x14"+
        "\x01\x13\x01\uffff\x01\x12\x01\x11\x01\x10\x02\uffff\x01\x0f\x01"+
        "\uffff\x01\x0e\x01\x0d\x01\uffff\x01\x0c\x01\x0b\x01\x0a\x01\uffff"+
        "\x01\x09\x01\x08\x01\x07\x01\x06\x01\x00\x01\x05\x01\x04\x01\x03"+
        "\x01\x02\x01\x01}>";
    static readonly string[] DFA55_transitionS = {
            "\x01\x01\x02\x04\x01\uffff\x01\x02",
            "\x03\x04\x01\uffff\x01\x05",
            "\x01\x06\x02\x04\x01\uffff\x01\x07\x0c\uffff\x03\x03",
            "",
            "",
            "\x01\x08\x02\x04\x01\uffff\x01\x09\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x0a\x0c\uffff\x03\x03",
            "\x01\x0b\x02\x04\x01\uffff\x01\x0c\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x0d\x0c\uffff\x03\x03",
            "\x01\x0e\x02\x04\x01\uffff\x01\x0f\x0c\uffff\x03\x03",
            "\x01\x10\x02\x04\x01\uffff\x01\x11\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x12\x0c\uffff\x03\x03",
            "\x01\x13\x02\x04\x01\uffff\x01\x14\x0c\uffff\x03\x03",
            "\x01\x15\x02\x04\x01\uffff\x01\x16\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x17\x0c\uffff\x03\x03",
            "\x01\x18\x02\x04\x01\uffff\x01\x19\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x1a\x0c\uffff\x03\x03",
            "\x01\x1b\x02\x04\x01\uffff\x01\x1c\x0c\uffff\x03\x03",
            "\x01\x1d\x02\x04\x01\uffff\x01\x1e\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x1f\x0c\uffff\x03\x03",
            "\x01\x20\x02\x04\x01\uffff\x01\x21\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x22\x0c\uffff\x03\x03",
            "\x01\x23\x02\x04\x01\uffff\x01\x24\x0c\uffff\x03\x03",
            "\x01\x25\x02\x04\x01\uffff\x01\x26\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x27\x0c\uffff\x03\x03",
            "\x01\x28\x02\x04\x01\uffff\x01\x29\x0c\uffff\x03\x03",
            "\x01\x2a\x02\x04\x01\uffff\x01\x2b\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x2c\x0c\uffff\x03\x03",
            "\x01\x2d\x02\x04\x01\uffff\x01\x2e\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x2f\x0c\uffff\x03\x03",
            "\x01\x30\x02\x04\x01\uffff\x01\x31\x0c\uffff\x03\x03",
            "\x01\x32\x02\x04\x01\uffff\x01\x33\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x34\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\x35\x02\x04\x01\uffff\x01\x36\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x37\x0c\uffff\x03\x03",
            "\x01\x38\x02\x04\x01\uffff\x01\x39\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x3a\x0c\uffff\x03\x03",
            "\x01\x3b\x02\x04\x01\uffff\x01\x3c\x0c\uffff\x03\x03",
            "\x01\x3d\x02\x04\x01\uffff\x01\x3e\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x3f\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x40\x0c\uffff\x03\x03",
            "\x01\x41\x02\x04\x01\uffff\x01\x42\x0c\uffff\x03\x03",
            "\x01\x43\x02\x04\x01\uffff\x01\x44\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x45\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\x46\x02\x04\x01\uffff\x01\x47\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x48\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x49\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x4a\x0c\uffff\x03\x03",
            "\x01\x4b\x02\x04\x01\uffff\x01\x4c\x0c\uffff\x03\x03",
            "\x01\x4d\x02\x04\x01\uffff\x01\x4e\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x4f\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\x50\x02\x04\x01\uffff\x01\x51\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x52\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x53\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\x54\x02\x04\x01\uffff\x01\x55\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x56\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x57\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x58\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\x59\x02\x04\x01\uffff\x01\x5a\x0c\uffff\x03\x03",
            "\x03\x04\x01\uffff\x01\x5b\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x5c\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x5d\x0c\uffff\x03\x03",
            "\x01\uffff",
            "\x01\uffff",
            "\x01\uffff",
            "\x03\x04\x01\uffff\x01\x5e\x0c\uffff\x03\x03",
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

    static readonly short[] DFA55_eot = DFA.UnpackEncodedString(DFA55_eotS);
    static readonly short[] DFA55_eof = DFA.UnpackEncodedString(DFA55_eofS);
    static readonly char[] DFA55_min = DFA.UnpackEncodedStringToUnsignedChars(DFA55_minS);
    static readonly char[] DFA55_max = DFA.UnpackEncodedStringToUnsignedChars(DFA55_maxS);
    static readonly short[] DFA55_accept = DFA.UnpackEncodedString(DFA55_acceptS);
    static readonly short[] DFA55_special = DFA.UnpackEncodedString(DFA55_specialS);
    static readonly short[][] DFA55_transition = DFA.UnpackEncodedStringArray(DFA55_transitionS);

    protected class DFA55 : DFA
    {
        public DFA55(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 55;
            this.eot = DFA55_eot;
            this.eof = DFA55_eof;
            this.min = DFA55_min;
            this.max = DFA55_max;
            this.accept = DFA55_accept;
            this.special = DFA55_special;
            this.transition = DFA55_transition;

        }

        override public string Description
        {
            get { return "()* loopback of 241:18: ( titleRest )*"; }
        }

    }


    protected internal int DFA55_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            ITokenStream input = (ITokenStream)_input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA55_89 = input.LA(1);

                   	 
                   	int index55_89 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_89);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA55_94 = input.LA(1);

                   	 
                   	int index55_94 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_94);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA55_93 = input.LA(1);

                   	 
                   	int index55_93 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_93);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA55_92 = input.LA(1);

                   	 
                   	int index55_92 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_92);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA55_91 = input.LA(1);

                   	 
                   	int index55_91 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_91);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA55_90 = input.LA(1);

                   	 
                   	int index55_90 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_90);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA55_88 = input.LA(1);

                   	 
                   	int index55_88 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_88);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 7 : 
                   	int LA55_87 = input.LA(1);

                   	 
                   	int index55_87 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_87);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 8 : 
                   	int LA55_86 = input.LA(1);

                   	 
                   	int index55_86 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_86);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 9 : 
                   	int LA55_85 = input.LA(1);

                   	 
                   	int index55_85 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_85);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 10 : 
                   	int LA55_83 = input.LA(1);

                   	 
                   	int index55_83 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_83);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 11 : 
                   	int LA55_82 = input.LA(1);

                   	 
                   	int index55_82 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_82);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 12 : 
                   	int LA55_81 = input.LA(1);

                   	 
                   	int index55_81 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_81);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 13 : 
                   	int LA55_79 = input.LA(1);

                   	 
                   	int index55_79 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_79);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 14 : 
                   	int LA55_78 = input.LA(1);

                   	 
                   	int index55_78 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_78);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 15 : 
                   	int LA55_76 = input.LA(1);

                   	 
                   	int index55_76 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_76);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 16 : 
                   	int LA55_73 = input.LA(1);

                   	 
                   	int index55_73 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_73);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 17 : 
                   	int LA55_72 = input.LA(1);

                   	 
                   	int index55_72 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_72);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 18 : 
                   	int LA55_71 = input.LA(1);

                   	 
                   	int index55_71 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_71);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 19 : 
                   	int LA55_69 = input.LA(1);

                   	 
                   	int index55_69 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_69);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 20 : 
                   	int LA55_68 = input.LA(1);

                   	 
                   	int index55_68 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_68);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 21 : 
                   	int LA55_66 = input.LA(1);

                   	 
                   	int index55_66 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_66);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 22 : 
                   	int LA55_63 = input.LA(1);

                   	 
                   	int index55_63 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_63);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 23 : 
                   	int LA55_62 = input.LA(1);

                   	 
                   	int index55_62 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_62);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 24 : 
                   	int LA55_60 = input.LA(1);

                   	 
                   	int index55_60 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_60);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 25 : 
                   	int LA55_57 = input.LA(1);

                   	 
                   	int index55_57 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_57);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 26 : 
                   	int LA55_52 = input.LA(1);

                   	 
                   	int index55_52 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_52);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 27 : 
                   	int LA55_51 = input.LA(1);

                   	 
                   	int index55_51 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_51);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 28 : 
                   	int LA55_49 = input.LA(1);

                   	 
                   	int index55_49 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_49);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 29 : 
                   	int LA55_33 = input.LA(1);

                   	 
                   	int index55_33 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_33);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 30 : 
                   	int LA55_41 = input.LA(1);

                   	 
                   	int index55_41 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_41);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 31 : 
                   	int LA55_46 = input.LA(1);

                   	 
                   	int index55_46 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred57_SpecFlowLang()) ) { s = 4; }

                   	else if ( (true) ) { s = 3; }

                   	 
                   	input.Seek(index55_46);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae55 =
            new NoViableAltException(dfa.Description, 55, _s, input);
        dfa.Error(nvae55);
        throw nvae55;
    }
 

    public static readonly BitSet FOLLOW_newlineWithSpaces_in_feature264 = new BitSet(new ulong[]{0x00000100C0000000UL});
    public static readonly BitSet FOLLOW_tags_in_feature275 = new BitSet(new ulong[]{0x0000010040000000UL});
    public static readonly BitSet FOLLOW_WS_in_feature286 = new BitSet(new ulong[]{0x0000010000000000UL});
    public static readonly BitSet FOLLOW_40_in_feature289 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_feature291 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_text_in_feature294 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_feature296 = new BitSet(new ulong[]{0x00000603C0000000UL});
    public static readonly BitSet FOLLOW_descriptionLine_in_feature306 = new BitSet(new ulong[]{0x00000603C0000000UL});
    public static readonly BitSet FOLLOW_background_in_feature317 = new BitSet(new ulong[]{0x00000600C0000000UL});
    public static readonly BitSet FOLLOW_scenarioKind_in_feature328 = new BitSet(new ulong[]{0x00000600C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_feature331 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_EOF_in_feature334 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_tags411 = new BitSet(new ulong[]{0x00000000C0000000UL});
    public static readonly BitSet FOLLOW_tag_in_tags414 = new BitSet(new ulong[]{0x00000000C0000002UL});
    public static readonly BitSet FOLLOW_AT_in_tag451 = new BitSet(new ulong[]{0x0000000100000000UL});
    public static readonly BitSet FOLLOW_word_in_tag453 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_tag456 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_tag458 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_word494 = new BitSet(new ulong[]{0x0000000100000002UL});
    public static readonly BitSet FOLLOW_WS_in_descriptionLine531 = new BitSet(new ulong[]{0x0000000140000000UL});
    public static readonly BitSet FOLLOW_descriptionLineText_in_descriptionLine534 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_descriptionLine536 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_background571 = new BitSet(new ulong[]{0x0000000200000000UL});
    public static readonly BitSet FOLLOW_T_BACKGROUND_in_background574 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_WS_in_background586 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_title_in_background588 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_background601 = new BitSet(new ulong[]{0x0000800040000000UL});
    public static readonly BitSet FOLLOW_givens_in_background603 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_scenarioOutline_in_scenarioKind641 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_scenario_in_scenarioKind652 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_tags_in_scenario671 = new BitSet(new ulong[]{0x0000020040000000UL});
    public static readonly BitSet FOLLOW_WS_in_scenario674 = new BitSet(new ulong[]{0x0000020000000000UL});
    public static readonly BitSet FOLLOW_41_in_scenario677 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_scenario679 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_title_in_scenario691 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_scenario693 = new BitSet(new ulong[]{0x0003800040000000UL});
    public static readonly BitSet FOLLOW_steps_in_scenario704 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_tags_in_scenarioOutline750 = new BitSet(new ulong[]{0x0000040040000000UL});
    public static readonly BitSet FOLLOW_WS_in_scenarioOutline753 = new BitSet(new ulong[]{0x0000040000000000UL});
    public static readonly BitSet FOLLOW_42_in_scenarioOutline756 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_scenarioOutline758 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_title_in_scenarioOutline769 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_scenarioOutline771 = new BitSet(new ulong[]{0x0003800040000000UL});
    public static readonly BitSet FOLLOW_steps_in_scenarioOutline781 = new BitSet(new ulong[]{0x0000180040000000UL});
    public static readonly BitSet FOLLOW_examples_in_scenarioOutline791 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_exampleSet_in_examples833 = new BitSet(new ulong[]{0x0000180040000002UL});
    public static readonly BitSet FOLLOW_WS_in_exampleSet870 = new BitSet(new ulong[]{0x0000180000000000UL});
    public static readonly BitSet FOLLOW_43_in_exampleSet874 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_44_in_exampleSet876 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_exampleSet879 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_text_in_exampleSet890 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_exampleSet893 = new BitSet(new ulong[]{0x0008000040000000UL});
    public static readonly BitSet FOLLOW_table_in_exampleSet895 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstStep_in_steps933 = new BitSet(new ulong[]{0x0003E00040000002UL});
    public static readonly BitSet FOLLOW_nextStep_in_steps935 = new BitSet(new ulong[]{0x0003E00040000002UL});
    public static readonly BitSet FOLLOW_firstGiven_in_firstStep968 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstWhen_in_firstStep977 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstThen_in_firstStep986 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstStep_in_nextStep1008 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstAnd_in_nextStep1017 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstBut_in_nextStep1026 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstAnd1049 = new BitSet(new ulong[]{0x0000200000000000UL});
    public static readonly BitSet FOLLOW_45_in_firstAnd1052 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_WS_in_firstAnd1054 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstAnd1056 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstBut1091 = new BitSet(new ulong[]{0x0000400000000000UL});
    public static readonly BitSet FOLLOW_46_in_firstBut1094 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_WS_in_firstBut1096 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstBut1098 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_firstGiven_in_givens1133 = new BitSet(new ulong[]{0x0003E00040000002UL});
    public static readonly BitSet FOLLOW_nextStep_in_givens1135 = new BitSet(new ulong[]{0x0003E00040000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstGiven1173 = new BitSet(new ulong[]{0x0000800000000000UL});
    public static readonly BitSet FOLLOW_47_in_firstGiven1176 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_WS_in_firstGiven1178 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstGiven1180 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstWhen1216 = new BitSet(new ulong[]{0x0001000000000000UL});
    public static readonly BitSet FOLLOW_48_in_firstWhen1219 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_WS_in_firstWhen1221 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstWhen1223 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_firstThen1259 = new BitSet(new ulong[]{0x0002000000000000UL});
    public static readonly BitSet FOLLOW_49_in_firstThen1262 = new BitSet(new ulong[]{0x0000000040000000UL});
    public static readonly BitSet FOLLOW_WS_in_firstThen1264 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_sentenceEnd_in_firstThen1266 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_text_in_sentenceEnd1303 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_sentenceEnd1305 = new BitSet(new ulong[]{0x000C000040000002UL});
    public static readonly BitSet FOLLOW_multilineText_in_sentenceEnd1307 = new BitSet(new ulong[]{0x0008000040000002UL});
    public static readonly BitSet FOLLOW_table_in_sentenceEnd1310 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_indent_in_multilineText1348 = new BitSet(new ulong[]{0x0004000000000000UL});
    public static readonly BitSet FOLLOW_50_in_multilineText1350 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_WS_in_multilineText1352 = new BitSet(new ulong[]{0x0000000400000000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_multilineText1355 = new BitSet(new ulong[]{0x00040005C0000000UL});
    public static readonly BitSet FOLLOW_multilineTextLine_in_multilineText1365 = new BitSet(new ulong[]{0x00040005C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_multilineText1376 = new BitSet(new ulong[]{0x0004000000000000UL});
    public static readonly BitSet FOLLOW_50_in_multilineText1379 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_WS_in_multilineText1381 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_multilineText1384 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_indent1422 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_multilineTextLine1451 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_text_in_multilineTextLine1454 = new BitSet(new ulong[]{0x0000000400000000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_multilineTextLine1457 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_tableRow_in_table1498 = new BitSet(new ulong[]{0x0008000040000000UL});
    public static readonly BitSet FOLLOW_tableRow_in_table1500 = new BitSet(new ulong[]{0x0008000040000002UL});
    public static readonly BitSet FOLLOW_WS_in_tableRow1547 = new BitSet(new ulong[]{0x0008000000000000UL});
    public static readonly BitSet FOLLOW_51_in_tableRow1550 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_tableCell_in_tableRow1552 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_WS_in_tableRow1555 = new BitSet(new ulong[]{0x0000000440000000UL});
    public static readonly BitSet FOLLOW_newlineWithSpaces_in_tableRow1558 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_tableCell1594 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_text_in_tableCell1597 = new BitSet(new ulong[]{0x0008000040000000UL});
    public static readonly BitSet FOLLOW_WS_in_tableCell1599 = new BitSet(new ulong[]{0x0008000000000000UL});
    public static readonly BitSet FOLLOW_51_in_tableCell1602 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_descriptionLineText1637 = new BitSet(new ulong[]{0x00000001C0000002UL});
    public static readonly BitSet FOLLOW_textRest_in_descriptionLineText1639 = new BitSet(new ulong[]{0x00000001C0000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_text1678 = new BitSet(new ulong[]{0x00000001C0000002UL});
    public static readonly BitSet FOLLOW_textRest_in_text1680 = new BitSet(new ulong[]{0x00000001C0000002UL});
    public static readonly BitSet FOLLOW_WS_in_textRest1718 = new BitSet(new ulong[]{0x00000001C0000000UL});
    public static readonly BitSet FOLLOW_textRest_in_textRest1720 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_textRest1730 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_title1749 = new BitSet(new ulong[]{0x00000005C0000002UL});
    public static readonly BitSet FOLLOW_titleRest_in_title1751 = new BitSet(new ulong[]{0x00000005C0000002UL});
    public static readonly BitSet FOLLOW_WS_in_titleRest1790 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_titleRest_in_titleRest1792 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_titleRest1802 = new BitSet(new ulong[]{0x00000005C0000000UL});
    public static readonly BitSet FOLLOW_titleRest_in_titleRest1804 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_wordchar_in_titleRest1814 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_titleRestLine1833 = new BitSet(new ulong[]{0x0000000540000000UL});
    public static readonly BitSet FOLLOW_titleRestLine_in_titleRestLine1835 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_titleRestLine1845 = new BitSet(new ulong[]{0x0000000540000000UL});
    public static readonly BitSet FOLLOW_titleRestLine_in_titleRestLine1847 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WORDCHAR_in_titleRestLine1857 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_set_in_wordchar0 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_newlineWithSpaces1905 = new BitSet(new ulong[]{0x0000000400000000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_newlineWithSpaces1908 = new BitSet(new ulong[]{0x0000000440000002UL});
    public static readonly BitSet FOLLOW_WS_in_newlineWithSpaces1911 = new BitSet(new ulong[]{0x0000000400000000UL});
    public static readonly BitSet FOLLOW_NEWLINE_in_newlineWithSpaces1914 = new BitSet(new ulong[]{0x0000000440000002UL});
    public static readonly BitSet FOLLOW_WS_in_synpred26_SpecFlowLang879 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_synpred44_SpecFlowLang1381 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_WS_in_synpred51_SpecFlowLang1555 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_titleRest_in_synpred57_SpecFlowLang1751 = new BitSet(new ulong[]{0x0000000000000002UL});

}
}