﻿namespace Stott.Security.Core.Features.Settings.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Settings.Repository;

public class CspSettingsService : ICspSettingsService
{
    private readonly ICspSettingsRepository _settingsRepository;

    private readonly ICacheWrapper _cacheWrapper;

    public CspSettingsService(
        ICspSettingsRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _settingsRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task<CspSettings> GetAsync()
    {
        return await _settingsRepository.GetAsync();
    }

    public async Task SaveAsync(CspSettingsModel cspSettings)
    {
        if (cspSettings == null) throw new ArgumentNullException(nameof(cspSettings));

        await _settingsRepository.SaveAsync(
            cspSettings.IsEnabled, 
            cspSettings.IsReportOnly, 
            cspSettings.IsWhitelistEnabled, 
            cspSettings.WhitelistAddress);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }
}