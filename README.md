<H2># AxisActionEventLib 1.0</H2>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Simply download the AxisActionEventLib.dll file in the list above and add a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

<h3>Prerequisite</h3>

- .net 4.5.2

<H3>Actions & Events principles</H3>

An Event is represented by an ActionRule whitch is composed of the following elements : 

- One primary condition  
- One or multiple extra conditions
- ActionConfiguration

For the ActionRule to be valid you need at least 1 primary condition or 1 extra condition with a valid ActionConfiguration

The conditions are responsible for triggering the ActionRule, they actually correspond to an EventInstance that can be raised on the device. (Conditions or EventInstances are represented by the EventTrigger class in the library, I will use the name EventTrigger from here)

An EventTrigger comes in 2 variants, 'simple' and 'extended'. Both have a TopicExpression member which corresponds to the EventInstance name (for example : tns1:VideoSource/tnsaxis:VideoSource/Tampering).

In the "Simple" variant you only have to assign a TopicExpression value for the EventTrigger to be valid.

In the "Extended" variant the EventTrigger will also have to contain EventTriggerParams which are extra parameters or conditions that have to be met for the EventInstance to be raised by the device.
EventTriggerParams are represented under the form of a KeyValuePair. 
For example the EventInstance : tns1:Device/tnsaxis:Device/IO/VirtualInput needs the following parameters : 
- Name="port", Value="1 to 32"
- Name="active", Value="1 or 0"

A complete description of the available EventInstances and their respective parameters can be found in the Axis VAPIX library documentation here : http://www.axis.com/partner_pages/vapix_library/#/subjects/t10037719/section/t10008227/display?section=t10008227-t10008226 

All Axis devices do share a common set of EventInstances but some models might have (or not) particular EventInstances, for example Thermal cameras will have specific EventInstances that can be raised in relation with temperatures thresholds. 

Now to make things easy (that's the point of a library right :-), you can use the GetEventIntances(...) method of the EventService object which will return a GetEventInstancesResponse object that will contain a List<> of available EventTriggers for that device. 

<h3>Comments</h3>

- ActionService and EventService inherit abstract base class SOAPRequest. It uses one method sendRequestAsync(...) to send a http request and returns a serviceResponse object which is a base object wrapping the http request response state and XML content. All the other more specific Responses objects inherit from serviceResponse.

- Exceptions are caught and if happen the serviceResponse .isSuccess property will be false and the Content property will contain the exception message 

- The device service address is default hardcoded too <http://yourip/vapix/services> the address mentionned above, you can change it by assigning the Service_URL property of the service object
