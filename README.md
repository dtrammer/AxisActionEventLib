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

<h3>Comments :</h3>

- ActionService and EventService inherit abstract base class SOAPRequest. It uses one method sendRequestAsync(...) to send a http request and returns a serviceResponse object which is a base object wrapping the http request response state and XML content. All the other more specific Responses objects inherit from serviceResponse

ServiceResponse members:
</br>bool IsSuccess; //True if HTTP response 200 OK
</br>HttpStatusCode HttpStatusCode; //HTTP Response status code
</br>XElement SOAPContent; //An Linq XElement object containing the HTTP response XML content
</br>string Content; //Scalar values are returned here

- Exceptions are processed caught and if happen the serviceResponse .isSuccess property will be false and the Content property will contain the exception message
