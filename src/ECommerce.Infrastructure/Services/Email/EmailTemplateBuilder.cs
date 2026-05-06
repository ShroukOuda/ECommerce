using System.Net;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services.Email;
  


public sealed class EmailTemplateBuilder
{
    private readonly EmailTemplateSettings _settings;

    public EmailTemplateBuilder(IOptions<EmailTemplateSettings> options)
        => _settings = options.Value;


    public string Wrap(string preheader, string bodyHtml)
    {
        var safePreheader = WebUtility.HtmlEncode(preheader);
        var year = DateTime.UtcNow.Year;

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="UTF-8"/>
              <meta name="viewport" content="width=device-width,initial-scale=1"/>
              <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
              <title>{{WebUtility.HtmlEncode(_settings.AppName)}}</title>
              <!--[if mso]><noscript><xml><o:OfficeDocumentSettings>
              <o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml></noscript><![endif]-->
              <style>
                *,*::before,*::after{box-sizing:border-box;}
                body{margin:0;padding:0;background:#f5f5f5;
                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Arial,sans-serif;
                     font-size:15px;color:#374151;line-height:1.6;}
                .wrapper{max-width:600px;margin:32px auto;background:#fff;
                          border-radius:12px;overflow:hidden;
                          box-shadow:0 2px 8px rgba(0,0,0,.08);}
                .header{background:{{_settings.PrimaryColor}};padding:28px 40px;text-align:center;}
                .header h1{margin:0;color:#fff;font-size:22px;font-weight:500;
                            letter-spacing:-.3px;}
                .body{padding:36px 40px;}
                .body h2{margin:0 0 12px;font-size:18px;font-weight:500;color:#111827;}
                .body p{margin:0 0 16px;}
                .body ul{padding-left:20px;line-height:1.8;}
                .btn{display:inline-block;padding:13px 28px;
                      background:{{_settings.PrimaryColor}};color:#fff;
                      text-decoration:none;border-radius:8px;
                      font-size:15px;font-weight:500;}
                .btn:hover{opacity:.9;}
                .divider{border:none;border-top:1px solid #f3f4f6;margin:24px 0;}
                .note{font-size:13px;color:#9ca3af;}
                .link{word-break:break-all;color:{{_settings.PrimaryColor}};font-size:12px;}
                .footer{background:#f9fafb;padding:20px 40px;text-align:center;
                         font-size:12px;color:#9ca3af;}
                @media(max-width:640px){
                  .wrapper{margin:0;border-radius:0;}
                  .body,.footer{padding:24px 20px;}
                  .header{padding:20px;}
                }
              </style>
            </head>
            <body>
              <!-- Preheader (hidden preview text in inbox) -->
              <span style="display:none;max-height:0;overflow:hidden;">{{safePreheader}}</span>

              <div class="wrapper">
                <div class="header">
                  <h1>{{WebUtility.HtmlEncode(_settings.LogoText)}}</h1>
                </div>
                <div class="body">
                  {{bodyHtml}}
                </div>
                <div class="footer">
                  &copy; {{year}} {{WebUtility.HtmlEncode(_settings.AppName)}}
                  &nbsp;&middot;&nbsp;
                  You are receiving this email because you have an account with us.
                </div>
              </div>
            </body>
            </html>
            """;
    }

   
    public string Button(string label, string url) =>
        $"""<p><a href="{WebUtility.HtmlEncode(url)}" class="btn">{WebUtility.HtmlEncode(label)}</a></p>""";

    public string Divider() => """<hr class="divider"/>""";

    public string Note(string text) =>
        $"""<p class="note">{WebUtility.HtmlEncode(text)}</p>""";

    public string FallbackLink(string url) =>
        $"""
        <p class="note">If the button doesn't work, paste this URL into your browser:</p>
        <a href="{WebUtility.HtmlEncode(url)}" class="link">{WebUtility.HtmlEncode(url)}</a>
        """;
}