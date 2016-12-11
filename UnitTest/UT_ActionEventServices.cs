using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ActionEventLib.action;
using ActionEventLib.types;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ActionEventLib.templates;
using ActionEventLib.events;
using AxisActionEventLib.events;

namespace ActionEventLibTests
{
    [TestClass]
    public class UT_ActionEventServices
    {
        string VALID_USER = "root";
        string VALID_PASS = "pass";
        string VALID_IP = "192.168.1.67";
        string WRONG_IP = "192.168.1.67777";
        string WRONG_USERPASS = "rooooot";

        #region ActionService

        ActionService actionService = new ActionService();
        ActionRule newActionRule;
        int VALID_ACTIONCONFIG_ID = 2;

        #region Action Templates
        [TestMethod]
        public async Task Get_ActionTemplates()
        {
            GetActionTemplatesResponse response = await actionService.GetActionTemplatesAsync(VALID_IP, VALID_USER, VALID_PASS);

            foreach (ActionTemplate t in response.Templates)
            {
                Console.WriteLine("New template : " + t.ToString());
                Console.WriteLine("\t" + "Recipient : " + t.RecipientTemplate);
                Console.WriteLine("\t" + "Ref token : " + (t.recipientTemplateObj != null ? t.recipientTemplateObj.TemplateToken : "Null"));
                Console.WriteLine("Template parameters:");
                foreach (string s in t.Get_Parameters())
                    Console.WriteLine("\t" + s);
            }
            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Templates.Count > 0);
        }
        [TestMethod]
        public async Task Get_ActionTemplatesWithWrongIP_ResponseReturnsFalseAndExplicitExceptionMessage()
        {
            GetActionTemplatesResponse response = await actionService.GetActionTemplatesAsync(WRONG_IP, VALID_USER, VALID_PASS);

            Console.WriteLine("Response IsSuccess : " + response.IsSuccess + " : Message : " + response.Content);

            Assert.IsTrue(!response.IsSuccess && response.Content.Length > 5);
        }
        [TestMethod]
        public async Task Get_ActionTemplatesWithWrongUserPass_ResponseReturnsFalseAndExceptionMessage()
        {
            GetActionTemplatesResponse response = await actionService.GetActionTemplatesAsync(VALID_IP, WRONG_USERPASS, WRONG_USERPASS);

            Console.WriteLine("Response IsSuccess : " + response.IsSuccess + " : Message : " + response.Content);

            Assert.IsTrue(!response.IsSuccess && response.Content.Length > 5);
        }
    #endregion

        #region Action Rules
    [TestMethod]
        public async Task Get_ActionRules()
        {
            GetActionRulesResponse response = await actionService.GetActionRules(VALID_IP, VALID_USER, VALID_PASS);

            if (!response.IsSuccess)
                Console.WriteLine("NOT 200 OK : " + response.Content);
            else
                foreach (ActionRule ar in response.ActionRules)
                {
                    Console.WriteLine(ar.ToString());
                    Console.WriteLine(ar.Configuration.ToString());
                }

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty);
        }

        [TestMethod]
        public async Task Add_ActionRuleWithValidParams_IsSuccessAndReturnsRuleID()
        {
            newActionRule = new ActionRule()
            {
                Name = "UnitTestRule",
                Enabled = true,
                Trigger = new EventTrigger("tns1:VideoSource/tnsaxis:LiveStreamAccessed", true, "boolean(//SimpleItem[@Name=\"accessed\" and @Value=\"1\"]"),
                Configuration = new ActionConfiguration() { ConfigID = VALID_ACTIONCONFIG_ID }
            };

            ServiceResponse response = await actionService.AddActionRule(VALID_IP, VALID_USER, VALID_PASS , newActionRule);

            if (!response.IsSuccess)
                Console.WriteLine("Error : " + response.Content);
            else
                Console.WriteLine("New action rule id : " + newActionRule.RuleID);

            Assert.IsTrue(response.IsSuccess && !response.SOAPContent.IsEmpty && newActionRule.RuleID != 0);
        }

        /// <summary>
        /// Purpose of this test is to try to create a new action rule with no startevent and a non propertystate eventtrigger instance as condition
        /// This can be faked 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Add_ActionRuleWithInvalidParams()
        {
            newActionRule = new ActionRule()
            {
                Name = "MyBadRule",
                Enabled = true,
                Configuration = new ActionConfiguration() { ConfigID = VALID_ACTIONCONFIG_ID },
                Trigger = new EventTrigger("tns1:VideoSource/tnsaxis:LiveStreamAccessed", true, "boolean(//SimpleItem[@Name=\"accessed\" and @Value=\"1\"]")
            }; 


            ServiceResponse response = await actionService.AddActionRule(VALID_IP, VALID_USER, VALID_PASS, newActionRule);

            Console.WriteLine("[AddActionRuleWithInvalidParams] Rule ID : " + response.Content);

            Assert.IsTrue(response.IsSuccess);
        }
    #endregion

        #region ActionConfig
        [TestMethod]
            public async Task Get_ActionConfigurations()
            {
                GetActionConfigurationsResponse response = await actionService.GetActionConfigurations(VALID_IP, VALID_USER, VALID_PASS);

                if (response.IsSuccess)
                    foreach (ActionConfiguration ac in response.Configurations)
                        Console.WriteLine(ac.ToString());

                Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty);
            }
            [TestMethod]
            public async Task Get_ActionConfigurationsWithWrongCredentials_IsUnauthorized()
            {
                GetActionConfigurationsResponse response = await actionService.GetActionConfigurations(VALID_IP, "root2", VALID_PASS);

                Assert.IsTrue(response.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized);
            }

            ActionConfiguration m_actionConfiguration;
            [TestMethod]
            public async Task Add_Action_Configurations()
            {
                m_actionConfiguration = new ActionConfiguration()
                {
                    Name = "UnitTestConfig",
                    TemplateToken = "com.axis.action.fixed.notification.http" 
                };

                //Action Template params
                m_actionConfiguration.Parameters.Add("parameters", "action=list");
                m_actionConfiguration.Parameters.Add("message", "Hello world!");
                //Recipient Template params
                m_actionConfiguration.Parameters.Add("upload_url", "http://10.21.66.24");
                m_actionConfiguration.Parameters.Add("login", "root");
                m_actionConfiguration.Parameters.Add("password", "pass");
                m_actionConfiguration.Parameters.Add("proxy_host", "");
                m_actionConfiguration.Parameters.Add("proxy_port", "");
                m_actionConfiguration.Parameters.Add("proxy_login", "");
                m_actionConfiguration.Parameters.Add("proxy_password", "");
                m_actionConfiguration.Parameters.Add("qos", "");

                ServiceResponse response = await actionService.AddActionConfiguration(VALID_IP , VALID_USER , VALID_PASS , m_actionConfiguration);

                Console.WriteLine("[AddActionConfigurations] " + m_actionConfiguration.ConfigID);

                Assert.IsTrue(response.IsSuccess && m_actionConfiguration.ConfigID != 0);
            }

            [TestMethod]
            public async Task Remove_ActionConfigurations()
            {
                ServiceResponse response = await actionService.RemoveActionConfiguration(VALID_IP, VALID_USER, VALID_PASS, VALID_ACTIONCONFIG_ID);
                Console.WriteLine(response.Content);
                if (response.SOAPContent != null)
                    Console.WriteLine(response.SOAPContent);
                Assert.IsTrue(response.IsSuccess && Regex.IsMatch((response.SOAPContent != null ? response.SOAPContent.ToString() : ""), "RemoveActionConfigurationResponse"));
            }
        #endregion

        #region Recipients
        [TestMethod]
        public async Task Get_RecipientTemplates()
        {
            GetRecipientTemplatesResponse response = await actionService.GetRecipientTemplatesAsync(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Templates.Count > 0);
        }
        [TestMethod]
        public async Task Get_RecipientConfig()
        {
            GetRecipientConfigurationsResponse response = await actionService.GetRecipientConfigurations(VALID_IP, VALID_USER, VALID_PASS);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && !response.SOAPContent.IsEmpty && response.Configurations != null);
        }

        RecipientConfiguration m_recipientConfig;
        [TestMethod]
        public async Task Add_RecipientConfigWithValidParams_IsSuccessAndReturnsConfigID()
        {
            m_recipientConfig = new RecipientConfiguration()
            {
                TemplateToken = "com.axis.recipient.tcp",
                Name = "TestConfig"
            };

            m_recipientConfig.Parameters.Add("host", VALID_IP);
            m_recipientConfig.Parameters.Add("port", "80");
            m_recipientConfig.Parameters.Add("qos", "");

            ServiceResponse response = await actionService.AddRecipientConfiguration(VALID_IP, VALID_USER, VALID_PASS, m_recipientConfig);

            Console.WriteLine("[AddRecipientConfigWithValidParams] " + m_recipientConfig.ConfigurationID);

            Assert.IsTrue(response.IsSuccess && m_recipientConfig.ConfigurationID != 0);
        }

        [TestMethod]
        public async Task RemoveRecipientConfigWithValidParams_IsSuccessWithRemoveRecipientConfigurationResponse()
        {
            ServiceResponse response = await actionService.RemoveRecipientConfiguration(VALID_IP, VALID_USER, VALID_PASS,1);

            Assert.IsTrue(response.IsSuccess && response.HttpStatusCode == System.Net.HttpStatusCode.OK && Regex.IsMatch(response.SOAPContent.ToString(), "RemoveRecipientConfigurationResponse"));
        }
        #endregion

        #endregion

        #region EventService

        EventService eventService = new EventService();

        [TestMethod]
        public async Task Get_EventInstances() {
            GetEventInstancesResponse response = await eventService.GetEventsInstancesAsync(VALID_IP, VALID_USER, VALID_PASS);

            foreach (EventTrigger e in response.EventInstances)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("IsPropertyState : " + e.isProperty);
                if (e.Params.Count > 0)
                {
                    Console.WriteLine("Parameters :");
                    foreach (EventTriggerParam p in e.Params)
                    {
                        Console.WriteLine("\t" + p.Name);
                        if (p.DefaultValues.Count > 0)
                        {
                            Console.WriteLine("\t" + "Possible values : ");
                            foreach (string s in p.DefaultValues)
                                Console.WriteLine("\t\t" + s);
                        }
                    }
                }
            }

            Assert.IsTrue(response.IsSuccess && response.EventInstances.Count > 0);
        }
        [TestMethod]
        public async Task Get_Scheduled_Events()
        {
            GetScheduledEventsResponse response = await eventService.GetScheduledEventsAsync(VALID_IP, VALID_USER, VALID_PASS);

            Console.WriteLine(response.HttpStatusCode + " - " + response.Content);

            if (response.IsSuccess)
                foreach (ScheduledEvent e in response.ScheduledEvents)
                    Console.WriteLine(e.ToString());


            Assert.IsTrue(response.IsSuccess && response.ScheduledEvents.Count > 0);
        }
        [TestMethod]
        public async Task Edit_ScheduledEvent()
        {
            ScheduledEvent newEvent = new ScheduledEvent()
            {
                Name = "UT_DailySchedule",
                Schedule = new ICalendar(new ScheduleTime(8), new ScheduleTime(23), new ScheduleDay[] { ScheduleDay.MO, ScheduleDay.TU, ScheduleDay.WE })
            };

            ServiceResponse resp = await eventService.Edit_ScheduledEventAsync(VALID_IP, VALID_USER, VALID_PASS, "com.axis.schedules.after_hours", newEvent);

            Assert.IsTrue(resp.IsSuccess);
        }
        [TestMethod]
        public async Task Add_Recurrence_ScheduledEvent()
        {
            ScheduledEvent se = new ScheduledEvent()
            {
                Name = "UnitTestScheduledEvent",
                Schedule = new ICalendar(10, PulseInterval.MINUTELY)
            };

            ServiceResponse response = await eventService.Add_ScheduledEventAsync(VALID_IP, VALID_USER, VALID_PASS,se);

            Console.WriteLine(response.Content);

            Assert.IsTrue(response.IsSuccess);
        }
        [TestMethod]
        public async Task Add_Daily_ScheduledEvent()
        {
            ScheduledEvent se = new ScheduledEvent()
            {
                Name = "UnitTestDailyScheduled",
                Schedule = new ICalendar(new ScheduleTime(8,0) , new ScheduleTime(23,0) , new ScheduleDay[] { ScheduleDay.MO, ScheduleDay.TU , ScheduleDay.SA } )
            };

            Console.WriteLine(se.ToString());

            ServiceResponse response = await eventService.Add_ScheduledEventAsync(VALID_IP, VALID_USER, VALID_PASS, se);

            Console.WriteLine(response.Content);

            Assert.IsTrue(response.IsSuccess);
        }
        [TestMethod]
        public async Task Add_Weekly_ScheduledEvent()
        {
            ICalendar calendar = new ICalendar(ScheduleDay.MO, new ScheduleTime(8, 0), ScheduleDay.FR, new ScheduleTime(23, 0));

            ScheduledEvent se = new ScheduledEvent()
            {
                Name = "UnitTestWeeklyScheduled",
                Schedule = calendar
            };

            Console.WriteLine(se.ToString());

            ServiceResponse response = await eventService.Add_ScheduledEventAsync(VALID_IP, VALID_USER, VALID_PASS, se);

            Console.WriteLine(response.Content);

            Assert.IsTrue(response.IsSuccess);
        }
        [TestMethod]
        public async Task Add_Monthly_ScheduledEvent()
        {
            ICalendar calendar = new ICalendar(10, new ScheduleTime(8, 0), 20, new ScheduleTime(17, 30),new ScheduleMonth[] { ScheduleMonth.DEC , ScheduleMonth.SEP });

            ScheduledEvent se = new ScheduledEvent()
            {
                Name = "UnitTestMonthlyScheduledEvent",
                Schedule = calendar
            };

           
            ServiceResponse response = await eventService.Add_ScheduledEventAsync(VALID_IP, VALID_USER, VALID_PASS, se);

            Console.WriteLine(response.Content);

            Assert.IsTrue(response.IsSuccess);
        }
        [TestMethod]
        public async Task Add_Yearly_ScheduledEvent()
        {
            ICalendar calendar = new ICalendar(ScheduleMonth.JAN, 15, new ScheduleTime(8), ScheduleMonth.JUL, 30, new ScheduleTime(22));

            ScheduledEvent se = new ScheduledEvent()
            {
                Name = "UnitTestYearlyScheduled",
                Schedule = calendar
            };

            Console.WriteLine(se.ToString());

            ServiceResponse response = await eventService.Add_ScheduledEventAsync(VALID_IP, VALID_USER, VALID_PASS, se);

            Console.WriteLine(response.Content);

            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task Change_VirtualInputStateAsync()
        {
            ServiceResponse resp = await eventService.Change_VirtualInputStateAsync(VALID_IP, VALID_USER, VALID_PASS, 4, true);

            Console.WriteLine(resp.Content);

            Assert.IsTrue(resp.IsSuccess);
        }


        #endregion

    }
}
