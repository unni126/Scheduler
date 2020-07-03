namespace Scheduler.Core.Data
{
    public static class ContextInitializer
    {
        public static void Initialize(SchedulerContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
