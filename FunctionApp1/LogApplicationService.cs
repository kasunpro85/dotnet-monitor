
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class LogApplicationService
    {
        public void SendLog()
        {
            SetAppSettings();
            PostList();            
        }

        private void SetAppSettings()
        {
            AppSettings.dbConnectionString = Environment.GetEnvironmentVariable("dbConnection");
           
        }

        private bool PostList()
        {
            Task<MonitoVal> monitoVal = Monitor();


            IDataService dataService = DataServiceBuilder.CreateDataService(AppSettings.dbConnectionString);
            try
            {
                dataService.BeginTransaction();
                DataService ds =  new DataService(dataService);
               bool result = ds.PostList(monitoVal);


                dataService.CommitTransaction();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally { dataService.Dispose(); }
        }

        static async Task<MonitoVal> Monitor()
        {
            double graseIntevel = Convert.ToDouble(Environment.GetEnvironmentVariable("graseIntevel"));

            string register = "", polling = "";
            string backendURL = Environment.GetEnvironmentVariable("BackendURL");
            string registerUrl = Environment.GetEnvironmentVariable("register");
            string pollingUrl = Environment.GetEnvironmentVariable("polling");
            HttpContent body = new StringContent("");
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(backendURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(60);
                DateTime date = DateTime.Now; double dDate = date.ToOADate();
               
                HttpResponseMessage httpResponse = await client.PostAsync(registerUrl, body);

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    httpResponse.EnsureSuccessStatusCode();
                    string val = await httpResponse.Content.ReadAsStringAsync();
                    RegisterVal registerVal = JsonConvert.DeserializeObject<RegisterVal>(val);
                    DateTime dateTime = date.AddSeconds(graseIntevel);                    
                    if (registerVal.graceTime <= dateTime.ToOADate())
                    {
                        registerVal.isActive = true;
                    }
                    else { registerVal.isActive = false; }

                    Function1.listRegisterVal.Add(registerVal);
                  

                    register = ($"http Content:" + val);
                }
                else { register = "false";
                    Function1.listRegisterVal.Add(new RegisterVal {isActive = false });
                }
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(backendURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMinutes(60);

                HttpResponseMessage httpResponse = await client.PostAsync(pollingUrl, body);

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    httpResponse.EnsureSuccessStatusCode();
                    string val = await httpResponse.Content.ReadAsStringAsync();
                    Function1.listpollingVal.Add(JsonConvert.DeserializeObject<PollingVal>(val));
                    polling = ($"http Content:" + val);
                }
                else { register = "false"; Function1.listpollingVal.Add(new PollingVal { isActive = false }); }
            }
            return new MonitoVal { register = register, polling = polling };
        }
    }

    public class MonitoVal
    {
        public string register { get; set; }
        public string polling { get; set; }
    }
    public class RegisterVal
    {

        public int monitors { get; set; }
        public double graceTime { get; set; }
        public int pollingTime { get; set; }
        public bool isActive { get; set; }

    }

    public class PollingVal
    {
        public int reqId { get; set; }         
        public double timestamp { get; set; }
        public bool isActive { get; set; }

    }
}
