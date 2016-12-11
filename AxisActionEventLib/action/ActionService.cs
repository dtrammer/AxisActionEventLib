using ActionEventLib.action;
using ActionEventLib.events;
using ActionEventLib.templates;
using ActionEventLib.types;
using AxisActionEventLib.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionEventLib.action
{
    /// <summary>
    /// Class that provides methods to query the Actions webservice API of Axis network devices
    /// The service URL is defaulted in the base SOAPRequest class to "http://{0}/vapix/services" 
    /// this can be changed via the "Service_URL" property don't forget the placeholder for the IP address
    /// Wsdl -> http://www.axis.com/vapix/ws/action1/ActionService.wsdl
    /// Documentation -> http://www.axis.com/partner_pages/vapix_library/#/subjects/t10037719/section/t10008227/display?section=t10008227-t10003111
    /// </summary>
    public sealed class ActionService : SOAPRequest
    {
        #region base service methods

        /// <summary>
        /// Method to create a new Recipient Configuration on a device
        /// If success the ID field of the passed Recipient Configuration instance will be set with the ID returned in the response
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="Configuration"></param>
        /// <returns>Returns the ID of the configuration stored on the device</returns>
        public async Task<ServiceResponse> AddRecipientConfiguration(string IP , string User , string Password , RecipientConfiguration Configuration) {
            return parseAddRecipientConfigResponse(await base.sendRequestAsync(IP,  User,  Password, @"<act:AddRecipientConfiguration><act:NewRecipientConfiguration>" + Configuration.ToString() + @"</act:NewRecipientConfiguration></act:AddRecipientConfiguration>") , Configuration);
        }

        /// <summary>
        /// Method to create a new action configuration, an actionconfiguration is needed for creating an actionRule (event) on the device
        /// If success the ID field of the passed ActionConfiguration instance will be set with the ID returned in the response
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="Configuration">ActionConfiguration reference</param>
        /// <returns>Returns the ID of the configuration stored on the device</returns>
        public async Task<ServiceResponse> AddActionConfiguration(string IP, string User, string Password, ActionConfiguration Configuration ) {
            return parseAddActionConfigResponse(await base.sendRequestAsync(IP, User,  Password, @"<act:AddActionConfiguration><act:NewActionConfiguration>" + Configuration.ToString() + "</act:NewActionConfiguration></act:AddActionConfiguration>"), Configuration);
        }

        /// <summary>
        /// Method to create a new ActionRule (Event) on the device. 
        /// If success the ID field of the passed ActionRule instance will be set with the ID returned in the response
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="NewRule">ActionRule reference</param>
        /// <returns></returns>
        public async Task<ServiceResponse> AddActionRule(string IP, string User, string Password, ActionRule NewRule)
        {
            string actionBody = @"<act:AddActionRule><act:NewActionRule>"
                                + NewRule.ToString()
                                + @"</act:NewActionRule></act:AddActionRule>";

            return this.parseAddActionRuleResponse(await base.sendRequestAsync( IP,  User,  Password, actionBody),NewRule);
        }

        /// <summary>
        /// Method to update an existing actionconfiguration
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <param name="Config"></param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> Update_ActionConfiguration(string IP , string User , string Password , ActionConfiguration Config)
        {
            ServiceResponse response = await this.RemoveActionConfiguration(IP, User, Password, Config.ConfigID);
            if (response.IsSuccess)
                response = await this.AddActionConfiguration(IP, User, Password, Config);
            return response;
        }

        /// <summary>
        /// Method to update an existing action rule
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <param name="Config"></param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> Update_ActionRule(string IP, string User, string Password, ActionRule Rule)
        {
            ServiceResponse response = await this.RemoveActionRule(IP, User, Password, Rule.RuleID);
            if (response.IsSuccess)
                response = await this.AddActionRule(IP, User, Password, Rule);
            return response;
        }

        /// <summary>
        /// Method to retrieve the supported Actions templates of a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns>The supported Action Templates of the device</returns>
        /// 
        public async Task<GetActionTemplatesResponse> GetActionTemplatesAsync(string IP , string User, string Password ) {
            GetRecipientTemplatesResponse recTempResp = await this.GetRecipientTemplatesAsync(IP, User, Password);
            GetActionTemplatesResponse actTempResp = parseGetActionTemplatesResponse(await base.sendRequestAsync(IP,  User,  Password , @"<act:GetActionTemplates />"));
            //Bind a recipientTemplate instance to an actionTemplate instance
            if(recTempResp.IsSuccess)
                foreach(ActionTemplate ac in actTempResp.Templates)
                    if (!string.IsNullOrEmpty(ac.RecipientTemplate))
                        ac.recipientTemplateObj = recTempResp.Templates.Where(x => x.TemplateToken == ac.RecipientTemplate).FirstOrDefault();
            return actTempResp;
        }

        /// <summary>
        /// Method to retrieve the supported recipients configuration templates of a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns>Supported Recipient templates of the device</returns>
        public async Task<GetRecipientTemplatesResponse> GetRecipientTemplatesAsync(string IP, string User, string Password ) {
            return parseGetRecipientTemplatesResponse(await base.sendRequestAsync(IP,  User,  Password, @"<act:GetRecipientTemplates />"));
        }

        /// <summary>
        /// Method to retrieve the ActionConfigurations stored on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns></returns>
        public async Task<GetActionConfigurationsResponse> GetActionConfigurations(string IP, string User, string Password ) {
            return parseGetActionConfigResponse(await base.sendRequestAsync(IP, User, Password, @"<act:GetActionConfigurations/>"));
        }

        /// <summary>
        /// Method to retrieve the existing Action Rules on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns></returns>
        public async Task<GetActionRulesResponse> GetActionRules(string IP, string User, string Password ) {
            GetActionConfigurationsResponse configResponse = await this.GetActionConfigurations(IP, User, Password);
            GetActionRulesResponse rulesResponse = parseGetActionRulesResponse(await base.sendRequestAsync(IP, User, Password, @"<act:GetActionRules/>"));
            if(configResponse.IsSuccess && rulesResponse.IsSuccess)
                foreach(ActionRule r in rulesResponse.ActionRules)
                    r.Configuration = configResponse.Configurations.Where(x => x.ConfigID == r.Configuration.ConfigID).FirstOrDefault();
            return rulesResponse;
        }

        /// <summary>
        /// Method to retrieve the exisiting Recipient Configurations on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns></returns>
        public async Task<GetRecipientConfigurationsResponse> GetRecipientConfigurations(string IP, string User, string Password) {
            return parseGetRecipientConfigurations(await base.sendRequestAsync(IP, User , Password , @"<act:GetRecipientConfigurations/>"));
        }

        /// <summary>
        /// Method to remove an action configuration that is stored on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="id">The ConfigurationID of the ActionConfiguration, this id is returned after a successfull call to [AddActionConfiguration]</param>
        /// <returns>ServiceResponse containing the RemoveActionConfigurationResponse</returns>
        public async Task<ServiceResponse> RemoveActionConfiguration(string IP , string User, string Password , int id) {
            return await base.sendRequestAsync(IP, User , Password, @"<act:RemoveActionConfiguration><act:ConfigurationID>" + id + @"</act:ConfigurationID></act:RemoveActionConfiguration>");
        }

        /// <summary>
        /// Method to remove an existing Action Rule on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="RuleID">Rule ID, it can be obtained with GetActionRules</param>
        /// <returns></returns>
        public async Task<ServiceResponse> RemoveActionRule(string IP, string User, string Password, int RuleID) {
            return await base.sendRequestAsync(IP, User , Password , @"<act:RemoveActionRule><act:RuleID>" + RuleID + @"</act:RuleID></act:RemoveActionRule>");
        }

        /// <summary>
        /// Method to remove a stored Recipient Configuration on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="ConfigurationID">The configuration id, it can be obtained with GetActionConfigurations</param>
        /// <returns></returns>
        public async Task<ServiceResponse> RemoveRecipientConfiguration(string IP, string User, string Password, int ConfigurationID) {
            return parseGetRecipientConfigurations(await base.sendRequestAsync(IP, User , Password, @"<act:RemoveRecipientConfiguration><act:ConfigurationID>" + ConfigurationID + @"</act:ConfigurationID></act:RemoveRecipientConfiguration>"));
        }
        #endregion

        #region XML parsing
        private ServiceResponse parseAddActionConfigResponse(ServiceResponse Response, ActionConfiguration Configuration)
        {
            if (Response.IsSuccess)
                try
                {
                    XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "AddActionConfigurationResponse");
                    Response.Content = configResponse.Element(NS_ACTION + "ConfigurationID").Value;
                    Configuration.ConfigID = int.Parse(Response.Content);
                }
                catch (Exception ex)
                {
                    Response.IsSuccess = false;
                    Response.Content = "[ParseAddActionConfigResponse] " + ex.Message;
                }

            return Response;
        }
        private ServiceResponse parseAddRecipientConfigResponse(ServiceResponse Response, RecipientConfiguration Configuration)
        {
            if (Response.IsSuccess)
                try
                {
                    Response.Content = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "AddRecipientConfigurationResponse").Element(NS_ACTION + "ConfigurationID").Value;
                    Configuration.ConfigurationID = int.Parse(Response.Content);
                }
                catch (Exception ex)
                {
                    Response.IsSuccess = false;
                    Response.Content = "[ParseAddRecipientConfigResponse] " + ex.Message;
                }

            return Response;
        }
        private ServiceResponse parseAddActionRuleResponse(ServiceResponse Response , ActionRule newRule)
        {
            if (Response.IsSuccess)
                try
                {
                    Response.Content = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "AddActionRuleResponse").Element(NS_ACTION + "RuleID").Value;
                    newRule.RuleID = int.Parse(Response.Content);
                }
                catch (Exception ex)
                {
                    Response.IsSuccess = false;
                    Response.Content = "[parseAddActionRuleResponse] " + ex.Message;
                }

            return Response;
        }
        private GetActionTemplatesResponse parseGetActionTemplatesResponse(ServiceResponse Response)
        {
            GetActionTemplatesResponse response = Response.Factory<GetActionTemplatesResponse>();
            if (Response.IsSuccess)
                try
                {
                    XElement templates = response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetActionTemplatesResponse").Element(NS_ACTION + "ActionTemplates");
                    ActionTemplate newTemplate;

                    foreach (XElement el in templates.Elements())
                    {
                        newTemplate = new ActionTemplate();

                        newTemplate.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;

                        if (el.Element(NS_ACTION + "RecipientTemplate") != null)
                            newTemplate.RecipientTemplate = el.Element(NS_ACTION + "RecipientTemplate").Value;

                        foreach (XElement param in el.Element(NS_ACTION + "Parameters").Elements())
                            newTemplate.Parameters.Add(param.Attribute("Name").Value, string.Empty);

                        response.Templates.Add(newTemplate);
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Content = "[ParseActionTemplatesResponse] " + ex.Message;
                }

            return response;

        }
        private GetRecipientTemplatesResponse parseGetRecipientTemplatesResponse(ServiceResponse Response)
        {
            GetRecipientTemplatesResponse response = Response.Factory<GetRecipientTemplatesResponse>();
            if (Response.IsSuccess)
                try
                {
                    XElement templates = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetRecipientTemplatesResponse").Element(NS_ACTION + "RecipientTemplates");
                    RecipientTemplate newTemplate;

                    foreach (XElement el in templates.Elements())
                    {
                        newTemplate = new RecipientTemplate();

                        newTemplate.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;

                        foreach (XElement param in el.Element(NS_ACTION + "Parameters").Elements())
                            newTemplate.Parameters.Add(param.Attribute("Name").Value, string.Empty);

                        response.Templates.Add(newTemplate);
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Content = "[ParseRecipientTemplatesResponse] " + ex.Message;
                }

            return response;
        }
        private GetActionConfigurationsResponse parseGetActionConfigResponse(ServiceResponse Response)
        {
            GetActionConfigurationsResponse response = Response.Factory<GetActionConfigurationsResponse>();
            if (Response.IsSuccess)
                try
                {
                    XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetActionConfigurationsResponse").Element(NS_ACTION + "ActionConfigurations");

                    ActionConfiguration conf;

                    foreach (XElement el in configResponse.Elements())
                    {
                        conf = new ActionConfiguration();
                        conf.ConfigID = int.Parse(el.Element(NS_ACTION + "ConfigurationID").Value);
                        conf.Name = el.Element(NS_ACTION + "Name").Value;
                        conf.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;

                        foreach (XElement element in el.Element(NS_ACTION + "Parameters").Elements())
                            conf.Parameters.Add(element.Attribute("Name").Value, element.Attribute("Value").Value);

                        response.Configurations.Add(conf);
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Content = "[ParseActionConfigResponse] " + ex.Message;
                }

            return response;
        }
        private GetActionRulesResponse parseGetActionRulesResponse(ServiceResponse Response)
        {
            GetActionRulesResponse response = Response.Factory<GetActionRulesResponse>();
            if (Response.IsSuccess)
                try
                {
                    XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetActionRulesResponse").Element(NS_ACTION + "ActionRules");
                    ActionRule rule;
                    foreach (XElement el in configResponse.Elements())
                    {
                        rule = new ActionRule();
                        //parse rule base info
                        rule.RuleID = int.Parse(el.Element(NS_ACTION + "RuleID").Value);
                        rule.Name = el.Element(NS_ACTION + "Name").Value;
                        rule.Enabled = bool.Parse(el.Element(NS_ACTION + "Enabled").Value);
                        //parse rule startevent
                        if (el.HasElement(NS_ACTION + "StartEvent"))
                            rule.Trigger = new EventTrigger(
                                el.Element(NS_ACTION + "StartEvent").Element(NS_TOPIC + "TopicExpression").Value, false ,
                                el.Element(NS_ACTION + "StartEvent").GetElementValue(NS_TOPIC + "MessageContent")
                            );
                        //parse rule conditions
                        if (el.HasElement(NS_ACTION + "Conditions"))
                            foreach (XElement condition in el.Element(NS_ACTION + "Conditions").Elements())
                            {
                                rule.AddExtraCondition(
                                    new EventTrigger(
                                        condition.Element(NS_TOPIC + "TopicExpression").Value, false,
                                        condition.GetElementValue(NS_TOPIC + "MessageContent")
                                    )
                                );
                            }
                        //parse rule actionconfiguration id
                        rule.Configuration = new ActionConfiguration() { ConfigID = int.Parse(el.Element(NS_ACTION + "PrimaryAction").Value) };
                        //parse activation timout
                        if (el.HasElement(NS_ACTION + "ActivationTimeout"))
                            rule.SetActivationTimeout(int.Parse(Regex.Match(el.Element(NS_ACTION + "ActivationTimeout").Value, @"\d+").Value));

                        response.ActionRules.Add(rule);
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Content = "[ParseActionRulesResponse] " + ex.Message;
                }

            return response;
        }
        private GetRecipientConfigurationsResponse parseGetRecipientConfigurations(ServiceResponse Response)
        {
            GetRecipientConfigurationsResponse response = Response.Factory<GetRecipientConfigurationsResponse>();
            if (Response.IsSuccess)
                try
                {
                    XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetRecipientConfigurationsResponse").Element(NS_ACTION + "RecipientConfigurations");
                    RecipientConfiguration recipient;

                    foreach (XElement el in configResponse.Elements())
                    {
                        recipient = new RecipientConfiguration();
                        recipient.ConfigurationID = int.Parse(el.Element(NS_ACTION + "ConfigurationID").Value);
                        recipient.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;
                        recipient.Name = el.Element(NS_ACTION + "Name").Value;

                        foreach (XElement element in el.Element(NS_ACTION + "Parameters").Elements())
                        {
                            recipient.Parameters.Add(
                                element.Attribute("Name").Value, element.Attribute("Value").Value
                                );
                        }

                        response.Configurations.Add(recipient);
                    }
                }
                catch (Exception ex)
                {
                    Response.IsSuccess = false;
                    Response.Content = "[ParseRecipientConfigurations] " + ex.Message;
                }

            return response;
        }
        #endregion
    }
}
