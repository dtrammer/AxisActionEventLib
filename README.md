<H2># AxisActionEventLib 1.0</H2>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Simply download the AxisActionEventLib.dll file in the list above and add a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

<H3>Quick usage :</H3>

- Axis devices action and event web-service address : http://yourip/vapix/services

- Use the ActionEventLib.action.ActionService and ActionEventLib.event.EventService objects to query the web-service

<h3>Sample :</h3>

ActionService actionService = new ActionService();</br>
GetActionTemplatesResponse response = await actionService.GetActionTemplatesAsync("IP", "user", "pass");
</br>
EventService eventService = new EventService();</br>
GetEventInstancesResponse Response = await eventService.GetEventsInstancesAsync("IP", "user", "pass");


