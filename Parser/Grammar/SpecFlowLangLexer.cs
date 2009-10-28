// $ANTLR 3.1.2 SpecFlowLang.g 2009-10-28 16:45:51

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


public partial class SpecFlowLangLexer : Lexer {
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

    public SpecFlowLangLexer() 
    {
		InitializeCyclicDFAs();
    }
    public SpecFlowLangLexer(ICharStream input)
		: this(input, null) {
    }
    public SpecFlowLangLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state) {
		InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
    	get { return "SpecFlowLang.g";} 
    }

    // $ANTLR start "T__40"
    public void mT__40() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__40;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:9:7: ( 'Feature:' )
            // SpecFlowLang.g:9:9: 'Feature:'
            {
            	Match("Feature:"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__40"

    // $ANTLR start "T__41"
    public void mT__41() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__41;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:10:7: ( 'Scenario:' )
            // SpecFlowLang.g:10:9: 'Scenario:'
            {
            	Match("Scenario:"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__41"

    // $ANTLR start "T__42"
    public void mT__42() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__42;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:11:7: ( 'Scenario Outline:' )
            // SpecFlowLang.g:11:9: 'Scenario Outline:'
            {
            	Match("Scenario Outline:"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__42"

    // $ANTLR start "T__43"
    public void mT__43() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__43;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:12:7: ( 'Examples:' )
            // SpecFlowLang.g:12:9: 'Examples:'
            {
            	Match("Examples:"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__43"

    // $ANTLR start "T__44"
    public void mT__44() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__44;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:13:7: ( 'Scenarios:' )
            // SpecFlowLang.g:13:9: 'Scenarios:'
            {
            	Match("Scenarios:"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__44"

    // $ANTLR start "T__45"
    public void mT__45() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__45;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:14:7: ( 'And' )
            // SpecFlowLang.g:14:9: 'And'
            {
            	Match("And"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__45"

    // $ANTLR start "T__46"
    public void mT__46() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__46;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:15:7: ( 'But' )
            // SpecFlowLang.g:15:9: 'But'
            {
            	Match("But"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__46"

    // $ANTLR start "T__47"
    public void mT__47() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__47;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:16:7: ( 'Given' )
            // SpecFlowLang.g:16:9: 'Given'
            {
            	Match("Given"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__47"

    // $ANTLR start "T__48"
    public void mT__48() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__48;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:17:7: ( 'When' )
            // SpecFlowLang.g:17:9: 'When'
            {
            	Match("When"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__48"

    // $ANTLR start "T__49"
    public void mT__49() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__49;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:18:7: ( 'Then' )
            // SpecFlowLang.g:18:9: 'Then'
            {
            	Match("Then"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__49"

    // $ANTLR start "T__50"
    public void mT__50() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__50;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:19:7: ( '\"\"\"' )
            // SpecFlowLang.g:19:9: '\"\"\"'
            {
            	Match("\"\"\""); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__50"

    // $ANTLR start "T__51"
    public void mT__51() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__51;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:20:7: ( '|' )
            // SpecFlowLang.g:20:9: '|'
            {
            	Match('|'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__51"

    // $ANTLR start "WSCHAR"
    public void mWSCHAR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLang.g:266:21: ( ( ' ' | '\\t' ) )
            // SpecFlowLang.g:266:23: ( ' ' | '\\t' )
            {
            	if ( input.LA(1) == '\t' || input.LA(1) == ' ' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "WSCHAR"

    // $ANTLR start "NONWCHR"
    public void mNONWCHR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLang.g:267:21: ( ( ' ' | '\\t' | '\\r' | '\\n' | '#' | '@' ) )
            // SpecFlowLang.g:267:23: ( ' ' | '\\t' | '\\r' | '\\n' | '#' | '@' )
            {
            	if ( (input.LA(1) >= '\t' && input.LA(1) <= '\n') || input.LA(1) == '\r' || input.LA(1) == ' ' || input.LA(1) == '#' || input.LA(1) == '@' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "NONWCHR"

    // $ANTLR start "NEWLINECHR"
    public void mNEWLINECHR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLang.g:268:21: ( ( '\\r' | '\\n' ) )
            // SpecFlowLang.g:268:23: ( '\\r' | '\\n' )
            {
            	if ( input.LA(1) == '\n' || input.LA(1) == '\r' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "NEWLINECHR"

    // $ANTLR start "NONNLCHR"
    public void mNONNLCHR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLang.g:269:21: ( ( '\\u0000' .. '\\t' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\uFFFF' ) )
            int alt1 = 3;
            int LA1_0 = input.LA(1);

            if ( ((LA1_0 >= '\u0000' && LA1_0 <= '\t')) )
            {
                alt1 = 1;
            }
            else if ( ((LA1_0 >= '\u000B' && LA1_0 <= '\f')) )
            {
                alt1 = 2;
            }
            else if ( ((LA1_0 >= '\u000E' && LA1_0 <= '\uFFFF')) )
            {
                alt1 = 3;
            }
            else 
            {
                NoViableAltException nvae_d1s0 =
                    new NoViableAltException("", 1, 0, input);

                throw nvae_d1s0;
            }
            switch (alt1) 
            {
                case 1 :
                    // SpecFlowLang.g:269:23: ( '\\u0000' .. '\\t' )
                    {
                    	// SpecFlowLang.g:269:23: ( '\\u0000' .. '\\t' )
                    	// SpecFlowLang.g:269:24: '\\u0000' .. '\\t'
                    	{
                    		MatchRange('\u0000','\t'); 

                    	}


                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:269:40: ( '\\u000B' .. '\\f' )
                    {
                    	// SpecFlowLang.g:269:40: ( '\\u000B' .. '\\f' )
                    	// SpecFlowLang.g:269:41: '\\u000B' .. '\\f'
                    	{
                    		MatchRange('\u000B','\f'); 

                    	}


                    }
                    break;
                case 3 :
                    // SpecFlowLang.g:269:57: ( '\\u000E' .. '\\uFFFF' )
                    {
                    	// SpecFlowLang.g:269:57: ( '\\u000E' .. '\\uFFFF' )
                    	// SpecFlowLang.g:269:58: '\\u000E' .. '\\uFFFF'
                    	{
                    		MatchRange('\u000E','\uFFFF'); 

                    	}


                    }
                    break;

            }
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NONNLCHR"

    // $ANTLR start "T_BACKGROUND"
    public void mT_BACKGROUND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_BACKGROUND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:272:14: ( 'Background:' )
            // SpecFlowLang.g:272:16: 'Background:'
            {
            	Match("Background:"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_BACKGROUND"

    // $ANTLR start "AT"
    public void mAT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:273:13: ( '@' )
            // SpecFlowLang.g:273:15: '@'
            {
            	Match('@'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AT"

    // $ANTLR start "COMMENT"
    public void mCOMMENT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COMMENT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:275:13: ( ( WSCHAR )* '#' ( NONNLCHR )* )
            // SpecFlowLang.g:275:15: ( WSCHAR )* '#' ( NONNLCHR )*
            {
            	// SpecFlowLang.g:275:15: ( WSCHAR )*
            	do 
            	{
            	    int alt2 = 2;
            	    int LA2_0 = input.LA(1);

            	    if ( (LA2_0 == '\t' || LA2_0 == ' ') )
            	    {
            	        alt2 = 1;
            	    }


            	    switch (alt2) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:275:15: WSCHAR
            			    {
            			    	mWSCHAR(); 

            			    }
            			    break;

            			default:
            			    goto loop2;
            	    }
            	} while (true);

            	loop2:
            		;	// Stops C# compiler whining that label 'loop2' has no statements

            	Match('#'); 
            	// SpecFlowLang.g:275:27: ( NONNLCHR )*
            	do 
            	{
            	    int alt3 = 2;
            	    int LA3_0 = input.LA(1);

            	    if ( ((LA3_0 >= '\u0000' && LA3_0 <= '\t') || (LA3_0 >= '\u000B' && LA3_0 <= '\f') || (LA3_0 >= '\u000E' && LA3_0 <= '\uFFFF')) )
            	    {
            	        alt3 = 1;
            	    }


            	    switch (alt3) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:275:27: NONNLCHR
            			    {
            			    	mNONNLCHR(); 

            			    }
            			    break;

            			default:
            			    goto loop3;
            	    }
            	} while (true);

            	loop3:
            		;	// Stops C# compiler whining that label 'loop3' has no statements

            	 _channel = Token.HIDDEN_CHANNEL; 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COMMENT"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:276:13: ( ( WSCHAR )+ )
            // SpecFlowLang.g:276:15: ( WSCHAR )+
            {
            	// SpecFlowLang.g:276:15: ( WSCHAR )+
            	int cnt4 = 0;
            	do 
            	{
            	    int alt4 = 2;
            	    int LA4_0 = input.LA(1);

            	    if ( (LA4_0 == '\t' || LA4_0 == ' ') )
            	    {
            	        alt4 = 1;
            	    }


            	    switch (alt4) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:276:15: WSCHAR
            			    {
            			    	mWSCHAR(); 

            			    }
            			    break;

            			default:
            			    if ( cnt4 >= 1 ) goto loop4;
            		            EarlyExitException eee4 =
            		                new EarlyExitException(4, input);
            		            throw eee4;
            	    }
            	    cnt4++;
            	} while (true);

            	loop4:
            		;	// Stops C# compiler whinging that label 'loop4' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WS"

    // $ANTLR start "NEWLINE"
    public void mNEWLINE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NEWLINE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:277:13: ( '\\r\\n' | '\\n' )
            int alt5 = 2;
            int LA5_0 = input.LA(1);

            if ( (LA5_0 == '\r') )
            {
                alt5 = 1;
            }
            else if ( (LA5_0 == '\n') )
            {
                alt5 = 2;
            }
            else 
            {
                NoViableAltException nvae_d5s0 =
                    new NoViableAltException("", 5, 0, input);

                throw nvae_d5s0;
            }
            switch (alt5) 
            {
                case 1 :
                    // SpecFlowLang.g:277:15: '\\r\\n'
                    {
                    	Match("\r\n"); 


                    }
                    break;
                case 2 :
                    // SpecFlowLang.g:277:24: '\\n'
                    {
                    	Match('\n'); 

                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NEWLINE"

    // $ANTLR start "WORDCHAR"
    public void mWORDCHAR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WORDCHAR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLang.g:279:13: ( ( ( '\\u0000' .. '\\b' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\u001F' ) | ( '!' .. '\"' ) | ( '$' .. '?' ) | ( 'A' .. '\\uFFFF' ) )+ )
            // SpecFlowLang.g:279:15: ( ( '\\u0000' .. '\\b' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\u001F' ) | ( '!' .. '\"' ) | ( '$' .. '?' ) | ( 'A' .. '\\uFFFF' ) )+
            {
            	// SpecFlowLang.g:279:15: ( ( '\\u0000' .. '\\b' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\u001F' ) | ( '!' .. '\"' ) | ( '$' .. '?' ) | ( 'A' .. '\\uFFFF' ) )+
            	int cnt6 = 0;
            	do 
            	{
            	    int alt6 = 7;
            	    int LA6_0 = input.LA(1);

            	    if ( ((LA6_0 >= '\u0000' && LA6_0 <= '\b')) )
            	    {
            	        alt6 = 1;
            	    }
            	    else if ( ((LA6_0 >= '\u000B' && LA6_0 <= '\f')) )
            	    {
            	        alt6 = 2;
            	    }
            	    else if ( ((LA6_0 >= '\u000E' && LA6_0 <= '\u001F')) )
            	    {
            	        alt6 = 3;
            	    }
            	    else if ( ((LA6_0 >= '!' && LA6_0 <= '\"')) )
            	    {
            	        alt6 = 4;
            	    }
            	    else if ( ((LA6_0 >= '$' && LA6_0 <= '?')) )
            	    {
            	        alt6 = 5;
            	    }
            	    else if ( ((LA6_0 >= 'A' && LA6_0 <= '\uFFFF')) )
            	    {
            	        alt6 = 6;
            	    }


            	    switch (alt6) 
            		{
            			case 1 :
            			    // SpecFlowLang.g:279:16: ( '\\u0000' .. '\\b' )
            			    {
            			    	// SpecFlowLang.g:279:16: ( '\\u0000' .. '\\b' )
            			    	// SpecFlowLang.g:279:17: '\\u0000' .. '\\b'
            			    	{
            			    		MatchRange('\u0000','\b'); 

            			    	}


            			    }
            			    break;
            			case 2 :
            			    // SpecFlowLang.g:280:7: ( '\\u000B' .. '\\f' )
            			    {
            			    	// SpecFlowLang.g:280:7: ( '\\u000B' .. '\\f' )
            			    	// SpecFlowLang.g:280:8: '\\u000B' .. '\\f'
            			    	{
            			    		MatchRange('\u000B','\f'); 

            			    	}


            			    }
            			    break;
            			case 3 :
            			    // SpecFlowLang.g:281:7: ( '\\u000E' .. '\\u001F' )
            			    {
            			    	// SpecFlowLang.g:281:7: ( '\\u000E' .. '\\u001F' )
            			    	// SpecFlowLang.g:281:8: '\\u000E' .. '\\u001F'
            			    	{
            			    		MatchRange('\u000E','\u001F'); 

            			    	}


            			    }
            			    break;
            			case 4 :
            			    // SpecFlowLang.g:282:7: ( '!' .. '\"' )
            			    {
            			    	// SpecFlowLang.g:282:7: ( '!' .. '\"' )
            			    	// SpecFlowLang.g:282:8: '!' .. '\"'
            			    	{
            			    		MatchRange('!','\"'); 

            			    	}


            			    }
            			    break;
            			case 5 :
            			    // SpecFlowLang.g:283:7: ( '$' .. '?' )
            			    {
            			    	// SpecFlowLang.g:283:7: ( '$' .. '?' )
            			    	// SpecFlowLang.g:283:8: '$' .. '?'
            			    	{
            			    		MatchRange('$','?'); 

            			    	}


            			    }
            			    break;
            			case 6 :
            			    // SpecFlowLang.g:284:7: ( 'A' .. '\\uFFFF' )
            			    {
            			    	// SpecFlowLang.g:284:7: ( 'A' .. '\\uFFFF' )
            			    	// SpecFlowLang.g:284:8: 'A' .. '\\uFFFF'
            			    	{
            			    		MatchRange('A','\uFFFF'); 

            			    	}


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


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WORDCHAR"

    override public void mTokens() // throws RecognitionException 
    {
        // SpecFlowLang.g:1:8: ( T__40 | T__41 | T__42 | T__43 | T__44 | T__45 | T__46 | T__47 | T__48 | T__49 | T__50 | T__51 | T_BACKGROUND | AT | COMMENT | WS | NEWLINE | WORDCHAR )
        int alt7 = 18;
        alt7 = dfa7.Predict(input);
        switch (alt7) 
        {
            case 1 :
                // SpecFlowLang.g:1:10: T__40
                {
                	mT__40(); 

                }
                break;
            case 2 :
                // SpecFlowLang.g:1:16: T__41
                {
                	mT__41(); 

                }
                break;
            case 3 :
                // SpecFlowLang.g:1:22: T__42
                {
                	mT__42(); 

                }
                break;
            case 4 :
                // SpecFlowLang.g:1:28: T__43
                {
                	mT__43(); 

                }
                break;
            case 5 :
                // SpecFlowLang.g:1:34: T__44
                {
                	mT__44(); 

                }
                break;
            case 6 :
                // SpecFlowLang.g:1:40: T__45
                {
                	mT__45(); 

                }
                break;
            case 7 :
                // SpecFlowLang.g:1:46: T__46
                {
                	mT__46(); 

                }
                break;
            case 8 :
                // SpecFlowLang.g:1:52: T__47
                {
                	mT__47(); 

                }
                break;
            case 9 :
                // SpecFlowLang.g:1:58: T__48
                {
                	mT__48(); 

                }
                break;
            case 10 :
                // SpecFlowLang.g:1:64: T__49
                {
                	mT__49(); 

                }
                break;
            case 11 :
                // SpecFlowLang.g:1:70: T__50
                {
                	mT__50(); 

                }
                break;
            case 12 :
                // SpecFlowLang.g:1:76: T__51
                {
                	mT__51(); 

                }
                break;
            case 13 :
                // SpecFlowLang.g:1:82: T_BACKGROUND
                {
                	mT_BACKGROUND(); 

                }
                break;
            case 14 :
                // SpecFlowLang.g:1:95: AT
                {
                	mAT(); 

                }
                break;
            case 15 :
                // SpecFlowLang.g:1:98: COMMENT
                {
                	mCOMMENT(); 

                }
                break;
            case 16 :
                // SpecFlowLang.g:1:106: WS
                {
                	mWS(); 

                }
                break;
            case 17 :
                // SpecFlowLang.g:1:109: NEWLINE
                {
                	mNEWLINE(); 

                }
                break;
            case 18 :
                // SpecFlowLang.g:1:117: WORDCHAR
                {
                	mWORDCHAR(); 

                }
                break;

        }

    }


    protected DFA7 dfa7;
	private void InitializeCyclicDFAs()
	{
	    this.dfa7 = new DFA7(this);
	    this.dfa7.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA7_SpecialStateTransition);
	}

    const string DFA7_eotS =
        "\x01\uffff\x09\x0f\x01\x1a\x01\uffff\x01\x1b\x03\uffff\x0a\x0f"+
        "\x02\uffff\x03\x0f\x01\x29\x01\x2a\x04\x0f\x01\x2f\x03\x0f\x02\uffff"+
        "\x02\x0f\x01\x35\x01\x36\x01\uffff\x04\x0f\x01\x3b\x02\uffff\x04"+
        "\x0f\x01\uffff\x04\x0f\x01\x44\x03\x0f\x01\uffff\x01\x4a\x01\uffff"+
        "\x01\x0f\x01\x4c\x01\x0f\x01\uffff\x01\x4e\x01\uffff\x01\x0f\x01"+
        "\uffff\x01\x50\x01\uffff";
    const string DFA7_eofS =
        "\x51\uffff";
    const string DFA7_minS =
        "\x01\x00\x01\x65\x01\x63\x01\x78\x01\x6e\x01\x61\x01\x69\x02\x68"+
        "\x01\x22\x01\x00\x01\uffff\x01\x09\x03\uffff\x01\x61\x01\x65\x01"+
        "\x61\x01\x64\x01\x74\x01\x63\x01\x76\x02\x65\x01\x22\x02\uffff\x01"+
        "\x74\x01\x6e\x01\x6d\x02\x00\x01\x6b\x01\x65\x02\x6e\x01\x00\x01"+
        "\x75\x01\x61\x01\x70\x02\uffff\x01\x67\x01\x6e\x02\x00\x01\uffff"+
        "\x02\x72\x01\x6c\x01\x72\x01\x00\x02\uffff\x01\x65\x01\x69\x01\x65"+
        "\x01\x6f\x01\uffff\x01\x3a\x01\x6f\x01\x73\x01\x75\x01\x00\x01\x20"+
        "\x01\x3a\x01\x6e\x01\uffff\x01\x00\x01\uffff\x01\x3a\x01\x00\x01"+
        "\x64\x01\uffff\x01\x00\x01\uffff\x01\x3a\x01\uffff\x01\x00\x01\uffff";
    const string DFA7_maxS =
        "\x01\uffff\x01\x65\x01\x63\x01\x78\x01\x6e\x01\x75\x01\x69\x02"+
        "\x68\x01\x22\x01\uffff\x01\uffff\x01\x23\x03\uffff\x01\x61\x01\x65"+
        "\x01\x61\x01\x64\x01\x74\x01\x63\x01\x76\x02\x65\x01\x22\x02\uffff"+
        "\x01\x74\x01\x6e\x01\x6d\x02\uffff\x01\x6b\x01\x65\x02\x6e\x01\uffff"+
        "\x01\x75\x01\x61\x01\x70\x02\uffff\x01\x67\x01\x6e\x02\uffff\x01"+
        "\uffff\x02\x72\x01\x6c\x01\x72\x01\uffff\x02\uffff\x01\x65\x01\x69"+
        "\x01\x65\x01\x6f\x01\uffff\x01\x3a\x01\x6f\x01\x73\x01\x75\x01\uffff"+
        "\x01\x73\x01\x3a\x01\x6e\x01\uffff\x01\uffff\x01\uffff\x01\x3a\x01"+
        "\uffff\x01\x64\x01\uffff\x01\uffff\x01\uffff\x01\x3a\x01\uffff\x01"+
        "\uffff\x01\uffff";
    const string DFA7_acceptS =
        "\x0b\uffff\x01\x0e\x01\uffff\x01\x0f\x01\x11\x01\x12\x0a\uffff"+
        "\x01\x0c\x01\x10\x0d\uffff\x01\x06\x01\x07\x04\uffff\x01\x0b\x05"+
        "\uffff\x01\x09\x01\x0a\x04\uffff\x01\x08\x08\uffff\x01\x01\x01\uffff"+
        "\x01\x03\x03\uffff\x01\x02\x01\uffff\x01\x04\x01\uffff\x01\x05\x01"+
        "\uffff\x01\x0d";
    const string DFA7_specialS =
        "\x01\x06\x09\uffff\x01\x0c\x14\uffff\x01\x03\x01\x04\x04\uffff"+
        "\x01\x09\x07\uffff\x01\x01\x01\x08\x05\uffff\x01\x00\x0b\uffff\x01"+
        "\x05\x04\uffff\x01\x0b\x02\uffff\x01\x0a\x02\uffff\x01\x07\x03\uffff"+
        "\x01\x02\x01\uffff}>";
    static readonly string[] DFA7_transitionS = {
            "\x09\x0f\x01\x0c\x01\x0e\x02\x0f\x01\x0e\x12\x0f\x01\x0c\x01"+
            "\x0f\x01\x09\x01\x0d\x1c\x0f\x01\x0b\x01\x04\x01\x05\x02\x0f"+
            "\x01\x03\x01\x01\x01\x06\x0b\x0f\x01\x02\x01\x08\x02\x0f\x01"+
            "\x07\x24\x0f\x01\x0a\uff83\x0f",
            "\x01\x10",
            "\x01\x11",
            "\x01\x12",
            "\x01\x13",
            "\x01\x15\x13\uffff\x01\x14",
            "\x01\x16",
            "\x01\x17",
            "\x01\x18",
            "\x01\x19",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "",
            "\x01\x0c\x16\uffff\x01\x0c\x02\uffff\x01\x0d",
            "",
            "",
            "",
            "\x01\x1c",
            "\x01\x1d",
            "\x01\x1e",
            "\x01\x1f",
            "\x01\x20",
            "\x01\x21",
            "\x01\x22",
            "\x01\x23",
            "\x01\x24",
            "\x01\x25",
            "",
            "",
            "\x01\x26",
            "\x01\x27",
            "\x01\x28",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "\x01\x2b",
            "\x01\x2c",
            "\x01\x2d",
            "\x01\x2e",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "\x01\x30",
            "\x01\x31",
            "\x01\x32",
            "",
            "",
            "\x01\x33",
            "\x01\x34",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "",
            "\x01\x37",
            "\x01\x38",
            "\x01\x39",
            "\x01\x3a",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "",
            "",
            "\x01\x3c",
            "\x01\x3d",
            "\x01\x3e",
            "\x01\x3f",
            "",
            "\x01\x40",
            "\x01\x41",
            "\x01\x42",
            "\x01\x43",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "\x01\x46\x19\uffff\x01\x45\x38\uffff\x01\x47",
            "\x01\x48",
            "\x01\x49",
            "",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "",
            "\x01\x4b",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "\x01\x4d",
            "",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            "",
            "\x01\x4f",
            "",
            "\x09\x0f\x02\uffff\x02\x0f\x01\uffff\x12\x0f\x01\uffff\x02"+
            "\x0f\x01\uffff\x1c\x0f\x01\uffff\uffbf\x0f",
            ""
    };

    static readonly short[] DFA7_eot = DFA.UnpackEncodedString(DFA7_eotS);
    static readonly short[] DFA7_eof = DFA.UnpackEncodedString(DFA7_eofS);
    static readonly char[] DFA7_min = DFA.UnpackEncodedStringToUnsignedChars(DFA7_minS);
    static readonly char[] DFA7_max = DFA.UnpackEncodedStringToUnsignedChars(DFA7_maxS);
    static readonly short[] DFA7_accept = DFA.UnpackEncodedString(DFA7_acceptS);
    static readonly short[] DFA7_special = DFA.UnpackEncodedString(DFA7_specialS);
    static readonly short[][] DFA7_transition = DFA.UnpackEncodedStringArray(DFA7_transitionS);

    protected class DFA7 : DFA
    {
        public DFA7(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 7;
            this.eot = DFA7_eot;
            this.eof = DFA7_eof;
            this.min = DFA7_min;
            this.max = DFA7_max;
            this.accept = DFA7_accept;
            this.special = DFA7_special;
            this.transition = DFA7_transition;

        }

        override public string Description
        {
            get { return "1:1: Tokens : ( T__40 | T__41 | T__42 | T__43 | T__44 | T__45 | T__46 | T__47 | T__48 | T__49 | T__50 | T__51 | T_BACKGROUND | AT | COMMENT | WS | NEWLINE | WORDCHAR );"; }
        }

    }


    protected internal int DFA7_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            IIntStream input = _input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA7_52 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_52 >= '\u0000' && LA7_52 <= '\b') || (LA7_52 >= '\u000B' && LA7_52 <= '\f') || (LA7_52 >= '\u000E' && LA7_52 <= '\u001F') || (LA7_52 >= '!' && LA7_52 <= '\"') || (LA7_52 >= '$' && LA7_52 <= '?') || (LA7_52 >= 'A' && LA7_52 <= '\uFFFF')) ) { s = 15; }

                   	else s = 59;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA7_45 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_45 >= '\u0000' && LA7_45 <= '\b') || (LA7_45 >= '\u000B' && LA7_45 <= '\f') || (LA7_45 >= '\u000E' && LA7_45 <= '\u001F') || (LA7_45 >= '!' && LA7_45 <= '\"') || (LA7_45 >= '$' && LA7_45 <= '?') || (LA7_45 >= 'A' && LA7_45 <= '\uFFFF')) ) { s = 15; }

                   	else s = 53;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA7_79 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_79 >= '\u0000' && LA7_79 <= '\b') || (LA7_79 >= '\u000B' && LA7_79 <= '\f') || (LA7_79 >= '\u000E' && LA7_79 <= '\u001F') || (LA7_79 >= '!' && LA7_79 <= '\"') || (LA7_79 >= '$' && LA7_79 <= '?') || (LA7_79 >= 'A' && LA7_79 <= '\uFFFF')) ) { s = 15; }

                   	else s = 80;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA7_31 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_31 >= '\u0000' && LA7_31 <= '\b') || (LA7_31 >= '\u000B' && LA7_31 <= '\f') || (LA7_31 >= '\u000E' && LA7_31 <= '\u001F') || (LA7_31 >= '!' && LA7_31 <= '\"') || (LA7_31 >= '$' && LA7_31 <= '?') || (LA7_31 >= 'A' && LA7_31 <= '\uFFFF')) ) { s = 15; }

                   	else s = 41;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA7_32 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_32 >= '\u0000' && LA7_32 <= '\b') || (LA7_32 >= '\u000B' && LA7_32 <= '\f') || (LA7_32 >= '\u000E' && LA7_32 <= '\u001F') || (LA7_32 >= '!' && LA7_32 <= '\"') || (LA7_32 >= '$' && LA7_32 <= '?') || (LA7_32 >= 'A' && LA7_32 <= '\uFFFF')) ) { s = 15; }

                   	else s = 42;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA7_64 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_64 >= '\u0000' && LA7_64 <= '\b') || (LA7_64 >= '\u000B' && LA7_64 <= '\f') || (LA7_64 >= '\u000E' && LA7_64 <= '\u001F') || (LA7_64 >= '!' && LA7_64 <= '\"') || (LA7_64 >= '$' && LA7_64 <= '?') || (LA7_64 >= 'A' && LA7_64 <= '\uFFFF')) ) { s = 15; }

                   	else s = 68;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA7_0 = input.LA(1);

                   	s = -1;
                   	if ( (LA7_0 == 'F') ) { s = 1; }

                   	else if ( (LA7_0 == 'S') ) { s = 2; }

                   	else if ( (LA7_0 == 'E') ) { s = 3; }

                   	else if ( (LA7_0 == 'A') ) { s = 4; }

                   	else if ( (LA7_0 == 'B') ) { s = 5; }

                   	else if ( (LA7_0 == 'G') ) { s = 6; }

                   	else if ( (LA7_0 == 'W') ) { s = 7; }

                   	else if ( (LA7_0 == 'T') ) { s = 8; }

                   	else if ( (LA7_0 == '\"') ) { s = 9; }

                   	else if ( (LA7_0 == '|') ) { s = 10; }

                   	else if ( (LA7_0 == '@') ) { s = 11; }

                   	else if ( (LA7_0 == '\t' || LA7_0 == ' ') ) { s = 12; }

                   	else if ( (LA7_0 == '#') ) { s = 13; }

                   	else if ( (LA7_0 == '\n' || LA7_0 == '\r') ) { s = 14; }

                   	else if ( ((LA7_0 >= '\u0000' && LA7_0 <= '\b') || (LA7_0 >= '\u000B' && LA7_0 <= '\f') || (LA7_0 >= '\u000E' && LA7_0 <= '\u001F') || LA7_0 == '!' || (LA7_0 >= '$' && LA7_0 <= '?') || (LA7_0 >= 'C' && LA7_0 <= 'D') || (LA7_0 >= 'H' && LA7_0 <= 'R') || (LA7_0 >= 'U' && LA7_0 <= 'V') || (LA7_0 >= 'X' && LA7_0 <= '{') || (LA7_0 >= '}' && LA7_0 <= '\uFFFF')) ) { s = 15; }

                   	if ( s >= 0 ) return s;
                   	break;
               	case 7 : 
                   	int LA7_75 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_75 >= '\u0000' && LA7_75 <= '\b') || (LA7_75 >= '\u000B' && LA7_75 <= '\f') || (LA7_75 >= '\u000E' && LA7_75 <= '\u001F') || (LA7_75 >= '!' && LA7_75 <= '\"') || (LA7_75 >= '$' && LA7_75 <= '?') || (LA7_75 >= 'A' && LA7_75 <= '\uFFFF')) ) { s = 15; }

                   	else s = 78;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 8 : 
                   	int LA7_46 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_46 >= '\u0000' && LA7_46 <= '\b') || (LA7_46 >= '\u000B' && LA7_46 <= '\f') || (LA7_46 >= '\u000E' && LA7_46 <= '\u001F') || (LA7_46 >= '!' && LA7_46 <= '\"') || (LA7_46 >= '$' && LA7_46 <= '?') || (LA7_46 >= 'A' && LA7_46 <= '\uFFFF')) ) { s = 15; }

                   	else s = 54;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 9 : 
                   	int LA7_37 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_37 >= '\u0000' && LA7_37 <= '\b') || (LA7_37 >= '\u000B' && LA7_37 <= '\f') || (LA7_37 >= '\u000E' && LA7_37 <= '\u001F') || (LA7_37 >= '!' && LA7_37 <= '\"') || (LA7_37 >= '$' && LA7_37 <= '?') || (LA7_37 >= 'A' && LA7_37 <= '\uFFFF')) ) { s = 15; }

                   	else s = 47;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 10 : 
                   	int LA7_72 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_72 >= '\u0000' && LA7_72 <= '\b') || (LA7_72 >= '\u000B' && LA7_72 <= '\f') || (LA7_72 >= '\u000E' && LA7_72 <= '\u001F') || (LA7_72 >= '!' && LA7_72 <= '\"') || (LA7_72 >= '$' && LA7_72 <= '?') || (LA7_72 >= 'A' && LA7_72 <= '\uFFFF')) ) { s = 15; }

                   	else s = 76;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 11 : 
                   	int LA7_69 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_69 >= '\u0000' && LA7_69 <= '\b') || (LA7_69 >= '\u000B' && LA7_69 <= '\f') || (LA7_69 >= '\u000E' && LA7_69 <= '\u001F') || (LA7_69 >= '!' && LA7_69 <= '\"') || (LA7_69 >= '$' && LA7_69 <= '?') || (LA7_69 >= 'A' && LA7_69 <= '\uFFFF')) ) { s = 15; }

                   	else s = 74;

                   	if ( s >= 0 ) return s;
                   	break;
               	case 12 : 
                   	int LA7_10 = input.LA(1);

                   	s = -1;
                   	if ( ((LA7_10 >= '\u0000' && LA7_10 <= '\b') || (LA7_10 >= '\u000B' && LA7_10 <= '\f') || (LA7_10 >= '\u000E' && LA7_10 <= '\u001F') || (LA7_10 >= '!' && LA7_10 <= '\"') || (LA7_10 >= '$' && LA7_10 <= '?') || (LA7_10 >= 'A' && LA7_10 <= '\uFFFF')) ) { s = 15; }

                   	else s = 26;

                   	if ( s >= 0 ) return s;
                   	break;
        }
        NoViableAltException nvae7 =
            new NoViableAltException(dfa.Description, 7, _s, input);
        dfa.Error(nvae7);
        throw nvae7;
    }
 
    
}
}