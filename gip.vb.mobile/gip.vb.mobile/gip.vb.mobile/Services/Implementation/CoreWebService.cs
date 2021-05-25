using System;
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

namespace gip.vb.mobile.Services
{
    public class CoreWebService : ICoreWebService, ILoginService
    {
        protected TimeoutHandler _TimeOutHandler;
        protected HttpClient _Client;
        protected VBUser _VBUser;
        protected static Guid? _CurrentSessionId;
        private bool _WithTimeoutHandler;

        public CoreWebService()
        {
            CreateNewHttpClient();
        }

        public CoreWebService(Uri baseAddress)
        {
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
            return await Get<TResult>(new Uri(uriString, UriKind.Relative));
        }

        protected async Task<WSResponse<TResult>> Get<TResult>(Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                HttpResponseMessage response = null;
                if (_TimeOutHandler != null)
                    response = await _Client.GetAsync(uri, cts.Token);
                else
                    response = await _Client.GetAsync(uri);
                //var json = await _Client.GetStringAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                    return result;
                }
                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }


        protected async Task<WSResponse<TResult>> Post<TResult, TParam>(TParam item, string uriString)
        {
            return await Post<TResult, TParam>(item, new Uri(uriString, UriKind.Relative));
        }

        protected async Task<WSResponse<TResult>> Post<TResult, TParam>(TParam item, Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
                var serializedItem = JsonConvert.SerializeObject(item, microsoftDateFormatSettings);
                var buffer = Encoding.UTF8.GetBytes(serializedItem);
                var byteContent = new ByteArrayContent(buffer);
                HttpResponseMessage response = null;
                if (_TimeOutHandler != null)
                    response = await _Client.PostAsync(uri, byteContent, cts.Token);
                else
                    response = await _Client.PostAsync(uri, byteContent);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                    return result;
                }

                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }


        protected async Task<WSResponse<TResult>> Put<TResult, TParam>(TParam item, string uriString)
        {
            return await Put<TResult, TParam>(item, new Uri(uriString, UriKind.Relative));
        }

        protected async Task<WSResponse<TResult>> Put<TResult, TParam>(TParam item, Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
                var serializedItem = JsonConvert.SerializeObject(item, microsoftDateFormatSettings);
                var buffer = Encoding.UTF8.GetBytes(serializedItem);
                var byteContent = new ByteArrayContent(buffer);
                HttpResponseMessage response = null;
                if (_TimeOutHandler != null)
                    response = await _Client.PutAsync(uri, byteContent, cts.Token);
                else
                    response = await _Client.PutAsync(uri, byteContent);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                    return result;
                }

                return await Task.FromResult(new WSResponse<TResult>(default(TResult)));
            }
        }


        protected async Task<WSResponse<TResult>> Delete<TResult>(string uriString)
        {
            return await Delete<TResult>(new Uri(uriString, UriKind.Relative));
        }

        protected async Task<WSResponse<TResult>> Delete<TResult>(Uri uri)
        {
            using (var cts = new CancellationTokenSource())
            {
                HttpResponseMessage response = null;
                if (_TimeOutHandler != null)
                    response = await _Client.DeleteAsync(uri, cts.Token);
                else
                    response = await _Client.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = await Task.Run(() => JsonConvert.DeserializeObject<WSResponse<TResult>>(json));
                    return result;
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
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
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

        #endregion
    }
}