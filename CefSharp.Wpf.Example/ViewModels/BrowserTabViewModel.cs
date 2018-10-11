// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.Example;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CefSharp.Wpf.Example.ViewModels
{
    public class BrowserTabViewModel : ViewModelBase
    {
        private string address;
        public string Address
        {
            get { return address; }
            set { Set(ref address, value); }
        }

        private string addressEditable;
        public string AddressEditable
        {
            get { return addressEditable; }
            set { Set(ref addressEditable, value); }
        }

        private string outputMessage;
        public string OutputMessage
        {
            get { return outputMessage; }
            set { Set(ref outputMessage, value); }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { Set(ref statusMessage, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        private IWpfWebBrowser webBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return webBrowser; }
            set { Set(ref webBrowser, value); }
        }

        private object evaluateJavaScriptResult;
        public object EvaluateJavaScriptResult
        {
            get { return evaluateJavaScriptResult; }
            set { Set(ref evaluateJavaScriptResult, value); }
        }

        private bool showSidebar;
        public bool ShowSidebar
        {
            get { return showSidebar; }
            set { Set(ref showSidebar, value); }
        }

        public ICommand GoCommand { get; private set; }
        public ICommand HomeCommand { get; private set; }
        public ICommand ExecuteJavaScriptCommand { get; private set; }
        public ICommand EvaluateJavaScriptCommand { get; private set; }
        public ICommand ShowDevToolsCommand { get; private set; }
        public ICommand CloseDevToolsCommand { get; private set; }

        public BrowserTabViewModel(string address)
        {
            Address = address;
            AddressEditable = Address;

            GoCommand = new RelayCommand(Go, () => !String.IsNullOrWhiteSpace(Address));
            HomeCommand = new RelayCommand(() => AddressEditable = Address = CefExample.DefaultUrl);
            ExecuteJavaScriptCommand = new RelayCommand<string>(ExecuteJavaScript, s => !String.IsNullOrWhiteSpace(s));
            EvaluateJavaScriptCommand = new RelayCommand<string>(EvaluateJavaScript, s => !String.IsNullOrWhiteSpace(s));
            ShowDevToolsCommand = new RelayCommand(() => webBrowser.ShowDevTools());
            CloseDevToolsCommand = new RelayCommand(() => webBrowser.CloseDevTools());

            PropertyChanged += OnPropertyChanged;

            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);
            OutputMessage = version;
        }

        private async void EvaluateJavaScript(string s)
        {
            try
            {
                var response = await webBrowser.EvaluateScriptAsync(s);
                if (response.Success && response.Result is IJavascriptCallback)
                {
                    response = await ((IJavascriptCallback)response.Result).ExecuteAsync("This is a callback from EvaluateJavaScript");
                }

                EvaluateJavaScriptResult = response.Success ? (response.Result ?? "null") : response.Message;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while evaluating Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteJavaScript(string s)
        {
            try
            {
                webBrowser.ExecuteScriptAsync(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while executing Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Address":
                    AddressEditable = Address;
                    break;

                case "WebBrowser":
                    if (WebBrowser != null)
                    {
                        WebBrowser.ConsoleMessage += OnWebBrowserConsoleMessage;
                        WebBrowser.StatusMessage += OnWebBrowserStatusMessage;
                        WebBrowser.LoadError += OnWebBrowserLoadError;

                        // TODO: This is a bit of a hack. It would be nicer/cleaner to give the webBrowser focus in the Go()
                        // TODO: method, but it seems like "something" gets messed up (= doesn't work correctly) if we give it
                        // TODO: focus "too early" in the loading process...
                        WebBrowser.FrameLoadEnd += delegate { Application.Current.Dispatcher.BeginInvoke((Action)(() => webBrowser.Focus())); };
                    }

                    break;
            }
        }

        private void OnWebBrowserConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            OutputMessage = e.Message;
        }

        private void OnWebBrowserStatusMessage(object sender, StatusMessageEventArgs e)
        {
            StatusMessage = e.Value;
        }

        private void OnWebBrowserLoadError(object sender, LoadErrorEventArgs args)
        {
            // Don't display an error for downloaded files where the user aborted the download.
            if (args.ErrorCode == CefErrorCode.Aborted)
                return;

            var errorMessage = "<html><body><h2>Failed to load URL " + args.FailedUrl +
                  " with error " + args.ErrorText + " (" + args.ErrorCode +
                  ").</h2></body></html>";

            webBrowser.LoadHtml(errorMessage, args.FailedUrl);
        }

        private void Go()
        {
            //Address = AddressEditable;

            var str = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                    <html><head><title></title><meta name=""viewport"" content=""width=device-width,initial-scale=1.0,user-scalable=1"" /><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" /><style type=""text/css"">@import url(http://fonts.googleapis.com/css?family=Merriweather:400,400italic,700italic,700|Open+Sans:400,400italic,700,700italic|Lato:400,400italic,700,700italic);</style><style type=""text/css"">#outlook a {padding: 0;}
                    .ReadMsgBody, .ExternalClass {width: 100%;}
                    .ExternalClass * {line-height: 100%;}
                    body { background-color: #ffffff; margin: 0; padding: 0;
                    -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }
                    table, td { border-collapse:collapse;
                    mso-table-lspace: 0pt; mso-table-rspace: 0pt; }
                    img { border: 0; height: auto; line-height: 100%;
                    outline: none; text-decoration: none;
                    -ms-interpolation-mode: bicubic; }
                    .column {display: inline-block !important; width: 100% !important;
                    text-align: left; vertical-align: top; }
                    @media only screen and (min-width:480px) {
                    .column-100 {
                    width: 100% !important; display: inline-block !important;
                    text-align: left; vertical-align: top; }
                    .column-50 {
                    width: 50% !important; display: inline-block !important;
                    text-align: left; vertical-align: top; }
                    .column-33 {
                    width: 33.3% !important; display: inline-block !important;
                    text-align: left; vertical-align: top; }}
                    @media only screen and (max-width:479px) {
                    .socialBlock, td.imageBlock, .buttonBlock,
                    .alignmentContainer { text-align: center !important }
                    .alignmentContainer table { margin-right: auto;
                    margin-left: auto;}}</style></head><body style=""background:#ffffff""><div style=""display:none;font-size:1px;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden;"">Vou ensinar como aumentar sua Frequência Vibracional para conquistar tudo o que deseja.  Você sabe em que frequência está vibrando? </div>
                      <!--[if gte mso 9]><xml><o:OfficeDocumentSettings><o:AllowPNG/><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml><![endif]-->
                      <div style=""background:#ffffff"">
                       <!--[if mso]><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"" style=""width:600px;""><tr><td><![endif]-->
                       <div style=""max-width: 600px; background: rgb(255, 255, 255); margin: 0px auto;"">
                        <table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"" style=""font-size: 0px; width: 100%; background: rgb(255, 255, 255);""><tbody><tr><td style=""text-align: center; vertical-align: top; font-size: 0px;"">
                            <!--[if mso]><table border=""0"" cellpadding=""0"" cellspacing=""0""><tr><![endif]-->
                            <!--[if mso]><td style=""vertical-align:top;width:600px;""><![endif]-->
                            <div class=""column column-100"" style=""display: inline-block; width: 100%; vertical-align: top;"">
                             <table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%""><tbody><tr><td style=""word-wrap: break-word; word-break: break-word; font-size: 0px; padding: 15px; text-align: left;"">
                                 <div>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 0px 0px 13px; line-height: 1.6;"">Gentee, batemos todos os recordes!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Nos primeiros dois episódios, com menos da metade do andamento desse documentário incrível, já foram mais de 1900 comentários somente na página da WebSérie, sem contar em todas as demais redes. Mais de 100 mil visualizações na primeira aula e mais de 50 mil visualizações na segunda!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Isso é simplesmente EXTRAORDINÁRIO!!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">É a comprovação de tudo que acredito, ensino, de toda a minha luta para fazer com que as pessoas entendam que somente são limitadas por si mesmas, por seus pensamentos, seus sentimentos incongruentes. E quando os resultados aparecem, mesmo assim, em poucas horas de conteúdo gratuito, é lindooooo! Transbordando de emoção com tanto carinho!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;""><a href=""https://qn482.infusionsoft.com/app/linkClick/102/ba9ffe876d891fa3/1636470/c1af841dfff22b9a"" class=""bard-text-block style-scope"" target=""_blank"" style=""color: rgb(43, 170, 223); padding: 0px; line-height: 1.6;""><b class=""bard-text-block style-scope"" style=""padding: 0px; line-height: 1.6;"">Descubra os Segredos Ocultos da Manifestação de Sonhos</b></a></p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">A cada minuto chegam mais comentários, depoimentos, casos de transformação instantâneas!! As duas Técnicas EXCLUSIVAS que apresentamos são realmente fenomenais, com poder de desprogramar qualquer memória do passado e te colocar em contato com seu verdadeiro EU, sua essência! É isso que vai te tornar um COCRIADOR DE SONHOS!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Se você ainda não deixou o seu, clica aqui e me conta o que está achando! O que está fazendo sentido pra você?? Eu amoooo saber que estou fazendo parte da história de cada um de vocês!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">E olha que máximo… o <b class=""bard-text-block style-scope"" style=""padding: 0px; line-height: 1.6;"">TERCEIRO EPISÓDIO</b> da WebSérie Quântica Holo Cocriação de Sonhos e Metas já está no ar!! Para assistir é só <a href=""https://qn482.infusionsoft.com/app/linkClick/108/42d48dc5a1d78188/1636470/c1af841dfff22b9a"" class=""bard-text-block style-scope"" target=""_blank"" style=""color: rgb(43, 170, 223); padding: 0px; line-height: 1.6;"">clicar nesse link</a>.</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;""><a href=""https://qn482.infusionsoft.com/app/linkClick/104/90aad55b55db6583/1636470/c1af841dfff22b9a"" class=""bard-text-block style-scope"" target=""_blank"" style=""color: rgb(43, 170, 223); padding: 0px; line-height: 1.6;""><b class=""bard-text-block style-scope"" style=""padding: 0px; line-height: 1.6;"">EPISÓDIO 3: MUDE SUA FREQUÊNCIA PARA MUDAR SUA VIDA</b></a></p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Vou ensinar como aumentar sua Frequência Vibracional para conquistar tudo o que deseja.</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Você sabe em que frequência está vibrando?</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Sabe qual é a frequência que cocria sonhos?</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Qual a Frequência da Prosperidade? Da alma gêmea? Frequência de cura?</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Não é possível receber do universo algo que você não sabe dar, pois tudo é energia e vibração. Se você emanar medo, angústia, incerteza, raiva… só vai conseguir que mais desses sentimentos retornem e dificultem o caminho de sua felicidade.</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Quer saber em que frequência você está vibrando e de quebra descobrir o terceiro SEGREDO da manifestação e cocriação da realidade? Clique no link abaixo e acompanhe este terceiro episódio, que está imperdível!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;""><a href=""https://qn482.infusionsoft.com/app/linkClick/106/712b34c11fe3f42c/1636470/c1af841dfff22b9a"" class=""bard-text-block style-scope"" target=""_blank"" style=""color: rgb(43, 170, 223); padding: 0px; line-height: 1.6;""><b class=""bard-text-block style-scope"" style=""padding: 0px; line-height: 1.6;"">ASSISTA AQUI</b></a></p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Neste episódio, experienciaremos juntos uma técnica INÉDITA, a Técnica Anahata – Alteração Instantânea e Mudança imediata da frequência vibracional, com ferramentas de reprogramação mental, vibracional e emocional, comandos quânticos e decretos metafísicos que acessam o Chakra do coração.</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Se você não viu os primeiros episódios, recomendo que faça isso agora mesmo, vai conseguir ver por este mesmo link. Depois me conta tudo que achou!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Não perca tempo, pois em poucos dias os vídeos vão sair do ar. Certamente, depois de tudo que vimos aqui, você não vai querer perder, não é mesmo??</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Louca pra saber o impacto que vai causar!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Amo muito tudo issooo!!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Com todo meu amor!</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Elainne Ourives</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Treinadora Mental e Reprogramadora de DNA</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Criadora da Técnica Hertz - Reprogramação da Frequência Vibracional</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">P.S.:</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;"">Se você quiser receber nossas informações de forma mais ágil, em seu celular, inscreva-se em nossas Listas VIP.</p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px; line-height: 1.6;""><a href=""https://qn482.infusionsoft.com/app/linkClick/110/b36f2047cde76610/1636470/c1af841dfff22b9a"" class=""bard-text-block style-scope"" target=""_blank"" style=""color: rgb(43, 170, 223); padding: 0px; line-height: 1.6;""><b class=""bard-text-block style-scope"" style=""padding: 0px; line-height: 1.6;"">Facebook</b></a></p>
                                  <p class=""bard-text-block style-scope"" style=""color: rgb(52, 52, 52); font-family: Helvetica, Arial, sans-serif; font-size: 16px; padding: 0px; margin: 13px 0px 0px; line-height: 1.6;""><a href=""https://qn482.infusionsoft.com/app/linkClick/112/75c4a4db9023e2ac/1636470/c1af841dfff22b9a"" class=""bard-text-block style-scope"" target=""_blank"" style=""color: rgb(43, 170, 223); padding: 0px; line-height: 1.6;""><b class=""bard-text-block style-scope"" style=""padding: 0px; line-height: 1.6;"">WhatsApp</b></a></p>
                                 </div></td></tr></tbody></table>
                            </div>
                            <!--[if mso]></td><![endif]-->
                            <!--[if mso]></tr></table><![endif]--></td></tr></tbody></table>
                       </div>
                       <!--[if mso]></td></tr></table><![endif]-->
                       <!--[if mso]><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" align=""center"" style=""width:600px;""><tr><td><![endif]-->
                       <div style=""max-width: 600px; background: rgb(255, 255, 255); margin: 0px auto;"">
                        <table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"" style=""font-size: 0px; width: 100%; background: rgb(255, 255, 255);""><tbody><tr><td style=""text-align: center; vertical-align: top; font-size: 0px;"">
                            <!--[if mso]><table border=""0"" cellpadding=""0"" cellspacing=""0""><tr><![endif]-->
                            <!--[if mso]><td style=""vertical-align:top;width:600px;""><![endif]-->
                            <div class=""column column-100"" style=""display: inline-block; width: 100%; vertical-align: top;"">
                             <table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%""><tbody><tr><td class=""alignmentContainer"" style=""word-wrap: break-word; word-break: break-word; font-size: 0px; padding: 15px; text-align: center;"">
                                 <div style=""margin-left: auto; margin-right: auto;"">
                                  <p style=""font-family: Helvetica, Arial, sans-serif; padding: 0px; margin: 0px 0px 13px; line-height: 1.6; color: rgb(153, 153, 153); font-size: 11px;""><a href=""https://qn482.infusionsoft.com/app/optOut/0/ecd25a0517b16535/1636470/c1af841dfff22b9a"" target=""_blank"" style=""text-decoration: underline; padding: 0px; line-height: 1.6; color: rgb(153, 153, 153);"">Unsubscribe</a></p>
                                  <p style=""font-family: Helvetica, Arial, sans-serif; padding: 0px; margin: 13px 0px 0px; line-height: 1.6; color: rgb(153, 153, 153); font-size: 11px;"">Elainne Ourives EPP Rua Visconde de Pelotas, 905, sala 12, Centro. Caxias do Sul, Rio Grande do Sul 95020182 Brazil (54) 991559604</p>
                                 </div></td></tr></tbody></table>
                            </div>
                            <!--[if mso]></td><![endif]-->
                            <!--[if mso]></tr></table><![endif]--></td></tr></tbody></table>
                       </div>
                       <!--[if mso]></td></tr></table><![endif]-->
                      </div>
                      <img src=""https://is-tracking-pixel-api-prod.appspot.com/api/v1/render/6300184217649152/6134361238798336"" width=""1"" height=""1"" border=""0"" alt="""" />
                    </body></html>";

            webBrowser.LoadHtml(str, "http://example.com/");

            // Part of the Focus hack further described in the OnPropertyChanged() method...
            Keyboard.ClearFocus();
        }

        public void LoadCustomRequestExample()
        {
            var frame = WebBrowser.GetMainFrame();

            //Create a new request knowing we'd like to use PostData
            var request = frame.CreateRequest(initializePostData:true);
            request.Method = "POST";
            request.Url = "custom://cefsharp/PostDataTest.html";
            request.PostData.AddData("test=123&data=456");

            frame.LoadRequest(request);
        }

        internal void ShowDevtools()
        {
            webBrowser.ShowDevTools();
        }
    }
}
