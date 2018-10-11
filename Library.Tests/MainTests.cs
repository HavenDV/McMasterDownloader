using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO.Compression;
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
            const string username = "ccemm1";
            const string password = "L$v8lm2o";
            const int delayMilliseconds = 0; // I recommend use delay > 0

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

            // I found out that this is not necessary. But it will look right for the server.
            /*
            await Wait(delayMilliseconds);

            // 2. Go to Import/Export page and save d2l_hitCode and d2l_referrer
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
                referrer = FindReferrer(html);
            }

            await Wait(delayMilliseconds);
            
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
                    { new StringContent(""), "d2l_action" },
                    { new StringContent(""), "d2l_actionparam" },
                    { new StringContent(""), "d2l_hitCode" },
                    { new StringContent(""), "d2l_rf" },
                    { new StringContent(@"{'ID':{btn_start:'z_a',btn_copyall:'z_b',btn_selectcomponents:'z_c',btn_copyall_parent:'z_d',btn_selectcomponents_parent
:'z_e',hdg_todoquestion:'z_f',hd_courseid:'z_g',rad_copy:'copyComponentsId',ctl_7:'z_h',btn_selectcourse
:'z_i',btn_selecttemplate:'z_j',btn_selectchildoffering:'z_k',ctl_12:'z_l',cnt_overwritecomponents:'z_m'
,chk_overwritecomponents:'z_n',lnk_copyhistory:'z_o',rad_copyparent:'copyParentTemplateComponentsId'
,ctl_13:'z_p',ctl_15:'z_q',rad_export:'exportComponentsId',ctl_16:'z_r',ctl_18:'z_s',rad_import:'importComponentsId'
,hd_parentorgunitid:'z_t',ctl_messagearea:'z_u'},'SID':{}}"), "d2l_controlMapPrev" },
                    { new StringContent(""), "selectedCourseId" },
                    { new StringContent("1"), "componentsMethod" },
                    { new StringContent("46665"), "parentOrgUnitId" },
                    { new StringContent(@"[{'btn_start':['z_a','Button',['Next(192371);;return false;'],{},0,0],'btn_copyall':['z_b','Button',
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
                    { new StringContent("[{'3':['grid','pagesize','htmleditor','hpg'],'1':['gridpagenum','search','pagenum'],'2':['lcs']},[]]"), "d2l_state" },
                    { new StringContent(referrer), "d2l_referrer" },
                    { new StringContent("{'Controls':[]}"), "d2l_multiedit" },
                    { new StringContent("{1:['gridpagenum','search','pagenum'],2:['lcs'],3:['grid','pagesize','htmleditor','hpg']}"), "d2l_stateScopes" },
                    { new StringContent(""), "d2l_stateGroups" },
                    { new StringContent("50"), "d2l_statePageId" }
                }
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);
            }

            await Wait(delayMilliseconds);

            // 4. Select Course Material
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importexport/export/export_select_confirm.d2l?ou=192371"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Referer", "https://avenue.cllmcmaster.ca/d2l/lms/importexport/export/export_select_components.d2l?ou=192371" },
                    { "Origin", "https://avenue.cllmcmaster.ca" },
                    { "Upgrade-Insecure-Requests", "1" }
                },
                Properties = { { "ou", "192371" } },
                Content = new MultipartFormDataContent("----WebKitFormBoundaryYswOsXskiEHjHh3u")
                {
                    { new StringContent(@""), "d2l_action" },
                    { new StringContent(@""), "d2l_actionparam" },
                    { new StringContent(@"1127947982593662402"), "d2l_hitCode" },
                    { new StringContent(@""), "d2l_rf" },
                    { new StringContent(@"{'ID':{ctl_2:'z_a',ctl_4:'z_b',ctl_6:'z_c',cb_selectall:'z_d',cb_includeschedule:'z_e',lbl_numschedule
:'z_f',rb_allschedule:'z_g',rb_selectschedule:'z_h',cb_includecontent:'z_i',lbl_numcontent:'z_j',rb_allcontent
:'z_k',rb_selectcontent:'z_l',cb_includediscuss:'z_m',lbl_numdiscuss:'z_n',rb_alldiscuss:'z_o',rb_selectdiscuss
:'z_p',cb_includedropbox:'z_q',lbl_numdropbox:'z_r',rb_alldropbox:'z_s',rb_selectdropbox:'z_t',cb_includegrades
:'z_u',lbl_numgrades:'z_v',rb_allgrades:'z_w',rb_selectgrades:'z_x',cb_includequestionlibrary:'z_y',lbl_numquestionlibrary
:'z_z',rb_allquestionlibrary:'z_ba',rb_selectquestionlibrary:'z_bb',cb_includequizzes:'z_bc',lbl_numquizzes
:'z_bd',rb_allquizzes:'z_be',rb_selectquizzes:'z_bf',cb_includerubrics:'z_bg',lbl_numrubrics:'z_bh',rb_allrubrics
:'z_bi',rb_selectrubrics:'z_bj',cb_includecourseappearance:'z_bk',rb_allcourseappearance:'z_bl',rb_selectcourseappearance
:'z_bm',hidden_tempfolderpath:'z_bn',hidden_importlocation:'z_bo',hidden_overwritefiles:'z_bp',hidden_importmetadata
:'z_bq',hidden_dateoffset:'z_br',hidden_exportformat:'z_bs',hidden_exportprotectedresources:'z_bt',hidden_copyprotectedresources
:'z_bu',hidden_overwritecomponents:'z_bv',hidden_copycourseid:'wizCopyCourseId',hidden_copyfrom:'z_bw'
,hidden_tools:'z_bx',hidden_toolmethods:'z_by',hidden_toolnumbers:'z_bz',hidden_selectedchat:'z_ca',hidden_selectedchecklists
:'z_cb',hidden_selectedawards:'z_cc',hidden_selectedcontent:'z_cd',hidden_selecteddiscuss:'z_ce',hidden_selecteddropbox
:'z_cf',hidden_selectedfaq:'z_cg',hidden_selectedfiles:'z_ch',hidden_selectedforms:'z_ci',hidden_selectedglossary
:'z_cj',hidden_selectedgrades:'z_ck',hidden_selectedgradessettings:'z_cl',hidden_selectedgroups:'z_cm'
,hidden_selectedhomepages:'z_cn',hidden_selectedlinks:'z_co',hidden_selectednavbars:'z_cp',hidden_selectednavigation
:'z_cq',hidden_selectednews:'z_cr',hidden_selectedquestionlibs:'z_cs',hidden_selectedquizzes:'z_ct',hidden_selectedschedule
:'z_cu',hidden_selectedselfassess:'z_cv',hidden_selectedsurveys:'z_cw',hidden_selectedwidgets:'z_cx'
,hidden_selectedcompetencies:'z_cy',hidden_selectedrubrics:'z_cz',hidden_selectedtoolnames:'z_da',hidden_selectedreleaseconditions
:'z_db',hidden_selectedintelligentagents:'z_dc',hidden_selectedltilinks:'z_dd',hidden_selectedltitps
:'z_de',hidden_selectedattendanceregisters:'z_df',hidden_includeassociatedfiles:'z_dg',hidden_associatedfileschecked
:'z_dh',hidden_selecteds3model:'z_di',hidden_selectedcourseappearanceids:'z_dj',hidden_selectedlearningoutcomes
:'z_dk',hidden_jobid:'z_dl',hidden_filename:'z_dm',ctl_messagearea:'z_dn'},'SID':{}}"), "d2l_controlMapPrev" },
                    { new StringContent(@"True"), "checkAll" },
                    { new StringContent(@"True"), "includeSchedule" },
                    { new StringContent(@"1"), "scheduleOpt" },
                    { new StringContent(@"True"), "includeContent" },
                    { new StringContent(@"1"), "contentOpt" },
                    { new StringContent(@"True"), "includeDiscuss" },
                    { new StringContent(@"1"), "discussOpt" },
                    { new StringContent(@"True"), "includeDropbox" },
                    { new StringContent(@"1"), "dropboxOpt" },
                    { new StringContent(@"True"), "includeGrades" },
                    { new StringContent(@"1"), "gradesOpt" },
                    { new StringContent(@"True"), "includeQuestionLibrary" },
                    { new StringContent(@"1"), "questionLibraryOpt" },
                    { new StringContent(@"True"), "includeQuizzes" },
                    { new StringContent(@"1"), "quizzesOpt" },
                    { new StringContent(@"True"), "includeRubrics" },
                    { new StringContent(@"1"), "rubricsOpt" },
                    { new StringContent(@"True"), "includeCourseAppearance" },
                    { new StringContent(@"1"), "courseAppearanceOpt" },
                    { new StringContent(@""), "wizTempFolderPath" },
                    { new StringContent(@""), "wizImportLocation" },
                    { new StringContent(@"False"), "wizOverwriteFiles" },
                    { new StringContent(@""), "wizImportMetadata" },
                    { new StringContent(@""), "wizDateOffset" },
                    { new StringContent(@""), "wizExportFormat" },
                    { new StringContent(@""), "wizExportProtectedResources" },
                    { new StringContent(@""), "wizCopyProtectedResources" },
                    { new StringContent(@""), "wizOverwriteComponents" },
                    { new StringContent(@""), "wizCopyCourseId" },
                    { new StringContent(@""), "wizCopyFrom" },
                    { new StringContent(@"true,true,true,true,false,true,false,false,false,false,false,false,true,true,false,false,false,true,false
,true"), "wizTools" },
                    { new StringContent(@"1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,"), "wizToolMethods" },
                    { new StringContent(@"48,33,12,48,0,23,0,0,0,0,0,0,10,81,0,0,0,1,0,1"), "wizToolNumbers" },
                    { new StringContent(@""), "wizSelectedChat" },
                    { new StringContent(@""), "wizSelectedChecklists" },
                    { new StringContent(@""), "wizSelectedAwards" },
                    { new StringContent(@""), "wizSelectedContent" },
                    { new StringContent(@""), "wizSelectedDiscuss" },
                    { new StringContent(@""), "wizSelectedDropbox" },
                    { new StringContent(@""), "wizSelectedFaq" },
                    { new StringContent(@""), "wizSelectedFiles" },
                    { new StringContent(@""), "wizSelectedForms" },
                    { new StringContent(@""), "wizSelectedGlossary" },
                    { new StringContent(@""), "wizSelectedGrades" },
                    { new StringContent(@""), "wizSelectedGradesSettings" },
                    { new StringContent(@""), "wizSelectedGroups" },
                    { new StringContent(@""), "wizSelectedHomepages" },
                    { new StringContent(@""), "wizSelectedLinks" },
                    { new StringContent(@""), "wizSelectedNavbars" },
                    { new StringContent(@""), "wizSelectedNavigation" },
                    { new StringContent(@""), "wizSelectedNews" },
                    { new StringContent(@""), "wizSelectedQuestionLibs" },
                    { new StringContent(@""), "wizSelectedQuizzes" },
                    { new StringContent(@""), "wizSelectedSchedule" },
                    { new StringContent(@""), "wizSelectedSelfAssess" },
                    { new StringContent(@""), "wizSelectedSurveys" },
                    { new StringContent(@""), "wizSelectedWidgets" },
                    { new StringContent(@""), "wizSelectedCompetencies" },
                    { new StringContent(@""), "wizSelectedRubrics" },
                    { new StringContent(@""), "wizSelectedToolNames" },
                    { new StringContent(@""), "wizSelectedReleaseConditions" },
                    { new StringContent(@""), "wizSelectedIntelligentAgents" },
                    { new StringContent(@""), "wizSelectedLtiLinks" },
                    { new StringContent(@""), "wizSelectedLtiTPs" },
                    { new StringContent(@""), "wizSelectedAttendanceRegisters" },
                    { new StringContent(@""), "wizIncludeAssociatedFiles" },
                    { new StringContent(@""), "wizAssociatedFilesChecked" },
                    { new StringContent(@""), "wizSelectedS3Model" },
                    { new StringContent(@""), "wizSelectedCourseAppearanceIds" },
                    { new StringContent(@""), "wizSelectedLearningOutcomes" },
                    { new StringContent(@""), "wizJobId" },
                    { new StringContent(@""), "wizFileName" },
                    { new StringContent(@"[{'ctl_2':['z_a','Button',['Next(192371);;return false;'],{},0,0],'ctl_4':['z_b','Button',['var n=new
 D2L.NavInfo();n.navigation=\u0027\\\/d2l\\\/lms\\\/importexport\\\/import_export.d2l\u0027;Nav.Go(n
, false, false);return false;'],{},0,0],'ctl_6':['z_c','Button',['var n=new D2L.NavInfo();n.navigation
=\u0027\\\/d2l\\\/lms\\\/importexport\\\/import_export.d2l\u0027;Nav.Go(n, false, false);return false
;'],{},0,0],'cb_selectall':['z_d','Checkbox',[1,1,1],{},0,1],'cb_includeschedule':['z_e','Checkbox',
[1,1,1],{},0,1],'lbl_numschedule':['z_f','Label',[],{},0,1],'rb_allschedule':['z_g','RadioButton',[1
],{},0,1],'rb_selectschedule':['z_h','RadioButton',[1],{},0,1],'cb_includecontent':['z_i','Checkbox'
,[1,1,1],{},0,1],'lbl_numcontent':['z_j','Label',[],{},0,1],'rb_allcontent':['z_k','RadioButton',[1]
,{},0,1],'rb_selectcontent':['z_l','RadioButton',[1],{},0,1],'cb_includediscuss':['z_m','Checkbox',[1
,1,1],{},0,1],'lbl_numdiscuss':['z_n','Label',[],{},0,1],'rb_alldiscuss':['z_o','RadioButton',[1],{}
,0,1],'rb_selectdiscuss':['z_p','RadioButton',[1],{},0,1],'cb_includedropbox':['z_q','Checkbox',[1,1
,1],{},0,1],'lbl_numdropbox':['z_r','Label',[],{},0,1],'rb_alldropbox':['z_s','RadioButton',[1],{},0
,1],'rb_selectdropbox':['z_t','RadioButton',[1],{},0,1],'cb_includegrades':['z_u','Checkbox',[1,1,1]
,{},0,1],'lbl_numgrades':['z_v','Label',[],{},0,1],'rb_allgrades':['z_w','RadioButton',[1],{},0,1],'rb_selectgrades'
:['z_x','RadioButton',[1],{},0,1],'cb_includequestionlibrary':['z_y','Checkbox',[1,1,1],{},0,1],'lbl_numquestionlibrary'
:['z_z','Label',[],{},0,1],'rb_allquestionlibrary':['z_ba','RadioButton',[1],{},0,1],'rb_selectquestionlibrary'
:['z_bb','RadioButton',[1],{},0,1],'cb_includequizzes':['z_bc','Checkbox',[1,1,1],{},0,1],'lbl_numquizzes'
:['z_bd','Label',[],{},0,1],'rb_allquizzes':['z_be','RadioButton',[1],{},0,1],'rb_selectquizzes':['z_bf'
,'RadioButton',[1],{},0,1],'cb_includerubrics':['z_bg','Checkbox',[1,1,1],{},0,1],'lbl_numrubrics':['z_bh'
,'Label',[],{},0,1],'rb_allrubrics':['z_bi','RadioButton',[1],{},0,1],'rb_selectrubrics':['z_bj','RadioButton'
,[1],{},0,1],'cb_includecourseappearance':['z_bk','Checkbox',[1,1,1],{},0,1],'rb_allcourseappearance'
:['z_bl','RadioButton',[1],{},0,1],'rb_selectcourseappearance':['z_bm','RadioButton',[1],{},0,1],'hidden_tempfolderpath'
:['z_bn','Hidden',[1],{},0,1],'hidden_importlocation':['z_bo','Hidden',[1],{},0,1],'hidden_overwritefiles'
:['z_bp','Hidden',[1],{},0,1],'hidden_importmetadata':['z_bq','Hidden',[1],{},0,1],'hidden_dateoffset'
:['z_br','Hidden',[1],{},0,1],'hidden_exportformat':['z_bs','Hidden',[1],{},0,1],'hidden_exportprotectedresources'
:['z_bt','Hidden',[1],{},0,1],'hidden_copyprotectedresources':['z_bu','Hidden',[1],{},0,1],'hidden_overwritecomponents'
:['z_bv','Hidden',[1],{},0,1],'hidden_copycourseid':['wizCopyCourseId','Hidden',[1],{},0,1],'hidden_copyfrom'
:['z_bw','Hidden',[1],{},0,1],'hidden_tools':['z_bx','Hidden',[1],{},0,1],'hidden_toolmethods':['z_by'
,'Hidden',[1],{},0,1],'hidden_toolnumbers':['z_bz','Hidden',[1],{},0,1],'hidden_selectedchat':['z_ca'
,'Hidden',[1],{},0,1],'hidden_selectedchecklists':['z_cb','Hidden',[1],{},0,1],'hidden_selectedawards'
:['z_cc','Hidden',[1],{},0,1],'hidden_selectedcontent':['z_cd','Hidden',[1],{},0,1],'hidden_selecteddiscuss'
:['z_ce','Hidden',[1],{},0,1],'hidden_selecteddropbox':['z_cf','Hidden',[1],{},0,1],'hidden_selectedfaq'
:['z_cg','Hidden',[1],{},0,1],'hidden_selectedfiles':['z_ch','Hidden',[1],{},0,1],'hidden_selectedforms'
:['z_ci','Hidden',[1],{},0,1],'hidden_selectedglossary':['z_cj','Hidden',[1],{},0,1],'hidden_selectedgrades'
:['z_ck','Hidden',[1],{},0,1],'hidden_selectedgradessettings':['z_cl','Hidden',[1],{},0,1],'hidden_selectedgroups'
:['z_cm','Hidden',[1],{},0,1],'hidden_selectedhomepages':['z_cn','Hidden',[1],{},0,1],'hidden_selectedlinks'
:['z_co','Hidden',[1],{},0,1],'hidden_selectednavbars':['z_cp','Hidden',[1],{},0,1],'hidden_selectednavigation'
:['z_cq','Hidden',[1],{},0,1],'hidden_selectednews':['z_cr','Hidden',[1],{},0,1],'hidden_selectedquestionlibs'
:['z_cs','Hidden',[1],{},0,1],'hidden_selectedquizzes':['z_ct','Hidden',[1],{},0,1],'hidden_selectedschedule'
:['z_cu','Hidden',[1],{},0,1],'hidden_selectedselfassess':['z_cv','Hidden',[1],{},0,1],'hidden_selectedsurveys'
:['z_cw','Hidden',[1],{},0,1],'hidden_selectedwidgets':['z_cx','Hidden',[1],{},0,1],'hidden_selectedcompetencies'
:['z_cy','Hidden',[1],{},0,1],'hidden_selectedrubrics':['z_cz','Hidden',[1],{},0,1],'hidden_selectedtoolnames'
:['z_da','Hidden',[1],{},0,1],'hidden_selectedreleaseconditions':['z_db','Hidden',[1],{},0,1],'hidden_selectedintelligentagents'
:['z_dc','Hidden',[1],{},0,1],'hidden_selectedltilinks':['z_dd','Hidden',[1],{},0,1],'hidden_selectedltitps'
:['z_de','Hidden',[1],{},0,1],'hidden_selectedattendanceregisters':['z_df','Hidden',[1],{},0,1],'hidden_includeassociatedfiles'
:['z_dg','Hidden',[1],{},0,1],'hidden_associatedfileschecked':['z_dh','Hidden',[1],{},0,1],'hidden_selecteds3model'
:['z_di','Hidden',[1],{},0,1],'hidden_selectedcourseappearanceids':['z_dj','Hidden',[1],{},0,1],'hidden_selectedlearningoutcomes'
:['z_dk','Hidden',[1],{},0,1],'hidden_jobid':['z_dl','Hidden',[1],{},0,1],'hidden_filename':['z_dm','Hidden'
,[1],{},0,1],'ctl_messagearea':['z_dn','MessageArea',['d_content_inner','d_page_header',0,[],null,null
],{},0,0]},{}]"), "d2l_controlMap" },
                    { new StringContent(@"[{'3':['grid','pagesize','htmleditor','hpg'],'1':['gridpagenum','search','pagenum'],'2':['lcs']},[]]"), "d2l_state" },
                    { new StringContent(referrer), "d2l_referrer" },
                    { new StringContent(@"{'Controls':[]}"), "d2l_multiedit" },
                    { new StringContent(@"{1:['gridpagenum','search','pagenum'],2:['lcs'],3:['grid','pagesize','htmleditor','hpg']}"), "d2l_stateScopes" },
                    { new StringContent(@""), "d2l_stateGroups" },
                    { new StringContent(@"325"), "d2l_statePageId" }
                }
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);
            }

            await Wait(delayMilliseconds);

            // 5. Confirm
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importexport/export/export_process.d2l?ou=192371"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Referer", "https://avenue.cllmcmaster.ca/d2l/lms/importexport/export/export_select_confirm.d2l?ou=192371" },
                    { "Origin", "https://avenue.cllmcmaster.ca" },
                    { "Upgrade-Insecure-Requests", "1" }
                },
                Properties = { { "ou", "192371" } },
                Content = new MultipartFormDataContent("----WebKitFormBoundaryYswOsXskiEHjHh3u")
                {
                    { new StringContent(@""), "d2l_action" },
                    { new StringContent(@""), "d2l_actionparam" },
                    { new StringContent(@"1127947982593756752"), "d2l_hitCode" },
                    { new StringContent(@""), "d2l_rf" },
                    { new StringContent(@"{'ID':{ctl_2:'z_a',ctl_4:'z_b',ctl_6:'z_c',hidden_tempfolderpath:'z_d',hidden_importlocation:'z_e',hidden_overwritefiles
:'z_f',hidden_importmetadata:'z_g',hidden_dateoffset:'z_h',hidden_exportformat:'z_i',hidden_exportprotectedresources
:'z_j',hidden_copyprotectedresources:'z_k',hidden_overwritecomponents:'z_l',hidden_copycourseid:'wizCopyCourseId'
,hidden_copyfrom:'z_m',hidden_tools:'z_n',hidden_toolmethods:'z_o',hidden_toolnumbers:'z_p',hidden_selectedchat
:'z_q',hidden_selectedchecklists:'z_r',hidden_selectedawards:'z_s',hidden_selectedcontent:'z_t',hidden_selecteddiscuss
:'z_u',hidden_selecteddropbox:'z_v',hidden_selectedfaq:'z_w',hidden_selectedfiles:'z_x',hidden_selectedforms
:'z_y',hidden_selectedglossary:'z_z',hidden_selectedgrades:'z_ba',hidden_selectedgradessettings:'z_bb'
,hidden_selectedgroups:'z_bc',hidden_selectedhomepages:'z_bd',hidden_selectedlinks:'z_be',hidden_selectednavbars
:'z_bf',hidden_selectednavigation:'z_bg',hidden_selectednews:'z_bh',hidden_selectedquestionlibs:'z_bi'
,hidden_selectedquizzes:'z_bj',hidden_selectedschedule:'z_bk',hidden_selectedselfassess:'z_bl',hidden_selectedsurveys
:'z_bm',hidden_selectedwidgets:'z_bn',hidden_selectedcompetencies:'z_bo',hidden_selectedrubrics:'z_bp'
,hidden_selectedtoolnames:'z_bq',hidden_selectedreleaseconditions:'z_br',hidden_selectedintelligentagents
:'z_bs',hidden_selectedltilinks:'z_bt',hidden_selectedltitps:'z_bu',hidden_selectedattendanceregisters
:'z_bv',hidden_includeassociatedfiles:'z_bw',hidden_associatedfileschecked:'z_bx',hidden_selecteds3model
:'z_by',hidden_selectedcourseappearanceids:'z_bz',hidden_selectedlearningoutcomes:'z_ca',hidden_jobid
:'z_cb',hidden_filename:'z_cc',lbl_schedule:'z_cd',lbl_numschedule:'z_ce',lbl_content:'z_cf',lbl_numcontent
:'z_cg',lbl_discuss:'z_ch',lbl_numdiscuss:'z_ci',lbl_dropbox:'z_cj',lbl_numdropbox:'z_ck',lbl_grades
:'z_cl',lbl_numgrades:'z_cm',lbl_questionlibrary:'z_cn',lbl_numquestionlibrary:'z_co',lbl_quizzes:'z_cp'
,lbl_numquizzes:'z_cq',lbl_rubrics:'z_cr',lbl_numrubrics:'z_cs',lbl_courseappearance:'z_ct',lbl_numcourseappearance
:'z_cu',h_exportfiles:'z_cv',ctl_8:'z_cw',cb_exportfiles:'z_cx',ctl_messagearea:'z_cy'},'SID':{}}"), "d2l_controlMapPrev" },
                    { new StringContent(@""), "wizTempFolderPath" },
                    { new StringContent(@""), "wizImportLocation" },
                    { new StringContent(@"False"), "wizOverwriteFiles" },
                    { new StringContent(@""), "wizImportMetadata" },
                    { new StringContent(@""), "wizDateOffset" },
                    { new StringContent(@""), "wizExportFormat" },
                    { new StringContent(@""), "wizExportProtectedResources" },
                    { new StringContent(@""), "wizCopyProtectedResources" },
                    { new StringContent(@""), "wizOverwriteComponents" },
                    { new StringContent(@""), "wizCopyCourseId" },
                    { new StringContent(@""), "wizCopyFrom" },
                    { new StringContent(@"true,true,true,true,false,true,false,false,false,false,false,false,true,true,false,false,false,true,false
,true"), "wizTools" },
                    { new StringContent(@"1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,"), "wizToolMethods" },
                    { new StringContent(@"48,33,12,48,0,23,0,0,0,0,0,0,10,81,0,0,0,1,0,1"), "wizToolNumbers" },
                    { new StringContent(@""), "wizSelectedChat" },
                    { new StringContent(@""), "wizSelectedChecklists" },
                    { new StringContent(@""), "wizSelectedAwards" },
                    { new StringContent(@""), "wizSelectedContent" },
                    { new StringContent(@""), "wizSelectedDiscuss" },
                    { new StringContent(@""), "wizSelectedDropbox" },
                    { new StringContent(@""), "wizSelectedFaq" },
                    { new StringContent(@""), "wizSelectedFiles" },
                    { new StringContent(@""), "wizSelectedForms" },
                    { new StringContent(@""), "wizSelectedGlossary" },
                    { new StringContent(@""), "wizSelectedGrades" },
                    { new StringContent(@""), "wizSelectedGradesSettings" },
                    { new StringContent(@""), "wizSelectedGroups" },
                    { new StringContent(@""), "wizSelectedHomepages" },
                    { new StringContent(@""), "wizSelectedLinks" },
                    { new StringContent(@""), "wizSelectedNavbars" },
                    { new StringContent(@""), "wizSelectedNavigation" },
                    { new StringContent(@""), "wizSelectedNews" },
                    { new StringContent(@""), "wizSelectedQuestionLibs" },
                    { new StringContent(@""), "wizSelectedQuizzes" },
                    { new StringContent(@""), "wizSelectedSchedule" },
                    { new StringContent(@""), "wizSelectedSelfAssess" },
                    { new StringContent(@""), "wizSelectedSurveys" },
                    { new StringContent(@""), "wizSelectedWidgets" },
                    { new StringContent(@""), "wizSelectedCompetencies" },
                    { new StringContent(@""), "wizSelectedRubrics" },
                    { new StringContent(@""), "wizSelectedToolNames" },
                    { new StringContent(@""), "wizSelectedReleaseConditions" },
                    { new StringContent(@""), "wizSelectedIntelligentAgents" },
                    { new StringContent(@""), "wizSelectedLtiLinks" },
                    { new StringContent(@""), "wizSelectedLtiTPs" },
                    { new StringContent(@""), "wizSelectedAttendanceRegisters" },
                    { new StringContent(@""), "wizIncludeAssociatedFiles" },
                    { new StringContent(@""), "wizAssociatedFilesChecked" },
                    { new StringContent(@""), "wizSelectedS3Model" },
                    { new StringContent(@""), "wizSelectedCourseAppearanceIds" },
                    { new StringContent(@""), "wizSelectedLearningOutcomes" },
                    { new StringContent(@""), "wizJobId" },
                    { new StringContent(@""), "wizFileName" },
                    { new StringContent(@"[{'ctl_2':['z_a','Button',['Next(192371);;return false;'],{},0,0],'ctl_4':['z_b','Button',['Back(192371
);;return false;'],{},0,0],'ctl_6':['z_c','Button',['var n=new D2L.NavInfo();n.navigation=\u0027\\\/d2l
\\\/lms\\\/importexport\\\/import_export.d2l\u0027;Nav.Go(n, false, false);return false;'],{},0,0],'hidden_tempfolderpath'
:['z_d','Hidden',[1],{},0,1],'hidden_importlocation':['z_e','Hidden',[1],{},0,1],'hidden_overwritefiles'
:['z_f','Hidden',[1],{},0,1],'hidden_importmetadata':['z_g','Hidden',[1],{},0,1],'hidden_dateoffset'
:['z_h','Hidden',[1],{},0,1],'hidden_exportformat':['z_i','Hidden',[1],{},0,1],'hidden_exportprotectedresources'
:['z_j','Hidden',[1],{},0,1],'hidden_copyprotectedresources':['z_k','Hidden',[1],{},0,1],'hidden_overwritecomponents'
:['z_l','Hidden',[1],{},0,1],'hidden_copycourseid':['wizCopyCourseId','Hidden',[1],{},0,1],'hidden_copyfrom'
:['z_m','Hidden',[1],{},0,1],'hidden_tools':['z_n','Hidden',[1],{},0,1],'hidden_toolmethods':['z_o','Hidden'
,[1],{},0,1],'hidden_toolnumbers':['z_p','Hidden',[1],{},0,1],'hidden_selectedchat':['z_q','Hidden',
[1],{},0,1],'hidden_selectedchecklists':['z_r','Hidden',[1],{},0,1],'hidden_selectedawards':['z_s','Hidden'
,[1],{},0,1],'hidden_selectedcontent':['z_t','Hidden',[1],{},0,1],'hidden_selecteddiscuss':['z_u','Hidden'
,[1],{},0,1],'hidden_selecteddropbox':['z_v','Hidden',[1],{},0,1],'hidden_selectedfaq':['z_w','Hidden'
,[1],{},0,1],'hidden_selectedfiles':['z_x','Hidden',[1],{},0,1],'hidden_selectedforms':['z_y','Hidden'
,[1],{},0,1],'hidden_selectedglossary':['z_z','Hidden',[1],{},0,1],'hidden_selectedgrades':['z_ba','Hidden'
,[1],{},0,1],'hidden_selectedgradessettings':['z_bb','Hidden',[1],{},0,1],'hidden_selectedgroups':['z_bc'
,'Hidden',[1],{},0,1],'hidden_selectedhomepages':['z_bd','Hidden',[1],{},0,1],'hidden_selectedlinks'
:['z_be','Hidden',[1],{},0,1],'hidden_selectednavbars':['z_bf','Hidden',[1],{},0,1],'hidden_selectednavigation'
:['z_bg','Hidden',[1],{},0,1],'hidden_selectednews':['z_bh','Hidden',[1],{},0,1],'hidden_selectedquestionlibs'
:['z_bi','Hidden',[1],{},0,1],'hidden_selectedquizzes':['z_bj','Hidden',[1],{},0,1],'hidden_selectedschedule'
:['z_bk','Hidden',[1],{},0,1],'hidden_selectedselfassess':['z_bl','Hidden',[1],{},0,1],'hidden_selectedsurveys'
:['z_bm','Hidden',[1],{},0,1],'hidden_selectedwidgets':['z_bn','Hidden',[1],{},0,1],'hidden_selectedcompetencies'
:['z_bo','Hidden',[1],{},0,1],'hidden_selectedrubrics':['z_bp','Hidden',[1],{},0,1],'hidden_selectedtoolnames'
:['z_bq','Hidden',[1],{},0,1],'hidden_selectedreleaseconditions':['z_br','Hidden',[1],{},0,1],'hidden_selectedintelligentagents'
:['z_bs','Hidden',[1],{},0,1],'hidden_selectedltilinks':['z_bt','Hidden',[1],{},0,1],'hidden_selectedltitps'
:['z_bu','Hidden',[1],{},0,1],'hidden_selectedattendanceregisters':['z_bv','Hidden',[1],{},0,1],'hidden_includeassociatedfiles'
:['z_bw','Hidden',[1],{},0,1],'hidden_associatedfileschecked':['z_bx','Hidden',[1],{},0,1],'hidden_selecteds3model'
:['z_by','Hidden',[1],{},0,1],'hidden_selectedcourseappearanceids':['z_bz','Hidden',[1],{},0,1],'hidden_selectedlearningoutcomes'
:['z_ca','Hidden',[1],{},0,1],'hidden_jobid':['z_cb','Hidden',[1],{},0,1],'hidden_filename':['z_cc','Hidden'
,[1],{},0,1],'lbl_schedule':['z_cd','Label',[],{},0,1],'lbl_numschedule':['z_ce','Label',[],{},0,1],'lbl_content'
:['z_cf','Label',[],{},0,1],'lbl_numcontent':['z_cg','Label',[],{},0,1],'lbl_discuss':['z_ch','Label'
,[],{},0,1],'lbl_numdiscuss':['z_ci','Label',[],{},0,1],'lbl_dropbox':['z_cj','Label',[],{},0,1],'lbl_numdropbox'
:['z_ck','Label',[],{},0,1],'lbl_grades':['z_cl','Label',[],{},0,1],'lbl_numgrades':['z_cm','Label',
[],{},0,1],'lbl_questionlibrary':['z_cn','Label',[],{},0,1],'lbl_numquestionlibrary':['z_co','Label'
,[],{},0,1],'lbl_quizzes':['z_cp','Label',[],{},0,1],'lbl_numquizzes':['z_cq','Label',[],{},0,1],'lbl_rubrics'
:['z_cr','Label',[],{},0,1],'lbl_numrubrics':['z_cs','Label',[],{},0,1],'lbl_courseappearance':['z_ct'
,'Label',[],{},0,1],'lbl_numcourseappearance':['z_cu','Label',[],{},0,1],'h_exportfiles':['z_cv','Heading'
,[0,0,1],{},0,1],'ctl_8':['z_cw','Field',[],,1,1],'cb_exportfiles':['z_cx','Checkbox',[1,1,1],{},0,1
],'ctl_messagearea':['z_cy','MessageArea',['d_content_inner','d_page_header',0,[],null,null],{},0,0]
},{}]"), "d2l_controlMap" },
                    { new StringContent(@"[{'3':['grid','pagesize','htmleditor','hpg'],'1':['gridpagenum','search','pagenum'],'2':['lcs']},[]]"), "d2l_state" },
                    { new StringContent(referrer), "d2l_referrer" },
                    { new StringContent(@"{'Controls':[]}"), "d2l_multiedit" },
                    { new StringContent(@"{1:['gridpagenum','search','pagenum'],2:['lcs'],3:['grid','pagesize','htmleditor','hpg']}"), "d2l_stateScopes" },
                    { new StringContent(@""), "d2l_stateGroups" },
                    { new StringContent(@"327"), "d2l_statePageId" }
                }
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);
            }*/

            await Wait(delayMilliseconds);
            
            // 6. Finish
            string zipUrl;
            using (var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://avenue.cllmcmaster.ca/d2l/lms/importExport/export/export_process_iframe.d2l?ou=192371"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Referer", "https://avenue.cllmcmaster.ca/d2l/lms/importExport/export/export_process.d2l?ou=192371" },
                    { "Origin", "https://avenue.cllmcmaster.ca" },
                    { "Upgrade-Insecure-Requests", "1" }
                },
                Properties = { { "ou", "192371" } },
                Content = new MultipartFormDataContent("----WebKitFormBoundaryYswOsXskiEHjHh3u")
                {
                    { new StringContent(@""), "d2l_action" },
                    { new StringContent(@""), "d2l_actionparam" },
                    { new StringContent(@"981141282554513772"), "d2l_hitCode" },
                    { new StringContent(@""), "d2l_rf" },
                    { new StringContent(@"{'ID':{ctl_2:'z_a',hidden_tempfolderpath:'z_b',hidden_importlocation:'z_c',hidden_overwritefiles:'z_d'
,hidden_importmetadata:'z_e',hidden_dateoffset:'z_f',hidden_exportformat:'z_g',hidden_exportprotectedresources
:'z_h',hidden_copyprotectedresources:'z_i',hidden_overwritecomponents:'z_j',hidden_copycourseid:'wizCopyCourseId'
,hidden_copyfrom:'z_k',hidden_tools:'z_l',hidden_toolmethods:'z_m',hidden_toolnumbers:'z_n',hidden_selectedchat
:'z_o',hidden_selectedchecklists:'z_p',hidden_selectedawards:'z_q',hidden_selectedcontent:'z_r',hidden_selecteddiscuss
:'z_s',hidden_selecteddropbox:'z_t',hidden_selectedfaq:'z_u',hidden_selectedfiles:'z_v',hidden_selectedforms
:'z_w',hidden_selectedglossary:'z_x',hidden_selectedgrades:'z_y',hidden_selectedgradessettings:'z_z'
,hidden_selectedgroups:'z_ba',hidden_selectedhomepages:'z_bb',hidden_selectedlinks:'z_bc',hidden_selectednavbars
:'z_bd',hidden_selectednavigation:'z_be',hidden_selectednews:'z_bf',hidden_selectedquestionlibs:'z_bg'
,hidden_selectedquizzes:'z_bh',hidden_selectedschedule:'z_bi',hidden_selectedselfassess:'z_bj',hidden_selectedsurveys
:'z_bk',hidden_selectedwidgets:'z_bl',hidden_selectedcompetencies:'z_bm',hidden_selectedrubrics:'z_bn'
,hidden_selectedtoolnames:'z_bo',hidden_selectedreleaseconditions:'z_bp',hidden_selectedintelligentagents
:'z_bq',hidden_selectedltilinks:'z_br',hidden_selectedltitps:'z_bs',hidden_selectedattendanceregisters
:'z_bt',hidden_includeassociatedfiles:'z_bu',hidden_associatedfileschecked:'z_bv',hidden_selecteds3model
:'z_bw',hidden_selectedcourseappearanceids:'z_bx',hidden_selectedlearningoutcomes:'z_by',hidden_jobid
:'z_bz',hidden_filename:'z_ca',lbl_export:'z_cb',lbl_sumexport:'exportLabelId',lbl_content:'z_cc',lbl_sumcontent
:'contentLabelId',lbl_questionlibrary:'z_cd',lbl_sumquestionlibrary:'questionLibLabelId',lbl_quizzes
:'z_ce',lbl_sumquizzes:'quizLabelId',lbl_discuss:'z_cf',lbl_sumdiscuss:'discussLabelId',lbl_dropbox:'z_cg'
,lbl_sumdropbox:'dropboxLabelId',lbl_schedule:'z_ch',lbl_sumschedule:'scheduleLabelId',lbl_grades:'z_ci'
,lbl_sumgrades:'gradeLabelId',lbl_rubrics:'z_cj',lbl_sumrubrics:'rubricsLabelId',lbl_courseappearance
:'z_ck',lbl_sumcourseappearance:'courseAppearanceLabelId',hid_zipfile_path:'zipFilePath',hid_success
:'success',hid_errormessage:'errorMessage',hid_warningmessage:'warningMessage',hd_exportfiles:'z_cl'
,ctl_messagearea:'z_cm'},'SID':{}}"), "d2l_controlMapPrev" },
                    { new StringContent(@""), "wizTempFolderPath" },
                    { new StringContent(@""), "wizImportLocation" },
                    { new StringContent(@"False"), "wizOverwriteFiles" },
                    { new StringContent(@""), "wizImportMetadata" },
                    { new StringContent(@""), "wizDateOffset" },
                    { new StringContent(@""), "wizExportFormat" },
                    { new StringContent(@""), "wizExportProtectedResources" },
                    { new StringContent(@""), "wizCopyProtectedResources" },
                    { new StringContent(@""), "wizOverwriteComponents" },
                    { new StringContent(@""), "wizCopyCourseId" },
                    { new StringContent(@""), "wizCopyFrom" },
                    { new StringContent(@"true,true,true,true,false,true,false,false,false,false,false,false,true,true,false,false,false,true,false
,true"), "wizTools" },
                    { new StringContent(@"1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,"), "wizToolMethods" },
                    { new StringContent(@"48,33,12,48,0,23,0,0,0,0,0,0,10,81,0,0,0,1,0,1"), "wizToolNumbers" },
                    { new StringContent(@""), "wizSelectedChat" },
                    { new StringContent(@""), "wizSelectedChecklists" },
                    { new StringContent(@""), "wizSelectedAwards" },
                    { new StringContent(@""), "wizSelectedContent" },
                    { new StringContent(@""), "wizSelectedDiscuss" },
                    { new StringContent(@""), "wizSelectedDropbox" },
                    { new StringContent(@""), "wizSelectedFaq" },
                    { new StringContent(@""), "wizSelectedFiles" },
                    { new StringContent(@""), "wizSelectedForms" },
                    { new StringContent(@""), "wizSelectedGlossary" },
                    { new StringContent(@""), "wizSelectedGrades" },
                    { new StringContent(@""), "wizSelectedGradesSettings" },
                    { new StringContent(@""), "wizSelectedGroups" },
                    { new StringContent(@""), "wizSelectedHomepages" },
                    { new StringContent(@""), "wizSelectedLinks" },
                    { new StringContent(@""), "wizSelectedNavbars" },
                    { new StringContent(@""), "wizSelectedNavigation" },
                    { new StringContent(@""), "wizSelectedNews" },
                    { new StringContent(@""), "wizSelectedQuestionLibs" },
                    { new StringContent(@""), "wizSelectedQuizzes" },
                    { new StringContent(@""), "wizSelectedSchedule" },
                    { new StringContent(@""), "wizSelectedSelfAssess" },
                    { new StringContent(@""), "wizSelectedSurveys" },
                    { new StringContent(@""), "wizSelectedWidgets" },
                    { new StringContent(@""), "wizSelectedCompetencies" },
                    { new StringContent(@""), "wizSelectedRubrics" },
                    { new StringContent(@""), "wizSelectedToolNames" },
                    { new StringContent(@""), "wizSelectedReleaseConditions" },
                    { new StringContent(@""), "wizSelectedIntelligentAgents" },
                    { new StringContent(@""), "wizSelectedLtiLinks" },
                    { new StringContent(@""), "wizSelectedLtiTPs" },
                    { new StringContent(@""), "wizSelectedAttendanceRegisters" },
                    { new StringContent(@""), "wizIncludeAssociatedFiles" },
                    { new StringContent(@""), "wizAssociatedFilesChecked" },
                    { new StringContent(@""), "wizSelectedS3Model" },
                    { new StringContent(@""), "wizSelectedCourseAppearanceIds" },
                    { new StringContent(@""), "wizSelectedLearningOutcomes" },
                    { new StringContent(@""), "wizJobId" },
                    { new StringContent(@""), "wizFileName" },
                    { new StringContent(@""), "zipFilePath" },
                    { new StringContent(@"true"), "success" },
                    { new StringContent(@""), "errorMessage" },
                    { new StringContent(@""), "warningMessage" },
                    { new StringContent(@"False"), "exportFiles" },
                    { new StringContent(@"[{'ctl_2':['z_a','Button',['Done(192371);;return false;'],{},0,1],'hidden_tempfolderpath':['z_b','Hidden'
,[1],{},0,1],'hidden_importlocation':['z_c','Hidden',[1],{},0,1],'hidden_overwritefiles':['z_d','Hidden'
,[1],{},0,1],'hidden_importmetadata':['z_e','Hidden',[1],{},0,1],'hidden_dateoffset':['z_f','Hidden'
,[1],{},0,1],'hidden_exportformat':['z_g','Hidden',[1],{},0,1],'hidden_exportprotectedresources':['z_h'
,'Hidden',[1],{},0,1],'hidden_copyprotectedresources':['z_i','Hidden',[1],{},0,1],'hidden_overwritecomponents'
:['z_j','Hidden',[1],{},0,1],'hidden_copycourseid':['wizCopyCourseId','Hidden',[1],{},0,1],'hidden_copyfrom'
:['z_k','Hidden',[1],{},0,1],'hidden_tools':['z_l','Hidden',[1],{},0,1],'hidden_toolmethods':['z_m','Hidden'
,[1],{},0,1],'hidden_toolnumbers':['z_n','Hidden',[1],{},0,1],'hidden_selectedchat':['z_o','Hidden',
[1],{},0,1],'hidden_selectedchecklists':['z_p','Hidden',[1],{},0,1],'hidden_selectedawards':['z_q','Hidden'
,[1],{},0,1],'hidden_selectedcontent':['z_r','Hidden',[1],{},0,1],'hidden_selecteddiscuss':['z_s','Hidden'
,[1],{},0,1],'hidden_selecteddropbox':['z_t','Hidden',[1],{},0,1],'hidden_selectedfaq':['z_u','Hidden'
,[1],{},0,1],'hidden_selectedfiles':['z_v','Hidden',[1],{},0,1],'hidden_selectedforms':['z_w','Hidden'
,[1],{},0,1],'hidden_selectedglossary':['z_x','Hidden',[1],{},0,1],'hidden_selectedgrades':['z_y','Hidden'
,[1],{},0,1],'hidden_selectedgradessettings':['z_z','Hidden',[1],{},0,1],'hidden_selectedgroups':['z_ba'
,'Hidden',[1],{},0,1],'hidden_selectedhomepages':['z_bb','Hidden',[1],{},0,1],'hidden_selectedlinks'
:['z_bc','Hidden',[1],{},0,1],'hidden_selectednavbars':['z_bd','Hidden',[1],{},0,1],'hidden_selectednavigation'
:['z_be','Hidden',[1],{},0,1],'hidden_selectednews':['z_bf','Hidden',[1],{},0,1],'hidden_selectedquestionlibs'
:['z_bg','Hidden',[1],{},0,1],'hidden_selectedquizzes':['z_bh','Hidden',[1],{},0,1],'hidden_selectedschedule'
:['z_bi','Hidden',[1],{},0,1],'hidden_selectedselfassess':['z_bj','Hidden',[1],{},0,1],'hidden_selectedsurveys'
:['z_bk','Hidden',[1],{},0,1],'hidden_selectedwidgets':['z_bl','Hidden',[1],{},0,1],'hidden_selectedcompetencies'
:['z_bm','Hidden',[1],{},0,1],'hidden_selectedrubrics':['z_bn','Hidden',[1],{},0,1],'hidden_selectedtoolnames'
:['z_bo','Hidden',[1],{},0,1],'hidden_selectedreleaseconditions':['z_bp','Hidden',[1],{},0,1],'hidden_selectedintelligentagents'
:['z_bq','Hidden',[1],{},0,1],'hidden_selectedltilinks':['z_br','Hidden',[1],{},0,1],'hidden_selectedltitps'
:['z_bs','Hidden',[1],{},0,1],'hidden_selectedattendanceregisters':['z_bt','Hidden',[1],{},0,1],'hidden_includeassociatedfiles'
:['z_bu','Hidden',[1],{},0,1],'hidden_associatedfileschecked':['z_bv','Hidden',[1],{},0,1],'hidden_selecteds3model'
:['z_bw','Hidden',[1],{},0,1],'hidden_selectedcourseappearanceids':['z_bx','Hidden',[1],{},0,1],'hidden_selectedlearningoutcomes'
:['z_by','Hidden',[1],{},0,1],'hidden_jobid':['z_bz','Hidden',[1],{},0,1],'hidden_filename':['z_ca','Hidden'
,[1],{},0,1],'lbl_export':['z_cb','Label',[],{},0,1],'lbl_sumexport':['exportLabelId','Label',[],{},0
,1],'lbl_content':['z_cc','Label',[],{},0,1],'lbl_sumcontent':['contentLabelId','Label',[],{},0,1],'lbl_questionlibrary'
:['z_cd','Label',[],{},0,1],'lbl_sumquestionlibrary':['questionLibLabelId','Label',[],{},0,1],'lbl_quizzes'
:['z_ce','Label',[],{},0,1],'lbl_sumquizzes':['quizLabelId','Label',[],{},0,1],'lbl_discuss':['z_cf'
,'Label',[],{},0,1],'lbl_sumdiscuss':['discussLabelId','Label',[],{},0,1],'lbl_dropbox':['z_cg','Label'
,[],{},0,1],'lbl_sumdropbox':['dropboxLabelId','Label',[],{},0,1],'lbl_schedule':['z_ch','Label',[],
{},0,1],'lbl_sumschedule':['scheduleLabelId','Label',[],{},0,1],'lbl_grades':['z_ci','Label',[],{},0
,1],'lbl_sumgrades':['gradeLabelId','Label',[],{},0,1],'lbl_rubrics':['z_cj','Label',[],{},0,1],'lbl_sumrubrics'
:['rubricsLabelId','Label',[],{},0,1],'lbl_courseappearance':['z_ck','Label',[],{},0,1],'lbl_sumcourseappearance'
:['courseAppearanceLabelId','Label',[],{},0,1],'hid_zipfile_path':['zipFilePath','Hidden',[1],{},0,1
],'hid_success':['success','Hidden',[1],{},0,1],'hid_errormessage':['errorMessage','Hidden',[],{},0,1
],'hid_warningmessage':['warningMessage','Hidden',[],{},0,1],'hd_exportfiles':['z_cl','Hidden',[1],{
},0,1],'ctl_messagearea':['z_cm','MessageArea',['d_content_inner','d_page_header',0,[],null,null],{}
,0,0]},{}]"), "d2l_controlMap" },
                    { new StringContent(@"[{'3':['grid','pagesize','htmleditor','hpg'],'1':['gridpagenum','search','pagenum'],'2':['lcs']},[]]"), "d2l_state" },
                    { new StringContent(@""), "d2l_referrer" }, // TODO: set referrer if you uncomment code above { new StringContent(referrer), "d2l_referrer" }
                    { new StringContent(@"{'Controls':[]}"), "d2l_multiedit" },
                    { new StringContent(@"{1:['gridpagenum','search','pagenum'],2:['lcs'],3:['grid','pagesize','htmleditor','hpg']}"), "d2l_stateScopes" },
                    { new StringContent(@""), "d2l_stateGroups" },
                    { new StringContent(@"328"), "d2l_statePageId" }
                }
            })
            using (var response = await Client.SendAsync(request))
            {
                CheckResponse(response);

                var html = await response.Content.ReadAsStringAsync();
                zipUrl = FindZipUrl(html);
            }

            await Wait(delayMilliseconds);

            // Download, unzip and get xml files
            var files = new List<string>();
            using (var response = await Client.GetAsync("https://avenue.cllmcmaster.ca" + zipUrl))
            {
                CheckResponse(response);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        using (var entryStream = entry.Open())
                        using (var reader = new StreamReader(entryStream))
                        {
                            var text = reader.ReadToEnd();

                            files.Add(text);
                        }
                    }
                }
            }

            Assert.IsTrue(files.Count > 0);
            foreach (var file in files)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(file));
            }

            // return files?
            // I recommend to use cache
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            Client.Dispose(); // Remember to call Dispose when you no longer need the Client.
        }

        private static async Task Wait(int delayMilliseconds)
        {
            if (delayMilliseconds <= 0)
            {
                return;
            }

            await Task.Delay(delayMilliseconds);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void CheckResponse(HttpResponseMessage response)
        {
            Assert.IsTrue(response.IsSuccessStatusCode, $@"
Error: {response.ReasonPhrase}
Status code: {response.StatusCode}
");
        }

        /*
        private static string FindReferrer(string html)
        {
            const int length = 32;
            const string text = "<input type=\"hidden\" name=\"d2l_referrer\" value=\"";
            var index = html.IndexOf(text, StringComparison.OrdinalIgnoreCase) + text.Length;

            return html.Substring(index, length);
        }
        */

        private static string FindZipUrl(string html)
        {
            const string text = "UpdateZipFilePath( \'zipFilePath\', \'";
            var index1 = html.IndexOf(text, StringComparison.OrdinalIgnoreCase) + text.Length;
            var index2 = html.IndexOf('\'', index1);

            return html.Substring(index1, index2 - index1);
        }
    }
}
