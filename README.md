# IntelliTrace Data Collector

##What is IntelliTrace<sup>:copyright:</sup>
IntelliTrace is a Visual studio feature that enables historical debugging of your application. When enabled, IntelliTrace records interesting events and method calls flow for your application. These interesting events and methods are configured via something called Collection Plans. This recorded information can then be played forward/back later on-demand that greatly simplifies the debugging process.  

If you are new to IntelliTrace, click [here](https://blogs.msdn.microsoft.com/zainnab/2013/02/12/understanding-intellitrace-part-i-what-the-is-intellitrace) for a good tutorial series.

##IntelliTrace for Production Debugging
IntelliTrace in not only meant for use during debugging sessions within Visual Studio. This powerful tool can also be used for production debugging scenarios. The process involves collection of IntelliTrace logs from production environment using IntelliTrace Standalone Collector that can be downloaded from [this](https://www.microsoft.com/en-us/download/confirmation.aspx?id=44909) link.

Channel 9 video [here](https://channel9.msdn.com/Shows/Visual-Studio-Toolbox/Collecting-IntelliTrace-Data-in-Production) provides an excellent introduction to using  IntelliTrace for Production environment. 

##What problem IntelliTrace Data Collector solves?
IntelliTrace could be used against different types of applications such as Desktop (Winform/WPF) Applications, Web Applications and Windows Services. The IntelliTrace logs collection of each of this type of application requires different steps. For example, for Winform/WPF application, it requires to launch target application using IntelliTrace Standalone Collector executable (called IntelliTraceSC.exe) with appropriate command line arguments and switches. Whereas for a Web Application, it uses PowerShell cmdlets to do the job. For case of a Windows Service, registry settings for target Windows Service need to be edited with appropriate values. You can find details of these steps at following links.

1. [Desktop and Web Applications](https://msdn.microsoft.com/en-us/library/hh398365.aspx)
2. [Windows Services](https://blogs.msdn.microsoft.com/visualstudioalm/2015/05/14/collect-data-from-a-windows-service-using-the-intellitrace-standalone-collector/)

The idea behind IntelliTrace Data Collector is to simplify the process of data collection from a Production environment through a user intuitive  UI.

##Configuring a Collection Plan
In order to config a collection plan, I used IntelliTraceCPConfig tool by Vlatko Ivanovski published at [codeplex](https://intellitracecpconfig.codeplex.com/). I just had to make couple of minor modifications in this tool to accept a collection plan as an argument. 

The IntelliTraceCPConfig tool is license under Ms-Pl License [here](https://intellitracecpconfig.codeplex.com/license). A copy of this license is included in this project with file named LICENSE - IntelliTraceCPConfig
 
