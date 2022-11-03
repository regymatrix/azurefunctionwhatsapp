using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace Company.Function
{
    public static class HttpExample
    {
        [FunctionName("HttpExample")]
        public static async Task<Lead> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Information is arrived.");
            //LeadLoverTeste
            string BaseUrl = "http://llapi.leadlovers.com/webapi/";
            Lead postLead;
            string email = req.Query["email"];
            Uri url = new Uri(BaseUrl + "lead" + "?token=" + "key" + "&email=" + email);
            //leadlover tratamento de dados
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response =  await client.GetAsync(url);
                //HttpResponseMessage response =  client.GetAsync(url).Result;
                // content =  response.Content.ReadAsStringAsync().Result;
                var content = await response.Content.ReadAsStreamAsync();

                postLead = await System.Text.Json.JsonSerializer.DeserializeAsync<Lead>(content, new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
            }
            //organizar number
            postLead.phone = NumberOrganization(postLead.phone);
           

            //enviando para zapfacil
             string urlpost = "https://api.painel.zapfacil.com/api/webhooks/2URFOtV6w6Ww00A5NqwTeNABfxvJvR4X";
            using (HttpClient clientpost = new HttpClient())
            {

                var postResponse = await clientpost.PostAsJsonAsync(urlpost, postLead);
            }

             return postLead;
        }

        private static string NumberOrganization(string number){
           
            string codePais = number.Substring(0,3);
            string ddd = number.Substring(3,2);
            string numerotelp1 = number.Substring(5,5);
            string numerotelp2 = number.Substring(10,4);            

            return codePais+" ("+ddd+") "+numerotelp1+"-"+numerotelp2;
        }
    }
}
