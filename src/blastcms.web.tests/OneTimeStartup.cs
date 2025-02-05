using System;
using AutoMapper;
using blastcms.web.Factories;
using blastcms.web.Registry;
using Finbuckle.MultiTenant;
using Marten;
using Moq;
using NUnit.Framework;
using ThrowawayDb.Postgres;
using Weasel.Core;

namespace blastcms.web.tests
{
    [SetUpFixture]
    public  class Tests
    {
        public static DocumentStore Store;
        public static ThrowawayDatabase Database;
        public static ISessionFactory SessionFactory;
        public static IMapper  Mapper;

        [OneTimeSetUp]
        public  void Setup()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddMaps(typeof(Startup)));
            Mapper = new Mapper(configuration);

            var testHost = Environment.GetEnvironmentVariable("DB_HOST")?? "localhost";

            Database = ThrowawayDatabase.Create(username: "blastcms_user", password: "not_magical_scary", host: testHost);

            Store = DocumentStore.For( _ =>
            {
                // Turn this off in production
                _.AutoCreateSchemaObjects = AutoCreate.All;

                // This is still mandatory
                _.Connection(Database.ConnectionString);

               _.Schema.Include<BlastcmsMartenRegistry>();
            });

            var multitenantContext = new Mock<IMultiTenantContext<TenantInfo>>();
            multitenantContext.Setup(p => p.TenantInfo).Returns(new TenantInfo { Id = "test-tenant-1", Name = "Test Tenant 1", Identifier = "test_tenant_1" });

            var multitenantContextAccessor = new Mock<IMultiTenantContextAccessor<TenantInfo>>();
            multitenantContextAccessor.Setup(p => p.MultiTenantContext).Returns(multitenantContext.Object);

            SessionFactory = new CustomSessionFactory(Store, multitenantContextAccessor.Object);

            Store.InitializeDatabase();

        }

        [OneTimeTearDown]
        public  void TearDown()
        {
            Store.Dispose();
            Database.Dispose();
        }
    }
}