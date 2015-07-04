
## TODO
- two-pass resolving (& implement IEnumerable on IResolve)
- IAddChild for ShellPloy to call
- WhatIf
- Generate ShellPloy from metadata? is that even feasible?
- prefer parameter sets with parameter values explicitly set
  over values from context

## In Progress
- Parameter validation
- Parameter sets (just like powershell's)
- Error.xxx on a serilog wrapper to log exceptions where they occur

## DONE
1. Include(x => x.Y) on ClientObject resolvers
2. Remove results, leverage Serilog instead
