<H2>C# AxisActionEventLib 1.0</H2>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

<h3>Prerequisite</h3>

- .net 4.5.2

<H3>Actions & Events principles</H3>

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Simply download the AxisActionEventLib.dll file in the list above and add a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

To understand the concept, structure and terminology of the Action & Event webservices have a look at the official documentation here : http://www.axis.com/files/manuals/vapix_event_action_56628_en_1404.pdf

The library encapsulates and facilitates many of the concepts described in the document.

<h3>Comments</h3>

- The default service address of Axis devices is <http://yourip/vapix/services>, this is used by default. This can be changed by assigning the Service_URL property of the service object

