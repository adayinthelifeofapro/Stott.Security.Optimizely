﻿namespace Stott.Security.Optimizely.Features.AllowList;

using System.Threading.Tasks;

public interface IAllowListService
{
    Task AddFromAllowListToCspAsync(string? violationSource, string? violationDirective);

    Task<bool> IsOnAllowListAsync(string? violationSource, string? violationDirective);

    Task<bool> IsAllowListValidAsync(string? allowListUrl);
}