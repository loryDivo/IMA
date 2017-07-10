# IMA
applicazione di gestione immagini cross platform

IDE utilizzato Visual Studio 2015 / 2017

Requisiti per avvio:
- Xamarin;
- strumento compilazione librerie cross platform c++ / c;
Android:
  - SDK Android (Emulatori etc.);
UWP:
  - SDK UWP (Emulatori etc.).
iOS:
  - piattaforma Apple ospitante MacOS Sierra o successive;
  - SDK iOS;
  - XCode;
  - Xamarin per MacOS.

Requisiti avvio server APP:
- Python;
- framework Flask.

Istruzioni

Per poter avviare l'applicativo è necessario scaricare L'IDE Visual Studio dal sito ufficiale. 
All'avvio del tool di installazione sarà possibile scegliere i pacchetti da installare (illustrati nella sezione Requsiti).

Android / UWP

è sufficiente scaricare e installare gli strumenti SDK per le piattaforme. Successivamente basterà emulare l'applicativo selezionando il progetto opportuno.

iOS
Per poter emulare l'applicativo su iOS è necessario stabilire una connessione con il dispositivo ospitante Mac, mediante un tool offerto da Xamarin. Una volta stabilità la connessione è sufficiente avviare il progetto e verrà automaticamente avviato sul Mac in questione

Python

è necessario scaricare il framework FLASK con le relative estensioni presenti sul sito ufficiale. Tramite il terminale è sufficiente digitare le seguenti righe:
"set FLASK_APP=server_connection.py" -> "flask run"
 A questo punto il server sarà in ascolto sull'indirizzo definito all'interno del codice sorgente.
