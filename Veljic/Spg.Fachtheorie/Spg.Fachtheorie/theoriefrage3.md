# Theoriefrage 3

## Frage:

Wie funktioniert das Transaktionsmanagement auf Ihrem Technologie-Stack?
Erl�utern Sie kurz die Verantwortlichkeiten und notwendigen Coding-Tasks f�r Sie als EntwicklerIn!

RAPHAEL FASOL

## Antwort

Um eine Transaktion ist es dann, wenn man etwas in der Datenbank mach also zum Beispiel etwas added und danach eine SaveChanges macht. Wenn eine Operation failed dann wird alles zum letzten SaveChanges gerollbacked

Alle Operation müssen innerhalb einer Transaktion funktionieren. 