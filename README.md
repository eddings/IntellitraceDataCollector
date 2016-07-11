# IntelliTrace Data Collector
IntelliTrace is a Visual studio feature that enables Historical Debugging. When enabled, IntelliTrace records interesting events and method calls flow for your application. This recorded data/flow can be played forward/back later on-demand that simplify the debugging process. 

Visual Studio team recommends to use IntelliTrace for the day to day debugging sessions, however, this powerful tool can also be used for production debugging scenarios. 

In order to config a collection plan, I used IntelliTraceCPConfig tool by Vlatko Ivanovski published at https://intellitracecpconfig.codeplex.com/
I had to make couple of modifications in the tool to accept a collection plan file name. The IntelliTraceCPConfig tool is license under Ms-Pl License as stated at following link.
https://intellitracecpconfig.codeplex.com/license
A copy of this licnese is included in this project with file named LICENSE - IntelliTraceCPConfig
 