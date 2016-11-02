using ActionEventLib.action;
using ActionEventLib.events;
using ActionEventLib.templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionEventLib.types
{
    /// <summary>
    /// Base class for sending HTTP requests with XML/SOAP content to the Action and Event web-services of Axis network devices
    /// The service URL is defaulted to "http://{0}/vapix/services" but can be changed by assigning the Service_URL property 
    /// </summary>
    public abstract class SOAPRequest
    {
        #region namespaces constants
        protected const string NS_SOAP_ENV = @"{http://www.w3.org/2003/05/soap-envelope}";
        protected const string NS_ONVIF = @"{http://www.onvif.org/ver10/schema}";
        protected const string NS_ACTION = @"{http://www.axis.com/vapix/ws/action1}";
        protected const string NS_EVENT = @"{http://www.axis.com/vapix/ws/event1}";
        protected const string NS_TOPIC = @"{http://docs.oasis-open.org/wsn/b-2}";
        protected const string NS_WTOPIC = @"{http://docs.oasis-open.org/wsn/t-1}";
        protected const string NS_TNS1 = @"{http://www.onvif.org/ver10/topics}";
        protected const string NS_TNSAXIS = @"{http://www.axis.com/2009/event/topics}";
        #endregion
        
        protected string _message_base = "<soap:Envelope xmlns:soap=\"" + NS_SOAP_ENV.Substring(1 , NS_SOAP_ENV.Length - 2)
                                                    + "\" xmlns:act=\"" + NS_ACTION.Substring(1, NS_ACTION.Length - 2) + "\" xmlns:even=\"" + NS_EVENT.Substring(1, NS_EVENT.Length - 2) + "\" xmlns:wsnt=\"" + NS_TOPIC.Substring(1, NS_TOPIC.Length - 2)
                                                    + "\" xmlns:tns1=\"" + NS_TNS1.Substring(1, NS_TNS1.Length - 2) + "\" xmlns:tt=\"" + NS_ONVIF.Substring(1, NS_ONVIF.Length - 2) + "\" xmlns:tnsaxis=\"" + NS_TNSAXIS.Substring(1, NS_TNSAXIS.Length - 2) + "\" >"
                                                    + @"<soap:Body>{0}</soap:Body>"
                                                + @"</soap:Envelope>";

        private string _service_url = "http://{0}/vapix/services";
        public string Service_URL {  get { return _service_url; } set { _service_url = value; } }

        protected async Task<ServiceResponse> sendRequestAsync(string IP, string User , string Password, string Action) {

            using (HttpClient httpClient = new HttpClient(new HttpClientHandler()
            {
                Credentials = new NetworkCredential(
                        User,
                        Password
                    ).GetCredential(new Uri(@"http://localhost"), "Digest")
            }, true))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(25);
                ServiceResponse serviceResponse = new ServiceResponse();

                try
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format(Service_URL, IP)) { Version = HttpVersion.Version10 })
                    {
                        request.Content = new StringContent(string.Format(_message_base, Action));

                        //DEBUG
                        Console.WriteLine("REQUEST CONTENT : " + await request.Content.ReadAsStringAsync());
                        //END DEBUG

                        HttpResponseMessage Response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

                        if (Response.IsSuccessStatusCode)
                        {

                            serviceResponse.IsSuccess = true; serviceResponse.HttpStatusCode = Response.StatusCode;
                            serviceResponse.SOAPContent = XElement.Parse(await Response.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            Console.WriteLine("FAULT RESPONSE CONTENT : " + await Response.Content.ReadAsStringAsync());
                            serviceResponse.IsSuccess = false;
                            serviceResponse.Content = await Response.Content.ReadAsStringAsync();
                            serviceResponse.HttpStatusCode = Response.StatusCode;
                        }

                    }
                }catch(Exception ex)
                {
                    serviceResponse.IsSuccess = false;
                    serviceResponse.Content = "[SendSOAPRequest] " + ex.Message;
                }

                return serviceResponse;
            }
        }

    }
}
