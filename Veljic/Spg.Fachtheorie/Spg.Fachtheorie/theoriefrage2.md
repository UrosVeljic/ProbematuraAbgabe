# Theoriefrage 2

## Frage:

Welche Arten von automatischen Software-Tests kennen Sie?
Welche technischen Notwendigkeiten (Bibliotheken, Patterns) ben�tigen Sie f�r welche Testarten?
Erl�utern Sie kurz deren Funktionsweise!

RAPHAEL FASOL

## Antwort

Mocking 
Packete wie NSubsitute und Moq werden verwendet um das Verhalten eine Repositories vorzutäuschen. Sie testen ob die Repositories genau das was in der Datenbanksteht zurückliefert.

IntegrationTesting
Beim IntegrationTesting wird die Response von der API als JSON Format geholt, das kann man dann in einen string umwandeln. Des weiteren wird dann der StatusCode überprüft ob dieser die Abfrage den richtigen zurückgibt. Außerdem kann man dann mit dem Service vergleichen und auch die Datenbank testen, ob sich was geändert hat.

Dependency Injection kann verwendet werden um den Context aber auch Repos und Services zu initalsieren, damit man diese dann nicht mehr mit new neu machen muss

Generic kann auch verwendet werden um mehrere Tests auf einmal schnell zu coden.