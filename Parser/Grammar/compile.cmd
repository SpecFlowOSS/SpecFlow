pushd "%~dp0"

java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangLexer.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangParser.g
java -cp ..\..\lib\antlr\antlr-3.1.2.jar org.antlr.Tool SpecFlowLangWalker.g

popd
