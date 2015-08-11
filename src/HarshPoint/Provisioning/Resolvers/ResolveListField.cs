using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;

namespace HarshPoint.Provisioning.Resolvers
{
    public sealed class ResolveListField : ClientObjectNestedResolveBuilder<Field, List>
    {
        public ResolveListField(IResolveBuilder<List, ClientObjectResolveContext> parent)
            : base(parent, l => l.Fields)                  
        {                                            
        }
    }
}
