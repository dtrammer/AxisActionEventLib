using ActionEventLib.action;
using ActionEventLib.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.actions
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
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="Configuration"></param>
        /// <returns>Returns the ID of the configuration stored on the device</returns>
        public async Task<ServiceResponse> AddRecipientConfiguration(string IP , string User , string Password , RecipientConfiguration Configuration) {
            return await base.sendRequestAsync(IP,  User,  Password, @"<act:AddRecipientConfiguration><act:NewRecipientConfiguration>" + Configuration.ToString() + @"</act:NewRecipientConfiguration></act:AddRecipientConfiguration>");
        }

        /// <summary>
        /// Method to create a new action configuration
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <param name="Configuration">ActionConfiguration Object</param>
        /// <returns>Returns the ID of the configuration stored on the device</returns>
        public async Task<ServiceResponse> AddActionConfiguration(string IP, string User, string Password, ActionConfiguration Configuration ) {
            return base.parseAddActionConfigResponse(
                await base.sendRequestAsync(IP, User,  Password,
                "<act:AddActionConfiguration><act:NewActionConfiguration>" + Configuration.ToString() + "</act:NewActionConfiguration></act:AddActionConfiguration>"
            ), Configuration);
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
            return base.parseGetActionTemplatesResponse(await base.sendRequestAsync(IP,  User,  Password , @"<act:GetActionTemplates />"));
        }

        /// <summary>
        /// Method to retrieve the supported recipients configuration templates of a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns>Supported Recipient templates of the device</returns>
        public async Task<GetRecipientTemplatesResponse> GetRecipientTemplatesAsync(string IP, string User, string Password ) {
            return base.parseGetRecipientTemplatesResponse(await base.sendRequestAsync(IP,  User,  Password, @"<act:GetRecipientTemplates />"));
        }

        /// <summary>
        /// Method to retrieve the ActionConfigurations stored on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns></returns>
        public async Task<GetActionConfigurationsResponse> GetActionConfigurations(string IP, string User, string Password ) {
            return base.parseGetActionConfigResponse(await base.sendRequestAsync(IP, User, Password, @"<act:GetActionConfigurations/>"));
        }

        /// <summary>
        /// Method to retrieve the existing Action Rules on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns></returns>
        public async Task<GetActionRulesResponse> GetActionRules(string IP, string User, string Password ) {
            return base.parseGetActionRulesResponse(await base.sendRequestAsync(IP , User, Password , @"<act:GetActionRules/>" ));
        }

        /// <summary>
        /// Method to retrieve the exisiting Recipient Configurations on a device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User to authenticate the http request</param>
        /// <param name="Password">Password to use</param>
        /// <returns></returns>
        public async Task<GetRecipientConfigurationsResponse> GetRecipientConfigurations(string IP, string User, string Password) {
            return base.parseGetRecipientConfigurations(await base.sendRequestAsync(IP, User , Password , @"<act:GetRecipientConfigurations/>"));
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
            return base.parseGetRecipientConfigurations(await base.sendRequestAsync(IP, User , Password, @"<act:RemoveRecipientConfiguration><act:ConfigurationID>" + ConfigurationID + @"</act:ConfigurationID></act:RemoveRecipientConfiguration>"));
        }
        #endregion
    }
}
