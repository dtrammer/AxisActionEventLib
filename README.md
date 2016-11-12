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

To understand the concept, structure and terminology of the Action & Event webservices please have a look at the official documentation here : http://www.axis.com/files/manuals/vapix_event_action_56628_en_1404.pdf

<h3>Comments</h3>

- ActionService and EventService inherit abstract base class SOAPRequest. It uses one method sendRequestAsync(...) to send a http request and returns a serviceResponse object which is a base object wrapping the http request response state and XML content. All the other more specific Responses objects inherit from serviceResponse.

- Exceptions are caught and if happen the serviceResponse .isSuccess property will be false and the Content property will contain the exception message 

- The device service address is default hardcoded too <http://yourip/vapix/services> the address mentionned above, you can change it by assigning the Service_URL property of the service object
