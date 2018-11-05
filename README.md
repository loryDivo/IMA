# IMA (Image Management Application)
Cross-platform application dedicated to the management of digital images. With this application you can load an image from the gallery, select multiple parts of it by entering the bounding box, send the image to the compressed server with the coordinates. the server will decompress the image and will be able to identify the image parts bounded by the bounding box

IDE used: Visual Studio 2015 / 2017

Requirements for starting up client APP:
- Xamarin;
- C++ / c compiler istruments;
Android:
  - SDK Android (Emulator etc.);
UWP:
  - SDK UWP (Emulator etc.).
iOS:
  - Apple platform that host MacOS;
  - SDK iOS;
  - XCode;
  - Xamarin for MacOS.

Requirements for starting up server APP:
- Python;
- framework Flask.

Istruzioni

To start the application you need to download the Visual Studio IDE from the official website.
When the installation tool is started, it will be possible to choose the packages to install (shown in the Requsiti section).

Android / UWP

Simply download and install the SDK tools for the platforms. Subsequently it will be sufficient to emulate the application by selecting the appropriate project.

iOS
To be able to emulate the application on iOS it is necessary to establish a connection with the Mac host device, using a tool offered by Xamarin. Once the connection is stable, simply start the project and it will automatically start on the Mac in question.

Python

it is necessary to download the FLASK framework with the related extensions on the official website. Through the terminal it is sufficient to enter the following lines:
"set FLASK_APP=server_connection.py" -> "flask run"
 At this point the server will listen at the address defined in the source code.
