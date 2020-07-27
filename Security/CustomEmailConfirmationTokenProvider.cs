using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MMOServer.Security
{
  public class CustomEmailConfirmationTokenProvider<TUser>
     : DataProtectorTokenProvider<TUser> where TUser : class
  {
    private readonly ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger;

    public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                    IOptions<CustomEmailConfirmationTokenProviderOptions> options,
                                    ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
      this.logger = logger;
    }
  }
}
