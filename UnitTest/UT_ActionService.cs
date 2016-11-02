using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActionEventLib.action;
using ActionEventLib.types;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ActionEventLib.templates;
using ActionEventLib.events;

namespace ActionEventLibTests
{
    [TestClass]
    public class UT_ActionService
    {
        ActionService m_service = new ActionService();
        string VALID_USER = "root";
        string VALID_PASS = "pass";
        string VALID_IP = "192.168.1.218";

        #region Action Templates
        [TestMethod]
        public async Task GetActionTemplatesWithValidParams_IsSuccessAndContainsHTTP200AndHasXMLcontentAndHasTemplates()
        {
            GetActionTemplatesResponse response = await m_service.GetActionTemplatesAsync(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Templates.Count > 0);
        }

        [TestMethod]
        public async Task GetActionTemplatesWithWrongParams_ResponseReturnsFalseAndExceptionMessage()
        {
            GetActionTemplatesResponse response = await m_service.GetActionTemplatesAsync("192.168.1.2184", VALID_USER, VALID_PASS);

            Assert.IsTrue(!response.IsSuccess && response.Content.Length > 5);
        }
        #endregion

        #region Action Rules

        [TestMethod]
        public async Task GetActionRulesWithValidParams_IsSuccessAndContainsHTTP200AndHasXMLContentAndHasRuleInstances()
        {
            GetActionRulesResponse response = await m_service.GetActionRules(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.ActionRules.Count > 0);
        }

        ActionRule m_actionRule;
        [TestMethod]
        public async Task AddActionRuleWithValidParams_IsSuccessAndReturnsRuleID()
        {
            m_actionRule = new ActionRule()
            {
                Name = "UnitTestRule",
                Enabled = true,
                Trigger = new EventTrigger(),
                Configuration = new ActionConfiguration() { ConfigurationID = 9 }
            };

            m_actionRule.Trigger.setExtTopic("tns1:Device/tnsaxis:Status/Temperature/Above_or_below", "boolean(//SimpleItem[@Name=\"sensor_level\" and @Value=\"1\"]) and boolean(//SimpleItem[@Name=\"sensor\" and @Value=\"0\"])");

            ServiceResponse response = await m_service.AddActionRule(VALID_IP, VALID_USER, VALID_PASS , m_actionRule);

            Console.WriteLine(response.IsSuccess);
            Console.WriteLine("[AddActionRuleWithValidParams] " + m_actionRule.RuleID);

            Assert.IsTrue(response.IsSuccess && !response.SOAPContent.IsEmpty && m_actionRule.RuleID != 0);
        }
        #endregion

        #region ActionConfig
        [TestMethod]
        public async Task GetActionConfigurationsWithValidParams_IsSuccessAndContainsHTTP200AndHasXMLContentAndHasItems()
        {
            GetActionConfigurationsResponse response = await m_service.GetActionConfigurations(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Configurations != null);
        }
        [TestMethod]
        public async Task GetActionConfigurationsWithWrongCredentials_IsUnauthorized()
        {
            GetActionConfigurationsResponse response = await m_service.GetActionConfigurations(VALID_IP, "root2", VALID_PASS);

            Assert.IsTrue(response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        ActionConfiguration m_actionConfiguration;
        [TestMethod]
        public async Task AddActionConfigurationsWithValidParams_IsSuccessAndReturnsConfigurationID()
        {
            m_actionConfiguration = new ActionConfiguration()
            {
                Name = "UnitTestConfig",
                actionTemplate = new ActionTemplate() { TemplateToken = "com.axis.action.unlimited.set_overlay" }
            };
            m_actionConfiguration.actionTemplate.Parameters.Add("text", "Hello world!");
            m_actionConfiguration.actionTemplate.Parameters.Add("channels", "1");

            ServiceResponse response = await m_service.AddActionConfiguration(VALID_IP , VALID_USER , VALID_PASS , m_actionConfiguration);

            Console.WriteLine("[AddActionConfigurations] " + m_actionConfiguration.ConfigurationID);

            Assert.IsTrue(response.IsSuccess && m_actionConfiguration.ConfigurationID != 0);
        }

        [TestMethod]
        public async Task RemoveActionConfigurationsWithValidParams_IsSuccessAndReturnsRemoveActionConfigurationResponse()
        {
            ServiceResponse response = await m_service.RemoveActionConfiguration(VALID_IP, VALID_USER, VALID_PASS, m_actionConfiguration.ConfigurationID);

            Assert.IsTrue(response.IsSuccess && Regex.IsMatch(response.SOAPContent.ToString(), "RemoveActionConfigurationResponse"));
        }
        #endregion

        #region Recipients
        [TestMethod]
        public async Task GetRecipientTemplatesWithValidParams_IsSuccessAndContainsHTTP200AndHasXMLContentAndHasTemplates()
        {
            GetRecipientTemplatesResponse response = await m_service.GetRecipientTemplatesAsync(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Templates.Count > 0);
        }
        [TestMethod]
        public async Task GetRecipientConfigWithValidParams_IsSuccessAndContainsHTTP200AndHasXMLContentAndHasRecipientInstances()
        {
            GetRecipientConfigurationsResponse response = await m_service.GetRecipientConfigurations(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Configurations != null);
        }

        RecipientConfiguration m_recipientConfig;
        [TestMethod]
        public async Task AddRecipientConfigWithValidParams_IsSuccessAndReturnsConfigID()
        {
            m_recipientConfig = new RecipientConfiguration()
            {
                TemplateToken = "com.axis.recipient.tcp",
                Name = "TestConfig"
            };

            m_recipientConfig.Parameters.Add("host", VALID_IP);
            m_recipientConfig.Parameters.Add("port", "80");
            m_recipientConfig.Parameters.Add("qos", "");

            ServiceResponse response = await m_service.AddRecipientConfiguration(VALID_IP, VALID_USER, VALID_PASS , m_recipientConfig);

            Console.WriteLine("[AddRecipientConfigWithValidParams] " + m_recipientConfig.ConfigurationID);

            Assert.IsTrue(response.IsSuccess && m_recipientConfig.ConfigurationID != 0);
        }

        [TestMethod]
        public async Task RemoveRecipientConfigWithValidParams_IsSuccessWithRemoveRecipientConfigurationResponse()
        {
            ServiceResponse response = await m_service.RemoveRecipientConfiguration(VALID_IP, VALID_USER, VALID_PASS, m_recipientConfig.ConfigurationID);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && Regex.IsMatch(response.SOAPContent.ToString() , "RemoveRecipientConfigurationResponse"));
        }
        #endregion

    }
}
