# AxisActionEventLib v1

Base library that provides connectivity with the new Action and Event web-services for Axis network cameras or devices.

HTTP communication with SOAP/XML message content

Based on services wsdl :

Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Axis device action and event web-service address : http://{0}/vapix/services

Structure library namespaces : 

action 
  ActionConfiguration
  ActionRule
  ActionService
  RecipientConfiguration
event
  Eventservice
  EventTrigger
templates
  ActionTemplate
  RecipientTemplate
types
  ICalendar
  ServiceResponse
  SOAPRequest
  
  
Usage :
