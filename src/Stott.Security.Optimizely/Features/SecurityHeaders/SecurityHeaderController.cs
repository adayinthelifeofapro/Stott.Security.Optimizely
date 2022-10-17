﻿namespace Stott.Security.Optimizely.Features.SecurityHeaders;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class SecurityHeaderController : BaseController
{
    private readonly ISecurityHeaderService _service;

    private readonly ILogger<SecurityHeaderController> _logger;

    public SecurityHeaderController(
        ISecurityHeaderService service,
        ILogger<SecurityHeaderController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Get()
    {
        try
        {
            var data = await _service.GetAsync();

            return CreateSuccessJson(new SecurityHeaderModel
            {
                XContentTypeOptions = data.XContentTypeOptions.ToString(),
                XXssProtection = data.XssProtection.ToString(),
                XFrameOptions = data.FrameOptions.ToString(),
                ReferrerPolicy = data.ReferrerPolicy.ToString(),
                CrossOriginEmbedderPolicy = data.CrossOriginEmbedderPolicy.ToString(),
                CrossOriginOpenerPolicy = data.CrossOriginOpenerPolicy.ToString(),
                CrossOriginResourcePolicy = data.CrossOriginResourcePolicy.ToString(),
                IsStrictTransportSecurityEnabled = data.IsStrictTransportSecurityEnabled,
                IsStrictTransportSecuritySubDomainsEnabled = data.IsStrictTransportSecuritySubDomainsEnabled,
                StrictTransportSecurityMaxAge = data.StrictTransportSecurityMaxAge
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve Security Header settings.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/headers/save")]
    public async Task<IActionResult> SaveHeaders(
        XContentTypeOptions xContentTypeOptions, 
        XssProtection xXssProtection, 
        XFrameOptions xFrameOptions, 
        ReferrerPolicy referrerPolicy)
    {
        try
        {
            await _service.SaveAsync(xContentTypeOptions, xXssProtection, referrerPolicy, xFrameOptions);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save Security Header Settings.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/cross-origin-policies/save")]
    public async Task<IActionResult> SaveCrossOriginPolicies(
        CrossOriginEmbedderPolicy crossOriginEmbedderPolicy,
        CrossOriginOpenerPolicy crossOriginOpenerPolicy,
        CrossOriginResourcePolicy crossOriginResourcePolicy)
    {
        try
        {
            await _service.SaveAsync(crossOriginEmbedderPolicy, crossOriginOpenerPolicy, crossOriginResourcePolicy);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save Security Header Settings.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/strict-transport-security/save")]
    public async Task<IActionResult> SaveStrictTransportSecurityHeaders(
        bool isStrictTransportSecurityEnabled,
        bool isStrictTransportSecuritySubDomainsEnabled,
        int strictTransportSecurityMaxAge)
    {
        try
        {
            await _service.SaveAsync(isStrictTransportSecurityEnabled, isStrictTransportSecuritySubDomainsEnabled, strictTransportSecurityMaxAge);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save Security Header Settings.");
            throw;
        }
    }
}
