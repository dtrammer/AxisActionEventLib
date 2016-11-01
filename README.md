<H1># AxisActionEventLib 1.0</H1>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Simply download the AxisActionEventLib.dll file in the list above and add a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

Quick usage :

- Axis devices action and event web-service address : http://yourip/vapix/services

- Use the ActionEventLib.action.ActionService and ActionEventLib.event.EventService objects to query the web-service

SAMPLE : 
<div style="background-color: #f8f8f8;padding: 10px;">
<code>
ActionService actionService = new ActionService();</br>
GetActionTemplatesResponse response = await actionService.GetActionTemplatesAsync("IP", "user", "pass");
</code>
</br>
<code>
EventService eventService = new EventService();</br>
GetEventInstancesResponse Response = await eventService.GetEventsInstancesAsync("IP", "user", "pass");
</code>
</div>
