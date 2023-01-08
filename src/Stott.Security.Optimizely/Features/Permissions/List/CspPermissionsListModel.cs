﻿namespace Stott.Security.Optimizely.Features.Permissions.List;

using System.Collections.Generic;

public class CspPermissionsListModel
{
    public List<string>? AllowedDirectives { get; set; }

    public IList<CspPermissionListModel>? Permissions { get; set; }
}