using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable StringLiteralTypo

namespace MainLibrary.Tests
{
    [TestClass]
    public class MainTests
    {
        private static HttpClient Client { get; set; }

        [TestMethod]
        public async Task MainTest()
        {
            var cookieJar = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                UseCookies = true,
                UseDefaultCredentials = false,
                UseProxy = true,
                CookieContainer = cookieJar,
                AllowAutoRedirect = true
            };
            Client = new HttpClient(handler);

            // Fake headers ;)
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/49.0");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://cap.mcmaster.ca");

            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://cap.mcmaster.ca/mcauth/mcauth;jsessionid="),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"app_id", "1505"},
                    {"SAMLRequest", ""},
                    {"submit", ""},
                    {"user_type", "ad"}
                })
            })
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                //var html = await response.Content.ReadAsStringAsync();

                //File.WriteAllText("D:/test.html", html);
            }


            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importExport/import_export.d2l?ou=192371"),
                Method = HttpMethod.Get
            })
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                //var html = await response.Content.ReadAsStringAsync();

                //File.WriteAllText("D:/test.html", html);
            }

            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importExport/import_export.d2l?ou=192371"),
                Method = HttpMethod.Get
            })
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                //var html = await response.Content.ReadAsStringAsync();

                //File.WriteAllText("D:/test.html", html);
            }

            //------------------
            // Upwork work below
            //------------------

            //STEP ONE
            //Export Components (https://avenue.cllmcmaster.ca/d2l/lms/importexport/import_export.d2l?ou=192371)
            //Include course files in the export package = false
            //Screen-recording of actions + Fiddler: https://www.useloom.com/share/d32e6f801baa484c9aef7c5839893525
            //Scrape HTML contents from next page

            //STEP TWO
            //Select All Components = true
            //Screen-recording of actions + Fiddler: https://www.useloom.com/share/63f0f92920424f6a93ff54ad883ac662
            //Scrape HTML contents from next page

            //STEP THREE
            //Confirm
            //Include course files in the export package = false
            //Screen-recording of actions + Fiddler: https://www.useloom.com/share/4253936dfa2e4066a26cd7bde5f2db4f
            //Scrape page after export is completed

            //STEP FOUR
            //Finish
            //Screen-recording of actions + Fiddler: https://www.useloom.com/share/f2d365c25d8f4c739b2eff6f83028119

            //STEP FIVE
            //Download export zip
            //This will be a .Net web application so the file cannot download to local computer.
            //If it can be done in memory that would be ideal.
            //Alternatively it can be downloaded to AWS S3 and then deleted after Step Six
            //Screen-recording of actions + Fiddler: https://www.useloom.com/share/c2fa9d239d25481f9afebf6ff48225a1

            //STEP SIX
            //Extract zip and read contents of each xml file into separate strings

            //-----------------
            //Upwork work above
            //-----------------
        }
    }
}
