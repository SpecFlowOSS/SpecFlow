# language: nl
Functionaliteit: Addition
  Um dumme Fehler zu vermeiden
  möchte ich als Matheidiot
  die Summe zweier Zahlen gesagt bekommen
 
  Abstract Scenario: Zwei Zahlen hinzufügen
    Gegeven sei ich habe <Eingabe_1> in den Taschenrechner eingegeben
    En ich habe <Eingabe_2> in den Taschenrechner eingegeben
    Als ich <Knopf> drücke
    Dan sollte das Ergebniss auf dem Bildschirm <Ausgabe> sein
 
  Voorbeelden:
    | Eingabe_1 | Eingabe_2 | Knopf | Ausgabe |
    | 20 | 30 | add | 50 |
    | 2 | 5 | add | 7 |
    | 0 | 40 | add | 40 |
    