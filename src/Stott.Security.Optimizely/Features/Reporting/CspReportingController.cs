﻿namespace Stott.Security.Optimizely.Features.Reporting;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Reporting.Repository;
using Stott.Security.Optimizely.Features.Whitelist;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class CspReportingController : BaseController
{
    private readonly ICspViolationReportRepository _repository;

    private readonly IWhitelistService _whitelistService;

    private readonly ILogger<CspReportingController> _logger;

    public CspReportingController(
        ICspViolationReportRepository repository,
        IWhitelistService whitelistService,
        ILogger<CspReportingController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));

        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Report([FromBody] ReportModel cspReport)
    {
        try
        {
            await _repository.SaveAsync(cspReport);

            var isOnWhitelist = await _whitelistService.IsOnWhitelistAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
            if (isOnWhitelist)
            {
                await _whitelistService.AddFromWhiteListToCspAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP Report.");
            throw;
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> ReportSummary()
    {
        try
        {
            var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
            var model = await _repository.GetReportAsync(reportDate);

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP Report.");
            throw;
        }
    }
}
