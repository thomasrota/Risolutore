# Risolutore di Protocolli di Routing

Questo progetto è un risolutore per tre diversi protocolli di routing: Link State Protocol, Distance Vector e Bellman-Ford. Utilizza il browser automation tramite Selenium per interagire con una piattaforma web e ottenere i dati dei grafi da risolvere.

## Funzionalità

- **Link State Protocol**: Calcola le tabelle di routing utilizzando il protocollo **link state**.
- **Distance Vector**: Determina le tabelle di routing utilizzando l'algoritmo di routing Distance Vector.
- **Bellman-Ford**: Trova il percorso più breve da un nodo sorgente a tutti gli altri nodi, gestendo anche la rilevazione di cicli di peso negativo.

## Struttura del Progetto
Il progetto è composto da diverse classi:

- `Node`: Rappresenta un nodo nel grafo con ID, etichetta e colore.
- `Arco`: Rappresenta un arco nel grafo con nodi di partenza e arrivo e il peso.
- `GraphData`: Contiene le liste di nodi e archi che costituiscono il grafo.
- `Program`: Contiene il metodo `Main` e altre funzioni per eseguire i diversi algoritmi di routing.

## Installazione
1. **Clonare il repository**:
    ```bash
    git clone https://github.com/thomasrota/Risolutore.git <directory>
    ```

2. **Configurare le dipendenze**:
    - Assicurarsi di avere .NET Core SDK installato. Puoi scaricarlo [qui](https://dotnet.microsoft.com/download).
    - Installare i pacchetti NuGet necessari:
        ```bash
        dotnet add package Selenium.WebDriver
        dotnet add package Selenium.WebDriver.EdgeDriver
        dotnet add package Newtonsoft.Json
        dotnet add package Selenium.Support
        ```

## Utilizzo
```bash
dotnet run Risolutore.csproj
```
 1. **Selezione del Protocollo**
		All'avvio, verrà richiesto di scegliere il tipo di esercizio da risolvere o se uscire:
    ```
    Tipologia esercizio da risolvere
		1. Link State Protocol
		2. Distance Vector
		3. Bellman-Ford
		4. Esci
	```

2. **Link State Protocol**
		Per eseguire il Link State Protocol, selezionare l'opzione 1. Il programma aprirà il link corrispondente, genererà un nuovo grafo e calcolerà le tabelle di routing.

3. **Distance Vector**
		Per eseguire il Distance Vector, selezionare l'opzione 2. Verrà chiesto se si desidera generare un nuovo grafo o utilizzare un codice grafo esistente. Il programma poi calcolerà le tabelle di routing.

4. **Bellman-Ford**
		Per eseguire Bellman-Ford, selezionare l'opzione 3. Verrà chiesto se si desidera generare un nuovo grafo o utilizzare un codice grafo esistente. Il programma calcolerà i percorsi più brevi dalla sorgente a tutti gli altri nodi.

## Esempio di Utilizzo
```
Tipologia esercizio da risolvere
1 - Link State Protocol
2 - Distance Vector
3 - Bellman-Ford
0 - Esci

Scelta: 1
```

```
Link State Packet per il Router R1:
  To R2: Costo = 10
  To R3: Costo = 1
  To R7: Costo = 1

Link State Packet per il Router R2:
  To R1: Costo = 10
  To R3: Costo = 1
  To R4: Costo = 7
  To R7: Costo = 2

Link State Packet per il Router R3:
  To R1: Costo = 1
  To R2: Costo = 1
  To R7: Costo = 1

Link State Packet per il Router R4:
  To R2: Costo = 7
  To R5: Costo = 8
  To R6: Costo = 2
  To R7: Costo = 2

Link State Packet per il Router R5:
  To R4: Costo = 8
  To R7: Costo = 9

Link State Packet per il Router R6:
  To R4: Costo = 2

Link State Packet per il Router R7:
  To R1: Costo = 1
  To R2: Costo = 2
  To R3: Costo = 1
  To R4: Costo = 2
  To R5: Costo = 9

        R1      R2      R3      R4      R5      R6      R7
R1      0       2       1       3       10      5       1
R2      2       0       1       4       11      6       2
R3      1       1       0       3       10      5       1
R4      3       4       3       0       8       2       2
R5      10      11      10      8       0       10      9
R6      5       6       5       2       10      0       4
R7      1       2       1       2       9       4       0
```

## Contribuire
Le richieste di pull sono benvenute. Per modifiche importanti, apri prima un problema per discutere cosa vorresti cambiare.
