using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
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
            const string username = "";
            const string password = "";

            var cookieJar = new CookieContainer();
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                UseCookies = true,
                UseDefaultCredentials = false,
                //UseProxy = true,
                CookieContainer = cookieJar,
                AllowAutoRedirect = true
            };
            Client = new HttpClient(handler);

            // Fake headers ;)
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/49.0");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            //Client.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://cap.mcmaster.ca");
            
            // 1. Login
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://cap.mcmaster.ca/mcauth/mcauth;jsessionid="),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Referer", "https://cap.mcmaster.ca" },
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"user_id", username},
                    {"pin", password},
                    {"app_id", "1505"},
                    {"SAMLRequest", ""},
                    {"submit", ""},
                    {"user_type", "ad"}
                })
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);
            }

            await Task.Delay(5000);

            // 2. Go to Import/Export page and save d2l_hitCode and d2l_referrer
            string hitCode;
            string referrer;
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importExport/import_export.d2l?ou=192371"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "Referer", "https://cap.mcmaster.ca" },
                },
                Properties = { { "ou", "192371" } },
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);

                var html = await response.Content.ReadAsStringAsync();
                hitCode = GetHitCode(html);
                referrer = GetReferrer(html);
            }

            await Task.Delay(5000);

            // 3. Select Components
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importexport/export/export_select_components.d2l?ou=192371"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Referer", "https://avenue.cllmcmaster.ca/d2l/lms/importexport/import_export.d2l?ou=192371" },
                    { "Origin", "https://avenue.cllmcmaster.ca" },
                    { "Upgrade-Insecure-Requests", "1" }
                },
                Properties = { { "ou", "192371" } },
                Content = new MultipartFormDataContent("----WebKitFormBoundaryYswOsXskiEHjHh3u")
                {
                    { FromString(""), "d2l_action" },
                    { FromString(""), "d2l_actionparam" },
                    { FromString(""), "d2l_hitCode" },
                    { FromString(""), "d2l_rf" },
                    { FromString(@"{'ID':{btn_start:'z_a',btn_copyall:'z_b',btn_selectcomponents:'z_c',btn_copyall_parent:'z_d',btn_selectcomponents_parent
:'z_e',hdg_todoquestion:'z_f',hd_courseid:'z_g',rad_copy:'copyComponentsId',ctl_7:'z_h',btn_selectcourse
:'z_i',btn_selecttemplate:'z_j',btn_selectchildoffering:'z_k',ctl_12:'z_l',cnt_overwritecomponents:'z_m'
,chk_overwritecomponents:'z_n',lnk_copyhistory:'z_o',rad_copyparent:'copyParentTemplateComponentsId'
,ctl_13:'z_p',ctl_15:'z_q',rad_export:'exportComponentsId',ctl_16:'z_r',ctl_18:'z_s',rad_import:'importComponentsId'
,hd_parentorgunitid:'z_t',ctl_messagearea:'z_u'},'SID':{}}"), "d2l_controlMapPrev" },
                    { FromString(""), "selectedCourseId" },
                    { FromString("1"), "componentsMethod" },
                    { FromString("46665"), "parentOrgUnitId" },
                    { FromString(@"[{'btn_start':['z_a','Button',['Next(192371);;return false;'],{},0,0],'btn_copyall':['z_b','Button',
['StartCopyAllJob( 192371, UI.GetControl( \u0027HD_courseId\u0027 ).GetValue() );;return false;'],{}
,0,0],'btn_selectcomponents':['z_c','Button',['SelectCourse( UI.GetControl( \u0027HD_courseId\u0027 
).GetValue() );;return false;'],{},0,0],'btn_copyall_parent':['z_d','Button',['StartCopyAllJob( 192371
, UI.GetControl( \u0027HD_parentOrgUnitId\u0027 ).GetValue() );;return false;'],{},0,0],'btn_selectcomponents_parent'
:['z_e','Button',['SelectCourse( UI.GetControl( \u0027HD_parentOrgUnitId\u0027 ).GetValue() );;return
 false;'],{},0,0],'hdg_todoquestion':['z_f','Heading',[0,0,1],{},0,1],'hd_courseid':['z_g','Hidden',
[1],{},0,1],'rad_copy':['copyComponentsId','RadioButton',[1],{},0,1],'ctl_7':['z_h','Field',[],,1,1]
,'btn_selectcourse':['z_i','Button',['OpenSearchWindow(192371, 1);return false;'],{},0,0],'btn_selecttemplate'
:['z_j','Button',['OpenSearchWindow(192371, 2);return false;'],{},0,0],'btn_selectchildoffering':['z_k'
,'Button',['OpenSearchWindow(192371, 3);return false;'],{},0,0],'ctl_12':['z_l','Checkbox',[1,1,1],{
},0,1],'cnt_overwritecomponents':['z_m','Container',[0,0,0,0,1,1,[1,'#999999',1],0,0,0,0,''],{},0,1]
,'chk_overwritecomponents':['z_n','Checkbox',[1,1,1],{},0,1],'lnk_copyhistory':['z_o','Link',['\/d2l
\/le\/conversion\/copy\/192371\/History?ou=192371',0],{},0,1],'rad_copyparent':['copyParentTemplateComponentsId'
,'RadioButton',[1],{},0,1],'ctl_13':['z_p','Field',[],,1,1],'ctl_15':['z_q','Checkbox',[1,1,1],{},0,1
],'rad_export':['exportComponentsId','RadioButton',[1],{},0,1],'ctl_16':['z_r','Field',[],,1,1],'ctl_18'
:['z_s','Checkbox',[1,1,1],{},0,1],'rad_import':['importComponentsId','RadioButton',[1],{},0,1],'hd_parentorgunitid'
:['z_t','Hidden',[1],{},0,1],'ctl_messagearea':['z_u','MessageArea',['d_content_inner','d_page_header'
,0,[],null,null],{},0,0]},{}]"), "d2l_controlMap" },
                    { FromString("[{'3':['grid','pagesize','htmleditor','hpg'],'1':['gridpagenum','search','pagenum'],'2':['lcs']},[]]"), "d2l_state" },
                    { FromString(referrer), "d2l_referrer" },
                    { FromString("{'Controls':[]}"), "d2l_multiedit" },
                    { FromString("{1:['gridpagenum','search','pagenum'],2:['lcs'],3:['grid','pagesize','htmleditor','hpg']}"), "d2l_stateScopes" },
                    { FromString(""), "d2l_stateGroups" },
                    { FromString("50"), "d2l_statePageId" }
                }
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);
                
                await CreateHtml(response, "D:/test.html");
            }

            /*
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importExport/export/export_process_iframe.d2l?ou=192371"),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"exportFiles", "False"},
                    {"wizTools", "true,true,true,true,false,true,false,false,false,false,false,false,true,true,false,false,false,true,false,true"},
                    {"wizToolMethods", "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,"},
                    {"wizToolNumbers", "48,33,12,48,0,23,0,0,0,0,0,0,10,81,0,0,0,1,0,1"},
                })
            })
            using (var response = await Client.SendAsync(request))
            {
                Assert.IsTrue(response.IsSuccessStatusCode);

                var html = await response.Content.ReadAsStringAsync();

                File.WriteAllText("D:/test.html", html);

                //Process.Start("D:/test.html");
            }
            */

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

        private static void CheckResponse(HttpResponseMessage response)
        {
            Assert.IsTrue(response.IsSuccessStatusCode, $@"
Error: {response.ReasonPhrase}
Status code: {response.StatusCode}
");
        }

        private static async Task CreateHtml(HttpResponseMessage response, string path)
        {
            var html = await response.Content.ReadAsStringAsync();

            File.WriteAllText(path, html);

            //Process.Start("D:/test.html");
        }

        private static ByteArrayContent FromString(string data)
        {
            return new ByteArrayContent(Encoding.UTF8.GetBytes(data));
            //return new StringContent(data);
        }

        private static string GetHitCode(string html)
        {
            const string text = "<input type=\"hidden\" name=\"d2l_hitCode\" value=\"";
            var index1 = html.IndexOf(text, StringComparison.OrdinalIgnoreCase) + text.Length;
            var index2 = html.IndexOf("\" />", index1, StringComparison.OrdinalIgnoreCase);

            return html.Substring(index1, index2 - index1);
        }

        private static string GetReferrer(string html)
        {
            const int length = 32;
            const string text = "<input type=\"hidden\" name=\"d2l_referrer\" value=\"";
            var index = html.IndexOf(text, StringComparison.OrdinalIgnoreCase) + text.Length;

            return html.Substring(index, length);
        }
    }
}
