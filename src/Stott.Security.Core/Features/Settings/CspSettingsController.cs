﻿namespace Stott.Security.Core.Features.Settings;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;
using Stott.Security.Core.Common.Validation;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Settings.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class CspSettingsController : BaseController
{
    private readonly ICspSettingsService _settings;

    private readonly ILoggingProvider _logger;

    public CspSettingsController(
        ICspSettingsService service,
        ILoggingProviderFactory loggingProviderFactory)
    {
        _settings = service;
        _logger = loggingProviderFactory.GetLogger(typeof(CspSettingsController));
    }

    [HttpGet]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var data = await _settings.GetAsync();

            return CreateSuccessJson(new CspSettingsModel
            {
                IsEnabled = data.IsEnabled,
                IsReportOnly = data.IsReportOnly,
                IsWhitelistEnabled = data.IsWhitelistEnabled,
                WhitelistAddress = data.WhitelistUrl
            });
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Failed to retrieve CSP settings.", exception);
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Save(CspSettingsModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _settings.SaveAsync(model);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Failed to save CSP settings.", exception);
            throw;
        }
    }
}
