pushd "%~dp0"

..\..\lib\msxsl\msxsl.exe ..\..\Languages.xml LexerTransformation.xslt language=en -o SpecFlowLangLexer_en.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer_en.g
del SpecFlowLangLexer_en.g

..\..\lib\msxsl\msxsl.exe ..\..\Languages.xml LexerTransformation.xslt language=de -o SpecFlowLangLexer_de.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer_de.g
del SpecFlowLangLexer_de.g

..\..\lib\msxsl\msxsl.exe ..\..\Languages.xml LexerTransformation.xslt language=fr -o SpecFlowLangLexer_fr.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer_fr.g
del SpecFlowLangLexer_fr.g

..\..\lib\msxsl\msxsl.exe ..\..\Languages.xml LexerTransformation.xslt language=hu -o SpecFlowLangLexer_hu.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer_hu.g
del SpecFlowLangLexer_hu.g

..\..\lib\msxsl\msxsl.exe ..\..\Languages.xml LexerTransformation.xslt language=nl -o SpecFlowLangLexer_nl.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer_nl.g
del SpecFlowLangLexer_nl.g

..\..\lib\msxsl\msxsl.exe ..\..\Languages.xml LexerTransformation.xslt language=sv -o SpecFlowLangLexer_sv.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer_sv.g
del SpecFlowLangLexer_sv.g


java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangParser.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangWalker.g

del *.tokens

popd
