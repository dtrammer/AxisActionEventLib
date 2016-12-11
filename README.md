<H2>C# AxisActionEventLib 1.0</H2>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

<h3>Prerequisite & install</h3>

- .net 4.5.2
- Download the AxisActionEventLib.dll and add it as a reference to your VS project


<H3>Actions & Events principles</H3>

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Simply download the AxisActionEventLib.dll file in the list above and add a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

To understand the concept, structure and terminology of the Action & Event webservices have a look at the official documentation here : http://www.axis.com/files/manuals/vapix_event_action_56628_en_1404.pdf

The library encapsulates and facilitates many of the concepts described in the document.


<h3>Comments</h3>

- The default service address of Axis devices is <http://yourip/vapix/services>, this is used by default. This can be changed by assigning the Service_URL property of the service object

- All Action|Event services objects methods are asynchronous 


<h3>Samples</h3>

- Instantiate an ActionService or EventService object

  ActionService actionService = new ActionService();
  EventService eventService = new EventService();

- Get Action | Recipient Templates and EventInstances

  GetActionTemplatesResponse response = await actionService.GetActionTemplatesAsync( "192.168.1.10" , "root" , "pass" );
  GetEventInstancesResponse response = await eventService.GetEventsInstancesAsync( "192.168.1.10" , "root" , "pass" );


The responses instances returned by the Action|Event services methods all share the same base type "ServiceResponse"
<table>
<th colspan="2">ServiceResponse</th>
<tr><td>bool IsSuccess</td><td>True if request succeeded with HTTP status 200, False in all other cases</td></tr>
<tr><td>HttpStatusCode HttpStatusCode</td><td>The status code of the http response</td></tr>
<tr><td>string Content</td><td>Will contain the result of the request if it succeeded, in other case it will contain the response body or exceptions messages</td></tr>
<tr><td>XElement SOAPContent</td><td>The SOAP/XML response body of a successfull request</td></tr>
</table>
