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

## Contribuire
---
Le richieste di pull sono benvenute. Per modifiche importanti, apri prima un problema per discutere cosa vorresti cambiare.
