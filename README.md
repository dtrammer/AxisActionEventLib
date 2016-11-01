# AxisActionEventLib 1.0

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Axis devices action and event web-service address : http://yourip/vapix/services

Simply download the AxisActionEventLib.dll file and add it as a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

Quick usage :

- Use the ActionEventLib.action.ActionService and ActionEventLib.event.EventService objects to query the web-service

- ActionService actionService = new ActionService();
  ServiceResponse response = await actionService.GetActionTemplatesAsync("IP", "user", "pass");

