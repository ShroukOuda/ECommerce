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
                body{margin:0;padding:0;background:#eef2ff;
                     font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Arial,sans-serif;
                     font-size:15px;color:#1f2937;line-height:1.7;}
                .outer{width:100%;padding:32px 16px;}
                .wrapper{max-width:640px;margin:0 auto;background:#ffffff;
                          border-radius:24px;overflow:hidden;
                          box-shadow:0 24px 60px rgba(15,23,42,.12);
                          border:1px solid rgba(148,163,184,.18);}
                .hero{background:linear-gradient(135deg,{{_settings.PrimaryColor}} 0%, #0f172a 100%);padding:30px 36px 28px;color:#fff;}
                .brand-row{font-size:12px;letter-spacing:.12em;text-transform:uppercase;opacity:.9;}
                .brand-name{display:block;font-size:24px;line-height:1.1;letter-spacing:-.03em;text-transform:none;margin-top:10px;font-weight:800;}
                .brand-tag{margin-top:10px;display:inline-block;background:rgba(255,255,255,.14);color:#fff;padding:6px 12px;border-radius:999px;font-size:12px;font-weight:600;}
                .hero-copy{margin-top:18px;font-size:15px;max-width:470px;color:rgba(255,255,255,.9);}
                .content{padding:36px 36px 28px;}
                .content h2{margin:0 0 12px;font-size:22px;line-height:1.25;font-weight:700;color:#0f172a;letter-spacing:-.03em;}
                .content p{margin:0 0 16px;}
                .content ul{margin:0 0 18px;padding:0;list-style:none;}
                .content li{margin:0 0 10px;padding-left:28px;position:relative;}
                .content li::before{content:'•';position:absolute;left:8px;top:0;color:{{_settings.PrimaryColor}};font-size:20px;line-height:1;}
                .panel{background:#f8fafc;border:1px solid #e2e8f0;border-radius:18px;padding:18px 20px;margin:22px 0;}
                    .btn{display:inline-block;padding:14px 26px;
                      background:{{_settings.PrimaryColor}};color:#fff;
                      text-decoration:none;border-radius:14px;
                      font-size:14px;font-weight:700;
                      box-shadow:0 10px 18px rgba(26,86,219,.22);}
                .btn:hover{opacity:.95;}
                .divider{border:none;border-top:1px solid #e2e8f0;margin:24px 0;}
                .note{font-size:13px;color:#64748b;}
                .link{word-break:break-all;color:{{_settings.PrimaryColor}};font-size:12px;}
                .footer{background:#f8fafc;padding:20px 36px;text-align:center;
                         font-size:12px;color:#64748b;border-top:1px solid #e2e8f0;}
                .footer strong{color:#0f172a;}
                @media(max-width:640px){
                  .outer{padding:0;}
                  .wrapper{border-radius:0;border-left:none;border-right:none;}
                  .hero,.content,.footer{padding-left:20px;padding-right:20px;}
                  .hero{padding-top:24px;padding-bottom:22px;}
                  .content{padding-top:28px;padding-bottom:20px;}
                }
              </style>
            </head>
            <body>
              <!-- Preheader (hidden preview text in inbox) -->
              <span style="display:none;max-height:0;overflow:hidden;opacity:0;color:transparent;">{{safePreheader}}</span>

              <div class="outer">
                <div class="wrapper">
                  <div class="hero">
                    <div class="brand-row">Marketplace for vendors and customers</div>
                    <span class="brand-name">{{WebUtility.HtmlEncode(_settings.LogoText)}}</span>
                    <div class="brand-tag">Smart selling. Better buying.</div>
                    <div class="hero-copy">A polished marketplace experience for product discovery, seller management, and customer orders.</div>
                  </div>
                  <div class="content">
                    <div class="panel">
                      {{bodyHtml}}
                    </div>
                  </div>
                  <div class="footer">
                    <strong>{{WebUtility.HtmlEncode(_settings.AppName)}}</strong><br/>
                    &copy; {{year}} All rights reserved.
                  </div>
                </div>
              </div>
            </body>
            </html>
            """;
    }

   
    public string Button(string label, string url) =>
    $"""
    <table role="presentation" cellspacing="0" cellpadding="0">
        <tr>
            <td bgcolor="{_settings.PrimaryColor}"
                style="border-radius:14px;">
                <a href="{WebUtility.HtmlEncode(url)}"
                   style="
                       display:inline-block;
                       padding:14px 26px;
                       color:#ffffff;
                       text-decoration:none;
                       font-weight:700;
                       font-size:14px;
                   ">
                    {WebUtility.HtmlEncode(label)}
                </a>
            </td>
        </tr>
    </table>
    """;

    public string Divider() => """<hr class="divider"/>""";

    public string Note(string text) =>
        $"""<p class="note">{WebUtility.HtmlEncode(text)}</p>""";

    public string FallbackLink(string url) =>
        $"""
        <p class="note">If the button doesn't work, paste this URL into your browser:</p>
        <a href="{WebUtility.HtmlEncode(url)}" class="link">{WebUtility.HtmlEncode(url)}</a>
        """;
}