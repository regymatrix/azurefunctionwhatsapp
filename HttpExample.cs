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
            log.LogInformation("C# HTTP trigger function processed a request.");
            //LeadLoverTeste
            string BaseUrl = "http://llapi.leadlovers.com/webapi/";
            Lead postLead;
            string email = req.Query["email"];
            Uri url = new Uri(BaseUrl + "lead" + "?token=" + "95FDFBCDDD1945F49DDA15E7473C2C8B" + "&email=" + email);
            //leadlover tratamento de dados
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                // content =  response.Content.ReadAsStringAsync().Result;
                var content = await response.Content.ReadAsStreamAsync();

                postLead = await System.Text.Json.JsonSerializer.DeserializeAsync<Lead>(content, new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true });
            }
            //organizar number
            string numero = postLead.phone;
            string codePais = numero.Substring(0,3);
            string ddd = numero.Substring(3,2);
            string numerotelp1 = numero.Substring(5,5);
            string numerotelp2 = numero.Substring(10,4);
            postLead.phone=codePais+" ("+ddd+") "+numerotelp1+"-"+numerotelp2;


            //enviando para zapfacil
             string urlpost = "https://api.painel.zapfacil.com/api/webhooks/2URFOtV6w6Ww00A5NqwTeNABfxvJvR4X";
            using (HttpClient clientpost = new HttpClient())
            {

                var postResponse = await clientpost.PostAsJsonAsync(urlpost, postLead);
            }


            //funcaopadrao
            // string name = req.Query["name"];
            // string idade= req.Query["idade"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"ok";

            //return new OkObjectResult(responseMessage);
             return postLead;
        }
    }
}
