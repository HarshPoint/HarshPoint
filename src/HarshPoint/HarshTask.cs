using System.Threading.Tasks;

namespace HarshPoint
{
    public static class HarshTask
    {
        public static Task Completed => Task.FromResult(false);
    }
}
