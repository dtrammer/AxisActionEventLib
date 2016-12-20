<H2>C# AxisActionEventLib 1.0</H2>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

<h3>Prerequisite & install</h3>

- .net 4.5.2
- Download the AxisActionEventLib.dll and add it as a reference to your VS project
  Use the object browser to explore the ActionEventLib namespace

<H3>Actions & Events principles</H3>

To understand the concept, structure and terminology of the Action & Event webservices have a look at the official documentation here : http://www.axis.com/files/manuals/vapix_event_action_56628_en_1404.pdf.
The library encapsulates and provides strongly typed objects for all the concepts described in the document.

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl


<h3>Comments</h3>

- Events are represented by ActionRule instances, it specifies how and when the Axis product performs an action. Example: record video when motion is detected outside office hours. An action rule consists of:
    - a start event
    - one or more conditions 
    - a primary action
    
   The primary action will be executed when the start event occurs and all specified conditions are fulfilled. 
   The start event can be omitted. Conditions can also be omitted, but either a start event or at least one condition must be specified.
   The action will be stopped when any of the conditions is no longer fulfilled.

- The default service address of Axis devices is <http://yourip/vapix/services>, this is used by default. This can be changed by assigning the Service_URL property of a service object

- All Action|Event services instance methods are asynchronous 


<h3>Quick samples</h3>

- For more samples have a look at the test class in the UnitTest folder

- Get Action | Recipient Templates
    
    ActionService actionService = new ActionService();</br>    
    GetActionTemplatesResponse actionTemplates = await actionService.GetActionTemplatesAsync( "192.168.1.10" , "root" , "pass" );

- Get EventInstances
    
    EventService eventService = new EventService();
    
    GetEventInstancesResponse eventInstances = await eventService.GetEventsInstancesAsync( "192.168.1.10" , "root" , "pass" );  

- Create a new Actionrule (event)
    
  Unless you know the different parameters that compose an ActionConfiguration template and Event Instance it's recommended that you first use the GetActionTemplates method and the GetEventInstances method to get the supported templates and events instances for the targeted device. This will also provide you with strongly typed instances that you can use directly to setup the action rule, example :
  
  
    

The responses instances returned by the Action|Event services methods all share the same base type "ServiceResponse". The more specific types only add one or more extra members.(ex.: GetActionTemplatesResponse will only append a new List<ActionTemplate> member)
<table>
<th colspan="2">ServiceResponse</th>
<tr><td>bool IsSuccess</td><td>True if request succeeded with HTTP status 200, False in all other cases</td></tr>
<tr><td>HttpStatusCode HttpStatusCode</td><td>The status code of the http response</td></tr>
<tr><td>string Content</td><td>Will contain the result of the request if it succeeded, in other case it will contain the response body or exceptions messages</td></tr>
<tr><td>XElement SOAPContent</td><td>XML DOM object containing the SOAP/XML response body of a successfull request</td></tr>
</table>
