#language:de
Funktionalität: Argument transformation mit englischer Formatierung
	Als Programmierer 
	Will ich die "binding-culture" spezifizieren können, welche für die Konvertierung von Step-Argumenten verwendet werden soll
	Damit ein nicht-englisches Feature trotzdem Step-Argumente mit englischer Formattierung (z.B. Nummern) verwenden kann
	
Szenario: Steps mit englisch formatierten Argumenten
	Wenn in App.config die bindingCulture auf 'en-US' konfiguriert ist 
	Dann ist 0.2 kleiner als 1
	Und die CurrentCulture während der Ausführung des Steps ist 'en-US'