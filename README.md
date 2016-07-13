# IntelliTrace Data Collector

##What is IntelliTrace<sup>:copyright:</sup>
IntelliTrace is a Visual studio feature that enables Historical Debugging of your application. When enabled, IntelliTrace records interesting events and method calls flow for your application. This recorded data/flow can be played forward/back later on-demand that simplify the debugging process.  

If you are new to IntelliTrace, click [here](https://blogs.msdn.microsoft.com/zainnab/2013/02/12/understanding-intellitrace-part-i-what-the-is-intellitrace) for a good tutorial series.

##IntelliTrace for Production Debugging
Visual Studio team recommends to use IntelliTrace for the day to day debugging sessions, however, this powerful tool can also be used for production debugging scenarios. 

Channel 9 video [here](https://channel9.msdn.com/Shows/Visual-Studio-Toolbox/Collecting-IntelliTrace-Data-in-Production) provides an excellent introduction to using  IntelliTrace for Production environment. 

##What do I need IntelliTrace Data Collector
You can collect IntelliTrace logs for different type of applications such as Desktop Application, Windows Service or a Web Portal. However these are different manual steps involved for collecting this data which could be error prone. Link [here](https://msdn.microsoft.com/en-us/library/hh398365(v=vs.110).aspx) describes all the steps requires for a Desktop or Web Application. Similarly you can find instructions for IntelliTrace data collection for a Windows Service [here](https://blogs.msdn.microsoft.com/visualstudioalm/2015/05/14/collect-data-from-a-windows-service-using-the-intellitrace-standalone-collector/). Idea behind IntelliTrace Data Collector is to simplify the process of this data collection specially from a Production environment.

##Configuring a Collection Plan
In order to config a collection plan, I used IntelliTraceCPConfig tool by Vlatko Ivanovski published at [codeplex](https://intellitracecpconfig.codeplex.com/). I just had to make couple of minor modifications in this tool to accept a collection plan as an argument. 

The IntelliTraceCPConfig tool is license under Ms-Pl License [here](https://intellitracecpconfig.codeplex.com/license). A copy of this licnese is included in this project with file named LICENSE - IntelliTraceCPConfig
 
