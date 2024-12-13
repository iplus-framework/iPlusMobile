// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using gip.mes.webservices;
using System.Net.Http.Headers;
using gip.core.webservices;
using gip.core.autocomponent;
using System.Net;
using System.Linq;
using gip.core.datamodel;
using System.Threading;
using Newtonsoft.Json.Serialization;

namespace gip.vbm.mobile.Services
{
    //public class ConverterContractResolver : DefaultContractResolver
    //{
    //    public static readonly ConverterContractResolver Instance = new ConverterContractResolver();

    //    override 

    //    protected override JsonContract CreateContract(Type objectType)
    //    {
    //        JsonContract contract = base.CreateContract(objectType);

    //        // this will only be called once and then cached
    //        if (objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset))
    //        {
    //            contract.Converter = new JavaScriptDateTimeConverter();
    //        }

    //        return contract;
    //    }
    //}

    public class CoreWebService : ICoreWebService, ILoginService
    {
        protected TimeoutHandler _TimeOutHandler;
        protected HttpClient _Client;
        protected VBUser _VBUser;
        protected static Guid? _CurrentSessionId;
        private bool _WithTimeoutHandler = false;

        private const int C_PerfID_Serialize = 100;
        private const int C_PerfID_HttpComm = 200;
        private const int C_PerfID_DeSerialize = 300;

        public CoreWebService()
        {
            _WithTimeoutHandler = false;
            CreateNewHttpClient();
        }

        public CoreWebService(Uri baseAddress)
        {
            _WithTimeoutHandler = false;
            CreateNewHttpClient();
            _Client.BaseAddress = baseAddress;
        }

        private void CreateNewHttpClient()
        {
            if (_TimeOutHandler != null)
                _TimeOutHandler.Dispose();
            if (_Client != null)
                _Client.Dispose();

            if (_WithTimeoutHandler)
            {
                _TimeOutHandler = new TimeoutHandler()
                {
                    DefaultTimeout = DefaultTimeOut,
                    InnerHandler = new HttpClientHandler()
                };
                _Client = new HttpClient(_TimeOutHandler);
            }
            else
            {
                _Client = new HttpClient();
                if (_DefaultTimeOut.HasValue)
                    _Client.Timeout = DefaultTimeOut;
            }
            _Client.BaseAddress = new Uri(App.SettingsViewModel.IPlusBackendUrl);
        }

        #region Properties
        public HttpClient Client
        {
            get
            {
                return _Client;
            }
        }

        public virtual bool IsMockService
        {
            get
            {
                return false;
            }
        }

        private TimeSpan? _DefaultTimeOut;
        public TimeSpan DefaultTimeOut
        {
            get
            {
                return _DefaultTimeOut.HasValue ? _DefaultTimeOut.Value : TimeSpan.FromSeconds(20);
            }
            set
            {
                _DefaultTimeOut = value;
            }
        }
        #endregion

        #region HTTP-Methods

        protected async Task<WSResponse<TResult>> Get<TResult>(string uriString)
        {
            var response = await Get<TResult>(new Uri(uriString, UriKind.Relative));
            if (response != null && IsLoginAgainResponse(response.Message))
            {
                WSResponse<VBUserRights> result = await Login(_VBUser.Username);
                if (result.Message != null)
                    response.Message = result.Message;
                else
                    response = await Get<TResult>(new Uri(uriString, UriKind.Relative));
            }

            return response;
        }

        protected async Task<WSResponse<TResult>> Get<TResult>(Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                HttpResponseMessage response = null;
                PerformanceEvent perfEventHttp = null;
                PerformanceEvent perfEventDeSerial = null;
                try
                {
                    perfEventHttp = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_HttpComm);
                    if (_TimeOutHandler != null)
                        response = await _Client.GetAsync(uri, cts.Token);
                    else
                        response = await _Client.GetAsync(uri);
                    //var json = await _Client.GetStringAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;

                        perfEventDeSerial = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_DeSerialize);
                        var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                        perfEventDeSerial = null;
                        return result;
                    }
                    else
                    {
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;
                    }
                }
                finally
                {
                    if (perfEventHttp != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                    if (perfEventDeSerial != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                }
                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }


        protected async Task<WSResponse<TResult>> Post<TResult, TParam>(TParam item, string uriString)
        {
            var response = await Post<TResult, TParam>(item, new Uri(uriString, UriKind.Relative));

            if (response != null && IsLoginAgainResponse(response.Message))
            {
                WSResponse<VBUserRights> result = await Login(_VBUser.Username);
                if (result.Message != null)
                    response.Message = result.Message;
                else
                    response = await Post<TResult, TParam>(item, new Uri(uriString, UriKind.Relative));
            }

            return response;
        }

        protected async Task<WSResponse<TResult>> Post<TResult, TParam>(TParam item, Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };

                PerformanceEvent perfEventHttp = null;
                PerformanceEvent perfEventSerial = null;
                PerformanceEvent perfEventDeSerial = null;
                try
                {
                    perfEventSerial = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_Serialize);
                    var serializedItem = JsonConvert.SerializeObject(item, microsoftDateFormatSettings);
                    var buffer = Encoding.UTF8.GetBytes(serializedItem);
                    var byteContent = new ByteArrayContent(buffer);
                    App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_Serialize, perfEventSerial);
                    perfEventSerial = null;

                    perfEventHttp = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_HttpComm);
                    HttpResponseMessage response = null;
                    if (_TimeOutHandler != null)
                        response = await _Client.PostAsync(uri, byteContent, cts.Token);
                    else
                        response = await _Client.PostAsync(uri, byteContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;

                        perfEventDeSerial = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_DeSerialize);
                        var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                        perfEventDeSerial = null;
                        return result;
                    }
                    else
                    {
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;
                    }
                }
                finally
                {
                    if (perfEventSerial != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_Serialize, perfEventSerial);
                    if (perfEventHttp != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                    if (perfEventDeSerial != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                }

                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }


        protected async Task<WSResponse<TResult>> Put<TResult, TParam>(TParam item, string uriString)
        {
            var response = await Put<TResult, TParam>(item, new Uri(uriString, UriKind.Relative));

            if (response != null && IsLoginAgainResponse(response.Message))
            {
                WSResponse<VBUserRights> result = await Login(_VBUser.Username);
                if (result.Message != null)
                    response.Message = result.Message;
                else
                    response = await Put<TResult, TParam>(item, new Uri(uriString, UriKind.Relative));
            }

            return response;
        }

        protected async Task<WSResponse<TResult>> Put<TResult, TParam>(TParam item, Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
                PerformanceEvent perfEventHttp = null;
                PerformanceEvent perfEventSerial = null;
                PerformanceEvent perfEventDeSerial = null;
                try
                {
                    perfEventSerial = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_Serialize);
                    var serializedItem = JsonConvert.SerializeObject(item, microsoftDateFormatSettings);
                    var buffer = Encoding.UTF8.GetBytes(serializedItem);
                    var byteContent = new ByteArrayContent(buffer);
                    App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_Serialize, perfEventSerial);
                    perfEventSerial = null;

                    perfEventHttp = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_HttpComm);
                    HttpResponseMessage response = null;
                    if (_TimeOutHandler != null)
                        response = await _Client.PutAsync(uri, byteContent, cts.Token);
                    else
                        response = await _Client.PutAsync(uri, byteContent);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;

                        perfEventDeSerial = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_DeSerialize);
                        var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                        perfEventDeSerial = null;
                        return result;
                    }
                    else
                    {
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;
                    }
                }
                finally
                {
                    if (perfEventSerial != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_Serialize, perfEventSerial);
                    if (perfEventHttp != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                    if (perfEventDeSerial != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                }

                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }


        protected async Task<WSResponse<TResult>> Delete<TResult>(string uriString)
        {
            var response = await Delete<TResult>(new Uri(uriString, UriKind.Relative));

            if (response != null && IsLoginAgainResponse(response.Message))
            {
                WSResponse<VBUserRights> result = await Login(_VBUser.Username);
                if (result.Message != null)
                    response.Message = result.Message;
                else
                    response = await Delete<TResult>(new Uri(uriString, UriKind.Relative));
            }

            return response;
        }

        protected async Task<WSResponse<TResult>> Delete<TResult>(Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                HttpResponseMessage response = null;
                PerformanceEvent perfEventHttp = null;
                PerformanceEvent perfEventDeSerial = null;
                try
                {
                    perfEventHttp = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_HttpComm);
                    if (_TimeOutHandler != null)
                        response = await _Client.DeleteAsync(uri, cts.Token);
                    else
                        response = await _Client.DeleteAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;

                        perfEventDeSerial = App.PerfLogger.Start(uri.WebServiceMethodName(), C_PerfID_DeSerialize);
                        var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                        perfEventDeSerial = null;
                        return result;
                    }
                    else
                    {
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                        perfEventHttp = null;
                    }
                }
                finally
                {
                    if (perfEventHttp != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_HttpComm, perfEventHttp);
                    if (perfEventDeSerial != null)
                        App.PerfLogger.Stop(uri.WebServiceMethodName(), C_PerfID_DeSerialize, perfEventDeSerial);
                }

                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }

        #endregion

        #region ICoreWebService

        public void ReloadSettings()
        {
            try
            {
                _Client.BaseAddress = new Uri($"{App.SettingsViewModel.IPlusBackendUrl}/");
            }
            catch
            {
                CreateNewHttpClient();
            }
        }

        public async Task<VBUserRights> Login(VBUser user)
        {
            _VBUser = user;
            _Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(
                    System.Text.UTF8Encoding.UTF8.GetBytes(
                       $"{user.Username}:{user.Password}")));

            if (_CurrentSessionId.HasValue)
            {
                IEnumerable<string> valuesExists;
                if (!_Client.DefaultRequestHeaders.TryGetValues("Cookie", out valuesExists))
                    _Client.DefaultRequestHeaders.Add("Cookie", CoreWebServiceConst.BuildSessionIdForCookieHeader(_CurrentSessionId.Value));
            }
            WSResponse<VBUserRights> result = await Login(user.Username);
            return result.Data;
        }


        public async Task<WSResponse<VBUserRights>> Login(string userName)
        {
            var responseMessage = await _Client.GetAsync(String.Format(CoreWebServiceConst.UriLogin_F, userName));
            string json = await responseMessage.Content.ReadAsStringAsync();
            WSResponse<VBUserRights> wSResponse = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<VBUserRights>>(json));
            if (wSResponse.Suceeded && wSResponse.Data != null)
            {
                IEnumerable<string> values;
                if (responseMessage.Headers.TryGetValues("Set-Cookie", out values))
                {
                    _CurrentSessionId = CoreWebServiceConst.DecodeSessionIdFromCookieHeader(values.FirstOrDefault());
                    IEnumerable<string> valuesExists;
                    if (!_Client.DefaultRequestHeaders.TryGetValues("Cookie", out valuesExists))
                        _Client.DefaultRequestHeaders.Add("Cookie", values);
                }
            }
            return wSResponse;
        }

        public async Task<WSResponse<bool>> Logout(string sessionID)
        {
            return await Get<bool>(String.Format(CoreWebServiceConst.UriLogout_F, sessionID));
        }


        public async Task<WSResponse<ACClass>> GetACClassByBarcodeAsync(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return await Task.FromResult(new WSResponse<ACClass>(null, new Msg(eMsgLevel.Error, "barcodeID is empty")));
            return await Get<ACClass>(String.Format(CoreWebServiceConst.UriACClass_BarcodeID_F, barcodeID));
        }

        private bool IsLoginAgainResponse(Msg responseMsg)
        {
            Msg loginAgainMsg = WSResponse<bool>.LoginAgainMessage;

            if (responseMsg == null)
                return false;

            return responseMsg.Row == loginAgainMsg.Row && responseMsg.Column == loginAgainMsg.Column && responseMsg.ACIdentifier == loginAgainMsg.ACIdentifier;
        }

        public async Task<WSResponse<bool>> DumpPerfLog(string perfLog)
        {
            if (String.IsNullOrEmpty(perfLog))
                return await Task.FromResult(new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "perfLog is null.")));
            return await Post<bool, string>(perfLog, CoreWebServiceConst.UriDumpPerfLog);
        }

        #endregion
    }
}