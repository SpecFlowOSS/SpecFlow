namespace FeatureTests.ContextInjection
{
    public class NestedContext
    {
        private readonly SingleContext _context;

        public NestedContext(SingleContext context)
        {
            _context = context;
        }

        public SingleContext TheNestedContext
        {
            get { return _context; }
        }
    }
}