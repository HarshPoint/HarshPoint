
## TODO
- two-pass resolving (& implement IEnumerable on IResolve)
  also, allow combining reslvers using Or (only first successful is returned)
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


two pass resolver lifecycle:
============================
* BindDefaultFromContext
* BindResolvers
	* user creates an IAsyncResolver<T> instance
	* HarshProvisioner.PrepareResolveContext calls PrepareResolveContext(ClientObjectResolveContextBuilder ctx)
	* provisioner implementation calls in PrepareResolveContext(ClientObjectResolveContextBuilder ctx) { ctx.Load(Lists, l => l.Title, l => l.Items.Include(i=>i.Id) )  }
		which saves include expressions in a new resolve context
	* provisioner base calls IAsyncResolver<T>.Resolve(IResolveContext<TProvisionerContext>) => IResolver<T>
		-> context contains include expressions
		=> loads queries and properties, saves them with any required transformations
		   in a ClientContextQueryResultsResolver<T> and returns it
	* saves the ClientContextQueryResultsResolver<T> in the parameter property
	* awaits ClientContext.ExecuteQueryAsync()
	* ClientContextQueryResultsResolver<T> : IResolver<T> : IEnumerable<T>

	TODO: merging queries ??? hopefully ClientContext does that

* ResolveParameterSet
* ValidateParameters
* Un/Provision

Resolver Use Cases
==================
* one object that must exist (eg: lookup field target list)
* one or more objects all of which must exist (eg: fields to add to a content type)
* one object that might exist (eg: when provisioning an object, we need to check if it already exists)
* one or more objects that might exist (some of them may fail) ????

Mutable Resolvers
=================
PRO: 