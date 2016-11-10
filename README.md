<H2># AxisActionEventLib 1.0</H2>

Base library that provides connectivity with the new Action and Event web-services API of Axis network cameras or devices.

It uses HTTP communication with SOAP/XML message content.

Based on services wsdl's :

- Action : http://www.axis.com/vapix/ws/action1/ActionService.wsdl
- Events : http://www.axis.com/vapix/ws/event1/EventService.wsdl

Simply download the AxisActionEventLib.dll file in the list above and add a new reference to your visual studio project file and use the object browser to explore the ActionEventLib namespace

<h3>Prerequisite</h3>

- .net 4.5.2

<H3>Intro</H3>

To make a long story short ;-) An Event is represented by an ActionRule and an ActionRule is composed of the following elements : 

- One primary condition (Not mandatory and called EventTrigger in the lib) 
- One or multiple extra conditions (Not mandatory and also called EventTrigger in the lib)
- ActionConfiguration (Mandatory)

<div style="padding:10px;background-color:#f5f5f5;">
! For the ActionRule to be valid you need at least 1 primary condition or 1 extra condition with a valid ActionConfiguration
</div>



<h3>Comments</h3>

- ActionService and EventService inherit abstract base class SOAPRequest. It uses one method sendRequestAsync(...) to send a http request and returns a serviceResponse object which is a base object wrapping the http request response state and XML content. All the other more specific Responses objects inherit from serviceResponse as a base class

- Exceptions are caught and if happen the serviceResponse .isSuccess property will be false and the Content property will contain the exception message 

- The device service address is default hardcoded too <http://yourip/vapix/services> the address mentionned above, you can change it by assigning the Service_URL property of the service object

    <h3>ServiceResponse members:</h3>
    <table>
      <tr>
      <td>bool IsSuccess</td><td>True if the HTTP request was successfull</td>
      </tr>
      <tr>
      <td>HttpStatusCode  StatusCode</td><td>HTTP Response status code</td>
      </tr>
      <tr>
      <td>XElement SOAPContent</td><td>A Linq XElement object containing the HTTP response XML body </td>
      </tr>
      <tr>
      <td>string Content</td><td>Http response content, it can also contain exception messages if one is raised</td>
      </tr>
    </table>
