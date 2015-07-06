
## TODO
- two-pass resolving (& implement IEnumerable on IResolve)
- WhatIf
- Generate ShellPloy from metadata? is that even feasible?
- prefer parameter sets with parameter values explicitly set
  over values from context
- return to results, this time without dynamic?

## In Progress
- Error.xxx on a serilog wrapper to log exceptions where they occur

## DONE
1. Include(x => x.Y) on ClientObject resolvers
2. Remove results, leverage Serilog instead
3. Parameter sets (just like powershell's)
4. Parameter validation
5. HarshProvisionerTreeBuilder for ShellPloy to call
