using AutoMapper;

namespace Demo1.Backend.Infrastructure
{
    public static class AutomapperConfiguration
    {
        public static IMapper Init()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();
            });
            return new Mapper(configuration);
        }
    }
}